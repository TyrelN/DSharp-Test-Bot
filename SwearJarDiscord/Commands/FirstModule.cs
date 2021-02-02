using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;


namespace SwearJarDiscord.Commands
{
     
    public class FirstModule : BaseCommandModule
    {
        [Command("greet")]
        public async Task GreetCommand(CommandContext ctx)
        {
            await ctx.RespondAsync($"Greetings Thanks for executing me");
        }

        [Command("leave")]
        public async Task Disconnect(CommandContext ctx, DiscordChannel channel)
        {
            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.RespondAsync("The lavalink connection was never established!");
                return;
            }
            var node = lava.ConnectedNodes.Values.First();
            if (channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Not a valid voice channel");
                return;
            }
            var conn = node.GetGuildConnection(channel.Guild);
            if (conn == null)
            {
                await ctx.RespondAsync("lavalink is not connected");
                return;
            }
            await conn.DisconnectAsync();
            await ctx.RespondAsync($"Left {channel.Name}");
            
        }

        [Command("join")]
        public async Task JoinCommand(CommandContext ctx, DiscordChannel channel)
        {
            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.RespondAsync("The lavalink connection isn't established!");
                return;
            }
            var node = lava.ConnectedNodes.Values.First();
            if (channel.Type != ChannelType.Voice)
            {
                await ctx.RespondAsync("Not a valid voice channel.");
                return;
            }
            
            await node.ConnectAsync(channel);
            await ctx.RespondAsync("Joinned {channel.Name}!");
        }

        [Command("join")]
        public async Task JoinCommand(CommandContext ctx)
        {
            var lava = ctx.Client.GetLavalink();
            if (!lava.ConnectedNodes.Any())
            {
                await ctx.RespondAsync("The lavalink connection isn't established!");
                return;
            }
            var node = lava.ConnectedNodes.Values.First();
            DiscordChannel channel = ctx.Member.VoiceState.Channel;
            if (channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel, so I can't join you!");
                return;
            }
            await node.ConnectAsync(channel);
        }
        [Command("play"), Description("Plays an audio file.")]
        public async Task Play(CommandContext ctx, [RemainingText, Description("Full path to the file to play.")]
            string search)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel");
                return;
            }
            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected");
                return;
            }
            var loadResult = await node.Rest.GetTracksAsync(search);
            if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed ||
                loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.RespondAsync($"Track search failed for {search}");
                return;
            }
            var track = loadResult.Tracks.First();
            
            await conn.PlayAsync(track);
            
            await ctx.RespondAsync($"Now playing {track.Title}");
        }
        
        [Command("play"), Description("Plays an audio file using a url.")]
        public async Task Play(CommandContext ctx, Uri url)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                await ctx.RespondAsync("You are not in a voice channel");
                return;
            }
            var lava = ctx.Client.GetLavalink();
            var node = lava.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                await ctx.RespondAsync("Lavalink is not connected");
                return;
            }
            var loadResult = await node.Rest.GetTracksAsync(url);
            if (loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed ||
                loadResult.LoadResultType == LavalinkLoadResultType.NoMatches)
            {
                await ctx.RespondAsync($"Track search failed for {url}");
                return;
            }
            var track = loadResult.Tracks.First();
            await conn.PlayAsync(track);
            
            await ctx.RespondAsync($"Now playing {track.Title}");
            
            
        }
        
    }
}