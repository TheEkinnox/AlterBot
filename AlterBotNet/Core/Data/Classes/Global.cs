#region USING

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

#endregion

namespace AlterBotNet.Core.Data.Classes
{
	public static class Global
	{
		#region CONSTANTES ET ATTRIBUTS STATIQUES
		internal static MethodInfo[] Commands = Assembly.GetEntryAssembly()?.GetTypes()
                      .SelectMany(t => t.GetMethods())
                      .Where(m => m.GetCustomAttributes(typeof(CommandAttribute), false).Length > 0)
                      .ToArray();
		internal static string CheminConfig = Assembly.GetEntryAssembly()?.Location
			.Replace(@"bin\Debug\netcoreapp2.1\AlterBotNet.dll", @"Core\Data\config.altr");

		internal static XmlDocument ConfigXml = Global.ChargerConfigXml();

		internal static string CheminComptesEnBanque = Assembly.GetEntryAssembly()?.Location
			.Replace(@"bin\Debug\netcoreapp2.1\AlterBotNet.dll", @"Ressources\Database\bank.altr");

		internal static string CheminComptesStuff = Assembly.GetEntryAssembly()?.Location
			.Replace(@"bin\Debug\netcoreapp2.1\AlterBotNet.dll", @"Ressources\Database\stuff.altr");

		internal static string CheminComptesStats = Assembly.GetEntryAssembly()?.Location
			.Replace(@"bin\Debug\netcoreapp2.1\AlterBotNet.dll", @"Ressources\Database\stats.altr");

		internal static string CheminComptesSpellXml = Assembly.GetEntryAssembly()?.Location
			.Replace(@"bin\Debug\netcoreapp2.1\AlterBotNet.dll", @"Ressources\Database\spellsXML.altr");

		internal static string CheminGrimoirePublic = Assembly.GetEntryAssembly()?.Location
			.Replace(@"bin\Debug\netcoreapp2.1\AlterBotNet.dll", @"Ressources\Database\grimoireXML.altr");

		internal static string CheminWarns = Assembly.GetEntryAssembly()?.Location
			.Replace(@"bin\Debug\netcoreapp2.1\AlterBotNet.dll", @"Ressources\Database\warns.altr");

		#endregion

		#region PROPRIÉTÉS ET INDEXEURS

		internal static DiscordSocketClient Client { get; set; }
		internal static SocketCommandContext Context { get; set; }
		internal static SocketTextChannel[] Banques { get; set; }
		internal static SocketTextChannel[] StuffLists { get; set; }
		internal static SocketTextChannel[] StatsLists { get; set; }
		internal static SocketTextChannel[] SpellsLists { get; set; }


		public static List<Warn> ListeWarns
		{
			get { return Global.ChargerListeWarns(); }
			set { Global.EnregistrerListeWarns(value); }
		}

		#endregion

		#region MÉTHODES

		public static bool IsRPCommand(MethodInfo cmd)
        {
			return cmd.GetCustomAttribute(typeof(RolePlayCommandAttribute),false) != null;
        }

		public static bool IsHiddenCommand(MethodInfo cmd)
        {
			return cmd.GetCustomAttribute(typeof(HiddenCommandAttribute),false) != null;
        }

		/// <summary>
		/// Vérifie si l'utilisateur est membre du Staff ou non
		/// </summary>
		/// <param name="user">Utilisateur à vérifier</param>
		/// <returns>True si l'utilisateur est membre du Staff ou false sinon</returns>
		public static bool IsStaff(SocketGuildUser user)
		{
			string targetRoleName = "Staff";
			return Global.HasRole(user, targetRoleName);
		}

		/// <summary>
		/// Vérifie si l'utilisateur possède le role indiqué ou non
		/// </summary>
		/// <param name="user">Utilisateur à vérifier</param>
		/// <param name="roleName">Nom du role à vérifier</param>
		/// <returns>True si l'utilisateur est membre du role indiqué ou false sinon</returns>
		public static bool HasRole(SocketGuildUser user, string roleName)
		{
			string targetRoleName = roleName;
			IEnumerable<ulong> result = from r in user.Guild.Roles
				where r.Name == targetRoleName
				select r.Id;
			ulong roleId = result.FirstOrDefault();
			if (roleId == 0) return false;
			SocketRole targetRole = user.Guild.GetRole(roleId);
			return user.Roles.Contains(targetRole);
		}

		/// <summary>
		/// Vérifie si l'utilisateur possède le role indiqué ou non
		/// </summary>
		/// <param name="roleName">Nom du role à vérifier</param>
		/// <returns>True si l'utilisateur est membre du role indiqué ou false sinon</returns>
		public static SocketRole GetRoleByName(SocketCommandContext context, string roleName)
		{
			string targetRoleName = roleName;
			IEnumerable<ulong> result = from r in context.Guild.Roles
				where r.Name == targetRoleName
				select r.Id;
			ulong roleId = result.FirstOrDefault();
			if (roleId == 0) throw new ArgumentException("Le role recherché n'existe pas");
			return context.Guild.GetRole(roleId);
		}

