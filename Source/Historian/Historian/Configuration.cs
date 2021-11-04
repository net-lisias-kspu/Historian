/*
	This file is part of Historian /L Unleashed
		© 2018-2021 Lisias T : http://lisias.net <support@lisias.net>
		© 2016-2018 Aelfhe1n
		© 2015-2016 Zeenobit

	Historian /L Unleashed is licensed as follows:
		* GPL 3.0 : https://www.gnu.org/licenses/gpl-3.0.txt

	Historian /L Unleashed is distributed in the hope that it will be
	useful, but WITHOUT ANY WARRANTY; without even the implied
	warranty of	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU General Public License 3.0 along
	with Historian /L Unleashed. If not, see <https://www.gnu.org/licenses/>.

*/
using KSP.Localization;
using System;
using System.Collections.Generic;

using PLUGINDATA = KSPe.IO.File<KSEA.Historian.Startup>.Asset;
using USERDATA = KSPe.IO.File<KSEA.Historian.Startup>.Data;
using CONFIGNODE = KSPe.IO.Data<KSEA.Historian.Startup>.ConfigNode;
using DEFAULTLAYOUTS = KSPe.IO.Asset<KSEA.Historian.Startup>;
using USERLAYOUTS = KSPe.IO.Data<KSEA.Historian.Startup>;
using Directory = KSPe.IO.Directory;
using Path = KSPe.IO.Path;

namespace KSEA.Historian
{
    public enum RightClickAction
    {
        None = 0,
        Suppress,
        AlwaysActive,
        AutoHideUI
    }

    public class Configuration
    {
		private const string LAYOUTS_DIR = "Layouts";
		private const string LAYOUTS_MASK = "*.layout";

		private static readonly CONFIGNODE HISTORIANCFG = CONFIGNODE.For("KSEA_HISTORIAN_CONFIGURATION", "Historian.cfg");

		static readonly System.Version CurrentVersion = new System.Version(Version.Number);

		private static Configuration instance = null;
		internal static Configuration Instance => instance ?? (instance = new Configuration());
		internal static void Set(Configuration configuration)
		{
			instance = configuration;
			instance.Save();
		}

		// defaults
		public static readonly Configuration Defaults = new Configuration {
            Layout = "Default",
            EnableLauncherButton = true,
            EnableToolbarButton = true,
            CustomText = "",
            PersistentConfigurationWindow = false,
            PersistentCustomText = false,
            AutoHideUI = true,
            TimeToRememberLastAction = 2000, // 2000ms = 2s
            DefaultSpaceCenterName = Localizer.GetStringByTag("#autoLOC_300900"), // #autoLOC_300900 = KSC
            KerbinMonthNames = new string[] { "Unnam", "Dosnam", "Trenam", "Cuatnam", "Cinqnam", "Seinam", "Sietnam", "Ocnam", "Nuevnam", "Diznam", "Oncnam", "Docenam" },
            KerbinDayNames = new string[] { "Akant", "Brant", "Casant", "Dovant", "Esant", "Flant" },
            RightClickAction = RightClickAction.Suppress,
            DefaultNoCrewLabel = Localizer.GetStringByTag("#autoLOC_258911"), // #autoLOC_258911 = none
            DefaultUnmannedLabel = Localizer.GetStringByTag("#autoLOC_286382") // #autoLOC_286382 = unmanned
        };

        private Configuration(bool fromDefaults = false)
        {
            if (fromDefaults)
            {
                Historian.Print("Creating from defaults");
                this.Layout = Defaults.Layout;
                this.EnableLauncherButton = Defaults.EnableLauncherButton;
                this.EnableToolbarButton = Defaults.EnableToolbarButton;
                this.CustomText = Defaults.CustomText;
                this.PersistentConfigurationWindow = Defaults.PersistentConfigurationWindow;
                this.PersistentCustomText = Defaults.PersistentCustomText;
                this.AutoHideUI = Defaults.AutoHideUI;
                this.TimeToRememberLastAction = Defaults.TimeToRememberLastAction;
                this.DefaultSpaceCenterName = Defaults.DefaultSpaceCenterName;
                this.KerbinMonthNames = (string[])Defaults.KerbinMonthNames.Clone();
                this.KerbinDayNames = (string[])Defaults.KerbinDayNames.Clone();
                this.RightClickAction = Defaults.RightClickAction;
                this.DefaultNoCrewLabel = Defaults.DefaultNoCrewLabel;
                this.DefaultUnmannedLabel = Defaults.DefaultUnmannedLabel;
            }
        }

