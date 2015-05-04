namespace Parsec.Core
{
    public delegate IResult<TToken, TOutput> Parser<TToken, out TOutput>(ITokenStream<TToken> stream);
}
