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
    internal class StartGameCommand : ApplicationCommandModule
    {
        [SlashCommand("startgame", "Start a game CounterCode - use this once all players have joined")]
        public async Task StartGame(InteractionContext context, [Option("name", "name of the game")] string name)
        {
            await context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            // Find the game

            QuerySnapshot MatchingGames = await Program.FirestoreDb.Collection("Games")
                .WhereEqualTo(nameof(Game.Name), name)
                .WhereEqualTo(nameof(Game.IsStarted), false)
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

            Game game = MatchingGames[0].ConvertTo<Game>();

            if(game.NumEvilPlayers < 0)
            {
                game.NumEvilPlayers = game.Players.Count / 2;
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

            game.Players.Shuffle();
            for(int i = 0; i < game.Players.Count; i++)
            {
                game.Players[i].Team = i < game.NumEvilPlayers ? Team.Evil : Team.Good;
            }
            game.Players[0].SpecialAbility = PlayerSpecialAbility.Infiltrator;

            // Shuffle again so that the evil players aren't all at the start, as other commands may expose the ordering of the player list
            game.Players.Shuffle();

            //await MatchingGames[0].Reference.UpdateAsync(nameof(Game.IsStarted), true);
            await MatchingGames[0].Reference.SetAsync(game);

            // Send start of game messages



            // Announce the start of the game

            List<string> MentionStrings = new List<string>(game.Players.Count);
            foreach(var player in game.Players)
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
