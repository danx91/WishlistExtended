using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace WishlistExtended.Patches
{
    internal class LocalGameStartedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(LocalGame), nameof(LocalGame.Create));
        }

        [PatchPostfix]
        static void Postfix()
        {
            //Currently disabled as it seems like it's too late to cahe hideout items
            //TODO: Consider using HideoutRepresentation._allStashItems as a replacement

            //Plugin.LogDebug("LocalGame started, invalidating stash cache");
            //StashService.InvalidateCache();
        }
    }
}
