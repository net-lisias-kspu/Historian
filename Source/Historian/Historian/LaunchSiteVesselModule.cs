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
                var defaultSpaceCenter = Historian.Instance.GetConfiguration().DefaultSpaceCenterName;
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

                var switcherSite = CheckKSCswitcher();
                if (switcherSite != kscDefaultName)
                    landedAt = switcherSite;

                var kkSite = CheckKerbalKonstructs();
                if (kkSite != kscDefaultName)
                    landedAt = kkSite;

                if (landedAt == kscDefaultName)
                    landedAt = defaultSpaceCenter;

                LaunchSiteName = landedAt;
            }
        }

        private string CheckKerbalKonstructs()
        {
            var kkSiteManager = Historian.Instance.ReflectedClassType("kkLaunchSiteManager");
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
            var switcher = Historian.Instance.ReflectedClassType("switcherLoader");
            string site = kscDefaultName;
            if (switcher != null)
            {
                try
                {
                    var instance = Reflect.GetStaticField(switcher, "instance");
                    var siteManager = Reflect.GetFieldValue(instance, "Sites");
                    var lastSite = (string)Reflect.GetFieldValue(siteManager, "lastSite");
                    var node = (ConfigNode)Reflect.GetMethodResult(siteManager, "getSiteByName", lastSite);


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
