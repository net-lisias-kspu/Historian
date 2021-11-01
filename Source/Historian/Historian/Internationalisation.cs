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

namespace KSEA.Historian
{
    public static class Internationalisation
    {
        public const string KermanKey = "#autoLOC_289806";
        public static string Kerman = Localizer.GetStringByTag(KermanKey);

        public const string NKey = "#autoLOC_7003272";
        public static string North = Localizer.GetStringByTag(NKey);
        public const string SKey = "#autoLOC_7003273";
        public static string South = Localizer.GetStringByTag(SKey);
        public const string EKey = "#autoLOC_7003274";
        public static string East = Localizer.GetStringByTag(EKey);
        public const string WKey = "#autoLOC_7003275";
        public static string West = Localizer.GetStringByTag(WKey);

        // http://forum.kerbalspaceprogram.com/index.php?/topic/158018-addon-localization-home/&do=findComment&comment=3068733
        public static string LocalizeBodyName(this string input) 
        {
            return Localizer.Format("<<1>>", input);
        }
                
    }
}
