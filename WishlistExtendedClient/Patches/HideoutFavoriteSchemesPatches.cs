using EFT.Hideout;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using WishlistExtended.Features;

namespace WishlistExtended.Patches
{
    internal class HideoutFavoriteAddPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(HideoutRepresentation), nameof(HideoutRepresentation.AddFavoriteScheme));
        }

        [PatchPostfix]
        private static void Postfix()
        {
            CraftDataService.InvalidateCache();
        }
    }

    internal class HideoutFavoriteRemovePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(HideoutRepresentation), nameof(HideoutRepresentation.RemoveFavoriteScheme));
        }

        [PatchPostfix]
        private static void Postfix()
        {
            CraftDataService.InvalidateCache();
        }
    }
}
