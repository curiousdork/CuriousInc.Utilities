namespace CuriousInc.Utilities.Source.Generators.Models;

public class GenerateSource
{
    public GenerateSource(string hint, string source)
    {
        Hint = hint;
        Source = source;
    }

    public string Hint { get; set; }
    public string Source { get; set; }
    
    public void Deconstruct(out string hint, out string source)
    {
        hint = Hint;
        source = Source;
    }
}