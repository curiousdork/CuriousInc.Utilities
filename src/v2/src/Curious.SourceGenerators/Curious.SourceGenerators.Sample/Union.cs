using Curious.SourceGenerator.Attributes;

namespace Curious.SourceGenerators.Sample;

[Union]
public readonly partial record struct Union<T1, T2>
    where T1 : notnull
    where T2 : notnull;

[Union]
public readonly partial record struct Union<T1, T2, T3>
    where T1 : notnull
    where T2 : notnull
    where T3 : notnull;
