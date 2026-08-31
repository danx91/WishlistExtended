using Comfort.Common;
using EFT;
using EFT.Hideout;
using System.Collections.Generic;
using System.Diagnostics;
using WishlistExtended.Config;
using WishlistExtended.Models;

namespace WishlistExtended.Features
{
    internal static class HideoutDataService
    {
        private static bool _dirty = false;
        private static readonly Dictionary<MongoID, List<WishlistHideoutData>> _cachedRequirements = [];
        private static readonly Dictionary<MongoID, ItemsCount> _totalNeeded = [];

        public static Dictionary<MongoID, List<WishlistHideoutData>> HideoutRequirements
        {
            get
            {
                if (_dirty)
                {
                    _dirty = false;
                    Stopwatch sw = Stopwatch.StartNew();
                    BuildHideoutRequirements();
                    Plugin.LogDebug($"Hideout build took {sw.Elapsed.TotalMilliseconds} ms");
                }

                return _cachedRequirements;
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
                    BuildHideoutRequirements();
                    Plugin.LogDebug($"Hideout build took {sw.Elapsed.TotalMilliseconds} ms");
                }

                return _totalNeeded;
            }
        }

        public static void InvalidateCache() => _dirty = true;

        private static void BuildHideoutRequirements()
        {
            _cachedRequirements.Clear();
            _totalNeeded.Clear();

            if (!Singleton<HideoutRepresentation>.Instantiated)
            {
                Plugin.LogSource?.LogError("HideoutDataService.BuildHideoutRequirements: Singleton HideoutClass is not instantiated");
            }

            Plugin.LogDebug("Building hideout requirements");

            List<AreaData> hideoutAreas = Singleton<HideoutRepresentation>.Instance.AreaDatas;

            if (hideoutAreas.Count == 0)
            {
                Plugin.LogDebug("Trying to build hideout requirements too early");
                _dirty = true;
                return;
            }

            var hideoutMode = Settings.HideoutMode!.Value;
            bool hideFuture = hideoutMode != Settings.EHideoutMode.All;
            bool lockedAsFuture = Settings.ShowLockedAsFuture!.Value;

            foreach (AreaData area in hideoutAreas)
            {
                Plugin.LogDebug($"Processing area: {area.Template.Type} ({area.Template.Id}), status: {area.Status}, enabled: {area.Enabled}");

                if (!area.Enabled)
                {
                    continue;
                }

                Stage stage = area.CurrentStage;
                while ((stage = area.StageAt(stage.Level + 1)).Level > 0)
                {
                    Plugin.LogDebug($"\tProcessing stage: {stage.Level}");

                    bool isCurrentStage = stage.Level == area.CurrentLevel + 1;

                    if (isCurrentStage &&
                        (area.Status == EAreaStatus.Constructing ||
                            area.Status == EAreaStatus.Upgrading ||
                            area.Status == EAreaStatus.ReadyToInstallConstruct ||
                            area.Status == EAreaStatus.ReadyToInstallUpgrade))
                    {
                        Plugin.LogDebug($"\t\tUpgrade in progress - skip");
                        continue;
                    }

                    bool isLocked = false;
                    if (lockedAsFuture)
                    {
                        foreach (Requirement req in stage.Requirements)
                        {
                            if (req is not ItemRequirement && !req.Fulfilled)
                            {
                                Plugin.LogDebug($"\t\tRequirements not fulfilled {req.Type} - break area");
                                isLocked = true;
                                break;
                            }
                        }

                        if (isLocked && hideFuture)
                        {
                            break;
                        }
                    }

                    foreach (Requirement req in stage.Requirements)
                    {
                        Plugin.LogDebug($"\t\tProcessing requirement: {req.Type}");

                        if (req is not ItemRequirement itemReq)
                        {
                            Plugin.LogDebug($"\t\t\tNot item requirement: {req.Type} - skip");
                            continue;
                        }

                        if (!WishlistManager.IsAvailableForWishlist(itemReq.Item))
                        {
                            Plugin.LogDebug($"\t\t\tItem {itemReq.TemplateId} is not suitable for wishlist");
                            continue;
                        }

                        if (!_cachedRequirements.TryGetValue(itemReq.TemplateId, out var itemUpgrades))
                        {
                            itemUpgrades = [];
                            _cachedRequirements.Add(itemReq.TemplateId, itemUpgrades);
                        }

                        if (!_totalNeeded.TryGetValue(itemReq.TemplateId, out var totalCount))
                        {
                            totalCount = new();
                            _totalNeeded.Add(itemReq.TemplateId, totalCount);
                        }

                        totalCount.Total += itemReq.IntCount;

                        if (itemReq.IsSpawnedInSession)
                        {
                            totalCount.Fir += itemReq.IntCount;
                        }

                        itemUpgrades.Add(new(
                            area.Template.Type,
                            stage.Level,
                            itemReq.IntCount,
                            itemReq.IsSpawnedInSession,
                            isLocked || !isCurrentStage
                        ));

                        Plugin.LogDebug($"\t\t\tRequirement added: " +
                            $"{itemReq.TemplateId} ({itemReq.IntCount}) -> {area.Template.Type} ({stage.Level}, FiR: {itemReq.IsSpawnedInSession}, Future: {!isCurrentStage}, Locked: {isLocked})");
                    }

                    if (hideFuture)
                    {
                        Plugin.LogDebug($"\t\tNo future - break");
                        break;
                    }
                }
            }

            Plugin.LogSource?.LogInfo($"Loaded hideout requirements - {_cachedRequirements.Count} unique items");

            foreach (var upgrades in _cachedRequirements.Values)
            {
                upgrades.Sort((a, b) =>
                {
                    int result = a.AreaType.CompareTo(b.AreaType);
                    if (result != 0)
                    {
                        return result;
                    }

                    return a.Stage.CompareTo(b.Stage);
                });
            }
        }
    }
}
