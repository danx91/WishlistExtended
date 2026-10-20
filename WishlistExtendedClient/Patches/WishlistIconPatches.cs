using EFT.Hideout;
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
            return AccessTools.Method(typeof(GridItemView), nameof(GridItemView.OnExamine));
        }

        [PatchPrefix]
        private static void Prefix(GridItemView __instance)
        {
            Plugin.LogDebug($"GridItemView.OnExamine {__instance.Item?.TemplateId}");
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
        private static void Prefix(ItemContext itemContext)
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
            return AccessTools.Method(typeof(ItemSpecificationPanel), nameof(ItemSpecificationPanel.OnRemoveFromWishlist));
        }

        [PatchPrefix]
        private static void Prefix(Item ____item)
        {
            Plugin.LogDebug($"ItemSpecificationPanel.OnRemoveFromWishlist {____item?.TemplateId}");
            WishlistManagerIsInWishlistPatch.CurrentItem = ____item;
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
            return AccessTools.Method(typeof(TraderDealScreen), nameof(TraderDealScreen.TryShowWishlistPurchaseNotification));
        }

        [PatchPrefix]
        private static void Prefix(Item item)
        {
            Plugin.LogDebug($"TraderDealScreen.TryShowWishlistPurchaseNotification {item?.TemplateId}");
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
            return AccessTools.Method(typeof(AreaBehaviour), nameof(AreaBehaviour.ShowNotification));
        }

        [PatchPrefix]
        private static void Prefix(Item item)
        {
            Plugin.LogDebug($"AreaBehaviour.ShowNotification {item?.TemplateId}");
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
            return AccessTools.DeclaredMethod(typeof(WishlistContextInteractions), nameof(WishlistContextInteractions.ExecuteInteractionInternal));
        }

        [PatchPrefix]
        private static void Prefix(WishlistContextInteractions __instance)
        {
            Plugin.LogDebug($"WishlistContextInteractions.ExecuteInteractionInternal {__instance._itemContext.Item?.TemplateId}");
            WishlistManagerIsInWishlistPatch.CurrentItem = __instance._itemContext.Item;
        }

        [PatchPostfix]
        private static void Postfix()
        {
            WishlistManagerIsInWishlistPatch.CurrentItem = null;
        }
    }
}
