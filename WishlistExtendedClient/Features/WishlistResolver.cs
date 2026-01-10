using EFT;
using EFT.InventoryLogic;
using System.Collections.Generic;
using WishlistExtended.Config;
using WishlistExtended.Models;
using ZGFueDkx.ZGCLib.helpers;

namespace WishlistExtended.Features
{
    internal static class WishlistResolver
    {
        public static bool IsInWishlist(MongoID templateId, out EWishlistGroup group, Item? item = null)
        {
            if (ResolveWhishlist(false, item).TryGetValue(templateId, out WishlistData data))
            {
                group = GetWishlistGroup(data);
                return true;
            }

            group = EWishlistGroup.Hideout;
            return false;
        }

        public static Dictionary<MongoID, EWishlistGroup> GetWishlist()
        {
            Dictionary<MongoID, EWishlistGroup> wishlist = [];

            foreach (var (itemId, data) in ResolveWhishlist())
            {
                wishlist[itemId] = GetWishlistGroup(data);
            }

            return wishlist;
        }

        public static Dictionary<MongoID, WishlistData> ResolveWhishlist(bool ignoreEnough = false, Item? item = null)
        {
            if (Settings.RaidOnly!.Value && !RaidUtils.IsInRaid())
            {
                return [];
            }

            bool excludeFir = item is not null && !item.MarkedAsSpawnedInSession;
            Dictionary<MongoID, WishlistData> wishlist = [];

            ResolveCrafts(wishlist);
            ResolveHideout(wishlist, !ignoreEnough, excludeFir);
            ResolveBarters(wishlist);
            ResolveQuests(wishlist, !ignoreEnough, excludeFir);

            return wishlist;
        }

        public static void ResolveCrafts(Dictionary<MongoID, WishlistData> wishlist)
        {
            if (Settings.CraftMode!.Value == Settings.ECraftMode.Disabled)
            {
                return;
            }

            foreach (var (itemId, craft) in CraftDataService.Crafts)
            {
                if (!wishlist.TryGetValue(itemId, out var wishlistData))
                {
                    wishlistData = new();
                    wishlist[itemId] = wishlistData;
                }

                wishlistData.Crafts.AddRange(craft);
            }
        }

        public static void ResolveHideout(Dictionary<MongoID, WishlistData> wishlist, bool hideEnough, bool excludeFir)
        {
            if (Settings.HideoutMode!.Value == Settings.EHideoutMode.Disabled)
            {
                return;
            }

            foreach (var (itemId, hideout) in HideoutDataService.HideoutRequirements)
            {
                if (hideEnough && Settings.RemoveIfEnough!.Value)
                {
                    ItemsCount inStash = StashService.GetItemCount(itemId);
                    ItemsCount needed = HideoutDataService.TotalNeeded.TryGetValue(itemId, out var total) ? total : new();

                    if (inStash.Total >= needed.Total && inStash.Fir >= needed.Fir)
                    {
                        continue;
                    }
                }

                foreach (WishlistHideoutData item in hideout)
                {
                    if (excludeFir && item.Fir)
                    {
                        continue;
                    }

                    if (!wishlist.TryGetValue(itemId, out var wishlistData))
                    {
                        wishlistData = new();
                        wishlist[itemId] = wishlistData;
                    }

                    wishlistData.Hideout.Add(item);
                }
            }
        }

        public static void ResolveBarters(Dictionary<MongoID, WishlistData> wishlist)
        {
            if (!Settings.IncludeBarters!.Value)
            {
                return;
            }

            foreach (var (itemId, barters) in BarterDataService.Barters)
            {
                if (!wishlist.TryGetValue(itemId, out var wishlistData))
                {
                    wishlistData = new();
                    wishlist[itemId] = wishlistData;
                }

                wishlistData.Barters.AddRange(barters);
            }
        }

        public static void ResolveQuests(Dictionary<MongoID, WishlistData> wishlist, bool hideEnough, bool excludeFir)
        {
            if (!Settings.IncludeQuests!.Value)
            {
                return;
            }

            foreach (var (itemId, quests) in QuestDataService.Quests)
            {
                if (hideEnough && Settings.RemoveIfEnough!.Value)
                {
                    ItemsCount inStash = StashService.GetItemCount(itemId);
                    ItemsCount needed = QuestDataService.TotalNeeded.TryGetValue(itemId, out var total) ? total : new();

                    if (inStash.Total >= needed.Total && inStash.Fir >= needed.Fir)
                    {
                        continue;
                    }
                }

                foreach (WishlistQuestData item in quests)
                {
                    if (excludeFir && item.Fir)
                    {
                        continue;
                    }

                    if (!wishlist.TryGetValue(itemId, out var wishlistData))
                    {
                        wishlistData = new();
                        wishlist[itemId] = wishlistData;
                    }

                    wishlistData.Quests.Add(item);
                }
            }
        }

        private static EWishlistGroup GetWishlistGroup(WishlistData data)
        {
            EWishlistGroup group = EWishlistGroup.Hideout;
            int priority = int.MinValue;

            if (data.HasHideout && Settings.HideoutPriority!.Value > priority)
            {
                group = EWishlistGroup.Hideout;
                priority = Settings.HideoutPriority!.Value;
            }

            if (data.HasCrafts && Settings.CraftsPriority!.Value > priority)
            {
                group = EWishlistGroup.Equipment;
                priority = Settings.HideoutPriority!.Value;
            }

            if (data.HasBarters && Settings.BartersPriority!.Value > priority)
            {
                group = EWishlistGroup.Trading;
                priority = Settings.BartersPriority!.Value;
            }

            if (data.HasQuests && Settings.QuestsPriority!.Value > priority)
            {
                group = EWishlistGroup.Quests;
            }

            return group;
        }
    }
}
