// using Headquarters.Utilities.Common.Monads;
//
// namespace Headquarters.Utilities.Common.Extensions;
//
// public static class MatchAsyncExtensions
// {
//     extension<T>(Either<Error, T> value)
//     {
//         public async Task<Either<Error, TResult>> MatchAsync<TResult>(Func<T, Task<Either<Error, TResult>>> success,
//             Func<Error, Task<Either<Error, TResult>>> failed)
//         {
//             return value.IsRight switch
//             {
//                 true => await success(value.Unwrap()),
//                 false => await failed(value.UnwrapError())
//             };
//         }
//         
//     }
//
//     extension<T>(Task<Either<Error, T>> value)
//     {
//         public async Task<Either<Error, TResult>> MatchAsync<TResult>(Func<T, Task<Either<Error, TResult>>> success,
//             Func<Error, Task<Either<Error, TResult>>> failed)
//         {
//             var temp = await value;
//             return temp.IsRight switch
//             {
//                 true => await success(temp.Unwrap()),
//                 false => await failed(temp.UnwrapError())
//             };
//         }
//     }
// }