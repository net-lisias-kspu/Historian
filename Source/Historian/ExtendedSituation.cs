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
using System.Collections.Generic;

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