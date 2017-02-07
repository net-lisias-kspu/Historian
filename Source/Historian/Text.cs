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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using static KSPUtil;

namespace KSEA.Historian
{

    public struct CommonInfo
    {
        public Vessel Vessel;
        public Orbit Orbit;
        public double UT;
        public int[] Time;
        public ITargetable Target;

        public int Year { get { return Time[4]; } }
        public int Day { get { return Time[3] + 1; } }
        public int Hour { get { return Time[2]; } }
        public int Minute { get { return Time[1]; } }
        public int Second { get { return Time[0]; } }

        public string DateFormat { get; set; }

        public Dictionary<string, TraitInfo> Traits { get; set; }
    }

    public struct TraitInfo
    {
        public string Name;
        public string Suffix;
        public string Colour;
    }



    public class Text : Element
    {
        const string DEFAULT_FONT_NAME = "NO DEFAULT";

        Color color = Color.white;
        string text = "";
        TextAnchor textAnchor = TextAnchor.MiddleCenter;
        int fontSize = 10;
        FontStyle fontStyle = FontStyle.Normal;
        string pilotColor, engineerColor, scientistColor, touristColor;
        Dictionary<string, TraitInfo> Traits = new Dictionary<string, TraitInfo>();
        int baseYear;
        string dateFormat = "dd MMM yyyy";
        bool isKerbincalendar;

        static string[] OSFonts = Font.GetOSInstalledFontNames();
        static Dictionary<string, Font> createdFonts = new Dictionary<string, Font>();
        string fontName;

        static DefaultDateTimeFormatter dateFormatter = new DefaultDateTimeFormatter();

        static readonly Dictionary<string, Action<StringBuilder, CommonInfo, string[]>> parsers
            = new Dictionary<string, Action<StringBuilder, CommonInfo, string[]>>();

        string[] allTraits;
        List<Token> TokenizedText;
        List<Token> TokenizedCustomText;

        public Text()
        {
            if (parsers.Count < 1)
                InitializeParameterDictionary();
        }

        public void SetText(string text) => this.text = text;

        protected override void OnDraw(Rect bounds)
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                alignment = textAnchor
            };
            style.normal.textColor = color;
            if (createdFonts.ContainsKey(fontName))
                style.font = createdFonts[fontName];
            style.fontSize = fontSize;
            style.fontStyle = fontStyle;
            style.richText = true;

