using System;
using System.Reflection;
using Telegram.Bot.Attributes;
using Telegram.Bot.Exceptions.CommandsException;

namespace Telegram.Bot.Commands.Parsers
{
    public class Command
    {
        public override string ToString()
        {
            return Name;
        }

        public string Name { get; }
        public string Description { get; }

        internal CommandContext ExecutedContext { get; private set; }

        private readonly MethodInfo info;

        internal Command(MethodInfo method)
        {
            var commAttr = method.GetCustomAttribute<CommandAttribute>();

            if (commAttr == null)
            {
                throw new InvalidMethodException(method, $"Method {method.Name} doesn't have {typeof(CommandAttribute).FullName}");
            }

            Name = commAttr.Name;
            Description = commAttr.Description;
            info = method;
        }

        internal bool Execute(CommandContext context, object[] constructorParams)
        {
            try
            {
                var declType = info.DeclaringType;

                if (declType.BaseType == typeof(CommandsBase))
                {
                    var instance = Activator.CreateInstance(declType, constructorParams);

                    var property = declType.GetProperty("Context");

                    property.SetValue(instance, context);

                    info.Invoke(instance, new object[] { });

                    ExecutedContext = context;

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
