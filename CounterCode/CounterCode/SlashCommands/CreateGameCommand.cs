using CounterCode.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterCode.SlashCommands
{
    internal class CreateGameCommand : ApplicationCommandModule
    {
        [SlashCommand("creategame", "Set up a new game of CounterCode")]
        public async Task CreateGame(InteractionContext context, [Option("name", "name for the game")] string name)
        {
            await context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            if(context.Guild == null)
            {
                await context.EditResponseAsync(
                    new DiscordWebhookBuilder()
                    .WithContent("You can only create a game from a server channel, not by direct message (I need to know what server to run the game in!)"));
                return;
            }

            if((await Program.FirestoreDb.Games()
                .WhereEqualTo(x => x.Name, name)
                .WhereNotEqualTo(x => x.GameState, GameState.Finished)
                .Count()
                .GetSnapshotAsync()).Count > 0)
            {
                await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Sorry, there is already an unfinished game with that name. Choose a different name."));
                return;
            }

            Game NewGame = new()
            {
                Name = name,
                GuildId = context.Guild.Id
            };

            await Program.FirestoreDb.Collection("Games")
                .AddAsync(NewGame);

            await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Created a new game called {NewGame.Name}. Players can now join with /joingame {NewGame.Name}"));
        }
    }
}
