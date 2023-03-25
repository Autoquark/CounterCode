using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Firestore;
using CounterCode.Models;

namespace CounterCode.SlashCommands
{
    internal class HandIn2Command : ApplicationCommandModule
    {
        [SlashCommand("handin2", "Hand in a code pair")]
        public async Task HandIn2(InteractionContext context, [Option("code1", "first code to hand in")] string code1string, [Option("code2", "second code to hand in")] string code2string)
        {
            await context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            // Check private message
            if (context.Channel.Users.Count != 2)
            {
                await context.EditResponseAsync(
                    new DiscordWebhookBuilder()
                    .WithContent("You must message me privately to perform that action"));
                return;
            }

            // Find game
            Game? game = await Program.FirestoreDb.GetUserCurrentGame(context.User.Id);
            if (game == null)
            {
                await context.EditResponseAsync(
                    new DiscordWebhookBuilder()
                    .WithContent("You are not currently in a game"));
                return;
            }

            // Check codes exist
            Code? code1 = game.Codes.FirstOrDefault(x => x.String.Equals(code1string, StringComparison.InvariantCultureIgnoreCase));

            if(code1 == null)
            {
                await context.EditResponseAsync(
                    new DiscordWebhookBuilder()
                    .WithContent($"{code1string} is not a valid code for this game"));
                return;
            }

            Code? code2 = game.Codes.FirstOrDefault(x => x.String.Equals(code2string, StringComparison.InvariantCultureIgnoreCase));

            if (code2 == null)
            {
                await context.EditResponseAsync(
                    new DiscordWebhookBuilder()
                    .WithContent($"{code2string} is not a valid code for this game"));
                return;
            }

            GamePlayer code1Player = (await code1.OwningPlayer!.GetSnapshotAsync()).ConvertTo<GamePlayer>();
            GamePlayer code2Player = (await code2.OwningPlayer!.GetSnapshotAsync()).ConvertTo<GamePlayer>();

            // Check if codes have been handed in
            //game.HandInEvents

            // Check if codes from this player combination have been handed in

            // Perform hand in
            int evilCount = code1Player.Team == Team.Evil ? 1 : 0 + code2Player.Team == Team.Evil ? 1 : 0;

            switch (evilCount)
            {
                case 0:
                    await context.EditResponseAsync(
                    new DiscordWebhookBuilder()
                    .WithContent($"({code1string}, {code2string}) is a Good Pair (two good codes). The Good Team have gained a point!"));
                break;

                case 1:
                    await context.EditResponseAsync(
                    new DiscordWebhookBuilder()
                    .WithContent($"({code1string}, {code2string}) is a Corrupted Pair (one good code, one evil code). The Good Team have lost a point!"));
                break;

                case 2:
                    await context.EditResponseAsync(
                    new DiscordWebhookBuilder()
                    .WithContent($"({code1string}, {code2string}) is an Unmasked Pair (two evil codes). The score has not been affected"));
                break;
            }


        }
    }
}
