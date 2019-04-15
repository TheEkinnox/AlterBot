﻿#region MÉTADONNÉES

// Nom du fichier : Roll.cs
// Auteur : Loick OBIANG (1832960)
// Date de création : 2019-02-05
// Date de modification : 2019-02-07

#endregion

#region USING

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Text;
using AlterBotNet.Core.Data.Classes;
using Discord;
using Discord.Commands;
using MathParserTK;

#endregion

namespace AlterBotNet.Core.Commands
{
    public class Roll : ModuleBase<SocketCommandContext>
    {
        #region ATTRIBUTS

        private Random rand = new Random();

        #endregion

        #region MÉTHODES

        [Command("roll"), Alias("de", "dice", "r"), Summary("Lance 1 dé (par défaut 1d100) **:warning:LES PRIORITES D'OPERATEURS NE SONT PAS RESPECTEES:warning:**")]
        public async Task LancerDe([Remainder]string input = "none")
        {
            MathParser parser = new MathParser();
            const int bonusCaly = 8;
            int max = 100;
            int[] resultat = new int[99999999];
            int[] nbr = new int[20];
            string msgResultat = "";
            string calcul = "";
            int sumResultats = 0;
            bool valide = true;
            bool containsCalcul = false;

            if (input == "help")
            {
                await this.Context.Channel.SendMessageAsync("Lance le nombre indiqué de dés (par défaut 1d100)");
            }
            else if (input == "magieprimaire" || input == "mp")
            {
                resultat[0] = this.rand.Next(1,4 + 1);
                Logs.WriteLine($"{this.Context.User.Username} a roll {resultat[0]}");
                switch (resultat[0])
                {
                    case 1:
                        Logs.WriteLine($"@{this.Context.User.Username} a obtenu une magie d'eau (@{Global.GetRoleByName(this.Context, "MJ").Name})");
                        await ReplyAsync($"{this.Context.User.Mention} a obtenu une magie d'eau ({Global.GetRoleByName(this.Context, "MJ").Mention})");
                        break;
                    case 2:
                        Logs.WriteLine($"@{this.Context.User.Username} a obtenu une magie d'air (@{Global.GetRoleByName(this.Context, "MJ").Name})");
                        await ReplyAsync($"{this.Context.User.Mention} a obtenu une magie d'air ({Global.GetRoleByName(this.Context, "MJ").Mention})");
                        break;
                    case 3:
                        Logs.WriteLine($"@{this.Context.User.Username} a obtenu une magie de feu (@{Global.GetRoleByName(this.Context, "MJ").Name})");
                        await ReplyAsync($"{this.Context.User.Mention} a obtenu une magie de feu ({Global.GetRoleByName(this.Context, "MJ").Mention})");
                        break;
                    case 4:
                        Logs.WriteLine($"@{this.Context.User.Username} a obtenu une magie de terre (@{Global.GetRoleByName(this.Context, "MJ").Name})");
                        await ReplyAsync($"{this.Context.User.Mention} a obtenu une magie de terre ({Global.GetRoleByName(this.Context, "MJ").Mention})");
                        break;
                }
            }
            else if (input.StartsWith("d"))
            {
                if (int.TryParse(input.Replace("d", ""), out max))
                {
                    resultat[0] = this.rand.Next(max + 1);
                    Logs.WriteLine($"{this.Context.User.Username} a roll {resultat[0]}");
                    await ReplyAsync($"{this.Context.User.Mention} a roll {resultat[0]}");
                    //return;
                }
                else
                {
                    await this.Context.Channel.SendMessageAsync("Valeur invalide");
                    Logs.WriteLine("Valeur invalide");
                    valide = false;
                    //return;
                }
            }
            else if (char.IsDigit(input[0]) && !(input.Contains("d")) && int.TryParse(input, out max))
            {
                resultat[0] = this.rand.Next(max + 1);
                Logs.WriteLine($"{this.Context.User.Username} a roll {resultat[0]}");
                await this.Context.Channel.SendMessageAsync($"{this.Context.User.Mention} a roll {resultat[0]}");
            }
            else if (char.IsDigit(input[0]) && input.Contains("d"))
            {
                string[] argus = input.Split('d', '+', '-', '*', '/');
                if (int.TryParse(argus[0], out int nbDes) && (!String.IsNullOrEmpty(argus[1]) && int.TryParse(argus[1], out max)))
                {
                    for (int i = 0; i < nbDes; i++)
                    {
                        resultat[i] = this.rand.Next(max + 1);
                        sumResultats += resultat[i];

                        if (i + 1 < nbDes)
                        {
                            msgResultat += ($"{resultat[i]}, ");
                        }
                        else
                        {
                            msgResultat += ($"{resultat[i]}");
                        }
                    }
                    if (input.Contains('+') || input.Contains('-') || input.Contains('*') || input.Contains('/'))
                    {
                        containsCalcul = true;
                        calcul = input.Replace($"{nbDes}d{max}", "");
                        Logs.WriteLine(sumResultats.ToString());
                        sumResultats = (int)parser.Parse(sumResultats.ToString() + calcul, false);
                        Logs.WriteLine("Calcul effectué");
                        Logs.WriteLine(sumResultats.ToString());
                        msgResultat += calcul;
                    }
                    for (int i = 0; i < argus.Length; i++)
                    {
                        Logs.WriteLine(argus[i]);
                    }
                    if (nbDes > 1 && valide && this.Context.User.Id != 298614183258488834)
                    {
                        Logs.WriteLine($"{this.Context.User.Username} a roll {sumResultats} ({msgResultat})");
                        await this.Context.Channel.SendMessageAsync($"{this.Context.User.Mention} a roll {sumResultats} ({msgResultat})");
                    }
                    else if (containsCalcul)
                    {
                        Logs.WriteLine($"{this.Context.User.Username} a roll {sumResultats} ({msgResultat}");
                        await this.Context.Channel.SendMessageAsync($"{this.Context.User.Mention} a roll {sumResultats} ({msgResultat} = {sumResultats})");
                    }
                    else if (valide)
                    {
                        //if (this.Context.User.Username != "TheEkinnox")
                        //{
                        Logs.WriteLine($"{this.Context.User.Username} a roll {sumResultats}");
                        await this.Context.Channel.SendMessageAsync($"{this.Context.User.Mention} a roll {sumResultats}");
                        //}
                        //else
                        //{
                        //    Logs.WriteLine($"{this.Context.User.Username} a roll {sumResultats}");
                        //    await this.Context.Channel.SendMessageAsync($"{this.Context.User.Mention} a roll {94}");
                        //}
                    }
                }
                else
                {
                    Logs.WriteLine("Entrée invalide");
                    await this.Context.Channel.SendMessageAsync("Entrée invalide");
                }
            }
            else if (input.ToLower() == "none")
            {
                resultat[0] = this.rand.Next(max);
                if (this.Context.User.Id != 298614183258488834)
                {
                    Logs.WriteLine($"{this.Context.User.Username} a roll {resultat[0]}");
                    await this.Context.Channel.SendMessageAsync($"{this.Context.User.Mention} a roll {resultat[0]}");
                }
                else if (this.Context.User.Id == 298614183258488834)
                {
                    if (resultat[0] - 8 < 0)
                        resultat[0] += 8;
                    Logs.WriteLine($"{this.Context.User.Username} a roll {resultat[0] + "-" + bonusCaly}");
                    await this.Context.Channel.SendMessageAsync($"{this.Context.User.Mention} a roll {resultat[0] - bonusCaly}");
                }
            }
            else
            {
                await this.Context.Channel.SendMessageAsync("Valeur invalide\n");
                Logs.WriteLine("Valeur invalide\n");
            }
        }

        #endregion
    }
}