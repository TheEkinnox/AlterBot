﻿#region MÉTADONNÉES

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
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once InheritdocConsiderUsage
    public class StuffCommand : ModuleBase<SocketCommandContext>
    {
        #region MÉTHODES

        [Command("stuff"), Summary("Affiche le stuff d'un personnage")]
        public async Task SendStuff([Remainder] string input = "none")
        {
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
                    message += "**Infos sur la commande bank**\n";
                    message += "Aide sur la commande: `stuff help`\n";
                    message += "(staff) Afficher la liste des comptes: `stuff list`\n";
                    message += "Afficher les objets d'un personnage: `stuff info (nom_Personnage)`\n";
                    // Todo: Ajouter un objet à un personnage
                    message += "(staff) Ajouter un objet à un personnage: `stuff add (objet) (nom_Personnage)`\n";
                    // Todo: Retirer un objet à un personnage
                    message += "(staff) Retirer un objet à un personnage: `stuff remove (objet) (nom_Personnage)`\n";
                    // Todo: Transférer un objet d'un compte à un autre
                    message += "Transférer un objet d'un compte à un autre: `stuff give (objet) (nom_Personnage1) (nom_Personnage2)`\n";
                    // Todo: Créer un nouveau compte
                    message += "Créer un nouveau compte: `stuff create (nomPersonnage)`\n";
                    // Todo: Supprimer un compte
                    message += "(staff) Supprimer un compte: `stuff delete (nomPersonnage)`\n";
                    message += "Trier la liste des comptes (par ordre alphabétique): `stuff sort`\n";
                    await ReplyAsync("Aide envoyée en mp");
                    Console.WriteLine($"Aide envoyée à {this.Context.User.Username} en mp");
                    await this.Context.User.SendMessageAsync(message);
                    Console.WriteLine(message);
                }
                // =============================================
                // = Gestion de la commande (staff) stuff list =
                // =============================================
                else if (input == "list")
                {
                    if (IsStaff((SocketGuildUser)this.Context.User))
                    {
                        await ReplyAsync(await methodes.AccountsListAsync(nomFichier));
                        Console.WriteLine(await methodes.AccountsListAsync(nomFichier));
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
                            Console.WriteLine($"{error} Nombre max d'arguments dépassé");
                        }
                        else if (argus.Length < 2) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 2 paramètres)
                        {
                            await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                            Console.WriteLine($"{error} Nombre insuffisant d'arguments");
                        }
                        else
                        {
                            StuffAccount infoAccount = await methodes.GetStuffAccountByNameAsync(nomFichier, argus[1]);
                            if (infoAccount != null)
                            {
                                await ReplyAsync(infoAccount.ToString());
                                Console.WriteLine(infoAccount.ToString());
                            }
                            else
                            {
                                await ReplyAsync($"{error} Compte \"**{argus[1]}**\" inexistant: stuff create (nom_Personnage) pour créer un nouveau compte");
                                Console.WriteLine($"{error} Compte \"**{argus[1]}**\" inexistant: stuff create (nom_Personnage) pour créer un nouveau compte");
                            }
                        }
                    }
                }
                // ==========================================================================
                // = Gestion de la commande (admin) bank deposit (objet) (nom_Personnage) =
                // ==========================================================================
                else if (input.StartsWith("add"))
                {
                    if (IsStaff((SocketGuildUser)this.Context.User))
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours deposit
                        if (argus[0] == "add")
                        {
                            if (argus.Length > 3) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre max d'arguments dépassé");
                                Console.WriteLine($"{error} Nombre max d'arguments dépassé");
                            }
                            else if (argus.Length < 3) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                                Console.WriteLine($"{error} Nombre insuffisant d'arguments");
                            }
                            else
                            {
                                StuffAccount depositAccount = await methodes.GetStuffAccountByNameAsync(nomFichier, argus[2]);
                                if (depositAccount != null)
                                {
                                    string dpName = depositAccount.Name;
                                    string[] dpItems = depositAccount.Items;
                                    ulong dpUserId = depositAccount.UserId;
                                    stuffAccounts.RemoveAt(await methodes.GetStuffAccountIndexByNameAsync(nomFichier, argus[2]));
                                    methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                    StuffAccount newAccount = new StuffAccount(dpName, dpItems, dpUserId);
                                    stuffAccounts.Add(newAccount);
                                    methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                    await ReplyAsync($"Objet \"{argus[1]}\" ajouté sur le compte de {dpName}");
                                    Console.WriteLine($"Objet \"{argus[1]}\" ajouté sur le compte de {dpName}");
                                    Console.WriteLine(newAccount.ToString());
                                }
                                else
                                {
                                    await ReplyAsync($"{error} Compte \"**{argus[2]}**\" inexistant: bank add (nom_Personnage) pour créer un nouveau compte");
                                    Console.WriteLine($"{error} Compte \"**{argus[2]}**\" inexistant: bank add (nom_Personnage) pour créer un nouveau compte");
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
                // ===========================================================================
                // = Gestion de la commande (admin) bank withdraw (montant) (nom_Personnage) =
                // ===========================================================================
                else if (input.StartsWith("withdraw") || input.StartsWith("wd"))
                {
                    if (IsStaff((SocketGuildUser)this.Context.User))
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours withdraw
                        if (argus[0] == "withdraw" || argus[0] == "wd")
                        {
                            if (argus.Length > 3 && !decimal.TryParse(argus[1], out decimal montant)) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre max d'arguments dépassé");
                                Console.WriteLine($"{error} Nombre max d'arguments dépassé");
                            }
                            else if (argus.Length < 3) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                                Console.WriteLine($"{error} Nombre insuffisant d'arguments");
                            }
                            else
                            {
                                decimal.TryParse(argus[1], out montant);
                                StuffAccount withdrawAccount = await methodes.GetStuffAccountByNameAsync(nomFichier, argus[2]);
                                if (withdrawAccount != null)
                                {
                                    string wdName = withdrawAccount.Name;
                                    string[] wdItems = withdrawAccount.Items;
                                    ulong wdUserId = withdrawAccount.UserId;
                                    stuffAccounts.RemoveAt(await methodes.GetStuffAccountIndexByNameAsync(nomFichier, argus[2]));
                                    methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                    StuffAccount newAccount = new StuffAccount(wdName, wdItems, wdUserId);
                                    stuffAccounts.Add(newAccount);
                                    methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                    await ReplyAsync($"{montant} couronnes retirée(s) sur le compte de {argus[2]}");
                                    Console.WriteLine($"{montant} couronnes retirée(s) sur le compte de {argus[2]}");
                                    Console.WriteLine(newAccount.ToString());
                                }
                                else
                                {
                                    await ReplyAsync($"{error} Compte \"**{argus[2]}**\" inexistant: bank add (nom_Personnage) pour créer un nouveau compte");
                                    Console.WriteLine($"{error} Compte \"**{argus[2]}**\" inexistant: bank add (nom_Personnage) pour créer un nouveau compte");
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
                            Console.WriteLine($"{error} Nombre max d'arguments dépassé");
                        }
                        else if (argus.Length < 4) // Sert à s'assurer que argus[1] == forcément montant (et qu'il n'y a que 4 paramètres)
                        {
                            await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                            Console.WriteLine($"{error} Nombre insuffisant d'arguments");
                        }
                        else
                        {
                            StuffAccount withdrawAccount = await methodes.GetStuffAccountByNameAsync(nomFichier, argus[3]);
                            StuffAccount depositAccount = await methodes.GetStuffAccountByNameAsync(nomFichier, argus[3]);
                            if (withdrawAccount != null && depositAccount != null)
                            {
                                //Retire l'argent du premier compte
                                string wdName = withdrawAccount.Name;
                                string[] wdItems = withdrawAccount.Items;
                                ulong wdUserId = withdrawAccount.UserId;
                                stuffAccounts.RemoveAt(await methodes.GetStuffAccountIndexByNameAsync(nomFichier, argus[2]));
                                methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                StuffAccount newAccount = new StuffAccount(wdName, wdItems, wdUserId);
                                stuffAccounts.Add(newAccount);
                                methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                Console.WriteLine(newAccount.ToString());
                                // Ajoute l'argent sur le 2eme compte
                                string dpName = depositAccount.Name;
                                string[] dpItems = depositAccount.Items;
                                ulong dpUserId = depositAccount.UserId;
                                stuffAccounts.RemoveAt(await methodes.GetStuffAccountIndexByNameAsync(nomFichier, argus[3]));
                                methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                StuffAccount newAccountDeposit = new StuffAccount(dpName, dpItems, dpUserId);
                                stuffAccounts.Add(newAccountDeposit);
                                methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                //await ReplyAsync($"{montant} couronnes ajoutée(s) sur le compte de {argus[3]}");
                                Console.WriteLine($"Objet ajouté sur le compte de {dpName}");
                                Console.WriteLine(newAccountDeposit.ToString());
                                await ReplyAsync($"Objet transféré du compte de {wdName} vers le compte de {dpName}");
                            }
                            else
                            {
                                await ReplyAsync($"{error} Comptes \"**{argus[2]}**\" et/ou \"**{argus[3]}**\" inexistants: bank add (nom_Personnage) pour créer un nouveau compte");
                                Console.WriteLine($"{error} Comptes \"**{argus[2]}**\" et/ou \"**{argus[3]}**\" inexistants: bank add (nom_Personnage) pour créer un nouveau compte");
                            }
                        }
                    }
                }
                // ===================================================
                // = Gestion de la commande bank create (nomPersonnage) =
                // ===================================================
                else if (input.StartsWith("create") || input.StartsWith("cr"))
                {
                    argus = input.Split(' ');
                    // Sert à s'assurer qu'argus[0] == toujours add
                    if (argus[0] == "create" || argus[0] == "cr")
                    {
                        if (argus.Length > 2) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 2 paramètres)
                        {
                            await ReplyAsync($"{error} Nombre max d'arguments dépassé");
                            Console.WriteLine($"{error} Nombre max d'arguments dépassé");
                        }
                        else if (argus.Length < 2) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 2 paramètres)
                        {
                            await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                            Console.WriteLine($"{error} Nombre insuffisant d'arguments");
                        }
                        else if (await methodes.GetStuffAccountByNameAsync(nomFichier, argus[1]) == null)
                        {
                            List<string> items = new List<string>();
                            StuffAccount newAccount = new StuffAccount(argus[1], items.ToArray(), userId);
                            stuffAccounts.Add(newAccount);
                            methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                            await ReplyAsync($"Compte de {argus[1]} créé");
                            Console.WriteLine($"Compte de {argus[1]} créé");
                            Console.WriteLine(await methodes.AccountsListAsync(nomFichier));
                        }
                        else if (await methodes.GetStuffAccountByNameAsync(nomFichier, argus[1]) != null)
                        {
                            await ReplyAsync($"{error} Le compte \"**{argus[1]}**\" existe déjà");
                            Console.WriteLine($"{error} Le compte \"**{argus[1]}**\" existe déjà");
                        }
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
                                Console.WriteLine($"{error} Nombre max d'arguments dépassé");
                            }
                            else if (argus.Length < 2) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 2 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                                Console.WriteLine($"{error} Nombre insuffisant d'arguments");
                            }
                            else if (await methodes.GetStuffAccountIndexByNameAsync(nomFichier, argus[1]) != -1)
                            {
                                int toRemIndex = await methodes.GetStuffAccountIndexByNameAsync(nomFichier, argus[1]);
                                stuffAccounts.RemoveAt(toRemIndex);
                                methodes.EnregistrerDonneesPersos(nomFichier, stuffAccounts);
                                await ReplyAsync($"Compte de {argus[1]} supprimé");
                                Console.WriteLine($"Compte de {argus[1]} supprimé");
                                Console.WriteLine(await methodes.AccountsListAsync(nomFichier));
                            }
                            else if (await methodes.GetStuffAccountIndexByNameAsync(nomFichier, argus[1]) == -1)
                            {
                                await ReplyAsync($"{error} Compte \"**{argus[1]}**\"  inexistant: stuff create (nom_Personnage) pour créer un nouveau compte");
                                Console.WriteLine($"{error} Compte \"**{argus[1]}**\"  inexistant: stuff create (nom_Personnage) pour créer un nouveau compte");
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
                        Console.WriteLine(e);
                        throw;
                    }
                }
                else
                {
                    await ReplyAsync(error);
                }
            }
            else if (input == "none")
            {
                try
                {
                    await ReplyAsync(error);
                    Console.WriteLine(error);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return;
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

        #endregion
    }
}