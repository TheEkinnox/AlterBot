#region MÉTADONNÉES

// Nom du fichier : WikiCommand.cs
// Auteur : Loick OBIANG (1832960)
// Date de création : 2019-05-31
// Date de modification : 2019-05-31

#endregion

#region USING

using System;
using System.Threading.Tasks;
using AlterBotNet.Core.Data.Classes;
using Discord;
using Discord.Commands;

#endregion

namespace AlterBotNet.Core.Commands
{
    public class WikiCommand : ModuleBase<SocketCommandContext>
    {
        #region MÉTHODES

        [Command("wiki"), Summary("Commande d'accès a une page du wiki (par défaut l'acceuil)")]
        public async Task SendWiki([Remainder]string option = "none")
        {
            try
            {
                switch (option.ToLower())
                {
                    case "orc":
                    case "orcs":
                        await ReplyAsync("Les Orcs:\nhttps://alternia.fandom.com/fr/wiki/Orc");
                        break;
                    case "humain":
                    case "humains":
                        await ReplyAsync("**Les Humains**\nhttps://alternia.fandom.com/fr/wiki/Humain");
                        break;
                    case "elfe":
                    case "elfes":
                        await ReplyAsync("**Les Elfes**\nhttps://alternia.fandom.com/fr/wiki/Elfe");
                        break;
                    case "nain":
                    case "nains":
                        await ReplyAsync("**Les Nains**\nhttps://alternia.fandom.com/fr/wiki/Nain");
                        break;
                    case "gobelin":
                    case "gobelins":
                        await ReplyAsync("**Les Gobelins**\nhttps://alternia.fandom.com/fr/wiki/Gobelin");
                        break;
                    case "gnome":
                    case "gnomes":
                        await ReplyAsync("**Les Gnomes**\nhttps://alternia.fandom.com/fr/wiki/Gnome");
                        break;
                    case "scaraphin":
                    case "scaraphins":
                        await ReplyAsync("**Les Scaraphins**\nhttps://alternia.fandom.com/fr/wiki/Scaraphin");
                        break;
                    case "miranei":
                    case "miraneis":
                        await ReplyAsync("**Les Miraneis**\nhttps://alternia.fandom.com/fr/wiki/Miranei");
                        break;
                    case "arkheen":
                    case "arkheens":
                    case "arkhéen":
                    case "arkhéens":
                        await ReplyAsync("**Les Arkhéens**\nhttps://alternia.fandom.com/fr/wiki/Arkhéen");
                        break;
                    case "troll":
                    case "trolls":
                        await ReplyAsync("**Les Trolls**\nhttps://alternia.fandom.com/fr/wiki/Troll");
                        break;
                    case "nephilim":
                    case "nephilims":
                        await ReplyAsync("**Les Nephilims :heart:**\nhttps://alternia.fandom.com/fr/wiki/Nephilim");
                        break;
                    case "comu":
                    case "communaute":
                    case "la communaute":
                        await ReplyAsync("**La Communauté**https://alternia.fandom.com/fr/wiki/Catégorie:La_Communauté");
                        break;
                    case "none":
                        await ReplyAsync("**Le Wiki**\nhttps://alternia.fandom.com/fr/");
                        break;
                    default:
                        await ReplyAsync($"https://alternia.fandom.com/fr/wiki/{option}");
                        //await ReplyAsync("L'option entrée est invalide. Veuillez réessayer.");
                        //throw new ArgumentException("L'option entrée est invalide.");
                        break;
                }
            }
            catch (Exception e)
            {
                Logs.WriteLine("Une erreur s'est produite lors de l'utilisation de la commande wiki avec le message suivant : " + e.Message);
                throw;
            }
        }

        #endregion
    }
}