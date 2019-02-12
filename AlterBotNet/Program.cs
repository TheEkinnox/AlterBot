using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;


namespace AlterBotNet
{
    class Program
    {
        private DiscordSocketClient _client;
        private CommandService _commands;

        static void Main(string[] args) 
            => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            this._client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug
            });

            this._commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });

            this._client.MessageReceived += Client_MessageReceived;
            await this._commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            this._client.Ready += Client_Ready;
            this._client.Log += Client_Log;
            this._client.UserJoined += AnnounceUserJoined;

            string token;
            using (FileStream stream = new FileStream((Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).Replace(@"bin\Debug\netcoreapp2.1", @"Data\Token.txt"), FileMode.Open, FileAccess.Read))
            using (StreamReader readToken = new StreamReader(stream))
            {
                token = readToken.ReadToEnd();
            }
                await this._client.LoginAsync(TokenType.Bot, token);
                await this._client.StartAsync();

                await Task.Delay(-1);
        }

        private async Task AnnounceUserJoined(SocketGuildUser user)
        {
            var guild = user.Guild;
            var channel = guild.SystemChannel;
            string guildName = guild.Name;
            if (guildName == "ServeurTest")
            {
                //await this.Context.Channel.SendMessageAsync("Bienvenue sur Alternia " + this.Context.User.Mention + "! Toutes les infos pour faire ta fiche sont ici :\n<#" + GetChannelByName("contexte-rp", guildName) + ">\n<#" + GetChannelByName("geographie-de-alternia", guildName) + ">\n" + GetChannelByName("banque", guildName) + "\n" + GetChannelByName("regles", guildName) + "\n" + GetChannelByName("liens-utiles", guildName) + "\n" + GetChannelByName("fiche-prototype", guildName) + "\n" + GetChannelByName("les-races-disponibles", guildName) +
                //                                            "\nSi tu as besoins d'aide n'hésite pas à demander à un membre du " + this.Context.Guild.GetRole(541492279894999080).Mention + "!", false, null, null);
                await channel.SendMessageAsync("Bienvenue sur Alternia " + user.Mention + "! Toutes les infos pour faire ta fiche sont ici :\n<#542072451324968972>\n<#542070741504360458>\n<#541493264180707338>\n<#542070805236940837>\n<#542072285033660437>\n<#542073013722546218>\n<#542073051790049302>" +
                                                            "\nSi tu as besoins d'aide n'hésite pas à demander à un membre du " + guild.GetRole(541492279894999080).Mention + " !", false, null, null);
            }
            else
            {
                await channel.SendMessageAsync("Bienvenue sur Alternia " + user.Mention + "! Toutes les infos pour faire ta fiche sont ici :\n<#410438433849212928>\n<#410531350102147072>\n<#411969883673329665>\n<#409789542825197568>\n<#409849626988904459>\n<#410424057050300427>\n<#410487492463165440>" +
                                                            "\nSi tu as besoins d'aide n'hésite pas à demander à un membre du " + guild.GetRole(420536907525652482).Mention + " !", false, null, null);
            }
        }

        private async Task Client_Log(LogMessage message)
        {
            Console.WriteLine($"{DateTime.Now} at {message.Source} {message.Message}");
        }

        private async Task Client_Ready()
        {
            await this._client.SetGameAsync("prefix: a! ^^ @AlterBot");
        }

        private async Task Client_MessageReceived(SocketMessage messageParam)
        {
            var message = messageParam as SocketUserMessage;
            var context = new SocketCommandContext(this._client, message);

            if (context.Message == null || context.Message.Content == "") return;
            if (context.User.IsBot) return;

            int argPos = 0;
            if (!(message.HasStringPrefix("^^", ref argPos) || message.HasStringPrefix("a!", ref argPos) || message.HasMentionPrefix(this._client.CurrentUser, ref argPos))) return;

            var result = await this._commands.ExecuteAsync(context, argPos, null);
            if (!result.IsSuccess)
            {
                Console.WriteLine($"{DateTime.Now} at Commands] Une erreur s'est produite en exécutant une commande. Texte: {context.Message.Content} | Erreur: {result.ErrorReason}");
            }

            if (!result.IsSuccess && result.ErrorReason.Contains("Unknown command"))
            {
                await context.Channel.SendMessageAsync($"La commande **{context.Message.Content}** n'existe pas");
            }

        }
    }
}
