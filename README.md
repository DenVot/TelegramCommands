<p align="center">
    <img src="Branding/logo.jpg">
</p>

# Telegram commands.

Extension of `Telegram.Bot` can help you to make your commands faster. New namespace `Telegram.Bot.Commands` contains classes and interface that can execute your command.

## Quick start

Install package by 

Package Manager:
```
Install-Package TelegramCommands -Version 1.2.0
```

.NET CLI:
```
dotnet add package TelegramCommands --version 1.2.0
```

**Example:**

```csharp
using System;
using Telegram.Bot;
using Telegram.Bot.Commands;

namespace QuickStart
{
    public static class Program
    {
        static ITelegramBotClient botClient;

        static void Main()
        {
            botClient = new TelegramBotClient("YOUR_TOKEN_HERE");

            var service = new CommandsService(botClient); //Initlize instance service.

            service.RegisterCommands<Commands>(); //Register your commands.
            service.UploadCommandsAsync().GetAwaiter().GetResult(); //Upload your commands to Telegram.

            botClient.StartReceiving();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            botClient.StopReceiving();
        }
    }

    public class Commands : CommandsBase
    {
        //   Command name   Descripion
        [Command("Ping", "Replies pong")]
        public async void Ping()
        {
            var client = Context.ContextClient;//Get ITelegramBotClient via Context

            await client.SendTextMessageAsync(Context.Chat, "Pong");//Send message.
        }
    }
}
```

For more additional information with examples see wiki.
