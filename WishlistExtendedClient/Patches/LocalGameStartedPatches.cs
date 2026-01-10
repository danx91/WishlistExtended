using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using WishlistExtended.Features;

namespace WishlistExtended.Patches
{
    internal class LocalGameStartedPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(LocalGame), nameof(LocalGame.smethod_6));
        }

        [PatchPostfix]
        static void Postfix()
        {
            StashService.InvalidateCache();
        }
    }
}
