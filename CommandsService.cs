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
        private readonly ITelegramBotClient client;

        /// <summary>
        /// Register commands.
        /// </summary>
        /// <typeparam name="T">Class that has void-s with <see cref="Attributes.CommandAttribute"/>.</typeparam>
        public void RegisterCommands<T>() where T : CommandsBase
        {
            var type = typeof(T);
            CommandParser.GetCommandsMethods(type); //Validation of commands
            executorType = type;
        }

        /// <summary>
        /// Upload commands to Telegram.
        /// </summary>        
        public async Task UploadCommands()
        {
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

                if (commands.Any(x => "/" + x.Name.ToLower() == e.Message.Text))
                {
                    var command = commands.FirstOrDefault(x => "/" + x.Name.ToLower() == e.Message.Text);

                    command.Execute(context);
                }
            }
        }
    }
}
