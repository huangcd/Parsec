namespace Parsec.Core
{
    public sealed class Nothing
    {
        private static readonly Nothing NothingInstance = new Nothing();

        public static Nothing Instance
        {
            get
            {
                return NothingInstance;
            }
        }

        private Nothing()
        {
        }
    }
}