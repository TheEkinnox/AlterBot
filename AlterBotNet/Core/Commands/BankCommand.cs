#region MÉTADONNÉES

// Nom du fichier : BankCommand.cs
// Auteur : Loick OBIANG (1832960)
// Date de création : 2019-02-07
// Date de modification : 2019-02-13

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
    /// Classe des commandes bank
    /// </summary>
    public class BankCommand : ModuleBase<SocketCommandContext>
    {
        #region ATTRIBUTS

        private Random _rand = new Random();

        #endregion

        #region MÉTHODES

        [Command("bank"), Alias("bnk", "money", "bk"), Summary("Affiche l'argent en banque d'un utilisateur")]
        public async Task SendBank([Remainder] string input = "none")
        {
            var mentionedUser = this.Context.Message.MentionedUsers.FirstOrDefault();
            BankAccount methodes = new BankAccount("");
            string[] argus;
            ulong userId = this.Context.User.Id;
            string error = "Valeur invalide, bank help pour plus d'information.";
            string message = "";
            string nomFichier = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1\AlterBotNet.dll", @"Ressources\Database\bank.altr");

            //string nomFichier = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\testBank.exe", @"Ressources\Database\bank.altr");
            List<BankAccount> bankAccounts = await methodes.ChargerDonneesPersosAsync(nomFichier);

            if (input != "none")
            {
                // ====================================
                // = Gestion de la commande bank help =
                // ====================================
                if (input == "help")
                {
                    message += "Aide sur la commande: `bank help`\n";
                    message += "Afficher la liste des comptes: `bank list`\n";
                    message += "Afficher le montant sur le compte d'un personnage: `bank info (nom_Personnage)`\n";
                    message += "(staff) Ajouter de l'argent sur le compte d'un personnage: `bank deposit (montant) (nom_Personnage)`\n";
                    message += "(staff) Retirer de l'argent sur le compte d'un personnage: `bank withdraw (montant) (nom_Personnage)`\n";
                    message += "(staff) Définir le montant sur le compte d'un personnage: `bank set (montant) (nom_Personnage)`\n";
                    message += "Transférer de l'argent d'un compte à un autre: `bank pay (montant) (nom_Personnage1) (nom_Personnage2)`\n";
                    message += "(stuff) (RP) Créer un nouveau compte: `bank add (nomPersonnage) {@propriétaire}`\n";
                    message += "(staff) Supprimer un compte: `bank delete (nomPersonnage)`\n";
                    message += "Trier la liste des comptes (par ordre alphabétique): `bank sort`\n";
                    message += "(staff) Définir le salaire d'un personnage: `bank sts (nom_personnage)`\n";
                    message += "(staff) Définir le propriétaire d'un personnage: `bank setowner (nom_personnage) (@propriétaire)`\n";
                    message += "(staff) Changer le nom d'un personnage: `bank rename (nom_personnage) (nouveauNom)`\n";
                    message += "(staff) Verser son salaire à un personnage: `bank givesal (nom_personnage)`\n";
                    try
                    {
                        await ReplyAsync("Aide envoyée en mp");
                        Logs.WriteLine($"message envoyé en mp à {this.Context.User.Username}");
                        EmbedBuilder eb = new EmbedBuilder();
                        eb.WithTitle("**Aide de la commande bank (bnk,money)**")
                            .WithColor(this._rand.Next(256), this._rand.Next(256), this._rand.Next(256))
                            .AddField("===================================", message);
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
                // ====================================
                // = Gestion de la commande bank list =
                // ====================================
                else if (input == "list")
                {
                    try
                    {
                        Logs.WriteLine((await methodes.AccountsListAsync(nomFichier)).Count.ToString());
                        foreach (string msg in await methodes.AccountsListAsync(nomFichier))
                        {
                            if (!string.IsNullOrEmpty(msg))
                            {
                                await ReplyAsync(msg);
                                Logs.WriteLine($"Liste envoyée sur le channel {this.Context.Channel.Name}");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Logs.WriteLine(e.ToString());
                        throw;
                    }
                }
                // =====================================================
                // = Gestion de la commande bank info (nom_Personnage) =
                // =====================================================
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
                            BankAccount infoAccount = await methodes.GetBankAccountByNameAsync(nomFichier, argus[1]);
                            if (infoAccount != null)
                            {
                                await ReplyAsync(infoAccount.ToString());
                                Logs.WriteLine(infoAccount.ToString());
                            }
                            else
                            {
                                await ReplyAsync($"{error} Compte \"**{argus[1]}**\" inexistant: bank add (nom_Personnage) pour créer un nouveau compte");
                                Logs.WriteLine($"{error} Compte \"**{argus[1]}**\" inexistant: bank add (nom_Personnage) pour créer un nouveau compte");
                            }
                        }
                    }
                }
                // ==========================================================================
                // = Gestion de la commande (admin) bank deposit (montant) (nom_Personnage) =
                // ==========================================================================
                else if (input.StartsWith("deposit") || input.StartsWith("dp"))
                {
                    if (IsStaff((SocketGuildUser) this.Context.User))
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours deposit
                        if (argus[0] == "deposit" || argus[0] == "dp")
                        {
                            if (argus.Length > 3 && !decimal.TryParse(argus[1], out decimal montant)) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
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
                                decimal.TryParse(argus[1], out montant);
                                BankAccount depositAccount = await methodes.GetBankAccountByNameAsync(nomFichier, argus[2]);
                                if (depositAccount != null)
                                {
                                    string dpName = depositAccount.Name;
                                    decimal dpSalaire = depositAccount.Salaire;
                                    ulong dpUserId = depositAccount.UserId;
                                    decimal ancienMontant = depositAccount.Amount;
                                    decimal nvMontant = ancienMontant + montant;
                                    bankAccounts.RemoveAt(await methodes.GetBankAccountIndexByNameAsync(nomFichier, dpName));
                                    methodes.EnregistrerDonneesPersos(nomFichier, bankAccounts);
                                    BankAccount newAccount = new BankAccount(dpName, nvMontant, dpUserId, dpSalaire);
                                    bankAccounts.Add(newAccount);
                                    methodes.EnregistrerDonneesPersos(nomFichier, bankAccounts);
                                    await ReplyAsync($"{montant} couronnes ajoutée(s) sur le compte de {dpName}");
                                    Logs.WriteLine($"{montant} couronnes ajoutée(s) sur le compte de {dpName}");
                                    Logs.WriteLine(newAccount.ToString());
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
                // ===========================================================================
                // = Gestion de la commande (admin) bank withdraw (montant) (nom_Personnage) =
                // ===========================================================================
                else if (input.StartsWith("withdraw") || input.StartsWith("wd"))
                {
                    if (IsStaff((SocketGuildUser) this.Context.User))
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours withdraw
                        if (argus[0] == "withdraw" || argus[0] == "wd")
                        {
                            if (argus.Length > 3 && !decimal.TryParse(argus[1], out decimal montant)) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
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
                                decimal.TryParse(argus[1], out montant);
                                BankAccount withdrawAccount = await methodes.GetBankAccountByNameAsync(nomFichier, argus[2]);
                                if (withdrawAccount != null)
                                {
                                    string wdName = withdrawAccount.Name;
                                    decimal wdSalaire = withdrawAccount.Salaire;
                                    ulong wdUserId = withdrawAccount.UserId;
                                    decimal ancienMontant = withdrawAccount.Amount;
                                    decimal nvMontant = ancienMontant - montant;
                                    nvMontant = nvMontant < 0 ? 0 : nvMontant;
                                    bankAccounts.RemoveAt(await methodes.GetBankAccountIndexByNameAsync(nomFichier, wdName));
                                    methodes.EnregistrerDonneesPersos(nomFichier, bankAccounts);
                                    BankAccount newAccount = new BankAccount(wdName, nvMontant, wdUserId, wdSalaire);
                                    bankAccounts.Add(newAccount);
                                    methodes.EnregistrerDonneesPersos(nomFichier, bankAccounts);
                                    await ReplyAsync($"{montant} couronnes retirée(s) sur le compte de {wdName}");
                                    Logs.WriteLine($"{montant} couronnes retirée(s) sur le compte de {wdName}");
                                    Logs.WriteLine(newAccount.ToString());
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
                // ======================================================================
                // = Gestion de la commande (admin) bank set (montant) (nom_Personnage) =
                // ======================================================================
                else if (input.StartsWith("set"))
                {
                    if (IsStaff((SocketGuildUser) this.Context.User))
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours set
                        if (argus[0] == "set")
                        {
                            if (argus.Length > 3 && !decimal.TryParse(argus[1], out decimal montant)) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
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
                                decimal.TryParse(argus[1], out montant);
                                BankAccount setAccount = await methodes.GetBankAccountByNameAsync(nomFichier, argus[2]);
                                if (setAccount != null)
                                {
                                    string setName = setAccount.Name;
                                    decimal nvMontant = montant;
                                    ulong setuserId = setAccount.UserId;
                                    decimal setSalaire = setAccount.Salaire;
                                    nvMontant = nvMontant < 0 ? 0 : nvMontant;
                                    bankAccounts.RemoveAt(await methodes.GetBankAccountIndexByNameAsync(nomFichier, setName));
                                    methodes.EnregistrerDonneesPersos(nomFichier, bankAccounts);
                                    BankAccount newAccount = new BankAccount(setName, nvMontant, setuserId, setSalaire);
                                    bankAccounts.Add(newAccount);
                                    methodes.EnregistrerDonneesPersos(nomFichier, bankAccounts);
                                    await ReplyAsync($"Le montant sur le compte de \"**{setName}**\" est désormais de \"**{setName}**\" couronnes");
                                    Logs.WriteLine($"Le montant sur le compte de {setName} est désormais de \"**{nvMontant}**\" couronnes");
                                    Logs.WriteLine(newAccount.ToString());
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
                // =================================================================================
                // = Gestion de la commande bank pay (montant) (nom_Personnage1) (nom_Personnage2) =
                // =================================================================================
                else if (input.StartsWith("pay") || input.StartsWith("transfer") || input.StartsWith("tr"))
                {
                    argus = input.Split(' ');
                    // Sert à s'assurer qu'argus[0] == toujours withdraw
                    if (argus[0] == "pay" || argus[0] == "transfer" || argus[0] == "tr")
                    {
                        if (argus.Length > 4 && !decimal.TryParse(argus[1], out decimal montant)) // Sert à s'assurer que argus[1] == forcément montant (et qu'il n'y a que 4 paramètres)
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
                            decimal.TryParse(argus[1], out montant);
                            BankAccount withdrawAccount = await methodes.GetBankAccountByNameAsync(nomFichier, argus[2]);
                            BankAccount depositAccount = await methodes.GetBankAccountByNameAsync(nomFichier, argus[3]);
                            if (withdrawAccount != null && depositAccount != null)
                            {
                                //Retire l'argent du premier compte
                                string wdName = withdrawAccount.Name;
                                decimal wdSalaire = withdrawAccount.Salaire;
                                ulong wdUserId = withdrawAccount.UserId;
                                decimal ancienMontant = withdrawAccount.Amount;
                                decimal nvMontant = ancienMontant - montant;
                                nvMontant = nvMontant < 0 ? 0 : nvMontant;
                                bankAccounts.RemoveAt(await methodes.GetBankAccountIndexByNameAsync(nomFichier, wdName));
                                methodes.EnregistrerDonneesPersos(nomFichier, bankAccounts);
                                BankAccount newAccount = new BankAccount(wdName, nvMontant, wdUserId, wdSalaire);
                                bankAccounts.Add(newAccount);
                                methodes.EnregistrerDonneesPersos(nomFichier, bankAccounts);
                                Logs.WriteLine($"{ancienMontant - nvMontant} couronnes retirée(s) sur le compte de {wdName}");
                                Logs.WriteLine(newAccount.ToString());
                                // Ajoute l'argent sur le 2eme compte
                                string dpName = depositAccount.Name;
                                decimal dpSalaire = depositAccount.Salaire;
                                ulong dpUserId = depositAccount.UserId;
                                decimal montantTr = nvMontant == 0 ? ancienMontant : montant;
                                decimal depositAccountAmount = depositAccount.Amount;
                                decimal depositAccountNewAmount = depositAccountAmount + montantTr;
                                bankAccounts.RemoveAt(await methodes.GetBankAccountIndexByNameAsync(nomFichier, dpName));
                                methodes.EnregistrerDonneesPersos(nomFichier, bankAccounts);
                                BankAccount newAccountDeposit = new BankAccount(dpName, depositAccountNewAmount, dpUserId, dpSalaire);
                                bankAccounts.Add(newAccountDeposit);
                                methodes.EnregistrerDonneesPersos(nomFichier, bankAccounts);
                                Logs.WriteLine($"{montantTr} couronnes ajoutée(s) sur le compte de {dpName}");
                                Logs.WriteLine(newAccountDeposit.ToString());
                                await ReplyAsync($"{montantTr} couronnes transférées du compte de {wdName} vers le compte de {dpName}");
                            }
                            else
                            {
                                await ReplyAsync($"{error} Comptes \"**{argus[2]}**\" et/ou \"**{argus[3]}**\" inexistants: bank add (nom_Personnage) pour créer un nouveau compte");
                                Logs.WriteLine($"{error} Comptes \"**{argus[2]}**\" et/ou \"**{argus[3]}**\" inexistants: bank add (nom_Personnage) pour créer un nouveau compte");
                            }
                        }
                    }
                }
                // ===================================================
                // = Gestion de la commande bank add (nomPersonnage) =
                // ===================================================
                else if (input.StartsWith("add"))
                {
                    if (HasRole((SocketGuildUser) this.Context.User, "RP") || IsStaff((SocketGuildUser)this.Context.User))
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours add
                        if (argus[0] == "add")
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
                            else if (await methodes.GetBankAccountByNameAsync(nomFichier, argus[1]) == null)
                            {
                                BankAccount newAccount;
                                if (mentionedUser != null)
                                {
                                    ulong soUserId = mentionedUser.Id;
                                    newAccount = new BankAccount(argus[1], 500, soUserId);
                                }
                                else
                                {
                                    newAccount = new BankAccount(argus[1], 500, userId);
                                }

                                bankAccounts.Add(newAccount);
                                methodes.EnregistrerDonneesPersos(nomFichier, bankAccounts);
                                await ReplyAsync($"Compte de {argus[1]} créé");
                                Logs.WriteLine($"Compte de {argus[1]} créé");
                                Logs.WriteLine((await methodes.AccountsListAsync(nomFichier)).ToString());
                            }
                            else if (await methodes.GetBankAccountByNameAsync(nomFichier, argus[1]) != null)
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
                            await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(420536907525652482).Mention} ou avoir le rôle {this.Context.Guild.GetRole(409787899761000449).Mention} pour utiliser cette commande");
                    }
                }
                // ======================================================
                // = Gestion de la commande bank delete (nomPersonnage) =
                // ======================================================
                else if (input.StartsWith("delete") || input.StartsWith("del"))
                {
                    argus = input.Split(' ');
                    if (IsStaff((SocketGuildUser) this.Context.User) || userId == (await methodes.GetBankAccountByNameAsync(nomFichier, argus[1])).UserId)
                    {
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
                            else if (await methodes.GetBankAccountIndexByNameAsync(nomFichier, argus[1]) != -1)
                            {
                                int toRemIndex = await methodes.GetBankAccountIndexByNameAsync(nomFichier, argus[1]);
                                bankAccounts.RemoveAt(toRemIndex);
                                methodes.EnregistrerDonneesPersos(nomFichier, bankAccounts);
                                await ReplyAsync($"Compte de {argus[1]} supprimé");
                                Logs.WriteLine($"Compte de {argus[1]} supprimé");
                                Logs.WriteLine((await methodes.AccountsListAsync(nomFichier)).ToString());
                            }
                            else if (await methodes.GetBankAccountIndexByNameAsync(nomFichier, argus[1]) == -1)
                            {
                                await ReplyAsync($"{error} Compte \"**{argus[1]}**\"  inexistant: bank add (nom_Personnage) pour créer un nouveau compte");
                                Logs.WriteLine($"{error} Compte \"**{argus[1]}**\"  inexistant: bank add (nom_Personnage) pour créer un nouveau compte");
                            }
                        }
                    }
                    else
                    {
                        if (this.Context.Guild.Name == "ServeurTest")
                            await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(541492279894999080).Mention} ou le propriétaire du compte pour utiliser cette commande");
                        else
                            await ReplyAsync($"Vous devez être membre du {this.Context.Guild.GetRole(420536907525652482).Mention} ou le propriétaire du compte pour utiliser cette commande");
                    }
                }
                // ====================================
                // = Gestion de la commande bank sort =
                // ====================================
                else if (input == "sort")
                {
                    try
                    {
                        List<BankAccount> sortedList = bankAccounts.OrderBy(o => o.Name).ToList();
                        methodes.EnregistrerDonneesPersos(nomFichier, sortedList);
                        await ReplyAsync("La liste des comptes en banque a été triée par ordre alphabétique");
                    }
                    catch (Exception e)
                    {
                        Logs.WriteLine(e.ToString());
                        throw;
                    }
                }
                // =========================================================================
                // = Gestion de la commande (admin) bank setsal (montant) (nom_Personnage) =
                // =========================================================================
                else if (input.StartsWith("sts"))
                {
                    if (IsStaff((SocketGuildUser) this.Context.User))
                    {
                        try
                        {
                            argus = input.Split(' ');
                            // Sert à s'assurer qu'argus[0] == toujours setsal
                            if (argus[0] == "sts")
                            {
                                try
                                {
                                    if (argus.Length > 3 && !decimal.TryParse(argus[1], out decimal montant)) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
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
                                        decimal.TryParse(argus[1], out montant);
                                        BankAccount setAccount = await methodes.GetBankAccountByNameAsync(nomFichier, argus[2]);
                                        if (setAccount != null)
                                        {
                                            string stsName = setAccount.Name;
                                            decimal stsMontant = setAccount.Amount;
                                            ulong stsUserId = setAccount.UserId;
                                            decimal nvMontant = montant;
                                            nvMontant = nvMontant < 0 ? 0 : nvMontant;
                                            bankAccounts.RemoveAt(await methodes.GetBankAccountIndexByNameAsync(nomFichier, argus[2]));
                                            methodes.EnregistrerDonneesPersos(nomFichier, bankAccounts);
                                            BankAccount newAccount = new BankAccount(stsName, stsMontant, stsUserId, nvMontant);
                                            bankAccounts.Add(newAccount);
                                            methodes.EnregistrerDonneesPersos(nomFichier, bankAccounts);
                                            await ReplyAsync($"Le salaire de {stsName} est désormais de {nvMontant} couronnes");
                                            Logs.WriteLine($"Le salaire de {stsName} est désormais de {nvMontant} couronnes");
                                            Logs.WriteLine(newAccount.ToString());
                                        }
                                        else
                                        {
                                            await ReplyAsync($"{error} Compte \"**{argus[2]}**\" inexistant: bank add (nom_Personnage) pour créer un nouveau compte");
                                            Logs.WriteLine($"{error} Compte \"**{argus[2]}**\" inexistant: bank add (nom_Personnage) pour créer un nouveau compte");
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Logs.WriteLine(e.ToString());
                                    throw;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logs.WriteLine(e.ToString());
                            throw;
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
                // ==============================================================================
                // = Gestion de la commande (admin) bank setowner (nom_Personnage) @utilisateur =
                // ==============================================================================
                else if (input.StartsWith("setowner") || input.StartsWith("setown") || input.StartsWith("so"))
                {
                    if (IsStaff((SocketGuildUser) this.Context.User))
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
                                BankAccount setAccount = await methodes.GetBankAccountByNameAsync(nomFichier, argus[1]);
                                if (setAccount != null)
                                {
                                    if (mentionedUser != null)
                                    {
                                        try
                                        {
                                            string soName = setAccount.Name;
                                            decimal soMontant = setAccount.Amount;
                                            decimal soSalaire = setAccount.Salaire;
                                            ulong soUserId = mentionedUser.Id;

                                            bankAccounts.RemoveAt(await methodes.GetBankAccountIndexByNameAsync(nomFichier, soName));
                                            methodes.EnregistrerDonneesPersos(nomFichier, bankAccounts);
                                            BankAccount newAccount = new BankAccount(soName, soMontant, soUserId, soSalaire);
                                            bankAccounts.Add(newAccount);
                                            methodes.EnregistrerDonneesPersos(nomFichier, bankAccounts);
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
                // ============================================================================
                // = Gestion de la commande (admin) bank rename (nom_Personnage) (nouveauNom) =
                // ============================================================================
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
                                BankAccount setAccount = await methodes.GetBankAccountByNameAsync(nomFichier, argus[1]);
                                if (setAccount != null && await methodes.GetBankAccountByNameAsync(nomFichier, argus[2]) == null)
                                {
                                    try
                                    {
                                        string rnName = setAccount.Name;
                                        string newRnName = argus[2];
                                        decimal rnMontant = setAccount.Amount;
                                        decimal rnSalaire = setAccount.Salaire;
                                        ulong rnUserId = setAccount.UserId;

                                        bankAccounts.RemoveAt(await methodes.GetBankAccountIndexByNameAsync(nomFichier, rnName));
                                        methodes.EnregistrerDonneesPersos(nomFichier, bankAccounts);
                                        BankAccount newAccount = new BankAccount(newRnName, rnMontant, rnUserId, rnSalaire);
                                        bankAccounts.Add(newAccount);
                                        methodes.EnregistrerDonneesPersos(nomFichier, bankAccounts);
                                        await ReplyAsync($"Le nom du compte de \"**{rnName}**\" est désormais \"**{newRnName}**\"");
                                        Logs.WriteLine($"Le nom du compte de \"**{rnName}**\" est désormais \"**{newRnName}**\"");
                                        Logs.WriteLine(newAccount.ToString());
                                    }
                                    catch (Exception e)
                                    {
                                        Logs.WriteLine(e.ToString());
                                        throw;
                                    }
                                }
                                else if ((await methodes.GetBankAccountByNameAsync(nomFichier, argus[2]) != null))
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
                // ================================================================
                // = Gestion de la commande (admin) bank givesal (nom_Personnage) =
                // ================================================================
                else if (input.StartsWith("givesal") || input.StartsWith("gs"))
                {
                    if (IsStaff((SocketGuildUser)this.Context.User))
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours deposit
                        if (argus[0] == "givesal" || argus[0] == "gs")
                        {
                            if (argus.Length > 2) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre max d'arguments dépassé");
                                Logs.WriteLine($"{error} Nombre max d'arguments dépassé");
                            }
                            else if (argus.Length < 2) // Sert à s'assurer que argus[1] == forcément nomPerso (et qu'il n'y a que 3 paramètres)
                            {
                                await ReplyAsync($"{error} Nombre insuffisant d'arguments");
                                Logs.WriteLine($"{error} Nombre insuffisant d'arguments");
                            }
                            else
                            {
                                BankAccount depositAccount = await methodes.GetBankAccountByNameAsync(nomFichier, argus[1]);
                                if (depositAccount != null)
                                {
                                    string dpName = depositAccount.Name;
                                    decimal dpSalaire = depositAccount.Salaire;
                                    await ReplyAsync($"Salaire de {dpSalaire} couronnes versé sur le compte de {dpName}");
                                    await Program.VerserSalaireAsync(depositAccount);
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
                else
                {
                    await ReplyAsync(error);
                }
            }
            else if (input == "none")
            {
                await ReplyAsync(error);
                Logs.WriteLine(error);
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
        /// Vérifie si l'utilisateur possède le role indiqué ou non
        /// </summary>
        /// <param name="user">Utilisateur à vérifier</param>
        /// <param name="roleName">Nom du role à vérifier</param>
        /// <returns>True si l'utilisateur est membre du role indiqué ou false sinon</returns>
        private bool HasRole(SocketGuildUser user, string roleName)
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