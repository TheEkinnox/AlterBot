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