namespace GenericSpecializationGenerator;

partial class GenericSpecializationGenerator
{
    public const string GenericSpecializationNamespaceName = "GenericSpecialization";
    public const string PrimaryGenericAttributeName = "PrimaryGenericAttribute";
    public static readonly string AttributeFullName = $"{GenericSpecializationNamespaceName}.{PrimaryGenericAttributeName}";

    public const string AttributesSourceCode = $$"""
    using System;
    namespace {{GenericSpecializationNamespaceName}};

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    internal sealed class {{PrimaryGenericAttributeName}}(
        string defaultMethod)
        : Attribute
    {
        public string DefaultMethod { get; } = defaultMethod;
    }
    """;
}
