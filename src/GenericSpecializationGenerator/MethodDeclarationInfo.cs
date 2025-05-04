using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenericSpecializationGenerator;

internal class MethodDeclarationInfo : IEquatable<MethodDeclarationInfo>
{
    private readonly string _identifier;
    public GeneratorAttributeSyntaxContext Context { get; }

    public IMethodSymbol Symbol => _symbol ??= (IMethodSymbol)Context.TargetSymbol;
    private IMethodSymbol? _symbol;

    public MethodDeclarationSyntax Node { get; }

    public MethodDeclarationInfo(GeneratorAttributeSyntaxContext context)
    {
        Context = context;
        Node = (MethodDeclarationSyntax)context.TargetNode;
        _identifier = Node.ToFullString();
    }

    public override string ToString()
    {
        var modifiers = string.Join(" ", Node.Modifiers);
        var returnType = Node.ReturnType;
        var name = Symbol.Name;
        var typeParameters = Node.TypeParameterList;
        var parameters = Node.ParameterList;
        var constraints = Node.ConstraintClauses;
        return $"{modifiers} {returnType} {name}{typeParameters}{parameters} {constraints}";
    }

    public override int GetHashCode()
        => SymbolEqualityComparer.Default.GetHashCode(Symbol);

    public override bool Equals(object obj)
        => obj is MethodDeclarationInfo other && Equals(this, other);

    public bool Equals(MethodDeclarationInfo other)
        => Equals(this, other);

    public static bool Equals(MethodDeclarationInfo x, MethodDeclarationInfo y)
        => x._identifier == y._identifier;

    public static bool operator ==(MethodDeclarationInfo x, MethodDeclarationInfo y) => Equals(x, y);
    public static bool operator !=(MethodDeclarationInfo x, MethodDeclarationInfo y) => !Equals(x, y);
}
