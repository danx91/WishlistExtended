using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using WishlistExtended.Features;

namespace WishlistExtended.Patches
{
    internal class TraderControllerPatches : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(TraderControllerClass), nameof(TraderControllerClass.method_8));
        }

        [PatchPostfix]
        private static void Postfix()
        {
            TooltipManager.OnItemExit();
            StashService.InvalidateCache();
        }
    }
}
