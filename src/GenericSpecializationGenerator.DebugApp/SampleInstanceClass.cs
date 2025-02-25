using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenericSpecialization;

namespace GenericSpecializationGenerator.DebugApp;

partial class SampleInstanceClass
{
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

    [PrimaryGeneric(nameof(BazDefault))]
    public partial T1 Baz<T1, T2>(T1 x, T2 y);

    private T1 BazDefault<T1, T2>(T1 x, T2 y)
    {
        Console.WriteLine($"default");
        return x;
    }

    private int Baz(int x, int y)
    {
        Console.WriteLine($"(int, int) specialized");
        return x;
    }

    private double Baz(double x, int y)
    {
        Console.WriteLine($"(double, int) specialized");
        return x;
    }

    private int Baz(int x, double y)
    {
        Console.WriteLine($"(int, double) specialized");
        return x;
    }

    private double Baz(double x, double y)
    {
        Console.WriteLine($"(double, double) specialized");
        return y;
    }

    private string Baz(string x, double y)
    {
        Console.WriteLine($"(string, double) specialized");
        return x;
    }
}
