using System;
using System.Collections.Generic;
using System.Text;

using Discord;
using Discord.WebSocket;

namespace AlterBotNet.Core.Data.Classes
{
    public static class Global
    {
        internal static DiscordSocketClient Client { get; set; }
    }
}
