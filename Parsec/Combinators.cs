using Parsec.Core;

namespace Parsec
{
    public static class Combinators
    {
        public static Parser<TToken, TPos, TOutput> And<TToken, TPos, TOutput>(
            Parser<TToken, TPos, TOutput> leftParser,
            Parser<TToken, TPos, TOutput> rightParser)
        {
            return stream => leftParser(stream).Match(
                    failure: Result.Failure<TToken, TPos, TOutput>,
                    success: (restStream, output) => rightParser(stream));
        }

        public static Parser<TToken, TPos, TOutput> Or<TToken, TPos, TOutput>(
            Parser<TToken, TPos, TOutput> leftParser,
            Parser<TToken, TPos, TOutput> rightParser)
        {
            return stream => leftParser(stream).Match(
                    failure: (restStream, output) => rightParser(stream),
                    success: Result.Success);
        }
    }
}
