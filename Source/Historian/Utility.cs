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
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KSEA.Historian
{
    public enum TriState
    {
        UseDefault = -2,
        True = -1,
        False = 0,

        // UseDefault synonyms
        Either = -2,
        Any = -2,
        Always = -2
    }

    public static class Extensions
    {
        public static bool? ToNullableBoolean(this TriState self)
        {
            switch (self)
            {
                case TriState.True:
                    return true;
                case TriState.False:
                    return false;
                default:
                    return null;
            }
        }

        public static TriState ToTriState(this bool? self)
        {
            if (self.HasValue)
            {
                return self.Value ? TriState.True : TriState.False;
            }
            return TriState.UseDefault;
        }

        static readonly string[] m_units = { "m", "km", "Mm", "Gm", "Tm", "Pm" };

        public static void AppendDistance(this StringBuilder result, double meters)
        {
            double d = meters;
            int i = 0;

            while (d > 1000.0)
            {
                d /= 1000.0f;
                ++i;
            }

            result.Append(d.ToString("F1"));
            result.Append(" ");
            result.Append(m_units[i]);
        }

        public static void AppendSpeed(this StringBuilder result, double speed)
        {
            result.AppendDistance(speed);
            result.Append(@"/s");
        }

        // AngleToDMS and ClanpTo180 converted from MechJeb at https://github.com/MuMech/MechJeb2/blob/master/MechJeb2/MuUtils.cs
        // and https://github.com/MuMech/MechJeb2/blob/master/MechJeb2/GuiUtils.cs
        public static void AppendAngleAsDMS(this StringBuilder result, double angle)
        {
            if (result == null)
                Historian.Print("Result is null");
            var degrees = (int)Math.Floor(Math.Abs(angle));
            var minutes = (int)Math.Floor(60 * (Math.Abs(angle) - degrees));
            var seconds = (int)Math.Floor(3600 * (Math.Abs(angle) - degrees - minutes / 60.0));

            result.Append($"{degrees:0}° {minutes:00}' {seconds:00}\"");
        }
    }

    public static class Reflect
    {
        public static Type GetExternalType(string typeName) 
            => AssemblyLoader.loadedAssemblies
                .SelectMany(a => a.assembly.GetExportedTypes())
                .SingleOrDefault(t => t.FullName == typeName);

        public static object GetStaticField(Type type, string fieldName)
            => type.GetField(fieldName, BindingFlags.Public | BindingFlags.Static).GetValue(null);

        public static object GetStaticPropery(Type type, string propName)
            => type.GetProperty(propName, BindingFlags.Public | BindingFlags.Static).GetValue(null, null);

        public static object GetFieldValue(object parent, string fieldName) 
            => parent.GetType().GetField(fieldName).GetValue(parent);

        public static object GetPropertyValue(object parent, string propName)
            => parent.GetType().GetProperty(propName).GetValue(parent, null);

        public static object GetMethodResult(object parent, string methodName, params object[] parameters)
            => parent.GetType().GetMethod(methodName).Invoke(parent, parameters);

        public static object GetStaticMethodResult(Type type, string methodName, params object[] parameters)
            => type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static).Invoke(null, parameters);

        public static void VoidMethod(object parent, string methodName, params object[] parameters)
            => parent.GetType().GetMethod(methodName).Invoke(parent, parameters);

        public static void StaticVoidMethod(Type type, string methodName, params object[] parameters)
            => type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static).Invoke(null, parameters);
    }

    public static class ConfigNodeExtension
    {
        public static Color GetColor(this ConfigNode self, string name, Color fallback)
        {
            if (self.HasValue(name))
            {
                try
                {
                    var value = self.GetValue(name);
                    return ConfigNode.ParseColor(value);
                }
                catch
                {
                    return fallback;
                }
            }

            return fallback;
        }

        public static Vector2 GetVector2(this ConfigNode self, string name, Vector2 fallback)
        {
            if (self.HasValue(name))
            {
                try
                {
                    var value = self.GetValue(name);
                    return ConfigNode.ParseVector2(value);
                }
                catch
                {
                    return fallback;
                }
            }

            return fallback;
        }

        public static string GetString(this ConfigNode self, string name, string fallback)
        {
            if (self.HasValue(name))
            {
                try
                {
                    return self.GetValue(name);
                }
                catch
                {
                    return fallback;
                }
            }

            return fallback;
        }

        public static bool GetBoolean(this ConfigNode self, string name, bool fallback)
        {
            if (self.HasValue(name))
            {
                try
                {
                    var value = self.GetValue(name);

                    return bool.Parse(value);
                }
                catch
                {
                    return fallback;
                }
            }

            return fallback;
        }

        public static float GetFloat(this ConfigNode self, string name, float fallback)
        {
            if (self.HasValue(name))
            {
                try
                {
                    var value = self.GetValue(name);
                    return float.Parse(value);
                }
                catch
                {
                    return fallback;
                }
            }

            return fallback;
        }

        public static int GetInteger(this ConfigNode self, string name, int fallback)
        {
            if (self.HasValue(name))
            {
                try
                {
                    var value = self.GetValue(name);
                    return int.Parse(value);
                }
                catch
                {
                    return fallback;
                }
            }

            return fallback;
        }

        public static Version GetVersion(this ConfigNode self, string name, Version fallback)
        {
            if (self.HasValue(name))
            {
                try
                {
                    var value = self.GetValue(name);
                    return new Version(value);
                }
                catch
                {
                    return fallback;
                }
            }

            return fallback;
        }

        public static T GetEnum<T>(this ConfigNode self, string name, T fallback)
        {
            if (self.HasValue(name))
            {
                try
                {
                    var value = self.GetValue(name);
                    return (T)(object)ConfigNode.ParseEnum(typeof(T), value);
                }
                catch
                {
                    return fallback;
                }
            }

            return fallback;
        }

        public static string[] TryReadStringArray(this ConfigNode self, string name, string[] fallback, bool fixedLength = true)
        {
            if (self.HasValue(name))
            {
                try
                {
                    var values = self.GetValue(name).Split(';');
                    if (fixedLength && values.Length != fallback.Length)
                    {
                        Historian.Print($"Wrong number of parameters for {name}. Expected {fallback.Length} but found {values.Length}");
                    }
                    else
                    {
                        return values;
                    }
                }
                catch (Exception e) {
                    Historian.Print($"Exception: {e.Message}. While attempting to read {name} from config");
                }
            }

            Historian.Print($"Unable to read array values for {name} - using defaults.");
            return (string[]) fallback.Clone();
        }
    }


}