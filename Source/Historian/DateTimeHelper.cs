using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static KSPUtil;

namespace KSEA.Historian
{
    public class SplitDateTimeValue
    {
        public static DefaultDateTimeFormatter dateFormatter = new DefaultDateTimeFormatter();
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
            this.Years = (int)ut / dateFormatter.Year;
            this.Days = (int)ut % dateTimeFormatter.Year / dateFormatter.Day;
            this.Hours = (int)ut % dateTimeFormatter.Day / dateTimeFormatter.Hour;
            this.Minutes = (int)ut % dateTimeFormatter.Hour / dateTimeFormatter.Minute;
            this.Seconds = (int)ut % dateTimeFormatter.Minute;
        }
    }
}
