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
        public bool IsStarted { get; set; } = false;

        [FirestoreProperty]
        public bool IsEnded { get; set; } = false;

        [FirestoreProperty]
        public List<GamePlayer> Players { get; set; } = new List<GamePlayer>();

        [FirestoreProperty]
        public List<Code> Codes { get; set; } = new List<Code>();

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
        public int CodesPerPlayer { get; set; } = 2;
    }
}
