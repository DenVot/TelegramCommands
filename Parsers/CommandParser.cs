using System;
using System.Collections.Generic;
using System.Reflection;
using Telegram.Bot.Attributes;

namespace Telegram.Bot.Commands.Parsers
{
    internal static class CommandParser
    {
        public static IEnumerable<MethodInfo> GetCommandsMethods(Type type)
        {
            if (type.BaseType == typeof(CommandsBase))
            {
                var allMethods = type.GetMethods();

                foreach (var method in allMethods)
                {
                    if (method.GetCustomAttribute<CommandAttribute>() != null)
                    {
                        var args = method.GetParameters();

                        if (args.Length == 0)
                            yield return method;
                        else
                            throw new InvalidOperationException($"{method} shouldn't have any arguments.");
                    }
                }
            }
            else
                throw new InvalidOperationException($"{type} not extends {typeof(CommandsBase)}.");
        }

        public static IEnumerable<MethodInfo> GetCommandsMethods<T>() => GetCommandsMethods(typeof(T));

        public static IEnumerable<Command> GetCommands(Type type)
        {
            var methods = GetCommandsMethods(type);

            foreach (var method in methods)
                yield return new Command(method);
        }

        public static IEnumerable<Command> GetCommands<T>() => GetCommands(typeof(T));
    }

    internal class Command
    {
        public string Name { get; }
        public string Description { get; }

        private readonly MethodInfo info;

        public Command(MethodInfo method)
        {
            var commAttr = method.GetCustomAttribute<CommandAttribute>();

            if (commAttr == null)
            {
                throw new InvalidOperationException($"Method {method.Name} doesn't have {typeof(CommandAttribute).FullName}");
            }

            Name = commAttr.Name;
            Description = commAttr.Description;
            info = method;
        }

        public bool Execute(CommandContext context)
        {
            try
            {
                var declType = info.DeclaringType;

                if (declType.BaseType == typeof(CommandsBase))
                {                    
                    var instance = Activator.CreateInstance(declType);

                    var property = declType.GetProperty("Context");

                    property.SetValue(instance, context);

                    info.Invoke(instance, new object[] { });                    
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
