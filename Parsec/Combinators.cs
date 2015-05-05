using System;
using System.Collections.Generic;
using System.Linq;
using Parsec.Core;

namespace Parsec
{
    public static class Combinators
    {
        public static Parser<TToken, TOutputRight> And<TToken, TOutputLeft, TOutputRight>(
            this Parser<TToken, TOutputLeft> leftParser,
            Parser<TToken, TOutputRight> rightParser)
        {
            return stream => leftParser(stream).Match(
                failure: Result.Failure<TToken, TOutputRight>,
                success: (restStream1, output1) => rightParser(restStream1).Match(
                    failure: Result.Failure<TToken, TOutputRight>,
                    success: Result.Success));
        }

        public static Parser<TToken, TOutput> And<TToken, TOutput1, TOutput2, TOutput>(
            this Parser<TToken, TOutput1> leftParser,
            Parser<TToken, TOutput2> rightParser,
            Func<TOutput1, TOutput2, TOutput> resultCombiner)
        {
            return stream => leftParser(stream).Match(
                failure: Result.Failure<TToken, TOutput>,
                success: (restStream1, output1) => rightParser(restStream1).Match(
                    failure: Result.Failure<TToken, TOutput>,
                    success: (restStream2, output2) => Result.Success(restStream2, resultCombiner(output1, output2))));
        }

        public static Parser<TToken, TOutput> Or<TToken, TOutput>(
            this Parser<TToken, TOutput> leftParser,
            Parser<TToken, TOutput> rightParser)
        {
            return stream => leftParser(stream).Match(
                    failure: (restStream, output) => rightParser(stream),
                    success: Result.Success);
        }

        public static Parser<TToken, TOutput> Any<TToken, TOutput>(
            this IEnumerable<Parser<TToken, TOutput>> parsers)
        {
            return stream =>
            {
                IResult<TToken, TOutput> result = Result.Failure<TToken, TOutput>(stream, Error.Create("No parser"));
                foreach (var parser in parsers)
                {
                    result = parser(stream);
                    if (result.Success())
                    {
                        return result;
                    }
                }
                return result;
            };
        }

        public static Parser<TToken, TOutput> Any<TToken, TOutput>(
            params Parser<TToken, TOutput>[] parsers)
        {
            return parsers.Any();
        }

        public static Parser<TToken, TOutput> Where<TToken, TOutput>(
            this Parser<TToken, TOutput> parser,
            Func<TOutput, Boolean> pred)
        {
            return stream => parser(stream).Match(
                failure: Result.Failure<TToken, TOutput>,
                success: (restStream, output) => pred(output)
                                                 ? Result.Success(restStream, output)
                                                 : Result.Failure<TToken, TOutput>(restStream, Error.Create("Pred failed")));
        }

        public static Parser<TToken, TOutput> Select<TToken, TIntermediate, TOutput>(
            this Parser<TToken, TIntermediate> parser,
            Func<TIntermediate, TOutput> selector)
        {
            return stream => parser(stream).Match(
                failure: Result.Failure<TToken, TOutput>,
                success: (restStream, output) => Result.Success(restStream, selector(output)));
        }

        public static Parser<TToken, TOutput> SelectMany<TToken, TIntermediate1, TIntermediate2, TOutput>(
            this Parser<TToken, TIntermediate1> parser,
            Func<TIntermediate1, Parser<TToken, TIntermediate2>> selector,
            Func<TIntermediate1, TIntermediate2, TOutput> projector)
        {
            return stream => parser(stream).Match(
                failure: Result.Failure<TToken, TOutput>,
                success: (restStream1, intermediate1) =>
                {
                    var parser1 = selector(intermediate1);
                    return parser1(restStream1).Match(
                        failure: Result.Failure<TToken, TOutput>,
                        success: (restStream2, intermediate2) => Result.Success(restStream2, projector(intermediate1, intermediate2)));
                });
        }

        public static Parser<TToken, IList<TOutput>> Sequence<TToken, TOutput>(
            this IEnumerable<Parser<TToken, TOutput>> parsers)
        {
            return stream => parsers.Aggregate(
                Succeed<TToken, IList<TOutput>>(new TOutput[0])(stream),
                (result, parser) => result.Match(
                    failure: Result.Failure<TToken, IList<TOutput>>,
                    success: (restStream, output) => parser(restStream).Match(
                        failure: Result.Failure<TToken, IList<TOutput>>,
                        success: (restRestStream, parserOutput) => Result.Success(restRestStream, output.Concat(new[] { parserOutput }).ToArray()))));
        }

        public static Parser<TToken, IList<TOutput>> Sequence<TToken, TOutput>(
            params Parser<TToken, TOutput>[] parsers)
        {
            return parsers.Sequence();
        }

        public static Parser<TToken, TOutput> Succeed<TToken, TOutput>(TOutput output)
        {
            return stream => Result.Success(stream, output);
        }

        public static Parser<TToken, TOutput[]> Repeat<TToken, TOutput>(
            this Parser<TToken, TOutput> parser)
        {
            return Repeat1(parser).Or(Succeed<TToken, TOutput[]>(new TOutput[0]));
        }

        public static Parser<TToken, TOutput[]> Repeat1<TToken, TOutput>(
            this Parser<TToken, TOutput> parser)
        {
            // Raw implementation
            //return stream => parser(stream).Match(
            //    failure: Result.Failure<TToken, TOutput[]>,
            //    success: (restStream, output) =>
            //    {
            //        return parser.Repeat()(restStream).Match(
            //            failure: (repeatRestStream, error) => Result.Success(repeatRestStream, new[] { output }),
            //            success: (repeatRestStream, repeatOutput) => Result.Success(repeatRestStream, new[] { output }.Concat(repeatOutput).ToArray()));
            //    });

            // Linq implementation
            // return parser.SelectMany(x => Repeat(parser), (x, xs) => (new[] { x }).Concat(xs).ToArray());
            return from x in parser
                   from xs in Repeat(parser)
                   select (new[] { x }).Concat(xs).ToArray();
        }
    }
}
