using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AlterBotNet.Core.Data.Classes
{
    public class BankAccount
    {
        private const decimal salaire = 0;

        public string Name { get; set; }
        public decimal Amount { get; set; }
        public ulong UserId { get; set; }
        public decimal Salaire { get; set; }

        /// <summary>
        /// Constructeur permettant l'initialisation d'un compte en banque
        /// </summary>
        /// <param name="name">Nom du propriétaire du compte</param>
        /// <param name="amount">Montant disponible sur le compte</param>
        /// <param name="userId">ID Discord du créateur du compte</param>
        public BankAccount(string name, decimal amount = 500, ulong userId = 0)
        {
            this.Name = name;
            this.Amount = amount;
            this.UserId = userId;
        }

        public void Deposit(decimal montant)
        {
            this.Amount += montant;
        }

        public void Withdraw(decimal montant)
        {
            this.Amount -= montant;
        }

        public void EnregistrerDonneesPersos(string cheminFichier, List<BankAccount> savedBankAccounts)
        {
            StreamWriter fluxEcriture = new StreamWriter(cheminFichier, false);

            String personneTexte;
            for (int i = 0; i < savedBankAccounts.Count; i++)
            {
                if (savedBankAccounts[i] != null)
                {
                    personneTexte = savedBankAccounts[i].Name + "," + savedBankAccounts[i].Amount + "," +
                                    savedBankAccounts[i].UserId;

                    fluxEcriture.WriteLine(personneTexte);
                }
            }

            fluxEcriture.Close();
        }

        public async Task<List<BankAccount>> ChargerDonneesPersosAsync(string cheminFichier)
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
            ulong userId;

            for (int i = 0; i < bankAccounts.Length; i++)
            {
                vectChamps = vectLignes[i].Split(',');
                name = vectChamps[0].Trim();
                amount = decimal.Parse(vectChamps[1]);
                userId = ulong.Parse(vectChamps[2]);

                bankAccounts[i] = new BankAccount(name, amount, userId);
            }

            return bankAccounts.ToList();
        }

        public List<BankAccount> ChargerDonneesPersos(string cheminFichier)
            => new BankAccount("").ChargerDonneesPersosAsync(cheminFichier).GetAwaiter().GetResult();


        public async Task<BankAccount> GetBankAccountByNameAsync(string nomFichier, string nomPerso)
        {
            List<BankAccount> regAccounts = ChargerDonneesPersos(nomFichier);
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

        public BankAccount GetBankAccountByName(string nomFichier, string nomPerso)
            => new BankAccount("").GetBankAccountByNameAsync(nomFichier, nomPerso).GetAwaiter().GetResult();

        public async Task<int> GetBankAccountIndexByNameAsync(string nomFichier, string nomPerso)
        {
            List<BankAccount> regAccounts = ChargerDonneesPersos(nomFichier);
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
            => new BankAccount("").GetBankAccountIndexByNameAsync(nomFichier, nomPerso).GetAwaiter().GetResult();

        public async Task<string> AccountsListAsync(string nomFichier)
        {
            List<BankAccount> regAccounts = ChargerDonneesPersos(nomFichier);
            string message = "";
            for (int i = 0; i < regAccounts.Count; i++)
            {
                message += $"{regAccounts[i].ToString()}\n";
            }

            return message;
        }

        public string AccountsList(string nomFichier)
            => new BankAccount("").AccountsListAsync(nomFichier).GetAwaiter().GetResult();

        public override string ToString()
        {
            string message = ($"**{this.Name}:** {this.Amount} couronnes");
            return message;
        }
    }
}