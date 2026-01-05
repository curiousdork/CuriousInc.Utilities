namespace CuriousInc.Common.Functional.Monads;

public readonly record struct Unit
{
    public static Unit Default => new();
    
    public static Task<Unit> Async => Task.FromResult(Default);
}