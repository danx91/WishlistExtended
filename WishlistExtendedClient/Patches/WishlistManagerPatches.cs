using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Collections.Generic;
using System.Reflection;
using WishlistExtended.Features;

namespace WishlistExtended.Patches
{
    internal class WishlistManagerIsInWishlistPatch : ModulePatch
    {
        public static Item? CurrentItem;

        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(WishlistManager), nameof(WishlistManager.IsInWishlist));
        }

        [PatchPrefix]
        private static bool Prefix(WishlistManager __instance, MongoID templateId, bool includeQol, out EWishlistGroup group, ref bool __result)
        {
            if (__instance._userItems.TryGetValue(templateId, out group))
            {
                __result = true;
                return false;
            }

            group = EWishlistGroup.Hideout;
            __result = includeQol && WishlistResolver.IsInWishlist(templateId, out group, CurrentItem);

            return false;
        }
    }

    internal class WishlistManagerGetWishlistPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(WishlistManager), nameof(WishlistManager.GetWishlist));
        }

        [PatchPrefix]
        private static bool Prefix(WishlistManager __instance, ref IReadOnlyDictionary<MongoID, EWishlistGroup> __result)
        {
            var result = new Dictionary<MongoID, EWishlistGroup>();

            foreach (var (itemId, group) in WishlistResolver.GetWishlist())
            {
                result[itemId] = group;
            }

            foreach (var (itemId, group) in __instance._userItems)
            {
                result[itemId] = group;
            }

            __result = result;
            return false;
        }
    }
}
