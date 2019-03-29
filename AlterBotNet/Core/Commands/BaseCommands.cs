#region MÉTADONNÉES

// Nom du fichier : BaseCommands.cs
// Auteur : Loick OBIANG (1832960)
// Date de création : 2019-02-04
// Date de modification : 2019-03-22

#endregion

#region USING

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AlterBotNet.Core.Data.Classes;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;

#endregion

namespace AlterBotNet.Core.Commands
{
    public class BaseCommands : ModuleBase<SocketCommandContext>
    {
        #region ATTRIBUTS

        private Random _rand = new Random();

        #endregion

        #region MÉTHODES
        [Command("ping"),Summary("Envoie le temps de latence (en ms) entre le bot et discord")]
        public async Task SendPing()
        {
            int latence = Global.Client.Latency;

            await ReplyAsync($"Latence: {latence}ms");
        }


        [Command("restart")]
        public async Task RestartBot()
        {
            if (Global.IsStaff((SocketGuildUser) this.Context.User))
            {
                await this.Context.Message.DeleteAsync();
                RestUserMessage restartMsg = await this.Context.Channel.SendMessageAsync("Redémarrage en cours...");
                await Task.Delay(Global.Client.Latency*10*2);
                await restartMsg.ModifyAsync(msg => msg.Content = "Redémarrage effectué avec succès!");
                this.Context.Client.Dispose();
                await Global.Client.LogoutAsync();
                await Global.Client.StopAsync();
                Program.Main();
            }
            else
            {
                await ReplyAsync("Vous devez être membre du staff pour utiliser cette commande");
                Logs.WriteLine($"{this.Context.User.Username} a tenté d'exécuter la commande {this.Context.Message.Content} et n'est pas membre du staff");
            }
        }

        [Command("hello"), Alias("helloworld", "world"), Summary("Commande hello world")]
        public async Task SendMessage()
        {
            await this.Context.Channel.SendMessageAsync($"Wsh {this.Context.User.Mention}, bien ou bien ma couille");
        }

        [Command("testembed"), Alias("embed", "te", "emb")]
        public async Task SendEmbed([Remainder] string input = "None")
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithAuthor("Test embed", this.Context.User.GetAvatarUrl());
            embed.WithColor(250, 125, 125);
            embed.WithFooter("Le proprio génial du discord", this.Context.Guild.Owner.GetAvatarUrl());
            embed.WithDescription("This is a **PEEEEERFECT** random desc with an `Awesome` link.\n" +
                                  "[Le meilleur des wiki :heart::heart:](https://alternia.fandom.com/fr/)");

            await this.Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("plop")]
        public async Task SendOctoplop()
        {
            // ReSharper disable once InconsistentNaming
            string imgsPath = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1\AlterBotNet.dll", @"Data\Plop\");
            string[] vectImgs = new string[]
            {
                $"{imgsPath}p0.jpg",
                $"{imgsPath}p1.jpg",
                $"{imgsPath}p2.jpg",
                $"{imgsPath}p3.jpg",
                $"{imgsPath}p4.jpg",
                $"{imgsPath}p5.jpg",
                $"{imgsPath}p6.jpg",
                $"{imgsPath}p7.jpg",
            };
            string[] vectCapt = new String[]
            {
                "POUUUUUUULPE!",
                "Quelqu'un a parlé de Takoyakis?",
                "FAAAAABULEUX!",
                "Un beau cadeau pour...naaaan oublie",
                "C mi ke jsui le + bô",
                "La vie est beeeeeelle!",
                "Plop mon poulpe",
                "Alors déjà on dit pas poulpe, c'est supeeeer péjoratif!",
                "Alors déjà on dit pas poulpe, c'est supeeeer péjoratif!",
                "Alors déjà on dit pas poulpe, c'est supeeeer péjoratif!",
                "Alors déjà on dit pas poulpe, c'est supeeeer péjoratif!"
            };
            try
            {
                await this.Context.Channel.SendFileAsync(vectImgs[this._rand.Next(vectImgs.Length)], $"{vectCapt[this._rand.Next(vectCapt.Length)]}");
            }
            catch (Exception e)
            {
                Logs.WriteLine(e.ToString());
                throw;
            }
        }

