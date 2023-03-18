using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterCode.Models
{
    // Represents a player in the context of a particular game; whenever a discord user joins a new game, a new GamePlayer is created for them
    [FirestoreData]
    internal class GamePlayer
    {
        [FirestoreProperty]
        public ulong DiscordUserId { get; set; }

        [FirestoreProperty]
        public Team Team { get; set; } = Team.Unknown;

        [FirestoreProperty]
        public PlayerSpecialAbility SpecialAbility { get; set; } = PlayerSpecialAbility.None;

        [FirestoreProperty]
        public DocumentReference? Scanned { get; set; } = null;
    }
}
