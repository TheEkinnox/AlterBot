#region MÉTADONNÉES

// Nom du fichier : Global.cs
// Auteur : Loick OBIANG (1832960)
// Date de création : 2019-02-13
// Date de modification : 2019-03-14

#endregion

#region USING

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

#endregion

namespace AlterBotNet.Core.Data.Classes
{
    public static class Global
    {
        #region CONSTANTES ET ATTRIBUTS STATIQUES

        internal static string CheminComptesEnBanque = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1\AlterBotNet.dll", @"Ressources\Database\bank.altr");
        internal static string CheminComptesStuff = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1\AlterBotNet.dll", @"Ressources\Database\stuff.altr");
        internal static string CheminComptesStats = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1\AlterBotNet.dll", @"Ressources\Database\stats.altr");

        #endregion

        #region PROPRIÉTÉS ET INDEXEURS

        internal static DiscordSocketClient Client { get; set; }
        internal static SocketTextChannel[] Banques { get; set; }
        internal static SocketTextChannel[] StuffLists { get; set; }
        internal static SocketTextChannel[] StatsLists { get; set; }

        #endregion

        #region MÉTHODES

        /// <summary>
        /// Vérifie si l'utilisateur est membre du Staff ou non
        /// </summary>
        /// <param name="user">Utilisateur à vérifier</param>
        /// <returns>True si l'utilisateur est membre du Staff ou false sinon</returns>
        public static bool IsStaff(SocketGuildUser user)
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
        public static bool HasRole(SocketGuildUser user, string roleName)
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

