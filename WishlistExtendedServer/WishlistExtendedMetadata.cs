using SPTarkov.Server.Core.Models.Spt.Mod;

namespace WishlistExtended
{
    public record SkillDistributionMetadata : AbstractModMetadata
    {
        public override string ModGuid { get; init; } = "com.zgfuedkx.wishlistextended";
        public override string Name { get; init; } = "Wishlist Extended";
        public override string Author { get; init; } = "ZGFueDkx";
        public override List<string>? Contributors { get; init; }
        public override SemanticVersioning.Version Version { get; init; } = new("1.1.0");
        public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");
        public override List<string>? Incompatibilities { get; init; }
        public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
        public override string? Url { get; init; } = "https://github.com/danx91/WishlistExtended";
        public override bool? IsBundleMod { get; init; } = false;
        public override string License { get; init; } = "GNU GPLv3";
    }

}
