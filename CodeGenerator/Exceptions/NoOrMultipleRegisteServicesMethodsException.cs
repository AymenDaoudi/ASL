using System;

namespace CodeGenerator.Exceptions
{
    public class NoOrMultipleRegisteServicesMethodsException : Exception
    {
        public static readonly string NO_REGISTER_SERVICES_METHOD_ERROR_MESSAGE = "Cannot find RegisterServices method.";
        public static readonly string MULTIPLE_REGISTER_SERVICES_METHODS_ERROR_MESSAGE = "Multiple RegisterServices methods found.";

        public NoOrMultipleRegisteServicesMethodsException(string message) : base(message)
        {
        }

        public NoOrMultipleRegisteServicesMethodsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}