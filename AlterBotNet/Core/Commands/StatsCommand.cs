#region MÉTADONNÉES

// Nom du fichier : StatsCommand.cs
// Auteur : Loick OBIANG (1832960)
// Date de création : 2019-03-14
// Date de modification : 2019-03-15

#endregion

#region USING

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlterBotNet.Core.Data.Classes;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

#endregion

namespace AlterBotNet.Core.Commands
{
    /// <summary>
    /// Classe des commandes stats
    /// </summary>
    public class StatsCommand : ModuleBase<SocketCommandContext>
    {
        #region ATTRIBUTS

        private Random _rand = new Random();

        #endregion

        #region MÉTHODES

        [Command("stats"), Alias("sts"), Summary("Affiche les stats d'un personnage")]
        public async Task SendStats([Remainder] string input = "none")
        {
            SocketUser mentionedUser = this.Context.Message.MentionedUsers.FirstOrDefault();
            string[] argus;
            ulong userId = this.Context.User.Id;
            string error = "Valeur invalide, stats help pour plus d'information.";
            string message = "";
            string nomFichier = Global.CheminComptesStats;

            List<StatsAccount> statsAccounts = await Global.ChargerDonneesStatsAsync(nomFichier);

            if (input != "none")
            {
                // ====================================
                // = Gestion de la commande stats help =
                // ====================================
                if (input == "help")
                {
                    string staff = "";
                    message += "Aide sur la commande: `stats help`\n";
                    staff += "(staff) Afficher la liste des comptes: `stats list`\n";
                    message += "Afficher les objets d'un personnage: `stats info (nom_Personnage)`\n";
                    staff += "(staff) Ajouter un nombre de points dans une stat à un personnage: `stats add (stat) (nom_Personnage) (quantité)`\n";
                    staff += "(staff) Retirer un nombre de points dans une stat à un personnage: `stats remove (stat) (nom_Personnage) (quantité)`\n";
                    staff += "(staff) Définir le nombre de points dans une stat d'un personnage: `stats set (stat) (nom_Personnage) (valeur)`\n";
                    staff += "(staff) (RP) Créer un nouveau compte: `stats create (nomPersonnage)`\n";
                    staff += "(staff) Supprimer un compte: `stats delete (nomPersonnage)`\n";
                    message += "Trier la liste des comptes (par ordre alphabétique): `stats sort`\n";
                    staff += "(staff) Définir le propriétaire d'un personnage: `stats setowner (nom_personnage) (@propriétaire)`\n";
                    staff += "(staff) Changer le nom d'un personnage: `stats rename (nom_personnage) (nouveauNom)`\n";
                    try
                    {
                        await ReplyAsync("Aide envoyée en mp");
                        Logs.WriteLine($"message envoyé en mp à {this.Context.User.Username}");
                        EmbedBuilder eb = new EmbedBuilder();
                        eb.WithTitle("**Aide de la commande stats**")
                            .WithColor(this._rand.Next(256), this._rand.Next(256), this._rand.Next(256))
                            .WithDescription("Stat => Force:str, Agilité:agi, Technique:tec, Magie:mag, Résistance:res, Intelligence:int, Esprit:esp")
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
                // = Gestion de la commande (staff) stats list =
                // =============================================
                else if (input == "list")
                {
                    if (Global.IsStaff((SocketGuildUser) this.Context.User))
                    {
                        if (string.IsNullOrEmpty((await Global.StatsAccountsListAsync(nomFichier)).ToString()))
                        {
                            await ReplyAsync("Liste vide");
                            Logs.WriteLine("Liste vide");
                        }
                        else
                        {
                            try
                            {
                                Logs.WriteLine((await Global.StatsAccountsListAsync(nomFichier)).Count.ToString());
                                foreach (string msg in await Global.StatsAccountsListAsync(nomFichier))
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
                        throw new Exception();
                        Logs.WriteLine($"\"{this.Context.User.Username}\" a tenté d'utiliser la commande stats list sans être membre du staff.");
                            await ReplyAsync($"Vous devez être membre du {Global.GetRoleByName(this.Context,"Staff").Mention} pour utiliser cette commande");
                    }
                }
                // ======================================
                // = Gestion de la commande stats update =
                // ======================================
                else if (input.StartsWith("update") || input.StartsWith("up"))
                {
                    argus = input.Split(' ');
                    // Sert à s'assurer qu'argus[0] == toujours update
                    if (argus[0] == "update" || argus[0] == "up")
                    {
                        await Global.UpdateStats();
                        Logs.WriteLine("Actualisation réussie");
                    }
                }
                // ======================================================
                // = Gestion de la commande stats info (nom_Personnage) =
                // ======================================================
                else if (input.StartsWith("info"))
                {
                    argus = input.Split(' ');
                    // Sert à s'assurer qu'argus[0] == toujours info
                    if (argus[0] == "info")
                    {
                        if (argus.Length > 2) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 2 paramètres)
                        {
                            await ReplyAsync($"{error} Nombre max d'arguments dépassé");
                            Logs.WriteLine($"{error} Nombre max d'arguments dépassé");
                        }
                        else if (argus.Length < 2) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 2 paramètres)
                        {
                            await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                            Logs.WriteLine($"{error} Nombre insuffisant d'arguments");
                        }
                        else
                        {
                            StatsAccount infoAccount = await Global.GetStatsAccountByNameAsync(nomFichier, argus[1]);
                            if (infoAccount != null && (infoAccount.UserId == userId || Global.IsStaff((SocketGuildUser) this.Context.User)))
                            {
                                try
                                {
                                    await ReplyAsync("Infos envoyées en mp");
                                    Logs.WriteLine($"Statistiques de \"{infoAccount.Name}\" envoyées en mp à \"{this.Context.User.Username}\"");
                                    EmbedBuilder eb = new EmbedBuilder();
                                    eb.WithTitle($"Statistiques de **{infoAccount.Name}**")
                                        .WithColor(this._rand.Next(256), this._rand.Next(256), this._rand.Next(256))
                                        .AddField("==============================================", infoAccount.ToString() + $"\nPropriétaire: {this.Context.Guild.GetUser(infoAccount.UserId).Username}");
                                    await this.Context.User.SendMessageAsync("", false, eb.Build());
                                }
                                catch (Exception e)
                                {
                                    Logs.WriteLine($"Une erreure est survenu lors de l'utilisation de la commande \"stats info\" avec le message suivant : {e.Message}");
                                }
                            }
                            else
                            {
                                if (infoAccount == null)
                                {
                                    await ReplyAsync($"{error} Compte \"**{argus[1]}**\" inexistant: stats create (nom_Personnage) pour créer un nouveau compte");
                                    Logs.WriteLine($"{error} Compte \"**{argus[1]}**\" inexistant: stats create (nom_Personnage) pour créer un nouveau compte");
                                }
                                else if (infoAccount.UserId != userId)
                                {
                                    await ReplyAsync($"{error} Vous devez être propriétaire du personnage ou membre du Staff pour utiliser cette commande");
                                    Logs.WriteLine($"{error} Vous devez être propriétaire du personnage ou membre du Staff pour utiliser cette commande");
                                }
                            }
                        }
                    }
                }
                // =============================================================================
                // = Gestion de la commande (admin) stats add (stat) (nom_Personnage) (valeur) =
                // =============================================================================
                else if (input.StartsWith("add"))
                {
                    if (Global.HasRole((SocketGuildUser) this.Context.User, "Admin"))
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours add
                        if (argus[0] == "add")
                        {
                            if (argus.Length > 4) // Sert à s'assurer qu'il y a - de 5 paramètres
                            {
                                await ReplyAsync($"{error} Nombre max d'arguments dépassé");
                                Logs.WriteLine($"{error} Nombre max d'arguments dépassé");
                            }
                            else if (argus.Length < 4) // Sert à s'assurer qu'il y a + de 3 paramètres
                            {
                                await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                                Logs.WriteLine($"{error} Nombre insuffisant d'arguments");
                            }
                            else
                            {
                                StatsAccount depositAccount = await Global.GetStatsAccountByNameAsync(nomFichier, argus[2]);
                                if (depositAccount != null)
                                {
                                    string dpName = depositAccount.Name;
                                    uint dpForce = depositAccount.Force;
                                    uint dpAgilité = depositAccount.Agilite;
                                    uint dpTechnique = depositAccount.Technique;
                                    uint dpMagie = depositAccount.Magie;
                                    uint dpResistance = depositAccount.Resistance;
                                    uint dpIntelligence = depositAccount.Intelligence;
                                    uint dpSociabilite = depositAccount.Sociabilite;
                                    uint dpEsprit = depositAccount.Esprit;
                                    ulong dpUserId = depositAccount.UserId;
                                    string stat = argus[1];
                                    if (argus[2].Contains('_'))
                                        argus[2] = argus[2].Replace("_", " ");
                                    if (argus[2].Contains('-'))
                                        argus[2] = argus[2].Replace("-", " ");
                                    if (uint.TryParse(argus[3], out uint valeurAjout))
                                    {
                                        try
                                        {
                                            switch (stat.ToLower())
                                            {
                                                case "str":
                                                case "force":
                                                    dpForce += valeurAjout;
                                                    stat = "force";
                                                    break;
                                                case "agi":
                                                case "agilite":
                                                case "agilité":
                                                    dpAgilité += valeurAjout;
                                                    stat = "agilité";
                                                    break;
                                                case "tec":
                                                case "technique":
                                                    dpTechnique += valeurAjout;
                                                    stat = "technique";
                                                    break;
                                                case "mag":
                                                case "magie":
                                                    dpMagie += valeurAjout;
                                                    stat = "magie";
                                                    break;
                                                case "res":
                                                case "resi":
                                                case "resistance":
                                                    dpResistance += valeurAjout;
                                                    stat = "resistance";
                                                    break;
                                                case "int":
                                                case "intel":
                                                case "intelligence":
                                                    dpIntelligence += valeurAjout;
                                                    stat = "intelligence";
                                                    break;
                                                case "soc":
                                                case "socio":
                                                case "social":
                                                case "sociabilité":
                                                case "sociabilite":
                                                    dpSociabilite += valeurAjout;
                                                    stat = "sociabilité";
                                                    break;
                                                case "esp":
                                                case "esprit":
                                                    dpEsprit += valeurAjout;
                                                    stat = "esprit";
                                                    break;
                                            }

                                            statsAccounts.RemoveAt(await Global.GetStatsAccountIndexByNameAsync(nomFichier, argus[2]));
                                            Global.EnregistrerDonneesStats(nomFichier, statsAccounts);
                                            StatsAccount newAccount = new StatsAccount(dpName, dpForce, dpAgilité, dpTechnique, dpMagie, dpResistance, dpIntelligence, dpSociabilite, dpEsprit, dpUserId);
                                            statsAccounts.Add(newAccount);
                                            Global.EnregistrerDonneesStats(nomFichier, statsAccounts);
                                            await ReplyAsync($"**{valeurAjout}** points en **{stat}** ajoutés à \"**{dpName}**\"");
                                            Logs.WriteLine($"**{valeurAjout}** points en **{stat}** ajoutés à \"**{dpName}**\"");
                                            Logs.WriteLine(newAccount.ToString());
                                        }
                                        catch (Exception e)
                                        {
                                            Logs.WriteLine(e.ToString());
                                            throw;
                                        }
                                    }
                                    else
                                    {
                                        await ReplyAsync($"{error} la valeur entrée est invalide");
                                        Logs.WriteLine($"{error} la valeur entrée est invalide");
                                    }
                                }
                                else
                                {
                                    await ReplyAsync($"{error} Compte \"**{argus[2]}**\" inexistant: stats create (nom_Personnage) pour créer un nouveau compte");
                                    Logs.WriteLine($"{error} Compte \"**{argus[2]}**\" inexistant: stats create (nom_Personnage) pour créer un nouveau compte");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (this.Context.Guild.Name == "ServeurTest")
                            await this.Context.Channel.SendMessageAsync($"Vous devez être admin pour utiliser cette commande");
                        else
                            await this.Context.Channel.SendMessageAsync($"Vous devez être admin pour utiliser cette commande");
                    }
                }
                // ================================================================================
                // = Gestion de la commande (admin) stats remove (stat) (nom_Personnage) (valeur) =
                // ================================================================================
                else if (input.StartsWith("remove") || input.StartsWith("rem"))
                {
                    if (Global.HasRole((SocketGuildUser) this.Context.User, "Admin"))
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours remove
                        if (argus[0] == "remove" || argus[0] == "rem")
                        {
                            if (argus.Length > 4) // Sert à s'assurer qu'il y a - de 5 paramètres
                            {
                                await ReplyAsync($"{error} Nombre max d'arguments dépassé");
                                Logs.WriteLine($"{error} Nombre max d'arguments dépassé");
                            }
                            else if (argus.Length < 4) // Sert à s'assurer qu'il y a + de 3 paramètres
                            {
                                await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                                Logs.WriteLine($"{error} Nombre insuffisant d'arguments");
                            }
                            else
                            {
                                StatsAccount withdrawAccount = await Global.GetStatsAccountByNameAsync(nomFichier, argus[2]);
                                if (withdrawAccount != null)
                                {
                                    string wdName = withdrawAccount.Name;
                                    uint wdForce = withdrawAccount.Force;
                                    uint wdAgilité = withdrawAccount.Agilite;
                                    uint wdTechnique = withdrawAccount.Technique;
                                    uint wdMagie = withdrawAccount.Magie;
                                    uint wdResistance = withdrawAccount.Resistance;
                                    uint wdIntelligence = withdrawAccount.Intelligence;
                                    uint wdSociabilite = withdrawAccount.Sociabilite;
                                    uint wdEsprit = withdrawAccount.Esprit;
                                    ulong wdUserId = withdrawAccount.UserId;
                                    string stat = argus[1];
                                    if (argus[2].Contains('_'))
                                        argus[2] = argus[2].Replace("_", " ");
                                    if (argus[2].Contains('-'))
                                        argus[2] = argus[2].Replace("-", " ");
                                    if (uint.TryParse(argus[3], out uint valeurAjout))
                                    {
                                        try
                                        {
                                            switch (stat.ToLower())
                                            {
                                                case "str":
                                                case "force":
                                                    wdForce -= valeurAjout;
                                                    stat = "force";
                                                    break;
                                                case "agi":
                                                case "agilite":
                                                case "agilité":
                                                    wdAgilité -= valeurAjout;
                                                    stat = "agilité";
                                                    break;
                                                case "tec":
                                                case "technique":
                                                    wdTechnique -= valeurAjout;
                                                    stat = "technique";
                                                    break;
                                                case "mag":
                                                case "magie":
                                                    wdMagie -= valeurAjout;
                                                    stat = "magie";
                                                    break;
                                                case "res":
                                                case "resi":
                                                case "resistance":
                                                    wdResistance -= valeurAjout;
                                                    stat = "resistance";
                                                    break;
                                                case "int":
                                                case "intel":
                                                case "intelligence":
                                                    wdIntelligence -= valeurAjout;
                                                    stat = "intelligence";
                                                    break;
                                                case "soc":
                                                case "socio":
                                                case "social":
                                                case "sociabilité":
                                                case "sociabilite":
                                                    wdSociabilite -= valeurAjout;
                                                    stat = "sociabilité";
                                                    break;
                                                case "esp":
                                                case "esprit":
                                                    wdEsprit -= valeurAjout;
                                                    stat = "esprit";
                                                    break;
                                            }

                                            statsAccounts.RemoveAt(await Global.GetStatsAccountIndexByNameAsync(nomFichier, argus[2]));
                                            Global.EnregistrerDonneesStats(nomFichier, statsAccounts);
                                            StatsAccount newAccount = new StatsAccount(wdName, wdForce, wdAgilité, wdTechnique, wdMagie, wdResistance, wdIntelligence, wdSociabilite, wdEsprit, wdUserId);
                                            statsAccounts.Add(newAccount);
                                            Global.EnregistrerDonneesStats(nomFichier, statsAccounts);
                                            await ReplyAsync($"**{valeurAjout}** points en **{stat}** ajoutés à \"**{wdName}**\"");
                                            Logs.WriteLine($"**{valeurAjout}** points en **{stat}** ajoutés à \"**{wdName}**\"");
                                            Logs.WriteLine(newAccount.ToString());
                                        }
                                        catch (Exception e)
                                        {
                                            Logs.WriteLine(e.ToString());
                                            throw;
                                        }
                                    }
                                    else
                                    {
                                        await ReplyAsync($"{error} la valeur entrée est invalide");
                                        Logs.WriteLine($"{error} la valeur entrée est invalide");
                                    }
                                }
                                else
                                {
                                    await ReplyAsync($"{error} Compte \"**{argus[2]}**\" inexistant: stats create (nom_Personnage) pour créer un nouveau compte");
                                    Logs.WriteLine($"{error} Compte \"**{argus[2]}**\" inexistant: stats create (nom_Personnage) pour créer un nouveau compte");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (this.Context.Guild.Name == "ServeurTest")
                            await ReplyAsync($"Vous devez être admin pour utiliser cette commande");
                        else
                            await ReplyAsync($"Vous devez être admin pour utiliser cette commande");
                    }
                }
                // =============================================================================
                // = Gestion de la commande (admin) stats set (stat) (nom_Personnage) (valeur) =
                // =============================================================================
                else if (input.StartsWith("set"))
                {
                    if (Global.HasRole((SocketGuildUser)this.Context.User, "Admin"))
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours add
                        if (argus[0] == "set")
                        {
                            if (argus.Length > 4) // Sert à s'assurer qu'il y a - de 5 paramètres
                            {
                                await ReplyAsync($"{error} Nombre max d'arguments dépassé");
                                Logs.WriteLine($"{error} Nombre max d'arguments dépassé");
                            }
                            else if (argus.Length < 4) // Sert à s'assurer qu'il y a + de 3 paramètres
                            {
                                await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                                Logs.WriteLine($"{error} Nombre insuffisant d'arguments");
                            }
                            else
                            {
                                StatsAccount depositAccount = await Global.GetStatsAccountByNameAsync(nomFichier, argus[2]);
                                if (depositAccount != null)
                                {
                                    string dpName = depositAccount.Name;
                                    uint dpForce = depositAccount.Force;
                                    uint dpAgilité = depositAccount.Agilite;
                                    uint dpTechnique = depositAccount.Technique;
                                    uint dpMagie = depositAccount.Magie;
                                    uint dpResistance = depositAccount.Resistance;
                                    uint dpIntelligence = depositAccount.Intelligence;
                                    uint dpSociabilite = depositAccount.Sociabilite;
                                    uint dpEsprit = depositAccount.Esprit;
                                    ulong dpUserId = depositAccount.UserId;
                                    string stat = argus[1];
                                    if (argus[2].Contains('_'))
                                        argus[2] = argus[2].Replace("_", " ");
                                    if (argus[2].Contains('-'))
                                        argus[2] = argus[2].Replace("-", " ");
                                    if (uint.TryParse(argus[3], out uint valeurAjout))
                                    {
                                        try
                                        {
                                            switch (stat.ToLower())
                                            {
                                                case "str":
                                                case "force":
                                                    dpForce = valeurAjout;
                                                    stat = "force";
                                                    break;
                                                case "agi":
                                                case "agilite":
                                                case "agilité":
                                                    dpAgilité = valeurAjout;
                                                    stat = "agilité";
                                                    break;
                                                case "tec":
                                                case "technique":
                                                    dpTechnique = valeurAjout;
                                                    stat = "technique";
                                                    break;
                                                case "mag":
                                                case "magie":
                                                    dpMagie = valeurAjout;
                                                    stat = "magie";
                                                    break;
                                                case "res":
                                                case "resi":
                                                case "resistance":
                                                    dpResistance = valeurAjout;
                                                    stat = "resistance";
                                                    break;
                                                case "int":
                                                case "intel":
                                                case "intelligence":
                                                    dpIntelligence = valeurAjout;
                                                    stat = "intelligence";
                                                    break;
                                                case "soc":
                                                case "socio":
                                                case "social":
                                                case "sociabilité":
                                                case "sociabilite":
                                                    dpSociabilite += valeurAjout;
                                                    stat = "sociabilité";
                                                    break;
                                                case "esp":
                                                case "esprit":
                                                    dpEsprit = valeurAjout;
                                                    stat = "esprit";
                                                    break;
                                            }

                                            statsAccounts.RemoveAt(await Global.GetStatsAccountIndexByNameAsync(nomFichier, argus[2]));
                                            Global.EnregistrerDonneesStats(nomFichier, statsAccounts);
                                            StatsAccount newAccount = new StatsAccount(dpName, dpForce, dpAgilité, dpTechnique, dpMagie, dpResistance, dpIntelligence, dpSociabilite, dpEsprit, dpUserId);
                                            statsAccounts.Add(newAccount);
                                            Global.EnregistrerDonneesStats(nomFichier, statsAccounts);
                                            await ReplyAsync($"\"**{dpName}**\" a désormais **{valeurAjout}** points en **{stat}**");
                                            Logs.WriteLine($"\"**{dpName}**\" a désormais **{valeurAjout}** points en **{stat}**");
                                            Logs.WriteLine(newAccount.ToString());
                                        }
                                        catch (Exception e)
                                        {
                                            Logs.WriteLine(e.ToString());
                                            throw;
                                        }
                                    }
                                    else
                                    {
                                        await ReplyAsync($"{error} la valeur entrée est invalide");
                                        Logs.WriteLine($"{error} la valeur entrée est invalide");
                                    }
                                }
                                else
                                {
                                    await ReplyAsync($"{error} Compte \"**{argus[2]}**\" inexistant: stats create (nom_Personnage) pour créer un nouveau compte");
                                    Logs.WriteLine($"{error} Compte \"**{argus[2]}**\" inexistant: stats create (nom_Personnage) pour créer un nouveau compte");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (this.Context.Guild.Name == "ServeurTest")
                            await this.Context.Channel.SendMessageAsync($"Vous devez être admin pour utiliser cette commande");
                        else
                            await this.Context.Channel.SendMessageAsync($"Vous devez être admin pour utiliser cette commande");
                    }
                }
                // =======================================================
                // = Gestion de la commande stats create (nomPersonnage) =
                // =======================================================
                else if (input.StartsWith("create") || input.StartsWith("cr"))
                {
                    if (Global.HasRole((SocketGuildUser) this.Context.User, "RP") || Global.IsStaff((SocketGuildUser) this.Context.User))
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours create
                        if (argus[0] == "create" || argus[0] == "cr")
                        {
                            if (argus[1].Contains('_'))
                                argus[1] = argus[1].Replace("_", " ");
                            if (argus[1].Contains('-'))
                                argus[1] = argus[1].Replace("-", " ");
                            if (argus.Length > 3) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 2 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre max d'arguments dépassé");
                                Logs.WriteLine($"{error} Nombre max d'arguments dépassé");
                            }
                            else if (argus.Length < 2) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 2 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                                Logs.WriteLine($"{error} Nombre insuffisant d'arguments");
                            }
                            else if (await Global.GetStatsAccountByNameAsync(nomFichier, argus[1]) == null)
                            {
                                StatsAccount newAccount;
                                if (mentionedUser != null)
                                {
                                    ulong crUserId = mentionedUser.Id;
                                    newAccount = new StatsAccount(argus[1], 0, 0, 0, 0, 0, 0, 0, 0, crUserId);
                                }
                                else
                                {
                                    newAccount = new StatsAccount(argus[1], 0, 0, 0, 0, 0, 0, 0, 0, userId);
                                }

                                statsAccounts.Add(newAccount);
                                Global.EnregistrerDonneesStats(nomFichier, statsAccounts);
                                await ReplyAsync($"Compte de {argus[1]} créé");
                                Logs.WriteLine($"Compte de {argus[1]} créé");
                            }
                            else if (await Global.GetStatsAccountByNameAsync(nomFichier, argus[1]) != null)
                            {
                                await ReplyAsync($"{error} Le compte \"**{argus[1]}**\" existe déjà");
                                Logs.WriteLine($"{error} Le compte \"**{argus[1]}**\" existe déjà");
                            }
                        }
                    }
                    else
                    {
                        if (this.Context.Guild.Name == "ServeurTest")
                            await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(541492279894999080).Mention} ou avoir le rôle {this.Context.Guild.GetRole(545753914998259715).Mention} pour utiliser cette commande");
                        else
                            await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(420536907525652482).Mention} ou avoir le rôle {this.Context.Guild.GetRole(545753914998259715).Mention} pour utiliser cette commande");
                    }
                }
                // =======================================================
                // = Gestion de la commande stats delete (nomPersonnage) =
                // =======================================================
                else if (input.StartsWith("delete") || input.StartsWith("del"))
                {
                    argus = input.Split(' ');
                    if (Global.IsStaff((SocketGuildUser) this.Context.User) || userId == (await Global.GetStatsAccountByNameAsync(nomFichier, argus[1])).UserId)
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours add
                        if (argus[0] == "delete" || argus[0] == "del")
                        {
                            if (argus[1].Contains('_'))
                                argus[1] = argus[1].Replace("_", " ");
                            if (argus[1].Contains('-'))
                                argus[1] = argus[1].Replace("-", " ");
                            if (argus.Length > 2) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 2 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre max d'arguments dépassé");
                                Logs.WriteLine($"{error} Nombre max d'arguments dépassé");
                            }
                            else if (argus.Length < 2) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 2 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                                Logs.WriteLine($"{error} Nombre insuffisant d'arguments");
                            }
                            else if (await Global.GetStatsAccountIndexByNameAsync(nomFichier, argus[1]) != -1)
                            {
                                int toRemIndex = await Global.GetStatsAccountIndexByNameAsync(nomFichier, argus[1]);
                                statsAccounts.RemoveAt(toRemIndex);
                                Global.EnregistrerDonneesStats(nomFichier, statsAccounts);
                                await ReplyAsync($"Compte de {argus[1]} supprimé");
                                Logs.WriteLine($"Compte de {argus[1]} supprimé");
                            }
                            else if (await Global.GetStatsAccountIndexByNameAsync(nomFichier, argus[1]) == -1)
                            {
                                await ReplyAsync($"{error} Compte \"**{argus[1]}**\"  inexistant: stats create (nom_Personnage) pour créer un nouveau compte");
                                Logs.WriteLine($"{error} Compte \"**{argus[1]}**\"  inexistant: stats create (nom_Personnage) pour créer un nouveau compte");
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
                // =====================================
                // = Gestion de la commande stats sort =
                // =====================================
                else if (input == "sort")
                {
                    try
                    {
                        List<StatsAccount> sortedList = statsAccounts.OrderBy(o => o.Name).ToList();
                        Global.EnregistrerDonneesStats(nomFichier, sortedList);
                        await ReplyAsync("La liste des comptes a été triée par ordre alphabétique");
                    }
                    catch (Exception e)
                    {
                        Logs.WriteLine(e.ToString());
                        throw;
                    }
                }
                // ===============================================================================
                // = Gestion de la commande (admin) stats setowner (nom_Personnage) @utilisateur =
                // ===============================================================================
                else if (input.StartsWith("so") || input.StartsWith("setowner") || input.StartsWith("setown"))
                {
                    if (Global.HasRole((SocketGuildUser) this.Context.User, "Admin"))
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours setowner
                        if (argus[0] == "setowner" || argus[0] == "setown" || argus[0] == "so")
                        {
                            if (argus.Length > 3) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre max d'arguments dépassé");
                                Logs.WriteLine($"{error} Nombre max d'arguments dépassé");
                            }
                            else if (argus.Length < 3) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                                Logs.WriteLine($"{error} Nombre insuffisant d'arguments");
                            }
                            else
                            {
                                if (argus[1].Contains('_'))
                                    argus[1] = argus[1].Replace("_", " ");
                                if (argus[1].Contains('-'))
                                    argus[1] = argus[1].Replace("-", " ");
                                StatsAccount setAccount = await Global.GetStatsAccountByNameAsync(nomFichier, argus[1]);
                                if (setAccount != null)
                                {
                                    if (mentionedUser != null)
                                    {
                                        try
                                        {
                                            string soName = setAccount.Name;
                                            uint soForce = setAccount.Force;
                                            uint soAgilité = setAccount.Agilite;
                                            uint soTechnique = setAccount.Technique;
                                            uint soMagie = setAccount.Magie;
                                            uint soResistance = setAccount.Resistance;
                                            uint soIntelligence = setAccount.Intelligence;
                                            uint soSociabilite = setAccount.Sociabilite;
                                            uint soEsprit = setAccount.Esprit;
                                            ulong soUserId = mentionedUser.Id;

                                            statsAccounts.RemoveAt(await Global.GetStatsAccountIndexByNameAsync(nomFichier, soName));
                                            StatsAccount newAccount = new StatsAccount(soName, soForce, soAgilité, soTechnique, soMagie, soResistance, soIntelligence, soSociabilite, soEsprit, soUserId);
                                            statsAccounts.Add(newAccount);
                                            Global.EnregistrerDonneesStats(nomFichier, statsAccounts);
                                            await ReplyAsync($"Le propriétaire du compte de \"**{soName}**\" est désormais \"**{mentionedUser.Mention}**\"");
                                            Logs.WriteLine($"Le propriétaire du compte de \"**{soName}**\" est désormais \"**{mentionedUser.Mention}**\"");
                                            Logs.WriteLine(newAccount.ToString());
                                        }
                                        catch (Exception e)
                                        {
                                            Logs.WriteLine(e.ToString());
                                            throw;
                                        }
                                    }
                                    else
                                    {
                                        await ReplyAsync($"{error} Vous devez mentionner un utilisateur (@utilisateur)");
                                        Logs.WriteLine($"{error} Vous devez mentionner un utilisateur (@utilisateur)");
                                    }
                                }
                                else
                                {
                                    await ReplyAsync($"{error} Compte \"**{argus[2]}**\" inexistant: bank add (nom_Personnage) pour créer un nouveau compte");
                                    Logs.WriteLine($"{error} Compte \"**{argus[2]}**\" inexistant: bank add (nom_Personnage) pour créer un nouveau compte");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (this.Context.Guild.Name == "ServeurTest")
                            await ReplyAsync($"Vous devez être admin pour utiliser cette commande");
                        else
                            await ReplyAsync($"Vous devez être admin pour utiliser cette commande");
                    }
                }
                // =============================================================================
                // = Gestion de la commande (admin) stats rename (nom_Personnage) (nouveauNom) =
                // =============================================================================
                else if (input.StartsWith("rename") || input.StartsWith("setname") || input.StartsWith("rn"))
                {
                    if (Global.HasRole((SocketGuildUser) this.Context.User, "Admin"))
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours setowner
                        if (argus[0] == "rename" || argus[0] == "setname" || argus[0] == "rn")
                        {
                            if (argus[1].Contains('_'))
                                argus[1] = argus[1].Replace("_", " ");
                            if (argus[1].Contains('-'))
                                argus[1] = argus[1].Replace("-", " ");
                            if (argus[2].Contains('_'))
                                argus[2] = argus[2].Replace("_", " ");
                            if (argus[2].Contains('-'))
                                argus[2] = argus[2].Replace("-", " ");
                            if (argus.Length > 3) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre max d'arguments dépassé");
                                Logs.WriteLine($"{error} Nombre max d'arguments dépassé");
                            }
                            else if (argus.Length < 3) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                                Logs.WriteLine($"{error} Nombre insuffisant d'arguments");
                            }
                            else
                            {
                                StatsAccount setAccount = await Global.GetStatsAccountByNameAsync(nomFichier, argus[1]);
                                if (setAccount != null && await Global.GetStatsAccountByNameAsync(nomFichier, argus[2]) == null)
                                {
                                    try
                                    {
                                        string rnName = setAccount.Name;
                                        string rnNewName = argus[2];
                                        uint rnForce = setAccount.Force;
                                        uint rnAgilité = setAccount.Agilite;
                                        uint rnTechnique = setAccount.Technique;
                                        uint rnMagie = setAccount.Magie;
                                        uint rnResistance = setAccount.Resistance;
                                        uint rnIntelligence = setAccount.Intelligence;
                                        uint rnSociabilite = setAccount.Sociabilite;
                                        uint rnEsprit = setAccount.Esprit;
                                        if (mentionedUser != null)
                                        {
                                            ulong rnUserId = mentionedUser.Id;

                                            statsAccounts.RemoveAt(await Global.GetStatsAccountIndexByNameAsync(nomFichier, rnName));
                                            Global.EnregistrerDonneesStats(nomFichier, statsAccounts);
                                            StatsAccount newAccount = new StatsAccount(rnNewName, rnForce, rnAgilité, rnTechnique, rnMagie, rnResistance, rnIntelligence, rnSociabilite, rnEsprit, rnUserId);
                                            statsAccounts.Add(newAccount);
                                            Global.EnregistrerDonneesStats(nomFichier, statsAccounts);
                                            await ReplyAsync($"Le nom du compte de \"**{rnName}**\" est désormais \"**{rnNewName}**\"");
                                            Logs.WriteLine($"Le nom du compte de \"**{rnName}**\" est désormais \"**{rnNewName}**\"");
                                            Logs.WriteLine(newAccount.ToString());
                                        }
                                        else
                                        {
                                            await ReplyAsync($"{error} vous devez mentionner un utilisateur");
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Logs.WriteLine(e.ToString());
                                        throw;
                                    }
                                }
                                else if (await Global.GetStatsAccountByNameAsync(nomFichier, argus[2]) != null)
                                {
                                    await ReplyAsync($"{error} Le compte \"**{argus[2]}**\" existe déjà");
                                    Logs.WriteLine($"{error} Le compte \"**{argus[2]}**\" existe déjà");
                                }
                                else
                                {
                                    await ReplyAsync($"{error} Compte \"**{argus[1]}**\" inexistant: bank add (nom_Personnage) pour créer un nouveau compte");
                                    Logs.WriteLine($"{error} Compte \"**{argus[1]}**\" inexistant: bank add (nom_Personnage) pour créer un nouveau compte");
                                }
                            }
                        }
                    }
                    else
                    {
                        if (this.Context.Guild.Name == "ServeurTest")
                            await ReplyAsync($"Vous devez être admin pour utiliser cette commande");
                        else
                            await ReplyAsync($"Vous devez être admin pour utiliser cette commande");
                    }
                }
                else
                {
                    try
                    {
                        await ReplyAsync(error);
                    }
                    catch (Exception e)
                    {
                        Logs.WriteLine(e.ToString());
                        throw;
                    }
                }
            }
            else if (input == "none")
            {
                try
                {
                    await ReplyAsync(error);
                    Logs.WriteLine(error);
                }
                catch (Exception e)
                {
                    Logs.WriteLine(e.ToString());
                    throw;
                }
            }
        }
        #endregion
    }
}