using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GenericSpecializationGenerator;

internal class MethodDeclarationInfo(IMethodSymbol symbol, MethodDeclarationSyntax node) : IEquatable<MethodDeclarationInfo>
{
    public IMethodSymbol Symbol { get; } = symbol;
    public MethodDeclarationSyntax Node { get; } = node;

    public override string ToString()
        => $"{string.Join(" ", Node.Modifiers)} {Node.ReturnType} {Symbol.Name}{Node.TypeParameterList}{Node.ParameterList}";

    public override int GetHashCode()
        => SymbolEqualityComparer.Default.GetHashCode(Symbol);

    public override bool Equals(object obj)
        => obj is MethodDeclarationInfo other && Equals(this, other);

    public bool Equals(MethodDeclarationInfo other)
        => Equals(this, other);

    public static bool Equals(MethodDeclarationInfo x, MethodDeclarationInfo y)
        => SymbolEqualityComparer.Default.Equals(x.Symbol, y.Symbol);

    public static bool operator ==(MethodDeclarationInfo x, MethodDeclarationInfo y) => Equals(x, y);
    public static bool operator !=(MethodDeclarationInfo x, MethodDeclarationInfo y) => !Equals(x, y);
}
