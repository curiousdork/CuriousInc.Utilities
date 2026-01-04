using System.Linq;
using System.Text;
using CuriousInc.Utilities.Source.Generators.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace CuriousInc.Utilities.Source.Generators;

[Generator]
public class StronglyTypedIdGenerator : StronglyTypedIdGeneratorBase, IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource("StronglyTypedId.g.cs", SourceText.From(AttributeCode, Encoding.UTF8)));
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource("IStronglyTypedId.g.cs", SourceText.From(InterfaceCode, Encoding.UTF8)));
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource("ICreatable.g.cs", SourceText.From(ICreatableInterface, Encoding.UTF8)));

        var candidates = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (s, _) => s is RecordDeclarationSyntax rs
                                 && rs.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword)
                                 && rs.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword))
                                 && rs.Modifiers.Any(m => m.IsKind(SyntaxKind.ReadOnlyKeyword)),
                static (ctx, _) =>
                {
                    var sds = (RecordDeclarationSyntax)ctx.Node;

                    return new SyntaxProviderCandidate
                    {
                        Symbol = ctx.SemanticModel.GetDeclaredSymbol(sds),
                        Syntax = sds
                    };
                })
            .Where( static x => x is { Symbol: not null })
            .Where(static t => HasAttribute(t.Symbol!, AttributeName));

        var partials = GeneratePartialType(candidates);
        var jsonConverters = GenerateJsonConverter(candidates);

        context.RegisterSourceOutput(partials, static (spc, item) =>
        {
            var root = CSharpSyntaxTree.ParseText(item.Source).GetRoot();
            var pretty = root.NormalizeWhitespace().ToFullString();
            spc.AddSource(item.Hint, SourceText.From(pretty, Encoding.UTF8));
        });
        
        context.RegisterSourceOutput(jsonConverters, static (spc, item) =>
        {
            var root = CSharpSyntaxTree.ParseText(item.Source).GetRoot();
            var pretty = root.NormalizeWhitespace().ToFullString();
            spc.AddSource(item.Hint, SourceText.From(pretty, Encoding.UTF8));
        });
        
    }
}