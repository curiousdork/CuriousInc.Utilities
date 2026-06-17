namespace Curious.SourceGenerators.AspNet;

internal sealed class UnionJsonConverterTarget(
    string? ns,
    string typeName,
    string[] typeParamNames)
{
    public string?   Namespace      { get; } = ns;
    public string    TypeName       { get; } = typeName;
    public string[]  TypeParamNames { get; } = typeParamNames;
}