		// =================
		// = Méthodes Bank =
		// =================
		/// <summary>
		/// Mise à jour des channels banque
		/// </summary>
		public static async Task UpdateBank()
		{
			try
			{
				foreach (SocketTextChannel banque in Global.Banques)
				{
					foreach (IMessage message in await banque.GetMessagesAsync().FlattenAsync())
					{
						await message.DeleteAsync();
					}

					foreach (string msg in await Global.BankAccountsListAsync(Global.CheminComptesEnBanque))
					{
						if (!string.IsNullOrEmpty(msg))
						{
							await banque.SendMessageAsync(msg);
							Logs.WriteLine(msg);
						}
					}
				}
			}
			catch (Exception e)
			{
				Logs.WriteLine(e.ToString());
				throw;
			}

			Logs.WriteLine("Comptes en banque mis à jour");
		}

		/// <summary>
		/// Méthode permettant d'ajouter le salaire défini pour un personnage au dit personnage
		/// </summary>
		public static async Task VerserSalaireAsync(BankAccount bankAccount)
		{
			string nomFichier = Assembly.GetEntryAssembly().Location.Replace(@"bin\Debug\netcoreapp2.1\AlterBotNet.dll", @"Ressources\Database\bank.altr");
			List<BankAccount> bankAccounts = await Global.ChargerDonneesBankAsync(nomFichier);
			if (bankAccount != null)
			{
				string bankName = bankAccount.Name;
				decimal bankSalaire = bankAccount.Salaire;
				ulong bankUserId = bankAccount.UserId;
				decimal ancienMontant = bankAccount.Amount;
				decimal nvMontant = ancienMontant + bankSalaire;
				bankAccounts.RemoveAt(await Global.GetBankAccountIndexByNameAsync(nomFichier, bankName));
				Global.EnregistrerDonneesBank(nomFichier, bankAccounts);
				BankAccount newAccount = new BankAccount(bankName, nvMontant, bankUserId, bankSalaire);
				bankAccounts.Add(newAccount);
				Global.EnregistrerDonneesBank(nomFichier, bankAccounts);
				Logs.WriteLine($"Salaire de {bankName} ({bankSalaire} couronnes) versé");
				Logs.WriteLine(newAccount.ToString());
			}
		}

