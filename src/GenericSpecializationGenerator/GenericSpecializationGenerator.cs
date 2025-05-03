using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenericSpecializationGenerator;
using PipelineParameter = (MethodDeclarationInfo method, GeneratorAttributeSyntaxContext context);

[Generator(LanguageNames.CSharp)]
public partial class GenericSpecializationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static context =>
        {
            context.AddSource("GenericSpecializationGenerator.Attributes.cs", AttributesSourceCode);
        });

        var primaryGenericMethod = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                AttributeFullName,
                static (node, token) => true,
                static (context, token) =>
                {
                    var targetSymbol = (IMethodSymbol)context.TargetSymbol;
                    var targetNode = (MethodDeclarationSyntax)context.TargetNode;
                    MethodDeclarationInfo method = new(targetSymbol, targetNode);
                    return (method, context);
                })
            .WithComparer(PipelineComparer.Instance);
        context.RegisterSourceOutput(primaryGenericMethod, Emit);
    }

    private static void Emit(
        SourceProductionContext context,
        PipelineParameter data)
    {
        var (method, source) = data;
        var compilation = source.SemanticModel.Compilation;
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
            .Single(attr => attr.AttributeClass?.ToDisplayString() == AttributeFullName);
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
            => p.RefKind switch
            {
                RefKind.None => $"{p.Type}",
                RefKind.Ref or
                RefKind.Out or
                RefKind.In => $"ref_{p.Type}",
                _ => throw new InvalidOperationException(),
            };

        var builder = GenerateSpecializedMethod(source, method, defaultMethod, specializedMethods);
        var hintName = builder.GetPreferHintName(suffix: $".{method.Symbol.Name}-{string.Join("-", method.Symbol.Parameters.Select(getParamName))}+Specialized");
        context.AddSource(
            hintName,
            builder.Build());
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


file class PipelineComparer : IEqualityComparer<PipelineParameter>
{
    public static PipelineComparer Instance { get; } = new();

    private PipelineComparer() { }

    public bool Equals(PipelineParameter x, PipelineParameter y)
        => MethodDeclarationInfo.Equals(x.method, y.method);

    public int GetHashCode(PipelineParameter obj)
        => obj.method.GetHashCode();
}