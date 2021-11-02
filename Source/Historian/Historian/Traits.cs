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
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace KSEA.Historian
{
    public struct TraitInfo
    {
        public string Name;
        public string Label;
        public string Colour;
    }

    public struct LegacyTraitColors
    {
        public string PilotColor;
        public string EngineerColor;
        public string ScientistColor;
        public string TouristColor;
    }

    public static class TraitsLoader
    {
        public static Dictionary<string, TraitInfo> Load(ConfigNode node, LegacyTraitColors legacyColors)
        {
            Dictionary<string, TraitInfo> traits = new Dictionary<string, TraitInfo>();
            AddLegacyTraits(traits, legacyColors);

            // load crew traits from file
            string traitsConfigFileName = node.GetString("TRAITDEFINITIONS", "");
            if (!string.IsNullOrEmpty(traitsConfigFileName))
                traits = LoadFile(traits, traitsConfigFileName);

            // allow individual traits to be overwritten
            ConfigNode[] nodes = node.GetNodes("TRAIT");
            traits = LoadNodes(traits, nodes);

            return traits;
        }

		private static Dictionary<string, TraitInfo> LoadFile(Dictionary<string, TraitInfo> traits, string traitConfigFileName)
		{
			ConfigNode[] nodes = Configuration.Instance.LoadTraits(traitConfigFileName);
			return LoadNodes(traits, nodes);
		}

		private static Dictionary<string, TraitInfo> LoadNodes(Dictionary<string, TraitInfo> traits, ConfigNode[] nodes)
        {
            // Historian.Print($"Loading {nodes.Length} trait nodes");
            for (int i = 0; i < nodes.Length; i++)
            {
                string name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nodes[i].GetString("Name", "?").ToLower());
                string suffix = "(" + name.Substring(0, 1) + ")";

                if (traits.ContainsKey(name))
                {
                    // Historian.Print($"Update trait");
                    TraitInfo t = traits[name];
                    t.Label = nodes[i].GetString("Label", t.Label);
                    t.Colour = nodes[i].GetString("Color", t.Colour);
                    traits[name] = t;
                    t.Debug("Edit");
                }
                else
                {
                    // Historian.Print($"New trait");
                    TraitInfo t = new TraitInfo
                    {
                        Name = name,
                        Label = nodes[i].GetString("Label", suffix),
                        Colour = nodes[i].GetString("Color", "clear")
                    };
                    t.Debug("Add");
                    traits.Add(name, t);
                }
            }

            // Historian.Print($"Total traits = {traits.Count}");
            return traits;
        }


        private static void AddLegacyTraits(Dictionary<string, TraitInfo> traits, LegacyTraitColors legacyColors)
        {
            if (!traits.ContainsKey("Pilot"))
                traits.Add("Pilot", new TraitInfo {
                    Name = "Pilot",
                    Label = "(P)",
                    Colour = legacyColors.PilotColor
                });
            if (!traits.ContainsKey("Engineer"))
                traits.Add("Engineer", new TraitInfo {
                    Name = "Engineer",
                    Label = "(E)",
                    Colour = legacyColors.EngineerColor
                });
            if (!traits.ContainsKey("Scientist"))
                traits.Add("Scientist", new TraitInfo {
                    Name = "Scientist",
                    Label = "(S)",
                    Colour = legacyColors.ScientistColor
                });
            if (!traits.ContainsKey("Tourist"))
                traits.Add("Tourist", new TraitInfo {
                    Name = "Tourist",
                    Label = "(T)",
                    Colour = legacyColors.TouristColor
                });

            if (!traits.ContainsKey("Unknown"))
                traits.Add("Unknown", new TraitInfo {
                    Name = "Unknown",
                    Label = "(?)",
                    Colour = "clear"
                });
        }

        private static void Debug(this TraitInfo t, string addOrEdit)
        {
            Historian.Print($"{addOrEdit} trait: {t.Name}, {t.Label}, {t.Colour}");
        }
    }
}
