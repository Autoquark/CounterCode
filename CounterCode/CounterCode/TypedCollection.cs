using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CounterCode
{
    internal class TypedCollection<T>
    {
        public TypedCollection(CollectionReference collectionReference)
        {
            Collection = collectionReference;
        }

        public CollectionReference Collection { get;  }
    }
}
