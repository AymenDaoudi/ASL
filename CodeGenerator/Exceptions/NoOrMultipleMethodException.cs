﻿using System;

namespace CodeGenerator.Roslyn.Exceptions
{
    public class NoOrMultipleMethodException : Exception
    {
        public static readonly string NO_METHOD_ERROR_MESSAGE = "Cannot find {0} method.";
        public static readonly string MULTIPLE_METHODS_ERROR_MESSAGE = "Multiple {0} methods found.";

        public NoOrMultipleMethodException(string message) : base(message)
        {
        }

        public NoOrMultipleMethodException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}