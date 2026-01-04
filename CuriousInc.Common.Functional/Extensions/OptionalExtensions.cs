using CuriousInc.Common.Functional.Monads;

namespace CuriousInc.Common.Functional.Extensions;

public static class OptionalExtensions
{
    extension<TStruct>(TStruct? value) where TStruct : struct
    {
        public Option<TStruct> ToOption()
        {
            return value.HasValue switch
            {
                true => Option<TStruct>.Some(value.Value),
                false => Option<TStruct>.None,
            };
        }
    }

    extension<T>(T? value) where T : class
    {
        public Option<T> ToOption()
        {
            return value switch
            {
                null => Option<T>.None,
                _ => Option<T>.Some(value),
            };
        }
    }
}