using BepInEx.Configuration;
using UnityEngine;
using WishlistExtended.Features;
using ZGFueDkx.ZGCLib.Config;

namespace WishlistExtended.Config
{
    internal static class Settings
    {
        public enum ECraftMode
        {
            Disabled,
            Favorite,
            Current,
            All,
        }

        public enum EHideoutMode
        {
            Disabled,
            Current,
            All,
        }

        public enum ETooltipMode
        {
            Disabled,
            Always,
            Hide,
            Cycle,
            HideFuture,
        };

        public enum ELooseLootAffix
        {
            Disabled,
            Prefix,
            Suffix,
        };

        public enum ELooseLootColorMode
        {
            Disabled,
            Name,
            Affix,
            Both,
        }

        public static ConfigEntry<ECraftMode>? CraftMode;
        public static ConfigEntry<EHideoutMode>? HideoutMode;
        public static ConfigEntry<bool>? IncludeBarters;
        public static ConfigEntry<bool>? IncludeQuests;
        public static ConfigEntry<int>? HideoutPriority;
        public static ConfigEntry<int>? CraftsPriority;
        public static ConfigEntry<int>? BartersPriority;
        public static ConfigEntry<int>? QuestsPriority;
        //public static ConfigEntry<bool>? BarterFallback;
        public static ConfigEntry<bool>? ShowLockedAsFuture;
        public static ConfigEntry<bool>? ShowStashCount;
        public static ConfigEntry<bool>? ShowRaidCount;
        public static ConfigEntry<bool>? RemoveIfEnough;
        public static ConfigEntry<bool>? ShowInCheckmark;
        public static ConfigEntry<bool>? RaidOnly;

        public static ConfigEntry<ETooltipMode>? TooltipMode;
        public static ConfigEntry<int>? TooltipSize;
        public static ConfigEntry<KeyboardShortcut>? ShowAllTooltipBind;
        public static ConfigEntry<KeyboardShortcut>? CycleTooltipBind;

        public static ConfigEntry<Color>? CurrentCraftsColor;
        public static ConfigEntry<Color>? FavoriteCraftsColor;
        public static ConfigEntry<Color>? FutureCraftsColor;
        public static ConfigEntry<Color>? CurrentHideoutColor;
        public static ConfigEntry<Color>? FutureHideoutColor;
        public static ConfigEntry<Color>? BarterColor;
        public static ConfigEntry<Color>? CurrentQuetsColor;

        public static ConfigEntry<ELooseLootAffix>? LooseLootAffix;
        public static ConfigEntry<ELooseLootColorMode>? LooseLootColor;
        public static ConfigEntry<Color>? LooseLootColorValue;
        public static ConfigEntry<string>? LooseLootAffixValue;


        public static ConfigEntry<bool>? ShowDebug;

