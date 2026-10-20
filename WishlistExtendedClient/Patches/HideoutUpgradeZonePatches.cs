using EFT.Hideout;
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
            return AccessTools.Method(typeof(HideoutRepresentation), nameof(HideoutRepresentation.UpgradeZone));
        }

        [PatchPostfix]
        private static void Postfix()
        {
            Plugin.LogDebug("Hideout zone upgrade started");
            WishlistService.RebuildAll();
        }
    }
}
