using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Utils;

namespace WishlistExtended
{
    [Injectable(typePriority: OnLoadOrder.PostDBModLoader + 1)]
    internal class WishlistExtendedMod(
        HttpResponseUtil httpResponseUtil,
        DatabaseService databaseService,
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
        private readonly DatabaseService _databaseService = databaseService;
        private readonly ISptLogger<WishlistExtendedMod> _logger = logger;


        public Task OnLoad()
        {
            _barters.Clear();

            int total = 0;
            var allTraders = _databaseService.GetTraders();

            foreach (var (traderId, trader) in allTraders)
            {
                List<BarterData> current = [];
                _barters.Add(traderId, current);

                TraderAssort assort = trader.Assort;

                foreach (var (tradeId, barterSchemes) in assort.BarterScheme)
                {
                    bool hasLoyaltyLevel = assort.LoyalLevelItems.TryGetValue(tradeId, out int loyaltyLevel);

                    foreach (var shcemeList in barterSchemes)
                    {
                        foreach (var scheme in shcemeList)
                        {
                            if (MoneyIds.Contains(scheme.Template))
                            {
                                continue;
                            }

                            Item? foundItem = assort.Items.Find(i => i.Id == tradeId);
                            if (foundItem is null)
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

            return Task.CompletedTask;
        }

        public ValueTask<string> GetBarters()
        {
            return new ValueTask<string>(_httpResponseUtil.NoBody(_barters));
        }
    }
}