		public string Layout { get; set; }

        public string DefaultSpaceCenterName { get; set; }

        public bool EnableLauncherButton { get; set; }

        public bool EnableToolbarButton { get; set; }

        public string CustomText { get; set; }

        public bool PersistentCustomText { get; set; }

        public bool PersistentConfigurationWindow { get; set; }

        public int TimeToRememberLastAction { get; set; }

        public bool AutoHideUI { get; set; }

        public RightClickAction RightClickAction { get; set; }

        public string DefaultNoCrewLabel { get; set; }

        public string DefaultUnmannedLabel { get; set; }

        public string[] KerbinMonthNames;
        public string[] KerbinDayNames;

        public List<Token> TokenizedCustomText;

        public static void Load()
        {
			try
			{
				// Hack. I should had implemented a Directory<T> thingy already...
				Directory.CreateDirectory(LayoutsDirectoryUser);
				Log.dbg("{0} is assured to exist.", LayoutsDirectoryUser);
			}
			catch (Exception e)
			{
				Log.err("Could not initialise user data on {0} due {1}. Things will not work as expected!", LayoutsDirectoryUser, e.Message);
			}

            try
            {
                HISTORIANCFG.Clear();
                ConfigNode node = (HISTORIANCFG.IsLoadable ? HISTORIANCFG.Load().Node : HISTORIANCFG.Node);
                Configuration configuration = new Configuration();

                System.Version version = node.GetVersion("Version", new System.Version());

                configuration.Layout 
                    = node.GetString("Layout", Defaults.Layout);
                configuration.EnableLauncherButton 
                    = node.GetBoolean("EnableLauncherButton", Defaults.EnableLauncherButton);
                configuration.EnableToolbarButton 
                    = node.GetBoolean("EnableToolbarButton", Defaults.EnableToolbarButton);
                configuration.AutoHideUI
                    = node.GetBoolean("AutoHideUI", Defaults.AutoHideUI);
                configuration.CustomText 
                    = node.GetString("CustomText", Defaults.CustomText);
                configuration.DefaultSpaceCenterName 
                    = node.GetString("DefaultSpaceCenterName", Defaults.DefaultSpaceCenterName);
                configuration.PersistentCustomText 
                    = node.GetBoolean("PersistentCustomText", Defaults.PersistentCustomText);
                configuration.PersistentConfigurationWindow 
                    = node.GetBoolean("PersistentConfigurationWindow", Defaults.PersistentConfigurationWindow);
                configuration.TimeToRememberLastAction 
                    = node.GetInteger("TimeToRememberLastAction", Defaults.TimeToRememberLastAction);
                configuration.KerbinDayNames
                    = node.TryReadStringArray("KerbinDayNames", Defaults.KerbinDayNames);
                configuration.KerbinMonthNames
                    = node.TryReadStringArray("KerbinMonthNames", Defaults.KerbinMonthNames);
                configuration.RightClickAction
                    = node.GetEnum("RightClickAction", RightClickAction.Suppress);
                configuration.DefaultNoCrewLabel
                    = node.GetString("DefaultNoCrewLabel", Defaults.DefaultNoCrewLabel);
                configuration.DefaultUnmannedLabel
                    = node.GetString("DefaultUnmannedLabel", Defaults.DefaultUnmannedLabel);

                if (!String.IsNullOrEmpty(configuration.CustomText))
                    configuration.TokenizedCustomText = Parser.GetTokens(configuration.CustomText);

                if (version != CurrentVersion)
                {
                    configuration.Save();
                }

                instance = configuration;
            }
            catch
            {
                Historian.Print($"Failed to load configuration file '{HISTORIANCFG}'. Attempting recovery ...");

                // ensure save directory exists.
                string dir = HISTORIANCFG.KspPath;
                Directory.CreateDirectory(Path.Combine(dir, LAYOUTS_DIR));

                Historian.Print("Creating configuration from default values");
                HISTORIANCFG.Clear();
                Configuration configuration = new Configuration(fromDefaults: true);
                Historian.Print("Saving configuration file");
                configuration.Save();

                instance = configuration;
            }
        }

