using System;

namespace Telegram.Bot.Exceptions.CommandsException
{
    public class ExtendException : SystemException
    {
        public Type Type { get; }
        public Type BaseType { get; }

        public ExtendException(Type type, Type extendType) : base($"{type} not extends {extendType}")
        {
            Type = type;
            BaseType = extendType;
        }
    }
}
