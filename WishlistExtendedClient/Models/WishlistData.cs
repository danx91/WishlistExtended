using EFT;
using System.Collections.Generic;

namespace WishlistExtended.Models
{
    internal class WishlistData
    {
        public bool HasCrafts => Crafts.Count > 0;
        public bool HasHideout => Hideout.Count > 0;
        public bool HasBarters => Barters.Count > 0;
        public bool HasQuests => Quests.Count > 0;
        public bool HasAny => HasCrafts || HasHideout || HasBarters || HasQuests;

        public int TotalNeededHideout = 0;
        public int TotalNeededQuests = 0;
        public int TotalNeeded => TotalNeededHideout + TotalNeededQuests;

        public List<WishlistCraftData> Crafts { get; private set; } = [];
        public List<WishlistHideoutData> Hideout { get; private set; } = [];
        public List<WishlistBarterData> Barters { get; private set; } = [];
        public List<WishlistQuestData> Quests { get; private set; } = [];
    }

    internal class WishlistCraftData(MongoID resultId, int quantity, EAreaType areaType, int areaStage, bool isFuture, bool isFavorite)
    {
        public MongoID ResultId { get; set; } = resultId;
        public int Quantity { get; set; } = quantity;
        public EAreaType AreaType { get; set; } = areaType;
        public int AreaStage { get; set; } = areaStage;
        public bool IsFuture { get; set; } = isFuture;
        public bool IsFavorite { get; set; } = isFavorite;
    }

    internal class WishlistHideoutData(EAreaType areaType, int stage, int quantity, bool fir, bool isFuture)
    {
        public EAreaType AreaType { get; set; } = areaType;
        public int Stage { get; set; } = stage;
        public int Quantity { get; set; } = quantity;
        public bool Fir { get; set; } = fir;
        public bool IsFuture { get; set; } = isFuture;
    }

    internal class WishlistBarterData(MongoID resultId, MongoID traderId, int quantity, int loyaltyLevel)
    {
        public MongoID ResultId { get; set; } = resultId;
        public MongoID TraderId { get; set; } = traderId;
        public int Quantity { get; set; } = quantity;
        public int LoyaltyLevel { get; set; } = loyaltyLevel;
    }

    internal class WishlistQuestData(string questName, MongoID traderId, int quantity, bool fir, bool isFuture)
    {
        public string QuestName { get; set; } = questName;
        public MongoID TraderId { get; set; } = traderId;
        public int Quantity { get; set; } = quantity;
        public bool Fir { get; set; } = fir;
        public bool IsFuture { get; set; } = isFuture;
    }
}
