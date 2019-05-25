#region MÉTADONNÉES

// Nom du fichier : TestJoin.cs
// Auteur : Loick OBIANG (1832960)
// Date de création : 2019-02-05
// Date de modification : 2019-02-05

#endregion

#region USING

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;

using Discord;
using Discord.Commands;

#endregion

namespace AlterBotNet.Core.Commands
{
    public class TestJoin : ModuleBase<SocketCommandContext>
    {
        #region MÉTHODES

        [Command("testjoin"), Alias("tj","test","join"), Summary("Simule l'arrivée d'un joueur sur le serveur")]
        public async Task JoinMessage()
        {
            await this.Context.Channel.SendMessageAsync(Config.WelcomeMessage + " " + this.Context.User.Mention);
        }

        public IChannel GetChannelByName(string name, string guildName)
        {
            IChannel message = null;
            IChannel[] channels = new IChannel[14];
            //Contexte-rp srvTest
            channels[0] = this.Context.Guild.GetChannel(542072451324968972);
            //Geographie srvTest
            channels[1] = this.Context.Guild.GetChannel(542070741504360458);
            //Banque srvTest
            channels[2] = this.Context.Guild.GetChannel(541493264180707338);
            //Regles srvTest
            channels[3] = this.Context.Guild.GetChannel(542070805236940837);
            //Liens-utiles srvTest
            channels[4] = this.Context.Guild.GetChannel(542072285033660437);
            //fiche-prototype srvTest
            channels[5] = this.Context.Guild.GetChannel(542073013722546218);
            //les-races-disponibles srvTest
            channels[6] = this.Context.Guild.GetChannel(542073051790049302);
            //Contexte-rp Alternia
            channels[7] = this.Context.Guild.GetChannel(410438433849212928);
            //Geographie Alternia
            channels[8] = this.Context.Guild.GetChannel(410531350102147072);
            //Banque Alternia
            channels[9] = this.Context.Guild.GetChannel(411969883673329665);
            //Regles Alternia
            channels[10] = this.Context.Guild.GetChannel(409789542825197568);
            //Liens-utiles Alternia
            channels[11] = this.Context.Guild.GetChannel(409849626988904459);
            //fiche-prototype Alternia
            channels[12] = this.Context.Guild.GetChannel(410424057050300427);
            //les-races-disponibles Alternia
            channels[13] = this.Context.Guild.GetChannel(410487492463165440);
            for (int i = 0; i < channels.Length; i++)
            {
                if (channels[i].Name == name && this.Context.Guild.Name == guildName)
                {
                    message = channels[i];
                }
            }

            return message;
        }

        #endregion
    }
}