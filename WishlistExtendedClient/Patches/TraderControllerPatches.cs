using EFT.InventoryLogic;
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
            return AccessTools.Method(typeof(ItemController), nameof(ItemController.SafeRefreshItemEvent));
        }

        [PatchPostfix]
        private static void Postfix()
        {
            Plugin.LogDebug("Item refreshed event, invalidating stash cache");
            TooltipManager.OnItemExit();
            StashService.InvalidateCache();
        }
    }
}
