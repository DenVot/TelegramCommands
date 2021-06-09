using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace Telegram.Bot.Commands
{
    public class CommandsBase
    {
        public CommandContext Context { get; set; }

        private readonly Dictionary<int, CallbackQuery> MessagesCallbacksQuery = new Dictionary<int, CallbackQuery>();
        private readonly Dictionary<int, string> MessagesReplies = new Dictionary<int, string>();
        private readonly Dictionary<int, InlineQuery> MessagesInlineReplies = new Dictionary<int, InlineQuery>();

        protected InlineQuery WaitInline(Message message, TimeSpan span)
        {
            Stopwatch stopwatch = new Stopwatch();
            Context.ContextClient.OnInlineQuery += ContextClient_OnInlineQuery;
            MessagesInlineReplies.Add(message.MessageId, null);
            stopwatch.Start();

            while (stopwatch.Elapsed < span)
            {
                if (MessagesInlineReplies[message.MessageId] != null)
                {
                    Context.ContextClient.OnInlineQuery -= ContextClient_OnInlineQuery;
                    var item = MessagesInlineReplies[message.MessageId];
                    MessagesInlineReplies.Remove(message.MessageId);
                    return item;
                }
            }

            return null;
        }        

        protected string WaitReply(Message message, TimeSpan span)
        {
            Stopwatch stopwatch = new Stopwatch();
            Context.ContextClient.OnMessage += ContextClient_OnMessage;
            MessagesReplies.Add(message.MessageId, null);
            stopwatch.Start();

            while (stopwatch.Elapsed < span)
            {
                if (MessagesReplies[message.MessageId] != null)
                {
                    Context.ContextClient.OnMessage -= ContextClient_OnMessage;
                    var item = MessagesReplies[message.MessageId];
                    MessagesReplies.Remove(message.MessageId);
                    return item;
                }
            }

            return null;
        }

        protected CallbackQuery WaitCallback(Message message, TimeSpan span)
        {
            Stopwatch stopwatch = new Stopwatch();
            Context.ContextClient.OnCallbackQuery += ContextClient_OnCallbackQuery;
            MessagesCallbacksQuery.Add(message.MessageId, null);            
            stopwatch.Start();
            
            while (stopwatch.Elapsed < span)
            {
                if (MessagesCallbacksQuery[message.MessageId] != null)
                {
                    Context.ContextClient.OnCallbackQuery += ContextClient_OnCallbackQuery;
                    var item = MessagesCallbacksQuery[message.MessageId];
                    MessagesCallbacksQuery.Remove(message.MessageId);
                    return item;
                }
            }
            
            return null;
        }        

        private void ContextClient_OnCallbackQuery(object sender, CallbackQueryEventArgs e)
        {
            if (MessagesCallbacksQuery.ContainsKey(e.CallbackQuery.Message.MessageId))
            {
                MessagesCallbacksQuery[e.CallbackQuery.Message.MessageId] = e.CallbackQuery;
            }
        }

        private void ContextClient_OnMessage(object sender, MessageEventArgs e)
        {
            if (MessagesReplies.Count > 0)
            {
                MessagesReplies[MessagesReplies.Keys.Last()] = e.Message.Text;
            }            
        }

        private void ContextClient_OnInlineQuery(object sender, InlineQueryEventArgs e)
        {
            if (MessagesInlineReplies.Count > 0)
            {
                MessagesInlineReplies[MessagesInlineReplies.Keys.Last()] = e.InlineQuery;
            }
        }
    }
}
