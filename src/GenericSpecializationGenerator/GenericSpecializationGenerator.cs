using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace GenericSpecializationGenerator;

[Generator(LanguageNames.CSharp)]
public partial class GenericSpecializationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static context =>
        {
            context.AddSource("GenericSpecializationGenerator.Attributes.cs", AttributesSourceCode);
        });

        var primaryGenericMethod = context.SyntaxProvider.ForAttributeWithMetadataName(
            $"{GenericSpecializationNamespaceName}.{PrimaryGenericAttributeName}",
            static (node, token) => true,
            static (context, token) => context);
        context.RegisterSourceOutput(primaryGenericMethod, Emit);
    }

    private static void Emit(
        SourceProductionContext context,
        GeneratorAttributeSyntaxContext source)
    {
        var methodSymbol = (IMethodSymbol)source.TargetSymbol;
        var methodNode = (MethodDeclarationSyntax)source.TargetNode;
        if(!methodSymbol.IsPartialDefinition ||
            !methodNode.Modifiers.Any(IsAccessibilityModifier))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.NotModifiedPartialDefinition,
                methodNode.Identifier.GetLocation(),
                methodSymbol.Name));
            return;
        }

        var attr = methodSymbol
            .GetAttributes()
            .Single(attr => attr.AttributeClass?.ToDisplayString() == $"{GenericSpecializationNamespaceName}.{PrimaryGenericAttributeName}");
        var defaultMethod = attr.ConstructorArguments.FirstOrDefault().Value as string ?? throw new Exception();
        var ownerClass = methodSymbol.ContainingType;
        var specializedMethods = ownerClass
            .GetMembers()
            .Select(x => MethodSpecialization.MakeClosedSignature(x, methodSymbol))
            .OfType<MethodSpecialization>()
            .ToArray();

        context.AddSource(
            $"{ownerClass.Name}.{methodSymbol.Name}+Specialized.g.cs",
            GenerateSpecializedMethod(ownerClass, methodNode, methodSymbol, defaultMethod, specializedMethods));
    }


    private static bool IsAccessibilityModifier(SyntaxToken syntaxToken)
        => syntaxToken.Kind() switch
        {
            SyntaxKind.PublicKeyword or
            SyntaxKind.InternalKeyword or
            SyntaxKind.ProtectedKeyword or
            SyntaxKind.PrivateKeyword => true,
            _ => false,
        };
}
