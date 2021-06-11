using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Telegram.Bot.Commands.Logs
{
    public class Logger
    {
        private readonly List<CommandsService> services = new List<CommandsService>();

        public delegate void LogHandler(string log);
        public event LogHandler OnLog;

        public Logger()
        { 
        
        }

        public Logger(IEnumerable<CommandsService> commands)
        {
            services.AddRange(commands.ToArray());

            foreach(var service in services)
                service.OnCommandExecuted += Service_OnCommandExecuted;
        }

        public Logger(CommandsService commandsService)
        {
            services.Add(commandsService);
            
            commandsService.OnCommandExecuted += Service_OnCommandExecuted;
        }

        public Logger AddService(CommandsService service)
        {
            services.Add(service);
            service.OnCommandExecuted += Service_OnCommandExecuted;

            return new Logger(services);
        }

        public Logger AddServices(IEnumerable<CommandsService> commandServices)
        {
            services.AddRange(commandServices.ToArray());

            foreach (var service in commandServices)
                service.OnCommandExecuted += Service_OnCommandExecuted;

            return new Logger(services);
        }

        private Task Service_OnCommandExecuted(CommandsService source, bool executionResult, Parsers.Command command)
        {
            OnLog?.Invoke($"[{DateTime.Now.ToShortTimeString()} | {DateTime.Now.ToShortDateString()}]" +
                $" Command \"{command}\" executed by user {command.ExecutedContext.User.Username}({command.ExecutedContext.User.Id}) in chat {command.ExecutedContext.Chat.Username ?? command.ExecutedContext.Chat.Id.ToString()} with result:" +
                $" {(executionResult ? "successful" : "failed")}.");
            return Task.CompletedTask;
        }
    }    
}
