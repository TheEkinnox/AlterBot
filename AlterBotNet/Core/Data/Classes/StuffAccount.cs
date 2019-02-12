#region MÉTADONNÉES

// Nom du fichier : StuffAccount.cs
// Auteur : Loick OBIANG (1832960)
// Date de création : 2019-02-11
// Date de modification : 2019-02-11

#endregion

#region USING

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

#endregion

namespace AlterBotNet.Core.Data.Classes
{
    public class StuffAccount
    {
        #region CONSTANTES ET ATTRIBUTS STATIQUES

        private const decimal salaire = 0;

        #endregion

        #region PROPRIÉTÉS ET INDEXEURS

        public string Name { get; set; }
        public ulong UserId { get; set; }
        public List<string> Items { get; set; }

        #endregion

        #region CONSTRUCTEURS

        /// <summary>
        /// Constructeur permettant l'initialisation d'un compte en banque
        /// </summary>
        /// <param name="name">Nom du propriétaire du compte</param>
        /// <param name="amount">Montant disponible sur le compte</param>
        /// <param name="userId">ID Discord du créateur du compte</param>
        public StuffAccount(string name = "", List<string> items = null, ulong userId = 0)
        {
            this.Name = name;
            this.Items = items;
            this.UserId = userId;
        }

        #endregion

        #region MÉTHODES

        public void EnregistrerDonneesPersos(string cheminFichier, List<StuffAccount> savedStuffAccounts)
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

        public async Task<List<StuffAccount>> ChargerDonneesPersosAsync(string cheminFichier)
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

        public List<StuffAccount> ChargerDonneesPersos(string cheminFichier)
            => new StuffAccount("").ChargerDonneesPersosAsync(cheminFichier).GetAwaiter().GetResult();


        public async Task<StuffAccount> GetStuffAccountByNameAsync(string nomFichier, string nomPerso)
        {
            List<StuffAccount> regAccounts = ChargerDonneesPersos(nomFichier);
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

        public StuffAccount GetStuffAccountByName(string nomFichier, string nomPerso)
            => new StuffAccount("").GetStuffAccountByNameAsync(nomFichier, nomPerso).GetAwaiter().GetResult();

        public async Task<int> GetStuffAccountIndexByNameAsync(string nomFichier, string nomPerso)
        {
            List<StuffAccount> regAccounts = ChargerDonneesPersos(nomFichier);
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

        public int GetBankAccountIndexByName(string nomFichier, string nomPerso)
            => new StuffAccount("").GetStuffAccountIndexByNameAsync(nomFichier, nomPerso).GetAwaiter().GetResult();

        public async Task<string> AccountsListAsync(string nomFichier)
        {
            List<StuffAccount> regAccounts = ChargerDonneesPersos(nomFichier);
            string message = "";
            for (int i = 0; i < regAccounts.Count; i++)
            {
                message += $"{regAccounts[i].ToString()}\n";
            }

            return message;
        }

        public string AccountsList(string nomFichier)
            => new StuffAccount("").AccountsListAsync(nomFichier).GetAwaiter().GetResult();

        public override string ToString()
        {
            string message = $"**{this.Name}:**";

            if (this.Items.Count > 0)
            {
                for (int j = 0; j < this.Items.Count; j++)
                {
                    if (!string.IsNullOrEmpty(this.Items[j]))
                    {
                        message += $"\n[{j}] {this.Items[j]}";
                    }
                }
            }
            else message += "\nInventaire vide";

            return message;
        }

        #endregion
    }
}