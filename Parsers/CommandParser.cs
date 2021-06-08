﻿using System;
using System.Collections.Generic;
using System.Linq;
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

        public static bool ValidateConstructors(Type type, IServiceProvider provider, out ConstructorInfo potentConstructor)
        {            
            List<ConstructorInfo> constructors = new List<ConstructorInfo>();

            foreach (var constructor in type.GetConstructors())
            {
                var parameters = constructor.GetParameters();
                bool canBe = true;

                foreach (var param in parameters)
                {
                    var paramType = param.ParameterType;
                    if (provider.GetService(paramType) == null)
                        canBe = false;
                }
                if (canBe)
                    constructors.Add(constructor);
            }

            if (constructors.Count > 0)
            {
                constructors = constructors.OrderBy(x => x.GetParameters().Length).ToList();
                potentConstructor = constructors[constructors
                    .Select(x => x.GetParameters().Length)
                    .ToList()
                    .IndexOf(constructors
                    .Select(x => x.GetParameters().Length).Max())];

                return true;
            }
            else
            {
                potentConstructor = null;
                return false;
            }
        }

        public static bool ValidateConstructors<T>(IServiceProvider provider, out ConstructorInfo potentConstructor) where T : CommandsBase => ValidateConstructors(typeof(T), provider, out potentConstructor);

        public static ConstructorInfo GetConstructorBuildData<T>(IServiceProvider provider, out object[] parameters) where T : CommandsBase => GetConstructorBuildData(typeof(T), provider, out parameters);

        public static ConstructorInfo GetConstructorBuildData(Type type, IServiceProvider provider, out object[] parameters)
        {
            var able = ValidateConstructors(type, provider, out ConstructorInfo result);

            if (able)
            {
                var parametersTypes = result.GetParameters().Select(x => x.ParameterType).ToArray();
                parameters = new object[result.GetParameters().Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    parameters[i] = provider.GetService(parametersTypes[i]);
                }

                return result;
            }
            else
            {
                parameters = new object[0];
                return null;
            }
        }

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

        public bool Execute(CommandContext context, object[] constructorParams)
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
