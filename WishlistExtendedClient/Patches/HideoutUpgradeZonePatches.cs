using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using WishlistExtended.Features;

namespace WishlistExtended.Patches
{
    internal class HideoutUpgradeZonePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(HideoutClass), nameof(HideoutClass.UpgradeZone));
        }

        [PatchPostfix]
        private static void Postfix()
        {
            WishlistService.RebuildAll();
        }
    }
}
