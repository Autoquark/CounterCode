using CounterCode.Models;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterCode
{
    // Terribly named class
    static internal class SharedLogic
    {
        public static async Task SendStartOfGameInfo(Game game, GamePlayer gamePlayer)
        {
            DiscordGuild Guild = await Program.DiscordClient.GetGuildAsync(game.GuildId);
            DiscordMember Member = await Guild.GetMemberAsync(gamePlayer.DiscordUserId);

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"The CounterCode game called \"{game.Name}\" has begun!");
            stringBuilder.AppendLine($"You are on the {gamePlayer.Team} Team");
            if(gamePlayer.SpecialAbility == PlayerSpecialAbility.Infiltrator)
            {
                stringBuilder.AppendLine("You are an Infiltrator - if a Good player scans you, you will appear Good!");
            }

            stringBuilder.AppendLine("Your secret codes are " + string.Join(", ", game.Codes.Where(x => x.OwningPlayer == gamePlayer.DocumentId).Select(x => x.Text)));

            await Member.SendMessageAsync(stringBuilder.ToString());
        }
    }
}
