using EFT.Quests;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using WishlistExtended.Features;

namespace WishlistExtended.Patches
{
    class QuestClassSetStatusPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Quest), nameof(Quest.SetStatus));
        }

        [PatchPrefix]
        static void Prefix(Quest __instance, ref EQuestStatus __state)
        {
            __state = __instance.QuestStatus;
        }

        [PatchPostfix]
        static void Postfix(Quest __instance, EQuestStatus __state)
        {
            if (__instance.QuestStatus != __state)
            {
                Plugin.LogDebug($"Quest status changed for {__instance.Id} from {__state} to {__instance.QuestStatus}. Invalidating quests cache");
                QuestDataService.InvalidateCache();
            }
        }
    }
}
