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
using System.Text;

namespace KSEA.Historian
{
    public static class KerbinDates
    {
#if DEBUG
        public static Configuration config;
#else
        public static Configuration config = Historian.Instance.GetConfiguration();
#endif

        public static string KerbinMonthName(this int dayNumber) => config.KerbinMonthNames[KerbinMonth(dayNumber) - 1];

        public static int KerbinMonth(this int dayNumber) => (int)Math.Floor((dayNumber - 0.01f) / 35.5) + 1;

        public static int[] FirstDayOfMonth = { 1, 36, 72, 107, 143, 178, 214, 249, 285, 320, 356, 391 };

        public static int KerbinDayOfMonth(this int dayNumber) => dayNumber - FirstDayOfMonth[KerbinMonth(dayNumber) - 1] + 1;

        public static int KerbinDayOfWeek(this int dayNumber) => ((dayNumber - 1) % 6) + 1;

        internal static int ParseRepeatPattern(String format, int pos, char patternChar)
        {
            int len = format.Length;
            int index = pos + 1;
            while ((index < len) && (format[index] == patternChar))
            {
                index++;
            }
            return (index - pos);
        }

        // var value = string.Format("Y{0}, D{1:D2}, {2}:{3:D2}:{4:D2}", time[4] + m_baseYear, time[3] + 1, time[2], time[1], time[0]);
        enum TimePart
        {
            Second = 0,
            Minute = 1,
            Hour = 2,
            Day = 3,
            Year = 4
        }

        public static string[] DigitFormat = { "", "0", "00", "000", "0000", "00000" };

        public static string FormattedDate(this int[] kerbinDate, string format, int baseYear = 1)
        {
            var result = new StringBuilder();

            int i = 0;
            int tokenLen;
            int kerbinDay = kerbinDate[(int)TimePart.Day] + 1;

            while (i < format.Length)
            {
                char ch = format[i];
                switch (ch)
                {
                    case 'h':
                    case 'H':
                        tokenLen = ParseRepeatPattern(format, i, ch);
                        result.Append(FormatDigits(kerbinDate[(int)TimePart.Hour], tokenLen, 1));
                        break;
                    case 'm':
                        tokenLen = ParseRepeatPattern(format, i, ch);
                        result.Append(FormatDigits(kerbinDate[(int)TimePart.Minute], tokenLen, 2));
                        break;
                    case 's':
                        tokenLen = ParseRepeatPattern(format, i, ch);
                        result.Append(FormatDigits(kerbinDate[(int)TimePart.Second], tokenLen, 2));
                        break;

                    case 'd':
                        //
                        // tokenLen == 1 : Day of month as digits with no leading zero.
                        // tokenLen == 2 : Day of month as digits with leading zero for single-digit months.
                        // tokenLen == 3 : Day of week as a three-leter abbreviation.
                        // tokenLen >= 4 : Day of week as its full name.
                        //
                        tokenLen = ParseRepeatPattern(format, i, ch);
                        if (tokenLen <= 2)
                        {
                            int day = kerbinDay.KerbinDayOfMonth();
                            result.Append(FormatDigits(day, tokenLen, 2));
                        }
                        else
                        {
                            int dayOfWeek = kerbinDay.KerbinDayOfWeek();
                            result.Append(FormatDayOfWeek(dayOfWeek, tokenLen));
                        }
                        break;
                    case 'M':
                        // 
                        // tokenLen == 1 : Month as digits with no leading zero.
                        // tokenLen == 2 : Month as digits with leading zero for single-digit months.
                        // tokenLen == 3 : Month as a three-letter abbreviation.
                        // tokenLen >= 4 : Month as its full name.
                        //
                        tokenLen = ParseRepeatPattern(format, i, ch);
                        int month = kerbinDay.KerbinMonth();
                        result.Append(FormatMonth(month, tokenLen));
                        break;
                    case 'y':
                        int year = kerbinDate[(int)TimePart.Year] + baseYear + 1;
                        tokenLen = ParseRepeatPattern(format, i, ch);
                        result.Append(FormatYear(year, tokenLen));
                        break;
                    case ':':
                    case '/':
                        result.Append(ch);
                        tokenLen = 1;
                        break;
                    default:
                        result.Append(ch);
                        tokenLen = 1;
                        break;
                }
                i += tokenLen;
            }
            return result.ToString();

        }

        static string FormatDigits(int value, int tokenLen, int maxPadding)
        {
            if (tokenLen > maxPadding)
                tokenLen = maxPadding;
            return value.ToString(DigitFormat[tokenLen]);
        }

        static string FormatYear(int year, int tokenLen) => year.ToString(new string('0', tokenLen));

        static string FormatMonth(int month, int tokenLen)
        {
            //var config = Historian.Instance.GetConfiguration();
            switch (tokenLen)
            {
                case 1:
                    return month.ToString();
                case 2:
                    return month.ToString("00");
                case 3:
                    return config.KerbinMonthNames[month - 1].Substring(0, 3);
                // case 4:
                default:
                    return config.KerbinMonthNames[month - 1];
            }
        }

        static string FormatDayOfWeek(int dayOfWeek, int tokenLen)
        {
            //var config = Historian.Instance.GetConfiguration();
            switch (tokenLen)
            {
                case 3:
                    return config.KerbinDayNames[dayOfWeek - 1].Substring(0, 3);
                default:
                    return config.KerbinDayNames[dayOfWeek - 1];
            }
        }
    }
}
