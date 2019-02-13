using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using AlterBotNet;

namespace AlterBotNet.Core.Data.Classes
{
    /// <summary>
    /// 
    /// </summary>
    public static class RepeatingTimer
    {
        private static Timer _loopingTimer;

        internal static Task StartTimer()
        {
            RepeatingTimer._loopingTimer = new Timer()
            {
                 Interval = 10000,
                 AutoReset = true,
                 Enabled=true
            };
            RepeatingTimer._loopingTimer.Elapsed += RepeatingTimer.OnTimerTicked;
            Console.WriteLine("StartTimer");
            return Task.CompletedTask;
        }

        private static void OnTimerTicked(object sender, ElapsedEventArgs e)
            => RepeatingTimer.OnTimerTickedAsync(sender, e).GetAwaiter().GetResult();

        private static bool _salaireVerse;
        private const DayOfWeek jourSalaire = DayOfWeek.Sunday;
        private const int heureSalaire = 23;
        private const int minuteSalaire = 59;

        private static async Task OnTimerTickedAsync(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Timer ticked");
            // =========================================
            // = Verse les salaires à la date indiquée =
            // =========================================
            if (DateTime.Now.DayOfWeek == RepeatingTimer.jourSalaire && DateTime.Now.Hour == RepeatingTimer.heureSalaire && DateTime.Now.Minute == RepeatingTimer.minuteSalaire && !RepeatingTimer._salaireVerse)
            {
                string nomFichier = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1\AlterBotNet.dll", @"Ressources\Database\bank.altr");
                BankAccount methodes = new BankAccount("");
                List<BankAccount> bankAccounts = (await methodes.ChargerDonneesPersosAsync(nomFichier));
                foreach (BankAccount bankAccount in bankAccounts)
                {
                    await Program.VerserSalaireAsync(bankAccount);
                }

                List<BankAccount> sortedList = bankAccounts.OrderBy(o => o.Name).ToList();
                methodes.EnregistrerDonneesPersos(nomFichier, sortedList);
                RepeatingTimer._salaireVerse = true;
            }
            else if (DateTime.Now.Hour == RepeatingTimer.heureSalaire && DateTime.Now.Minute == RepeatingTimer.minuteSalaire+1)
            {
                RepeatingTimer._salaireVerse = false;
            }

            // =============================================
            // = Actualise la liste dans le channel banque =
            // =============================================
        }
    }
}
