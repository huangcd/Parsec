namespace Parsec.Core
{
    public interface ITokenStream<out TToken, out TPos>
    {
        ITokenStream<TToken, TPos> MoveNext();

        IOptional<IPair<TToken, TPos>> Current { get; }
    }
}
