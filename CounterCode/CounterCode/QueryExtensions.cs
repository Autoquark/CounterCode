using CounterCode.Models;
using Google.Api;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CounterCode
{
    internal static class QueryExtensions
    {
        // Returns a query that will return all Games
        public static Query<Game> Games(this FirestoreDb db) => new(db.Collection("Games"));
        public static Query<Game> FinishedGames(this FirestoreDb db) => new(db.Collection("FinishedGames"));

        // Returns a query that will return all GamePlayers from all games
        public static Query<GamePlayer> GamePlayers(this FirestoreDb db) => new(db.CollectionGroup(nameof(Game.GamePlayers)));

        // Fetches a discord user's current game from the database, or null if they have no active game
        public static async Task<Game?> GetUserCurrentGame(this FirestoreDb db, ulong discordUserId)
        {
            return (await db.GamePlayers()
                .WhereEqualTo(x => x.DiscordUserId, discordUserId)
                .Limit(1)
                .GetSnapshotAsync())
                .SingleOrDefault()
                ?.ConvertTo<Game>();
        }
    }
}
