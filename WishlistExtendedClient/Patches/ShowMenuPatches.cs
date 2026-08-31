using EFT;
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
            return AccessTools.Method(typeof(MainMenuShowOperation), nameof(MainMenuShowOperation.ShowMenuScreenSync));
        }

        [PatchPostfix]
        private static void Postfix()
        {
            Plugin.LogDebug("ShowMenuScreenSync called, initializing wishlist service and rebuilding all caches");
            WishlistService.Init();
            WishlistService.RebuildAll();
            BarterDataService.LoadBarters();
            StashService.BuildCache();
        }
    }
}
