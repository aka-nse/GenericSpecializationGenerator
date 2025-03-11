# GenericSpecializationGenerator

[<img src="https://img.shields.io/badge/-GitHub-blue.svg?logo=github" />](https://github.com/aka-nse/GenericSpecializationGenerator)

[<img src="https://img.shields.io/badge/-NuGet-019733.svg?logo=nuget" />](https://www.nuget.org/packages/akanse.GenericSpecializationGenerator)

Provides generic type specialization.

## What's this?

This is a source generator that achieves specialization of .Net generic methods,
similar to template specialization in C++.

Considering the nature of JIT, it can generate source code with very low overhead,
especially in release builds.

## Usage

```CSharp
using GenericSpecialization.DebugApp;

partial class SampleInstanceClass
{
    // Add `PrimaryGenericAttribute` to generic method declaration
    // with a parameter of the method name which will be called for default code path.
    [PrimaryGeneric(nameof(FooDefault))]
    public partial void Foo<T>(T input);

    // Define default implementation of target generic method.
    private void FooDefault<T>(T input)
    {
        Console.WriteLine($"default");
    }

    // Write non-generic method which has same signature with the closed of target generic method declaration.
    // They will be detected automatically and be treated as implementation of generic specialization.
    private void Foo(int input)
    {
        Console.WriteLine($"int specialized");
    }

    private void Foo(double input)
    {
        Console.WriteLine($"double specialized");
    }

    // When derived types are included, the priority is determined based on the standard overload resolution order.
    private static void Foo(List<double> input)
    {
        Console.WriteLine($"List<double> specialized");
    }

    private static void Foo(IList<double> input)
    {
        Console.WriteLine($"IList<double> specialized");
    }

    private static void Foo(IEnumerable<double> input)
    {
        Console.WriteLine($"IEnumerable<double> specialized");
    }
}
```

Against the preceding code, this generator will generate such as following code:

```CSharp
// <auto-generated/>
#nullable enable
#pragma warning disable CS8600
#pragma warning disable CS8601
#pragma warning disable CS8602
#pragma warning disable CS8603
#pragma warning disable CS8604
using System.Runtime.CompilerServices;
namespace GenericSpecializationGenerator.DebugApp;

partial class SampleInstanceClass
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public partial void Foo<T>(T input)
    {
        if(typeof(T) == typeof(int))
        {
            var _input = Unsafe.As<T, int>(ref input);
            Foo(_input);
            return;
        }
        if(typeof(T) == typeof(double))
        {
            var _input = Unsafe.As<T, double>(ref input);
            Foo(_input);
            return;
        }
        if(typeof(System.Collections.Generic.List<double>).IsAssignableFrom(typeof(T)))
        {
            var _input = Unsafe.As<T, System.Collections.Generic.List<double>>(ref input);
            Foo(_input);
            return;
        }
        if(typeof(System.Collections.Generic.IList<double>).IsAssignableFrom(typeof(T)))
        {
            var _input = Unsafe.As<T, System.Collections.Generic.IList<double>>(ref input);
            Foo(_input);
            return;
        }
        if(typeof(System.Collections.Generic.IEnumerable<double>).IsAssignableFrom(typeof(T)))
        {
            var _input = Unsafe.As<T, System.Collections.Generic.IEnumerable<double>>(ref input);
            Foo(_input);
            return;
        }
        FooDefault(input);
        return;
    }
}
```

### Rules

- Both of void-return and non-void-return methods are supported.
- Both of instance and static methods are supported.
  - If primary generic method is an instance method,
    both of instance and static methods are available as specialization or default.
  - If primary generic method is a static method,
    only static methods are available as specialization or default.
- Primary generic method declaration must has accessibility modifier.
- Generic specialization method must has same signature with primary generic method except that the generic type arguments are replaced by concrete types.
- Generic specialization method must be in the same class with primary generic method declaration.

### Limitation

These might be changed in future.

- Partial specialization is not supported now.
  - where only some of the multiple type arguments are determined
  - where type constraints become stricter instead of concrete types being substituted

## License

Apache License Version 2.0

## Release Note

### v0.0.1

- Adds a source generator to add `GenericSpecialization.PrimaryGenericAttribute` class and generate generic specialization code.
