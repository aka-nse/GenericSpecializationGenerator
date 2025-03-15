using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
            static (context, token) =>
            {
                var methodSymbol = (IMethodSymbol)context.TargetSymbol;
                var methodNode = (MethodDeclarationSyntax)context.TargetNode;
                return new MethodDeclarationInfo(methodSymbol, methodNode);
            })
            .WithComparer(EqualityComparer<MethodDeclarationInfo>.Default)
            .Combine(context.CompilationProvider);
        context.RegisterSourceOutput(primaryGenericMethod, Emit);
    }

    private static void Emit(
        SourceProductionContext context,
        (MethodDeclarationInfo method, Compilation compilation) data)
    {
        var (method, compilation) = data;
        if (!method.Symbol.IsPartialDefinition ||
            !method.Node.Modifiers.Any(IsAccessibilityModifier))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.NotModifiedPartialDefinition,
                method.Node.Identifier.GetLocation(),
                method.Symbol.Name));
            return;
        }

        var attr = method.Symbol
            .GetAttributes()
            .Single(attr => attr.AttributeClass?.ToDisplayString() == $"{GenericSpecializationNamespaceName}.{PrimaryGenericAttributeName}");
        var defaultMethod = attr.ConstructorArguments.FirstOrDefault().Value as string ?? throw new Exception();
        var usings = method.Node.Ancestors().OfType<CompilationUnitSyntax>().Single().Usings;
        var ownerClass = method.Symbol.ContainingType;
        var comparer = new MethodSpecializationComparer(compilation);
        var specializedMethods = ownerClass
            .GetMembers()
            .Select(x => MethodSpecialization.MakeClosedSignature(x, method.Symbol))
            .OfType<MethodSpecialization>()
            .OrderBy(x => x, comparer)
            .ToArray();

        static string getParamName(IParameterSymbol p)
        {
            var name = $"{p.Type}";
            name = name.Replace("<", "_").Replace(">", "_").Replace(",", "_");
            return p.RefKind switch
            {
                RefKind.None => $"{name}",
                RefKind.Ref or
                RefKind.Out or
                RefKind.In => $"ref_{name}",
                _ => throw new InvalidOperationException(),
            };
        }

        var hintName = $"{ownerClass.Name}.{method.Symbol.Name}-{string.Join("-", method.Symbol.Parameters.Select(getParamName))}+Specialized.g.cs";
        context.AddSource(
            hintName,
            GenerateSpecializedMethod(usings, ownerClass, method, defaultMethod, specializedMethods));
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
