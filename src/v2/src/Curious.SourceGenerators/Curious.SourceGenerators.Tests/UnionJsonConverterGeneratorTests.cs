using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Curious.SourceGenerators.AspNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace Curious.SourceGenerators.Tests;

public class UnionJsonConverterGeneratorTests
{
    [Fact]
    public void GenerateConverter_ForValidUnion_EmitsFactoryAndConverter()
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

        Assert.Empty(result.Diagnostics);
        var generated = Assert.Single(result.GeneratedSources);
        Assert.Equal("Response.2.JsonConverter.g.cs", generated.HintName);

        var text = generated.SourceText.ToString();
        Assert.Contains("[JsonConverter(typeof(Response2JsonConverterFactory))]", text);
        Assert.Contains("public sealed class Response2JsonConverterFactory : global::System.Text.Json.Serialization.JsonConverterFactory", text);
        Assert.Contains("typeToConvert.GetGenericTypeDefinition() == typeof(Response<,>)", text);
        Assert.Contains("public sealed class ResponseJsonConverter<TSuccess, TError> : global::System.Text.Json.Serialization.JsonConverter<Response<TSuccess, TError>>", text);
        Assert.Contains("case \"$index\":", text);
        Assert.Contains("case \"value\":", text);
        Assert.Contains("0 => Response<TSuccess, TError>.FromTSuccess(global::System.Text.Json.JsonSerializer.Deserialize<TSuccess>(ref reader, options)!)", text);
        Assert.Contains("1 => Response<TSuccess, TError>.FromTError(global::System.Text.Json.JsonSerializer.Deserialize<TError>(ref reader, options)!)", text);
        Assert.Contains("writer.WriteNumber(\"$index\", 0)", text);
        Assert.Contains("writer.WriteNumber(\"$index\", 1)", text);
    }

    [Fact]
    public void GenerateConverter_ForThreeTypeParameters_EmitsCorrectArity()
    {
        const string source = """
            using Curious.SourceGenerator.Attributes;

            namespace Demo;

            [Union]
            public readonly partial record struct Shape<TCircle, TRectangle, TTriangle>
                where TCircle : notnull
                where TRectangle : notnull
                where TTriangle : notnull
            {
            }
            """;

        var result = RunGenerator(source, out _);

        Assert.Empty(result.Diagnostics);
        var generated = Assert.Single(result.GeneratedSources);
        Assert.Equal("Shape.3.JsonConverter.g.cs", generated.HintName);

        var text = generated.SourceText.ToString();
        Assert.Contains("[JsonConverter(typeof(Shape3JsonConverterFactory))]", text);
        Assert.Contains("typeToConvert.GetGenericTypeDefinition() == typeof(Shape<,,>)", text);
        Assert.Contains("public sealed class ShapeJsonConverter<TCircle, TRectangle, TTriangle>", text);
        Assert.Contains("0 => Shape<TCircle, TRectangle, TTriangle>.FromTCircle(", text);
        Assert.Contains("1 => Shape<TCircle, TRectangle, TTriangle>.FromTRectangle(", text);
        Assert.Contains("2 => Shape<TCircle, TRectangle, TTriangle>.FromTTriangle(", text);
    }

    [Fact]
    public void GenerateConverter_ForTypeWithoutNotNullConstraint_ProducesNoOutput()
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

        Assert.Empty(result.GeneratedSources);
    }

    [Fact]
    public void GenerateConverter_ForNonPartialType_ProducesNoOutput()
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

        Assert.Empty(result.GeneratedSources);
    }

    [Fact]
    public void GenerateConverter_ForGlobalNamespace_OmitsNamespaceDeclaration()
    {
        const string source = """
            using Curious.SourceGenerator.Attributes;

            [Union]
            public readonly partial record struct Result<TOk, TErr>
                where TOk : notnull
                where TErr : notnull
            {
            }
            """;

        var result = RunGenerator(source, out _);

        Assert.Empty(result.Diagnostics);
        var generated = Assert.Single(result.GeneratedSources);
        var text = generated.SourceText.ToString();
        Assert.DoesNotContain("namespace ", text);
    }

    private static GeneratorRunResult RunGenerator(string userSource, out Compilation outputCompilation)
    {
        var attributeSyntaxTree = CSharpSyntaxTree.ParseText("""
            using System;

            namespace Curious.SourceGenerator.Attributes;

            [AttributeUsage(AttributeTargets.Struct)]
            public sealed class UnionAttribute : Attribute { }
            """);

        var userSyntaxTree = CSharpSyntaxTree.ParseText(userSource);

        var compilation = CSharpCompilation.Create(
            assemblyName: "UnionJsonConverterGeneratorTests",
            syntaxTrees: [attributeSyntaxTree, userSyntaxTree],
            references: GetMetadataReferences(),
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        GeneratorDriver driver = CSharpGeneratorDriver.Create(new UnionJsonConverterGenerator());
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out outputCompilation, out _);

        return Assert.Single(driver.GetRunResult().Results);
    }

    private static ImmutableArray<MetadataReference> GetMetadataReferences() =>
        ((string?)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES"))!
        .Split(Path.PathSeparator)
        .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
        .ToImmutableArray();
}
