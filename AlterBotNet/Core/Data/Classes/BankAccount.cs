#region MÉTADONNÉES

// Nom du fichier : BankAccount.cs
// Auteur : Loick OBIANG (1832960)
// Date de création : 2019-02-09
// Date de modification : 2019-03-01

#endregion

#region USING

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;

#endregion

namespace AlterBotNet.Core.Data.Classes
{
    public class BankAccount
    {
        #region PROPRIÉTÉS ET INDEXEURS

        public string Name { get; set; }
        public decimal Amount { get; set; }
        public ulong UserId { get; set; }
        public decimal Salaire { get; set; }

        #endregion

        #region CONSTRUCTEURS

        /// <summary>
        /// Constructeur permettant l'initialisation d'un compte en banque
        /// </summary>
        /// <param name="name">Nom du propriétaire du compte</param>
        /// <param name="amount">Montant disponible sur le compte</param>
        /// <param name="userId">ID Discord du créateur du compte</param>
        /// <param name="salaire">Salaire du compte</param>
        public BankAccount(string name, decimal amount = 500, ulong userId = 0, decimal salaire = 0)
        {
            this.Name = name;
            this.Amount = amount;
            this.UserId = userId;
            this.Salaire = salaire;
        }

        #endregion

        #region MÉTHODES

        public override string ToString()
        {
            string montant = $"{this.Amount:c0}";
            string salaire = $"{this.Salaire:c0}";
            string message = $"**{this.Name + ":**"} {montant.Replace("$", "Couronnes")}\nSalaire: {salaire.Replace("$", "Couronnes")}";
            return message;
        }

        #endregion
    }
}