#region MÉTADONNÉES

// Nom du fichier : Roll.cs
// Auteur : Loick OBIANG (1832960)
// Date de création : 2019-02-05
// Date de modification : 2019-02-07

#endregion

#region USING

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Text;
using AlterBotNet.Core.Data.Classes;
using Discord;
using Discord.Commands;

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
            int max = 100;
            int[] resultat = new int[99999999];
            int[] nbr = new int[20];
            string msgResultat = "";
            string message;
            int sumResultats = 0;
            bool valide = true;
            bool found = false;
            char[] operat = new char[15];
            for (int i = 0; i < operat.Length; i++)
            {
                operat[i] = ' ';
            }


            if (input=="help")
            {
                await this.Context.Channel.SendMessageAsync("Lance 1 dé (par défaut 1d100) **:warning:LES PRIORITES D'OPERATEURS NE SONT PAS RESPECTEES:warning:**");
            }
            else if (input.StartsWith("d"))
            {
                if (int.TryParse(input.Replace("d", ""), out max))
                {
                    resultat[0] = this.rand.Next(max + 1);
                    Console.WriteLine($"{this.Context.User.Mention} a roll {resultat[0]}");
                    await this.Context.Channel.SendMessageAsync($"{this.Context.User.Mention} a roll {resultat[0]}");
                    //return;
                }
                else
                {
                    await this.Context.Channel.SendMessageAsync("Valeur invalide");
                    Console.WriteLine("Valeur invalide");
                    valide = false;
                    //return;
                }
            }
            else if (char.IsDigit(input[0]) && !(input.Contains("d")) && int.TryParse(input, out max))
            {
                resultat[0] = this.rand.Next(max + 1);
                Console.WriteLine($"{this.Context.User.Mention} a roll {resultat[0]}");
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
                    //if (argus[1].Contains('+') || argus[1].Contains('-') || argus[1].Contains('*') || argus[1].Contains('/'))
                    //{
                    //    // Vérifie la position des opérateurs et l'enregistre dans un vecteur d'entier
                    //    for (int i = 0; i < input.Length; i++)
                    //    {
                    //        Console.WriteLine(input[i]);
                    //        switch (input[i])
                    //        {
                    //            case '+':
                    //                {
                    //                    for (int j = i; j > 0 && operat[i - j] != '+'; j--)
                    //                    {
                    //                        if (operat[i] != '/' && operat[i] != '-' && operat[i] != '*')
                    //                        {
                    //                            operat[i] = '+';
                    //                        }
                    //                    }
                    //                    break;
                    //                }
                    //            case '-':
                    //                {
                    //                    for (int j = i; j > 0 && operat[i] != '-'; j--)
                    //                    {
                    //                        if (operat[i] != '+' && operat[i] != '/' && operat[i - j] != '*')
                    //                        {
                    //                            operat[i] = '-';
                    //                        }
                    //                    }
                    //                    break;
                    //                }
                    //            case '*':
                    //                {
                    //                    for (int j = i; j > 0 && operat[i] != '*'; j--)
                    //                    {
                    //                        if (operat[i] != '+' && operat[i] != '-' && operat[i] != '/')
                    //                        {
                    //                            operat[i] = '*';
                    //                        }
                    //                    }
                    //                    break;
                    //                }
                    //            case '/':
                    //                {
                    //                    for (int j = i; j > 0 && operat[i - j] != '/'; j--)
                    //                    {
                    //                        if (operat[i] != '+' && operat[i] != '-' && operat[i] != '*')
                    //                        {
                    //                            operat[i] = '/';
                    //                        }
                    //                    }
                    //                    break;
                    //                }
                    //            default:
                    //                Console.WriteLine(".");
                    //                break;
                    //        }
                    //    }

                    //    // Trie les opérateurs dans un nouveau vecteur de chars
                    //    Char[] sortedOperat = new char[operat.Length];
                    //    for (int i = 0; i < operat.Length; i++)
                    //    {
                    //        int lastOpeIndex = 0;
                    //        if (operat[i] == '+' || operat[i] == '-' || operat[i] == '*' || operat[i] == '/')
                    //        {
                    //            sortedOperat[lastOpeIndex] = operat[i];
                    //        }
                    //    }

                    //    // Effectue le calcul prévue
                    //    for (int i = 2; i < argus.Length && int.TryParse(argus[i], out nbr[i - 2]); i++)
                    //    {
                    //        switch (sortedOperat[i - 2])
                    //        {
                    //            case '+':
                    //                sumResultats += nbr[i - 2];
                    //                break;
                    //            case '-':
                    //                sumResultats -= nbr[i - 2];
                    //                break;
                    //            case '*':
                    //                sumResultats *= nbr[i - 2];
                    //                break;
                    //            case '/':
                    //                sumResultats /= nbr[i - 2];
                    //                break;
                    //        }
                    //        //else
                    //        //{
                    //        //    Console.WriteLine("Entrée invalide");
                    //        //    valide = false;
                    //        //}

                    //        Console.WriteLine($"====>{sortedOperat[i - 2]}");
                    //    }

                    //}
                    Console.WriteLine(argus.Length);
                    for (int i = 0; i < argus.Length; i++)
                    {
                        Console.WriteLine(argus[i]);
                    }
                    if (nbDes > 1 && valide)
                    {
                        Console.WriteLine($"{this.Context.User.Mention} a roll {sumResultats} ({msgResultat})");
                        await this.Context.Channel.SendMessageAsync($"{this.Context.User.Mention} a roll {sumResultats} ({msgResultat})");
                    }
                    //else if (operat[0] != ' ')
                    //{
                    //    Console.WriteLine($"{User} a roll {sumResultats} ({msgResultat} +-");
                    //    await this.Context.Channel.SendMessageAsync($"{User} a roll {sumResultats} ({msgResultat} +-");
                    //}
                    else if (valide)
                    {
                        Console.WriteLine($"{this.Context.User.Mention} a roll {sumResultats}");
                        await this.Context.Channel.SendMessageAsync($"{this.Context.User.Mention} a roll {sumResultats}");
                    }
                }
                else
                {
                    Console.WriteLine("Entrée invalide");
                    await this.Context.Channel.SendMessageAsync("Entrée invalide");
                }
            }
            else if (input.ToLower() == "none")
            {
                resultat[0] = this.rand.Next(max);
                Console.WriteLine($"{this.Context.User.Mention} a roll {resultat[0]}");
                await ReplyAsync($"{this.Context.User.Mention} a roll {resultat[0]}");
            }
            else
            {
                await this.Context.Channel.SendMessageAsync("Valeur invalide\n");
                Console.WriteLine("Valeur invalide\n");
            }
        }

        #endregion
    }
}