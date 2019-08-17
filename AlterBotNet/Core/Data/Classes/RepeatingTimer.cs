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
            => RepeatingTimer.OnTimerTickedAsync().GetAwaiter().GetResult();

        static string _cheminComptesEnBanque = Global.CheminComptesEnBanque;
        private static bool _salaireVerse = false;
        private const DayOfWeek jourSalaire = DayOfWeek.Sunday;
        private static int _heureSalaire = 18;
        private static int _minuteSalaire = 00;

        private static int _ticksPasses = 120;

        private static async Task OnTimerTickedAsync()
        {
            Logs.WriteLine("Timer ticked");

            // Todo: Versement automatique des salaires tous les lundi à minuit (Dimanche 18h au Canada)
            // =========================================
            // = Verse les salaires à la date indiquée =
            // =========================================
            if (DateTime.Now.DayOfWeek == RepeatingTimer.jourSalaire && DateTime.Now.Hour == RepeatingTimer._heureSalaire && DateTime.Now.Minute == RepeatingTimer._minuteSalaire && !RepeatingTimer._salaireVerse && RepeatingTimer._ticksPasses >= 3)
            {
                Logs.WriteLine("Versement des salaires");
                List<BankAccount> bankAccounts = await Global.ChargerDonneesBankAsync(RepeatingTimer._cheminComptesEnBanque);
                for (int i = 0; i < bankAccounts.Count; i++)
                {
                    try
                    {
                        BankAccount depositAccount = await Global.GetBankAccountByNameAsync(RepeatingTimer._cheminComptesEnBanque, bankAccounts[i].Name);
                        if (depositAccount != null)
                        {
                            string dpName = depositAccount.Name;
                            decimal dpSalaire = depositAccount.Salaire;
                            Logs.WriteLine($"Salaire de {dpSalaire} couronnes versé sur le compte de {dpName}");
                            await Global.VerserSalaireAsync(depositAccount);
                        }
                    }
                    catch (Exception exception)
                    {
                        Logs.WriteLine(exception.ToString());
                    }
                }

                RepeatingTimer._ticksPasses = 120;
                RepeatingTimer._salaireVerse = true;
            }
            else if (DateTime.Now.Minute != RepeatingTimer._minuteSalaire && RepeatingTimer._salaireVerse)
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
