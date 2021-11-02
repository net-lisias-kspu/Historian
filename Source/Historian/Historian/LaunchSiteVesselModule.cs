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

namespace KSEA.Historian
{
    public class LaunchSiteVesselModule : VesselModule
    {
        private const string kscDefaultName = "KSC";
        [KSPField(isPersistant = true)]
        public string LaunchSiteName = "";

        readonly string localisedLaunchPad = Localizer.GetStringByTag("#autoLOC_300898");
        readonly string localisedRunway = Localizer.GetStringByTag("#autoLOC_300899");


        public void FixedUpdate()
        {
            if (!HighLogic.LoadedSceneIsFlight || vessel == null || !vessel.loaded || vessel.situation != Vessel.Situations.PRELAUNCH)
                return;

            if (string.IsNullOrEmpty(LaunchSiteName))
            {
                string defaultSpaceCenter = Configuration.Instance.DefaultSpaceCenterName;
                string landedAt = "";

                try
                {
                    landedAt = (string.IsNullOrEmpty(vessel.landedAt))
                    ? ScienceUtil.GetExperimentBiomeLocalized(vessel.mainBody, vessel.latitude, vessel.longitude)
                    : Localizer.Format(vessel.displaylandedAt); // http://forum.kerbalspaceprogram.com/threads/123896-Human-Friendly-Landing-Zone-Title

                    if (landedAt == localisedLaunchPad || landedAt == localisedRunway)
                        landedAt = kscDefaultName;

                }
                catch (Exception ex)
                {
                    Historian.Print("Exception parsing landedAt");
                    Historian.Print(ex.Message);
                }

                string switcherSite = CheckKSCswitcher();
                if (switcherSite != kscDefaultName)
                    landedAt = switcherSite;

                string kkSite = CheckKerbalKonstructs();
                if (kkSite != kscDefaultName)
                    landedAt = kkSite;

                if (landedAt == kscDefaultName)
                    landedAt = defaultSpaceCenter;

                LaunchSiteName = landedAt;
            }
        }

        private string CheckKerbalKonstructs()
        {
            Type kkSiteManager = Historian.Instance.ReflectedClassType("kkLaunchSiteManager");
            string current = kscDefaultName;
            if (kkSiteManager != null)
            {
                try
                {
                    current = Reflect.GetStaticMethodResult(kkSiteManager, "getCurrentLaunchSite").ToString();
                    
                }
                catch (Exception ex)
                {
                    Historian.Print("Exception getting launchsite from Kerbal Konstructs");
                    Historian.Print(ex.Message);
                }
            }

            return current;
        }

        private static string CheckKSCswitcher()
        {
            Type switcher = Historian.Instance.ReflectedClassType("switcherLoader");
            string site = kscDefaultName;
            if (switcher != null)
            {
                try
                {
                    object instance = Reflect.GetStaticField(switcher, "instance");
                    object siteManager = Reflect.GetFieldValue(instance, "Sites");
                    string lastSite = (string)Reflect.GetFieldValue(siteManager, "lastSite");
                    ConfigNode node = (ConfigNode)Reflect.GetMethodResult(siteManager, "getSiteByName", lastSite);


                    if (node == null)
                        site = lastSite;
                    else
                        site = node.GetValue("displayName");

                }
                catch (Exception ex)
                {
                    Historian.Print("Exception getting launchsite from KSC Switcher");
                    Historian.Print(ex.Message);
                }
            }

            return site;
        }
    }
}
