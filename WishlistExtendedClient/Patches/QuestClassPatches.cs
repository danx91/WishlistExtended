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
            return AccessTools.Method(typeof(QuestClass), nameof(QuestClass.SetStatus));
        }

        [PatchPrefix]
        static void Prefix(QuestClass __instance, ref EQuestStatus __state)
        {
            __state = __instance.QuestStatus;
        }

        [PatchPostfix]
        static void Postfix(QuestClass __instance, EQuestStatus __state)
        {
            if (__instance.QuestStatus != __state)
            {
                QuestDataService.InvalidateCache();
            }
        }
    }
}
