using EFT;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using System.Reflection;
using WishlistExtended.Features;

namespace WishlistExtended.Patches
{
    internal class GetActionClassPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return AccessTools.Method(typeof(InteractionContextHelper), nameof(InteractionContextHelper.GetAvailableInteractionState));
        }

        [PatchPrefix]
        private static void Prefix(GamePlayerOwner owner, Item rootItem, ref string lootItemName)
        {
            if (rootItem is not InventoryEquipment && owner.Player.HandsController.SupportPickup())
            {
                LooseLootService.ApplyWishlistText(rootItem.TemplateId, ref lootItemName);
            }
        }
    }
}
