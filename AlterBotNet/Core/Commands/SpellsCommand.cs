using AlterBotNet.Core.Data.Classes;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace AlterBotNet.Core.Commands
{
    public class SpellsCommand : ModuleBase<SocketCommandContext>
    {
        private Random _rand = new Random();
        #region MÉTHODES

        [Command("spell"), Alias("spl"), Summary("Affiche le contenu du grimoire d'un personnage")]
        public async Task SendSpell([Remainder]string input = "none")
        {
            SocketUser mentionedUser = this.Context.Message.MentionedUsers.FirstOrDefault();
            string[] argus;
            ulong userId = this.Context.User.Id;
            string error = "Valeur invalide, spell help pour plus d'information.";
            string message = "";
            string nomFichierXml = Global.CheminComptesSpellXml;

            List<SpellAccount> spellAccounts = await Global.ChargerDonneesSpellXmlAsync(nomFichierXml);

            if (input != "none")
            {
                // ====================================
                // = Gestion de la commande spell help =
                // ====================================
                if (input == "help")
                {
                    string staff = "";
                    message += "Aide sur la commande: `spell help`\n";
                    staff += "(staff) Afficher la liste des comptes: `spell list`\n";
                    staff += "(staff) Afficher la liste des comptes: `spell update`\n";
                    message += "Afficher les sortilège/enchantements d'un personnage: `spell info (nom_Personnage)`\n";
                    staff += "(staff) Ajouter un sortilège/enchantement à un personnage: `spell add (sortilège/enchantement) (nom_Personnage)`\n";
                    staff += "(staff) Retirer un sortilège/enchantement à un personnage: `spell remove (sortilège/enchantement) (nom_Personnage)`\n";
                    message += "Transférer un sortilège/enchantement d'un compte à un autre: `spell learn (sortilège/enchantement) (nom_Personnage1) (nom_Personnage2)`\n";
                    staff += "(staff) (RP) Créer un nouveau compte: `spell create (nomPersonnage)`\n";
                    staff += "(staff) Supprimer un compte: `spell delete (nomPersonnage)`\n";
                    message += "Trier la liste des comptes (par ordre alphabétique): `spell sort`\n";
                    staff += "(staff) Définir le propriétaire d'un personnage: `spell setowner (nom_personnage) (@propriétaire)`\n";
                    staff += "(staff) Changer le nom d'un personnage: `spell rename (nom_personnage) (nouveauNom)`\n";
                    staff += "(staff) Remplacer un sortilège/enchantement dans l'inventaire d'un personnage: `spell replace (nom_personnage) (sortilège/enchantement) (nouveau sortilège/enchantement)`\n";
                    try
                    {
                        await ReplyAsync("Aide envoyée en mp");
                        Logs.WriteLine($"message envoyé en mp à {this.Context.User.Username}");
                        EmbedBuilder eb = new EmbedBuilder();
                        eb.WithTitle(("**Aide de la commande spell (stf,inv)**"))
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
                // = Gestion de la commande (staff) spell list =
                // =============================================
                else if (input == "list")
                {
                    if (Global.IsStaff((SocketGuildUser)this.Context.User))
                    {
                        if (string.IsNullOrEmpty((await Global.XmlSpellAccountsListAsync(nomFichierXml)).ToString()))
                        {
                            await ReplyAsync("Liste vide");
                            Logs.WriteLine("Liste vide");
                        }
                        else
                        {
                            try
                            {
                                foreach (SpellAccount spell in Global.ChargerDonneesSpellXml(nomFichierXml))
                                {
                                    Logs.WriteLine(spell.ToString());
                                }
                                Logs.WriteLine((await Global.XmlSpellAccountsListAsync(nomFichierXml)).Count.ToString());
                                foreach (string msg in await Global.XmlSpellAccountsListAsync(nomFichierXml))
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
                // ======================================
                // = Gestion de la commande spell update =
                // ======================================
                else if (input.StartsWith("update") || input.StartsWith("up"))
                {
                    argus = input.Split(' ');
                    // Sert à s'assurer qu'argus[0] == toujours update
                    if (argus[0] == "update" || argus[0] == "up")
                    {
                        await Global.UpdateSpellXml();
                        Logs.WriteLine("Actualisation réussie");
                    }
                }
                // ======================================================
                // = Gestion de la commande spell info (nom_Personnage) =
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
                            SpellAccount infoAccount = await Global.GetXmlSpellAccountByNameAsync(nomFichierXml, argus[1]);
                            if (infoAccount != null && (infoAccount.UserId == userId || Global.IsStaff((SocketGuildUser)this.Context.User)))
                            {
                                try
                                {
                                    await ReplyAsync("Infos envoyées en mp");
                                    Logs.WriteLine($"Infos du compte (spell) de {infoAccount.Name} envoyé en mp à {this.Context.User.Username}");
                                    EmbedBuilder eb = new EmbedBuilder();
                                    eb.WithTitle(($"Inventaire de **{infoAccount.Name}**"))
                                        .WithColor(this._rand.Next(256), this._rand.Next(256), this._rand.Next(256))
                                        .AddField("==============================================", infoAccount.TextForm());
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
                                    await ReplyAsync($"{error} Compte \"**{argus[1]}**\" inexistant: spell create (nom_Personnage) pour créer un nouveau compte");
                                    Logs.WriteLine($"{error} Compte \"**{argus[1]}**\" inexistant: spell create (nom_Personnage) pour créer un nouveau compte");
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
                // =================================================================================
                // = Gestion de la commande (staff) spell add (sort/enchantement) (nom_Personnage) =
                // =================================================================================
                else if (input.StartsWith("add"))
                {
                    if (Global.IsStaff((SocketGuildUser)this.Context.User))
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
                                SpellAccount depositAccount = await Global.GetXmlSpellAccountByNameAsync(nomFichierXml, argus[2]);
                                if (argus[1].Contains('_'))
                                    argus[1] = argus[1].Replace("_", " ");
                                if (argus[1].Contains('-'))
                                    argus[1] = argus[1].Replace("-", " ");
                                if (depositAccount != null && !depositAccount.PossedeSpell(argus[1]))
                                {
                                    string dpName = depositAccount.Name;
                                    List<Spell> dpSpell = depositAccount.Spells;
                                    ulong dpUserId = depositAccount.UserId;
                                    try
                                    {
                                        Spell toAdd = PublicGrim.FindSpell(argus[1]);
                                        if (toAdd != null)
                                        {
                                            dpSpell.Add(toAdd);
                                            spellAccounts.RemoveAt(await Global.GetXmlSpellAccountIndexByNameAsync(nomFichierXml, argus[2]));
                                            SpellAccount newAccount = new SpellAccount(dpName, dpSpell, dpUserId);
                                            spellAccounts.Add(newAccount);
                                            Global.EnregistrerDonneesSpellXml(nomFichierXml, spellAccounts);
                                            await ReplyAsync($"{PublicGrim.FindSpell(argus[1]).Type} \"**{toAdd.SpellName}**\" ajouté sur le compte de \"**{dpName}**\"");
                                            Logs.WriteLine($"{PublicGrim.FindSpell(argus[1]).Type} \"**{toAdd.SpellName}**\" ajouté sur le compte de \"**{dpName}**\" par {this.Context.User.Username}");
                                            Logs.WriteLine(newAccount.ToString());
                                        }
                                        else
                                        {
                                            Logs.WriteLine($"{this.Context.User.Username} a tenté d'ajouter le sortilège/enchantement {argus[1]} qui n'est pas dans le grimoire comun.");
                                            await ReplyAsync($"Le sortilège/enchantement \"**{argus[1]}**\" n'est pas dans le grimoire. Vérifiez l'orthographe et réessayez.");
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Logs.WriteLine(e.ToString());
                                        throw;
                                    }
                                }
                                else if (depositAccount == null)
                                {
                                    await ReplyAsync($"{error} Compte \"**{argus[2]}**\" inexistant: spell create (nom_Personnage) pour créer un nouveau compte");
                                    Logs.WriteLine($"{error} Compte \"**{argus[2]}**\" inexistant: spell create (nom_Personnage) pour créer un nouveau compte");
                                }
                                else if (depositAccount.PossedeSpell(argus[1]))
                                {
                                    await ReplyAsync($"\"**{argus[2]}**\" possède déjà le sortilège/enchantement {argus[1]}");
                                    Logs.WriteLine($"\"**{argus[2]}**\" possède déjà le sortilège/enchantement {argus[1]}");
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
                // =========================================================================================
                // = Gestion de la commande (admin) spell remove (sortilège/enchantement) (nom_Personnage) =
                // =========================================================================================
                else if (input.StartsWith("remove") || input.StartsWith("rm"))
                {
                    if (Global.IsStaff((SocketGuildUser)this.Context.User))
                    {
                        argus = input.Split(' ');
                        // Sert à s'assurer qu'argus[0] == toujours remove
                        if (argus[0] == "remove" || argus[0] == "rm")
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
                                SpellAccount withdrawAccount = await Global.GetXmlSpellAccountByNameAsync(nomFichierXml, argus[2]);
                                if (withdrawAccount != null)// Sert à s'assurer que le compte existe bien
                                {
                                    string wdName = withdrawAccount.Name;
                                    List<Spell> wdSpells = withdrawAccount.Spells;
                                    ulong wdUserId = withdrawAccount.UserId;
                                    if (argus[1].Contains('_'))
                                        argus[1] = argus[1].Replace("_", " ");
                                    if (argus[1].Contains('-'))
                                        argus[1] = argus[1].Replace("-", " ");
                                    if (int.TryParse(argus[1], out int indexObj))
                                    {
                                        if (wdSpells[indexObj] != null)
                                        {
                                            try
                                            {
                                                string nomSpell = wdSpells[indexObj].SpellName;
                                                wdSpells.RemoveAt(indexObj);
                                                spellAccounts.RemoveAt(await Global.GetXmlSpellAccountIndexByNameAsync(nomFichierXml, argus[2]));
                                                Global.EnregistrerDonneesSpellXml(nomFichierXml, spellAccounts);
                                                SpellAccount newAccount = new SpellAccount(wdName, wdSpells, wdUserId);
                                                spellAccounts.Add(newAccount);
                                                Global.EnregistrerDonneesSpellXml(nomFichierXml, spellAccounts);
                                                await ReplyAsync($"{PublicGrim.FindSpell(argus[1]).Type} \"**{nomSpell}**\" retiré du compte de \"**{wdName}**\"");
                                                Logs.WriteLine($"{PublicGrim.FindSpell(argus[1]).Type} \"**{nomSpell}**\" retiré du compte de \"**{wdName}**\"");
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
                                                await ReplyAsync($"{error} \"**{wdName}**\" ne possède pas de sortilège/enchantement à l'index \"**{argus[1]}**\"");
                                                Logs.WriteLine($"{error} \"**{wdName}**\" ne possède pas de sortilège/enchantement à l'index \"**{argus[1]}**\"");
                                            }
                                            catch (Exception e)
                                            {
                                                Logs.WriteLine(e.ToString());
                                                throw;
                                            }
                                        }
                                    }
                                    else if (withdrawAccount.PossedeSpell(argus[1]))
                                    {
                                        try
                                        {
                                            wdSpells.RemoveAt(Global.FindXmlSpellIndex(argus[1]));
                                            spellAccounts.RemoveAt(await Global.GetXmlSpellAccountIndexByNameAsync(nomFichierXml, argus[2]));
                                            Global.EnregistrerDonneesSpellXml(nomFichierXml, spellAccounts);
                                            SpellAccount newAccount = new SpellAccount(wdName, wdSpells, wdUserId);
                                            spellAccounts.Add(newAccount);
                                            Global.EnregistrerDonneesSpellXml(nomFichierXml, spellAccounts);
                                            await ReplyAsync($"{PublicGrim.FindSpell(argus[1]).Type} \"**{argus[1]}**\" retiré du compte de \"**{wdName}**\"");
                                            Logs.WriteLine($"{PublicGrim.FindSpell(argus[1]).Type} \"**{argus[1]}**\" retiré du compte de \"**{wdName}**\"");
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
                                        await ReplyAsync($"{error} \"**{wdName}**\" ne possède pas le sortilège/enchantement \"**{argus[1]}**\"");
                                        Logs.WriteLine($"{error} \"**{wdName}**\" ne possède pas le sortilège/enchantement \"**{argus[1]}**\"");
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
                // = Gestion de la commande spell learn (sortilège/enchantement) (nom_Personnage1) (nom_Personnage2) =
                // ===================================================================================
                else if (input.StartsWith("learn") || input.StartsWith("transfer") || input.StartsWith("tr"))
                {
                    argus = input.Split(' ');
                    // Sert à s'assurer qu'argus[0] == toujours withdraw
                    if (argus[0] == "learn" || argus[0] == "transfer" || argus[0] == "tr")
                    {
                        if (argus.Length > 4) // Sert à s'assurer que argus[1] == forcément spell (et qu'il n'y a que 4 paramètres)
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
                            SpellAccount withdrawAccount = await Global.GetXmlSpellAccountByNameAsync(nomFichierXml, argus[2]);
                            SpellAccount depositAccount = await Global.GetXmlSpellAccountByNameAsync(nomFichierXml, argus[3]);
                            if (withdrawAccount != null && depositAccount != null)
                            {
                                // Paramètres pour le compte qui donne
                                string wdName = withdrawAccount.Name;
                                List<Spell> wdSpells = withdrawAccount.Spells;
                                // Paramètres pour le compte qui reçoit
                                string dpName = depositAccount.Name;
                                List<Spell> dpSpells = depositAccount.Spells;
                                ulong dpUserId = depositAccount.UserId;
                                if (argus[1].Contains('_'))
                                    argus[1] = argus[1].Replace("_", " ");
                                if (argus[1].Contains('-'))
                                    argus[1] = argus[1].Replace("-", " ");
                                if (int.TryParse(argus[1], out int indexObj))
                                {
                                    if (!string.IsNullOrEmpty(wdSpells[indexObj].SpellName))
                                    {
                                        Spell spell = wdSpells[indexObj];
                                        dpSpells.Add(spell);
                                        spellAccounts.RemoveAt(await Global.GetXmlSpellAccountIndexByNameAsync(nomFichierXml, dpName));
                                        Global.EnregistrerDonneesSpellXml(nomFichierXml, spellAccounts);
                                        SpellAccount newDpAccount = new SpellAccount(dpName, dpSpells, dpUserId);
                                        spellAccounts.Add(newDpAccount);
                                        Global.EnregistrerDonneesSpellXml(nomFichierXml, spellAccounts);
                                        Logs.WriteLine($"Spell {PublicGrim.FindSpell(argus[1]).Type} \"**{spell.SpellName}**\" ajouté sur le compte de \"**{dpName}**\"");

                                        await ReplyAsync($"{PublicGrim.FindSpell(argus[1]).Type} \"**{spell.SpellName}**\" a été appris par {wdName} à {dpName}");
                                        Logs.WriteLine($"{PublicGrim.FindSpell(argus[1]).Type} \"**{spell.SpellName}**\" a été appris par {wdName} à {dpName}");
                                    }
                                    else
                                    {
                                        await ReplyAsync($"{error} \"**{wdName}**\" ne possède pas de sortilège/enchantement à l'index \"**{argus[1]}**\"");
                                        Logs.WriteLine($"{error} \"**{wdName}**\" ne possède pas de sortilège/enchantement à l'index \"**{argus[1]}**\"");
                                    }
                                }
                                else if (withdrawAccount.PossedeSpell(argus[1]))
                                {
                                    try
                                    {
                                        Spell spell = PublicGrim.FindSpell(argus[1]);
                                        dpSpells.Add(spell);
                                        spellAccounts.RemoveAt(await Global.GetXmlSpellAccountIndexByNameAsync(nomFichierXml, dpName));
                                        Global.EnregistrerDonneesSpellXml(nomFichierXml, spellAccounts);
                                        SpellAccount newDpAccount = new SpellAccount(dpName, dpSpells, dpUserId);
                                        spellAccounts.Add(newDpAccount);
                                        Global.EnregistrerDonneesSpellXml(nomFichierXml, spellAccounts);
                                        Logs.WriteLine($"{PublicGrim.FindSpell(argus[1]).Type} \"**{spell.SpellName}**\" ajouté sur le compte de \"**{dpName}**\"");

                                        await ReplyAsync($"{PublicGrim.FindSpell(argus[1]).Type} \"**{spell.SpellName}**\" a été appris par {wdName} à {dpName}");
                                        Logs.WriteLine($"{PublicGrim.FindSpell(argus[1]).Type} \"**{spell.SpellName}**\" a été appris par {wdName} à {dpName}");
                                    }
                                    catch (Exception e)
                                    {
                                        Logs.WriteLine(e.ToString());
                                        return;
                                    }
                                }
                                else
                                {
                                    if (depositAccount.PossedeSpell(argus[1]))
                                    {
                                        await ReplyAsync($"{error} \"**{dpName}**\" possède déjà le sortilège/enchantement \"**{argus[1]}**\"");
                                        Logs.WriteLine($"{error} \"**{dpName}**\" possède déjà le sortilège/enchantement \"**{argus[1]}**\"");
                                    }
                                    else if (!withdrawAccount.PossedeSpell(argus[1]))
                                    {
                                        await ReplyAsync($"{error} \"**{wdName}**\" ne possède pas le sortilège/enchantement \"**{argus[1]}**\"");
                                        Logs.WriteLine($"{error} \"**{wdName}**\" ne possède pas le sortilège/enchantement \"**{argus[1]}**\"");
                                    }
                                }
                            }
                            else
                            {
                                await ReplyAsync($"{error} Compte(s) \"**{argus[2]}**\" et/ou \"**{argus[3]}**\" inexistant(s): spell create (nom_Personnage) pour créer un nouveau compte");
                                Logs.WriteLine($"{error} Compte(s) \"**{argus[2]}**\" et/ou \"**{argus[3]}**\" inexistant(s): spell create (nom_Personnage) pour créer un nouveau compte");
                            }
                        }
                    }
                }
                // =======================================================
                // = Gestion de la commande spell create (nomPersonnage) =
                // =======================================================
                else if (input.StartsWith("create") || input.StartsWith("cr"))
                {
                    if (Global.HasRole((SocketGuildUser)this.Context.User, "RP") || Global.IsStaff((SocketGuildUser)this.Context.User))
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
                            else if (await Global.GetXmlSpellAccountByNameAsync(nomFichierXml, argus[1]) == null)
                            {
                                SpellAccount newAccount;
                                if (mentionedUser != null)
                                {
                                    ulong crUserId = mentionedUser.Id;
                                    newAccount = new SpellAccount(argus[1], new List<Spell>(), crUserId);
                                }
                                else
                                {
                                    newAccount = new SpellAccount(argus[1], new List<Spell>(), userId);
                                }

                                spellAccounts.Add(newAccount);
                                Global.EnregistrerDonneesSpellXml(nomFichierXml, spellAccounts);
                                await ReplyAsync($"Compte de\"**{argus[1]}**\" créé");
                                Logs.WriteLine($"Compte de \"**{argus[1]}**\" créé");
                            }
                            else if (await Global.GetXmlSpellAccountByNameAsync(nomFichierXml, argus[1]) != null)
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
                // = Gestion de la commande spell delete (nomPersonnage) =
                // =======================================================
                else if (input.StartsWith("delete") || input.StartsWith("del"))
                {
                    argus = input.Split(' ');
                    if (Global.IsStaff((SocketGuildUser)this.Context.User) || userId == (await Global.GetXmlSpellAccountByNameAsync(nomFichierXml, argus[1])).UserId)
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
                            else if (await Global.GetXmlSpellAccountIndexByNameAsync(nomFichierXml, argus[1]) != -1)
                            {
                                int toRemIndex = await Global.GetXmlSpellAccountIndexByNameAsync(nomFichierXml, argus[1]);
                                spellAccounts.RemoveAt(toRemIndex);
                                Global.EnregistrerDonneesSpellXml(nomFichierXml, spellAccounts);
                                await ReplyAsync($"Compte de {argus[1]} supprimé");
                                Logs.WriteLine($"Compte de {argus[1]} supprimé");
                            }
                            else if (await Global.GetXmlSpellAccountIndexByNameAsync(nomFichierXml, argus[1]) == -1)
                            {
                                await ReplyAsync($"{error} Compte \"**{argus[1]}**\"  inexistant: spell create (nom_Personnage) pour créer un nouveau compte");
                                Logs.WriteLine($"{error} Compte \"**{argus[1]}**\"  inexistant: spell create (nom_Personnage) pour créer un nouveau compte");
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
                // = Gestion de la commande spell sort =
                // =====================================
                else if (input == "sort")
                {
                    try
                    {
                        List<SpellAccount> sortedList = spellAccounts.OrderBy(o => o.Name).ToList();
                        Global.EnregistrerDonneesSpellXml(nomFichierXml, sortedList);
                        await ReplyAsync("La liste des comptes a été triée par ordre alphabétique");
                    }
                    catch (Exception e)
                    {
                        Logs.WriteLine(e.ToString());
                        throw;
                    }
                }
                // ===============================================================================
                // = Gestion de la commande (admin) spell setowner (nom_Personnage) @utilisateur =
                // ===============================================================================
                else if (input.StartsWith("setowner") || input.StartsWith("setown") || input.StartsWith("so"))
                {
                    if (Global.IsStaff((SocketGuildUser)this.Context.User))
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
                                SpellAccount setAccount = await Global.GetXmlSpellAccountByNameAsync(nomFichierXml, argus[1]);
                                if (setAccount != null)
                                {
                                    if (mentionedUser != null)
                                    {
                                        try
                                        {
                                            string soName = setAccount.Name;
                                            ulong soUserId = mentionedUser.Id;
                                            List<Spell> soSpells = setAccount.Spells;

                                            spellAccounts.RemoveAt(await Global.GetXmlSpellAccountIndexByNameAsync(nomFichierXml, soName));
                                            Global.EnregistrerDonneesSpellXml(nomFichierXml, spellAccounts);
                                            SpellAccount newAccount = new SpellAccount(soName, soSpells, soUserId);
                                            spellAccounts.Add(newAccount);
                                            Global.EnregistrerDonneesSpellXml(nomFichierXml, spellAccounts);
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
                // = Gestion de la commande (admin) spell rename (nom_Personnage) (nouveauNom) =
                // =============================================================================
                else if (input.StartsWith("rename") || input.StartsWith("setname") || input.StartsWith("rn"))
                {
                    if (Global.IsStaff((SocketGuildUser)this.Context.User))
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
                                SpellAccount setAccount = await Global.GetXmlSpellAccountByNameAsync(nomFichierXml, argus[1]);
                                if (setAccount != null && await Global.GetXmlSpellAccountByNameAsync(nomFichierXml, argus[2]) == null)
                                {
                                    try
                                    {
                                        string rnName = setAccount.Name;
                                        string rnNewName = argus[2];
                                        ulong rnUserId = setAccount.UserId;
                                        List<Spell> rnSpells = setAccount.Spells;

                                        spellAccounts.RemoveAt(await Global.GetXmlSpellAccountIndexByNameAsync(nomFichierXml, rnName));
                                        Global.EnregistrerDonneesSpellXml(nomFichierXml, spellAccounts);
                                        SpellAccount newAccount = new SpellAccount(rnNewName, rnSpells, rnUserId);
                                        spellAccounts.Add(newAccount);
                                        Global.EnregistrerDonneesSpellXml(nomFichierXml, spellAccounts);
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
                                else if ((await Global.GetXmlSpellAccountByNameAsync(nomFichierXml, argus[2]) != null))
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
                // = Gestion de la commande (admin) spell replace (spell) (nom_Personnage) =
                // ========================================================================
                else if (input.StartsWith("replace") || input.StartsWith("rep"))
                {
                    if (Global.IsStaff((SocketGuildUser)this.Context.User))
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
                                SpellAccount repAccount = await Global.GetXmlSpellAccountByNameAsync(nomFichierXml, argus[1]);
                                if (repAccount != null)// Sert à s'assurer que le compte existe bien
                                {
                                    string repAccountName = repAccount.Name;
                                    List<Spell> repAccountSpells = repAccount.Spells;
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
                                        if (!string.IsNullOrEmpty(repAccountSpells[indexObj].SpellName))
                                        {
                                            try
                                            {
                                                Spell spell = repAccountSpells[indexObj];
                                                repAccountSpells[indexObj] = PublicGrim.FindSpell(argus[3]);
                                                spellAccounts.RemoveAt(await Global.GetXmlSpellAccountIndexByNameAsync(nomFichierXml, repAccountName));
                                                Global.EnregistrerDonneesSpellXml(nomFichierXml, spellAccounts);
                                                SpellAccount newAccount = new SpellAccount(repAccountName, repAccountSpells, repAccountUserId);
                                                spellAccounts.Add(newAccount);
                                                Global.EnregistrerDonneesSpellXml(nomFichierXml, spellAccounts);
                                                await ReplyAsync($"{PublicGrim.FindSpell(argus[1]).Type} \"**{spell.SpellName}**\" remplacé par \"**{argus[3]}**\" sur le compte de \"**{repAccountName}**\"");
                                                Logs.WriteLine($"{PublicGrim.FindSpell(argus[1]).Type} \"**{spell.SpellName}**\" remplacé par \"**{argus[3]}**\" sur le compte de \"**{repAccountName}**\"");
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
                                                await ReplyAsync($"{error} \"**{repAccountName}**\" ne possède pas de sortilège/enchantement à l'index \"**{argus[1]}**\"");
                                                Logs.WriteLine($"{error} \"**{repAccountName}**\" ne possède pas de sortilège/enchantement à l'index \"**{argus[1]}**\"");
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
                                        await ReplyAsync($"{error} Vous devez utiliser le numéro du sortilège/enchantement pour le remplacer");
                                        Logs.WriteLine($"{error} Vous devez utiliser le numéro du sortilège/enchantement pour le remplacer");
                                    }
                                    else
                                    {
                                        await ReplyAsync($"{error} \"**{repAccountName}**\" ne possède pas le sortilège/enchantement \"**{argus[1]}**\"");
                                        Logs.WriteLine($"{error} \"**{repAccountName}**\" ne possède pas le sortilège/enchantement \"**{argus[1]}**\"");
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
        #endregion
    }
}
