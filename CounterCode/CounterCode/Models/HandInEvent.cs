using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterCode.Models
{
    internal class HandInEvent : FirestoreDocument
    {
        [FirestoreProperty]
        public DocumentReference? Player { get; set; }

        [FirestoreProperty]
        public DocumentReference? Code1 { get; set; }

        [FirestoreProperty]
        public DocumentReference? Code2 { get; set; }
    }
}
