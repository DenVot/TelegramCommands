using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Telegram.Bot.Commands
{
    public class CommandsServices : IEnumerable<CommandsService>
    {
        private readonly List<CommandsService> Services = new List<CommandsService>();
        private readonly ITelegramBotClient client;

        public CommandsServices(ITelegramBotClient client)
        {
            this.client = client;
        }

        public void CreateService<T>(IServiceProvider provider = null, bool slash = true) where T : CommandsBase
        {
            var service = new CommandsService(client);

            service.RegisterCommands<T>(provider, slash);
            Services.Add(service);
        }

        public void AddService(CommandsService service)
        {
            Services.Add(service);
        }

        public void RemoveService(CommandsService service)
        {
            Services.Remove(service);
        }

        public async Task UploadCommandsAsync()
        {
            List<BotCommand> commands = new List<BotCommand>();

            foreach (var service in this)
            {
                foreach (var comm in service.Commands)
                {
                    commands.Add(new BotCommand
                    { 
                        Command = comm.Name.ToLower(),
                        Description = comm.Description
                    });
                }
            }

            if (commands.Count > 0)
                await client.SetMyCommandsAsync(commands);
            else
                throw new InvalidOperationException("No commands to upload.");
        }

        public IEnumerator<CommandsService> GetEnumerator() => Services.GetEnumerator();        

        IEnumerator IEnumerable.GetEnumerator() => Services.GetEnumerator();
    }
}
