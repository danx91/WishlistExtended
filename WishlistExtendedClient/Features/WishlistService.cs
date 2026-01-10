using Comfort.Common;
using EFT;
using SPT.Reflection.Utils;

namespace WishlistExtended.Features
{
    internal static class WishlistService
    {
        public static void Init()
        {
            if (Singleton<HideoutClass>.Instantiated)
            {
                Singleton<HideoutClass>.Instance.OnAreaUpdated += () => RebuildAll();
            }
            else
            {
                Plugin.LogSource?.LogError("WishlistService.Init: Singleton HideoutClass is not instantiated");
            }

            Profile profile = ClientAppUtils.GetClientApp().GetClientBackEndSession().Profile;

            profile.Skills.AnySkillUp.Action_1 += (_, _) => RebuildAll();
            profile.OnTraderLoyaltyChanged += (_) => RebuildAll();
        }

        public static void RebuildAll()
        {
            CraftDataService.InvalidateCache();
            HideoutDataService.InvalidateCache();
            QuestDataService.InvalidateCache();
        }
    }
}
