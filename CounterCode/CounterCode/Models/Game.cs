using DSharpPlus.Entities;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterCode.Models
{
    // Represents a game of CounterCode
    [FirestoreData]
    internal class Game : FirestoreDocument
    {

        [FirestoreProperty]
        public string Name { get; set; } = "Unknown";

        // Id of the Guild (discord 'server') in which the game is being played
        [FirestoreProperty]
        public ulong GuildId { get; set; } = 0;

        [FirestoreProperty]
        public GameState GameState { get; set; } = GameState.Setup;

        public CollectionReference? GamePlayers => DocumentId?.Collection(nameof(GamePlayers));
        public CollectionReference? HandInEvents => DocumentId?.Collection(nameof(HandInEvents));

        [FirestoreProperty]
        public List<Code> Codes { get; set; } = new List<Code>();

        // Discord user ids of players in this game. Denormalised so we can query which games a player is in
        [FirestoreProperty]
        public List<ulong> PlayerDiscordIds { get; set; } = new List<ulong>();

        // Number of Evil players in the game. When created this defaults to -1, meaning that a default proportion of players will be evil.
        // Once the game has started if the default proportion was used this value will be updated to reflect the actual number of evil players.
        [FirestoreProperty]
        public int NumEvilPlayers { get; set; } = -1;

        // Number of Evil players who are Infiltrators. When created this defaults to -1, meaning that a default proportion of Evil players will be
        // Infiltrators. Once the game has started if the default proportion was used this value will be updated to reflect the actual number of
        // Infiltrators.
        [FirestoreProperty]
        public int NumInfiltrators { get; set; } = -1;

        [FirestoreProperty]
        public int CodesPerPlayer { get; set; } = 4;

        [FirestoreProperty]
        public HandInType HandInType { get; set; } = HandInType.TwoCodes;

        public Query<GamePlayer> GamePlayersQuery() => new(DocumentId!.Collection(nameof(GamePlayers)));
    }
}
