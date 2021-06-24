using System;

namespace CodeGenerator.Exceptions
{
    public class NoOrMultipleRegisterRepositoriesMethodsException : Exception
    {
        public static readonly string NO_REGISTER_REPOSITORIES_METHOD_ERROR_MESSAGE = "Cannot find RegisterRepositories method.";
        public static readonly string MULTIPLE_REGISTER_REPOSITORIES_METHODS_ERROR_MESSAGE = "Multiple RegisterRepositories methods found.";

        public NoOrMultipleRegisterRepositoriesMethodsException(string message) : base(message)
        {
        }

        public NoOrMultipleRegisterRepositoriesMethodsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}