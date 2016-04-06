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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
    }

    public class Text : Element
    {
        Color m_Color = Color.white;
        string m_Text = "";
        TextAnchor m_TextAnchor = TextAnchor.MiddleCenter;
        int m_FontSize = 10;
        FontStyle m_FontStyle = FontStyle.Normal;
        string m_pilotColor, m_engineerColor, m_scientistColor, m_touristColor;
        int m_baseYear;
        string m_dateFormat;
        bool m_isKerbincalendar;

        static DefaultDateTimeFormatter m_dateFormatter = new DefaultDateTimeFormatter();

        static readonly Dictionary<string, Func<CommonInfo, string>> m_parsers = new Dictionary<string, Func<CommonInfo, string>>();

        readonly static string[] m_AllTraits = { "Pilot", "Engineer", "Scientist", "Tourist" };

        public Text()
        {
            if (m_parsers.Count < 1)
                InitializeParameterDictionary();
        }

        public void SetText(string text)
        {
            m_Text = text;
        }

        protected override void OnDraw(Rect bounds)
        {
            var style = new GUIStyle(GUI.skin.label);

            style.alignment = m_TextAnchor;
            style.normal.textColor = m_Color;
            style.fontSize = m_FontSize;
            style.fontStyle = m_FontStyle;
            style.richText = true;

            var content = new GUIContent();
            content.text = Parse(m_Text);

            GUI.Label(bounds, content, style);
        }

        protected override void OnLoad(ConfigNode node)
        {
            m_Color = node.GetColor("Color", Color.white);
            m_Text = node.GetString("Text", "");
            m_TextAnchor = node.GetEnum("TextAnchor", TextAnchor.MiddleCenter);
            m_FontSize = node.GetInteger("FontSize", 10);
            m_FontStyle = node.GetEnum("FontStyle", FontStyle.Normal);

            m_pilotColor = node.GetString("PilotColor", "clear");
            m_engineerColor = node.GetString("EngineerColor", "clear");
            m_scientistColor = node.GetString("ScientistColor", "clear");
            m_touristColor = node.GetString("TouristColor", "clear");

            m_isKerbincalendar = GameSettings.KERBIN_TIME;
            m_baseYear = node.GetInteger("BaseYear", m_isKerbincalendar ? 1 : 1951);
            m_dateFormat = node.GetString("DateFormat", CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern);
        }

        void InitializeParameterDictionary()
        {
            m_parsers.Add("N", NewLineParser);
            m_parsers.Add("Custom", CustomParser);
            m_parsers.Add("Date", DateParser);
            m_parsers.Add("DateKAC", DateParserKAC);
            m_parsers.Add("UT", UTParser);
            m_parsers.Add("T+", METParser);
            m_parsers.Add("MET", METParser);
            m_parsers.Add("Year", YearParser);
            m_parsers.Add("YearKAC", YearParserKAC);
            m_parsers.Add("Day", DayParser);
            m_parsers.Add("DayKAC", DayParserKAC);
            m_parsers.Add("Hour", HourParser);
            m_parsers.Add("Minute", MinuteParser);
            m_parsers.Add("Second", SecondParser);
            m_parsers.Add("Vessel", VesselParser);
            m_parsers.Add("Body", BodyParser);
            m_parsers.Add("Biome", BiomeParser);
            m_parsers.Add("Situation", SituationParser);
            m_parsers.Add("LandingZone", LandingZoneParser);
            m_parsers.Add("Altitude", AltitudeParser);
            m_parsers.Add("Latitude", LatitudeParser);
            m_parsers.Add("LatitudeDMS", LatitudeDMSParser);
            m_parsers.Add("Longitude", LongitudeParser);
            m_parsers.Add("LongitudeDMS", LongitudeDMSParser);
            m_parsers.Add("Heading", HeadingParser);
            m_parsers.Add("Mach", MachParser);
            m_parsers.Add("Speed", SpeedParser);
            m_parsers.Add("SrfSpeed", SurfaceSpeedParser);
            m_parsers.Add("OrbSpeed", OrbitalSpeedParser);
            m_parsers.Add("Ap", ApParser);
            m_parsers.Add("Pe", PeParser);
            m_parsers.Add("Inc", IncParser);
            m_parsers.Add("Ecc", EccParser);
            m_parsers.Add("LAN", LanParser);
            m_parsers.Add("ArgPe", ArgPeParser);
            m_parsers.Add("Period", PeriodParser);
            m_parsers.Add("Orbit", OrbitParser);
            m_parsers.Add("Crew", CrewParser);
            m_parsers.Add("CrewShort", CrewShortParser);
            m_parsers.Add("CrewList", CrewListParser);
            m_parsers.Add("Pilots", PilotsParser);
            m_parsers.Add("PilotsShort", PilotsShortParser);
            m_parsers.Add("PilotsList", PilotsListParser);
            m_parsers.Add("Engineers", EngineersParser);
            m_parsers.Add("EngineersShort", EngineersShortParser);
            m_parsers.Add("EngineersList", EngineersListParser);
            m_parsers.Add("Scientists", ScientistsParser);
            m_parsers.Add("ScientistsShort", ScientistsShortParser);
            m_parsers.Add("ScientistsList", ScientistsListParser);
            m_parsers.Add("Tourists", TouristsParser);
            m_parsers.Add("TouristsShort", TouristsShortParser);
            m_parsers.Add("TouristsList", TouristsListParser);
            m_parsers.Add("Target", TargetParser);
            m_parsers.Add("LaunchSite", LaunchSiteParser);
        }

        protected string Parse(string text)
        {
            var result = new StringBuilder();

            // get common data sources
            var ut = Planetarium.GetUniversalTime();
            var time = m_isKerbincalendar 
                ? m_dateFormatter.GetKerbinDateFromUT((int)ut) 
                : m_dateFormatter.GetEarthDateFromUT((int)ut);
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
                TraitColours = new string[] { m_pilotColor, m_engineerColor, m_scientistColor, m_touristColor }
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
                        if (m_parsers.ContainsKey(token))
                        {
                            // run parser for matching token
                            result.Append(m_parsers[token](info));
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

        int GetTokenLength(string text, int pos)
        {
            return text.IndexOf('>', pos) - pos - 1;
        }

        #region Parsers

        string NewLineParser(CommonInfo info) => Environment.NewLine;

        string CustomParser(CommonInfo info) => Parse(Historian.Instance.GetConfiguration().CustomText.Replace("<Custom>", "")); // avoid recurssion.

        string DateParser(CommonInfo info) 
            => m_isKerbincalendar
                ? info.Time.FormattedDate(m_dateFormat, m_baseYear)
                : new DateTime(info.Year + m_baseYear, 1, 1, info.Hour, info.Minute, info.Second).AddDays(info.Day - 1).ToString(m_dateFormat);

        string DateParserKAC(CommonInfo info)
            => m_isKerbincalendar
                ? info.Time.FormattedDate(m_dateFormat, m_baseYear)
                : new DateTime(m_baseYear, 1, 1).AddSeconds(info.UT).ToString(m_dateFormat);

        string UTParser(CommonInfo info) => $"Y{info.Year + 1}, D{(info.Day):D3}, {info.Hour}:{info.Minute:D2}:{info.Second:D2}";

        string YearParser(CommonInfo info) => (info.Year + m_baseYear).ToString();

        string YearParserKAC(CommonInfo info)
            => (m_isKerbincalendar)
                ? (info.Year + m_baseYear).ToString()
                : new DateTime(m_baseYear, 1, 1).AddSeconds(info.UT).ToString("yyyy");

        string DayParser(CommonInfo info) => info.Day.ToString();

        string DayParserKAC(CommonInfo info)
            => (m_isKerbincalendar)
                ? (info.Day.ToString())
                : new DateTime(m_baseYear, 1, 1).AddSeconds(info.UT).DayOfYear.ToString();

        string HourParser(CommonInfo info) => info.Hour.ToString();

        string MinuteParser(CommonInfo info) => info.Minute.ToString();

        string SecondParser(CommonInfo info) => info.Second.ToString();

        string METParser(CommonInfo info)
        {
            if (info.Vessel != null)
            {
                int[] t;
                if (m_isKerbincalendar)
                    t = m_dateFormatter.GetKerbinDateFromUT((int)info.Vessel.missionTime);
                else
                    t = m_dateFormatter.GetEarthDateFromUT((int)info.Vessel.missionTime);
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
            var t = m_isKerbincalendar
                ? m_dateFormatter.GetKerbinDateFromUT((int)period)
                : m_dateFormatter.GetEarthDateFromUT((int)period);
            return (t[4] > 0)
                     ? $"{t[4] + 1}y, {t[3] + 1}d, {t[2]:D2}:{t[1]:D2}:{t[0]:D2}"
                     : (t[3] > 0)
                         ? $"{t[3] + 1}d, {t[2]:D2}:{t[1]:D2}:{t[0]:D2}"
                         : $"{t[2]:D2}:{t[1]:D2}:{t[0]:D2}";
        }

        string OrbitParser(CommonInfo info)
            => info.Orbit == null ? "" : $"{SimplifyDistance(info.Orbit.ApA)} x {SimplifyDistance(info.Orbit.PeA)}";

        string CrewParser(CommonInfo info)
            => GenericCrewParser(info.Vessel, isList: false, isShort: false, traits: m_AllTraits, traitColours: info.TraitColours);

        string CrewShortParser(CommonInfo info)
            => GenericCrewParser(info.Vessel, isList: false, isShort: true, traits: m_AllTraits, traitColours: info.TraitColours);

        string CrewListParser(CommonInfo info)
            => GenericCrewParser(info.Vessel, isList: true, isShort: false, traits: m_AllTraits, traitColours: info.TraitColours);

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
            var switcher = Historian.Instance.KscSwitcherLoader;
            if (switcher == null) return "KSC";
            try
            {
                var instance = Reflect.GetStaticField(switcher, "instance");
                var siteManager = Reflect.GetFieldValue(instance, "Sites");
                var lastSite = (string)Reflect.GetFieldValue(siteManager, "lastSite");
                var node = (ConfigNode)Reflect.GetMethodResult(siteManager, "getSiteByName", lastSite);

                if (node == null)
                    return lastSite;

                return node.GetValue("displayName");
            }
            catch
            {
                Historian.Print("Exception getting launchsite");
                return "ERROR";
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