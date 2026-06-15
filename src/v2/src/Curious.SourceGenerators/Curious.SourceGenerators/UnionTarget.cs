using Microsoft.CodeAnalysis;

namespace Curious.SourceGenerators;

internal sealed class UnionTarget
{
    public UnionTarget(
        Location location,
        string? ns,
        string? typeName,
        string[]? typeParamNames,
        DiagnosticKind diagnostic)
    {
        Location      = location;
        Namespace     = ns;
        TypeName      = typeName;
        TypeParamNames = typeParamNames;
        Diagnostic    = diagnostic;
    }

    public Location      Location       { get; }
    public string?       Namespace      { get; }
    public string?       TypeName       { get; }
    public string[]?     TypeParamNames { get; }
    public DiagnosticKind Diagnostic    { get; }
}
