using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Eft.Common;
using SPTarkov.Server.Core.Utils;

namespace WishlistExtended
{
    [Injectable]
    internal class WishlistExtendedRouter(
        JsonUtil jsonUtil,
        WishlistExtendedMod wishlistExtendedMod
    ) : StaticRouter(
        jsonUtil,
        [
            new RouteAction<EmptyRequestData>(
                "/wishlist-extended/barters/rebuild",
                async (url, info, sessionId, output, cancellationToken) => {
                    wishlistExtendedMod.RebuildBarters(cancellationToken);
                    return await wishlistExtendedMod.GetBarters();
                }
            ),
            new RouteAction<EmptyRequestData>(
                "/wishlist-extended/barters",
                (url, info, sessionId, output, cancellationToken) => wishlistExtendedMod.GetBarters()
            )
        ]
    )
    { }
}
