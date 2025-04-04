﻿using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenericSpecializationGenerator;

partial class GenericSpecializationGenerator
{
    private abstract class GenerationLogic
    {
        protected GenerationLogic() { }

        public string GenerateSpecializedMethod(
        SyntaxList<UsingDirectiveSyntax> usings,
        INamedTypeSymbol ownerType,
        MethodDeclarationInfo method,
        string defaultMethodName,
        MethodSpecialization[] specializedMethods)
        {
            var ns = ownerType.ContainingNamespace.IsGlobalNamespace
                ? ""
                : $"namespace {ownerType.ContainingNamespace};";
            var (typeInitializer, typeTerminator) = GetTypeDefinition(ownerType);

            SourceCodeGenerationHandler generator = $$"""
            // <auto-generated/>
            #nullable enable
            #pragma warning disable CS8600
            #pragma warning disable CS8601
            #pragma warning disable CS8602
            #pragma warning disable CS8603
            #pragma warning disable CS8604
            {{usings.ForeachIndented(syntax => $"{syntax}")}}
            using __Unsafe = System.Runtime.CompilerServices.Unsafe;
            {{ns}}

            {{typeInitializer}}
            #if NETCOREAPP3_0_OR_GREATER
                [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveOptimization)]
            #endif
                {{method}}
                {
                    {{specializedMethods.Select(GetSpecializedCode)}}
                    {{GetDefaultReturn(method.Symbol, defaultMethodName)}}
                }

            {{typeTerminator}}
            """;
            return generator.ToString();
        }

        protected SourceCodeGenerationHandler GetSpecializedCode(MethodSpecialization specialized)
        {
            var methodSymbol = specialized.PrimaryMethod;
            var condition = GetCondition(methodSymbol.TypeArguments, specialized.ClosedTypeArgs);
            var paramMaps = methodSymbol.Parameters.Zip(specialized.SpecializedMethod.Parameters, (x, y) => (arg: x, mapped: y));
            SourceCodeGenerationHandler generator = $$"""
            if({{condition}})
            {
                {{paramMaps.ForeachIndented(tpl => GetVariableCast(specialized, tpl))}}
                {{GetReturn(methodSymbol, specialized)}}
            }
            """;
            return generator;
        }

        protected abstract SourceCodeGenerationHandler GetReturn(IMethodSymbol methodSymbol, MethodSpecialization specialized);

        protected abstract SourceCodeGenerationHandler GetDefaultReturn(IMethodSymbol methodSymbol, string defaultMethodName);

        private static (string initializer, string terminator) GetTypeDefinition(INamedTypeSymbol ownerType)
        {
            var initializer = "";
            var terminator = "";

            for(var type = ownerType; type is not null; type = type.ContainingType)
            {
                var typeInitializer = type.TypeKind switch
                {
                    TypeKind.Class => $"partial class {type.Name}",
                    TypeKind.Struct => $"partial struct {type.Name}",
                    TypeKind.Interface => $"partial interface {type.Name}",
                    _ => throw new ArgumentException(),
                };
                initializer = typeInitializer + " { " + initializer;
                terminator += "} ";
            }
            return (initializer, terminator);
        }

        private static string GetCondition(IEnumerable<ITypeSymbol> openTypeArgs, IEnumerable<INamedTypeSymbol> closedTypeArgs)
        {
            static string core(ITypeSymbol open, INamedTypeSymbol closed)
                => closed.IsValueType
                ? $"typeof({open}) == typeof({closed})"
                : $"typeof({closed}).IsAssignableFrom(typeof({open}))";

            return string.Join(" && ", openTypeArgs.Zip(closedTypeArgs, core));
        }

