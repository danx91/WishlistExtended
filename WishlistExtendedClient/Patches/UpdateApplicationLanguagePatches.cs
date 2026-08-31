using EFT;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using WishlistExtended.Features;

namespace WishlistExtended.Patches
{
    class UpdateApplicationLanguagePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(LocalizationManager), nameof(LocalizationManager.UpdateApplicationLanguage));
        }

        [PatchPostfix]
        private static void Postfix()
        {
            LocalizationService.LoadLocale(LocalizationManager.Instance.Culture);
        }
    }
}
