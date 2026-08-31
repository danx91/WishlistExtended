using EFT.Quests;
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
            return AccessTools.DeclaredMethod(typeof(QuestControllerClientLocalGame), nameof(QuestControllerClientLocalGame.HandoverItem));
        }

        [PatchPostfix]
        private static void Postfix()
        {
            Plugin.LogDebug("QuestControllerClientLocalGame.HandoverItem");
            QuestDataService.InvalidateCache();
        }
    }

    internal class GClass4008HandoverPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.DeclaredMethod(typeof(QuestControllerClientGame), nameof(QuestControllerClientGame.HandoverItem));
        }

        [PatchPostfix]
        private static void Postfix()
        {
            Plugin.LogDebug("QuestControllerClientGame.HandoverItem");
            QuestDataService.InvalidateCache();
        }
    }

    internal class LocalControllerHandoverPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.DeclaredMethod(typeof(QuestControllerClientBackend), nameof(QuestControllerClientBackend.HandoverItem));
        }

        [PatchPostfix]
        private static void Postfix()
        {
            Plugin.LogDebug("QuestControllerClientBackend.HandoverItem");
            QuestDataService.InvalidateCache();
        }
    }
}
