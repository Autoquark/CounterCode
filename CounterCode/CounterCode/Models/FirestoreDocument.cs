using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterCode.Models
{
    [FirestoreData]
    internal class FirestoreDocument
    {
        [FirestoreDocumentId]
        private DocumentReference? _documentId { get; set; }

        public DocumentReference? DocumentId { get; }
    }
}
