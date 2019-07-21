#region USING

using Discord;
using Discord.WebSocket;

#endregion

namespace AlterBotNet.Core.Data.Classes
{
	public class Warn
	{
		#region PROPRIÉTÉS ET INDEXEURS

		public ulong WarnedUserId { get; }
		private string Reason { get; }

		#endregion

		#region CONSTRUCTEURS

		public Warn(ulong pWarnedUserId, string pReason)
		{
			this.WarnedUserId = pWarnedUserId;
			this.Reason = pReason;
		}

		#endregion

		#region MÉTHODES

		public override string ToString()
		{
			return $"{this.WarnedUserId.ToString()};{this.Reason}";
		}

		#endregion
	}
}