using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Curious.SourceGenerators;
using Xunit;

namespace Curious.SourceGenerators.Tests;

public class UnionGeneratorTests
{
    [Fact]
    public void GenerateSource_ForValidReadonlyPartialRecordStruct_EmitsUnionImplementation()
    {
        const string source = """
            using Curious.SourceGenerator.Attributes;

            namespace Demo;

            [Union]
            public readonly partial record struct Response<TSuccess, TError>
                where TSuccess : notnull
                where TError : notnull
            {
            }
            """;

        var result = RunGenerator(source, out var outputCompilation);

        Assert.Empty(GetErrorDiagnostics(outputCompilation));
        Assert.Empty(result.Diagnostics);
        Assert.Single(result.GeneratedSources);

        var generated = result.GeneratedSources[0];
        Assert.Equal("Response.2.g.cs", generated.HintName);

        var text = generated.SourceText.ToString();
        Assert.Contains("readonly partial record struct Response<TSuccess, TError>", text);
        Assert.Contains("public static Response<TSuccess, TError> FromTSuccess(TSuccess tSuccess)", text);
        Assert.Contains("public static Response<TSuccess, TError> FromTError(TError tError)", text);
        Assert.Contains("public static Response<TSuccess, TError> From(TSuccess tSuccess) => FromTSuccess(tSuccess);", text);
        Assert.Contains("public static Response<TSuccess, TError> From(TError tError) => FromTError(tError);", text);
        Assert.Contains("public bool IsTSuccess => _index == 0;", text);
        Assert.Contains("public bool IsTError => _index == 1;", text);
        Assert.Contains("public TResult Match<TResult>(Func<TSuccess, TResult> tSuccess, Func<TError, TResult> tError) =>", text);
        Assert.Contains("public void Switch(Action<TSuccess> tSuccess, Action<TError> tError)", text);
        Assert.Contains("public Response<TResult, TError> MapTSuccess<TResult>(Func<TSuccess, TResult> mapper)", text);
        Assert.Contains("public Response<TSuccess, TResult> MapTError<TResult>(Func<TError, TResult> mapper)", text);
        Assert.Contains("public bool Equals(Response<TSuccess, TError> other) =>", text);
        Assert.Contains("public override int GetHashCode() =>", text);
        Assert.Contains("public override string ToString() =>", text);
    }

    [Fact]
    public void GenerateSource_ForThreeTypeParameters_UsesArityInHintName()
    {
        const string source = """
            using Curious.SourceGenerator.Attributes;

            namespace Demo;

            [Union]
            public readonly partial record struct Response<TSuccess, TError, TPending>
                where TSuccess : notnull
                where TError : notnull
                where TPending : notnull
            {
            }
            """;

        var result = RunGenerator(source, out var outputCompilation);

        Assert.Empty(GetErrorDiagnostics(outputCompilation));
        Assert.Empty(result.Diagnostics);

        var generated = Assert.Single(result.GeneratedSources);
        Assert.Equal("Response.3.g.cs", generated.HintName);

        var text = generated.SourceText.ToString();
        Assert.Contains("readonly partial record struct Response<TSuccess, TError, TPending>", text);
        Assert.Contains("public bool IsTPending => _index == 2;", text);
        Assert.Contains("public Response<TSuccess, TError, TResult> MapTPending<TResult>(Func<TPending, TResult> mapper)", text);
    }

    [Fact]
    public void GenerateSource_ForNonReadonlyRecordStruct_ReportsUnion001()
    {
        const string source = """
            using Curious.SourceGenerator.Attributes;

            [Union]
            public partial record struct Response<TSuccess, TError>
                where TSuccess : notnull
                where TError : notnull
            {
            }
            """;

        var result = RunGenerator(source, out _);

        var diagnostic = Assert.Single(result.Diagnostics);
        Assert.Equal("UNION001", diagnostic.Id);
    }

    [Fact]
    public void GenerateSource_ForNonPartialRecordStruct_ReportsUnion002()
    {
        const string source = """
            using Curious.SourceGenerator.Attributes;

            [Union]
            public readonly record struct Response<TSuccess, TError>
                where TSuccess : notnull
                where TError : notnull
            {
            }
            """;

        var result = RunGenerator(source, out _);

        var diagnostic = Assert.Single(result.Diagnostics);
        Assert.Equal("UNION002", diagnostic.Id);
    }

    [Fact]
    public void GenerateSource_ForSingleTypeParameter_ReportsUnion003()
    {
        const string source = """
            using Curious.SourceGenerator.Attributes;

            [Union]
            public readonly partial record struct Response<TSuccess>
                where TSuccess : notnull
            {
            }
            """;

        var result = RunGenerator(source, out _);

        var diagnostic = Assert.Single(result.Diagnostics);
        Assert.Equal("UNION003", diagnostic.Id);
    }

    [Fact]
    public void GenerateSource_ForMissingNotNullConstraint_ReportsUnion004()
    {
        const string source = """
            using Curious.SourceGenerator.Attributes;

            [Union]
            public readonly partial record struct Response<TSuccess, TError>
                where TSuccess : notnull
            {
            }
            """;

        var result = RunGenerator(source, out _);

        var diagnostic = Assert.Single(result.Diagnostics);
        Assert.Equal("UNION004", diagnostic.Id);
    }

    private static GeneratorRunResult RunGenerator(string userSource, out Compilation outputCompilation)
    {
        var attributeSyntaxTree = CSharpSyntaxTree.ParseText("""
            using System;

            namespace Curious.SourceGenerator.Attributes;

            [AttributeUsage(AttributeTargets.Struct)]
            public sealed class UnionAttribute : Attribute
            {
            }
            """);

        var userSyntaxTree = CSharpSyntaxTree.ParseText(userSource);

        var compilation = CSharpCompilation.Create(
            assemblyName: "UnionGeneratorTests",
            syntaxTrees: [attributeSyntaxTree, userSyntaxTree],
            references: GetMetadataReferences(),
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(new UnionGenerator());
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out outputCompilation, out _);

        return Assert.Single(driver.GetRunResult().Results);
    }

    private static ImmutableArray<Diagnostic> GetErrorDiagnostics(Compilation compilation) =>
        compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error).ToImmutableArray();

    private static ImmutableArray<MetadataReference> GetMetadataReferences() =>
        ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
        .Split(Path.PathSeparator)
        .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
        .ToImmutableArray();
}
