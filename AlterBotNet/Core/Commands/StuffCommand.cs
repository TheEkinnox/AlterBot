#region MÉTADONNÉES

// Nom du fichier : BankCommand.cs
// Auteur : Loick OBIANG (1832960)
// Date de création : 2019-02-07
// Date de modification : 2019-02-11

#endregion

#region USING

using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Classe des commandes stuff
    /// </summary>
    public class StuffCommand : ModuleBase<SocketCommandContext>
    {
        private Random _rand = new Random();
        #region MÉTHODES

        [Command("stuff"), Alias("stf","inv"), Summary("Affiche le stuff d'un personnage")]
        public async Task SendStuff([Remainder]string input = "none")
        {
            SocketUser mentionedUser = this.Context.Message.MentionedUsers.FirstOrDefault();
            StuffAccount methodes = new StuffAccount("");
            string[] argus;
            ulong userId = this.Context.User.Id;
            string error = "Valeur invalide, stuff help pour plus d'information.";
            string message = "";
            string nomFichier = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1\AlterBotNet.dll", @"Ressources\Database\stuff.altr");

            //string nomFichier = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\testBank.exe", @"Ressources\Database\bank.altr");
            List<StuffAccount> stuffAccounts = await methodes.ChargerDonneesPersosAsync(nomFichier);

            if (input != "none")
            {
                // ====================================
                // = Gestion de la commande stuff help =
                // ====================================
                if (input == "help")
                {
                    string staff = "";
                    message += "Aide sur la commande: `stuff help`\n";
                    staff += "(staff) Afficher la liste des comptes: `stuff list`\n";
                    message += "Afficher les objets d'un personnage: `stuff info (nom_Personnage)`\n";
                    staff += "(staff) Ajouter un objet à un personnage: `stuff add (objet) (nom_Personnage)`\n";
                    staff += "(staff) Retirer un objet à un personnage: `stuff remove (objet) (nom_Personnage)`\n";
                    message += "Transférer un objet d'un compte à un autre: `stuff give (objet) (nom_Personnage1) (nom_Personnage2)`\n";
                    staff += "(staff) (RP) Créer un nouveau compte: `stuff create (nomPersonnage)`\n";
                    staff += "(staff) Supprimer un compte: `stuff delete (nomPersonnage)`\n";
                    message += "Trier la liste des comptes (par ordre alphabétique): `stuff sort`\n";
                    staff += "(staff) Définir le propriétaire d'un personnage: `stuff setowner (nom_personnage) (@propriétaire)`\n";
                    staff += "(staff) Changer le nom d'un personnage: `bank rename (nom_personnage) (nouveauNom)`\n";
                    staff += "(staff) Remplacer un objet dans l'inventaire d'un personnage: `bank replace (nom_personnage) (objet) (nouvel-objet)`\n";
                    try
                    {
                        await ReplyAsync("Aide envoyée en mp");
                        Logs.WriteLine($"message envoyé en mp à {this.Context.User.Username}");
                        EmbedBuilder eb = new EmbedBuilder();
                        eb.WithTitle(("**Aide de la commande stuff (stf,inv)**"))
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
                // = Gestion de la commande (staff) stuff list =
                // =============================================
                else if (input == "list")
                {
                    if (IsStaff((SocketGuildUser)this.Context.User))
                    {
                        if (string.IsNullOrEmpty(await methodes.AccountsListAsync(nomFichier)))
                        {
                            await ReplyAsync("Liste vide");
                            Logs.WriteLine("Liste vide");
                        }
                        else
                        {
                            try
                            {
                                await ReplyAsync(await methodes.AccountsListAsync(nomFichier));
                                Logs.WriteLine(await methodes.AccountsListAsync(nomFichier));
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
                // ======================================================
                // = Gestion de la commande stuff info (nom_Personnage) =
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
                            StuffAccount infoAccount = await methodes.GetStuffAccountByNameAsync(nomFichier, argus[1]);
                            if (infoAccount != null && (infoAccount.UserId == userId || IsStaff((SocketGuildUser)this.Context.User)))
                            {
                                try
                                {
                                    await ReplyAsync("Infos envoyées en mp");
                                    Logs.WriteLine($"message envoyé en mp à {this.Context.User.Username}");
                                    EmbedBuilder eb = new EmbedBuilder();
                                    eb.WithTitle(($"Inventaire de **{infoAccount.Name}**"))
                                        .WithColor(this._rand.Next(256), this._rand.Next(256), this._rand.Next(256))
                                        .AddField("==============================================", infoAccount.ToString());
                                    //await this.Context.User.SendMessageAsync(infoAccount.ToString());
                                    await this.Context.User.SendMessageAsync("", false, eb.Build());
                                    Logs.WriteLine(infoAccount.ToString());
                                }
                                catch (Exception e)
                                {
                                    Logs.WriteLine(e.ToString());
                                    throw;
                                }
                            }
                            else
                            {
                                if (infoAccount == null)
                                {
                                    await ReplyAsync($"{error} Compte \"**{argus[1]}**\" inexistant: stuff create (nom_Personnage) pour créer un nouveau compte");
                                    Logs.WriteLine($"{error} Compte \"**{argus[1]}**\" inexistant: stuff create (nom_Personnage) pour créer un nouveau compte");
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
                // =====================================================================
                // = Gestion de la commande (staff) stuff add (objet) (nom_Personnage) =
                // =====================================================================
                else if (input.StartsWith("add"))
                {
                    if (IsStaff((SocketGuildUser)this.Context.User))
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours add
                        if (argus[0] == "add")
                        {
                            if (argus.Length > 3) // Sert à s'assurer que argus[2] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre max d'arguments dépassé");
                                Logs.WriteLine($"{error} Nombre max d'arguments dépassé");
                            }
                            else if (argus.Length < 3) // Sert à s'assurer que argus[2] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                                Logs.WriteLine($"{error} Nombre insuffisant d'arguments");
                            }
                            else
                            {
                                StuffAccount depositAccount = await methodes.GetStuffAccountByNameAsync(nomFichier, argus[2]);
                                if (depositAccount != null)
                                {
                                    string dpName = depositAccount.Name;
                                    List<string> dpItems = depositAccount.Items;
                                    ulong dpUserId = depositAccount.UserId;
                                    if (argus[1].Contains('_'))
                                        argus[1] = argus[1].Replace("_", " ");
                                    if (argus[1].Contains('-'))
                                        argus[1] = argus[1].Replace("-", " ");
                                    try
                                    {
                                        dpItems.Add(argus[1]);
                                        stuffAccounts.RemoveAt(await methodes.GetStuffAccountIndexByNameAsync(nomFichier, argus[2]));
                                        methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                        StuffAccount newAccount = new StuffAccount(dpName, dpItems, dpUserId);
                                        stuffAccounts.Add(newAccount);
                                        methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                        await ReplyAsync($"Objet \"**{argus[1]}**\" ajouté sur le compte de \"**{dpName}**\"");
                                        Logs.WriteLine($"Objet \"**{argus[1]}**\" ajouté sur le compte de \"**{dpName}**\"");
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
                                    await ReplyAsync($"{error} Compte \"**{argus[2]}**\" inexistant: stuff create (nom_Personnage) pour créer un nouveau compte");
                                    Logs.WriteLine($"{error} Compte \"**{argus[2]}**\" inexistant: stuff create (nom_Personnage) pour créer un nouveau compte");
                                }
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
                // ========================================================================
                // = Gestion de la commande (admin) stuff remove (objet) (nom_Personnage) =
                // ========================================================================
                else if (input.StartsWith("remove") || input.StartsWith("rem"))
                {
                    if (IsStaff((SocketGuildUser)this.Context.User))
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours remove
                        if (argus[0] == "remove" || argus[0] == "rem")
                        {
                            if (argus.Length > 3) // Sert à s'assurer qu'il n'y a que 3 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre max d'arguments dépassé");
                                Logs.WriteLine($"{error} Nombre max d'arguments dépassé");
                            }
                            else if (argus.Length < 3) // Sert à s'assurer qu'il n'y a que 3 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                                Logs.WriteLine($"{error} Nombre insuffisant d'arguments");
                            }
                            else
                            {
                                StuffAccount withdrawAccount = await methodes.GetStuffAccountByNameAsync(nomFichier, argus[2]);
                                if (withdrawAccount != null)// Sert à s'assurer que le compte existe bien
                                {
                                    string wdName = withdrawAccount.Name;
                                    List<string> wdItems = withdrawAccount.Items;
                                    ulong wdUserId = withdrawAccount.UserId;
                                    if (argus[1].Contains('_'))
                                        argus[1] = argus[1].Replace("_", " ");
                                    if (argus[1].Contains('-'))
                                        argus[1] = argus[1].Replace("-", " ");
                                    if (int.TryParse(argus[1],out int indexObj))
                                    {
                                        if (!string.IsNullOrEmpty(wdItems[indexObj]))
                                        {
                                            try
                                            {
                                                string nomObj = wdItems[indexObj];
                                                wdItems.RemoveAt(indexObj);
                                                stuffAccounts.RemoveAt(await methodes.GetStuffAccountIndexByNameAsync(nomFichier, argus[2]));
                                                methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                                StuffAccount newAccount = new StuffAccount(wdName, wdItems, wdUserId);
                                                stuffAccounts.Add(newAccount);
                                                methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                                await ReplyAsync($"Objet \"**{nomObj}**\" retiré du compte de \"**{wdName}**\"");
                                                Logs.WriteLine($"Objet \"**{nomObj}**\" retiré du compte de \"**{wdName}**\"");
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
                                            try
                                            {
                                                await ReplyAsync($"{error} \"**{wdName}**\" ne possède pas d'objet à l'index \"**{argus[1]}**\"");
                                                Logs.WriteLine($"{error} \"**{wdName}**\" ne possède pas d'objet à l'index \"**{argus[1]}**\"");
                                            }
                                            catch (Exception e)
                                            {
                                                Logs.WriteLine(e.ToString());
                                                throw;
                                            }
                                        }
                                    }
                                    else if (wdItems.Contains(argus[1]))
                                    {
                                        try
                                        {
                                            wdItems.Remove(argus[1]);
                                            stuffAccounts.RemoveAt(await methodes.GetStuffAccountIndexByNameAsync(nomFichier, argus[2]));
                                            methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                            StuffAccount newAccount = new StuffAccount(wdName, wdItems, wdUserId);
                                            stuffAccounts.Add(newAccount);
                                            methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                            await ReplyAsync($"Objet \"**{argus[1]}**\" retiré du compte de \"**{wdName}**\"");
                                            Logs.WriteLine($"Objet \"**{argus[1]}**\" retiré du compte de \"**{wdName}**\"");
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
                                        await ReplyAsync($"{error} \"**{wdName}**\" ne possède pas l'objet \"**{argus[1]}**\"");
                                        Logs.WriteLine($"{error} \"**{wdName}**\" ne possède pas l'objet \"**{argus[1]}**\"");
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
                            await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(541492279894999080).Mention} pour utiliser cette commande");
                        else
                            await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(420536907525652482).Mention} pour utiliser cette commande");
                    }
                }
                // ===================================================================================
                // = Gestion de la commande stuff give (objet) (nom_Personnage1) (nom_Personnage2) =
                // ===================================================================================
                else if (input.StartsWith("give") || input.StartsWith("transfer") || input.StartsWith("tr"))
                {
                    argus = input.Split(' ');
                    // Sert à s'assurer qu'argus[0] == toujours withdraw
                    if (argus[0] == "give" || argus[0] == "transfer" || argus[0] == "tr")
                    {
                        if (argus.Length > 4) // Sert à s'assurer que argus[1] == forcément objet (et qu'il n'y a que 4 paramètres)
                        {
                            await ReplyAsync($"{error} Nombre max d'arguments dépassé");
                            Logs.WriteLine($"{error} Nombre max d'arguments dépassé");
                        }
                        else if (argus.Length < 4) // Sert à s'assurer que argus[1] == forcément montant (et qu'il n'y a que 4 paramètres)
                        {
                            await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                            Logs.WriteLine($"{error} Nombre insuffisant d'arguments");
                        }
                        else
                        {
                            StuffAccount withdrawAccount = await methodes.GetStuffAccountByNameAsync(nomFichier, argus[2]);
                            StuffAccount depositAccount = await methodes.GetStuffAccountByNameAsync(nomFichier, argus[3]);
                            if (withdrawAccount != null && depositAccount != null)
                            {
                                // Paramètres pour le compte qui donne
                                string wdName = withdrawAccount.Name;
                                List<string> wdItems = withdrawAccount.Items;
                                ulong wdUserId = withdrawAccount.UserId;
                                // Paramètres pour le compte qui reçoit
                                string dpName = depositAccount.Name;
                                List<string> dpItems = depositAccount.Items;
                                ulong dpUserId = depositAccount.UserId;
                                if (argus[1].Contains('_'))
                                    argus[1] = argus[1].Replace("_", " ");
                                if (argus[1].Contains('-'))
                                    argus[1] = argus[1].Replace("-", " ");
                                if (int.TryParse(argus[1], out int indexObj))
                                {
                                    if (!string.IsNullOrEmpty(wdItems[indexObj]))
                                    {
                                        string nomObj = wdItems[indexObj];
                                        wdItems.RemoveAt(indexObj);
                                        stuffAccounts.RemoveAt(await methodes.GetStuffAccountIndexByNameAsync(nomFichier, argus[2]));
                                        methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                        StuffAccount newAccount = new StuffAccount(wdName, wdItems, wdUserId);
                                        stuffAccounts.Add(newAccount);
                                        methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                        Logs.WriteLine($"Objet \"**{nomObj}**\" retiré du compte de \"**{wdName}**\"");
                                        Logs.WriteLine(newAccount.ToString());
                                        dpItems.Add(nomObj);
                                        stuffAccounts.RemoveAt(await methodes.GetStuffAccountIndexByNameAsync(nomFichier, dpName));
                                        methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                        StuffAccount newDpAccount = new StuffAccount(dpName, dpItems, dpUserId);
                                        stuffAccounts.Add(newDpAccount);
                                        methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                        Logs.WriteLine($"Objet \"**{nomObj}**\" ajouté sur le compte de \"**{dpName}**\"");
                                        Logs.WriteLine(newDpAccount.ToString());

                                        await ReplyAsync($"L'objet \"**{nomObj}**\" a été transféré du compte de {wdName} vers le compte de {dpName}");
                                        Logs.WriteLine($"L'objet \"**{nomObj}**\" a été transféré du compte de {wdName} vers le compte de {dpName}");
                                    }
                                    else
                                    {
                                        await ReplyAsync($"{error} \"**{wdName}**\" ne possède pas d'objet à l'index \"**{argus[1]}**\"");
                                        Logs.WriteLine($"{error} \"**{wdName}**\" ne possède pas d'objet à l'index \"**{argus[1]}**\"");
                                    }
                                }
                                else if (wdItems.Contains(argus[1]))
                                {
                                    try
                                    {
                                        wdItems.Remove(argus[1]);
                                        stuffAccounts.RemoveAt(await methodes.GetStuffAccountIndexByNameAsync(nomFichier, wdName));
                                        methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                        StuffAccount newAccount = new StuffAccount(wdName, wdItems, wdUserId);
                                        stuffAccounts.Add(newAccount);
                                        methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                        Logs.WriteLine($"Objet \"**{argus[1]}**\" retiré du compte de \"**{wdName}**\"");
                                        Logs.WriteLine(newAccount.ToString());
                                    }
                                    catch (Exception e)
                                    {
                                        Logs.WriteLine(e.ToString());
                                        return;
                                    }
                                    try
                                    {
                                        dpItems.Add(argus[1]);
                                        stuffAccounts.RemoveAt(await methodes.GetStuffAccountIndexByNameAsync(nomFichier, dpName));
                                        methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                        StuffAccount newDpAccount = new StuffAccount(dpName, dpItems, dpUserId);
                                        stuffAccounts.Add(newDpAccount);
                                        methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                        Logs.WriteLine($"Objet \"**{argus[1]}**\" ajouté sur le compte de \"**{dpName}**\"");
                                        Logs.WriteLine(newDpAccount.ToString());
                                    }
                                    catch (Exception e)
                                    {
                                        Logs.WriteLine(e.ToString());
                                        return;
                                    }

                                    await ReplyAsync($"L'objet \"**{argus[1]}**\" a été transféré du compte de {wdName} vers le compte de {dpName}");
                                    Logs.WriteLine($"L'objet \"**{argus[1]}**\" a été transféré du compte de {wdName} vers le compte de {dpName}");
                                }
                                else
                                {
                                    if (dpItems.Contains(argus[1]))
                                    {
                                        await ReplyAsync($"{error} \"**{dpName}**\" possède déjà l'objet \"**{argus[1]}**\"");
                                        Logs.WriteLine($"{error} \"**{dpName}**\" possède déjà l'objet \"**{argus[1]}**\"");
                                    }
                                    else if (!wdItems.Contains(argus[1]))
                                    {
                                        await ReplyAsync($"{error} \"**{wdName}**\" ne possède pas l'objet \"**{argus[1]}**\"");
                                        Logs.WriteLine($"{error} \"**{wdName}**\" ne possède pas l'objet \"**{argus[1]}**\"");
                                    }
                                }
                            }
                            else
                            {
                                await ReplyAsync($"{error} Compte(s) \"**{argus[2]}**\" et/ou \"**{argus[3]}**\" inexistant(s): stuff create (nom_Personnage) pour créer un nouveau compte");
                                Logs.WriteLine($"{error} Compte(s) \"**{argus[2]}**\" et/ou \"**{argus[3]}**\" inexistant(s): stuff create (nom_Personnage) pour créer un nouveau compte");
                            }
                        }
                    }
                }
                // =======================================================
                // = Gestion de la commande stuff create (nomPersonnage) =
                // =======================================================
                else if (input.StartsWith("create") || input.StartsWith("cr"))
                {
                    if (HasRole((SocketGuildUser) this.Context.User, "RP") || IsStaff((SocketGuildUser) this.Context.User))
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours create
                        if (argus[0] == "create" || argus[0] == "cr")
                        {
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
                            else if (await methodes.GetStuffAccountByNameAsync(nomFichier, argus[1]) == null)
                            {
                                List<string> items = new List<string>();
                                StuffAccount newAccount;
                                if (mentionedUser != null)
                                {
                                    ulong crUserId = mentionedUser.Id;
                                    newAccount = new StuffAccount(argus[1], items, crUserId);
                                }
                                else
                                {
                                    newAccount = new StuffAccount(argus[1], items, userId);
                                }

                                stuffAccounts.Add(newAccount);
                                methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                await ReplyAsync($"Compte de {argus[1]} créé");
                                Logs.WriteLine($"Compte de {argus[1]} créé");
                                Logs.WriteLine(await methodes.AccountsListAsync(nomFichier));
                            }
                            else if (await methodes.GetStuffAccountByNameAsync(nomFichier, argus[1]) != null)
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
                // = Gestion de la commande stuff delete (nomPersonnage) =
                // =======================================================
                else if (input.StartsWith("delete") || input.StartsWith("del"))
                {
                    argus = input.Split(' ');
                    if (IsStaff((SocketGuildUser)this.Context.User) || userId == (await methodes.GetStuffAccountByNameAsync(nomFichier, argus[1])).UserId)
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours add
                        if (argus[0] == "delete" || argus[0] == "del")
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
                            else if (await methodes.GetStuffAccountIndexByNameAsync(nomFichier, argus[1]) != -1)
                            {
                                int toRemIndex = await methodes.GetStuffAccountIndexByNameAsync(nomFichier, argus[1]);
                                stuffAccounts.RemoveAt(toRemIndex);
                                methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                await ReplyAsync($"Compte de {argus[1]} supprimé");
                                Logs.WriteLine($"Compte de {argus[1]} supprimé");
                                Logs.WriteLine(await methodes.AccountsListAsync(nomFichier));
                            }
                            else if (await methodes.GetStuffAccountIndexByNameAsync(nomFichier, argus[1]) == -1)
                            {
                                await ReplyAsync($"{error} Compte \"**{argus[1]}**\"  inexistant: stuff create (nom_Personnage) pour créer un nouveau compte");
                                Logs.WriteLine($"{error} Compte \"**{argus[1]}**\"  inexistant: stuff create (nom_Personnage) pour créer un nouveau compte");
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
                // = Gestion de la commande stuff sort =
                // =====================================
                else if (input == "sort")
                {
                    try
                    {
                        List<StuffAccount> sortedList = stuffAccounts.OrderBy(o => o.Name).ToList();
                        methodes.EnregistrerDonneesPersos(nomFichier, sortedList);
                        await ReplyAsync("La liste des comptes a été triée par ordre alphabétique");
                    }
                    catch (Exception e)
                    {
                        Logs.WriteLine(e.ToString());
                        throw;
                    }
                }
                // ===============================================================================
                // = Gestion de la commande (admin) stuff setowner (nom_Personnage) @utilisateur =
                // ===============================================================================
                else if (input.StartsWith("setowner") || input.StartsWith("setown") || input.StartsWith("so"))
                {
                    if (IsStaff((SocketGuildUser)this.Context.User))
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
                                StuffAccount setAccount = await methodes.GetStuffAccountByNameAsync(nomFichier, argus[1]);
                                if (setAccount != null)
                                {
                                    if (mentionedUser != null)
                                    {
                                        try
                                        {
                                            string soName = setAccount.Name;
                                            ulong soUserId = mentionedUser.Id;
                                            List<string> soItems = setAccount.Items;

                                            stuffAccounts.RemoveAt(await methodes.GetStuffAccountIndexByNameAsync(nomFichier, soName));
                                            methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                            StuffAccount newAccount = new StuffAccount(soName, soItems, soUserId);
                                            stuffAccounts.Add(newAccount);
                                            methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
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
                            await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(541492279894999080).Mention} pour utiliser cette commande");
                        else
                            await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(420536907525652482).Mention} pour utiliser cette commande");
                    }
                }
                // =============================================================================
                // = Gestion de la commande (admin) stuff rename (nom_Personnage) (nouveauNom) =
                // =============================================================================
                else if (input.StartsWith("rename") || input.StartsWith("setname") || input.StartsWith("rn"))
                {
                    if (IsStaff((SocketGuildUser)this.Context.User))
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours setowner
                        if (argus[0] == "rename" || argus[0] == "setname" || argus[0] == "rn")
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
                                StuffAccount setAccount = await methodes.GetStuffAccountByNameAsync(nomFichier, argus[1]);
                                if (setAccount != null && await methodes.GetStuffAccountByNameAsync(nomFichier, argus[2]) == null)
                                {
                                    try
                                    {
                                        string rnName = setAccount.Name;
                                        string rnNewName = argus[2];
                                        ulong rnUserId = setAccount.UserId;
                                        List<string> rnItems = setAccount.Items;

                                        stuffAccounts.RemoveAt(await methodes.GetStuffAccountIndexByNameAsync(nomFichier, rnName));
                                        methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                        StuffAccount newAccount = new StuffAccount(rnNewName, rnItems, rnUserId);
                                        stuffAccounts.Add(newAccount);
                                        methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                        await ReplyAsync($"Le nom du compte de \"**{rnName}**\" est désormais \"**{rnNewName}**\"");
                                        Logs.WriteLine($"Le nom du compte de \"**{rnName}**\" est désormais \"**{rnNewName}**\"");
                                        Logs.WriteLine(newAccount.ToString());
                                    }
                                    catch (Exception e)
                                    {
                                        Logs.WriteLine(e.ToString());
                                        throw;
                                    }
                                }
                                else if ((await methodes.GetStuffAccountByNameAsync(nomFichier, argus[2]) != null))
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
                            await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(541492279894999080).Mention} pour utiliser cette commande");
                        else
                            await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(420536907525652482).Mention} pour utiliser cette commande");
                    }
                }
                // ========================================================================
                // = Gestion de la commande (admin) stuff replace (objet) (nom_Personnage) =
                // ========================================================================
                else if (input.StartsWith("replace") || input.StartsWith("rep"))
                {
                    if (IsStaff((SocketGuildUser)this.Context.User))
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours remove
                        if (argus[0] == "replace" || argus[0] == "rep")
                        {
                            if (argus.Length > 4) // Sert à s'assurer qu'il n'y a que 3 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre max d'arguments dépassé");
                                Logs.WriteLine($"{error} Nombre max d'arguments dépassé");
                            }
                            else if (argus.Length < 4) // Sert à s'assurer qu'il n'y a que 3 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                                Logs.WriteLine($"{error} Nombre insuffisant d'arguments");
                            }
                            else
                            {
                                StuffAccount repAccount = await methodes.GetStuffAccountByNameAsync(nomFichier, argus[1]);
                                if (repAccount != null)// Sert à s'assurer que le compte existe bien
                                {
                                    string repAccountName = repAccount.Name;
                                    List<string> repAccountItems = repAccount.Items;
                                    ulong repAccountUserId = repAccount.UserId;
                                    if (argus[2].Contains('_') || argus[3].Contains('_'))
                                    {
                                        argus[2] = argus[2].Replace("_", " ");
                                        argus[3] = argus[3].Replace("_", " ");
                                    }
                                    if (argus[2].Contains('-') || argus[3].Contains('-'))
                                    {
                                        argus[2] = argus[2].Replace("-", " ");
                                        argus[3] = argus[3].Replace("-", " ");
                                    }
                                    if (int.TryParse(argus[2], out int indexObj))
                                    {
                                        if (!string.IsNullOrEmpty(repAccountItems[indexObj]))
                                        {
                                            try
                                            {
                                                string nomObj = repAccountItems[indexObj];
                                                repAccountItems[indexObj] = argus[3];
                                                stuffAccounts.RemoveAt(await methodes.GetStuffAccountIndexByNameAsync(nomFichier, repAccountName));
                                                methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                                StuffAccount newAccount = new StuffAccount(repAccountName, repAccountItems, repAccountUserId);
                                                stuffAccounts.Add(newAccount);
                                                methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                                await ReplyAsync($"Objet \"**{nomObj}**\" remplacé par l'objet \"**{argus[3]}**\" sur le compte de \"**{repAccountName}**\"");
                                                Logs.WriteLine($"Objet \"**{nomObj}**\" remplacé par l'objet \"**{argus[3]}**\" sur le compte de \"**{repAccountName}**\"");
                                            }
                                            catch (Exception e)
                                            {
                                                Logs.WriteLine(e.ToString());
                                                throw;
                                            }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                await ReplyAsync($"{error} \"**{repAccountName}**\" ne possède pas d'objet à l'index \"**{argus[1]}**\"");
                                                Logs.WriteLine($"{error} \"**{repAccountName}**\" ne possède pas d'objet à l'index \"**{argus[1]}**\"");
                                            }
                                            catch (Exception e)
                                            {
                                                Logs.WriteLine(e.ToString());
                                                throw;
                                            }
                                        }
                                    }
                                    else if (!int.TryParse(argus[2], out indexObj))
                                    {
                                        await ReplyAsync($"{error} Vous devez utiliser le numéro de l'objet pour le remplacer");
                                        Logs.WriteLine($"{error} Vous devez utiliser le numéro de l'objet pour le remplacer");
                                    }
                                    else
                                    {
                                        await ReplyAsync($"{error} \"**{repAccountName}**\" ne possède pas l'objet \"**{argus[1]}**\"");
                                        Logs.WriteLine($"{error} \"**{repAccountName}**\" ne possède pas l'objet \"**{argus[1]}**\"");
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
                            await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(541492279894999080).Mention} pour utiliser cette commande");
                        else
                            await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(420536907525652482).Mention} pour utiliser cette commande");
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

        /// <summary>
        /// Vérifie si l'utilisateur est membre du Staff ou non
        /// </summary>
        /// <param name="user">Utilisateur à vérifier</param>
        /// <returns>True si l'utilisateur est membre du Staff ou false sinon</returns>
        private bool IsStaff(SocketGuildUser user)
        {
            string targetRoleName = "Staff";
            IEnumerable<ulong> result = from r in user.Guild.Roles
                                        where r.Name == targetRoleName
                                        select r.Id;
            ulong roleId = result.FirstOrDefault();
            if (roleId == 0) return false;
            SocketRole targetRole = user.Guild.GetRole(roleId);
            return user.Roles.Contains(targetRole);
        }

        /// <summary>
        /// Vérifie si l'utilisateur est membre du role indiqué ou non
        /// </summary>
        /// <param name="user">Utilisateur à vérifier</param>
        /// <param name="roleName">Nom du role à vérifier</param>
        /// <returns>True si l'utilisateur est membre du role indiqué ou false sinon</returns>
        private bool HasRole(SocketGuildUser user,string roleName)
        {
            string targetRoleName = roleName;
            IEnumerable<ulong> result = from r in user.Guild.Roles
                where r.Name == targetRoleName
                select r.Id;
            ulong roleId = result.FirstOrDefault();
            if (roleId == 0) return false;
            SocketRole targetRole = user.Guild.GetRole(roleId);
            return user.Roles.Contains(targetRole);
        }
        #endregion
    }
}