using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GenericSpecialization;

namespace GenericSpecializationGenerator.DebugApp;

internal partial class SampleInnerClass
{
    public partial class InnerBaseClass
    {
        [PrimaryGeneric(nameof(Example1Default))]
        private protected static partial void Example1<T>(T input) where T : unmanaged, INumber<T>;

        private static void Example1Default<T>(T input)
        {
            Console.WriteLine($"default");
        }

        private static void Example1(int input)
        {
            Console.WriteLine($"int specialized");
        }

        private static void Example1(double input)
        {
            Console.WriteLine($"double specialized");
        }
    }

    public class InnerDerivedClass : InnerBaseClass
    {
        public static void Example1Derived<T>(T input) where T : unmanaged, INumber<T>
            => Example1(input);
    }
}
