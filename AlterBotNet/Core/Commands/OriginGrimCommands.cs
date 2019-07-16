#region MÉTADONNÉES

// Nom du fichier : OriginGrimCommands.cs
// Auteur : Loick OBIANG (1832960)
// Date de création : 2019-04-20
// Date de modification : 2019-05-28

#endregion

#region USING

using System;
using System.Threading.Tasks;
using AlterBotNet.Core.Data.Classes;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

#endregion

namespace AlterBotNet.Core.Commands
{
    public class OriginGrimCommands : ModuleBase<SocketCommandContext>
    {
        #region ATTRIBUTS

        private Random _rand = new Random();

        #endregion

        #region MÉTHODES

        [Command("grim"), Alias("grm"), Summary("Affiche le contenu du grimoire commun")]
        public async Task SendPublicGrim([Remainder] string input = "none")
        {
            string message = "";
            string nomFichierXml = Global.CheminGrimoirePublic;

            Logs.WriteLine("Commande grim utilisée par " + this.Context.User.Username);
            if (input != "none")
            {
                // ====================================
                // = Gestion de la commande grim help =
                // ====================================
                if (input == "help")
                {
                    string staff = "";
                    message += "Aide sur la commande: `grim help`\n";
                    staff += "(staff) Afficher la liste des comptes: `grim list`\n";
                    staff += "(staff) Afficher la liste des comptes: `grim update`\n";
                    message += "(staff) Trier la liste des comptes (par ordre alphabétique): `grim sort`\n";
                    try
                    {
                        await ReplyAsync("Aide envoyée en mp");
                        Logs.WriteLine($"message envoyé en mp à {this.Context.User.Username}");
                        EmbedBuilder eb = new EmbedBuilder();
                        eb.WithTitle("**Aide de la commande grim (grm)**")
                            .WithColor(this._rand.Next(256), this._rand.Next(256), this._rand.Next(256))
                            .AddField("========== Staff ==========", staff)
                            .AddField("========== Autre ==========", message);
                        //await this.Context.User.SendMessageAsync(infoAccount.ToString());
                        await this.Context.User.SendMessageAsync("", false, eb.Build());
                        Logs.WriteLine(message);
                    }
                    catch (Exception e)
                    {
                        Logs.WriteLine(e.ToString());
                        throw;
                    }
                }
                // =============================================
                // = Gestion de la commande (staff) grim list =
                // =============================================
                else if (input == "list")
                {
                    await this.Context.Message.DeleteAsync();
                    await ReplyAsync("**Grimoire Commun:**\n");
                    if (Global.IsStaff((SocketGuildUser) this.Context.User))
                    {
                        if (string.IsNullOrEmpty((await Global.XmlGrimSpellsListAsync(nomFichierXml)).ToString()))
                        {
                            await ReplyAsync("Liste vide");
                            Logs.WriteLine("Liste vide");
                        }
                        else
                        {
                            try
                            {
                                foreach (string msg in await Global.XmlGrimSpellsListAsync(nomFichierXml))
                                {
                                    if (!string.IsNullOrEmpty(msg))
                                    {
                                        await ReplyAsync(msg);
                                    }
                                }

                                Logs.WriteLine($"Liste envoyée sur le channel {this.Context.Channel.Name}");
                            }
                            catch (Exception e)
                            {
                                Logs.WriteLine(e.ToString());
                                throw;
                            }
                        }
                    }
                    else
                    {
                        if (this.Context.Guild.Name == "ServeurTest")
                            await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(541492279894999080).Mention} pour utiliser cette commande");
                        else
                            await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(420536907525652482).Mention} pour utiliser cette commande");
                    }
                }
            }
        }

        #endregion
    }
}