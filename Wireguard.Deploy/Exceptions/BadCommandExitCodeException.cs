using System;

namespace ArkProjects.Wireguard.Deploy.Exceptions
{
    public class BadCommandExitCodeException : Exception
    {
        public int ExpectedCode { get; }
        public int ReturnedCode { get; }

        public BadCommandExitCodeException(string message, int returnedCode, int expectedCode = 0) : base(message)
        {
            ExpectedCode = expectedCode;
            ReturnedCode = returnedCode;
        }

        public BadCommandExitCodeException(int returnedCode, int expectedCode = 0) : this($"Expected {expectedCode} but return {returnedCode}", returnedCode, expectedCode)
        {
        }
    }
}