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
        public int Minute {  get { return Time[1]; } }
        public int Second { get { return Time[0]; } }

        public string[] TraitColours { get; set; }
        public string DateFormat { get; set; }
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
        int baseYear;
        string dateFormat = "dd MMM yyyy";
        bool isKerbincalendar;

        static string[] OSFonts = Font.GetOSInstalledFontNames();
        static Dictionary<string,Font> createdFonts = new Dictionary<string, Font>();
        string fontName;
        
        static DefaultDateTimeFormatter dateFormatter = new DefaultDateTimeFormatter();

        static readonly Dictionary<string, Func<CommonInfo, string>> parsers 
            = new Dictionary<string, Func<CommonInfo, string>>();

        readonly static string[] allTraits = { "Pilot", "Engineer", "Scientist", "Tourist" };

        public Text()
        {
            if (parsers.Count < 1)
                InitializeParameterDictionary();
        }

        public void SetText(string text) => this.text = text;

        protected override void OnDraw(Rect bounds)
        {
            var style = new GUIStyle(GUI.skin.label);

            style.alignment = textAnchor;
            style.normal.textColor = color;
            if (createdFonts.ContainsKey(fontName))
                style.font = createdFonts[fontName];
            style.fontSize = fontSize;
            style.fontStyle = fontStyle;
            style.richText = true;

            var content = new GUIContent();
            content.text = Parse(text);

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
            baseYear = node.GetInteger("BaseYear", isKerbincalendar ? 1 : 1951);
            dateFormat = node.GetString("DateFormat", "");
            if (string.IsNullOrEmpty(dateFormat))
                dateFormat = CultureInfo.CurrentUICulture.DateTimeFormat.LongDatePattern;
        }

        void InitializeParameterDictionary()
        {
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
        }

        protected string Parse(string text)
        {
            var result = new StringBuilder();

            // get common data sources
            var ut = Planetarium.GetUniversalTime();
            var time = isKerbincalendar 
                ? dateFormatter.GetKerbinDateFromUT((int)ut) 
                : dateFormatter.GetEarthDateFromUT((int)ut);
            var vessel = FlightGlobals.ActiveVessel;
            var orbit = vessel?.GetOrbit();
            var target = vessel?.targetObject;

            var info = new CommonInfo
            {
                Vessel = vessel,
                Orbit = orbit,
                Time = time,
                UT = ut,
                Target = target,
                TraitColours = new string[] { pilotColor, engineerColor, scientistColor, touristColor },
                DateFormat = dateFormat
            };

            // scan template text string for parameter tokens
            int i = 0, tokenLen;
            while (i < text.Length)
            {
                char ch = text[i];
                if (ch == '<')
                {
                    // possible token found
                    tokenLen = GetTokenLength(text, i);
                    if (tokenLen >= 0)
                    {
                        // extract token
                        var token = text.Substring(i + 1, tokenLen);
                        // check if recognised
                        if (parsers.ContainsKey(token))
                        {
                            // run parser for matching token
                            result.Append(parsers[token](info));
                        }
                        else
                        {
                            // token not found copy as literal
                            result.Append("<");
                            result.Append(token);
                            result.Append(">");
                        }
                        // include < and > in counted tokenlength
                        tokenLen += 2;
                    }
                    else
                    {
                        // no end token found treat as literal
                        tokenLen = 1;
                        result.Append(ch);
                    }
                }
                else
                {
                    // literal
                    tokenLen = 1;
                    result.Append(ch);
                }
                i += tokenLen;
            }

            var output = result.ToString();

            return output;
        }

        int GetTokenLength(string rawText, int pos) => rawText.IndexOf('>', pos) - pos - 1;

        #region Parsers

        string NewLineParser(CommonInfo info) => Environment.NewLine;

        string CustomParser(CommonInfo info) => Parse(Historian.Instance.GetConfiguration().CustomText.Replace("<Custom>", "")); // avoid recurssion.

        string DateFormatParser(CommonInfo info) => info.DateFormat;

        string RealDateParser(CommonInfo info)
            => DateTime.Now.ToString(info.DateFormat);

        string DateParser(CommonInfo info) 
            => isKerbincalendar
                ? info.Time.FormattedDate(info.DateFormat, baseYear)
                : new DateTime(info.Year + baseYear, 1, 1, info.Hour, info.Minute, info.Second).AddDays(info.Day - 1).ToString(info.DateFormat);

        string DateParserKAC(CommonInfo info)
            => isKerbincalendar
                ? info.Time.FormattedDate(info.DateFormat, baseYear)
                : new DateTime(baseYear, 1, 1).AddSeconds(info.UT).ToString(info.DateFormat);

        string UTParser(CommonInfo info) => $"Y{info.Year + 1}, D{(info.Day):D3}, {info.Hour}:{info.Minute:D2}:{info.Second:D2}";

        string YearParser(CommonInfo info) => (info.Year + baseYear).ToString();

        string YearParserKAC(CommonInfo info)
            => (isKerbincalendar)
                ? (info.Year + baseYear).ToString()
                : new DateTime(baseYear, 1, 1).AddSeconds(info.UT).ToString("yyyy");

        string DayParser(CommonInfo info) => info.Day.ToString();

        string DayParserKAC(CommonInfo info)
            => (isKerbincalendar)
                ? (info.Day.ToString())
                : new DateTime(baseYear, 1, 1).AddSeconds(info.UT).DayOfYear.ToString();

        string HourParser(CommonInfo info) => info.Hour.ToString();

        string MinuteParser(CommonInfo info) => info.Minute.ToString();

        string SecondParser(CommonInfo info) => info.Second.ToString();

        string METParser(CommonInfo info)
        {
            if (info.Vessel != null)
            {
                int[] t;
                if (isKerbincalendar)
                    t = dateFormatter.GetKerbinDateFromUT((int)info.Vessel.missionTime);
                else
                    t = dateFormatter.GetEarthDateFromUT((int)info.Vessel.missionTime);
                return (t[4] > 0)
                    ? $"T+ {t[4]}y, {t[3]}d, {t[2]:D2}:{t[1]:D2}:{t[0]:D2}"
                    : (t[3] > 0)
                        ? $"T+ {t[3]}d, {t[2]:D2}:{t[1]:D2}:{t[0]:D2}"
                        : $"T+ {t[2]:D2}:{t[1]:D2}:{t[0]:D2}";
            }
            return "";
        }

        string VesselParser(CommonInfo info) => info.Vessel?.vesselName;

        string BodyParser(CommonInfo info) => info.Vessel != null ? Planetarium.fetch.CurrentMainBody.bodyName : "";

        string SituationParser(CommonInfo info)
            => (info.Vessel == null) 
                    ? "" 
                    : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.Vessel.situation.ToString().Replace("_", "-").ToLower());

        string BiomeParser(CommonInfo info)
            => (info.Vessel == null) 
                    ? "" 
                    : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ScienceUtil.GetExperimentBiome(info.Vessel.mainBody, info.Vessel.latitude, info.Vessel.longitude).ToLower());

        string LandingZoneParser(CommonInfo info)
        {
            if (info.Vessel == null) return "";
            var landedAt = (string.IsNullOrEmpty(info.Vessel.landedAt))
                ? ScienceUtil.GetExperimentBiome(info.Vessel.mainBody, info.Vessel.latitude, info.Vessel.longitude)
                : Vessel.GetLandedAtString(info.Vessel.landedAt); // http://forum.kerbalspaceprogram.com/threads/123896-Human-Friendly-Landing-Zone-Title
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(landedAt.ToLower());
        }

        string LatitudeParser(CommonInfo info) => info.Vessel == null ? "" : info.Vessel.latitude.ToString("F3");

        string LatitudeDMSParser(CommonInfo info) 
            => info.Vessel == null ? "" : AngleToDMS(info.Vessel.latitude) + (info.Vessel.latitude > 0 ? " N" : " S");

        string LongitudeParser(CommonInfo info)
        {
            if (info.Vessel == null) return "";
            return ClampTo180(info.Vessel.longitude).ToString("F3");
        }

        string LongitudeDMSParser(CommonInfo info)
        {
            if (info.Vessel == null)return "";
            var longitude = ClampTo180(info.Vessel.longitude);
            return AngleToDMS(longitude) + (longitude > 0 ? " E" : " W");
        }

        string HeadingParser(CommonInfo info) => FlightGlobals.ship_heading.ToString("F1");

        string AltitudeParser(CommonInfo info) => info.Vessel == null ? "" : SimplifyDistance(info.Vessel.altitude);

        string MachParser(CommonInfo info) => info.Vessel == null ? "" : info.Vessel.mach.ToString("F1");

        string SpeedParser(CommonInfo info) => info.Vessel == null ? "" : SimplifyDistance(info.Vessel.srfSpeed) + @"/s";

        string SurfaceSpeedParser(CommonInfo info) => info.Vessel == null ? "" : SimplifyDistance(info.Vessel.srfSpeed) + @"/s";

        string OrbitalSpeedParser(CommonInfo info) => info.Orbit == null ? "" : SimplifyDistance(info.Vessel.obt_speed) + @"/s";

        string ApParser(CommonInfo info) => info.Orbit == null ? "" : SimplifyDistance(info.Orbit.ApA);

        string PeParser(CommonInfo info) => info.Orbit == null ? "" : SimplifyDistance(info.Orbit.PeA);

        string IncParser(CommonInfo info) => info.Orbit == null ? "" : info.Orbit.inclination.ToString("F2") + "°";

        string EccParser(CommonInfo info) => info.Orbit == null ? "" : info.Orbit.eccentricity.ToString("F3");

        string LanParser(CommonInfo info) => info.Orbit == null ? "" : info.Orbit.LAN.ToString("F1") + "°";

        string ArgPeParser(CommonInfo info) => info.Orbit == null ? "" : info.Orbit.argumentOfPeriapsis.ToString("F1") + "°";

        string PeriodParser(CommonInfo info)
        {
            if (info.Orbit == null)
                return "";

             var period = info.Orbit.period;
            var t = isKerbincalendar
                ? dateFormatter.GetKerbinDateFromUT((int)period)
                : dateFormatter.GetEarthDateFromUT((int)period);
            return (t[4] > 0)
                     ? $"{t[4] + 1}y, {t[3] + 1}d, {t[2]:D2}:{t[1]:D2}:{t[0]:D2}"
                     : (t[3] > 0)
                         ? $"{t[3] + 1}d, {t[2]:D2}:{t[1]:D2}:{t[0]:D2}"
                         : $"{t[2]:D2}:{t[1]:D2}:{t[0]:D2}";
        }

        string OrbitParser(CommonInfo info)
            => info.Orbit == null ? "" : $"{SimplifyDistance(info.Orbit.ApA)} x {SimplifyDistance(info.Orbit.PeA)}";

        string CrewParser(CommonInfo info)
            => GenericCrewParser(info.Vessel, isList: false, isShort: false, traits: allTraits, traitColours: info.TraitColours);

        string CrewShortParser(CommonInfo info)
            => GenericCrewParser(info.Vessel, isList: false, isShort: true, traits: allTraits, traitColours: info.TraitColours);

        string CrewListParser(CommonInfo info)
            => GenericCrewParser(info.Vessel, isList: true, isShort: false, traits: allTraits, traitColours: info.TraitColours);

        string PilotsParser(CommonInfo info)
            => GenericCrewParser(info.Vessel, isList: false, isShort: false, traits: new string[] { "Pilot" }, traitColours: info.TraitColours);

        string PilotsShortParser(CommonInfo info)
            => GenericCrewParser(info.Vessel, isList: false, isShort: true, traits: new string[] { "Pilot" }, traitColours: info.TraitColours);

        string PilotsListParser(CommonInfo info)
            => GenericCrewParser(info.Vessel, isList: true, isShort: false, traits: new string[] { "Pilot" }, traitColours: info.TraitColours);

        string EngineersParser(CommonInfo info)
            => GenericCrewParser(info.Vessel, isList: false, isShort: false, traits: new string[] { "Engineer" }, traitColours: info.TraitColours);

        string EngineersShortParser(CommonInfo info)
            => GenericCrewParser(info.Vessel, isList: false, isShort: true, traits: new string[] { "Engineer" }, traitColours: info.TraitColours);

        string EngineersListParser(CommonInfo info)
            => GenericCrewParser(info.Vessel, isList: true, isShort: false, traits: new string[] { "Engineer" }, traitColours: info.TraitColours);

        string ScientistsParser(CommonInfo info)
            => GenericCrewParser(info.Vessel, isList: false, isShort: false, traits: new string[] { "Scientist" }, traitColours: info.TraitColours);

        string ScientistsShortParser(CommonInfo info)
            => GenericCrewParser(info.Vessel, isList: false, isShort: true, traits: new string[] { "Scientist" }, traitColours: info.TraitColours);

        string ScientistsListParser(CommonInfo info)
            => GenericCrewParser(info.Vessel, isList: true, isShort: false, traits: new string[] { "Scientist" }, traitColours: info.TraitColours);

        string TouristsParser(CommonInfo info)
            => GenericCrewParser(info.Vessel, isList: false, isShort: false, traits: new string[] { "Tourist" }, traitColours: info.TraitColours);

        string TouristsShortParser(CommonInfo info)
            => GenericCrewParser(info.Vessel, isList: false, isShort: true, traits: new string[] { "Tourist" }, traitColours: info.TraitColours);

        string TouristsListParser(CommonInfo info)
            => GenericCrewParser(info.Vessel, isList: true, isShort: false, traits: new string[] { "Tourist" }, traitColours: info.TraitColours);

        string TargetParser(CommonInfo info) => info.Target == null ? "" : info.Target.GetName();

        string LaunchSiteParser(CommonInfo info)
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
                        return lastSite == "KSC" ? defaultSpaceCenter : lastSite;

                    var displayName = node.GetValue("displayName");
                    return displayName == "KSC" ? defaultSpaceCenter : displayName;
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
                    return current == "KSC" ? defaultSpaceCenter : current;
                }
                catch (Exception ex)
                {
                    Historian.Print("Exception getting launchsite from Kerbal Konstructs");
                }
            }
            return defaultSpaceCenter;
        }

        string ListFontsParser(CommonInfo info) => string.Join(", ", OSFonts);

        string VesselTypeParser(CommonInfo info) => info.Vessel?.vesselType.ToString();



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

        string KKSpaceCenterParser(CommonInfo info)
        {
            var scManager = Historian.Instance.ReflectedClassType("kkSpaceCenterManager");
            if (scManager == null)
                return "NO KK";
            if (info.Vessel == null)
                return "N/A";
            try {
                return GetKerbalKonstructsSpaceCenterInfo(info, scManager).BaseName;
            }
            catch (Exception ex)
            {
                return ex.Message + "\n" + ex.StackTrace;
            }
        }

        string KKDistanceParser(CommonInfo info)
        {
            var scManager = Historian.Instance.ReflectedClassType("kkSpaceCenterManager");
            if (scManager == null)
                return "NO KK";
            if (info.Vessel == null)
                return "";
            try
            {
                return SimplifyDistance(GetKerbalKonstructsSpaceCenterInfo(info, scManager).Distance);
            }
            catch (Exception ex)
            {
                return ex.Message + "\n" + ex.StackTrace;
            }
        }

        #endregion

        // ############# Helper functions

        string GenericCrewParser(Vessel vessel, bool isList, bool isShort, string[] traits, string[] traitColours)
        {
            if (vessel == null || vessel.isEVA || !vessel.isCommandable)
                return "";

            var isSingleTrait = traits.Length == 1;

            Func<string, string> nameFilter = x => x;
            if (isShort) nameFilter = x => x.Replace(" Kerman", "");

            var crew = vessel.GetVesselCrew()
                .Where(c => traits.Contains(c.trait))
                .Select(c => TraitColor(c.trait, traitColours) + nameFilter(c.name) + "</color>")
                .ToArray();

            if (crew.Length <= 0)
                return isSingleTrait ? "None" : "Unmanned";

            if (isList)
                return "• " + string.Join(Environment.NewLine + "• ", crew);

            return string.Join(", ", crew) + (isShort ? (isSingleTrait ? TraitColor(traits[0], traitColours) + " Kerman</color>" : " Kerman") : "");
        }


        string TraitColor(string trait, string[] traitColours)
        {
            switch (trait)
            {
                case "Pilot":
                    return "<color=" + traitColours[0] + ">";
                case "Engineer":
                    return "<color=" + traitColours[1] + ">";
                case "Scientist":
                    return "<color=" + traitColours[2] + ">";
                case "Tourist":
                    return "<color=" + traitColours[3] + ">";
                default:
                    return "<color=clear>";
            }
        }

        static readonly string[] m_units = { "m", "km", "Mm", "Gm", "Tm", "Pm" };

        protected static string SimplifyDistance(double meters)
        {
            double d = meters;
            int i = 0;

            while (d > 1000.0)
            {
                d /= 1000.0f;
                ++i;
            }

            return $"{d:F1} {m_units[i]}";
        }

        // AngleToDMS and ClanpTo180 converted from MechJeb at https://github.com/MuMech/MechJeb2/blob/master/MechJeb2/MuUtils.cs
        // and https://github.com/MuMech/MechJeb2/blob/master/MechJeb2/GuiUtils.cs
        public static string AngleToDMS(double angle)
        {
            var degrees = (int)Math.Floor(Math.Abs(angle));
            var minutes = (int)Math.Floor(60 * (Math.Abs(angle) - degrees));
            var seconds = (int)Math.Floor(3600 * (Math.Abs(angle) - degrees - minutes / 60.0));

            return $"{degrees:0}° {minutes:00}' {seconds:00}\"";
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
}