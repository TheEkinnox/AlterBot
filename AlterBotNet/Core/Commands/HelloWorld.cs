using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Text;

using Discord;
using Discord.Commands;

namespace AlterBotNet.Core.Commands
{
    public class HelloWorld : ModuleBase<SocketCommandContext>
    {
        private Random _rand = new Random();

        [Command("hello"), Alias("helloworld", "world"), Summary("Commande hello world")]
        public async Task SendMessage()
        {
            await this.Context.Channel.SendMessageAsync($"Wsh {this.Context.User.Mention}, bien ou bien ma couille");
        }

        [Command("testembed"), Alias("embed", "te", "emb")]
        public async Task SendEmbed([Remainder]string input = "None")
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithAuthor("Test embed", this.Context.User.GetAvatarUrl());
            embed.WithColor(250, 125, 125);
            embed.WithFooter("Le proprio génial du discord", this.Context.Guild.Owner.GetAvatarUrl());
            embed.WithDescription("This is a **PEEEEERFECT** random desc with an `Awesome` link.\n" +
                                  "[Le meilleur des wiki :heart::heart:](http://fr.alternia.wikia.com)");

            await this.Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("plop")]
        public async Task SendOctoplop()
        {
            // ReSharper disable once InconsistentNaming
            string imgsPath = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1\AlterBotNet.dll", @"Data\Plop\");
            string[] vectImgs = new string[]
            {
                ($"{imgsPath}p0.jpg"),
                ($"{imgsPath}p1.jpg"),
                ($"{imgsPath}p2.jpg"),
                ($"{imgsPath}p3.jpg"),
                ($"{imgsPath}p4.jpg"),
                ($"{imgsPath}p5.jpg"),
                ($"{imgsPath}p6.jpg"),
                ($"{imgsPath}p7.jpg"),
            };
            string[] vectCapt = new String[]
            {
                "POUUUUUUULPE!",
                "Quelqu'un a parlé de Takoyakis?",
                "FAAAAABULEUX!",
                "Un beau cadeau pour...naaaan oublie",
                "C mi ke jsui le + bô",
                "La vie est beeeeeelle!",
                "Plop mon poulpe",
                "Alors déjà on dit pas poulpe, c'est supeeeer péjoratif!",
                "Alors déjà on dit pas poulpe, c'est supeeeer péjoratif!",
                "Alors déjà on dit pas poulpe, c'est supeeeer péjoratif!",
                "Alors déjà on dit pas poulpe, c'est supeeeer péjoratif!"
            };
            try
            {
                //await this.Context.Channel.SendFileAsync((@"C:\Users\1832960\source\repos\AlterBotNet\AlterBotNet\Data\Plop\p6.jpg"));
                await this.Context.Channel.SendFileAsync(vectImgs[this._rand.Next(vectImgs.Length)], $"{vectCapt[this._rand.Next(vectCapt.Length)]}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
        }

        [Command("say")]
        public async Task SendSayMessage([Remainder] string input = "")
        {
            await this.Context.Message.DeleteAsync();
            await ReplyAsync(input);
        }

        [Command("help"), Summary("Envoie la liste des commandes disponibles en mp")]
        public async Task SendHelp()
        {
            string message = "";
            message += "\n";
            message += "Liste des commandes: `help`\n";
            message += "Aide sur la commande bank: `bank help`\n";
            message += "Aide sur la commande stuff: `stuff help`\n";
            message += "Envoyer une image de poulpe avec un message aléatoire: `plop`\n";
            message += "Lancer un dé: `roll 1d100`\n";
            message += "Faire parler le bot (c useless): `say message`\n";
            message += "Saluer l'utilisateur qui a envoyé la commande: `hello`\n";
            message += "Tester le message de bienvenue sur le serveur: `testjoin`\n";
            try
            {
                await ReplyAsync("Infos envoyées en mp");
                Console.WriteLine($"message envoyé en mp à {this.Context.User.Username}");
                EmbedBuilder eb = new EmbedBuilder();
                eb.WithTitle(("**Liste des commandes disponibles**"))
                    .WithColor(this._rand.Next(256), this._rand.Next(256), this._rand.Next(256))
                    .AddField("==============================================", message);
                //await this.Context.User.SendMessageAsync(infoAccount.ToString());
                await this.Context.User.SendMessageAsync("", false, eb.Build());
                Console.WriteLine(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }
        }
    }
}
