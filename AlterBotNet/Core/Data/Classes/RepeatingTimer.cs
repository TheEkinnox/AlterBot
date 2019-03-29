using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using AlterBotNet;
using Discord;
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

        internal static Task StartTimer()
        {
            RepeatingTimer._initialBankAccounts = Global.ChargerDonneesBankAsync(RepeatingTimer._cheminComptesEnBanque).GetAwaiter().GetResult();

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
        private static bool _salaireVerse = false;
        private const DayOfWeek jourSalaire = DayOfWeek.Sunday;
        private static int _heureSalaire = 18;
        private static int _minuteSalaire = 00;

        private static int _ticksPasses = 120;

        private static async Task OnTimerTickedAsync(object sender, ElapsedEventArgs e)
        {
            Logs.WriteLine("Timer ticked");

            // Todo: Versement automatique des salaires tous les lundi à minuit (Dimanche 18h au Canada)
            // =========================================
            // = Verse les salaires à la date indiquée =
            // =========================================
            if (DateTime.Now.DayOfWeek == RepeatingTimer.jourSalaire && DateTime.Now.Hour == RepeatingTimer._heureSalaire && DateTime.Now.Minute == RepeatingTimer._minuteSalaire && !RepeatingTimer._salaireVerse && RepeatingTimer._ticksPasses >= 3)
            {
                List<BankAccount> bankAccounts = await Global.ChargerDonneesBankAsync(RepeatingTimer._cheminComptesEnBanque);

                foreach (var bankAccount in bankAccounts)
                {
                    try
                    {
                        if (bankAccount != null)
                        {
                            string dpName = bankAccount.Name;
                            decimal dpSalaire = bankAccount.Salaire;
                            await Program.VerserSalaireAsync(bankAccount);
                            Logs.WriteLine($"Salaire de {dpSalaire} couronnes versé sur le compte de {dpName}");
                        }
                    }
                    catch (Exception exception)
                    {
                        Logs.WriteLine(exception.ToString());
                        throw;
                    }
                }

                RepeatingTimer._ticksPasses = 120;
                List<BankAccount> sortedList = bankAccounts.OrderBy(o => o.Name).ToList();
                Global.EnregistrerDonneesBank(RepeatingTimer._cheminComptesEnBanque, sortedList);
                RepeatingTimer._salaireVerse = true;
            }
            else if (DateTime.Now.Minute != RepeatingTimer._minuteSalaire)
            {
                RepeatingTimer._salaireVerse = false;
            }

            if (Global.Client.LoginState != Discord.LoginState.LoggedIn)
            {
                Program.Main();
            }
        }
    }
}
