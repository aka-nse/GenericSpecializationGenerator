#define TEST_STATIC_FOO_1
// #define TEST_STATIC_FOO_2
// #define TEST_STATIC_BAR
// #define TEST_STATIC_BAZ

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GenericSpecialization;

namespace GenericSpecializationGenerator.DebugApp;

static partial class SampleStaticClass
{
#if TEST_STATIC_FOO_1

    [PrimaryGeneric(nameof(FooDefault))]
    public static partial void Foo<T>(T input) where T : unmanaged, INumber<T>;
    
    private static void FooDefault<T>(T input)
    {
        Console.WriteLine($"default");
    }

    private static void Foo(int input)
    {
        Console.WriteLine($"int specialized");
    }

    private static void Foo(double input)
    {
        Console.WriteLine($"double specialized");
    }

#endif

#if TEST_STATIC_FOO_2

    [PrimaryGeneric(nameof(FooDefault))]
    public static partial void Foo<T>(T input, ref int refArg, in int inArg, out int outArg) where T : unmanaged, INumber<T>;

    private static void FooDefault<T>(T input, ref int refArg, in int inArg, out int outArg)
    {
        Console.WriteLine($"default");
        outArg = refArg + inArg;
    }

    private static void Foo(int input, ref int refArg, in int inArg, out int outArg)
    {
        Console.WriteLine($"int specialized");
        outArg = refArg + inArg;
    }

    private static void Foo(double input, ref int refArg, in int inArg, out int outArg)
    {
        Console.WriteLine($"double specialized");
        outArg = refArg + inArg;
    }

#endif

#if TEST_STATIC_BAR

    [PrimaryGeneric(nameof(BarDefault))]
    public static partial T Bar<T>(T input);

    private static T BarDefault<T>(T input)
    {
        Console.WriteLine($"default");
        return input;
    }

    private static int Bar(int input)
    {
        Console.WriteLine($"int specialized");
        return input;
    }

    private static double Bar(double input)
    {
        Console.WriteLine($"double specialized");
        return input;
    }

#endif

#if TEST_STATIC_BAZ

    [PrimaryGeneric(nameof(BazDefault))]
    public static partial T1 Baz<T1, T2>(T1 x, T2 y)
        where T1 : unmanaged, INumber<T1>
        where T2 : unmanaged, INumberBase<T2>, IComparable<T2>;

    private static T1 BazDefault<T1, T2>(T1 x, T2 y)
    {
        Console.WriteLine($"default");
        return x;
    }

    private static int Baz(int x, int y)
    {
        Console.WriteLine($"(int, int) specialized");
        return x;
    }

    private static double Baz(double x, int y)
    {
        Console.WriteLine($"(double, int) specialized");
        return x;
    }

    private static int Baz(int x, double y)
    {
        Console.WriteLine($"(int, double) specialized");
        return x;
    }

    private static double Baz(double x, double y)
    {
        Console.WriteLine($"(double, double) specialized");
        return y;
    }

    private static string Baz(string x, double y)
    {
        Console.WriteLine($"(string, double) specialized");
        return x;
    }

#endif
}
