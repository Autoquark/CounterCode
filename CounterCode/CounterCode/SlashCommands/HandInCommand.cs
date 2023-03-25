using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterCode.SlashCommands
{
    internal class HandInCommand : ApplicationCommandModule
    {
        [SlashCommand("handin", "Hand in a code")]
        public async Task HandIn(InteractionContext context, [Option("code", "code to hand in")] string code)
        {
            await context.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

            if (context.Channel.Users.Count != 2)
            {
                await context.EditResponseAsync(
                    new DiscordWebhookBuilder()
                    .WithContent("You must message me privately to perform that action"));
                return;
            }
        }
    }
}
