namespace Parsec.Core
{
    public delegate IResult<TToken, TOutput, TPos> Parser<TToken, out TOutput, TPos>(ITokenStream<TToken, TPos> stream);
}
