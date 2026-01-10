using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using WishlistExtended.Features;

namespace WishlistExtended.Patches
{
    internal class GClass4007HandoverPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.DeclaredMethod(typeof(GClass4007), nameof(GClass4007.HandoverItem));
        }

        [PatchPostfix]
        private static void Postfix()
        {
            Plugin.LogDebug("GClass4007.HandoverItem");
            QuestDataService.InvalidateCache();
        }
    }

    internal class GClass4008HandoverPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.DeclaredMethod(typeof(GClass4008), nameof(GClass4008.HandoverItem));
        }

        [PatchPostfix]
        private static void Postfix()
        {
            Plugin.LogDebug("GClass4008.HandoverItem");
            QuestDataService.InvalidateCache();
        }
    }

    internal class LocalControllerHandoverPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.DeclaredMethod(typeof(LocalQuestControllerClass), nameof(LocalQuestControllerClass.HandoverItem));
        }

        [PatchPostfix]
        private static void Postfix()
        {
            Plugin.LogDebug("LocalQuestControllerClass.HandoverItem");
            QuestDataService.InvalidateCache();
        }
    }
}
