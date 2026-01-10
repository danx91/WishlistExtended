using EFT.InventoryLogic;
using EFT.UI;
using EFT.UI.DragAndDrop;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;

namespace WishlistExtended.Patches
{
    internal class GridItemViewNewViewPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GridItemView), nameof(GridItemView.NewGridItemView));
        }

        [PatchPrefix]
        private static void Prefix(Item item)
        {
            Plugin.LogDebug($"GridItemView.NewGridItemView {item?.TemplateId}");
            WishlistManagerIsInWishlistPatch.CurrentItem = item;
        }

        [PatchPostfix]
        private static void Postfix()
        {
            WishlistManagerIsInWishlistPatch.CurrentItem = null;
        }
    }

    internal class GridItemViewValidateViewPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.DeclaredMethod(typeof(GridItemView), nameof(GridItemView.ValidateWishlistView));
        }

        [PatchPrefix]
        private static void Prefix(GridItemView __instance)
        {
            Plugin.LogDebug($"GridItemView.ValidateWishlistView {__instance.Item?.TemplateId}");
            WishlistManagerIsInWishlistPatch.CurrentItem = __instance.Item;
        }

        [PatchPostfix]
        private static void Postfix()
        {
            WishlistManagerIsInWishlistPatch.CurrentItem = null;
        }
    }

    internal class GridItemViewExaminePatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GridItemView), nameof(GridItemView.method_24));
        }

        [PatchPrefix]
        private static void Prefix(GridItemView __instance)
        {
            Plugin.LogDebug($"GridItemView.method_24 {__instance.Item?.TemplateId}");
            WishlistManagerIsInWishlistPatch.CurrentItem = __instance.Item;
        }

        [PatchPostfix]
        private static void Postfix()
        {
            WishlistManagerIsInWishlistPatch.CurrentItem = null;
        }
    }

    internal class ItemSpecificationPanelShowPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ItemSpecificationPanel), nameof(ItemSpecificationPanel.Show));
        }

        [PatchPrefix]
        private static void Prefix(ItemContextAbstractClass itemContext)
        {
            Plugin.LogDebug($"ItemSpecificationPanel.Show {itemContext.Item?.TemplateId}");
            WishlistManagerIsInWishlistPatch.CurrentItem = itemContext.Item;
        }

        [PatchPostfix]
        private static void Postfix()
        {
            WishlistManagerIsInWishlistPatch.CurrentItem = null;
        }
    }

    internal class ItemSpecificationPanelRemoveWishlistPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(ItemSpecificationPanel), nameof(ItemSpecificationPanel.method_24));
        }

        [PatchPrefix]
        private static void Prefix(Item ___item_0)
        {
            Plugin.LogDebug($"ItemSpecificationPanel.method_24 {___item_0?.TemplateId}");
            WishlistManagerIsInWishlistPatch.CurrentItem = ___item_0;
        }

        [PatchPostfix]
        private static void Postfix()
        {
            WishlistManagerIsInWishlistPatch.CurrentItem = null;
        }
    }

    internal class TraderDealScreenWishlistPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(TraderDealScreen), nameof(TraderDealScreen.method_2));
        }

        [PatchPrefix]
        private static void Prefix(Item item)
        {
            Plugin.LogDebug($"TraderDealScreen.method_2 {item?.TemplateId}");
            WishlistManagerIsInWishlistPatch.CurrentItem = item;
        }

        [PatchPostfix]
        private static void Postfix()
        {
            WishlistManagerIsInWishlistPatch.CurrentItem = null;
        }
    }

    internal class AreaBehaviourNotificationPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(GClass2447), nameof(GClass2447.ShowNotification));
        }

        [PatchPrefix]
        private static void Prefix(Item item)
        {
            Plugin.LogDebug($"GClass2447.ShowNotification {item?.TemplateId}");
            WishlistManagerIsInWishlistPatch.CurrentItem = item;
        }

        [PatchPostfix]
        private static void Postfix()
        {
            WishlistManagerIsInWishlistPatch.CurrentItem = null;
        }
    }

    internal class InteractionExecuteInternalPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.DeclaredMethod(typeof(GClass3782), nameof(GClass3782.ExecuteInteractionInternal));
        }

        [PatchPrefix]
        private static void Prefix(GClass3782 __instance)
        {
            Plugin.LogDebug($"GClass3782.ExecuteInteractionInternal {__instance.ItemContextAbstractClass.Item?.TemplateId}");
            WishlistManagerIsInWishlistPatch.CurrentItem = __instance.ItemContextAbstractClass.Item;
        }

        [PatchPostfix]
        private static void Postfix()
        {
            WishlistManagerIsInWishlistPatch.CurrentItem = null;
        }
    }
}