            var content = new GUIContent()
            {
                text = Parse(text)
            };
            GUI.Label(bounds, content, style);
        }

        protected override void OnLoad(ConfigNode node)
        {
            color = node.GetColor("Color", Color.white);
            text = node.GetString("Text", "");
            textAnchor = node.GetEnum("TextAnchor", TextAnchor.MiddleCenter);
            fontName = node.GetString("Font", DEFAULT_FONT_NAME);
            if (!OSFonts.Contains(fontName) && fontName != DEFAULT_FONT_NAME)
            {
                Historian.Print($"The requested font '{fontName}' is not installed in your OS");
                fontName = DEFAULT_FONT_NAME;
            }
            else if (!createdFonts.ContainsKey(fontName))
                createdFonts.Add(fontName, Font.CreateDynamicFontFromOSFont(fontName, 12));
            fontSize = node.GetInteger("FontSize", 10);
            fontStyle = node.GetEnum("FontStyle", FontStyle.Normal);

            pilotColor = node.GetString("PilotColor", "clear");
            engineerColor = node.GetString("EngineerColor", "clear");
            scientistColor = node.GetString("ScientistColor", "clear");
            touristColor = node.GetString("TouristColor", "clear");

            isKerbincalendar = GameSettings.KERBIN_TIME;
            baseYear = node.GetInteger("BaseYear", isKerbincalendar ? 0 : 1951);
            dateFormat = node.GetString("DateFormat", "");
            if (string.IsNullOrEmpty(dateFormat))
                dateFormat = CultureInfo.CurrentUICulture.DateTimeFormat.LongDatePattern;
            // looks like this doesn't work properly - CultuireInfo.*.name always returns en-US in KSP during my testing

            Traits.Clear();
            var traits = node.GetNodes("TRAIT");
            for (int i = 0; i < traits.Length; i++)
            {
                var t = new TraitInfo();
                t.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(traits[i].GetString("Name", "UNKNOWN").ToLower());
                t.Suffix = traits[i].GetString("Suffix", t.Name.Substring(0, 1));
                t.Colour = traits[i].GetString("Color", "clear");
                Traits.Add(t.Name,t);
            }
            AddLegacyTraits();

            if (!Traits.ContainsKey("UNKNOWN"))
                Traits.Add("UNKNOWN", new TraitInfo { Name = "UNKNOWN", Suffix = "?", Colour = "clear" });

            allTraits = Traits.Select(t => t.Key).ToArray();

            // run parser over text to generate tokenized form
            TokenizedText = Parser.GetTokens(text);
            
        }

        private void AddLegacyTraits()
        {
            if (!Traits.ContainsKey("Pilot"))
                Traits.Add("Pilot", new TraitInfo { Name = "Pilot", Suffix = "P", Colour = pilotColor });
            if (!Traits.ContainsKey("Engineer"))
                Traits.Add("Engineer", new TraitInfo { Name = "Engineer", Suffix = "E", Colour = engineerColor });
            if (!Traits.ContainsKey("Scientist"))
                Traits.Add("Scientist", new TraitInfo { Name = "Scientist", Suffix = "S", Colour = scientistColor });
            if (!Traits.ContainsKey("Tourist"))
                Traits.Add("Tourist", new TraitInfo { Name = "Tourist", Suffix = "T", Colour = touristColor });
        }

        void InitializeParameterDictionary()
        {
            // each parser is an action that appends its result to the passed in stringbuilder
            parsers.Add("N", NewLineParser);
            parsers.Add("Custom", CustomParser);
            parsers.Add("Date", DateParser);
            parsers.Add("DateKAC", DateParserKAC);
            parsers.Add("UT", UTParser);
            parsers.Add("T+", METParser);
            parsers.Add("MET", METParser);
            parsers.Add("Year", YearParser);
            parsers.Add("YearKAC", YearParserKAC);
            parsers.Add("Day", DayParser);
            parsers.Add("DayKAC", DayParserKAC);
            parsers.Add("Hour", HourParser);
            parsers.Add("Minute", MinuteParser);
            parsers.Add("Second", SecondParser);
            parsers.Add("Vessel", VesselParser);
            parsers.Add("Body", BodyParser);
            parsers.Add("Biome", BiomeParser);
            parsers.Add("Situation", SituationParser);
            parsers.Add("LandingZone", LandingZoneParser);
            parsers.Add("Altitude", AltitudeParser);
            parsers.Add("Latitude", LatitudeParser);
            parsers.Add("LatitudeDMS", LatitudeDMSParser);
            parsers.Add("Longitude", LongitudeParser);
            parsers.Add("LongitudeDMS", LongitudeDMSParser);
            parsers.Add("Heading", HeadingParser);
            parsers.Add("Mach", MachParser);
            parsers.Add("Speed", SpeedParser);
            parsers.Add("SrfSpeed", SurfaceSpeedParser);
            parsers.Add("OrbSpeed", OrbitalSpeedParser);
            parsers.Add("Ap", ApParser);
            parsers.Add("Pe", PeParser);
            parsers.Add("Inc", IncParser);
            parsers.Add("Ecc", EccParser);
            parsers.Add("LAN", LanParser);
            parsers.Add("ArgPe", ArgPeParser);
            parsers.Add("Period", PeriodParser);
            parsers.Add("Orbit", OrbitParser);
            parsers.Add("Crew", CrewParser);
            parsers.Add("CrewShort", CrewShortParser);
            parsers.Add("CrewList", CrewListParser);
            parsers.Add("Pilots", PilotsParser);
            parsers.Add("PilotsShort", PilotsShortParser);
            parsers.Add("PilotsList", PilotsListParser);
            parsers.Add("Engineers", EngineersParser);
            parsers.Add("EngineersShort", EngineersShortParser);
            parsers.Add("EngineersList", EngineersListParser);
            parsers.Add("Scientists", ScientistsParser);
            parsers.Add("ScientistsShort", ScientistsShortParser);
            parsers.Add("ScientistsList", ScientistsListParser);
            parsers.Add("Tourists", TouristsParser);
            parsers.Add("TouristsShort", TouristsShortParser);
            parsers.Add("TouristsList", TouristsListParser);
            parsers.Add("Target", TargetParser);
            parsers.Add("LaunchSite", LaunchSiteParser);
            parsers.Add("RealDate", RealDateParser);
            parsers.Add("ListFonts", ListFontsParser);
            parsers.Add("VesselType", VesselTypeParser);
            parsers.Add("KK-Distance", KKDistanceParser);
            parsers.Add("KK-SpaceCenter", KKSpaceCenterParser);
            parsers.Add("DateFormat", DateFormatParser);

            parsers.Add("StageNumber", StageNumberParser);
            parsers.Add("LastAction", LastActionParser);
            parsers.Add("EvaState", EvaStateParser);
        }

        protected string Parse(string text)
        {
            

            // get common data sources
            var ut = Planetarium.GetUniversalTime();

            var vessel = FlightGlobals.ActiveVessel;
            var orbit = vessel?.GetOrbit();
            var target = vessel?.targetObject;

            var info = new CommonInfo
            {
                Vessel = vessel,
                Orbit = orbit,
                Time = new SplitDateTimeValue(ut).TimeParts,
                UT = ut,
                Target = target,
                DateFormat = dateFormat,
                Traits = Traits
            };

            var result = StringBuilderCache.Acquire();
            result.ExpandTokenizedText(TokenizedText, info, parsers, allowCustomTag: true);
            return result.ToStringAndRelease();
            //Token token;
            //for (int i = 0; i < TokenizedText.Count; i++)
            //{
            //    token = TokenizedText[i];
            //    if (token.IsLiteral)
            //    {
            //        result.Append(token.Key);
            //    }
            //    if (parsers.ContainsKey(TokenizedText[i].Key))
            //    {
            //        // run parser for matching token - each action must append to the stringbuilder
            //        parsers[tokens[i].Key](result, info, tokens[i].Args);
            //    }
            //    else
            //    {
            //        // token not found copy as literal
            //        result.Append("<");
            //        result.Append(tokens[i].Key);
            //        if (tokens[i].Args != null)
            //            result.Append("(").Append(tokens[i].Args).Append(")");
            //        result.Append(">");
            //    }
            //}

        }

        #region Parsers

        void NewLineParser(StringBuilder result, CommonInfo info, string[] args) => result.Append(Environment.NewLine);

        void CustomParser(StringBuilder result, CommonInfo info, string[] args)
        {
            var config = Historian.Instance.GetConfiguration();
            if (!string.IsNullOrEmpty(config.CustomText))
            {
                if (config.TokenizedCustomText == null)
                    config.TokenizedCustomText = Parser.GetTokens(config.CustomText);
                result.ExpandTokenizedText(config.TokenizedCustomText, info, parsers, allowCustomTag: false);
            }
        }

        void DateFormatParser(StringBuilder result, CommonInfo info, string[] args) => result.Append(info.DateFormat);

        void RealDateParser(StringBuilder result, CommonInfo info, string[] args)
            => result.Append(DateTime.Now.ToString(info.DateFormat));

        void DateParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (isKerbincalendar)
                result.Append(info.Time.FormattedDate(info.DateFormat, baseYear));
            else
                result.Append(new DateTime(info.Year + baseYear, 1, 1, info.Hour, info.Minute, info.Second).AddDays(info.Day - 1).ToString(info.DateFormat));
        }

        void DateParserKAC(StringBuilder result, CommonInfo info, string[] args)
        {
            if (isKerbincalendar)
                result.Append(info.Time.FormattedDate(info.DateFormat, baseYear));
            else
                result.Append(new DateTime(baseYear, 1, 1).AddSeconds(info.UT).ToString(info.DateFormat));
        }

        void UTParser(StringBuilder result, CommonInfo info, string[] args) => result.Append($"Y{info.Year + 1}, D{(info.Day):D3}, {info.Hour}:{info.Minute:D2}:{info.Second:D2}");

        void YearParser(StringBuilder result, CommonInfo info, string[] args) => result.Append(info.Year + ((isKerbincalendar) ? baseYear+1 : baseYear));

        void YearParserKAC(StringBuilder result, CommonInfo info, string[] args)
        {
            if (isKerbincalendar)
                result.Append(info.Year + baseYear);
            else
                result.Append(new DateTime(baseYear, 1, 1).AddSeconds(info.UT).ToString("yyyy"));
        }

        void DayParser(StringBuilder result, CommonInfo info, string[] args) => result.Append(info.Day);

        void DayParserKAC(StringBuilder result, CommonInfo info, string[] args)
        {

            if (isKerbincalendar)
                result.Append(info.Day);
            else
                result.Append(new DateTime(baseYear, 1, 1).AddSeconds(info.UT).DayOfYear);
        }

        void HourParser(StringBuilder result, CommonInfo info, string[] args) => result.Append(info.Hour);

        void MinuteParser(StringBuilder result, CommonInfo info, string[] args) => result.Append(info.Minute);

        void SecondParser(StringBuilder result, CommonInfo info, string[] args) => result.Append(info.Second);

        void METParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Vessel != null)
            {
                var t = new SplitDateTimeValue(info.Vessel.missionTime);
                if (t.Years > 0)
                    result.Append($"{t.Years + 1}y, {t.Days + 1}d, {t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}");
                else
                    if (t.Days > 0)
                        result.Append($"{t.Days + 1}d, {t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}");
                    else
                        result.Append($"{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}");

            }
        }

        void VesselParser(StringBuilder result, CommonInfo info, string[] args) => result.Append(info.Vessel?.vesselName);

        void BodyParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Vessel != null) result.Append(Planetarium.fetch.CurrentMainBody.bodyName);
        }

        void SituationParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Vessel != null)
                result.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.Vessel.situation.ToString().Replace("_", "-").ToLower()));
        }

        void BiomeParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Vessel != null)
                result.Append( CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ScienceUtil.GetExperimentBiome(info.Vessel.mainBody, info.Vessel.latitude, info.Vessel.longitude).ToLower()));
        }

        void LandingZoneParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Vessel != null)
            {
                var landedAt = (string.IsNullOrEmpty(info.Vessel.landedAt))
                    ? ScienceUtil.GetExperimentBiome(info.Vessel.mainBody, info.Vessel.latitude, info.Vessel.longitude)
                    : Vessel.GetLandedAtString(info.Vessel.landedAt); // http://forum.kerbalspaceprogram.com/threads/123896-Human-Friendly-Landing-Zone-Title
                result.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(landedAt.ToLower()));
            }
        }

        void LatitudeParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Vessel != null)
                result.Append(info.Vessel.latitude.ToString("F3"));
        }

        void LatitudeDMSParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Vessel != null)
            {
                result.AppendAngleAsDMS(info.Vessel.latitude);
                result.Append(info.Vessel.latitude > 0 ? " N" : " S");
            }

        }

        void LongitudeParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Vessel != null) 
                result.Append(ClampTo180(info.Vessel.longitude).ToString("F3"));
        }

        void LongitudeDMSParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Vessel != null)
            {
                var longitude = ClampTo180(info.Vessel.longitude);
                result.AppendAngleAsDMS(longitude);
                result.Append(longitude > 0 ? " E" : " W");
            }
        }

        void HeadingParser(StringBuilder result, CommonInfo info, string[] args) => result.Append(FlightGlobals.ship_heading.ToString("F1"));

        void AltitudeParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Vessel != null)
                result.AppendDistance(info.Vessel.altitude);
        }

        void MachParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Vessel != null)
                result.Append(info.Vessel.mach.ToString("F1"));
        }

        void SpeedParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Vessel != null)
                result.AppendSpeed(info.Vessel.srfSpeed);
        }

        void SurfaceSpeedParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Vessel != null)
                result.AppendSpeed(info.Vessel.srfSpeed);
        }

        void OrbitalSpeedParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Orbit != null)
                result.AppendSpeed(info.Vessel.obt_speed);
        }

        void ApParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Orbit != null)
                result.AppendDistance(info.Orbit.ApA);
        }

        void PeParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Orbit != null)
                result.AppendDistance(info.Orbit.PeA);
        }


        void IncParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Orbit != null)
            {
                result.Append(info.Orbit.inclination.ToString("F2"));
                result.Append("°");
            }
        }

        void EccParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Orbit != null)
                result.Append(info.Orbit.eccentricity.ToString("F3"));
        }

        void LanParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Orbit != null)
            {
                result.Append(info.Orbit.LAN.ToString("F1"));
                result.Append("°");
            }
        }

        void ArgPeParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Orbit != null)
            {
                result.Append(info.Orbit.argumentOfPeriapsis.ToString("F1"));
                result.Append("°");
            }
        }

        void PeriodParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Orbit != null)
            {

                var period = info.Orbit.period;
                var t = new SplitDateTimeValue(period);
                if (t.Years > 0)
                {
                    result.Append($"{t.Years + 1}y, {t.Days + 1}d, {t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}");
                }
                else {
                    if (t.Days > 0) 
                        result.Append($"{t.Days + 1}d, {t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}");
                    else
                        result.Append( $"{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}");
                }
            }
        }

        void OrbitParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Orbit != null)
            {
                result.AppendDistance(info.Orbit.ApA);
                result.Append(" x ");
                result.AppendDistance(info.Orbit.PeA);
            }
        }

        void CrewParser(StringBuilder result, CommonInfo info, string[] args)
             => GenericCrewParser(result, info.Vessel, isList: false, isShort: false, showSuffix:false, traitsFilter: allTraits, traitsInfo: info.Traits);

        void CrewShortParser(StringBuilder result, CommonInfo info, string[] args)
            => GenericCrewParser(result, info.Vessel, isList: false, isShort: true, showSuffix: false, traitsFilter: allTraits, traitsInfo: info.Traits);

        void CrewListParser(StringBuilder result, CommonInfo info, string[] args)
            => GenericCrewParser(result, info.Vessel, isList: true, isShort: false, showSuffix: false, traitsFilter: allTraits, traitsInfo: info.Traits);

        void PilotsParser(StringBuilder result, CommonInfo info, string[] args)
            => GenericCrewParser(result, info.Vessel, isList: false, isShort: false, showSuffix: false, traitsFilter: new string[] { "Pilot" }, traitsInfo: info.Traits);

        void PilotsShortParser(StringBuilder result, CommonInfo info, string[] args)
            => GenericCrewParser(result, info.Vessel, isList: false, isShort: true, showSuffix: false, traitsFilter: new string[] { "Pilot" }, traitsInfo: info.Traits);

        void PilotsListParser(StringBuilder result, CommonInfo info, string[] args)
            => GenericCrewParser(result, info.Vessel, isList: true, isShort: false, showSuffix: false, traitsFilter: new string[] { "Pilot" }, traitsInfo: info.Traits);

        void EngineersParser(StringBuilder result, CommonInfo info, string[] args)
            => GenericCrewParser(result, info.Vessel, isList: false, isShort: false, showSuffix: false, traitsFilter: new string[] { "Engineer" }, traitsInfo: info.Traits);

        void EngineersShortParser(StringBuilder result, CommonInfo info, string[] args)
            => GenericCrewParser(result, info.Vessel, isList: false, isShort: true, showSuffix: false, traitsFilter: new string[] { "Engineer" }, traitsInfo: info.Traits);

        void EngineersListParser(StringBuilder result, CommonInfo info, string[] args)
            => GenericCrewParser(result, info.Vessel, isList: true, isShort: false, showSuffix: false, traitsFilter: new string[] { "Engineer" }, traitsInfo: info.Traits);

        void ScientistsParser(StringBuilder result, CommonInfo info, string[] args)
            => GenericCrewParser(result, info.Vessel, isList: false, isShort: false, showSuffix: false, traitsFilter: new string[] { "Scientist" }, traitsInfo: info.Traits);

        void ScientistsShortParser(StringBuilder result, CommonInfo info, string[] args)
            => GenericCrewParser(result, info.Vessel, isList: false, isShort: true, showSuffix: false, traitsFilter: new string[] { "Scientist" }, traitsInfo: info.Traits);

        void ScientistsListParser(StringBuilder result, CommonInfo info, string[] args)
            => GenericCrewParser(result, info.Vessel, isList: true, isShort: false, showSuffix: false, traitsFilter: new string[] { "Scientist" }, traitsInfo: info.Traits);

        void TouristsParser(StringBuilder result, CommonInfo info, string[] args)
            => GenericCrewParser(result, info.Vessel, isList: false, isShort: false, showSuffix: false, traitsFilter: new string[] { "Tourist" }, traitsInfo: info.Traits);

        void TouristsShortParser(StringBuilder result, CommonInfo info, string[] args)
            => GenericCrewParser(result, info.Vessel, isList: false, isShort: true, showSuffix: false, traitsFilter: new string[] { "Tourist" }, traitsInfo: info.Traits);

        void TouristsListParser(StringBuilder result, CommonInfo info, string[] args)
            => GenericCrewParser(result, info.Vessel, isList: true, isShort: false, showSuffix: false, traitsFilter: new string[] { "Tourist" }, traitsInfo: info.Traits);

        void TargetParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Target != null)
                result.Append(info.Target.GetName());
        }

        void LaunchSiteParser(StringBuilder result, CommonInfo info, string[] args)
        {
            var defaultSpaceCenter = Historian.Instance.GetConfiguration().DefaultSpaceCenterName;
            var switcher = Historian.Instance.ReflectedClassType("switcherLoader");
            if (switcher != null)
            {
                try
                {
                    var instance = Reflect.GetStaticField(switcher, "instance");
                    var siteManager = Reflect.GetFieldValue(instance, "Sites");
                    var lastSite = (string)Reflect.GetFieldValue(siteManager, "lastSite");
                    var node = (ConfigNode)Reflect.GetMethodResult(siteManager, "getSiteByName", lastSite);

                    if (node == null)
                        result.Append(lastSite == "KSC" ? defaultSpaceCenter : lastSite);

                    var displayName = node.GetValue("displayName");
                    result.Append(displayName == "KSC" ? defaultSpaceCenter : displayName);
                }
                catch
                {
                    Historian.Print("Exception getting launchsite from KSC Switcher");
                }
            }

            var kkSiteManager = Historian.Instance.ReflectedClassType("kkLaunchSiteManager");
            if (kkSiteManager != null && info.Vessel != null)
            {
                try
                {
                    var current = Reflect.GetStaticMethodResult(kkSiteManager, "getCurrentLaunchSite").ToString();
                    result.Append(current == "KSC" ? defaultSpaceCenter : current);
                }
                catch (Exception ex)
                {
                    Historian.Print("Exception getting launchsite from Kerbal Konstructs");
                    Historian.Print(ex.Message);
                }
            }
            result.Append(defaultSpaceCenter);
        }

        void ListFontsParser(StringBuilder result, CommonInfo info, string[] args) => result.Append(string.Join(", ", OSFonts));

        void VesselTypeParser(StringBuilder result, CommonInfo info, string[] args) => result.Append(info.Vessel?.vesselType);

        void StageNumberParser(StringBuilder result, CommonInfo info, string[] args) => result.Append(info.Vessel?.currentStage);

        void LastActionParser(StringBuilder result, CommonInfo info, string[] args) => result.Append(Historian.Instance.LastAction);

        void EvaStateParser(StringBuilder result, CommonInfo info, string[] args)
        {
            if (info.Vessel == null || !info.Vessel.isEVA)
                return;
            try
            {
                if (info.Vessel.evaController.JetpackDeployed)
                    result.Append( "Jetpack ");
                if (info.Vessel.evaController.JetpackIsThrusting)
                    result.Append("thrusting ");
                result.Append(info.Vessel.evaController.fsm.currentStateName);
            }
            catch (Exception e)
            {
                result.Append("Error: ");
                result.Append(e.Message);
            }

        }

        class KerbalKonstructsInfo
        {
            public SpaceCenter SpaceCenter;
            public float Distance;
            public string BaseName;
        }

        KerbalKonstructsInfo GetKerbalKonstructsSpaceCenterInfo(CommonInfo info, Type scManager)
        {
            Vector3 position = info.Vessel.gameObject.transform.position;
            SpaceCenter closestCenter = null;
            float distance = 1000.0f;
            float recoveryFactor = 0.0f;
            float recoveryRange = 0.0f;
            string baseName = null;
            var args = new object[] { position, closestCenter, distance, recoveryFactor, recoveryRange, baseName };
            Reflect.StaticVoidMethod(scManager, "getClosestSpaceCenter", args);
            var kkInfo = new KerbalKonstructsInfo
            {
                SpaceCenter = (SpaceCenter)args[1],
                Distance = (float)args[2],
                BaseName = args[5].ToString()
            };
            if (kkInfo.BaseName == "KSC")
                kkInfo.BaseName = Historian.Instance.GetConfiguration().DefaultSpaceCenterName;
            return kkInfo;
        }

        void KKSpaceCenterParser(StringBuilder result, CommonInfo info, string[] args)
        {
            var scManager = Historian.Instance.ReflectedClassType("kkSpaceCenterManager");
            if (scManager == null)
            {
                result.Append("NO KK");
                return;
            }
            if (info.Vessel == null)
            {
                result.Append("N/A");
                return;
            }
            try
            {
                result.Append(GetKerbalKonstructsSpaceCenterInfo(info, scManager).BaseName);
            }
            catch (Exception ex)
            {
                result.AppendLine(ex.Message);
                result.Append(ex.StackTrace);
            }
        }

        void KKDistanceParser(StringBuilder result, CommonInfo info, string[] args)
        {
            var scManager = Historian.Instance.ReflectedClassType("kkSpaceCenterManager");
            if (scManager == null)
            {
                result.Append("NO KK");
                return;
            }
            if (info.Vessel == null)
                return;
            try
            {
               result.AppendDistance(GetKerbalKonstructsSpaceCenterInfo(info, scManager).Distance);
            }
            catch (Exception ex)
            {
                result.AppendLine(ex.Message);
                result.Append(ex.StackTrace);
            }
        }

        #endregion

        // ############# Helper functions

        void GenericCrewParser(StringBuilder result, Vessel vessel, bool isList, bool isShort, bool showSuffix, string[] traitsFilter, Dictionary<string, TraitInfo> traitsInfo)
        {
            if (vessel == null || vessel.isEVA || !vessel.isCommandable)
                return;

            var isSingleTrait = traitsFilter.Length == 1;

            var crewCount = 0;
            var crew = vessel.GetVesselCrew();
            TraitInfo trait;

            for (int i = 0; i < crew.Count; i++)
            {
                var crewMember = crew[i];
                // allow filter to be either singular or plural
                if (traitsFilter.Contains(crewMember.trait) || traitsFilter.Contains(crewMember.trait + "s"))
                {
                    crewCount++;
                    if (isList)
                        result.Append("• ");
                    else
                    {
                        if (crewCount > 1)
                            result.Append(", ");
                    }

                    trait = traitsInfo["UNKNOWN"];
                    if (traitsInfo.ContainsKey(crewMember.trait))
                        trait = traitsInfo[crewMember.trait];

                    result.AppendTraitColor(trait.Name, traitsInfo);
                    if (isShort)
                        result.Append(crewMember.name.Replace(" Kerman", ""));
                    else
                        result.Append(crewMember.name);

                    if (showSuffix)
                        result.Append(" (").Append(trait.Suffix).Append(")");
                    result.Append("</color>");

                    if (isList)
                        result.AppendLine();
                }
            }
            if (crewCount == 0)
            {
                var cfg = Historian.Instance.GetConfiguration();
                result.Append(isSingleTrait ? cfg.DefaultNoCrewLabel : cfg.DefaultUnmannedLabel);
            }
            else
                if (isShort)
                {
                    if (isSingleTrait)
                        result.AppendTraitColor(traitsFilter[0], traitsInfo).Append(" Kerman</color>");
                    else
                        result.Append(" Kerman");
                }


            //var crew = vessel.GetVesselCrew()
            //    .Where(c => traitsFilter.Contains(c.trait))
            //    .Select(c => TraitColor(c.trait, traitColours) + nameFilter(c.name) + "</color>")
            //    .ToArray();

            //if (crew.Length <= 0)
            //{
            //    var cfg = Historian.Instance.GetConfiguration();
            //    result.Append(isSingleTrait ? cfg.DefaultNoCrewLabel : cfg.DefaultUnmannedLabel);
            //    return;
            //}

            //if (isList)
            //{
            //    result.Append("• ");
            //    result.Append(string.Join(Environment.NewLine + "• ", crew));
            //    return;
            //}

            //result.Append(string.Join(", ", crew));
            //result.Append(isShort ? (isSingleTrait ? TraitColor(traitsFilter[0], traitColours) + " Kerman</color>" : " Kerman") : "");
        }

        public static double ClampTo180(double angle)
        {
            // clamp to 360 then 180
            angle = angle % 360.0;
            if (angle < 0) angle += 360.0;
            if (angle > 180) angle -= 360;
            return angle;
        }

    }

    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendTraitColor(this StringBuilder sb, string trait, Dictionary<string, TraitInfo> traits)
        {
            sb.Append("<color=");
            if (traits.ContainsKey(trait))
                sb.Append(traits[trait].Colour);
            else
                sb.Append("clear");
            sb.Append(">");
            return sb;
        }
    }


}