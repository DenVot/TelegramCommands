using System;
using System.Reflection;

namespace Telegram.Bot.Exceptions.CommandsException
{
    public class InvalidMethodException : SystemException
    {
        public MethodInfo ExceptionMethod { get; }

        public InvalidMethodException(MethodInfo method, string message = null) : base(message ?? $"Invalid {method.Name} method.")
        {
            ExceptionMethod = method;
        }
    }
}
