namespace Parsec
{
    public sealed class Null
    {
        private static readonly Null NullInstance = new Null();

        public static Null Instance
        {
            get
            {
                return NullInstance;
            }
        }

        private Null()
        {
        }
    }
}