        private static string GetVariableCast(MethodSpecialization specialized, (IParameterSymbol arg, IParameterSymbol mapped) tpl)
        {
            var (arg, mapped) = tpl;
            return arg.RefKind switch
            {
                RefKind.None => $"var {specialized.VariablePrefix}{mapped.Name} = __Unsafe.As<{arg.Type}, {mapped.Type}>(ref {arg.Name});",
                RefKind.In => $"ref var {specialized.VariablePrefix}{mapped.Name} = ref __Unsafe.As<{arg.Type}, {mapped.Type}>(ref __Unsafe.AsRef(in {arg.Name}));",
                RefKind.Ref => $"ref var {specialized.VariablePrefix}{mapped.Name} = ref __Unsafe.As<{arg.Type}, {mapped.Type}>(ref {arg.Name});",
                RefKind.Out => $"__Unsafe.SkipInit(out {arg.Name}); ref var {specialized.VariablePrefix}{mapped.Name} = ref __Unsafe.As<{arg.Type}, {mapped.Type}>(ref {arg.Name});",
                _ => throw new ArgumentException(),
            };
        }

        protected static string GetSpecializedCallParameters(IMethodSymbol methodSymbol, MethodSpecialization specialized)
        {
            string core(IParameterSymbol arg)
                => arg.RefKind switch
                {
                    RefKind.None => $"{specialized.VariablePrefix}{arg.Name}",
                    RefKind.In => $"in {specialized.VariablePrefix}{arg.Name}",
                    RefKind.Ref => $"ref {specialized.VariablePrefix}{arg.Name}",
                    RefKind.Out => $"out {specialized.VariablePrefix}{arg.Name}",
                    _ => throw new ArgumentException(),
                };

            return string.Join(", ", methodSymbol.Parameters.Select(core));
        }

        protected static string GetDefaultCallParameters(IMethodSymbol methodSymbol)
        {
            static string core(IParameterSymbol arg)
                => arg.RefKind switch
                {
                    RefKind.None => $"{arg.Name}",
                    RefKind.In => $"in {arg.Name}",
                    RefKind.Ref => $"ref {arg.Name}",
                    RefKind.Out => $"out {arg.Name}",
                    _ => throw new ArgumentException(),
                };

            return string.Join(", ", methodSymbol.Parameters.Select(core));
        }
    }


    private class GenerationVoidReturnLogic : GenerationLogic
    {
        public static GenerationLogic Instance { get; } = new GenerationVoidReturnLogic();

        protected override SourceCodeGenerationHandler GetReturn(IMethodSymbol methodSymbol, MethodSpecialization specialized)
            => $$"""
            {{methodSymbol.Name}}({{GetSpecializedCallParameters(methodSymbol, specialized)}});
            return;
            """;

        protected override SourceCodeGenerationHandler GetDefaultReturn(IMethodSymbol methodSymbol, string defaultMethodName)
            => $$"""
            {{defaultMethodName}}({{GetDefaultCallParameters(methodSymbol)}});
            return;
            """;
    }


    private class GenerationNonVoidReturnLogic : GenerationLogic
    {
        public static GenerationLogic Instance { get; } = new GenerationNonVoidReturnLogic();

        protected override SourceCodeGenerationHandler GetReturn(IMethodSymbol methodSymbol, MethodSpecialization specialized)
            => $$"""
            var {{specialized.VariablePrefix}}retval = {{methodSymbol.Name}}({{GetSpecializedCallParameters(methodSymbol, specialized)}});
            return __Unsafe.As<{{specialized.SpecializedMethod.ReturnType}}, {{methodSymbol.ReturnType}}>(ref {{specialized.VariablePrefix}}retval);
            """;

        protected override SourceCodeGenerationHandler GetDefaultReturn(IMethodSymbol methodSymbol, string defaultMethodName)
            => $$"""
            return {{defaultMethodName}}({{GetDefaultCallParameters(methodSymbol)}});
            """;
    }


    private static string GenerateSpecializedMethod(
        SyntaxList<UsingDirectiveSyntax> usings,
        INamedTypeSymbol ownerClass,
        MethodDeclarationInfo method,
        string defaultMethodName,
        MethodSpecialization[] specializedMethods)
    {
        var logic = method.Symbol.ReturnType.SpecialType == SpecialType.System_Void
            ? GenerationVoidReturnLogic.Instance
            : GenerationNonVoidReturnLogic.Instance;
        return logic.GenerateSpecializedMethod(usings, ownerClass, method, defaultMethodName, specializedMethods);
    }


}
