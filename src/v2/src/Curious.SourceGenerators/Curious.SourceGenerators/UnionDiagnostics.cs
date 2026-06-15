using Microsoft.CodeAnalysis;

namespace Curious.SourceGenerators;

internal enum DiagnosticKind
{
    None,
    NotReadonlyRecordStruct,
    NotPartial,
    TooFewTypeParameters,
    MissingNotNullConstraint
}

internal static class UnionDiagnostics
{
    private const string Category = "UnionGenerator";

    public static readonly DiagnosticDescriptor NotReadonlyRecordStruct = new(
        id: "UNION001",
        title: "[Union] must be applied to a readonly record struct",
        messageFormat: "[Union] can only be applied to a 'readonly record struct'. '{0}' does not qualify.",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor NotPartial = new(
        id: "UNION002",
        title: "[Union] struct must be declared partial",
        messageFormat: "'{0}' must be declared 'partial' so the Union source generator can emit its implementation",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor TooFewTypeParameters = new(
        id: "UNION003",
        title: "[Union] struct must have at least two generic type parameters",
        messageFormat: "'{0}' must declare at least two generic type parameters to form a Union",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor MissingNotNullConstraint = new(
        id: "UNION004",
        title: "[Union] type parameters must each carry a 'notnull' constraint",
        messageFormat: "Every type parameter of '{0}' must be constrained with 'where T : notnull'",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
