using Telegram.Bot.Types;

namespace Telegram.Bot.Commands
{
    /// <summary>
    /// Command context that contains main data to create dialog with user.
    /// </summary>
    public class CommandContext
    {
        /// <summary>
        /// Telegram client.
        /// </summary>
        public ITelegramBotClient ContextClient { get; }
        /// <summary>
        /// Chat where message has received.
        /// </summary>
        public Chat Chat { get; }
        /// <summary>
        /// Author of message.
        /// </summary>
        public User User { get; }        
        /// <summary>
        /// Message.
        /// </summary>
        public Message Message { get; }

        /// <summary>
        /// Initilize new context.
        /// </summary>
        /// <param name="client">Context client.</param>
        /// <param name="chat">Chat where message has received.</param>
        /// <param name="user">Author of message.</param>
        /// <param name="message">Message.</param>
        public CommandContext(ITelegramBotClient client, Chat chat, User user, Message message)
        {
            ContextClient = client;
            Chat = chat;
            User = user;
            Message = message;
        }
    }
}
