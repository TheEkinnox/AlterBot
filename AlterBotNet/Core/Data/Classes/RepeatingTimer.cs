﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using AlterBotNet;
using Discord.WebSocket;

namespace AlterBotNet.Core.Data.Classes
{
    /// <summary>
    /// Classe permettant la génération d'une horloge
    /// </summary>
    public static class RepeatingTimer
    {
        private static Timer _loopingTimer;
        private static List<BankAccount> _initialBankAccounts;
        private static SocketTextChannel[] _banques;

        internal static Task StartTimer()
        {
            RepeatingTimer._initialBankAccounts = RepeatingTimer._methodes.ChargerDonneesPersosAsync(RepeatingTimer._cheminComptesEnBanque).GetAwaiter().GetResult();
            RepeatingTimer._banques = new SocketTextChannel[]
            {
                //Alternia
                Global.Client.GetGuild(399539166364303380).GetTextChannel(411969883673329665),
                //ServeurTest
                Global.Client.GetGuild(360639832017338368).GetTextChannel(541493264180707338)
            };

            RepeatingTimer._loopingTimer = new Timer()
            {
                 Interval = 15000,
                 AutoReset = true,
                 Enabled=true
            };
            RepeatingTimer._loopingTimer.Elapsed += RepeatingTimer.OnTimerTicked;
            Logs.WriteLine("StartTimer");
            return Task.CompletedTask;
        }

        private static void OnTimerTicked(object sender, ElapsedEventArgs e)
            => RepeatingTimer.OnTimerTickedAsync(sender, e).GetAwaiter().GetResult();

        static string _cheminComptesEnBanque = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1\AlterBotNet.dll", @"Ressources\Database\bank.altr");
        static BankAccount _methodes = new BankAccount("");
        private static bool _salaireVerse = false;
        private const DayOfWeek jourSalaire = DayOfWeek.Friday;
        private static int _heureSalaire = DateTime.Now.Hour;
        private static int _minuteSalaire = DateTime.Now.Minute;

        private static int _ticksPasses = 3;

        private static async Task OnTimerTickedAsync(object sender, ElapsedEventArgs e)
        {
            Logs.WriteLine("Timer ticked");

            // Todo: Versement automatique des salaires tous les lundi à minuit (Dimanche 18h au Canada)
            // =========================================
            // = Verse les salaires à la date indiquée =
            // =========================================
            //if (DateTime.Now.DayOfWeek == RepeatingTimer.jourSalaire && DateTime.Now.Hour == RepeatingTimer._heureSalaire && DateTime.Now.Minute == RepeatingTimer._minuteSalaire && !RepeatingTimer._salaireVerse && RepeatingTimer._ticksPasses >= 3)
            //{
            //    List<BankAccount> bankAccounts = await RepeatingTimer._methodes.ChargerDonneesPersosAsync(RepeatingTimer._cheminComptesEnBanque);

            //    for (int i = 0; i < bankAccounts.Count; i++)
            //    {
            //        try
            //        {
            //            BankAccount bankAccount = bankAccounts[i];
            //            if (bankAccount != null)
            //            {
            //                string dpName = bankAccount.Name;
            //                decimal dpSalaire = bankAccount.Salaire;
            //                await Program.VerserSalaireAsync(bankAccount);
            //                Logs.WriteLine($"Salaire de {dpSalaire} couronnes versé sur le compte de {dpName}");
            //            }
            //        }
            //        catch (Exception exception)
            //        {
            //            Logs.WriteLine(exception.ToString());
            //            throw;
            //        }
            //    }

            //    RepeatingTimer._ticksPasses = 120;
            //    List<BankAccount> sortedList = bankAccounts.OrderBy(o => o.Name).ToList();
            //    RepeatingTimer._methodes.EnregistrerDonneesPersos(RepeatingTimer._cheminComptesEnBanque, sortedList);
            //    RepeatingTimer._salaireVerse = true;
            //}
            //else if (DateTime.Now.Minute != RepeatingTimer._minuteSalaire)
            //{
            //    RepeatingTimer._salaireVerse = false;
            //}

            // =============================================
            // = Actualise la liste dans le channel banque =
            // =============================================
            List<BankAccount> updatedBankAccounts = RepeatingTimer._methodes.ChargerDonneesPersosAsync(RepeatingTimer._cheminComptesEnBanque).GetAwaiter().GetResult();
            if (!updatedBankAccounts.Equals(RepeatingTimer._initialBankAccounts))
            {
                RepeatingTimer._ticksPasses++;
                Logs.WriteLine(RepeatingTimer._ticksPasses.ToString());
                if (RepeatingTimer._ticksPasses >= 120)
                {
                    try
                    {
                        RepeatingTimer._methodes.EnregistrerDonneesPersos(RepeatingTimer._cheminComptesEnBanque, updatedBankAccounts);
                        RepeatingTimer._initialBankAccounts = updatedBankAccounts;
                        Logs.WriteLine("Comptes en banque mis à jour");
                        await Program.UpdateBank(RepeatingTimer._banques);
                    }
                    catch (Exception exception)
                    {
                        Logs.WriteLine(exception.ToString());
                        throw;
                    }

                    RepeatingTimer._ticksPasses = 0;
                }
            }

        }
    }
}
