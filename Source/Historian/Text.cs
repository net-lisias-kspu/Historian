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
        static Dictionary<string, Font> createdFonts = new Dictionary<string, Font>();
        string fontName;

        static DefaultDateTimeFormatter dateFormatter = new DefaultDateTimeFormatter();

        static readonly Dictionary<string, Action<StringBuilder, CommonInfo>> parsers
            = new Dictionary<string, Action<StringBuilder, CommonInfo>>();

        readonly static string[] allTraits = { "Pilot", "Engineer", "Scientist", "Tourist" };

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
            var result = StringBuilderCache.Acquire();
            if (result == null)
                result = new StringBuilder();
            ParseIntoBuilder(result, text);
            return result.ToStringAndRelease();
        }

        protected void ParseIntoBuilder(StringBuilder result, string text)
        {

            // get common data sources
            var ut = Planetarium.GetUniversalTime();

            //var time = isKerbincalendar 
            //    ? dateFormatter.GetKerbinDateFromUT((int)ut) 
            //    : dateFormatter.GetEarthDateFromUT((int)ut);
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
                            // run parser for matching token - each action must append to the stringbuilder
                            parsers[token](result, info);
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


        }

        int GetTokenLength(string rawText, int pos) => rawText.IndexOf('>', pos) - pos - 1;

        #region Parsers

        void NewLineParser(StringBuilder result, CommonInfo info) => result.Append(Environment.NewLine);

        void CustomParser(StringBuilder result, CommonInfo info) => ParseIntoBuilder(result, Historian.Instance.GetConfiguration().CustomText.Replace("<Custom>", "")); // avoid recurssion.

        void DateFormatParser(StringBuilder result, CommonInfo info) => result.Append(info.DateFormat);

        void RealDateParser(StringBuilder result, CommonInfo info)
            => result.Append(DateTime.Now.ToString(info.DateFormat));

        void DateParser(StringBuilder result, CommonInfo info)
        {
            if (isKerbincalendar)
                result.Append(info.Time.FormattedDate(info.DateFormat, baseYear));
            else
                result.Append(new DateTime(info.Year + baseYear, 1, 1, info.Hour, info.Minute, info.Second).AddDays(info.Day - 1).ToString(info.DateFormat));
        }

        void DateParserKAC(StringBuilder result, CommonInfo info)
        {
            if (isKerbincalendar)
                result.Append(info.Time.FormattedDate(info.DateFormat, baseYear));
            else
                result.Append(new DateTime(baseYear, 1, 1).AddSeconds(info.UT).ToString(info.DateFormat));
        }

        void UTParser(StringBuilder result, CommonInfo info) => result.Append($"Y{info.Year + 1}, D{(info.Day):D3}, {info.Hour}:{info.Minute:D2}:{info.Second:D2}");

        void YearParser(StringBuilder result, CommonInfo info) => result.Append(info.Year + baseYear);

        void YearParserKAC(StringBuilder result, CommonInfo info)
        {
            if (isKerbincalendar)
                result.Append(info.Year + baseYear);
            else
                result.Append(new DateTime(baseYear, 1, 1).AddSeconds(info.UT).ToString("yyyy"));
        }

        void DayParser(StringBuilder result, CommonInfo info) => result.Append(info.Day);

        void DayParserKAC(StringBuilder result, CommonInfo info)
        {

            if (isKerbincalendar)
                result.Append(info.Day);
            else
                result.Append(new DateTime(baseYear, 1, 1).AddSeconds(info.UT).DayOfYear);
        }

        void HourParser(StringBuilder result, CommonInfo info) => result.Append(info.Hour);

        void MinuteParser(StringBuilder result, CommonInfo info) => result.Append(info.Minute);

        void SecondParser(StringBuilder result, CommonInfo info) => result.Append(info.Second);

        void METParser(StringBuilder result, CommonInfo info)
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

        void VesselParser(StringBuilder result, CommonInfo info) => result.Append(info.Vessel?.vesselName);

        void BodyParser(StringBuilder result, CommonInfo info)
        {
            if (info.Vessel != null) result.Append(Planetarium.fetch.CurrentMainBody.bodyName);
        }

        void SituationParser(StringBuilder result, CommonInfo info)
        {
            if (info.Vessel != null)
                result.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(info.Vessel.situation.ToString().Replace("_", "-").ToLower()));
        }

        void BiomeParser(StringBuilder result, CommonInfo info)
        {
            if (info.Vessel != null)
                result.Append( CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ScienceUtil.GetExperimentBiome(info.Vessel.mainBody, info.Vessel.latitude, info.Vessel.longitude).ToLower()));
        }

        void LandingZoneParser(StringBuilder result, CommonInfo info)
        {
            if (info.Vessel != null)
            {
                var landedAt = (string.IsNullOrEmpty(info.Vessel.landedAt))
                    ? ScienceUtil.GetExperimentBiome(info.Vessel.mainBody, info.Vessel.latitude, info.Vessel.longitude)
                    : Vessel.GetLandedAtString(info.Vessel.landedAt); // http://forum.kerbalspaceprogram.com/threads/123896-Human-Friendly-Landing-Zone-Title
                result.Append(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(landedAt.ToLower()));
            }
        }

        void LatitudeParser(StringBuilder result, CommonInfo info)
        {
            if (info.Vessel != null)
                result.Append(info.Vessel.latitude.ToString("F3"));
        }

        void LatitudeDMSParser(StringBuilder result, CommonInfo info)
        {
            if (info.Vessel != null)
            {
                result.AppendAngleAsDMS(info.Vessel.latitude);
                result.Append(info.Vessel.latitude > 0 ? " N" : " S");
            }

        }

        void LongitudeParser(StringBuilder result, CommonInfo info)
        {
            if (info.Vessel != null) 
                result.Append(ClampTo180(info.Vessel.longitude).ToString("F3"));
        }

        void LongitudeDMSParser(StringBuilder result, CommonInfo info)
        {
            if (info.Vessel != null)
            {
                var longitude = ClampTo180(info.Vessel.longitude);
                result.AppendAngleAsDMS(longitude);
                result.Append(longitude > 0 ? " E" : " W");
            }
        }

        void HeadingParser(StringBuilder result, CommonInfo info) => result.Append(FlightGlobals.ship_heading.ToString("F1"));

        void AltitudeParser(StringBuilder result, CommonInfo info)
        {
            if (info.Vessel != null)
                result.AppendDistance(info.Vessel.altitude);
        }

        void MachParser(StringBuilder result, CommonInfo info)
        {
            if (info.Vessel != null)
                result.Append(info.Vessel.mach.ToString("F1"));
        }

        void SpeedParser(StringBuilder result, CommonInfo info)
        {
            if (info.Vessel != null)
                result.AppendSpeed(info.Vessel.srfSpeed);
        }

        void SurfaceSpeedParser(StringBuilder result, CommonInfo info)
        {
            if (info.Vessel != null)
                result.AppendSpeed(info.Vessel.srfSpeed);
        }

        void OrbitalSpeedParser(StringBuilder result, CommonInfo info)
        {
            if (info.Orbit != null)
                result.AppendSpeed(info.Vessel.obt_speed);
        }

        void ApParser(StringBuilder result, CommonInfo info)
        {
            if (info.Orbit != null)
                result.AppendDistance(info.Orbit.ApA);
        }

        void PeParser(StringBuilder result, CommonInfo info)
        {
            if (info.Orbit != null)
                result.AppendDistance(info.Orbit.PeA);
        }


        void IncParser(StringBuilder result, CommonInfo info)
        {
            if (info.Orbit != null)
            {
                result.Append(info.Orbit.inclination.ToString("F2"));
                result.Append("°");
            }
        }

        void EccParser(StringBuilder result, CommonInfo info)
        {
            if (info.Orbit != null)
                result.Append(info.Orbit.eccentricity.ToString("F3"));
        }

        void LanParser(StringBuilder result, CommonInfo info)
        {
            if (info.Orbit != null)
            {
                result.Append(info.Orbit.LAN.ToString("F1"));
                result.Append("°");
            }
        }

        void ArgPeParser(StringBuilder result, CommonInfo info)
        {
            if (info.Orbit != null)
            {
                result.Append(info.Orbit.argumentOfPeriapsis.ToString("F1"));
                result.Append("°");
            }
        }

        void PeriodParser(StringBuilder result, CommonInfo info)
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

        void OrbitParser(StringBuilder result, CommonInfo info)
        {
            if (info.Orbit != null)
            {
                result.AppendDistance(info.Orbit.ApA);
                result.Append(" x ");
                result.AppendDistance(info.Orbit.PeA);
            }
        }

        void CrewParser(StringBuilder result, CommonInfo info)
             => GenericCrewParser(result, info.Vessel, isList: false, isShort: false, traits: allTraits, traitColours: info.TraitColours);

        void CrewShortParser(StringBuilder result, CommonInfo info)
            => GenericCrewParser(result, info.Vessel, isList: false, isShort: true, traits: allTraits, traitColours: info.TraitColours);

        void CrewListParser(StringBuilder result, CommonInfo info)
            => GenericCrewParser(result, info.Vessel, isList: true, isShort: false, traits: allTraits, traitColours: info.TraitColours);

        void PilotsParser(StringBuilder result, CommonInfo info)
            => GenericCrewParser(result, info.Vessel, isList: false, isShort: false, traits: new string[] { "Pilot" }, traitColours: info.TraitColours);

        void PilotsShortParser(StringBuilder result, CommonInfo info)
            => GenericCrewParser(result, info.Vessel, isList: false, isShort: true, traits: new string[] { "Pilot" }, traitColours: info.TraitColours);

        void PilotsListParser(StringBuilder result, CommonInfo info)
            => GenericCrewParser(result, info.Vessel, isList: true, isShort: false, traits: new string[] { "Pilot" }, traitColours: info.TraitColours);

        void EngineersParser(StringBuilder result, CommonInfo info)
            => GenericCrewParser(result, info.Vessel, isList: false, isShort: false, traits: new string[] { "Engineer" }, traitColours: info.TraitColours);

        void EngineersShortParser(StringBuilder result, CommonInfo info)
            => GenericCrewParser(result, info.Vessel, isList: false, isShort: true, traits: new string[] { "Engineer" }, traitColours: info.TraitColours);

        void EngineersListParser(StringBuilder result, CommonInfo info)
            => GenericCrewParser(result, info.Vessel, isList: true, isShort: false, traits: new string[] { "Engineer" }, traitColours: info.TraitColours);

        void ScientistsParser(StringBuilder result, CommonInfo info)
            => GenericCrewParser(result, info.Vessel, isList: false, isShort: false, traits: new string[] { "Scientist" }, traitColours: info.TraitColours);

        void ScientistsShortParser(StringBuilder result, CommonInfo info)
            => GenericCrewParser(result, info.Vessel, isList: false, isShort: true, traits: new string[] { "Scientist" }, traitColours: info.TraitColours);

        void ScientistsListParser(StringBuilder result, CommonInfo info)
            => GenericCrewParser(result, info.Vessel, isList: true, isShort: false, traits: new string[] { "Scientist" }, traitColours: info.TraitColours);

        void TouristsParser(StringBuilder result, CommonInfo info)
            => GenericCrewParser(result, info.Vessel, isList: false, isShort: false, traits: new string[] { "Tourist" }, traitColours: info.TraitColours);

        void TouristsShortParser(StringBuilder result, CommonInfo info)
            => GenericCrewParser(result, info.Vessel, isList: false, isShort: true, traits: new string[] { "Tourist" }, traitColours: info.TraitColours);

        void TouristsListParser(StringBuilder result, CommonInfo info)
            => GenericCrewParser(result, info.Vessel, isList: true, isShort: false, traits: new string[] { "Tourist" }, traitColours: info.TraitColours);

        void TargetParser(StringBuilder result, CommonInfo info)
        {
            if (info.Target != null)
                result.Append(info.Target.GetName());
        }

        void LaunchSiteParser(StringBuilder result, CommonInfo info)
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

        void ListFontsParser(StringBuilder result, CommonInfo info) => result.Append(string.Join(", ", OSFonts));

        void VesselTypeParser(StringBuilder result, CommonInfo info) => result.Append(info.Vessel?.vesselType);

        void StageNumberParser(StringBuilder result, CommonInfo info) => result.Append(info.Vessel?.currentStage);

        void LastActionParser(StringBuilder result, CommonInfo info) => result.Append(Historian.Instance.LastAction);

        void EvaStateParser(StringBuilder result, CommonInfo info)
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

        void KKSpaceCenterParser(StringBuilder result, CommonInfo info)
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

        void KKDistanceParser(StringBuilder result, CommonInfo info)
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

        void GenericCrewParser(StringBuilder result, Vessel vessel, bool isList, bool isShort, string[] traits, string[] traitColours)
        {
            if (vessel == null || vessel.isEVA || !vessel.isCommandable)
                return;

            var isSingleTrait = traits.Length == 1;

            Func<string, string> nameFilter = x => x;
            if (isShort) nameFilter = x => x.Replace(" Kerman", "");

            var crew = vessel.GetVesselCrew()
                .Where(c => traits.Contains(c.trait))
                .Select(c => TraitColor(c.trait, traitColours) + nameFilter(c.name) + "</color>")
                .ToArray();

            if (crew.Length <= 0)
            {
                result.Append(isSingleTrait ? "None" : "Unmanned");
                return;
            }

            if (isList)
            {
                result.Append("• ");
                result.Append(string.Join(Environment.NewLine + "• ", crew));
                return;
            }

            result.Append(string.Join(", ", crew));
            result.Append(isShort ? (isSingleTrait ? TraitColor(traits[0], traitColours) + " Kerman</color>" : " Kerman") : "");
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