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
            return AccessTools.Method(typeof(GClass2067), nameof(GClass2067.IsInWishlist));
        }

        [PatchPrefix]
        private static bool Prefix(GClass2067 __instance, MongoID templateId, bool includeQol, out EWishlistGroup group, ref bool __result)
        {
            if (__instance.Dictionary_0.TryGetValue(templateId, out group))
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
            return AccessTools.Method(typeof(GClass2067), nameof(GClass2067.GetWishlist));
        }

        [PatchPrefix]
        private static bool Prefix(GClass2067 __instance, ref IReadOnlyDictionary<MongoID, EWishlistGroup> __result)
        {
            var result = new Dictionary<MongoID, EWishlistGroup>();

            foreach (var (itemId, group) in WishlistResolver.GetWishlist())
            {
                result[itemId] = group;
            }

            foreach (var (itemId, group) in __instance.Dictionary_0)
            {
                result[itemId] = group;
            }

            __result = result;
            return false;
        }
    }
}
