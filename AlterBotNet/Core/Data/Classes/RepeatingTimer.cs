using System;
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
        private const DayOfWeek jourSalaire = DayOfWeek.Monday;
        private const int heureSalaire = 00;
        private const int minuteSalaire = 00;

        private static int _ticksPasses = 120;

        private static async Task OnTimerTickedAsync(object sender, ElapsedEventArgs e)
        {
            Logs.WriteLine("Timer ticked");

            // =========================================
            // = Verse les salaires à la date indiquée =
            // =========================================
            try
            {
                if (DateTime.Now.DayOfWeek == RepeatingTimer.jourSalaire && DateTime.Now.Hour == RepeatingTimer.heureSalaire && DateTime.Now.Minute == RepeatingTimer.minuteSalaire && !RepeatingTimer._salaireVerse)
                {
                    List<BankAccount> bankAccounts = (await RepeatingTimer._methodes.ChargerDonneesPersosAsync(RepeatingTimer._cheminComptesEnBanque));
                    foreach (BankAccount bankAccount in bankAccounts)
                    {
                        await Program.VerserSalaireAsync(bankAccount);
                        RepeatingTimer._ticksPasses = 120;
                    }

                    List<BankAccount> sortedList = bankAccounts.OrderBy(o => o.Name).ToList();
                    RepeatingTimer._methodes.EnregistrerDonneesPersos(RepeatingTimer._cheminComptesEnBanque, sortedList);
                    RepeatingTimer._salaireVerse = true;
                }
                else if (DateTime.Now.Minute != RepeatingTimer.minuteSalaire)
                {
                    RepeatingTimer._salaireVerse = false;
                }
            }
            catch (Exception exception)
            {
                Logs.WriteLine(exception.ToString());
                throw;
            }

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
