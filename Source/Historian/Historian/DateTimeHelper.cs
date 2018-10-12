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