        public static void Init(ConfigFile config)
        {
            ConfigCategory general = config.MakeCategory(1, "General");
            ConfigCategory tooltip = config.MakeCategory(2, "Tooltip");
            ConfigCategory colors = config.MakeCategory(3, "Colors");
            ConfigCategory loose = config.MakeCategory(4, "Loose Loot");
            ConfigCategory debug = config.MakeCategory(9, "Debug");

            /*
             * GENERAL
             */
            CraftMode = general.BindConfig(
                "Hideout crafting wishlist mode",
                ECraftMode.All,
                "Specifies which crafts should be wishlisted:\n" +
                "Disabled - don't include any crafts\n" +
                "Favorite - include only favorite crafts\n" +
                "Current - include all currently available crafts\n" +
                "All - include all crafts (even future ones)"
            );

            CraftMode.SettingChanged += (_, _) => WishlistService.RebuildAll();

            HideoutMode = general.BindConfig(
                "Hideout upgrades wishlist mode",
                EHideoutMode.All,
                "Specifies which hideout upgrades should be wishlisted:\n" +
                "Disabled - don't include any upgrades\n" +
                "Current - include all currently available upgrades\n" +
                "All - include all hideout upgrades (even future ones)"
            );

            HideoutMode.SettingChanged += (_, _) => WishlistService.RebuildAll();

            IncludeBarters = general.BindConfig(
                "Include barters",
                true,
                "Whether or not to include barters. Mod must be installed on the server for barters wishlist!"
            );

            IncludeBarters.SettingChanged += (_, _) => WishlistService.RebuildAll();

            IncludeQuests = general.BindConfig(
                "Include quests",
                false,
                "Whether or not to include current quests."
            );

            IncludeQuests.SettingChanged += (_, _) => WishlistService.RebuildAll();

            CraftsPriority = general.BindConfig(
                "Crafts priority",
                2,
                "Icon priority for craftings - higher value is more important"
            );

            HideoutPriority = general.BindConfig(
                "Hideout priority",
                3,
                "Icon priority for hideout upgrades - higher value is more important"
            );

            BartersPriority = general.BindConfig(
                "Barters priority",
                1,
                "Icon priority for barters - higher value is more important"
            );

            QuestsPriority = general.BindConfig(
                "Quests priority",
                4,
                "Icon priority for quests - higher value is more important"
            );

            /*BarterFallback = general.BindConfig(
                "Barter fallback",
                true,
                "Whether mod should try to fetch barters in other ways if this mod is not installed on the server"
            );*/

            ShowLockedAsFuture = general.BindConfig(
                "Show locked as future",
                true,
                "Whether locked upgrades/crafts should be listed as future instead of current"
            );

            ShowLockedAsFuture.SettingChanged += (_, _) => WishlistService.RebuildAll();

            ShowStashCount = general.BindConfig(
                "Show amount of items in stash",
                true,
                "Whether total amount of items in stash should be displayed in the tooltip"
            );

            ShowRaidCount = general.BindConfig(
                "Include in-raid items",
                false,
                "Whether items in inventory should be included in amount of items in stash"
            );

            ShowRaidCount.SettingChanged += (_, _) => StashService.InvalidateCache();

            RemoveIfEnough = general.BindConfig(
                "Remove from wishlist if have enough",
                false,
                "Whether items should be removed from wishlist if you have enough items. If crafts or barters are enabled, item will not be removed if it's present in either of these categories"
            );

            ShowInCheckmark = general.BindConfig(
                "Show in checkmark tooltip",
                false,
                "Whether wishlist details should be also shown in checkmark tooltip"
            );

            RaidOnly = general.BindConfig(
                "In-Raid only",
                false,
                "Whether the auto-generated wishlist should work only in raid. Manual wishlist will work regardless"
            );

            RaidOnly.SettingChanged += (_, _) => WishlistService.RebuildAll();

            /*
             * TOOLTIP
             */
            TooltipMode = tooltip.BindConfig(
                "Tooltip mode",
                ETooltipMode.Always,
                "Defines the behavior of tooltip:\n" +
                "Disabled - no wishlist details in the tooltip\n" +
                "Always - no binds, everythig is shown\n" +
                "Hide - don't show anything, use 'Show all' bind to show\n" +
                "Cycle - only one category, use 'Cycle tooltip' bind to change, use 'Show all' bind to show all\n" +
                "HideFuture - hide future crafts/modules, use 'Show all' bind to show them"
            );

            TooltipSize = tooltip.BindConfig(
                "Tooltip text size",
                30,
                "Determines size of the tooltip text. 30 is normal, 0 is extremely small",
                acceptableValue: new AcceptableValueRange<int>(0, 30)
            );

            ShowAllTooltipBind = tooltip.BindConfig(
                "Show all bind",
                new KeyboardShortcut(KeyCode.LeftShift),
                "Shows info hidden by currently selected 'Tooltip mode'"
            );

            CycleTooltipBind = tooltip.BindConfig(
                "Cycle tooltip bind",
                new KeyboardShortcut(KeyCode.LeftAlt),
                "When pressed, ctagory displayed in tooltip is changed (Cycle tooltip mode only)"
            );

            /*
             * COLORS
             */
            CurrentCraftsColor = colors.BindColor(
                "Current crafts color",
                "#be99ff",
                "Color of current crafts in tooltip"
            );

            FavoriteCraftsColor = colors.BindColor(
                "Favorite crafts color",
                "#d7b442",
                "Color of favorite crafts in tooltip"
            );

            FutureCraftsColor = colors.BindColor(
                "Future crafts color",
                "#7e33ff",
                "Color of future crafts in tooltip"
            );

            CurrentHideoutColor = colors.BindColor(
                "Current hideout upgrades color",
                "#ff8080",
                "Color of current hideout upgrades in tooltip"
            );

            FutureHideoutColor = colors.BindColor(
                "Future hideout upgrades color",
                "#ff1a1a",
                "Color of future hideout upgrades in tooltip"
            );

            BarterColor = colors.BindColor(
                "Barters color",
                "#5cd65c",
                "Color of barters in tooltip"
            );

            CurrentQuetsColor = colors.BindColor(
                "Current quests color",
                "#85e0e0",
                "Color of current quests in tooltip"
            );

            /*
             * LOOSE
             */
            LooseLootAffix = loose.BindConfig(
                "Loose loot affix mode",
                ELooseLootAffix.Suffix,
                "Specifies how to add the affix to loose loot names:\n" +
                "Disabled - do nothing\n" +
                "Prefix - show the affix before the item name\n" +
                "Suffix - show the affix after the item name"
            );

            LooseLootColor = loose.BindConfig(
                "Loose loot color mode",
                ELooseLootColorMode.Affix,
                "Specifies how loose loot names will be colored:\n" +
                "Disabled - do nothing\n" +
                "Name - set the color of the item name\n" +
                "Affix - set the color of the affix\n" +
                "Both - set the color of both the item name and the affix"
            );

            LooseLootColorValue = loose.BindColor(
                "Loose loot color",
                "#ffff33",
                "Color of the item name (only in Color mode)"
            );

            LooseLootAffixValue = loose.BindConfig(
                "Loose loot affix",
                "✦",
                "Affix to show next to the item name when enabled"
            );

            /*
             * Debug
             */
            ShowDebug = debug.BindConfig(
                "Debug logs",
                false,
                "Enable debug logs in Player.log"
            );

            debug.BindButton("Rebuild caches", "Rebuild", "Forces rebuild of all caches", () =>
            {
                BarterDataService.LoadBarters();
                WishlistService.RebuildAll();
            });

            Plugin.LogSource?.LogInfo("Settings loaded");
        }
    }
}
