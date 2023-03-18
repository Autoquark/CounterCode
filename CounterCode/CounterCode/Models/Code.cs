using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterCode.Models
{
    [FirestoreData]
    internal class Code
    {
        [FirestoreProperty]
        public string Text { get; set; } = "";

        [FirestoreProperty]
        public Team Team { get; set; } = Team.Unknown;

        // GamePlayer who handed this code in
        [FirestoreProperty]
        public DocumentReference? HandedInBy { get; set; } = null;

        // GamePlayer who compromised this code
        [FirestoreProperty]
        public DocumentReference? CompromisedBy { get; set; } = null;
    }
}
