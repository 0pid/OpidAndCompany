using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Enums;
using UnityEngine;

namespace OpidAndCompany
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.HardDependency)]
    [LobbyCompatibility(CompatibilityLevel.ClientOnly, VersionStrictness.None)]
    [BepInDependency(LethalLib.Plugin.ModGUID)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }

        public static AssetBundle MyAssetBundle;

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            MyAssetBundle = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, "opidsitems"));
            if (MyAssetBundle == null)
            {
                Logger.LogError("Failed to load custom assets."); // ManualLogSource for your plugin
                return;
            }

            // 
            int iRarity = 30;
            Item cuteCeramicCritter = MyAssetBundle.LoadAsset<Item>("assets/cuteceramiccritter.asset");
            LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(cuteCeramicCritter.spawnPrefab);
            LethalLib.Modules.Utilities.FixMixerGroups(cuteCeramicCritter.spawnPrefab);
            LethalLib.Modules.Items.RegisterScrap(cuteCeramicCritter, iRarity, LethalLib.Modules.Levels.LevelTypes.All);

            // Register Glow Stick
            int iPrice = 1;
            TerminalNode iTerminalNode = MyAssetBundle.LoadAsset<TerminalNode>("Assets/iTerminalNode.asset");
            Item MyTestItem = MyAssetBundle.LoadAsset<Item>("Assets/GlowStick.asset");
            LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(MyTestItem.spawnPrefab);
            LethalLib.Modules.Utilities.FixMixerGroups(MyTestItem.spawnPrefab);
            LethalLib.Modules.Items.RegisterShopItem(MyTestItem, null, null, iTerminalNode, iPrice);

            Patch();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
            Logger.LogInfo("  ╔═══════════════════════════════════════╗");
            Logger.LogInfo("  ║   Thanks for testing, brave souls!    ║");
            Logger.LogInfo("  ║      Your sacrifices are noted.       ║");
            Logger.LogInfo("  ╠═══════════════════════════════════════╣");
            Logger.LogInfo("  ║ ∙Digfish    ∙Kraven0004   ∙MacMeaties ║");
            Logger.LogInfo("  ║ ∙Ingleflats ∙NextGenPants             ║");
            Logger.LogInfo("  ╚═══════════════════════════════════════╝");            
        }

        internal static void Patch()
        {
            Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

            Logger.LogDebug("Patching...");

            Harmony.PatchAll();

            Logger.LogDebug("Finished patching!");
        }

        internal static void Unpatch()
        {
            Logger.LogDebug("Unpatching...");

            Harmony?.UnpatchSelf();

            Logger.LogDebug("Finished unpatching!");
        }
    }
}
