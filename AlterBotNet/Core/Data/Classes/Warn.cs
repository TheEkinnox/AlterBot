#region USING

using Discord;
using Discord.WebSocket;

#endregion

namespace AlterBotNet.Core.Data.Classes
{
	public class Warn
	{
		#region PROPRIÉTÉS ET INDEXEURS

		public ulong WarnedUser { get; }
		private string Reason { get; }

		#endregion

		#region CONSTRUCTEURS

		public Warn(ulong pWarnedUser, string pReason)
		{
			this.WarnedUser = pWarnedUser;
			this.Reason = pReason;
		}

		#endregion

		#region MÉTHODES

		public override string ToString()
		{
			return $"{this.WarnedUser.ToString()};{this.Reason}";
		}

		#endregion
	}
}