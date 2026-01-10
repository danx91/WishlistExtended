using EFT.InventoryLogic;
using EFT.UI;
using EFT.UI.DragAndDrop;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using WishlistExtended.Config;
using WishlistExtended.Models;
using ZGFueDkx.ZGCLib.Config;

namespace WishlistExtended.Features
{
    internal static class TooltipManager
    {
        private static bool _fromQuest = false;

        private static Item? _item;
        private static WishlistData? _wishlist;

        private static SimpleTooltip? _tooltip;
        private static string? _tooltipText;

        private static int _tooltipCycle = 0;

        private static readonly AccessTools.FieldRef<SimpleTooltip, TextMeshProUGUI> _labelRef =
            AccessTools.FieldRefAccess<SimpleTooltip, TextMeshProUGUI>("_label");

        public static void OnItemShowTooltip(GridItemView gridItem)
        {
            if (WishlistResolver.ResolveWhishlist(true, gridItem.Item).TryGetValue(gridItem.Item.TemplateId, out _wishlist))
            {
                _item = gridItem.Item;
            }
            else
            {
                Cleanup();
            }
        }

        public static void OnQuestShowTooltip()
        {
            if (Settings.ShowInCheckmark!.Value)
            {
                return;
            }

            _fromQuest = true;
            _tooltip = null;
        }

        public static void OnItemExit() => Cleanup();

        public static void OnTooltipShow(SimpleTooltip tooltip, ref string text)
        {
            if (_item is null)
            {
                return;
            }

            if (_fromQuest)
            {
                _fromQuest = false;
                return;
            }

            _tooltip = tooltip;

            string? tooltipText = MakeTooltip();
            if (tooltipText is null)
            {
                _tooltipText = "";
                return;
            }

            _tooltipText = tooltipText;
            text += tooltipText;
        }

        public static void OnTooltipUpdate(SimpleTooltip tooltip)
        {
            if (_item is null || _tooltip is null || _tooltipText is null)
            {
                return;
            }

            if (tooltip != _tooltip)
            {
                Cleanup();
                return;
            }

            if (Settings.CycleTooltipBind!.Value.IsDown())
            {
                _tooltipCycle++;
            }

            string? tooltipText = MakeTooltip() ?? "";

            if (_tooltipText.Equals(tooltipText, StringComparison.Ordinal))
            {
                return;
            }

            TextMeshProUGUI? label = _labelRef(tooltip);
            if (label is null)
            {
                return;
            }

            if (_tooltipText.Length == 0)
            {
                label.text += tooltipText;
            }
            else
            {
                _tooltip.SetText(label.text.Replace(_tooltipText, tooltipText, StringComparison.Ordinal));
            }

            _tooltipText = tooltipText;
        }

        private static void Cleanup()
        {
            _fromQuest = false;
            _item = null;
            _wishlist = null;
            _tooltip = null;
            _tooltipText = null;
        }

        private static string? MakeTooltip()
        {
            var tooltipMode = Settings.TooltipMode!.Value;
            if (tooltipMode == Settings.ETooltipMode.Disabled)
            {
                return null;
            }

            bool showAll = Settings.ShowAllTooltipBind!.Value.IsPressed();

            string? tooltip = tooltipMode switch
            {
                Settings.ETooltipMode.Disabled => null,

                Settings.ETooltipMode.Hide when showAll => FormatFull(),
                Settings.ETooltipMode.Hide => null,

                Settings.ETooltipMode.HideFuture when showAll => FormatFull(),
                Settings.ETooltipMode.HideFuture => FormatShort(),

                Settings.ETooltipMode.Cycle when showAll => FormatFull(),
                Settings.ETooltipMode.Cycle => FormatCycle(),

                Settings.ETooltipMode.Always => FormatFull(),

                _ => null
            };

            if (tooltip is not null)
            {
                int size = 70 + Settings.TooltipSize!.Value;
                if (size != 100)
                {
                    tooltip = $"<size={(int)size}%>{tooltip}</size>";
                }
            }

            return tooltip;
        }

