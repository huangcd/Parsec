namespace Parsec.Core
{
    public interface ITokenStream<out TToken>
    {
        ITokenStream<TToken> MoveNext();

        IOptional<TToken> Current { get; }
    }
}
