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
    internal class EndGameCommand : ApplicationCommandModule
    {
        [SlashCommand("endgame", "End an ongoing game, or cancel a game in the setup phase")]
        public async Task EndGame(InteractionContext context, [Option("name", "name of the game to end")] string name)
        {
            await context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            QuerySnapshot Games = await Program.FirestoreDb.Games()
                .WhereEqualTo(x => x.Name, name)
                .WhereNotEqualTo(x => x.GameState, GameState.Finished)
                .GetSnapshotAsync();

            if(!Games.Any())
            {
                await context.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"There is no unfinished game named {name}"));
                return;
            }

            if(Games.Count > 1 )
            {
                await context.EditResponseAsync(
                    new DiscordWebhookBuilder()
                        .WithContent("Uh-oh, somehow there's multiple unfinished games with that name. Please report this bug!"));
                return;
            }

            Game game = Games[0].ConvertTo<Game>();
            if(game.GameState == GameState.Setup)
            {
                await Games[0].Reference.DeleteAsync();
                await context.EditResponseAsync(
                    new DiscordWebhookBuilder()
                        .WithContent($"The game {game.Name} that was in the setup phase has been cancelled"));
                return;
            }

            await context.EditResponseAsync(
                    new DiscordWebhookBuilder()
                        .WithContent("Uh-oh, somehow there's multiple unfinished games with that name. Please report this bug!"));
            return;
        }
    }
}
