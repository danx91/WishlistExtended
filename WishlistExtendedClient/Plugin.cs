using BepInEx;
using BepInEx.Logging;
using SPT.Reflection.Patching;
using System.IO;
using System.Reflection;
using WishlistExtended.Config;

namespace WishlistExtended
{
    [
        BepInPlugin("com.zgfuedkx.wishlistextended", "ZGFueDkx-WishlistExtended", "1.1.0"),
        BepInDependency("com.SPT.core", "4.0.0"),
    ]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource? LogSource;
        public static string? ModPath;

        public void Awake()
        {
            LogSource = Logger;

            ModPath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Plugin)).Location);
            ModPath.Replace("\\", "/");

            Settings.Init(Config);

            new PatchManager(this, true).EnablePatches();

            LogSource.LogInfo($"WishlistExtended by ZGFueDkx version {Info.Metadata.Version} started");
        }

        public static void LogDebug(string msg)
        {
            if (Settings.ShowDebug!.Value)
            {
                LogSource?.LogDebug(msg);
            }
        }
    }
}
