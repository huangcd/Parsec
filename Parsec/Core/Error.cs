using System;

namespace Parsec.Core
{
    public interface IError
    {
        string Message { get; }
    }

    public static class Error
    {
        public static IError Create(string message)
        {
            return new ErrorImpl(message);
        }

        public static IError EndOfInput(string message)
        {
            return new EndOfInputImpl(message);
        }

        private sealed class EndOfInputImpl : IError
        {
            internal EndOfInputImpl(string message)
            {
                Message = message;
            }

            public string Message { get; private set; }

            public Nothing Pos { get { return Nothing.Instance; } }
        }

        private sealed class ErrorImpl : IError
        {
            internal ErrorImpl(string message)
            {
                Message = message;
            }

            public string Message { get; private set; }
        }
    }
}