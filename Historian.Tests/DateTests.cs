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

using System.Diagnostics;
using KSEA.Historian;
using NUnit.Framework;

namespace Historian.Tests
{
    [TestFixture(Category = "Dates")]
    public class DateTests
    {
        [TestFixtureSetUp]
        public static void TestSetUp()
        {
            KerbinDates.config = Configuration.Defaults;
        }

        public static string[] KerbinMonthNames = { "Unnam", "Dosnam", "Trenam", "Cuatnam", "Cinqnam", "Seinam", "Sietnam", "Ocnam", "Nuevnam", "Diznam", "Oncnam", "Docenam" };

        [TestCase(1, "Unnam")]
        [TestCase(15, "Unnam")]
        [TestCase(35, "Unnam")]
        [TestCase(36, "Dosnam")]
        [TestCase(42, "Dosnam")]
        [TestCase(71, "Dosnam")]
        [TestCase(72, "Trenam")]
        [TestCase(426, "Docenam")]
        public void MonthNameTests(int input, string expected)
        {
            Assert.That(input.KerbinMonthName(), Is.EqualTo(expected));
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(15, 15)]
        [TestCase(30, 30)]
        [TestCase(35, 35)]
        [TestCase(36, 1)]
        [TestCase(71, 36)]
        [TestCase(72, 1)]
        [TestCase(106, 35)]
        [TestCase(107, 1)]
        [TestCase(142, 36)]
        [TestCase(143, 1)]
        [TestCase(177, 35)]
        [TestCase(178, 1)]
        [TestCase(213, 36)]
        [TestCase(214, 1)]
        [TestCase(248, 35)]
        [TestCase(249, 1)]
        [TestCase(284, 36)]
        [TestCase(285, 1)]
        [TestCase(319, 35)]
        [TestCase(320, 1)]
        [TestCase(355, 36)]
        [TestCase(356, 1)]
        [TestCase(390, 35)]
        [TestCase(391, 1)]
        [TestCase(426, 36)]
        public void DayOfMonthTests(int input, int expected)
        {
            Assert.That(input.KerbinDayOfMonth(), Is.EqualTo(expected));
        }

        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 4)]
        [TestCase(5, 5)]
        [TestCase(6, 6)]
        [TestCase(7, 1)]
        [TestCase(61, 1)]
        [TestCase(426, 6)]
        public void DayOfWeekTests(int input, int expected)
        {
            Assert.That(input.KerbinDayOfWeek(), Is.EqualTo(expected));
        }

        // s, m, h, d, y
        [TestCase(new int[] { 4, 3, 2, 76, 0 }, "y", "1")]
        [TestCase(new int[] { 4, 3, 2, 4, 0 }, "yyy", "001")]
        [TestCase(new int[] { 4, 3, 2, 4, 0 }, "d/M/y", "5/1/1")]
        [TestCase(new int[] { 4, 3, 2, 76, 0 }, "d/M/y", "6/3/1")]
        [TestCase(new int[] { 4, 3, 2, 76, 0 }, "dd/MM/yyy", "06/03/001")]
        [TestCase(new int[] { 4, 3, 2, 76, 0 }, "MM/dd/yyy", "03/06/001")]
        [TestCase(new int[] { 4, 3, 2, 76, 0 }, "dd MMMM yyy", "06 Trenam 001")]
        [TestCase(new int[] { 5, 4, 3, 1, 0 }, "h:m:s", "3:4:5")]
        [TestCase(new int[] { 5, 4, 3, 1, 0 }, "hh:mm:ss", "3:04:05")]
        [TestCase(new int[] { 5, 4, 3, 1, 0 }, "H:mm:ss", "3:04:05")]
        [TestCase(new int[] { 5, 4, 3, 76, 0 }, "h:mm:ss on dddd dd MMMM yyy", "3:04:05 on Esant 06 Trenam 001")]
        [TestCase(new int[] { 1, 1, 1, 0, 0 }, "MMM", "Unn")]
        [TestCase(new int[] { 1, 1, 1, 0, 0 }, "ddd", "Aka")]
        [TestCase(new int[] { 1, 1, 1, 6, 0 }, "dddd", "Akant")]
        [TestCase(new int[] { 1, 1, 1, 5, 0 }, "dddd", "Flant")]
        [TestCase(new int[] { 4, 3, 2, 0, 1 }, "y", "2")]
        [TestCase(new int[] { 4, 3, 2, 0, 1 }, "yyy", "002")]
        [TestCase(new int[] { 4, 3, 2, 0, 1 }, "d/M/y", "1/1/2")]
        [TestCase(new int[] { 4, 3, 2, 349, 0}, "dddd d MMMM yyy", "Brant 31 Diznam 001")]
        public void DateFormatTests(int[] input, string format, string expected)
        {
            Assert.That(input.FormattedDate(format, baseYear: 0), Is.EqualTo(expected));
        }

        [TestCase(100, "y", "1")]
        [TestCase(60, "d", "1")]
        public void DateFormatTestsUT(int ut, string format, string expected)
        {
            var input = new SplitDateTimeValue(ut).TimeParts;
            Debug.Write(ut);
            Debug.Write($"{input[0]} {input[1]} {input[2]} {input[3]} {input[4]}");
            Assert.That(input.FormattedDate(format, baseYear: 0), Is.EqualTo(expected));
        }

        [Test]
        public void DateSplitterTest()
        {
            var input = 21600 * 350 + 12345; // some time in Y1 D350
            Debug.Write(input);

            GameSettings.KERBIN_TIME = true;
            
            
            var result = new SplitDateTimeValue(input);
            Debug.Write($"Y:{SplitDateTimeValue.dateFormatter.Year}, D:{SplitDateTimeValue.dateFormatter.Day}, H:{SplitDateTimeValue.dateFormatter.Hour}");
            Debug.Write(SplitDateTimeValue.dateFormatter.PrintDate(input, true));
            Assert.That(result.Years, Is.EqualTo(0));
            Assert.That(result.Days, Is.EqualTo(350));
        }
    }
}
