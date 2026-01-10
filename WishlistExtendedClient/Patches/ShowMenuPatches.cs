using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using WishlistExtended.Features;

namespace WishlistExtended.Patches
{
    internal class ShowMenuPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(MainMenuControllerClass), nameof(MainMenuControllerClass.ShowMenuScreenSync));
        }

        [PatchPostfix]
        private static void Postfix()
        {
            WishlistService.Init();
            WishlistService.RebuildAll();
            BarterDataService.LoadBarters();
            StashService.InvalidateCache();
        }
    }
}
