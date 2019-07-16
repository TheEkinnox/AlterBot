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
        #region PROPRIÉTÉS ET INDEXEURS

        public string Name { get; }
        public ulong UserId { get; }
        public List<string> Items { get; }

        #endregion

        #region CONSTRUCTEURS

        /// <summary>
        /// Constructeur permettant l'initialisation d'un compte en banque
        /// </summary>
        /// <param name="name">Nom du propriétaire du compte</param>
        /// <param name="items">Objets disponibles sur le compte</param>
        /// <param name="userId">ID Discord du propriétaire du compte</param>
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

        internal bool HasItem(string nomItem, bool exactSearch = false)
        {
            for (int i = 0; i < this.Items.Count; i++)
            {
                if((exactSearch && this.Items[i].ToString() == nomItem) || (!exactSearch && this.Items.ToString().Contains(nomItem)))
                    return true;
            }

            return false;
        }

        // todo: implémenter la méthode RemoveItem()
        public void RemoveItem(string nomItem = null, int indexItem = -1, bool remAll = false)
        {
            if (string.IsNullOrWhiteSpace(nomItem) && indexItem == -1 && !remAll)
                throw new ArgumentNullException(null, "Aucun paramètre n'a été entré pour la méthode \"RemoveItem()\".");
            if ((nomItem != null && !HasItem(nomItem, true)) || nomItem == null && indexItem != -1 && this.Items[indexItem] == null)
                throw new NullReferenceException("L'objet recherché ne peut être supprimé car il n'existe pas.");
        }
        #endregion
    }
}