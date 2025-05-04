using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenericSpecializationGenerator;

internal class MethodDeclarationInfo : IEquatable<MethodDeclarationInfo>
{
    private readonly string _symbolIdentifier;
    public IMethodSymbol Symbol { get; }
    public MethodDeclarationSyntax Node { get; }

    public MethodDeclarationInfo(GeneratorAttributeSyntaxContext context)
    {
        Symbol = (IMethodSymbol)context.TargetSymbol;
        Node = (MethodDeclarationSyntax)context.TargetNode;
        _symbolIdentifier = Symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    }

    public override string ToString()
    {
        var modifiers = string.Join(" ", Node.Modifiers);
        var returnType = Node.ReturnType;
        var name = Symbol.Name;
        var typeParams = Node.TypeParameterList;
        var @params = Node.ParameterList;
        var constraints = Node.ConstraintClauses;
        return $"{modifiers} {returnType} {name}{typeParams}{@params} {constraints}";
    }

    public override int GetHashCode()
        => SymbolEqualityComparer.Default.GetHashCode(Symbol);

    public override bool Equals(object obj)
        => obj is MethodDeclarationInfo other && Equals(this, other);

    public bool Equals(MethodDeclarationInfo other)
        => Equals(this, other);

    public static bool Equals(MethodDeclarationInfo x, MethodDeclarationInfo y)
        => x._symbolIdentifier == y._symbolIdentifier;

    public static bool operator ==(MethodDeclarationInfo x, MethodDeclarationInfo y) => Equals(x, y);
    public static bool operator !=(MethodDeclarationInfo x, MethodDeclarationInfo y) => !Equals(x, y);
}
