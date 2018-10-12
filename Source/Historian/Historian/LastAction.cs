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