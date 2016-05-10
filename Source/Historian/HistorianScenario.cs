using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KSEA.Historian
{
    public class HistorianSettings : ScenarioModule
    {
        
        public string Layout { get; set; } = "";

        public string DefaultSpaceCenterName { get; set; } = "KSC";

        public bool EnableLauncherButton { get; set; } = true;

        public bool EnableToolbarButton { get; set; } = true;

        public string CustomText { get; set; } = "";

        public bool PersistentCustomText { get; set; } = false;

        public bool PersistentConfigurationWindow { get; set; } = false;
        static HistorianSettings Instance;

        public static HistorianSettings fetch
        {
            get
            {
                if (Instance == null)
                {
                    Instance = HighLogic.CurrentGame
                        .scenarios
                        .Select(s => s.moduleRef)
                        .OfType<HistorianSettings>()
                        .SingleOrDefault();
                }
                return Instance;
            }
        }

        public override void OnLoad(ConfigNode node)
        {
            Historian.Print("Reading persistence data");
            Layout = node.GetString("CurrentLayout", "Default");
            DefaultSpaceCenterName = node.GetString("DefaultSpaceCenterName", "KSC");
            EnableLauncherButton = node.GetBoolean("EnableLauncherButton", true);
            EnableToolbarButton = node.GetBoolean("EnableToolbarButton", true);
            CustomText = node.GetString("CustomText", "");
            PersistentCustomText = node.GetBoolean("PersistentCustomText", false);
            PersistentConfigurationWindow = node.GetBoolean("PersistentConfigurationWindow", false);

        }

        public override void OnSave(ConfigNode node)
        {
            Historian.Print("Saving persistence data");
            node.AddValue("CurrentLayout", Layout);
            node.AddValue("DefaultSpaceCenterName", DefaultSpaceCenterName);
            node.AddValue("EnableLauncherButton", EnableLauncherButton);
            node.AddValue("EnableToolbarButton", EnableToolbarButton);
            node.AddValue("CustomText", CustomText);
            node.AddValue("PersistentCustomText", PersistentCustomText);
            node.AddValue("PersistentConfigurationWindow", PersistentConfigurationWindow);
        }

        public static void CreateSettings(Game game)
        {
            if (!game.scenarios.Any(p => p.moduleName == typeof(HistorianSettings).Name))
            {
                ProtoScenarioModule proto = game.AddProtoScenarioModule(typeof(HistorianSettings), GameScenes.SPACECENTER, GameScenes.FLIGHT, GameScenes.EDITOR, GameScenes.TRACKSTATION);
                proto.Load(ScenarioRunner.fetch);
            }
        }
    }

    public class HistorianLoader
    {
        public static HistorianLoader Instance = null;

        public HistorianLoader()
        {
            GameEvents.onGameStateCreated.Add(onGameStateCreated);
        }

        void onGameStateCreated(Game game)
        {
            Historian.Print("Create game settings");
            HistorianSettings.CreateSettings(game);
        }
    }

    [KSPAddon(KSPAddon.Startup.MainMenu, false)]
    public class ScenarioSpawn : MonoBehaviour
    {
        void Start()
        {
            if ((object)HistorianLoader.Instance == null)
            {
                HistorianLoader.Instance = new HistorianLoader();
            }
            enabled = false;
        }
    }
}