        private static string? FormatFull()
        {
            StringBuilder result = new();

            AppendCrafts(result, true);
            AppendHideout(result, true);
            AppendBarters(result);
            AppendQuests(result, true);

            if (result.Length == 0)
            {
                return null;
            }

            string prefix = "\n";

            if (Settings.ShowStashCount!.Value)
            {
                prefix += GetInStashText() + "\n";
            }

            return prefix + result.ToString();
        }

        private static string? FormatShort()
        {
            StringBuilder result = new();

            AppendCrafts(result, false);
            AppendHideout(result, false);
            AppendBarters(result);
            AppendQuests(result, false);

            if (result.Length == 0)
            {
                return null;
            }

            string prefix = "\n";

            if (Settings.ShowStashCount!.Value)
            {
                prefix += GetInStashText() + "\n";
            }

            return prefix + result.ToString();
        }

        private static string? FormatCycle()
        {
            StringBuilder result = new();
            List<int> steps = new(4);

            if (Settings.CraftMode!.Value != Settings.ECraftMode.Disabled)
            {
                steps.Add(0);
            }

            if (Settings.HideoutMode!.Value != Settings.EHideoutMode.Disabled)
            {
                steps.Add(1);
            }

            if (Settings.IncludeBarters!.Value)
            {
                steps.Add(2);
            }

            if (Settings.IncludeQuests!.Value)
            {
                steps.Add(3);
            }

            switch (steps[_tooltipCycle % steps.Count])
            {
                case 0:
                    AppendCrafts(result, true);
                    break;
                case 1:
                    AppendHideout(result, true);
                    break;
                case 2:
                    AppendBarters(result);
                    break;
                case 3:
                    AppendQuests(result, true);
                    break;
            }
            ;

            if (result.Length == 0)
            {
                return null;
            }

            string prefix = "\n";

            if (Settings.ShowStashCount!.Value)
            {
                prefix += GetInStashText() + "\n";
            }

            return prefix + result.ToString();
        }

        private static string GetInStashText()
        {
            ItemsCount count = StashService.GetItemCount(_item!.TemplateId);
            return String.Format("wext_in_stash".Localized(), $"<color=#20f020>{count.Total}</color>", $"<color=#f0f020>{count.Fir}</color>");
        }

        private static void AppendCrafts(StringBuilder sb, bool includeFuture)
        {
            if (!_wishlist!.HasCrafts)
            {
                return;
            }

            StringBuilder favoriteCrafts = new();
            StringBuilder currentCrafts = new();
            StringBuilder futureCrafts = new();

            string favoriteColor = Settings.FavoriteCraftsColor!.GetHexColor();
            string currentColor = Settings.CurrentCraftsColor!.GetHexColor();
            string futureColor = Settings.FutureCraftsColor!.GetHexColor();

            foreach (var craft in _wishlist!.Crafts)
            {
                if (!includeFuture && craft.IsFuture)
                {
                    continue;
                }

                AppendCraft(
                    craft.IsFavorite ? favoriteCrafts : (craft.IsFuture ? futureCrafts : currentCrafts),
                    craft,
                    craft.IsFavorite ? favoriteColor : (craft.IsFuture ? futureColor : currentColor)
                );
            }

            if (favoriteCrafts.Length > 0)
            {
                sb.AppendLine("wext_favorite_crafts".Localized());
                sb.Append(favoriteCrafts);
            }

            if (currentCrafts.Length > 0)
            {
                sb.AppendLine(favoriteCrafts.Length > 0 ? "wext_other_crafts".Localized() : "wext_current_crafts".Localized());
                sb.Append(currentCrafts);
            }

            if (futureCrafts.Length > 0)
            {
                sb.AppendLine("wext_future_crafts".Localized());
                sb.Append(futureCrafts);
            }
        }

