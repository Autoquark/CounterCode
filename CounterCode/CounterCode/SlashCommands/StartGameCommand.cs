using CounterCode.Models;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Google.Apis.Util;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterCode.SlashCommands
{
    internal class StartGameCommand : ApplicationCommandModule
    {
        [SlashCommand("startgame", "Start a game CounterCode - use this once all players have joined")]
        public async Task StartGame(InteractionContext context, [Option("name", "name of the game")] string name)
        {
            await context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            // Find the game

            QuerySnapshot MatchingGames = await Program.FirestoreDb.Games()
                .WhereEqualTo(x => x.Name, name)
                .WhereEqualTo(x => x.GameState, GameState.Setup)
                .GetSnapshotAsync();

            if (MatchingGames.Count == 0)
            {
                await context.EditResponseAsync(
                    new DiscordWebhookBuilder().WithContent("Sorry, there's no game with that name or it has already started. You can create a game with /creategame"));
                return;
            }
            else if (MatchingGames.Count > 1)
            {
                await context.EditResponseAsync(
                    new DiscordWebhookBuilder()
                        .WithContent("Uh-oh, somehow there's multiple unstarted games with that name. Please report this bug, and try creating a game with a different name"));
                return;
            }

            // Start of game logic; assign teams etc.

            DocumentSnapshot GameDocument = MatchingGames[0];
            Game game = GameDocument.ConvertTo<Game>();

            var players = (await game.GamePlayers!.GetSnapshotAsync()).Select(x => x.ConvertTo<GamePlayer>()).ToList();
            if(game.NumEvilPlayers < 0)
            {
                game.NumEvilPlayers = players.Count / 2;
            }
            // I don't know why you would want to do this, maybe as a practical joke?
            else if(game.NumEvilPlayers == 0)
            {
                game.NumInfiltrators = 0;
            }

            if(game.NumEvilPlayers > 0 && game.NumInfiltrators < 0)
            {
                game.NumInfiltrators = Math.Max(1, game.NumEvilPlayers / 3);
            }

            // Generate codes
            for (int i = 0; i < players.Count; i++)
            {
                for (int j = 0; j < game.CodesPerPlayer; j++)
                {
                    StringBuilder code = new StringBuilder();
                    for (int k = 0; k < 4; k++)
                    {
                        code.Append((char)('A' + Program.Random.Next(0, 26)));
                    }

                    game.Codes.Add(new Code
                    {
                        String = code.ToString(),
                        OwningPlayer = players[i].DocumentId
                    });
                }
            }

            // Shuffle the player indices, and make the first N of them evil
            // We don't want to use the existing ordering of players because that's just the order they joined the game
            // We don't want to shuffle the actual list because that would (I think) change the DocumentReference for all players
            var ShuffledIndices = players.Indices().Shuffled();
            for (int i = 0; i < ShuffledIndices.Count; i++)
            {
                GamePlayer player = players[ShuffledIndices[i]];
                player.Team = i < game.NumEvilPlayers ? Team.Evil : Team.Good;

                if (i < game.NumInfiltrators)
                {
                    player.SpecialAbility = PlayerSpecialAbility.Infiltrator;
                }

                await player.DocumentId!.SetAsync(player);
            }

            game.GameState = GameState.InProgress;

            await MatchingGames[0].Reference.SetAsync(game);

            // Send start of game messages

            foreach(var player in players)
            {
                await SharedLogic.SendStartOfGameInfo(game, player);
            }

            // Announce the start of the game

            List<string> MentionStrings = new List<string>(players.Count);
            foreach(var player in players)
            {
                MentionStrings.Add((await Program.DiscordClient.GetUserAsync(player.DiscordUserId)).Mention);
            }

            StringBuilder Builder = new StringBuilder($"The game {game.Name} has begun! Players are {string.Join(", ", MentionStrings)}");
            if(game.NumInfiltrators > 0)
            {
                Builder.AppendLine($"There are {game.NumEvilPlayers} Evil players including {game.NumInfiltrators} infiltrator(s)!");
            }
            else
            {
                Builder.AppendLine($"There are {game.NumEvilPlayers} Evil players!");
            }

            await context.EditResponseAsync(
                new DiscordWebhookBuilder()
                .WithContent(Builder.ToString()));
        }
    }
}
