﻿using Google.Cloud.Firestore;
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
        public DocumentReference? DocumentId { get; set; }
    }
}
