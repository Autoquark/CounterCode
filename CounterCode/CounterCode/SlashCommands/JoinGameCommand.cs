using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CounterCode.Models;
using Google.Cloud.Firestore;

namespace CounterCode.SlashCommands
{
    internal class JoinGameCommand : ApplicationCommandModule
    {
        [SlashCommand("joingame", "Join a game of CounterCode")]
        public async Task JoinGame(InteractionContext context, [Option("name", "name of the game")] string name)
        {
            await context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            QuerySnapshot MatchingGames = await Program.FirestoreDb.Collection("Games")
                .WhereEqualTo(nameof(Game.Name), name)
                .WhereEqualTo(nameof(Game.GameState), GameState.Setup)
                .GetSnapshotAsync();

            if (MatchingGames.Count == 0)
            {
                await context.EditResponseAsync(
                    new DiscordWebhookBuilder().WithContent("Sorry, there's no game with that name or it has already started. You can create a game with /creategame"));
                return;
            }
            else if(MatchingGames.Count > 1)
            {
                await context.EditResponseAsync(
                    new DiscordWebhookBuilder()
                        .WithContent("Uh-oh, somehow there's multiple unstarted games with that name. Please report this bug, and try creating a game with a different name"));
                return;
            }

            DocumentSnapshot GameDocument = MatchingGames[0];
            Game game = GameDocument.ConvertTo<Game>();
            var players = (await game.GamePlayers!.GetSnapshotAsync()).Select(x => x.ConvertTo<GamePlayer>()).ToList();

            if (players.Any(x => x.DiscordUserId == context.User.Id))
            {
                await context.EditResponseAsync(
                    new DiscordWebhookBuilder()
                        .WithContent("You have already joined that game"));
                return;
            }

            Game? existingGame = await Program.FirestoreDb.GetUserCurrentGame(context.User.Id);
            if (existingGame != null)
            {
                await context.EditResponseAsync(
                    new DiscordWebhookBuilder()
                        .WithContent($"You are already in the game {existingGame.Name}"));
                return;
            }

            GamePlayer NewPlayer = new()
            {
                DiscordUserId = context.User.Id
            };

            game.PlayerDiscordIds.Add(NewPlayer.DiscordUserId);

            await game.GamePlayers.AddAsync(NewPlayer);
            await game.DocumentId!.SetAsync(game);

            await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Joined the game {game.Name}"));
        }
    }
}
