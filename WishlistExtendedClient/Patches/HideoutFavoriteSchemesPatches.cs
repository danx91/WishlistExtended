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
            return AccessTools.Method(typeof(HideoutClass), nameof(HideoutClass.method_38));
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
            return AccessTools.Method(typeof(HideoutClass), nameof(HideoutClass.method_39));
        }

        [PatchPostfix]
        private static void Postfix()
        {
            CraftDataService.InvalidateCache();
        }
    }
}
