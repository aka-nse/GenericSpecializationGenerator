// #define TEST_INSTANCE_FOO
// #define TEST_INSTANCE_BAR
// #define TEST_INSTANCE_BAZ

#if true
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericSpecialization;
#endif

namespace GenericSpecializationGenerator.DebugApp;

partial class SampleInstanceClass
{
#if TEST_INSTANCE_FOO

    [PrimaryGeneric(nameof(FooDefault))]
    public partial void Foo<T>(T input);
    
    private static void FooDefault<T>(T input)
    {
        Console.WriteLine($"default");
    }

    private void Foo(int input)
    {
        Console.WriteLine($"int specialized");
    }

    private static void Foo(double input)
    {
        Console.WriteLine($"double specialized");
    }

    private static void Foo(IList<double> input)
    {
        Console.WriteLine($"IList<double> specialized");
    }

    private static void Foo(IEnumerable<double> input)
    {
        Console.WriteLine($"IEnumerable<double> specialized");
    }

    private static void Foo(List<double> input)
    {
        Console.WriteLine($"List<double> specialized");
    }

#endif

#if TEST_INSTANCE_BAR

    [PrimaryGeneric(nameof(BarDefault))]
    public partial T Bar<T>(T input);

    private T BarDefault<T>(T input)
    {
        Console.WriteLine($"default");
        return input;
    }

    private int Bar(int input)
    {
        Console.WriteLine($"int specialized");
        return input;
    }

    private double Bar(double input)
    {
        Console.WriteLine($"double specialized");
        return input;
    }

    private static IList<double> Bar(IList<double> input)
    {
        Console.WriteLine($"IList<double> specialized");
        return input;
    }

    private static IEnumerable<double> Bar(IEnumerable<double> input)
    {
        Console.WriteLine($"IEnumerable<double> specialized");
        return input;
    }

    private static List<double> Bar(List<double> input)
    {
        Console.WriteLine($"List<double> specialized");
        return input;
    }

#endif

#if TEST_INSTANCE_BAZ

    [PrimaryGeneric(nameof(BazDefault))]
    public partial T1 Baz<T1, T2>(T1 x, T2 _x);

    private T1 BazDefault<T1, T2>(T1 x, T2 _x)
    {
        Console.WriteLine($"default");
        return x;
    }

    private object Baz(object x, int _x)
    {
        Console.WriteLine($"(object, int) specialized");
        return x;
    }

    private string Baz(string x, double _x)
    {
        Console.WriteLine($"(string, double) specialized");
        return x;
    }

    private object Baz(object x, double _x)
    {
        Console.WriteLine($"(object, double) specialized");
        return x;
    }

    private int Baz(int x, int _x)
    {
        Console.WriteLine($"(int, int) specialized");
        return x;
    }

    private double Baz(double x, int _x)
    {
        Console.WriteLine($"(double, int) specialized");
        return x;
    }

    private int Baz(int x, double _x)
    {
        Console.WriteLine($"(int, double) specialized");
        return x;
    }

    private double Baz(double x, double _x)
    {
        Console.WriteLine($"(double, double) specialized");
        return _x;
    }

#endif
}