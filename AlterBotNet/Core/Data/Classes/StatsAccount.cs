#region MÉTADONNÉES

// Nom du fichier : StatsAccount.cs
// Auteur : Loick OBIANG (1832960)
// Date de création : 2019-03-14
// Date de modification : 2019-03-14

#endregion

namespace AlterBotNet.Core.Data.Classes
{
    public class StatsAccount
    {
        #region PROPRIÉTÉS ET INDEXEURS

        public string Name { get; set; }
        public uint Force { get; set; }
        public uint Agilite { get; set; }
        public uint Technique { get; set; }
        public uint Magie { get; set; }
        public uint Resistance { get; set; }
        public uint Intelligence { get; set; }
        public uint Esprit { get; set; }
        public ulong UserId { get; set; }

        #endregion

        #region CONSTRUCTEURS

        public StatsAccount(string name, uint force = 0, uint agilite = 0, uint technique = 0, uint magie = 0, uint resistance = 0, uint intelligence = 0, uint esprit = 0, ulong userId = 0)
        {
            this.Name = name;
            this.Force = force;
            this.Agilite = agilite;
            this.Technique = technique;
            this.Magie = magie;
            this.Resistance = resistance;
            this.Intelligence = intelligence;
            this.Esprit = esprit;
            this.UserId = userId;
        }

        #endregion

        #region MÉTHODES

        public override string ToString()
        {
            string message = $"**{this.Name}:**";
            message += $"\nForce: {this.Force}";
            message += $"\nAgilité: {this.Agilite}";
            message += $"\nTechnique: {this.Technique}";
            message += $"\nMagie: {this.Magie}";
            message += $"\nRésistance: {this.Resistance}";
            message += $"\nIntelligence: {this.Intelligence}";
            message += this.Esprit > 0 ? $"\nEsprit: {this.Esprit}" : "";

            return message;
        }

        #endregion
    }
}