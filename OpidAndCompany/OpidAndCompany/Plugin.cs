using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using Discord;
using HarmonyLib;
using LobbyCompatibility.Attributes;
using LobbyCompatibility.Enums;
using OpidAndCompany.Behaviours;
using UnityEngine;

namespace OpidAndCompany
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, "1.0.5")]
    [BepInDependency("BMX.LobbyCompatibility", BepInDependency.DependencyFlags.HardDependency)]
    [LobbyCompatibility(CompatibilityLevel.ClientOnly, VersionStrictness.None)]
    [BepInDependency(LethalLib.Plugin.ModGUID)]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }
        public static AssetBundle opidsItems;

        // Configuration

        public static ConfigEntry<bool> SpawnCuteCeramicCritter;
        public static ConfigEntry<int> CuteCeramicCritterRarity;

        public static ConfigEntry<int> GlowStickIntensity;
        public static ConfigEntry<bool> ConfigSyncOthers;
        private static int GlowStickDefaultIntensity = 25;
        private static bool SpawnCuteCeramicCritterDefault = true;
        private static int CuteCeramicCritterRarityDefault = 30;

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            opidsItems = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, "opidsitems"));
            if (opidsItems == null)
            {
                Logger.LogError("Failed to load custom assets."); // ManualLogSource for your plugin
                return;
            }
            // Configuration            
            GlowStickIntensity = this.Config.Bind<int>("Purchasable Items", "Glow Stick Intensity", GlowStickDefaultIntensity, "[Client Side] An integer representing the brightness level of the glow stick’s light. Higher values result in a stronger glow.");
            SpawnCuteCeramicCritter = this.Config.Bind<bool>("Scrap", "Spawn Cute Ceramic Critter", SpawnCuteCeramicCritterDefault, "[Server Side] Determines if the Cute Ceramic Critter will spawn.");
            CuteCeramicCritterRarity = this.Config.Bind<int>("Scrap", "Cute Ceramic Critter Rarity", CuteCeramicCritterRarityDefault, "[Server Side] Controls the spawn rate of the Cute Ceramic Critter. Higher values increase its likelihood of spawning.");

            // 
            if (SpawnCuteCeramicCritter.Value)
            {                
                Logger.LogInfo($"Cute Ceramic Critter is set to spawn with a rarity of {CuteCeramicCritterRarity.Value}");
                Item cuteCeramicCritter = opidsItems.LoadAsset<Item>("assets/cuteceramiccritter.asset");
                LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(cuteCeramicCritter.spawnPrefab);
                LethalLib.Modules.Utilities.FixMixerGroups(cuteCeramicCritter.spawnPrefab);
                LethalLib.Modules.Items.RegisterScrap(cuteCeramicCritter, CuteCeramicCritterRarity.Value, LethalLib.Modules.Levels.LevelTypes.All);
            }            

            // Register Glow Stick
            int iPrice = 1;
            TerminalNode iTerminalNode = opidsItems.LoadAsset<TerminalNode>("Assets/iTerminalNode.asset");
            Item glowStickItem = opidsItems.LoadAsset<Item>("Assets/GlowStick.asset");
            Glowstick script = glowStickItem.spawnPrefab.AddComponent<Glowstick>();
            script.grabbable = true;
            script.grabbableToEnemies = true;
            script.itemProperties = glowStickItem;

            LethalLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(glowStickItem.spawnPrefab);
            LethalLib.Modules.Utilities.FixMixerGroups(glowStickItem.spawnPrefab);
            LethalLib.Modules.Items.RegisterShopItem(glowStickItem, null, null, iTerminalNode, iPrice);

            Patch();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
            Logger.LogInfo("  ╔══════════════════════════════════════════╗");
            Logger.LogInfo("  ║     Thanks for testing, brave souls!     ║");
            Logger.LogInfo("  ║        Your sacrifices are noted.        ║");
            Logger.LogInfo("  ╠══════════════════════════════════════════╣");
            Logger.LogInfo("  ║ ∙Digfish    ∙Kraven0004   ∙MacMeaties    ║");
            Logger.LogInfo("  ║ ∙Ingleflats ∙NextGenPants ∙Cirno The 9th ║");
            Logger.LogInfo("  ╚══════════════════════════════════════════╝");            
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
