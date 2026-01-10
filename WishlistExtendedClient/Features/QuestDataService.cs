using EFT;
using EFT.Quests;
using SPT.Reflection.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using WishlistExtended.Models;

namespace WishlistExtended.Features
{
    internal class QuestDataService
    {
        private static bool _dirty = false;
        private static readonly Dictionary<MongoID, List<WishlistQuestData>> _cachedQuests = [];
        private static readonly Dictionary<MongoID, ItemsCount> _totalNeeded = [];

        public static Dictionary<MongoID, List<WishlistQuestData>> Quests
        {
            get
            {
                if (_dirty)
                {
                    _dirty = false;
                    Stopwatch sw = Stopwatch.StartNew();
                    BuildQuests();
                    Plugin.LogDebug($"Quests build took {sw.Elapsed.TotalMilliseconds} ms");
                }

                return _cachedQuests;
            }
        }

        public static Dictionary<MongoID, ItemsCount> TotalNeeded
        {
            get
            {
                if (_dirty)
                {
                    _dirty = false;
                    Stopwatch sw = Stopwatch.StartNew();
                    BuildQuests();
                    Plugin.LogDebug($"Quests build took {sw.Elapsed.TotalMilliseconds} ms");
                }

                return _totalNeeded;
            }
        }

        public static void InvalidateCache() => _dirty = true;

        private static void BuildQuests()
        {
            _cachedQuests.Clear();

            Plugin.LogDebug("Building quests");

            Profile profile = ClientAppUtils.GetClientApp().GetClientBackEndSession().Profile;

            foreach (QuestDataClass quest in profile.QuestsData)
            {
                if (quest.Template is null || quest.Status != EQuestStatus.Started)
                {
                    Plugin.LogDebug($"Quest ${quest.Id} ({quest.Template?.Id}) is missing template or status != started");
                    continue;
                }

                Plugin.LogDebug($"Processing quest ${quest.Id} ({quest.Template.Id}):");

                if (!quest.Template.Conditions.TryGetValue(EQuestStatus.AvailableForFinish, out var conditions))
                {
                    Plugin.LogDebug("\tNo AvailableForFinish conditions found - skip");
                    continue;
                }

                foreach (Condition condition in conditions)
                {
                    Plugin.LogDebug($"\tProcessing condition ${condition.id}:");

                    if (quest.CompletedConditions.Contains(condition.id))
                    {
                        Plugin.LogDebug("\t\tCondition completed - skip");
                        continue;
                    }

                    if (condition is not ConditionHandoverItem conditionHandover)
                    {
                        Plugin.LogDebug("\t\tNot handover condition - skip");
                        continue;
                    }

                    foreach (string target in conditionHandover.target)
                    {
                        MongoID itemId = new(target);
                        int quantity = (int)conditionHandover.value;

                        if (profile.TaskConditionCounters.TryGetValue(condition.id, out var counter))
                        {
                            quantity -= counter.Value;
                        }

                        if (!_cachedQuests.TryGetValue(itemId, out var questItems))
                        {
                            questItems = [];
                            _cachedQuests.Add(itemId, questItems);
                        }

                        if (!_totalNeeded.TryGetValue(itemId, out var totalCount))
                        {
                            totalCount = new();
                            _totalNeeded.Add(itemId, totalCount);
                        }

                        totalCount.Total = quantity;

                        if (conditionHandover.onlyFoundInRaid)
                        {
                            totalCount.Fir = quantity;
                        }

                        questItems.Add(new(
                            quest.Template.NameLocaleKey,
                            quest.Template.TraderId,
                            quantity,
                            conditionHandover.onlyFoundInRaid,
                            false
                        ));

                        Plugin.LogDebug($"\t\tQuest added: {itemId} ({quantity}, fir: {conditionHandover.onlyFoundInRaid}) -> {quest.Id}");
                    }
                }
            }

            Plugin.LogSource?.LogInfo($"Loaded quests - {_cachedQuests.Count} unique items");

            foreach (var quests in _cachedQuests.Values)
            {
                quests.Sort((a, b) =>
                {
                    return a.TraderId.CompareTo(b.TraderId);
                });
            }
        }
    }
}
