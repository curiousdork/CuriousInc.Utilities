using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CuriousInc.Utilities.Source.Generators.Models;

public class SyntaxProviderCandidate
{
    public ClassDeclarationSyntax? ClassSyntax { get; set; } = null;
    public RecordDeclarationSyntax? Syntax { get; set; } = null;
    public INamedTypeSymbol? Symbol { get; set; } = null;

    public void Deconstruct(out INamedTypeSymbol symbol)
    {
        symbol = Symbol!;
    }
    
    public void Deconstruct(out INamedTypeSymbol symbol, out RecordDeclarationSyntax syntax)
    {
        symbol = Symbol!;
        syntax = Syntax!;
    }
    public void Deconstruct(out INamedTypeSymbol symbol, out RecordDeclarationSyntax syntax, out ClassDeclarationSyntax? classSyntax)
    {
        symbol = Symbol!;
        syntax = Syntax!;
        classSyntax = ClassSyntax;
    }
}