using EFT;
using EFT.InventoryLogic;
using SPT.Reflection.Utils;
using System.Collections.Generic;
using System.Diagnostics;
using WishlistExtended.Config;
using WishlistExtended.Models;
using ZGFueDkx.ZGCLib.helpers;

namespace WishlistExtended.Features
{
    internal static class StashService
    {
        private static bool _dirty = false;
        private static readonly Dictionary<MongoID, ItemsCount> _stashCache = [];
        private static readonly Dictionary<MongoID, ItemsCount> _equipmentCache = [];

        public static void InvalidateCache() => _dirty = true;

        public static void BuildCache()
        {
            bool inRaid = RaidUtils.IsInRaid();

            if (!inRaid)
            {
                BuildStashCache();
            }

            if (!inRaid || Settings.ShowRaidCount!.Value)
            {
                BuildEquipmentCache();
            }
            else
            {
                _equipmentCache.Clear();
            }
        }

        public static ItemsCount GetItemCount(MongoID templateId)
        {
            if (_dirty)
            {
                Stopwatch sw = Stopwatch.StartNew();
                _dirty = false;
                BuildCache();
                Plugin.LogDebug($"Items cache build took {sw.Elapsed.TotalMilliseconds} ms");
            }

            ItemsCount result = new();

            if (_stashCache.TryGetValue(templateId, out ItemsCount stashCount))
            {
                result.Total += stashCount.Total;
                result.Fir += stashCount.Fir;
            }

            if (_equipmentCache.TryGetValue(templateId, out ItemsCount equipmentCount))
            {
                result.Total += equipmentCount.Total;
                result.Fir += equipmentCount.Fir;
            }

            return result;
        }

        private static void BuildStashCache()
        {
            _stashCache.Clear();

            Profile profile = ClientAppUtils.GetClientApp().GetClientBackEndSession().Profile;
            IEnumerable<Item> itemsToCache = profile.Inventory.GetPlayerItems(EPlayerItems.Stash |
                EPlayerItems.HideoutStashes |
                EPlayerItems.SortingTable);

            foreach (Item item in itemsToCache)
            {
                if (!_stashCache.TryGetValue(item.TemplateId, out ItemsCount itemsCount))
                {
                    itemsCount = new();
                    _stashCache.Add(item.TemplateId, itemsCount);
                }

                itemsCount.Total += item.StackObjectsCount;

                if (item.MarkedAsSpawnedInSession)
                {
                    itemsCount.Fir += item.StackObjectsCount;
                }
            }

            Plugin.LogSource?.LogInfo($"Stash cache built - total items in cache: {_stashCache.Count}");
        }

        private static void BuildEquipmentCache()
        {
            _equipmentCache.Clear();

            Profile profile = ClientAppUtils.GetClientApp().GetClientBackEndSession().Profile;
            IEnumerable<Item> itemsToCache = profile.Inventory.GetPlayerItems(EPlayerItems.Equipment);

            foreach (Item item in itemsToCache)
            {
                if (!_equipmentCache.TryGetValue(item.TemplateId, out ItemsCount itemsCount))
                {
                    itemsCount = new();
                    _equipmentCache.Add(item.TemplateId, itemsCount);
                }

                itemsCount.Total += item.StackObjectsCount;

                if (item.MarkedAsSpawnedInSession)
                {
                    itemsCount.Fir += item.StackObjectsCount;
                }
            }

            Plugin.LogSource?.LogInfo($"Equipment cache built - total items in cache: {_equipmentCache.Count}");
        }
    }
}