        private static void AppendHideout(StringBuilder sb, bool includeFuture)
        {
            if (!_wishlist!.HasHideout)
            {
                return;
            }

            StringBuilder currentAreas = new();
            StringBuilder futureAreas = new();

            string currentColor = Settings.CurrentHideoutColor!.GetHexColor();
            string futureColor = Settings.FutureHideoutColor!.GetHexColor();

            int currentCount = 0;
            int futureCount = 0;

            foreach (var upgrade in _wishlist!.Hideout)
            {
                if (!includeFuture && upgrade.IsFuture)
                {
                    continue;
                }

                if (upgrade.IsFuture)
                {
                    AppendArea(
                        futureAreas,
                        upgrade,
                        futureColor,
                        ref futureCount
                    );
                }
                else
                {
                    AppendArea(
                        currentAreas,
                        upgrade,
                        currentColor,
                        ref currentCount
                    );
                }
            }

            if (currentAreas.Length > 0)
            {
                sb.Append("wext_current_upgrades".Localized())
                    .Append(" (")
                    .Append(currentCount)
                    .AppendLine(")")
                    .Append(currentAreas);
            }

            if (futureAreas.Length > 0)
            {
                sb.Append("wext_future_upgrades".Localized())
                    .Append(" (")
                    .Append(futureCount)
                    .AppendLine(")")
                    .Append(futureAreas);
            }
        }

        private static void AppendBarters(StringBuilder sb)
        {
            if (!_wishlist!.HasBarters)
            {
                return;
            }

            StringBuilder barters = new();
            string color = Settings.BarterColor!.GetHexColor();

            foreach (var barter in _wishlist!.Barters)
            {
                AppendBarter(
                    barters,
                    barter,
                    color
                );
            }

            if (barters.Length > 0)
            {
                sb.AppendLine("wext_barters".Localized());
                sb.Append(barters);
            }
        }

        private static void AppendQuests(StringBuilder sb, bool includeFuture)
        {
            if (!_wishlist!.HasQuests)
            {
                return;
            }

            StringBuilder currentQuests = new();

            string currentColor = Settings.CurrentQuetsColor!.GetHexColor();

            int currentCount = 0;

            foreach (var quest in _wishlist!.Quests)
            {
                AppendQuest(
                    currentQuests,
                    quest,
                    currentColor,
                    ref currentCount
                );
            }

            if (currentQuests.Length > 0)
            {
                sb.Append("wext_current_quests".Localized())
                    .Append(" (")
                    .Append(currentCount)
                    .AppendLine(")")
                    .Append(currentQuests);
            }
        }

        private static void AppendCraft(StringBuilder sb, WishlistCraftData data, string color)
        {
            sb.Append("  ")
                .Append(data.Quantity)
                .Append($" → <color=")
                .Append(color)
                .Append('>')
                .Append($"{data.ResultId} ShortName".Localized())
                .Append("</color> @ <color=")
                .Append(color)
                .Append('>')
                .Append(data.AreaType.LocalizeAreaName())
                .Append(" lvl ")
                .Append(data.AreaStage.ToString())
                .AppendLine("</color>");
        }

        private static void AppendArea(StringBuilder sb, WishlistHideoutData data, string color, ref int count)
        {
            sb.Append("  ")
                .Append(data.Quantity)
                .Append(" → <color=")
                .Append(color)
                .Append('>')
                .Append(data.AreaType.LocalizeAreaName())
                .Append(" lvl ")
                .Append(data.Stage.ToString())
                .AppendLine("</color>");

            count += data.Quantity;
        }

        private static void AppendBarter(StringBuilder sb, WishlistBarterData data, string color)
        {
            sb.Append("  ")
                .Append(data.Quantity)
                .Append(" → <color=")
                .Append(color)
                .Append('>')
                .Append($"{data.ResultId} ShortName".Localized())
                .Append("</color> @ <color=")
                .Append(color)
                .Append('>')
                .Append($"{data.TraderId} Nickname".Localized())
                .Append(" LL")
                .Append(data.LoyaltyLevel.ToString())
                .AppendLine("</color>");
        }

        private static void AppendQuest(StringBuilder sb, WishlistQuestData data, string color, ref int count)
        {
            sb.Append("  ")
                .Append(data.Quantity)
                .Append(" → <color=")
                .Append(color)
                .Append('>')
                .Append(data.QuestName.Localized())
                .Append("</color> @ <color=")
                .Append(color)
                .Append('>')
                .Append($"{data.TraderId} Nickname".Localized())
                .AppendLine("</color>");

            count += data.Quantity;
        }
    }
}
