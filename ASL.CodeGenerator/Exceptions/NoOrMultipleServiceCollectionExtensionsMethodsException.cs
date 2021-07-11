using System;

namespace ASL.CodeGenerator.Exceptions
{
    public class NoOrMultipleServiceCollectionExtensionsMethodsException : Exception
    {
        public static readonly string NO_METHOD_ERROR_MESSAGE = "Cannot find {0} method in IServiceCollectionExtensions class.";
        public static readonly string MULTIPLE_METHODS_ERROR_MESSAGE = "Multiple {0} methods found in IServiceCollectionExtensions class.";

        public NoOrMultipleServiceCollectionExtensionsMethodsException(string message) : base(message)
        {
        }

        public NoOrMultipleServiceCollectionExtensionsMethodsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}