        [Command("say")]
        public async Task SendSayMessage([Remainder] string input = "")
        {
            await this.Context.Message.DeleteAsync();
            await this.Context.Channel.SendMessageAsync(input);
        }

        [Command("help"), Summary("Envoie la liste des commandes disponibles en mp")]
        public async Task SendHelp()
        {
            string rp = "";
            string autre = "";
            rp += "\n";
            rp += "Liste des commandes: `help`\n";
            rp += "Aide sur la commande bank: `bank help`\n";
            rp += "Aide sur la commande stuff: `stuff help`\n";
            rp += "Aide sur la commande stats: `stats help`\n";
            autre += "Envoyer une image de poulpe avec un message aléatoire: `plop`\n";
            rp += "Lancer un dé: `roll 1d100`\n";
            autre += "Faire parler le bot (c useless): `say message`\n";
            autre += "Saluer l'utilisateur qui a envoyé la commande: `hello`\n";
            autre += "Tester le message de bienvenue sur le serveur: `testjoin`\n";
            try
            {
                await ReplyAsync("Infos envoyées en mp");
                Logs.WriteLine($"message envoyé en mp à {this.Context.User.Username}");
                EmbedBuilder eb = new EmbedBuilder();
                eb.WithTitle("**Liste des commandes disponibles**")
                    .WithColor(this._rand.Next(256), this._rand.Next(256), this._rand.Next(256))
                    .AddField("=========== Commandes RP ===========", rp)
                    .AddField("========= Autres Commandes =========", autre);
                //await this.Context.User.SendMessageAsync(infoAccount.ToString());
                await this.Context.User.SendMessageAsync("", false, eb.Build());
                Logs.WriteLine(rp);
            }
            catch (Exception e)
            {
                Logs.WriteLine(e.ToString());
                throw;
            }
        }

        [Command("welcome"), Alias("wc"), Summary("Envoie un message de bienvenue à l'utilisateur mentionné")]
        public async Task SendWelcome([Remainder] string input = "none")
        {
            await this.Context.Message.DeleteAsync();
            SocketUser mentionedUser = this.Context.Message.MentionedUsers.FirstOrDefault() ?? this.Context.User;
            string guildName = this.Context.Guild.Name;
            if (guildName == "ServeurTest")
            {
                //await this.Context.Channel.SendMessageAsync("Bienvenue sur Alternia " + this.Context.User.Mention + "! Toutes les infos pour faire ta fiche sont ici :\n<#" + GetChannelByName("contexte-rp", guildName) + ">\n<#" + GetChannelByName("geographie-de-alternia", guildName) + ">\n" + GetChannelByName("banque", guildName) + "\n" + GetChannelByName("regles", guildName) + "\n" + GetChannelByName("liens-utiles", guildName) + "\n" + GetChannelByName("fiche-prototype", guildName) + "\n" + GetChannelByName("les-races-disponibles", guildName) +
                //                                            "\nSi tu as besoins d'aide n'hésite pas à demander à un membre du " + this.Context.Guild.GetRole(541492279894999080).Mention + "!", false, null, null);
                await this.Context.Channel.SendMessageAsync("Bienvenue sur Alternia " + mentionedUser.Mention + "! Toutes les infos pour faire ta fiche sont ici :\n<#542072451324968972>\n<#542070741504360458>\n<#541493264180707338>\n<#542070805236940837>\n<#542072285033660437>\n<#542073013722546218>\n<#542073051790049302>" +
                                                            "\nSi tu as besoins d'aide n'hésite pas à demander à un membre du " + this.Context.Guild.GetRole(541492279894999080).Mention + " !");
            }
            else
            {
                await this.Context.Channel.SendMessageAsync("Bienvenue sur Alternia " + mentionedUser.Mention + "! Toutes les infos pour faire ta fiche sont ici :\n<#410438433849212928>\n<#410531350102147072>\n<#411969883673329665>\n<#409789542825197568>\n<#409849626988904459>\n<#410424057050300427>\n<#410487492463165440>" +
                                                            "\nSi tu as besoins d'aide n'hésite pas à demander à un membre du " + this.Context.Guild.GetRole(420536907525652482).Mention + " !");
            }
        }

