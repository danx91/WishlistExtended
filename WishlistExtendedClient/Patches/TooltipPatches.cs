using EFT.UI;
using EFT.UI.DragAndDrop;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using WishlistExtended.Features;

namespace WishlistExtended.Patches
{
    internal class SimpleTooltipShowPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.DeclaredMethod(typeof(SimpleTooltip), nameof(SimpleTooltip.Show));
        }

        [PatchPrefix]
        //[HarmonyBefore("com.swiftxp.spt.showmethemoney")] //Not working - Show Me the Money sadly doesn't use its GUID in patches
        private static void Prefix(SimpleTooltip __instance, ref string text)
        {
            TooltipManager.OnTooltipShow(__instance, ref text);
        }
    }

    internal class TooltipUpdatePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(Tooltip), nameof(Tooltip.Update));
        }

        [PatchPrefix]
        private static void Prefix(Tooltip __instance)
        {
            if (__instance is not SimpleTooltip simpleTooltip)
            {
                return;
            }

            TooltipManager.OnTooltipUpdate(simpleTooltip);
        }
    }

    internal class QuestTooltipPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(QuestItemViewPanel), nameof(QuestItemViewPanel.CG_Awake));
        }

        [PatchPrefix]
        private static void Prefix()
        {
            TooltipManager.OnQuestShowTooltip();
        }
    }
}