        // =================
        // = Méthodes Bank =
        // =================
        /// <summary>
        /// Mise à jour des channels banque
        /// </summary>
        public static async Task UpdateBank()
        {
            try
            {
                foreach (SocketTextChannel banque in Global.Banques)
                {
                    foreach (IMessage message in await banque.GetMessagesAsync().FlattenAsync())
                    {
                        await message.DeleteAsync();
                    }

                    foreach (string msg in await Global.BankAccountsListAsync(Global.CheminComptesEnBanque))
                    {
                        if (!string.IsNullOrEmpty(msg))
                        {
                            await banque.SendMessageAsync(msg);
                            Logs.WriteLine(msg);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logs.WriteLine(e.ToString());
                throw;
            }

            Logs.WriteLine("Comptes en banque mis à jour");
        }

        public static async Task VerserSalaireAsync()
        {
            List<BankAccount> bankAccounts = await Global.ChargerDonneesBankAsync(Global.CheminComptesEnBanque);

            foreach (BankAccount bankAccount in bankAccounts)
            {
                try
                {
                    if (bankAccount != null)
                    {
                        string dpName = bankAccount.Name;
                        decimal dpSalaire = bankAccount.Salaire;
                        await Program.VerserSalaireAsync(bankAccount);
                        Logs.WriteLine($"Salaire de {dpSalaire} couronnes versé sur le compte de {dpName}");
                    }
                }
                catch (Exception exception)
                {
                    Logs.WriteLine(exception.ToString());
                    throw;
                }
            }
        }

        public static void EnregistrerDonneesBank(string cheminFichier, List<BankAccount> savedBankAccounts)
        {
            StreamWriter fluxEcriture = new StreamWriter(cheminFichier, false);

            String personneTexte;
            for (int i = 0; i < savedBankAccounts.Count; i++)
            {
                if (savedBankAccounts[i] != null)
                {
                    personneTexte = savedBankAccounts[i].Name + "," + savedBankAccounts[i].Amount + "," +
                                    savedBankAccounts[i].Salaire + "," + savedBankAccounts[i].UserId;

                    fluxEcriture.WriteLine(personneTexte);
                }
            }

            fluxEcriture.Close();
        }

        public static async Task<List<BankAccount>> ChargerDonneesBankAsync(string cheminFichier)
        {
            StreamReader fluxLecture = new StreamReader(cheminFichier);

            String fichierTexte = fluxLecture.ReadToEnd();
            fluxLecture.Close();

            fichierTexte = fichierTexte.Replace("\r", "");

            String[] vectLignes = fichierTexte.Split('\n');

            int nbLignes = vectLignes.Length;

            if (vectLignes[vectLignes.Length - 1] == "")
            {
                nbLignes = vectLignes.Length - 1;
            }

            BankAccount[] bankAccounts = new BankAccount[nbLignes];


            String[] vectChamps;
            string name;
            decimal amount;
            decimal salaire;
            ulong userId;

            for (int i = 0; i < bankAccounts.Length; i++)
            {
                vectChamps = vectLignes[i].Split(',');
                name = vectChamps[0].Trim();
                amount = decimal.Parse(vectChamps[1]);
                salaire = decimal.Parse(vectChamps[2]);
                userId = ulong.Parse(vectChamps[3]);

                bankAccounts[i] = new BankAccount(name, amount, userId, salaire);
            }

            return bankAccounts.ToList();
        }

        public static List<BankAccount> ChargerDonneesBank(string cheminFichier)
            => Global.ChargerDonneesBankAsync(cheminFichier).GetAwaiter().GetResult();


        public static async Task<BankAccount> GetBankAccountByNameAsync(string nomFichier, string nomPerso)
        {
            List<BankAccount> regAccounts = Global.ChargerDonneesBank(nomFichier);
            BankAccount userAccount = null;
            for (int i = 0; i < regAccounts.Count; i++)
            {
                if (regAccounts[i].Name.ToLower().Equals(nomPerso.ToLower()))
                {
                    userAccount = regAccounts[i];
                }
            }

            return userAccount;
        }

        public static BankAccount GetBankAccountByName(string nomFichier, string nomPerso)
            => Global.GetBankAccountByNameAsync(nomFichier, nomPerso).GetAwaiter().GetResult();

        public static async Task<int> GetBankAccountIndexByNameAsync(string nomFichier, string nomPerso)
        {
            List<BankAccount> regAccounts = Global.ChargerDonneesBank(nomFichier);
            int userAccountIndex = -1;
            for (int i = 0; i < regAccounts.Count; i++)
            {
                if (regAccounts[i].Name.ToLower() == nomPerso.ToLower())
                {
                    userAccountIndex = i;
                }
            }

            return userAccountIndex;
        }

        public static int GetBankAccountIndexByName(string nomFichier, string nomPerso)
            => Global.GetBankAccountIndexByNameAsync(nomFichier, nomPerso).GetAwaiter().GetResult();

        public static async Task<List<string>> BankAccountsListAsync(string nomFichier)
        {
            List<BankAccount> regAccounts = Global.ChargerDonneesBank(nomFichier);
            List<string> message = new List<string>();
            int lastIndex = 0;
            for (int i = 0; i < regAccounts.Count / 5 + regAccounts.Count % 5; i++)
            {
                try
                {
                    message.Add("");

                    for (int j = lastIndex; j < lastIndex + regAccounts.Count / 5 + regAccounts.Count % 5 && j < regAccounts.Count && regAccounts[j] != null; j++)
                    {
                        try
                        {
                            message[i] += $"{regAccounts[j].ToString()}\n";
                        }
                        catch (Exception e)
                        {
                            Logs.WriteLine(e.ToString());
                            throw;
                        }
                    }

                    lastIndex += regAccounts.Count / 5 + regAccounts.Count % 5;
                }
                catch (Exception e)
                {
                    Logs.WriteLine(e.ToString());
                    throw;
                }
            }

            return message;
        }

        public static List<string> BankAccountsList(string nomFichier)
            => Global.BankAccountsListAsync(nomFichier).GetAwaiter().GetResult();

        // ==================
        // = Méthodes stuff =
        // ==================
        /// <summary>
        /// Mise à jour des channels StuffList
        /// </summary>
        public static async Task UpdateStuff()
        {
            try
            {
                foreach (SocketTextChannel stuffList in Global.StuffLists)
                {
                    foreach (IMessage message in await stuffList.GetMessagesAsync().FlattenAsync())
                    {
                        await message.DeleteAsync();
                    }

                    foreach (string msg in await Global.StuffAccountsListAsync(Global.CheminComptesStuff))
                    {
                        if (!string.IsNullOrEmpty(msg))
                        {
                            await stuffList.SendMessageAsync(msg);
                            Logs.WriteLine(msg);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logs.WriteLine(e.ToString());
                throw;
            }

            Logs.WriteLine("Comptes de stuff mis à jour");
        }

        public static void EnregistrerDonneesStuff(string cheminFichier, List<StuffAccount> savedStuffAccounts)
        {
            StreamWriter fluxEcriture = new StreamWriter(cheminFichier, false);

            String personneTexte;
            for (int i = 0; i < savedStuffAccounts.Count; i++)
            {
                if (savedStuffAccounts[i] != null)
                {
                    personneTexte = $"{savedStuffAccounts[i].Name}";
                    for (int j = 0; j < savedStuffAccounts[i].Items.Count; j++)
                    {
                        if (savedStuffAccounts[i].Items[j] != null)
                        {
                            personneTexte += $",{savedStuffAccounts[i].Items[j]}";
                        }
                    }

                    personneTexte += $",{savedStuffAccounts[i].UserId}";

                    fluxEcriture.WriteLine(personneTexte);
                }
            }

            fluxEcriture.Close();
        }

        public static async Task<List<StuffAccount>> ChargerDonneesStuffAsync(string cheminFichier)
        {
            StreamReader fluxLecture = new StreamReader(cheminFichier);

            String fichierTexte = fluxLecture.ReadToEnd();
            fluxLecture.Close();

            fichierTexte = fichierTexte.Replace("\r", "");

            String[] vectLignes = fichierTexte.Split('\n');

            int nbLignes = vectLignes.Length;

            if (vectLignes[vectLignes.Length - 1] == "")
            {
                nbLignes = vectLignes.Length - 1;
            }

            StuffAccount[] stuffAccounts = new StuffAccount[nbLignes];


            String[] vectChamps;
            string name;
            ulong userId;

            for (int i = 0; i < stuffAccounts.Length; i++)
            {
                List<string> items = new List<string>();
                vectChamps = vectLignes[i].Split(',');
                name = vectChamps[0].Trim();
                for (int j = 1; !ulong.TryParse(vectChamps[j], out userId) && vectChamps[j] != null; j++)
                {
                    items.Add(vectChamps[j]);
                }

                stuffAccounts[i] = new StuffAccount(name, items, userId);
            }

            return stuffAccounts.ToList();
        }

        public static List<StuffAccount> ChargerDonneesStuff(string cheminFichier)
            => Global.ChargerDonneesStuffAsync(cheminFichier).GetAwaiter().GetResult();


        public static async Task<StuffAccount> GetStuffAccountByNameAsync(string nomFichier, string nomPerso)
        {
            List<StuffAccount> regAccounts = Global.ChargerDonneesStuff(nomFichier);
            StuffAccount userAccount = null;
            for (int i = 0; i < regAccounts.Count; i++)
            {
                if (regAccounts[i].Name.ToLower().Equals(nomPerso.ToLower()))
                {
                    userAccount = regAccounts[i];
                }
            }

            return userAccount;
        }

        public static StuffAccount GetStuffAccountByName(string nomFichier, string nomPerso)
            => Global.GetStuffAccountByNameAsync(nomFichier, nomPerso).GetAwaiter().GetResult();

        public static async Task<int> GetStuffAccountIndexByNameAsync(string nomFichier, string nomPerso)
        {
            List<StuffAccount> regAccounts = Global.ChargerDonneesStuff(nomFichier);
            int userAccountIndex = -1;
            for (int i = 0; i < regAccounts.Count; i++)
            {
                if (regAccounts[i].Name.ToLower() == nomPerso.ToLower())
                {
                    userAccountIndex = i;
                }
            }

            return userAccountIndex;
        }

        public static int GetStuffAccountIndexByName(string nomFichier, string nomPerso)
            => Global.GetStuffAccountIndexByNameAsync(nomFichier, nomPerso).GetAwaiter().GetResult();

        public static async Task<List<string>> StuffAccountsListAsync(string nomFichier)
        {
            List<StuffAccount> regAccounts = Global.ChargerDonneesStuff(nomFichier);
            List<string> message = new List<string>();
            int lastIndex = 0;
            for (int i = 0; i < regAccounts.Count / 5 + regAccounts.Count % 5; i++)
            {
                try
                {
                    message.Add("");

                    for (int j = lastIndex; j < lastIndex + regAccounts.Count / 5 + regAccounts.Count % 5 && j < regAccounts.Count && regAccounts[j] != null; j++)
                    {
                        try
                        {
                            message[i] += $"{regAccounts[j].ToString()}\n";
                        }
                        catch (Exception e)
                        {
                            Logs.WriteLine(e.ToString());
                            throw;
                        }
                    }

                    lastIndex += regAccounts.Count / 5 + regAccounts.Count % 5;
                }
                catch (Exception e)
                {
                    Logs.WriteLine(e.ToString());
                    throw;
                }
            }

            return message;
        }

        public static List<string> StuffAccountsList(string nomFichier)
            => Global.StuffAccountsListAsync(nomFichier).GetAwaiter().GetResult();

        #endregion

        // ==================
        // = Méthodes stats =
        // ==================
        /// <summary>
        /// Mise à jour des channels Statistiques
        /// </summary>
        public static async Task UpdateStats()
        {
            try
            {
                foreach (SocketTextChannel statsList in Global.StatsLists)
                {
                    foreach (IMessage message in await statsList.GetMessagesAsync().FlattenAsync())
                    {
                        await message.DeleteAsync();
                    }

                    foreach (string msg in await Global.StatsAccountsListAsync(Global.CheminComptesStats))
                    {
                        if (!string.IsNullOrEmpty(msg))
                        {
                            await statsList.SendMessageAsync(msg);
                            Logs.WriteLine(msg);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logs.WriteLine(e.ToString());
                throw;
            }

            Logs.WriteLine("Comptes de stats mis à jour");
        }

        public static void EnregistrerDonneesStats(string cheminFichier, List<StatsAccount> savedStatsAccounts)
        {
            StreamWriter fluxEcriture = new StreamWriter(cheminFichier, false);

            String personneTexte;
            for (int i = 0; i < savedStatsAccounts.Count; i++)
            {
                if (savedStatsAccounts[i] != null)
                {
                    personneTexte = $"{savedStatsAccounts[i].Name}";
                    personneTexte += $",{savedStatsAccounts[i].Force}";
                    personneTexte += $",{savedStatsAccounts[i].Agilite}";
                    personneTexte += $",{savedStatsAccounts[i].Technique}";
                    personneTexte += $",{savedStatsAccounts[i].Magie}";
                    personneTexte += $",{savedStatsAccounts[i].Resistance}";
                    personneTexte += $",{savedStatsAccounts[i].Intelligence}";
                    personneTexte += $",{savedStatsAccounts[i].Esprit}";
                    personneTexte += $",{savedStatsAccounts[i].UserId}";

                    fluxEcriture.WriteLine(personneTexte);
                }
            }

            fluxEcriture.Close();
        }

        public static async Task<List<StatsAccount>> ChargerDonneesStatsAsync(string cheminFichier)
        {
            StreamReader fluxLecture = new StreamReader(cheminFichier);

            String fichierTexte = fluxLecture.ReadToEnd();
            fluxLecture.Close();

            fichierTexte = fichierTexte.Replace("\r", "");

            String[] vectLignes = fichierTexte.Split('\n');

            int nbLignes = vectLignes.Length;

            if (vectLignes[vectLignes.Length - 1] == "")
            {
                nbLignes = vectLignes.Length - 1;
            }

            StatsAccount[] statsAccounts = new StatsAccount[nbLignes];

            String[] vectChamps;
            string name;

            for (int i = 0; i < statsAccounts.Length; i++)
            {
                vectChamps = vectLignes[i].Split(',');
                name = vectChamps[0].Trim();
                uint.TryParse(vectChamps[1], out uint force);
                uint.TryParse(vectChamps[2], out uint agilite);
                uint.TryParse(vectChamps[3], out uint technique);
                uint.TryParse(vectChamps[4], out uint magie);
                uint.TryParse(vectChamps[5], out uint resistance);
                uint.TryParse(vectChamps[6], out uint intelligence);
                uint.TryParse(vectChamps[7], out uint esprit);
                ulong.TryParse(vectChamps[8], out ulong userId);

                statsAccounts[i] = new StatsAccount(name, force, agilite, technique, magie, resistance, intelligence, esprit, userId);
            }

            return statsAccounts.ToList();
        }

        public static List<StatsAccount> ChargerDonneesStats(string cheminFichier)
            => Global.ChargerDonneesStatsAsync(cheminFichier).GetAwaiter().GetResult();


        public static async Task<StatsAccount> GetStatsAccountByNameAsync(string nomFichier, string nomPerso)
        {
            List<StatsAccount> regAccounts = Global.ChargerDonneesStats(nomFichier);
            StatsAccount userAccount = null;
            for (int i = 0; i < regAccounts.Count; i++)
            {
                if (regAccounts[i].Name.ToLower().Equals(nomPerso.ToLower()))
                {
                    userAccount = regAccounts[i];
                }
            }

            return userAccount;
        }

        public static StatsAccount GetStatsAccountByName(string nomFichier, string nomPerso)
            => Global.GetStatsAccountByNameAsync(nomFichier, nomPerso).GetAwaiter().GetResult();

        public static async Task<int> GetStatsAccountIndexByNameAsync(string nomFichier, string nomPerso)
        {
            List<StatsAccount> regAccounts = Global.ChargerDonneesStats(nomFichier);
            int userAccountIndex = -1;
            for (int i = 0; i < regAccounts.Count; i++)
            {
                if (regAccounts[i].Name.ToLower() == nomPerso.ToLower())
                {
                    userAccountIndex = i;
                }
            }

            return userAccountIndex;
        }

        public static int GetStatsAccountIndexByName(string nomFichier, string nomPerso)
            => Global.GetStatsAccountIndexByNameAsync(nomFichier, nomPerso).GetAwaiter().GetResult();

        public static async Task<List<string>> StatsAccountsListAsync(string nomFichier)
        {
            List<StatsAccount> regAccounts = Global.ChargerDonneesStats(nomFichier);
            List<string> message = new List<string>();
            int lastIndex = 0;
            for (int i = 0; i < regAccounts.Count / 5 + regAccounts.Count % 5; i++)
            {
                try
                {
                    message.Add("");

                    for (int j = lastIndex; j < lastIndex + regAccounts.Count / 5 + regAccounts.Count % 5 && j < regAccounts.Count && regAccounts[j] != null; j++)
                    {
                        try
                        {
                            message[i] += $"{regAccounts[j].ToString()}\n";
                        }
                        catch (Exception e)
                        {
                            Logs.WriteLine(e.ToString());
                            throw;
                        }
                    }

                    lastIndex += regAccounts.Count / 5 + regAccounts.Count % 5;
                }
                catch (Exception e)
                {
                    Logs.WriteLine(e.ToString());
                    throw;
                }
            }

            return message;
        }

        public static List<string> StatsAccountsList(string nomFichier)
            => Global.StatsAccountsListAsync(nomFichier).GetAwaiter().GetResult();
    }
}