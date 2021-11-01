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
using static KSPUtil;

namespace KSEA.Historian
{
    public class SplitDateTimeValue
    {
        // public static DefaultDateTimeFormatter dateFormatter = new DefaultDateTimeFormatter();
        public int[] TimeParts = new int[] { 0, 0, 0, 0, 0 };
        public int Years
        {
            get { return TimeParts[4]; }
            private set { TimeParts[4] = value; }
        }
        public int Days
        {
            get { return TimeParts[3]; }
            private set { TimeParts[3] = value; }
        }
        public int Hours
        {
            get { return TimeParts[2]; }
            private set { TimeParts[2] = value; }
        }
        public int Minutes
        {
            get { return TimeParts[1]; }
            private set { TimeParts[1] = value; }
        }
        public int Seconds
        {
            get { return TimeParts[0]; }
            private set { TimeParts[0] = value; }
        }

        public SplitDateTimeValue(double ut)
        {
            this.Years = (int)ut / dateTimeFormatter.Year;
            this.Days = (int)ut % dateTimeFormatter.Year / dateTimeFormatter.Day;
            this.Hours = (int)ut % dateTimeFormatter.Day / dateTimeFormatter.Hour;
            this.Minutes = (int)ut % dateTimeFormatter.Hour / dateTimeFormatter.Minute;
            this.Seconds = (int)ut % dateTimeFormatter.Minute;
        }
    }
}
