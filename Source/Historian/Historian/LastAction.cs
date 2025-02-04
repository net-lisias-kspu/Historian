﻿/*
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
namespace KSEA.Historian
{
    public enum LastAction
    {
        None,
        Abort,
        Stage,
        AG1,
        AG2,
        AG3,
        AG4,
        AG5,
        AG6,
        AG7,
        AG8,
        AG9,
        AG10,

        // for conditional checking
        Any,
        AnyOrNone
    }
}