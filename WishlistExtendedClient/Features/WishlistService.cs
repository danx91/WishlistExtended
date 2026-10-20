using Comfort.Common;
using EFT;
using EFT.Hideout;
using SPT.Reflection.Utils;

namespace WishlistExtended.Features
{
    internal static class WishlistService
    {
        public static void Init()
        {
            if (Singleton<HideoutRepresentation>.Instantiated)
            {
                Singleton<HideoutRepresentation>.Instance.OnAreaUpdated += () => RebuildAll();
            }
            else
            {
                Plugin.LogSource?.LogError("WishlistService.Init: Singleton HideoutRepresentation is not instantiated");
            }

            Profile profile = ClientAppUtils.GetClientApp().GetClientBackEndSession().Profile;

            profile.Skills.AnySkillUp._innerEvent += (_, _) =>
            {
                Plugin.LogDebug("WishlistService: SkillUp event triggered, rebuilding all");
                RebuildAll();
            };
            profile.OnTraderLoyaltyChanged += (_) =>
            {
                Plugin.LogDebug("WishlistService: TraderLoyaltyChanged event triggered, rebuilding all");
                RebuildAll();
            };
        }

        public static void RebuildAll()
        {
            CraftDataService.InvalidateCache();
            HideoutDataService.InvalidateCache();
            QuestDataService.InvalidateCache();
        }
    }
}
