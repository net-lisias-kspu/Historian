using KSP.Localization;

namespace KSEA.Historian
{
    public static class Internationalisation
    {
        public const string KermanKey = "#autoLOC_289806";
        public static string Kerman = Localizer.GetStringByTag(KermanKey);

        public const string NKey = "#autoLOC_7003272";
        public static string North = Localizer.GetStringByTag(NKey);
        public const string SKey = "#autoLOC_7003273";
        public static string South = Localizer.GetStringByTag(NKey);
        public const string EKey = "#autoLOC_7003274";
        public static string East = Localizer.GetStringByTag(NKey);
        public const string WKey = "#autoLOC_7003275";
        public static string West = Localizer.GetStringByTag(NKey);

        // http://forum.kerbalspaceprogram.com/index.php?/topic/158018-addon-localization-home/&do=findComment&comment=3068733
        public static string LocalizeBodyName(this string input) 
        {
            return Localizer.Format("<<1>>", input);
        }
                
    }
}
