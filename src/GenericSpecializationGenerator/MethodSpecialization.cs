using System.Linq;
using Microsoft.CodeAnalysis;
namespace GenericSpecializationGenerator;

internal class MethodSpecialization(
    IMethodSymbol primaryMethod,
    IMethodSymbol specializedMethod,
    IReadOnlyList<INamedTypeSymbol> closedTypeArgs
    )
{
    public IMethodSymbol PrimaryMethod { get; } = primaryMethod;
    public IMethodSymbol SpecializedMethod { get; } = specializedMethod;
    public IReadOnlyList<INamedTypeSymbol> ClosedTypeArgs { get; } = closedTypeArgs;

    public string VariablePrefix
    {
        get
        {
            var prefix = "_";
            while (PrimaryMethod.Parameters.Any(x => x.Name.StartsWith(prefix)))
            {
                prefix += "_";
            }
            return prefix;
        }
    }


    public static MethodSpecialization? MakeClosedSignature(ISymbol maybeSpecialized, IMethodSymbol primary)
        => MakeClosedSignature(maybeSpecialized as IMethodSymbol, primary);


    public static MethodSpecialization? MakeClosedSignature(IMethodSymbol? maybeSpecialized, IMethodSymbol primary)
    {
        if(maybeSpecialized is null)
        {
            return null;
        }
        if (maybeSpecialized.Name != primary.Name)
        {
            return null;
        }
        var specializedParams = maybeSpecialized.Parameters.Select(x => x.Type).ToArray();
        var primaryParams = primary.Parameters.Select(x => x.Type).ToArray();
        if (specializedParams.Length != primaryParams.Length)
        {
            return null;
        }

        var typeArgCloses = new INamedTypeSymbol?[primary.TypeArguments.Length];
        for (var i = 0; i < primaryParams.Length; ++i)
        {
            if (!IsGenericTypeMatch(primary.TypeArguments.AsSpan(), typeArgCloses, specializedParams[i], primaryParams[i]))
            {
                return null;
            }
        }
        if(typeArgCloses.Any(x => x is null))
        {
            return null;
        }
        return new(primary, maybeSpecialized, typeArgCloses!);
    }

    // determines whether specializedType is a valid specialization of primaryType or not
    private static bool IsGenericTypeMatch(
        ReadOnlySpan<ITypeSymbol> openTypeArgs,
        Span<INamedTypeSymbol?> closedTypeArgs,
        ITypeSymbol specializedType,
        ITypeSymbol primaryType)
    {
        if (SymbolEqualityComparer.Default.Equals(specializedType, primaryType))
        {
            // primaryType is concrete and matches with specializedType
            return true;
        }

        if(primaryType is ITypeParameterSymbol primaryTypeParam)
        {
            // primaryType is a bare type parameter
            var typeParamIndex = openTypeArgs.IndexOf(primaryTypeParam, SymbolEqualityComparer.Default);
            if (typeParamIndex < 0)
            {
                // invalid type parameter
                return false;
            }
            if (closedTypeArgs[typeParamIndex] is null && specializedType is INamedTypeSymbol closedSpecializedType)
            {
                // 1st time to map the type parameter
                closedTypeArgs[typeParamIndex] = closedSpecializedType;
                return true;
            }
            if (SymbolEqualityComparer.Default.Equals(closedTypeArgs[typeParamIndex], specializedType))
            {
                // 2nd or more time to map the type parameter
                return true;
            }

            // type parameter is already mapped to another type
            return false;
        }

        if(primaryType is INamedTypeSymbol primaryNamedType &&
            specializedType is INamedTypeSymbol specializedNamedType &&
            primaryNamedType.IsGenericType &&
            specializedNamedType.IsGenericType &&
            SymbolEqualityComparer.Default.Equals(primaryNamedType.ConstructedFrom, specializedNamedType.ConstructedFrom))
        {
            // primaryType is a generic type and matches with specializedType
            for (var i = 0; i < primaryNamedType.TypeArguments.Length; ++i)
            {
                if (!IsGenericTypeMatch(openTypeArgs, closedTypeArgs, specializedNamedType.TypeArguments[i], primaryNamedType.TypeArguments[i]))
                {
                    return false;
                }
            }
            return true;
        }

        // type mismatch
        return false;
    }

    private static IEnumerable<ITypeParameterSymbol> ScanContainingTypeParams(ITypeSymbol primaryType)
    {
        if(primaryType is ITypeParameterSymbol typeParam)
        {
            yield return typeParam;
            yield break;
        }
        if(primaryType is INamedTypeSymbol namedType && namedType.IsGenericType)
        {
            foreach (var arg in namedType.TypeArguments)
            {
                foreach (var innerTypeParam in ScanContainingTypeParams(arg))
                {
                    yield return innerTypeParam;
                }
            }
        }
    }
}


internal class MethodSpecializationComparer(Compilation compilation)
    : IComparer<MethodSpecialization>
{
    public int Compare(MethodSpecialization x, MethodSpecialization y)
    {
        if(!SymbolEqualityComparer.Default.Equals(x.PrimaryMethod, y.PrimaryMethod))
        {
            throw new ArgumentException();
        }
        if(x.ClosedTypeArgs.Count != y.ClosedTypeArgs.Count)
        {
            throw new ArgumentException();
        }

        int order;
        for(var i = 0; i < x.ClosedTypeArgs.Count; ++i)
        {
            var xx = x.ClosedTypeArgs[i];
            var yy = y.ClosedTypeArgs[i];
            if(SymbolEqualityComparer.Default.Equals(xx, yy))
            {
                continue;
            }
            if(TryCompare(xx.IsUnmanagedType, yy.IsUnmanagedType, out order))
            {
                if(order == 0)
                {
                    continue;
                }
                return order;
            }
            if (TryCompare(xx.IsValueType, yy.IsValueType, out order))
            {
                if (order == 0)
                {
                    continue;
                }
                return order;
            }
            if(compilation.HasImplicitConversion(xx, yy))
            {
                return -1;
            }
            if (compilation.HasImplicitConversion(yy, xx))
            {
                return +1;
            }
        }
        return 0;
    }

    private static bool TryCompare(bool x, bool y, out int order)
    {
        switch ((x, y))
        {
        case (true, true):
            order = 0;
            return true;
        case (true, false):
            order = -1;
            return true;
        case (false, true):
            order = +1;
            return true;
        default:
            order = default;
            return false;
        }
    }
}