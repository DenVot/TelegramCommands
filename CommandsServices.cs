using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Telegram.Bot.Commands
{
    /// <summary>
    /// Collection of <see cref="CommandsService"/>.
    /// </summary>
    public class CommandsServices : IEnumerable<CommandsService>
    {
        private readonly List<CommandsService> Services = new List<CommandsService>();
        private readonly ITelegramBotClient client;

        /// <summary>
        /// Ininitlize new instance of <see cref="CommandsServices"/>.
        /// </summary>
        /// <param name="client">Telegram client</param>
        public CommandsServices(ITelegramBotClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Create new command service with specified type which extends <see cref="CommandsBase"/>.
        /// </summary>
        /// <typeparam name="T">Type which extends <see cref="CommandsBase"/>.</typeparam>
        /// <param name="provider">Provider which contains parameters for constructor.</param>
        /// <param name="slash">Are slash commands?</param>
        public void CreateService<T>(IServiceProvider provider = null, bool slash = true) where T : CommandsBase
        {
            var service = new CommandsService(client);

            service.RegisterCommands<T>(provider, slash);
            Services.Add(service);
        }

        /// <summary>
        /// Adds service to collection.
        /// </summary>
        /// <param name="service">Service.</param>
        public void AddService(CommandsService service)
        {
            Services.Add(service);
        }

        /// <summary>
        /// Remove service from collection.
        /// </summary>
        /// <param name="service">Service.</param>
        public void RemoveService(CommandsService service)
        {
            Services.Remove(service);
        }

        /// <summary>
        /// Uploads all commands to Telegram.
        /// </summary>        
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

        /// <inheritdoc/>
        public IEnumerator<CommandsService> GetEnumerator() => Services.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => Services.GetEnumerator();
    }
}