		public static async Task VerserSalairesAsync()
		{
			List<BankAccount> bankAccounts = await Global.ChargerDonneesBankAsync(Global.CheminComptesEnBanque);

			for (int i = 0; i < bankAccounts.Count; i++)
			{
				try
				{
					BankAccount depositAccount = await Global.GetBankAccountByNameAsync(Global.CheminComptesEnBanque, bankAccounts[i].Name);
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
		}

		public static void EnregistrerDonneesBank(string cheminFichier, List<BankAccount> savedBankAccounts)
		{
			StreamWriter fluxEcriture = new StreamWriter(cheminFichier, false);

			String personneTexte;
			for (int i = 0; i < savedBankAccounts.Count; i++)
			{
				if (savedBankAccounts[i] != null)
				{
					personneTexte = savedBankAccounts[i].Name + "," + savedBankAccounts[i].Amount + "," +
					                savedBankAccounts[i].Salaire + "," + savedBankAccounts[i].UserId;

					fluxEcriture.WriteLine(personneTexte);
				}
			}

			fluxEcriture.Close();
		}

		public static async Task<List<BankAccount>> ChargerDonneesBankAsync(string cheminFichier)
		{
			StreamReader fluxLecture = new StreamReader(cheminFichier);

			String fichierTexte = fluxLecture.ReadToEnd();
			fluxLecture.Close();

			fichierTexte = fichierTexte.Replace("\r", "");

			String[] vectLignes = fichierTexte.Split('\n');

			int nbLignes = vectLignes.Length;

			if (vectLignes[vectLignes.Length - 1] == "")
			{
				nbLignes = vectLignes.Length - 1;
			}

			BankAccount[] bankAccounts = new BankAccount[nbLignes];


			String[] vectChamps;
			string name;
			decimal amount;
			decimal salaire;
			ulong userId;

			for (int i = 0; i < bankAccounts.Length; i++)
			{
				vectChamps = vectLignes[i].Split(',');
				name = vectChamps[0].Trim();
				amount = decimal.Parse(vectChamps[1]);
				salaire = decimal.Parse(vectChamps[2]);
				userId = ulong.Parse(vectChamps[3]);

				bankAccounts[i] = new BankAccount(name, amount, userId, salaire);
			}

			return bankAccounts.ToList();
		}

		public static List<BankAccount> ChargerDonneesBank(string cheminFichier)
			=> Global.ChargerDonneesBankAsync(cheminFichier).GetAwaiter().GetResult();


		public static async Task<BankAccount> GetBankAccountByNameAsync(string nomFichier, string nomPerso)
		{
			List<BankAccount> regAccounts = Global.ChargerDonneesBank(nomFichier);
			BankAccount userAccount = null;
			for (int i = 0; i < regAccounts.Count; i++)
			{
				if (regAccounts[i].Name.ToLower().Equals(nomPerso.ToLower()))
				{
					userAccount = regAccounts[i];
				}
			}

			return userAccount;
		}

		public static BankAccount GetBankAccountByName(string nomFichier, string nomPerso)
			=> Global.GetBankAccountByNameAsync(nomFichier, nomPerso).GetAwaiter().GetResult();

		public static async Task<int> GetBankAccountIndexByNameAsync(string nomFichier, string nomPerso)
		{
			List<BankAccount> regAccounts = Global.ChargerDonneesBank(nomFichier);
			int userAccountIndex = -1;
			for (int i = 0; i < regAccounts.Count; i++)
			{
				if (regAccounts[i].Name.ToLower() == nomPerso.ToLower())
				{
					userAccountIndex = i;
				}
			}

			return userAccountIndex;
		}

		public static int GetBankAccountIndexByName(string nomFichier, string nomPerso)
			=> Global.GetBankAccountIndexByNameAsync(nomFichier, nomPerso).GetAwaiter().GetResult();

		public static async Task<List<string>> BankAccountsListAsync(string nomFichier)
		{
			List<BankAccount> regAccounts = Global.ChargerDonneesBank(nomFichier);
			List<string> message = new List<string>();
			int lastIndex = 0;
			int nbMessages = regAccounts.Count / 5 + regAccounts.Count % 5;
			for (int i = 0; i < nbMessages; i++)
			{
				try
				{
					message.Add("");

					for (int j = lastIndex; j < lastIndex + regAccounts.Count / nbMessages + regAccounts.Count % nbMessages && j < regAccounts.Count && regAccounts[j] != null; j++)
					{
						try
						{
							message[i] += $"``` ```{regAccounts[j].ToString()}";
						}
						catch (Exception e)
						{
							Logs.WriteLine(e.ToString());
							throw;
						}
					}

					lastIndex += regAccounts.Count / nbMessages + regAccounts.Count % nbMessages;
				}
				catch (Exception e)
				{
					Logs.WriteLine(e.ToString());
					throw;
				}
			}

			return message;
		}

		public static List<string> BankAccountsList(string nomFichier)
			=> Global.BankAccountsListAsync(nomFichier).GetAwaiter().GetResult();

		// ==================
		// = Méthodes stuff =
		// ==================
		/// <summary>
		/// Mise à jour des channels StuffList
		/// </summary>
		public static async Task UpdateStuff()
		{
			try
			{
				foreach (SocketTextChannel stuffList in Global.StuffLists)
				{
					foreach (IMessage message in await stuffList.GetMessagesAsync().FlattenAsync())
					{
						await message.DeleteAsync();
					}

					foreach (string msg in await Global.StuffAccountsListAsync())
					{
						if (!string.IsNullOrEmpty(msg))
						{
							await stuffList.SendMessageAsync(msg);
							Logs.WriteLine(msg);
						}
					}
				}
			}
			catch (Exception e)
			{
				Logs.WriteLine(e.ToString());
				throw;
			}

			Logs.WriteLine("Comptes de stuff mis à jour");
		}

		public static void EnregistrerDonneesStuff(string cheminFichier, List<StuffAccount> savedStuffAccounts)
		{
			StreamWriter fluxEcriture = new StreamWriter(cheminFichier, false);

			String personneTexte;
			for (int i = 0; i < savedStuffAccounts.Count; i++)
			{
				if (savedStuffAccounts[i] != null)
				{
					personneTexte = $"{savedStuffAccounts[i].Name}";
					for (int j = 0; j < savedStuffAccounts[i].Items.Count; j++)
					{
						if (savedStuffAccounts[i].Items[j] != null)
						{
							personneTexte += $",{savedStuffAccounts[i].Items[j]}";
						}
					}

					personneTexte += $",{savedStuffAccounts[i].UserId}";

					fluxEcriture.WriteLine(personneTexte);
				}
			}

			fluxEcriture.Close();
		}

		public static async Task<List<StuffAccount>> ChargerDonneesStuffAsync()
		{
			StreamReader fluxLecture = new StreamReader(Global.CheminComptesStuff);

			String fichierTexte = fluxLecture.ReadToEnd();
			fluxLecture.Close();

			fichierTexte = fichierTexte.Replace("\r", "");

			String[] vectLignes = fichierTexte.Split('\n');

			int nbLignes = vectLignes.Length;

			if (vectLignes[vectLignes.Length - 1] == "")
			{
				nbLignes = vectLignes.Length - 1;
			}

			StuffAccount[] stuffAccounts = new StuffAccount[nbLignes];


			String[] vectChamps;
			string name;
			ulong userId;

			for (int i = 0; i < stuffAccounts.Length; i++)
			{
				List<string> items = new List<string>();
				vectChamps = vectLignes[i].Split(',');
				name = vectChamps[0].Trim();
				for (int j = 1; !ulong.TryParse(vectChamps[j], out userId) && vectChamps[j] != null; j++)
				{
					items.Add(vectChamps[j]);
				}

				stuffAccounts[i] = new StuffAccount(name, items, userId);
			}

			return stuffAccounts.ToList();
		}

		private static List<StuffAccount> ChargerDonneesStuff()
			=> Global.ChargerDonneesStuffAsync().GetAwaiter().GetResult();


		public static async Task<StuffAccount> GetStuffAccountByNameAsync(string nomPerso)
		{
			List<StuffAccount> regAccounts = Global.ChargerDonneesStuff();
			StuffAccount userAccount = null;
			for (int i = 0; i < regAccounts.Count; i++)
			{
				if (regAccounts[i].Name.ToLower().Equals(nomPerso.ToLower()))
				{
					userAccount = regAccounts[i];
				}
			}

			return userAccount;
		}

		public static StuffAccount GetStuffAccountByName(string nomPerso)
			=> Global.GetStuffAccountByNameAsync(nomPerso).GetAwaiter().GetResult();

		public static async Task<int> GetStuffAccountIndexByNameAsync(string nomPerso)
		{
			List<StuffAccount> regAccounts = Global.ChargerDonneesStuff();
			int userAccountIndex = -1;
			for (int i = 0; i < regAccounts.Count; i++)
			{
				if (regAccounts[i].Name.ToLower() == nomPerso.ToLower())
				{
					userAccountIndex = i;
				}
			}

			return userAccountIndex;
		}

		public static int GetStuffAccountIndexById(ulong userId)
			=> Global.GetStuffAccountIndexByIdAsync(userId).GetAwaiter().GetResult();

		public static async Task<int> GetStuffAccountIndexByIdAsync(ulong userId)
		{
			List<StuffAccount> regAccounts = Global.ChargerDonneesStuff();
			int userAccountIndex = -1;
			for (int i = 0; i < regAccounts.Count; i++)
			{
				if (regAccounts[i].UserId == userId)
				{
					return i;
				}
			}

			return userAccountIndex;
		}

		public static int GetStuffAccountIndexByName(string nomPerso)
			=> Global.GetStuffAccountIndexByNameAsync(nomPerso).GetAwaiter().GetResult();

		public static async Task<StuffAccount> GetStuffAccountByIdAsync(ulong userId)
		{
			List<StuffAccount> regAccounts = Global.ChargerDonneesStuff();
			for (int i = 0; i < regAccounts.Count; i++)
			{
				if (regAccounts[i].UserId == userId)
				{
					return regAccounts[i];
				}
			}

			return null;
		}

		public static StuffAccount GetStuffAccountById(ulong userId)
			=> Global.GetStuffAccountByIdAsync(userId).GetAwaiter().GetResult();

		public static async Task<List<string>> StuffAccountsListAsync()
		{
			List<StuffAccount> regAccounts = Global.ChargerDonneesStuff();
			List<string> message = new List<string>();
			int lastIndex = 0;
			for (int i = 0; i < regAccounts.Count / 5 + regAccounts.Count % 5; i++)
			{
				try
				{
					message.Add("");

					for (int j = lastIndex; j < lastIndex + regAccounts.Count / 5 + regAccounts.Count % 5 && j < regAccounts.Count && regAccounts[j] != null; j++)
					{
						try
						{
							message[i] += $"``` ```{regAccounts[j].ToString()}";
						}
						catch (Exception e)
						{
							Logs.WriteLine(e.ToString());
							throw;
						}
					}

					lastIndex += regAccounts.Count / 5 + regAccounts.Count % 5;
				}
				catch (Exception e)
				{
					Logs.WriteLine(e.ToString());
					throw;
				}
			}

			return message;
		}

		public static List<string> StuffAccountsList()
			=> Global.StuffAccountsListAsync().GetAwaiter().GetResult();

		// ==================
		// = Méthodes stats =
		// ==================
		/// <summary>
		/// Mise à jour des channels Statistiques
		/// </summary>
		public static async Task UpdateStats()
		{
			try
			{
				foreach (SocketTextChannel statsList in Global.StatsLists)
				{
					foreach (IMessage message in await statsList.GetMessagesAsync().FlattenAsync())
					{
						await message.DeleteAsync();
					}

					foreach (string msg in await Global.StatsAccountsListAsync(Global.CheminComptesStats))
					{
						if (!string.IsNullOrEmpty(msg))
						{
							await statsList.SendMessageAsync(msg);
							Logs.WriteLine(msg);
						}
					}
				}
			}
			catch (Exception e)
			{
				Logs.WriteLine(e.ToString());
				throw;
			}

			Logs.WriteLine("Comptes de stats mis à jour");
		}

		public static void EnregistrerDonneesStats(string cheminFichier, List<StatsAccount> savedStatsAccounts)
		{
			StreamWriter fluxEcriture = new StreamWriter(cheminFichier, false);

			String personneTexte;
			for (int i = 0; i < savedStatsAccounts.Count; i++)
			{
				if (savedStatsAccounts[i] != null)
				{
					personneTexte = $"{savedStatsAccounts[i].Name}";
					personneTexte += $",{savedStatsAccounts[i].Force}";
					personneTexte += $",{savedStatsAccounts[i].Agilite}";
					personneTexte += $",{savedStatsAccounts[i].Technique}";
					personneTexte += $",{savedStatsAccounts[i].Magie}";
					personneTexte += $",{savedStatsAccounts[i].Resistance}";
					personneTexte += $",{savedStatsAccounts[i].Intelligence}";
					personneTexte += $",{savedStatsAccounts[i].Sociabilite}";
					personneTexte += $",{savedStatsAccounts[i].Esprit}";
					personneTexte += $",{savedStatsAccounts[i].UserId}";

					fluxEcriture.WriteLine(personneTexte);
				}
			}

			fluxEcriture.Close();
		}

		public static async Task<List<StatsAccount>> ChargerDonneesStatsAsync(string cheminFichier)
		{
			StreamReader fluxLecture = new StreamReader(cheminFichier);

			String fichierTexte = fluxLecture.ReadToEnd();
			fluxLecture.Close();

			fichierTexte = fichierTexte.Replace("\r", "");

			String[] vectLignes = fichierTexte.Split('\n');

			int nbLignes = vectLignes.Length;

			if (vectLignes[vectLignes.Length - 1] == "")
			{
				nbLignes = vectLignes.Length - 1;
			}

			StatsAccount[] statsAccounts = new StatsAccount[nbLignes];

			String[] vectChamps;
			string name;

			for (int i = 0; i < statsAccounts.Length; i++)
			{
				vectChamps = vectLignes[i].Split(',');
				name = vectChamps[0].Trim();
				uint.TryParse(vectChamps[1], out uint force);
				uint.TryParse(vectChamps[2], out uint agilite);
				uint.TryParse(vectChamps[3], out uint technique);
				uint.TryParse(vectChamps[4], out uint magie);
				uint.TryParse(vectChamps[5], out uint resistance);
				uint.TryParse(vectChamps[6], out uint intelligence);
				uint.TryParse(vectChamps[7], out uint sociabilite);
				uint.TryParse(vectChamps[8], out uint esprit);
				ulong.TryParse(vectChamps[9], out ulong userId);

				statsAccounts[i] = new StatsAccount(name, force, agilite, technique, magie, resistance, intelligence, sociabilite, esprit, userId);
			}

			return statsAccounts.ToList();
		}

		public static List<StatsAccount> ChargerDonneesStats(string cheminFichier)
			=> Global.ChargerDonneesStatsAsync(cheminFichier).GetAwaiter().GetResult();


		public static async Task<StatsAccount> GetStatsAccountByNameAsync(string nomFichier, string nomPerso)
		{
			List<StatsAccount> regAccounts = Global.ChargerDonneesStats(nomFichier);
			StatsAccount userAccount = null;
			for (int i = 0; i < regAccounts.Count; i++)
			{
				if (regAccounts[i].Name.ToLower().Equals(nomPerso.ToLower()))
				{
					userAccount = regAccounts[i];
				}
			}

			return userAccount;
		}

		public static StatsAccount GetStatsAccountByName(string nomFichier, string nomPerso)
			=> Global.GetStatsAccountByNameAsync(nomFichier, nomPerso).GetAwaiter().GetResult();

		public static async Task<int> GetStatsAccountIndexByNameAsync(string nomFichier, string nomPerso)
		{
			List<StatsAccount> regAccounts = Global.ChargerDonneesStats(nomFichier);
			int userAccountIndex = -1;
			for (int i = 0; i < regAccounts.Count; i++)
			{
				if (regAccounts[i].Name.ToLower() == nomPerso.ToLower())
				{
					userAccountIndex = i;
				}
			}

			return userAccountIndex;
		}

		public static int GetStatsAccountIndexByName(string nomFichier, string nomPerso)
			=> Global.GetStatsAccountIndexByNameAsync(nomFichier, nomPerso).GetAwaiter().GetResult();

		public static async Task<List<string>> StatsAccountsListAsync(string nomFichier)
		{
			List<StatsAccount> regAccounts = Global.ChargerDonneesStats(nomFichier);
			List<string> message = new List<string>();
			int lastIndex = 0;
			for (int i = 0; i < regAccounts.Count / 5 + regAccounts.Count % 5; i++)
			{
				try
				{
					message.Add("");

					for (int j = lastIndex; j < lastIndex + regAccounts.Count / 5 + regAccounts.Count % 5 && j < regAccounts.Count && regAccounts[j] != null; j++)
					{
						try
						{
							message[i] += $"``` ```{regAccounts[j].ToString()}";
						}
						catch (Exception e)
						{
							Logs.WriteLine(e.ToString());
							throw;
						}
					}

					lastIndex += regAccounts.Count / 5 + regAccounts.Count % 5;
				}
				catch (Exception e)
				{
					Logs.WriteLine(e.ToString());
					throw;
				}
			}

			return message;
		}

		public static List<string> StatsAccountsList(string nomFichier)
			=> Global.StatsAccountsListAsync(nomFichier).GetAwaiter().GetResult();

		// ======================
		// = Méthodes spell XML =
		// ======================
		/// <summary>
		/// Mise à jour des channels Statistiques
		/// </summary>
		public static async Task UpdateSpellXml()
		{
			try
			{
				foreach (SocketTextChannel spellList in Global.SpellsLists)
				{
					foreach (IMessage message in await spellList.GetMessagesAsync().FlattenAsync())
					{
						await message.DeleteAsync();
					}

					foreach (string msg in await Global.StatsAccountsListAsync(Global.CheminComptesStats))
					{
						if (!string.IsNullOrEmpty(msg))
						{
							await spellList.SendMessageAsync(msg);
							Logs.WriteLine(msg);
						}
					}
				}
			}
			catch (Exception e)
			{
				Logs.WriteLine(e.ToString());
				throw;
			}

			Logs.WriteLine("Comptes de stats mis à jour");
		}

		/// <summary>
		/// Mise à jour des channels SpellList
		/// </summary>
		public static void EnregistrerDonneesSpellXml(string cheminFichier, List<SpellAccount> savedSpellAccounts)
		{
			XmlDocument spellXml = new XmlDocument();

			XmlDeclaration xmlDeclaration = spellXml.CreateXmlDeclaration("1.0", "utf-8", null);
			spellXml.AppendChild(xmlDeclaration);

			XmlElement elemGrimoires = spellXml.CreateElement("grimoires");
			spellXml.AppendChild(elemGrimoires);
			XmlElement elemGrimoire, elemNom, elemSpells, elemSpell, elemSpellName, elemSpellType, elemFormule, elemSpellEffect, elemSpellLevel, elemOwnerId;
			foreach (SpellAccount spellAccount in savedSpellAccounts)
			{
				elemGrimoire = spellXml.CreateElement("grimoire");

				elemNom = spellXml.CreateElement("nom");
				elemNom.InnerText = spellAccount.Name;
				elemGrimoire.AppendChild(elemNom);
				elemSpells = spellXml.CreateElement("spells");
				elemGrimoire.AppendChild(elemSpells);

				foreach (Spell spell in spellAccount.Spells)
				{
					elemSpell = spellXml.CreateElement("spell");
					elemSpellName = spellXml.CreateElement("spellname");
					elemSpellName.InnerText = spell.SpellName;

					elemSpellType = spellXml.CreateElement("type");
					elemSpellType.InnerText = spell.Type.ToString();

					elemFormule = spellXml.CreateElement("formule");
					elemFormule.InnerText = spell.SpellFullIncantation;

					elemSpellEffect = spellXml.CreateElement("effets");
					elemSpellEffect.InnerText = spell.SpellEffects;

					elemSpellLevel = spellXml.CreateElement("niveau");
					switch (spell.Level)
					{
						case SpellLevel.Simple:
							elemSpellLevel.InnerText = "Simple";
							break;
						case SpellLevel.Basique:
							elemSpellLevel.InnerText = "Basique";
							break;
						case SpellLevel.Avance:
							elemSpellLevel.InnerText = "Avance";
							break;
						case SpellLevel.Expert:
							elemSpellLevel.InnerText = "Expert";
							break;
						case SpellLevel.Maitre:
							elemSpellLevel.InnerText = "Maitre";
							break;
					}

					elemSpell.AppendChild(elemSpellName);
					elemSpell.AppendChild(elemSpellType);
					elemSpell.AppendChild(elemFormule);
					elemSpell.AppendChild(elemSpellEffect);
					elemSpell.AppendChild(elemSpellLevel);
					elemSpells.AppendChild(elemSpell);
				}

				elemOwnerId = spellXml.CreateElement("ownerid");
				elemOwnerId.InnerText = spellAccount.UserId.ToString();

				elemGrimoire.AppendChild(elemOwnerId);
				elemGrimoires.AppendChild(elemGrimoire);
			}

			spellXml.Save(cheminFichier);
		}

		public static async Task<List<SpellAccount>> ChargerDonneesSpellXmlAsync(string cheminFichier)
		{
			List<SpellAccount> spellAccounts = new List<SpellAccount>();
			try
			{
				XmlDocument spellXml = new XmlDocument();
				spellXml.Load(cheminFichier);
				XmlNodeList listeElemGrimoire = spellXml.GetElementsByTagName("grimoire");

				foreach (XmlElement grimoire in listeElemGrimoire)
				{
					string name, spellName, incant, effects;
					List<Spell> spells = new List<Spell>();
					SpellType spellType;
					SpellLevel spellLevel;
					ulong userId;
					name = grimoire.GetElementsByTagName("nom")[0].InnerText;
					XmlNodeList listeElemSpells = grimoire.GetElementsByTagName("spells");
					if (listeElemSpells.Count > 0)
						foreach (XmlElement spell in ((XmlElement) listeElemSpells[0]).GetElementsByTagName("spell"))
						{
							spellName = spell.GetElementsByTagName("spellname")[0].InnerText;
							switch (spell.GetElementsByTagName("type")[0].InnerText)
							{
								case "Sortilege":
									spellType = SpellType.Sortilege;
									break;
								case "Enchantement":
									spellType = SpellType.Enchantement;
									break;
								default:
									spellType = 0;
									break;
							}

							incant = spell.GetElementsByTagName("formule")[0].InnerText;
							effects = spell.GetElementsByTagName("effets")[0].InnerText;
							switch (spell.GetElementsByTagName("niveau")[0].InnerText)
							{
								case "Simple":
									spellLevel = SpellLevel.Simple;
									break;
								case "Basique":
									spellLevel = SpellLevel.Basique;
									break;
								case "Avancé":
									spellLevel = SpellLevel.Avance;
									break;
								case "Expert":
									spellLevel = SpellLevel.Expert;
									break;
								case "Maitre":
									spellLevel = SpellLevel.Maitre;
									break;
								default:
									spellLevel = 0;
									break;
							}

							spells.Add(new Spell(spellName, spellType, incant, effects, spellLevel));
						}

					userId = ulong.Parse(grimoire.GetElementsByTagName("ownerid")[0].InnerText);
					spellAccounts.Add(new SpellAccount(name, spells, userId));
					spellAccounts.TrimExcess();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}

			return spellAccounts;
		}

		public static List<SpellAccount> ChargerDonneesSpellXml(string cheminFichier)
			=> Global.ChargerDonneesSpellXmlAsync(cheminFichier).GetAwaiter().GetResult();

		public static async Task<SpellAccount> GetXmlSpellAccountByNameAsync(string nomFichier, string nomPerso)
		{
			List<SpellAccount> regAccounts = Global.ChargerDonneesSpellXml(nomFichier);
			SpellAccount userAccount = null;
			for (int i = 0; i < regAccounts.Count; i++)
			{
				if (regAccounts[i].Name.ToLower().Equals(nomPerso.ToLower()))
				{
					userAccount = regAccounts[i];
				}
			}

			return userAccount;
		}

		public static SpellAccount GetXmlSpellAccountByName(string nomFichier, string nomPerso)
			=> Global.GetXmlSpellAccountByNameAsync(nomFichier, nomPerso).GetAwaiter().GetResult();

		public static async Task<int> GetXmlSpellAccountIndexByNameAsync(string nomFichier, string nomPerso)
		{
			List<SpellAccount> regAccounts = Global.ChargerDonneesSpellXml(nomFichier);
			int userAccountIndex = -1;
			for (int i = 0; i < regAccounts.Count; i++)
			{
				if (regAccounts[i].Name.ToLower() == nomPerso.ToLower())
				{
					userAccountIndex = i;
				}
			}

			return userAccountIndex;
		}

		public static int GetXmlSpellAccountIndexByName(string nomFichier, string nomPerso)
			=> Global.GetXmlSpellAccountIndexByNameAsync(nomFichier, nomPerso).GetAwaiter().GetResult();

		public static async Task<List<string>> XmlSpellAccountsListAsync(string nomFichier)
		{
			List<SpellAccount> regAccounts = Global.ChargerDonneesSpellXml(nomFichier);
			List<string> message = new List<string>();
			int lastIndex = 0;
			for (int i = 0; i < regAccounts.Count / 5 + regAccounts.Count % 5; i++)
			{
				try
				{
					message.Add("");

					for (int j = lastIndex; j < lastIndex + regAccounts.Count / 5 + regAccounts.Count % 5 && j < regAccounts.Count && regAccounts[j] != null; j++)
					{
						try
						{
							message[i] += $"``` ```{regAccounts[j].TextForm()}\n";
						}
						catch (Exception e)
						{
							Logs.WriteLine(e.ToString());
							throw;
						}
					}

					lastIndex += regAccounts.Count / 5 + regAccounts.Count % 5;
				}
				catch (Exception e)
				{
					Logs.WriteLine(e.ToString());
					throw;
				}
			}

			return message;
		}

		public static List<string> XmlSpellAccountsList(string nomFichier)
			=> Global.XmlSpellAccountsListAsync(nomFichier).GetAwaiter().GetResult();

		public static async Task<List<string>> XmlGrimSpellsListAsync(string nomFichier)
		{
			List<Spell> grimSpells = Global.ChargerDonneesSpellXml(nomFichier)[0].Spells;
			List<string> message = new List<string>();
			int lastIndex = 0;
			for (int i = 0; i < grimSpells.Count / 5 + grimSpells.Count % 5; i++)
			{
				try
				{
					message.Add("");

					for (int j = lastIndex; j < lastIndex + grimSpells.Count / 5 + grimSpells.Count % 5 && j < grimSpells.Count && grimSpells[j] != null; j++)
					{
						try
						{
							message[i] += $"\n``` ```{grimSpells[j].TextForm()}";
						}
						catch (Exception e)
						{
							Logs.WriteLine(e.ToString());
							throw;
						}
					}

					lastIndex += grimSpells.Count / 5 + grimSpells.Count % 5;
				}
				catch (Exception e)
				{
					Logs.WriteLine(e.ToString());
					throw;
				}
			}

			return message;
		}

		public static Spell FindXmlSpell(string nomSpell)
		{
			nomSpell = nomSpell.ToLower().Replace("é", "e").Replace("è", "e").Replace("ë", "e").Replace("ä", "a").Replace("ï", "i");
			Spell spell;
			List<SpellAccount> spells = Global.ChargerDonneesSpellXml(Global.CheminComptesSpellXml);
			for (int i = 0; i < spells.Count; i++)
			{
				spell = (from s in spells[i].Spells
					where s.SpellName.ToLower().Replace("é", "e").Replace("è", "e").Replace("ë", "e").Replace("ä", "a").Replace("ï", "i") == nomSpell
					select s).FirstOrDefault();
				if (spell != null)
					return spell;
			}

			return null;
		}

		public static int FindXmlSpellIndex(string nomSpell)
		{
			nomSpell = nomSpell.ToLower().Replace("é", "e").Replace("è", "e").Replace("ë", "e").Replace("ä", "a").Replace("ï", "i");
			Spell spell;
			List<SpellAccount> spells = Global.ChargerDonneesSpellXml(Global.CheminComptesSpellXml);
			for (int i = 0; i < spells.Count; i++)
			{
				spell = (from s in spells[i].Spells
					where s.SpellName.ToLower().Replace("é", "e").Replace("è", "e").Replace("ë", "e").Replace("ä", "a").Replace("ï", "i") == nomSpell
					select s).FirstOrDefault();
				if (spell != null)
					return i;
			}

			return -1;
		}

		// ===================
		// = Méthodes config =
		// ===================
		private static XmlDocument ChargerConfigXml()
		{
			XmlDocument configXml = new XmlDocument();
			try
			{
				if (!File.Exists(Global.CheminConfig))
				{
					File.Create(Global.CheminConfig);
					throw new NullReferenceException($"Le fichier est inexistant.");
				}

				configXml.Load(Global.CheminConfig);
			}
			catch (Exception e)
			{
				Logs.WriteLine("Une erreur s'est produite lors du chargement de la configuration du bot avec le message suivant : " + e.Message);
				Environment.Exit(0);
			}

			return configXml;
		}

		public static void EnregistrerConfigXml(XmlDocument nouvelleConfig)
		{
			XmlDocument configXml = new XmlDocument();
			try
			{
				if (!File.Exists(Global.CheminConfig))
					File.Create(Global.CheminConfig);

				XmlDeclaration xmlDeclaration = configXml.CreateXmlDeclaration("1.0", "utf-8", null);
				configXml.AppendChild(xmlDeclaration);

				XmlElement root = configXml.CreateElement("config");
				XmlElement version, prefixPrim, prefixSec, motd, welcomeMessage;

				version = configXml.CreateElement("version");
				version.InnerText = nouvelleConfig.GetElementsByTagName("version")[0].InnerText;
				root.AppendChild(version);
				prefixPrim = configXml.CreateElement("prefixprim");
				prefixPrim.InnerText = nouvelleConfig.GetElementsByTagName("prefixprim")[0].InnerText;
				root.AppendChild(prefixPrim);
				prefixSec = configXml.CreateElement("prefixsec");
				prefixSec.InnerText = nouvelleConfig.GetElementsByTagName("prefixsec")[0].InnerText;
				root.AppendChild(prefixSec);
				motd = configXml.CreateElement("motd");
				motd.InnerText = nouvelleConfig.GetElementsByTagName("motd")[0].InnerText;
				root.AppendChild(motd);
				welcomeMessage = configXml.CreateElement("welcomemessage");
				welcomeMessage.InnerText = nouvelleConfig.GetElementsByTagName("welcomemessage")[0].InnerText;
				root.AppendChild(welcomeMessage);
				configXml.AppendChild(root);
				configXml.Save(Global.CheminConfig);
			}
			catch (Exception e)
			{
				Logs.WriteLine("Une erreur s'est produite lors de l'enregistrement de la configuration du bot avec le message suivant : " + e.Message);
				Environment.Exit(0);
			}
		}

		// =================
		// = Méthodes Warn =
		// =================

		public static async Task<List<Warn>> ChargerListeWarnsAsync()
		{
			StreamReader fluxLecture = new StreamReader(Global.CheminWarns);
			string fichierWarns = fluxLecture.ReadToEnd().Replace('\r', ' ').Trim();
			fluxLecture.Close();

			List<Warn> warns = new List<Warn>();

			foreach (string ligne in fichierWarns.Split('\n').ToList())
			{
				if(!string.IsNullOrWhiteSpace(ligne))
				{
					string[] elements = ligne.Split(';');
					warns.Add(new Warn(ulong.Parse(elements[0] /*Id de l'utilisateur*/), elements[1] /*Raison de l'avertissement*/));
				}
			}

			return warns;
		}

		private static List<Warn> ChargerListeWarns()
			=> Global.ChargerListeWarnsAsync().GetAwaiter().GetResult();

		private static void EnregistrerListeWarns(List<Warn> warns)
		{
			StreamWriter fluxEcriture = new StreamWriter(Global.CheminWarns, false);
			foreach (Warn warn in warns)
				if (warn != null && !string.IsNullOrWhiteSpace(warn.ToString()))
					fluxEcriture.WriteLine(warn.ToString());
			fluxEcriture.Close();
		}

		#endregion
	}

    public class RolePlayCommandAttribute : Attribute
    {
    }

    public class HiddenCommandAttribute : Attribute
    {
    }
}