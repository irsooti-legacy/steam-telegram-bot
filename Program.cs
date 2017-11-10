using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using SteamInfoPlayerBot.Services;
using Telegram.Bot.Types.Enums;

namespace SteamInfoPlayerBot
{
    class Program
    {
        private static readonly TelegramBotClient _botClient = new Telegram.Bot.TelegramBotClient(Environment.GetEnvironmentVariable("TELEGRAM_TOKEN"));
        private static readonly SteamService _steamClient = new SteamService(Environment.GetEnvironmentVariable("STEAM_TOKEN"));
        public static void Main()
        {
            // Initialize the bot
            // InitBotAsync().GetAwaiter().GetResult();
            // Bot starts to hear
            _botClient.StartReceiving();
            _botClient.OnMessage += hearMessages;
            _botClient.OnReceiveError += BotOnReceiveError;
            Console.ReadLine();
            _botClient.StopReceiving();
            // BuildWebHost(args).Run();
        }

        static async Task InitBotAsync()
        {
            var me = await _botClient.GetMeAsync();
            System.Console.WriteLine("Hello! My name is " + me.FirstName);
        }

        private static async void hearMessages(object sender, MessageEventArgs e)
        {
            if ((e.Message.Text is null) || (e.Message.Type != MessageType.TextMessage)) return;
            if (e.Message.Text.StartsWith("/steam"))
            {
                string[] str = e.Message.Text.Split("/steam ");
                if (str.Length < 2)
                {
                    await _botClient.SendTextMessageAsync(e.Message.Chat.Id, "Errore, l'argomento è vuoto");
                    return;
                }
                string msg = e.Message.Text.Split("/steam ")[1];

                try
                {
                    var playerInfo = await _steamClient.PlayerInfo(msg);

                    string optCaption = "";
                    if (playerInfo.PlayingGameId != null)
                        optCaption =
                            $"◾️ Is playing at {playerInfo.PlayingGameName}\n";
                    else optCaption = $"◾️ Last seen: {playerInfo.LastLoggedOffDate}";


                    string caption =
                        $"◾️ Nick: {playerInfo.Nickname}\n" +
                        $"◾️ Real Name: {playerInfo.RealName}\n" +
                        $"◾️ Status: {playerInfo.UserStatus}\n" +
                        $"{optCaption}";
                    await _botClient.SendPhotoAsync(e.Message.Chat.Id, new Telegram.Bot.Types.FileToSend(playerInfo.AvatarFullUrl), caption);
                }

                catch (System.Exception err)
                {
                    await _botClient.SendTextMessageAsync(e.Message.Chat.Id, err.Message);
                }

            }
        }
        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Debugger.Break();
        }
    }
}
