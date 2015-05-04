using System;

namespace Parsec.Core
{
    public interface IError
    {
        String Message { get; }
    }

    public static class Error
    {
        public static IError Create(String message)
        {
            return new ErrorImpl(message);
        }

        public static IError EndOfInput(String message)
        {
            return new EndOfInputImpl(message);
        }

        private sealed class EndOfInputImpl : IError
        {
            internal EndOfInputImpl(String message)
            {
                Message = message;
            }

            public string Message { get; private set; }

            public Nothing Pos { get { return Nothing.Instance; } }
        }

        private sealed class ErrorImpl : IError
        {
            internal ErrorImpl(String message)
            {
                Message = message;
            }

            public string Message { get; private set; }
        }
    }
}