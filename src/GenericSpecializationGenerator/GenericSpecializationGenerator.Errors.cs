using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace GenericSpecializationGenerator;

partial class GenericSpecializationGenerator
{
    private static class DiagnosticDescriptors
    {
        public static DiagnosticDescriptor NotModifiedPartialDefinition { get; } = new (
            id: "GenericSP0001",
            title: "GenericSpecializationGenerator",
            messageFormat: $"The target of {PrimaryGenericAttributeName} must be modified partial definition.",
            category: "GeneratorRule",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }
}
