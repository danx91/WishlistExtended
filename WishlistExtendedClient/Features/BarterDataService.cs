using Comfort.Common;
using EFT;
using EFT.InventoryLogic;
using Newtonsoft.Json;
using SPT.Common.Http;
using System;
using System.Collections.Generic;
using WishlistExtended.Models;
using ZGFueDkx.ZGCLib.Helpers;

namespace WishlistExtended.Features
{
    internal static class BarterDataService
    {
        private static readonly Dictionary<MongoID, List<WishlistBarterData>> _cachedBarters = [];
        public static Dictionary<MongoID, List<WishlistBarterData>> Barters => _cachedBarters;

        public static void LoadBarters()
        {
            Plugin.LogDebug("Processing barters");

            _cachedBarters.Clear();

            if (!Singleton<ItemFactoryClass>.Instantiated)
            {
                Plugin.LogSource?.LogError("BarterDataService.LoadBarters: Singleton ItemFactoryClass is not instantiated");
                return;
            }
            string? response = null;
            try
            {
                response = RequestHandler.GetJson("/wishlist-extended/barters");
            }
            catch (Exception ex)
            {
                Plugin.LogSource?.LogError("Error loading barters");
                Plugin.LogSource?.LogError(ex.ToString());
                return;
            }

            var barterData = JsonConvert.DeserializeObject<Dictionary<MongoID, List<BarterData>>>(
                response,
                JsonSettingsFactory.GetJsonSerializerSettings(Plugin.LogSource)
            );

            Plugin.LogDebug(response);

            if (barterData is null)
            {
                Plugin.LogSource?.LogError("Failed to parse _questData!");
                Plugin.LogSource?.LogError(response);
                return;
            }

            Dictionary<MongoID, Item> items = [];
            ItemFactoryClass itemFactory = Singleton<ItemFactoryClass>.Instance;

            foreach (var (traderId, barterList) in barterData)
            {
                Plugin.LogDebug($"\tProcessing trader: {traderId}");

                foreach (var barter in barterList)
                {
                    if (barter.ItemId is not MongoID itemId || barter.ResultId is not MongoID resultId)
                    {
                        Plugin.LogSource?.LogError($"\t\tTrader {traderId} barter has missing values");
                        continue;
                    }

                    if (!items.TryGetValue(itemId, out Item item))
                    {
                        item = itemFactory.CreateItem(MongoID.Generate(true), itemId, null);
                        items.Add(itemId, item);
                    }

                    if (!GClass2067.IsAvailableForWishlist(item))
                    {
                        Plugin.LogDebug($"\t\tItem {itemId} is not suitable for wishlist");
                        continue;
                    }

                    if (!_cachedBarters.TryGetValue(itemId, out var itemBarters))
                    {
                        itemBarters = [];
                        _cachedBarters.Add(itemId, itemBarters);
                    }

                    itemBarters.Add(new(
                        resultId,
                        traderId,
                        barter.Quantity ?? 1,
                        barter.LoyaltyLevel ?? 1
                    ));

                    Plugin.LogDebug($"\t\tBarter added: {itemId} ({barter.Quantity}) -> {resultId} (LL: {barter.LoyaltyLevel})");
                }
            }

            Plugin.LogSource?.LogInfo($"Loaded barters - {_cachedBarters.Count} unique items");

            foreach (var itemBarters in _cachedBarters.Values)
            {
                itemBarters.Sort((a, b) =>
                {
                    int result = a.TraderId.CompareTo(b.TraderId);
                    if (result != 0)
                    {
                        return result;
                    }

                    return a.LoyaltyLevel.CompareTo(b.LoyaltyLevel);
                });
            }
        }
    }
}
