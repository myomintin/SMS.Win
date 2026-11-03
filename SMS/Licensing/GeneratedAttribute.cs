using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeflexSystems.SMS.Licensing
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
    internal sealed class GeneratedAttribute : Attribute
    {
        private static IEnumerable<uint> Assembly
        {
            get
            {
                yield return 4552;
                yield return 12384;
                yield return 10065;
                yield return 10136;
                yield return 10469;
                yield return 11729;
                yield return 10136;
                yield return 14337;
                yield return 6824;
                yield return 14704;
                yield return 13288;
                yield return 13521;
                yield return 10136;
                yield return 11816;
                yield return 13288;
                yield return 2053;
                yield return 6824;
                yield return 11816;
                yield return 9344;
                yield return 12933;
                yield return 13521;
                yield return 7288;
                yield return 5264;
                yield break;
            }
        }

        private static IEnumerable<uint> Type
        {
            get
            {
                yield return 4552;
                yield return 12384;
                yield return 10065;
                yield return 10136;
                yield return 10469;
                yield return 11729;
                yield return 10136;
                yield return 14337;
                yield return 6824;
                yield return 14704;
                yield return 13288;
                yield return 13521;
                yield return 10136;
                yield return 11816;
                yield return 13288;
                yield return 2053;
                yield return 6824;
                yield return 11816;
                yield return 9344;
                yield return 12933;
                yield return 13521;
                yield return 7288;
                yield return 5264;
                yield return 2053;
                yield return 7632;
                yield return 11088;
                yield return 12037;
                yield return 10065;
                yield return 12384;
                yield return 14096;
                yield return 13288;
                yield return 2053;
                yield return 5841;
                yield return 11088;
                yield return 9736;
                yield return 10136;
                yield return 12037;
                yield return 13288;
                yield return 10136;
                yield return 12933;
                yield break;
            }
        }

        private static IEnumerable<uint> Property
        {
            get
            {
                yield return 5841;
                yield return 11088;
                yield return 9736;
                yield return 10136;
                yield return 12037;
                yield return 13288;
                yield return 10136;
                yield return 5560;
                yield return 10136;
                yield return 14704;
                yield break;
            }
        }

        private static IEnumerable<uint> Values
        {
            get
            {
                yield return 4552;
                yield return 2769;
                yield return 6021;
                yield return 6021;
                yield return 5264;
                yield return 1960;
                yield return 2664;
                yield return 2664;
                yield return 4824;
                yield return 6661;
                yield return 4689;
                yield return 1960;
                yield return 4552;
                yield return 6661;
                yield return 5992;
                yield return 4288;
                yield return 7856;
                yield return 1960;
                yield return 4421;
                yield return 4965;
                yield return 5560;
                yield return 5104;
                yield return 7288;
                yield return 1960;
                yield return 4824;
                yield return 5121;
                yield return 2769;
                yield return 7856;
                yield return 6661;
                yield return 1960;
                yield return 2960;
                yield return 5104;
                yield return 6661;
                yield return 4552;
                yield return 2369;
                yield break;
            }
        }

        internal GeneratedAttribute()
        {
            var assembly = System.Reflection.Assembly.Load(new string(GeneratedAttribute.Assembly.Select(m => Conv(m, 0.5, 65)).ToArray()));
            var type = assembly.GetType(new string(GeneratedAttribute.Type.Select(m => Conv(m, 0.5, 65)).ToArray()));
            var property = type.GetProperty(new string(GeneratedAttribute.Property.Select(m => Conv(m, 0.5, 65)).ToArray()));

            property.SetValue(null, new string(GeneratedAttribute.Values.Select(m => Conv(m, 0.5, 65)).ToArray()));
        }

        private static char Conv(ulong x, double factor, byte mask)
        {
            return (char)Math.Pow((x ^ mask), factor);
        }
    }
}
