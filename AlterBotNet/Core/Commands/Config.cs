#region MÉTADONNÉES

// Nom du fichier : Config.cs
// Auteur : Loick OBIANG (1832960)
// Date de création : 2019-05-25
// Date de modification : 2019-05-28

#endregion

#region USING

using System;
using AlterBotNet.Core.Data.Classes;

#endregion

namespace AlterBotNet.Core.Commands
{
    public static class Config
    {
        #region PROPRIÉTÉS ET INDEXEURS

        public static string Version
        {
            get
            {
                string message = null;
                try
                {
                    message = Global.ConfigXml.GetElementsByTagName("version")[0].InnerText;
                }
                catch (Exception e)
                {
                    Logs.WriteLine("Une erreur s'est produite lors de l'obtention du motd avec le message suivant : " + e.Message);
                }

                return message;
            }
            set
            {
                try
                {
                    Global.ConfigXml.GetElementsByTagName("version")[0].InnerText = value;
                    Global.EnregistrerConfigXml(Global.ConfigXml);
                }
                catch (Exception e)
                {
                    Logs.WriteLine("Une erreur s'est produite lors de la modification du motd avec le message suivant : " + e.Message);
                }
            }
        }

        public static string PrefixPrim
        {
            get
            {
                string message = null;
                try
                {
                    message = Global.ConfigXml.GetElementsByTagName("prefixprim")[0].InnerText;
                }
                catch (Exception e)
                {
                    Logs.WriteLine("Une erreur s'est produite lors de l'obtention du motd avec le message suivant : " + e.Message);
                }

                return message;
            }
            set
            {
                try
                {
                    Global.ConfigXml.GetElementsByTagName("prefixprim")[0].InnerText = value;
                    Global.EnregistrerConfigXml(Global.ConfigXml);
                }
                catch (Exception e)
                {
                    Logs.WriteLine("Une erreur s'est produite lors de la modification du motd avec le message suivant : " + e.Message);
                }
            }
        }

        public static string PrefixSec
        {
            get
            {
                string message = null;
                try
                {
                    message = Global.ConfigXml.GetElementsByTagName("prefixsec")[0].InnerText;
                }
                catch (Exception e)
                {
                    Logs.WriteLine("Une erreur s'est produite lors de l'obtention du motd avec le message suivant : " + e.Message);
                }

                return message;
            }
            set
            {
                try
                {
                    Global.ConfigXml.GetElementsByTagName("prefixsec")[0].InnerText = value;
                    Global.ConfigXml.Save(Global.CheminConfig);
                }
                catch (Exception e)
                {
                    Logs.WriteLine("Une erreur s'est produite lors de la modification du motd avec le message suivant : " + e.Message);
                }
            }
        }

        public static string Motd
        {
            get
            {
                string message = null;
                try
                {
                    message = Global.ConfigXml.GetElementsByTagName("motd")[0].InnerText;
                }
                catch (Exception e)
                {
                    Logs.WriteLine("Une erreur s'est produite lors de l'obtention du motd avec le message suivant : " + e.Message);
                }

                return message;
            }
            set
            {
                try
                {
                    Global.ConfigXml.GetElementsByTagName("motd")[0].InnerText = value;
                    Global.EnregistrerConfigXml(Global.ConfigXml);
                }
                catch (Exception e)
                {
                    Logs.WriteLine("Une erreur s'est produite lors de la modification du motd avec le message suivant : " + e.Message);
                }
            }
        }

        public static string WelcomeMessage
        {
            get
            {
                string message = null;
                try
                {
                    message = Global.ConfigXml.GetElementsByTagName("welcomemessage")[0].InnerText;
                }
                catch (Exception e)
                {
                    Logs.WriteLine("Une erreur s'est produite lors de l'obtention du message de bienvenue avec le message suivant : " + e.Message);
                }

                return message;
            }
            set
            {
                try
                {
                    Global.ConfigXml.GetElementsByTagName("welcomemessage")[0].InnerText = value;
                    Global.EnregistrerConfigXml(Global.ConfigXml);
                }
                catch (Exception e)
                {
                    Logs.WriteLine("Une erreur s'est produite lors de la modification du message de bienvenue avec le message suivant : " + e.Message);
                }
            }
        }

        #endregion
    }
}