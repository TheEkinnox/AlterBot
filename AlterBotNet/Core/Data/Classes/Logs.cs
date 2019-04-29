#region MÉTADONNÉES

// Nom du fichier : Logs.cs
// Auteur : Loick OBIANG (1832960)
// Date de création : 2019-02-21
// Date de modification : 2019-03-14

#endregion

#region USING

using System;
using System.IO;
using System.Reflection;

#endregion

namespace AlterBotNet.Core.Data.Classes
{
    public static class Logs
    {
        #region CONSTANTES ET ATTRIBUTS STATIQUES

        private static string _nomFichier = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1\AlterBotNet.dll", @"Data\Logs\");
        private static string _date = $"{DateTime.Now.Day}{DateTime.Now.Month}{DateTime.Now.Year}.altr";

        #endregion

        #region MÉTHODES

        public static void WriteLine(string args)
        {
            if (!File.Exists(Logs._nomFichier + Logs._date))
                File.Create(Logs._nomFichier + Logs._date);
            StreamReader fluxLecture = new StreamReader(Logs._nomFichier + Logs._date);
            fluxLecture.Close();
            StreamWriter fluxEcriture = new StreamWriter(Logs._nomFichier + Logs._date, true);
            fluxEcriture.WriteLine($"{DateTime.Now} {args}");
            fluxEcriture.Close();
            Console.WriteLine($"{DateTime.Now} {args}");
        }

        public static void Write(string args)
        {
            if (!File.Exists(Logs._nomFichier + Logs._date))
                File.Create(Logs._nomFichier + Logs._date);
            StreamReader fluxLecture = new StreamReader(Logs._nomFichier + Logs._date);
            fluxLecture.Close();
            StreamWriter fluxEcriture = new StreamWriter(Logs._nomFichier + Logs._date, true);
            fluxEcriture.Write($"{args}");
            fluxEcriture.Close();
            Console.Write($"{args}");
        }

        #endregion
    }
}