        public void Save()
        {
            try
            {
                ConfigNode node = HISTORIANCFG.Node;

                node.AddValue("Version", CurrentVersion.ToString());
                node.AddValue("Layout", Layout);
                node.AddValue("EnableLauncherButton", EnableLauncherButton);
                node.AddValue("EnableToolbarButton", EnableToolbarButton);
                node.AddValue("AutoHideUI", AutoHideUI);
                node.AddValue("CustomText", CustomText);
                node.AddValue("PersistentCustomText", PersistentCustomText);
                node.AddValue("PersistentConfigurationWindow", PersistentConfigurationWindow);
                node.AddValue("DefaultSpaceCenterName", DefaultSpaceCenterName);
                node.AddValue("TimeToRememberLastAction", TimeToRememberLastAction);
                node.AddValue("KerbinDayNames", string.Join(";", KerbinDayNames));
                node.AddValue("KerbinMonthNames", string.Join(";", KerbinMonthNames));
                node.AddValue("RightClickAction", RightClickAction);
                node.AddValue("DefaultNoCrewLabel", DefaultNoCrewLabel);
                node.AddValue("DefaultUnmannedLabel", DefaultUnmannedLabel);

                HISTORIANCFG.Save();

                Historian.Print($"Configuration saved at '{HISTORIANCFG.KspPath}'.");
            }
            catch
            {
                Historian.Print($"Failed to save configuration file '{HISTORIANCFG}'.");
            }
        }

		internal void LoadLayouts(List<Layout> layouts)
		{
			Log.trace("Searching for layouts ...");
			// Another leaked abstraction. I should not be handling unabstracted file names anymore!
			this.LoadLayouts(layouts, LayoutsDirectoryDefault, DEFAULTLAYOUTS.ConfigNode.ListFor(LAYOUTS_MASK, false, LAYOUTS_DIR));
			this.LoadLayouts(layouts, LayoutsDirectoryUser, USERLAYOUTS.ConfigNode.ListFor(LAYOUTS_MASK, false, LAYOUTS_DIR));
		}

		internal void LoadLayouts(List<Layout> layouts, string dir, string[] files)
		{
			foreach (string file in files)
				LoadLayout(file, dir, layouts);
		}

		private void LoadLayout(string file, string dir, List<Layout> layouts)
		{
			string layoutName = Path.GetFileNameWithoutExtension(file);
			file = Path.Combine(dir, file); // Another leaked abstraction. I should not be handling unabstracted file names anymore!
			try
			{
				ConfigNode node = ConfigNode.Load(file).GetNode("KSEA_HISTORIAN_LAYOUT");

				if (layouts.FindIndex(layout => layout.Name == layoutName) < 0)
				{
					Layout layout = KSEA.Historian.Layout.Load(layoutName, node);
					layouts.Add(layout);

					Log.trace("Found layout '{0}'.", layoutName);
				}
				else
				{
					Log.warn("Layout with name '{0}' already exists. Unable to load duplicate.", layoutName);
				}
			}
			catch (Exception e)
			{
				Log.err("Failed to load layout '{0}' due {1}.", layoutName, e.Message);
			}
		}

		// Hack. I should had implemented a Directory<T> thingy already...
		private static readonly string LayoutsDirectoryDefault = PLUGINDATA.Solve(LAYOUTS_DIR);
		private static readonly string LayoutsDirectoryUser = USERDATA.Solve(LAYOUTS_DIR);
		internal ConfigNode[] LoadTraits(string traitConfigFileName)
		{
			ConfigNode[] r = this.LoadTraits(Configuration.LayoutsDirectoryUser, traitConfigFileName);
			r = r ?? this.LoadTraits(Configuration.LayoutsDirectoryDefault, traitConfigFileName);
			if (null == r)
			{
				Historian.Print($"ERROR: Unable to find traits config file '{traitConfigFileName}' from GameData neither user's PluginData");
				r = new ConfigNode[0];
			}
			return r;
		}

		private ConfigNode[] LoadTraits(string dir, string traitConfigFileName)
		{
			// Leaked abstraction. I should not be using System.IO and real pathnames here...
			traitConfigFileName = Path.Combine(dir, traitConfigFileName);
			if (!System.IO.File.Exists(traitConfigFileName)) 
				return null;

			Historian.Print($"Loading traits from '{traitConfigFileName}'");
			return ConfigNode.Load(traitConfigFileName).GetNodes("TRAIT");
		}

	}
}