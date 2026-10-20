using SPTarkov.Server.Core.Models.Spt.Mod;

namespace WishlistExtended
{
    public record SkillDistributionMetadata : IModMetadata
    {
        public string ModGuid { get; init; } = "com.zgfuedkx.wishlistextended";
        public string Name { get; init; } = "Wishlist Extended";
        public string Author { get; init; } = "ZGFueDkx";
        public List<string>? Contributors { get; init; }
        public SemanticVersioning.Version Version { get; init; } = new("1.2.0");
        public SemanticVersioning.Range SptVersion { get; init; } = new("~4.1.0");
        public bool HasPrepatcher { get; init; } = false;
        public List<string>? Incompatibilities { get; init; }
        public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
        public string? Url { get; init; } = "https://github.com/danx91/WishlistExtended";
        public string License { get; init; } = "GNU GPLv3";
    }

}
