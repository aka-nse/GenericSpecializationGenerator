// #define TEST_STATIC_1
// #define TEST_STATIC_2
// #define TEST_STATIC_3
// #define TEST_STATIC_4
// #define TEST_STATIC_5
// #define TEST_STATIC_6

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
#if TEST_STATIC_1

    [PrimaryGeneric(nameof(Example1Default))]
    public static partial void Example1<T>(T input) where T : unmanaged, INumber<T>;
    
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

#endif

#if TEST_STATIC_2

    [PrimaryGeneric(nameof(Example2Default))]
    public static partial void Example2<T>(T input, ref int refArg, in int inArg, out int outArg) where T : unmanaged, INumber<T>;

    private static void Example2Default<T>(T input, ref int refArg, in int inArg, out int outArg)
    {
        Console.WriteLine($"default");
        outArg = refArg + inArg;
    }

    private static void Example2(int input, ref int refArg, in int inArg, out int outArg)
    {
        Console.WriteLine($"int specialized");
        outArg = refArg + inArg;
    }

    private static void Example2(double input, ref int refArg, in int inArg, out int outArg)
    {
        Console.WriteLine($"double specialized");
        outArg = refArg + inArg;
    }

#endif

#if TEST_STATIC_3

    [PrimaryGeneric(nameof(Example3Default))]
    public static partial T Example3<T>(T input);

    private static T Example3Default<T>(T input)
    {
        Console.WriteLine($"default");
        return input;
    }

    private static int Example3(int input)
    {
        Console.WriteLine($"int specialized");
        return input;
    }

    private static double Example3(double input)
    {
        Console.WriteLine($"double specialized");
        return input;
    }

#endif

#if TEST_STATIC_4

    [PrimaryGeneric(nameof(Example4Default))]
    public static partial T1 Example4<T1, T2>(T1 x, T2 y)
        where T1 : unmanaged, INumber<T1>
        where T2 : unmanaged, INumberBase<T2>, IComparable<T2>;

    private static T1 Example4Default<T1, T2>(T1 x, T2 y)
    {
        Console.WriteLine($"default");
        return x;
    }

    private static int Example4(int x, int y)
    {
        Console.WriteLine($"(int, int) specialized");
        return x;
    }

    private static double Example4(double x, int y)
    {
        Console.WriteLine($"(double, int) specialized");
        return x;
    }

    private static int Example4(int x, double y)
    {
        Console.WriteLine($"(int, double) specialized");
        return x;
    }

    private static double Example4(double x, double y)
    {
        Console.WriteLine($"(double, double) specialized");
        return y;
    }

    private static string Example4(string x, double y)
    {
        Console.WriteLine($"(string, double) specialized");
        return x;
    }

#endif

#if TEST_STATIC_5

    [PrimaryGeneric(nameof(Example5Default))]
    public static partial Vector<T> Example5<T>(Vector<T> x, Vector<T> y)
        where T : unmanaged, INumber<T>;


    private static Vector<T> Example5Default<T>(Vector<T> x, Vector<T> y)
        where T : unmanaged, INumber<T>
        => default;

    private static Vector<int> Example5(Vector<int> x, Vector<int> y)
        => x + y;

#endif

#if TEST_STATIC_6

    [PrimaryGeneric(nameof(Example6Default))]
    public static partial Vector<T> Example6<T>(Vector<T> x, T y)
        where T : unmanaged, INumber<T>;


    private static Vector<T> Example6Default<T>(Vector<T> x, T y)
        where T : unmanaged, INumber<T>
        => default;

    private static Vector<int> Example6(Vector<int> x, int y)
        => x + new Vector<int>(y);

#endif
}
