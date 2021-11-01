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
using System;

namespace KSEA.Historian
{
    public enum ExtendedSituation
    {
        Default = 0,
        Landed = 1,
        Splashed = 2,
        Prelaunch = 4,
        Flying = 8,
        SubOrbital = 16,
        Orbiting = 32,
        Escaping = 64,
        Docked = 128,
        RagDolled = 256,
        Clambering = 512,
        OnLadder = 1024
    }

    public static class SituationExtensions
    {
        public static ExtendedSituation Extend(this Vessel.Situations? situation, TriState isEva, bool fallback)
        {
            if (!situation.HasValue)
                return ExtendedSituation.Default;

            if (isEva == TriState.True)
            {
                var kerbal = FlightGlobals.ActiveVessel.evaController;

                var ragDolled = kerbal.isRagdoll;
                var onLadder = kerbal.OnALadder;
                var clambering = kerbal.fsm.currentStateName.StartsWith("Clamber", StringComparison.InvariantCulture);

                // Historian.Print(kerbal.fsm.currentStateName);
                if (!fallback)
                {
                    if (ragDolled) return ExtendedSituation.RagDolled;

                    if (clambering) return ExtendedSituation.Clambering;

                    if (onLadder) return ExtendedSituation.OnLadder;
                }

            }

            if (!fallback)
                return (ExtendedSituation)situation.Value;

            return ExtendedSituation.Default;
        }
    }
}