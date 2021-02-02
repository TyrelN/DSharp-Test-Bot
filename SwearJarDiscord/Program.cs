using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SwearJarDiscord.Commands;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;


namespace SwearJarDiscord
{
   
    class Program
    {
        public static DiscordClient discord;
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }
        
        private static async Task MainAsync()
        {
            
            var json = string.Empty;
            using(var fs = File.OpenRead("../../../configuration.json"))//a hacky way to arrive at the config directory
                using(var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    json = await sr.ReadToEndAsync().ConfigureAwait(false);
            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);
            //json config reading complete
           
            
            discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
                Intents = DiscordIntents.GuildEmojis
                | DiscordIntents.GuildVoiceStates
                | DiscordIntents.GuildMessageReactions
                | DiscordIntents.GuildMessages
                | DiscordIntents.Guilds
               
            });
            DiscordChannel channel = null;
        
            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] {configJson.Prefix}
            });
            commands.RegisterCommands<FirstModule>();
            var endpoint = new ConnectionEndpoint
            {
                Hostname = "127.0.0.1",
                Port = 5688
            };
            
            var lavalinkConfig = new LavalinkConfiguration
            {
                Password = "youshallnotpass",
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };
            var lavalink = discord.UseLavalink();
            //below are the event listeners
            discord.VoiceStateUpdated += async (s, e) => //when someone joins leaves or moves voice channel
            {
                DiscordGuild guild = e.Guild;
                DiscordChannel currentChannel = e.Channel;
                string user = e.User.Username;
               if (user.Equals("User"))
                {
                    await discord.SendMessageAsync(await discord.GetChannelAsync(803584072572076055), $"{user }alert ",true);
                }
            };
            
            discord.MessageCreated += async (s, e) =>
            {
                String user = e.Author.Username;
                //await e.Message.ModifyAsync("ahhhh another one.");
                if (e.Message.Content.ToLower().Contains("fiddlesticks"))
                {
                    await e.Message.DeleteAsync();
                    await e.Message.RespondAsync($"Wuh-oh {user}, we don't use that kinda language 'round these parts.");
                }
                   
                if (e.Author.Username.Equals("User") && e.Message.Content.ToLowerInvariant().Contains("lol"))
                {
                    await e.Message.RespondAsync($"It probably wasn't that funny {user}, chill out");
                }
            };
            
            await discord.ConnectAsync();
            await lavalink.ConnectAsync(lavalinkConfig);
            await Task.Delay(-1);
            
        }

        

    }
    
}