using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Args;
using Telegram.Bot.Commands.Parsers;
using Telegram.Bot.Types;

namespace Telegram.Bot.Commands
{
    /// <summary>
    /// Help to create commands for Telegram bot.
    /// </summary>
    public class CommandsService
    {
        /// <summary>
        /// Initilize new instance of <see cref="CommandsService"/>.
        /// </summary>
        /// <param name="client">Telegram client.</param>        
        public CommandsService(ITelegramBotClient client)
        {
            client.OnMessage += Client_OnMessage;
            this.client = client;
        }

        private Type executorType = null;
        private IServiceProvider provider = null;
        private bool slashCommands = true;

        private readonly ITelegramBotClient client;

        public IEnumerable<Command> Commands { get; private set; }

        /// <summary>
        /// Register commands.
        /// </summary>
        /// <param name="services">Provider that contains params for constructor.</param>
        /// <typeparam name="T">Class that has void-s with <see cref="Attributes.CommandAttribute"/>.</typeparam>
        public void RegisterCommands<T>(IServiceProvider services = null, bool slash = true) where T : CommandsBase
        {
            var type = typeof(T);
            Commands = CommandParser.GetCommands(type);
            executorType = type; 
            
            if (services == null)
                provider = new ServiceCollection()
                    .BuildServiceProvider();
            else
                provider = services;

            var validate = CommandParser.ValidateConstructors(executorType, provider, out _);
            if (!validate)
                throw new InvalidOperationException("Can't find constructor with parameters.");

            slashCommands = slash;
        }

        /// <summary>
        /// Upload commands to Telegram.
        /// </summary>        
        public async Task UploadCommandsAsync()
        {
            if (!slashCommands)
                throw new InvalidOperationException("If you want to upload commands, they should be slash commands!");

            var list = new List<BotCommand>();

            foreach (var command in CommandParser.GetCommands(executorType))
            {
                list.Add(new BotCommand
                { 
                    Command = command.Name.ToLower(),
                    Description = command.Description
                });
            }

            await client.SetMyCommandsAsync(list);
        }

        private void Client_OnMessage(object sender, MessageEventArgs e)
        {
            if (executorType != null && e.Message.Text != null)
            {
                var context = new CommandContext(client, e.Message.Chat, e.Message.From, e.Message);
                var commands = CommandParser.GetCommands(executorType);

                if ((commands.Any(x => "/" + x.Name.ToLower() == e.Message.Text) && slashCommands) || (commands.Any(x => x.Name.ToLower() == e.Message.Text.ToLower()) && !slashCommands))
                {
                    var command = commands.FirstOrDefault(x => (slashCommands ? "/" : null) + x.Name.ToLower() == e.Message.Text.ToLower());
                    CommandParser.GetConstructorBuildData(executorType, provider, out object[] parameters);
                    command.Execute(context, parameters);
                }
            }
        }
    }
}
