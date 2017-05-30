/**
* This file is part of Historian.
* 
* Historian is free software: you can redistribute it and/or modify
* it under the terms of the GNU General Public License as published by
* the Free Software Foundation, either version 3 of the License, or
* (at your option) any later version.
* 
* Historian is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU General Public License for more details.
* 
* You should have received a copy of the GNU General Public License
* along with Historian. If not, see <http://www.gnu.org/licenses/>.
**/
using KSP.Localization;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace KSEA.Historian
{
    public struct TraitInfo
    {
        public string Name;
        public string DisplayName;
        public string Suffix;
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
        public static Dictionary<string, TraitInfo> Load(string traitConfigFileName, LegacyTraitColors legacyColors)
        {
            Dictionary<string, TraitInfo> traits = new Dictionary<string, TraitInfo>();

            var path = Path.Combine(Historian.Instance.ModDirectory, "Layouts");
            traitConfigFileName = Path.Combine(path, traitConfigFileName);
            if (!System.IO.File.Exists(traitConfigFileName))
            {
                Historian.Print($"ERROR: Unable to find traits config file '{traitConfigFileName}' in 'GameData/Historian/Layouts'");
                AddLegacyTraits(traits, legacyColors);
                return traits;
            }
            Historian.Print($"Loading traits from file {traitConfigFileName}");

            
            var nodes = ConfigNode.Load(traitConfigFileName).GetNodes("TRAIT");
            return Load(traits, nodes, legacyColors);
        }

        public static Dictionary<string, TraitInfo> Load(Dictionary<string, TraitInfo> traits, ConfigNode[] nodes, LegacyTraitColors legacyColors)
        {
            Historian.Print($"Loading {nodes.Length} trait nodes");
            for (int i = 0; i < nodes.Length; i++)
            {
                var name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nodes[i].GetString("Name", "Unknown").ToLower());
                var suffix = name.Substring(0, 1);

                if (traits.ContainsKey(name))
                {
                    Historian.Print($"Update trait {name}");
                    var t = traits[name];
                    t.DisplayName = nodes[i].GetString("DisplayName", t.DisplayName);
                    t.Suffix = nodes[i].GetString("Suffix", t.Suffix);
                    t.Colour = nodes[i].GetString("Color", t.Colour);
                }
                else
                {
                    Historian.Print($"New trait {name}");
                    var t = new TraitInfo
                    {
                        Name = name,
                        DisplayName = nodes[i].GetString("DisplayName", name),
                        Suffix = nodes[i].GetString("Suffix", suffix),
                        Colour = nodes[i].GetString("Color", "clear")
                    };
                    traits.Add(name, t);
                }
            }

            Historian.Print($"Total traits = {traits.Count}");
            AddLegacyTraits(traits, legacyColors);

            return traits;
        }


        private static void AddLegacyTraits(Dictionary<string, TraitInfo> traits, LegacyTraitColors legacyColors)
        {
            if (!traits.ContainsKey("Pilot"))
                traits.Add("Pilot", new TraitInfo {
                    Name = "Pilot",
                    DisplayName = Localizer.GetStringByTag("#autoLOC_500101"),
                    Suffix = "(P)",
                    Colour = legacyColors.PilotColor
                });
            if (!traits.ContainsKey("Engineer"))
                traits.Add("Engineer", new TraitInfo {
                    Name = "Engineer",
                    DisplayName = Localizer.GetStringByTag("#autoLOC_500103"),
                    Suffix = "(E)",
                    Colour = legacyColors.EngineerColor
                });
            if (!traits.ContainsKey("Scientist"))
                traits.Add("Scientist", new TraitInfo {
                    Name = "Scientist",
                    DisplayName = Localizer.GetStringByTag("#autoLOC_500105"),
                    Suffix = "(S)",
                    Colour = legacyColors.ScientistColor
                });
            if (!traits.ContainsKey("Tourist"))
                traits.Add("Tourist", new TraitInfo {
                    Name = "Tourist",
                    DisplayName = Localizer.GetStringByTag("#autoLOC_500107"),
                    Suffix = "(T)",
                    Colour = legacyColors.TouristColor
                });

            if (!traits.ContainsKey("Unknown"))
                traits.Add("Unknown", new TraitInfo {
                    Name = "Unknown",
                    DisplayName = "Unknown",
                    Suffix = "(?)",
                    Colour = "clear"
                });
        }
    }
}
