namespace Parsec.Core
{
    public interface IPair<out TToken, out TPos>
    {
        TToken Token { get; }

        TPos Pos { get; }
    }

    public static class Pair
    {
        public static IPair<TToken, TPos> Create<TToken, TPos>(TToken token, TPos pos)
        {
            return new PairImpl<TToken, TPos>(token, pos);
        }

        private sealed class PairImpl<TToken, TPos> : IPair<TToken, TPos>
        {
            internal PairImpl(TToken token, TPos pos)
            {
                Token = token;
                Pos = pos;
            }

            public TToken Token { get; private set; }

            public TPos Pos { get; private set; }
        }
    }
}