        [Command("ban"), Summary("Ban l'utilisateur mentionné")]
        [RequireBotPermission(GuildPermission.Administrator), RequireUserPermission(GuildPermission.Administrator)]
        public async Task BanUser([Remainder] string input = "none")
        {
            try
            {
                SocketUser mentionedUser = this.Context.Message.MentionedUsers.FirstOrDefault();
                if (input == "none" || mentionedUser == null)
                {
                    await ReplyAsync("Vous devez au moins mentionner un utilisateur");
                    return;
                }

                string toRemove = input.Split(' ')[0];
                string raison = input.Replace(toRemove, "");
                if (!mentionedUser.IsBot)
                    await mentionedUser.SendMessageAsync($"Vous avez été banni du serveur {this.Context.Guild.Name} pour la raison suivante: {raison}");
                await this.Context.Guild.GetUser(260385529474842626).SendMessageAsync($"{mentionedUser.Username} a été banni du serveur {this.Context.Guild.Name} pour la raison suivante: {raison}");
                Logs.WriteLine($"{mentionedUser} a été banni du serveur {this.Context.Guild.Name} pour la raison suivante: {raison}");
                await ((IGuildChannel) this.Context.Channel).Guild.AddBanAsync(mentionedUser, 7, raison);
            }
            catch (Exception e)
            {
                Logs.WriteLine(e.ToString());
                throw;
            }
        }

        [Command("deban"), Summary("Deban l'utilisateur mentionné")]
        [RequireBotPermission(GuildPermission.Administrator), RequireUserPermission(GuildPermission.Administrator)]
        public async Task UnbanUser(ulong userId)
        {
            try
            {
                SocketUser mentionedUser = this.Context.Client.GetUser(userId);
                if (!mentionedUser.IsBot)
                    await mentionedUser.SendMessageAsync($"Vous avez été de-banni du serveur {this.Context.Guild.Name}");
                    await this.Context.Guild.GetUser(260385529474842626).SendMessageAsync($"{mentionedUser.Username} a été de-banni du serveur {this.Context.Guild.Name}");
                    Logs.WriteLine($"{mentionedUser.Username} a été de-banni du serveur {this.Context.Guild.Name}");
                await this.Context.Guild.RemoveBanAsync(mentionedUser);
            }
            catch (Exception e)
            {
                Logs.WriteLine(e.ToString());
                throw;
            }
        }

        [Command("kick"), Summary("Kick l'utilisateur mentionné")]
        [RequireBotPermission(GuildPermission.Administrator), RequireUserPermission(GuildPermission.Administrator)]
        public async Task KickUser([Remainder] string input = "none")
        {
            try
            {
                SocketUser mentionedUser = this.Context.Message.MentionedUsers.FirstOrDefault();
                if (input == "none" || mentionedUser == null)
                {
                    await ReplyAsync("Vous devez au moins mentionner un utilisateur");
                    return;
                }

                string toRemove = input.Split(' ')[0];
                string raison = input.Replace(toRemove, "");
                if (!mentionedUser.IsBot)
                    await mentionedUser.SendMessageAsync($"Vous avez été éjecté (kick) du serveur {this.Context.Guild.Name} pour la raison suivante: {raison}");
                    await this.Context.Guild.GetUser(260385529474842626).SendMessageAsync($"{mentionedUser.Username} a été éjecté (kick) du serveur {this.Context.Guild.Name} pour la raison suivante: {raison}");
                    Logs.WriteLine($"{mentionedUser.Username} a été éjecté (kick) du serveur {this.Context.Guild.Name} pour la raison suivante: {raison}");
                await ((SocketGuildUser) mentionedUser).KickAsync(raison);
            }
            catch (Exception e)
            {
                Logs.WriteLine(e.ToString());
                throw;
            }
        }

        #endregion
    }
}