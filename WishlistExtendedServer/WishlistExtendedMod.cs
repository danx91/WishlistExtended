using SPTarkov.Common.Models.Logging;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Tables;
using SPTarkov.Server.Core.Utils;

namespace WishlistExtended
{
    [Injectable(typePriority: OnLoadOrder.PostLoad)]
    internal class WishlistExtendedMod(
        HttpResponseUtil httpResponseUtil,
        TradersTable tradersTable,
        ISptLogger<WishlistExtendedMod> logger
    ) : IOnLoad
    {
        private static readonly List<MongoId> MoneyIds = [
            "5449016a4bdc2d6f028b456f",
            "5696686a4bdc2da3298b456a",
            "569668774bdc2da2298b4568"
        ];

        private static readonly Dictionary<MongoId, List<BarterData>> _barters = [];

        private readonly HttpResponseUtil _httpResponseUtil = httpResponseUtil;
        private readonly TradersTable _tradersTable = tradersTable;
        private readonly ISptLogger<WishlistExtendedMod> _logger = logger;


        public async Task OnLoadAsync(CancellationToken cancellationToken)
        {
            _barters.Clear();

            int total = 0;

            foreach (var (traderId, trader) in _tradersTable)
            {
                List<BarterData> current = [];
                _barters.Add(traderId, current);

                TraderAssort assort = trader.Assort;
                var loyalLevelItems = assort.LoyalLevelItems;
                var itemsById = assort.Items.ToDictionary(i => i.Id);

                foreach (var (tradeId, barterSchemes) in assort.BarterScheme)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!itemsById.TryGetValue(tradeId, out Item? foundItem))
                    {
                        continue;
                    }

                    bool hasLoyaltyLevel = loyalLevelItems.TryGetValue(tradeId, out int loyaltyLevel);

                    foreach (var shcemeList in barterSchemes)
                    {
                        foreach (var scheme in shcemeList)
                        {
                            if (MoneyIds.Contains(scheme.Template))
                            {
                                continue;
                            }

                            current.Add(new BarterData(scheme.Template, foundItem.Template, (int?)scheme.Count ?? 1, hasLoyaltyLevel ? loyaltyLevel : 1));
                            total++;
                        }
                    }
                }
            }

            _logger.Info($"[WishlistExtended] Loaded {total} barters from {_barters.Count} traders");
        }

        public ValueTask<string> GetBarters()
        {
            return new ValueTask<string>(_httpResponseUtil.NoBody(_barters));
        }
    }
}
