using Curious.SourceGenerator.Attributes;

namespace Curious.SourceGenerators.Sample;

// 2-arm union
[Union]
public readonly partial record struct StringOrInt<TString, TInt>
    where TString : notnull
    where TInt : notnull;

// 3-arm union
[Union]
public readonly partial record struct Shape<TCircle, TRectangle, TTriangle>
    where TCircle : notnull
    where TRectangle : notnull
    where TTriangle : notnull;


internal static class SampleUsage
{
    internal static void Run()
    {
        StringOrInt<string, int> a = StringOrInt<string, int>.FromTString("hello");
        StringOrInt<string, int> b = StringOrInt<string, int>.FromTInt(42);
        Union<string, string> c = Union<string, string>.FromT1("hello");
        Union<string, int, bool> d = Union<string, int, bool>.FromT3(true);

        System.Console.WriteLine(a.Match(s => $"string: {s}", i => $"int: {i}"));
        System.Console.WriteLine(b.Match(s => $"string: {s}", i => $"int: {i}"));
        System.Console.WriteLine(a.IsTString);
        System.Console.WriteLine(b.IsTInt);
        System.Console.WriteLine(a.ToString());

        StringOrInt<int, int> mapped = a.MapTString(s => s.Length);
        System.Console.WriteLine(mapped.Match(l => $"length: {l}", i => $"int: {i}"));
    }
}
