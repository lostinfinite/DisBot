using System;
using System.Diagnostics;
using System.ServiceProcess;
using Discord;
using Discord.WebSocket;

namespace DiscordBotService
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please read and accept the End User License Agreement (EULA) before proceeding.");

            string eulaPath = @"https://github.com/lostinfinite/DisBot/raw/main/End%20User%20License%20Agreement%20(EULA).pdf";
            Process.Start(eulaPath);

            Console.WriteLine("Do you accept the terms of the EULA? (y/n)");
            string response = Console.ReadLine();
            if (response.ToLower() != "y")
            {
                Console.WriteLine("You must accept the EULA to use this program.");
                return;
            }

            Console.WriteLine("Please enter your Discord bot token:");
            var token = Console.ReadLine();

            if (!IsValidCode(token))
            {
                Console.WriteLine("Invalid Discord bot token.");
                return;
            }

            var botName = GetBotName(token);

            var service = new DiscordBotService(botName, token);
            ServiceBase.Run(service);
        }

        static bool IsValidCode(string token)
        {
            return token.Contains("discord.rs") || token.Contains("discord.js") || token.Contains("discord.py");
        }

        static string GetBotName(string token)
        {
            var client = new DiscordSocketClient();
            client.LoginAsync(TokenType.Bot, token).Wait();
            return client.CurrentUser.Username;
        }
    }

    public class DiscordBotService : ServiceBase
    {
        private readonly string _botName;
        private readonly string _token;
        private DiscordSocketClient _client;

        public DiscordBotService(string botName, string token)
        {
            _botName = botName;
            _token = token;
        }

        protected override void OnStart(string[] args)
        {
            _client = new DiscordSocketClient();
            _client.LoginAsync(TokenType.Bot, _token).Wait();
            _client.StartAsync().Wait();
            Console.WriteLine($"{_botName} is running in the background.");
        }

        protected override void OnStop()
        {
            _client.StopAsync().Wait();
            Console.WriteLine($"{_botName} has been stopped.");
        }
    }
}
