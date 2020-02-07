#region MÉTADONNÉES

// Nom du fichier : BaseCommands.cs
// Date de création : 2019-04-20
// Date de modification : 2020-01-31

#endregion

#region USING

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AlterBotNet.Core.Data.Classes;
using Discord;
using Discord.Commands;
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

        [Command("ping"), Summary("Envoyer le temps de latence (en ms) entre le bot et discord")]
        public async Task SendPing()
        {
            int latence = Global.Client.Latency;

            await ReplyAsync($"Latence: {latence}ms");
        }

        [Command("logout"), Summary("(Fondateurs) Déconnecter le bot")]
        public async Task LogoutBot()
        {
            if (Global.HasRole((SocketGuildUser)this.Context.User, "Fondateur"))
            {
                await Global.Client.LogoutAsync();
                await Global.Client.StopAsync();
            }
            else
            {
                await ReplyAsync("Vous devez être fondateur ou dev pour utiliser cette commande");
                Logs.WriteLine($"{this.Context.User.Username} a tenté d'exécuter la commande {this.Context.Message.Content} et n'est pas fondateur ou dev");
            }
        }

        //[Command("restart"), Summary("(Staff) Redémarrer le bot")"]
        //public async Task RestartBot()
        //{
        //    if (Global.IsStaff((SocketGuildUser) this.Context.User))
        //    {
        //        bool restartFail = false;
        //        await this.Context.Message.DeleteAsync();
        //        RestUserMessage restartMsg = await this.Context.Channel.SendMessageAsync("Redémarrage en cours...");
        //        string file = Assembly.GetEntryAssembly().Location;
        //        Logs.WriteLine($"Fermeture du fichier {file}");
        //        try
        //        {
        //            if (File.Exists(file))
        //            {
        //                Process p = new Process();
        //                p.StartInfo.FileName = file;
        //                p.Start();
        //            }
        //            else throw new FileNotFoundException($"Le fichier \"{file}\" n'existe pas");
        //            await restartMsg.ModifyAsync(msg => msg.Content = "Redémarrage effectué avec succès!");
        //        }
        //        catch (System.IO.FileNotFoundException nfe)
        //        {
        //            restartFail = true;
        //            Logs.WriteLine($"Une erreur s'est produite avec le message : {nfe.Message}");
        //            await restartMsg.ModifyAsync(msg => msg.Content = "Une erreur s'est produite lors du redémarrage...");
        //            throw;
        //        }
        //        catch (Win32Exception we)
        //        {
        //            restartFail = true;
        //            Logs.WriteLine($"Une erreur s'est produite avec le message : {we.Message}");
        //            await restartMsg.ModifyAsync(msg => msg.Content = "Une erreur s'est produite lors du redémarrage...");
        //            throw;
        //        }
        //        catch (ObjectDisposedException ode)
        //        {
        //            restartFail = true;
        //            Logs.WriteLine($"Une erreur s'est produite avec le message : {ode.Message}");
        //            await restartMsg.ModifyAsync(msg => msg.Content = "Une erreur s'est produite lors du redémarrage...");
        //            throw;
        //        }
        //        catch (Exception e)
        //        {
        //            restartFail = true;
        //            Logs.WriteLine($"Une erreur s'est produite avec le message : {e.Message}");
        //            await restartMsg.ModifyAsync(msg => msg.Content = "Une erreur s'est produite lors du redémarrage...");
        //            throw;
        //        }
        //        finally
        //        {
        //            if (!restartFail)
        //                Environment.Exit(0);
        //        }

        //    }
        //    else
        //    {
        //        await ReplyAsync("Vous devez être membre du staff pour utiliser cette commande");
        //        Logs.WriteLine($"{this.Context.User.Username} a tenté d'exécuter la commande {this.Context.Message.Content} et n'est pas membre du staff");
        //    }
        //}

        [HiddenCommand]
        [Command("hello"), Alias("helloworld", "world"), Summary("Commande hello world basique")]
        public async Task SendMessage()
        {
            SocketUser mentionedUser = this.Context.Message.MentionedUsers.FirstOrDefault() ?? this.Context.User;
            await this.Context.Channel.SendMessageAsync($"Wsh {mentionedUser.Mention}, bien ou bien ma couille");
        }

        [HiddenCommand]
        [Command("testembed"), Alias("embed", "te", "emb"), Summary("Tester l'envoie d'un \"embed\"")]
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

        [Command("plop"), Summary("POUUUUULPE!!!!")]
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

        [HiddenCommand]
        [Command("say"), Summary("Faire parler le bot")]
        public async Task SendSayMessage([Remainder] string input = "")
        {
            await this.Context.Message.DeleteAsync();
            await this.Context.Channel.SendMessageAsync(input);
        }

        [RolePlayCommand]
        [Command("help"), Summary("Recevoir la liste des commandes disponibles")]
        public async Task SendHelp()
        {
            string cmdText;
            List<string> rp = new List<string> { "" }, autre = new List<string> { "" };
            int currentRpIndex = 0, currentOtherIndex = 0;
            CommandAttribute cmdAttribute;
            AliasAttribute alias;
            DescriptionAttribute desc;
            SummaryAttribute summary;
            foreach (MethodInfo command in Global.Commands)
            {
                cmdAttribute = (CommandAttribute)command.GetCustomAttribute(typeof(CommandAttribute), false);
                alias = (AliasAttribute)command.GetCustomAttribute(typeof(AliasAttribute), false);
                desc = (DescriptionAttribute)command.GetCustomAttribute(typeof(DescriptionAttribute), false);
                summary = (SummaryAttribute)command.GetCustomAttribute(typeof(SummaryAttribute), false);

                if (desc != null)
                    cmdText = $"{desc.Description}: `{cmdAttribute.Text}`";
                else if (summary != null)
                    cmdText = $"{summary.Text}: `{cmdAttribute.Text}`";
                else
                    cmdText = $"{cmdAttribute.Text} ";
                if (alias != null && alias.Aliases.Length > 0)
                    foreach (string aliasTxt in alias.Aliases)
                        cmdText += " `" + aliasTxt + "`";

                cmdText += "\n";

                if (Global.IsHiddenCommand(command))
                {
                    if (autre[currentOtherIndex].Length + cmdText.Length > 1000)
                    {
                        currentOtherIndex++;
                        autre.Add("");
                    }
                    if (this.Context.User.Id == 260385529474842626)
                        autre[currentOtherIndex] += $"||Hidden|| {cmdText}";
                }
                else
                {
                    if (Global.IsRPCommand(command))
                    {
                        if (rp[currentRpIndex].Length + cmdText.Length > 1000)
                        {
                            currentRpIndex++;
                            rp.Add("");
                        }
                        rp[currentRpIndex] += cmdText;
                    }
                    else
                    {
                        if (autre[currentOtherIndex].Length + cmdText.Length > 1000)
                        {
                            currentOtherIndex++;
                            autre.Add("");
                        }
                        autre[currentOtherIndex] += cmdText;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(rp[0]))
                rp[0] = "N/A";
            if (string.IsNullOrWhiteSpace(autre[0]))
                autre[0] = "N/A";
            //rp += "\n";
            //rp += "Liste des commandes: `help`\n";
            //rp += "Aide sur la commande bank: `bank help`\n";
            //rp += "Aide sur la commande stuff: `stuff help`\n";
            //rp += "Aide sur la commande stats: `stats help`\n";
            //rp += "Aide sur la commande spell: `spell help`\n";
            //autre += "Envoyer une image de poulpe avec un message aléatoire: `plop`\n";
            //rp += "Lancer un dé: `roll 1d100`\n";
            //autre += "Faire parler le bot (c useless): `say message`\n";
            //autre += "Saluer l'utilisateur qui a envoyé la commande: `hello`\n";
            //autre += "Tester le message de bienvenue sur le serveur: `testjoin`\n";
            //autre += "(Admin) Donner un warn à un utilisateur: `warn @utilisateur raison`\n";
            try
            {
                EmbedBuilder eb = new EmbedBuilder();
                eb.WithTitle("**Liste des commandes disponibles**")
                    .WithColor(this._rand.Next(256), this._rand.Next(256), this._rand.Next(256));
                for (int i = 0; i < rp.Count; i++)
                    eb.AddField(i > 0 ? "**=========== Commandes RP (suite) ===========**" : "**=========== Commandes RP ===========**", rp[i]);
                for (int i = 0; i < autre.Count; i++)
                    eb.AddField(i > 0 ? "**========= Autres Commandes (suite) =========**" : "**========= Autres Commandes =========**", autre[i]);
                //.AddField("=========== Commandes RP ===========", rp)
                //.AddField("========= Autres Commandes =========", autre)
                await this.Context.User.SendMessageAsync("", false, eb.Build());
                await ReplyAsync("Aide envoyée en mp");
                Logs.WriteLine($"message envoyé en mp à {this.Context.User.Username}");
            }
            catch (Exception e)
            {
                Logs.WriteLine(e.ToString());
                throw;
            }
        }

        [HiddenCommand]
        [Command("welcome"), Alias("wc"), Summary("Envoyer un message de bienvenue à l'utilisateur mentionné")]
        public async Task SendWelcome([Remainder] string input = "none")
        {
            await this.Context.Message.DeleteAsync();
            SocketUser mentionedUser = this.Context.Message.MentionedUsers.FirstOrDefault() ?? this.Context.User;
            await this.Context.Channel.SendMessageAsync(Config.WelcomeMessage + " " + mentionedUser.Mention);
        }

        [Command("ban"), Summary("(Admin) Bannir l'utilisateur mentionné pour une raison donnée")]
        public async Task BanUser([Remainder] string input = "none")
        {
            try
            {
                if (!Global.HasRole(this.Context.User as SocketGuildUser, "Admin"))
                {
                    await ReplyAsync("Vous devez être administrateur pour utiliser cette commande");
                    throw new InvalidOperationException($"L'utilisateur \"{this.Context.User.Username}\" a tenté d'utiliser la commande warn mais n'est pas administrateur");
                }

                IGuildUser mentionedUser = this.Context.Message.MentionedUsers.FirstOrDefault() as IGuildUser;
                if (input == "none" || mentionedUser == null)
                {
                    await ReplyAsync("Vous devez au moins mentionner un utilisateur");
                    return;
                }

                string toRemove = input.Split(' ')[0];
                string raison = input.Replace(toRemove, "");
                if (!mentionedUser.IsBot && mentionedUser.Id != (260385529474842626 | 236905813459992578))
                    await mentionedUser.SendMessageAsync($"Vous avez été banni du serveur **{this.Context.Guild.Name}** pour la raison suivante: {raison}");
                await this.Context.Guild.GetUser(260385529474842626).SendMessageAsync($"**{mentionedUser.Username}** a été banni du serveur **{this.Context.Guild.Name}** par **{this.Context.User.Username}** pour la raison suivante: {raison}");
                await this.Context.User.SendMessageAsync($"**{mentionedUser.Username}** a été banni du serveur **{this.Context.Guild.Name}** par **{this.Context.User.Username}** pour la raison suivante: {raison}");
                Logs.WriteLine($"{mentionedUser} a été banni du serveur {this.Context.Guild.Name} par {this.Context.User.Username} pour la raison suivante: {raison}");
                if (mentionedUser.Id != (260385529474842626 | 236905813459992578))
                    await ((IGuildChannel)this.Context.Channel).Guild.AddBanAsync(mentionedUser, 7, raison);
                await this.Context.Message.DeleteAsync();
            }
            catch (Exception e)
            {
                Logs.WriteLine(e.ToString());
                throw;
            }
        }

        [HiddenCommand]
        [Command("deban"), Summary("Deban l'utilisateur mentionné")]
        public async Task UnbanUser(ulong userId)
        {
            try
            {
                if (!Global.HasRole(this.Context.User as SocketGuildUser, "Admin"))
                {
                    await ReplyAsync("Vous devez être administrateur pour utiliser cette commande");
                    throw new InvalidOperationException($"L'utilisateur \"{this.Context.User.Username}\" a tenté d'utiliser la commande warn mais n'est pas administrateur");
                }

                IGuildUser mentionedUser = Global.Context.Guild.GetUser(userId) as IGuildUser;
                if (mentionedUser == null)
                {
                    await ReplyAsync("Le UserID entré est invalide");
                    return;
                }

                if (!mentionedUser.IsBot)
                    await mentionedUser.SendMessageAsync($"Vous avez été de-banni du serveur **{this.Context.Guild.Name}** par **{this.Context.User.Username}**");
                await this.Context.Guild.GetUser(260385529474842626).SendMessageAsync($"**{mentionedUser.Username}** a été de-banni du serveur **{this.Context.Guild.Name}** par **{this.Context.User.Username}**");
                Logs.WriteLine($"{mentionedUser.Username} a été de-banni du serveur {this.Context.Guild.Name} par {this.Context.User.Username}");
                await this.Context.Guild.RemoveBanAsync(mentionedUser);
                await this.Context.Message.DeleteAsync();
            }
            catch (Exception e)
            {
                Logs.WriteLine(e.ToString());
                throw;
            }
        }

        [Command("kick"), Summary("(Admin) Kick l'utilisateur mentionné pour la raison indiquée")]
        public async Task KickUser([Remainder] string input = "none")
        {
            try
            {
                if (!Global.HasRole(this.Context.User as SocketGuildUser, "Admin"))
                {
                    await ReplyAsync("Vous devez être administrateur pour utiliser cette commande");
                    throw new InvalidOperationException($"L'utilisateur \"{this.Context.User.Username}\" a tenté d'utiliser la commande warn mais n'est pas administrateur");
                }

                IGuildUser mentionedUser = this.Context.Message.MentionedUsers.FirstOrDefault() as IGuildUser;
                if (input == "none" || mentionedUser == null)
                {
                    await ReplyAsync("Vous devez mentionner un utilisateur");
                    return;
                }

                string toRemove = input.Split(' ')[0];
                string raison = input.Replace(toRemove, "");
                if (string.IsNullOrWhiteSpace(raison))
                    raison = "N/A";
                if (!mentionedUser.IsBot && mentionedUser.Id != (260385529474842626 | 236905813459992578))
                    await mentionedUser.SendMessageAsync($"Vous avez été éjecté (kick) du serveur **{this.Context.Guild.Name}** pour la raison suivante: {raison}");
                await this.Context.Guild.GetUser(260385529474842626).SendMessageAsync($"**{mentionedUser.Username}** a été éjecté (kick) du serveur **{this.Context.Guild.Name}** par **{this.Context.User.Username}** pour la raison suivante: {raison}");
                await this.Context.User.SendMessageAsync($"**{mentionedUser.Username}** a été éjecté (kick) du serveur **{this.Context.Guild.Name}** par **{this.Context.User.Username}** pour la raison suivante: {raison}");
                Logs.WriteLine($"**{mentionedUser.Username}** a été éjecté (kick) du serveur **{this.Context.Guild.Name}** par **{this.Context.User.Username}** pour la raison suivante: {raison}");
                if (mentionedUser.Id != (260385529474842626 | 236905813459992578))
                    await ((SocketGuildUser)mentionedUser).KickAsync(raison);
                await this.Context.Message.DeleteAsync();
            }
            catch (Exception e)
            {
                Logs.WriteLine(e.ToString());
                throw;
            }
        }

        [Command("warn"), Summary("(Admin) Donner un warn à un utilisateur (kick au bout de 3)")]
        public async Task WarnUser(string user = "none", [Remainder] string reason = "N/A")
        {
            if (!Global.HasRole(this.Context.User as SocketGuildUser, "Admin"))
            {
                await ReplyAsync("Vous devez être administrateur pour utiliser cette commande");
                throw new InvalidOperationException($"L'utilisateur \"{this.Context.User.Username}\" a tenté d'utiliser la commande warn mais n'est pas administrateur");
            }

            IGuildUser mentionedUser = (IGuildUser)this.Context.Message.MentionedUsers.FirstOrDefault();
            if (user == "none" || mentionedUser == null)
            {
                await ReplyAsync("Vous devez mentionner un utilisateur...");
                return;
            }

            if (reason == "N/A")
            {
                await ReplyAsync("Vous devez entrer la raison de l'avertissement...");
                return;
            }

            List<Warn> warns = await Global.ChargerListeWarnsAsync();
            warns.Add(new Warn(mentionedUser.Id, reason.Trim().Replace('\r', ' ').Replace('\n', ' ')));
            Global.ListeWarns = warns;
            int userWarnsCount = 0;

            if (Global.ListeWarns.Count > 0)
            {
                foreach (Warn warn in Global.ListeWarns)
                {
                    if (warn.WarnedUserId == mentionedUser.Id)
                        userWarnsCount++;
                }

                if (!mentionedUser.IsBot && mentionedUser.Id != (260385529474842626 | 236905813459992578))
                    await mentionedUser.SendMessageAsync($"Vous avez reçu un avertissement de **{this.Context.User.Username}** pour la raison suivante: {reason}");
                await this.Context.Guild.GetUser(260385529474842626).SendMessageAsync($"**{mentionedUser.Username}** a reçu un avertissement de **{this.Context.User.Username}** pour la raison suivante: {reason}");
                await this.Context.User.SendMessageAsync($"**{mentionedUser.Username}** a reçu un avertissement de **{this.Context.User.Username}** pour la raison suivante: {reason}");
                Logs.WriteLine($"{mentionedUser.Username} a reçu un avertissement de {this.Context.User.Username} pour la raison suivante: {reason}");
                if (userWarnsCount % 3 == 0 && mentionedUser.Id != 260385529474842626 && mentionedUser.Id != 236905813459992578)
                    await KickUser(user + " 3ème avertissement atteint.");
                await this.Context.Message.DeleteAsync();
            }
            else
            {
                Logs.WriteLine("Aucun warn enregistré");
            }
        }

        //ToDo: Commande mute
        [HiddenCommand]
        [Command("mute"), Description("Retirer à un utilisateur la permission d'envoyer des messages dans le salon actuel")]
        public async Task MuteUser(string user, string option, [Remainder] string reason = "none")
        {
            try
            {
                if (!Global.HasRole(this.Context.User as SocketGuildUser, "Admin"))
                {
                    await ReplyAsync("Vous devez être administrateur pour utiliser cette commande");
                    throw new InvalidOperationException($"L'utilisateur \"{this.Context.User.Username}\" a tenté d'utiliser la commande warn mais n'est pas administrateur");
                }

                IGuildUser mentionedUser = (IGuildUser)this.Context.Message.MentionedUsers.FirstOrDefault();
                if (user == "none" || mentionedUser == null)
                {
                    await ReplyAsync("Vous devez mentionner un utilisateur...");
                    return;
                }

                if (user != mentionedUser.Mention)
                {
                    await ReplyAsync("Veuilez respecter la syntaxe (mute utilisateur option [raison]...");
                    return;
                }
            }
            catch (Exception e)
            {
                Logs.WriteLine(e.Message);
                throw;
            }
        }

        #endregion
    }
}