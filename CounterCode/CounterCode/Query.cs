using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CounterCode
{
    // Represents a Firestore query with a corresponding C# type for the resulting documents
    internal class Query<T>
    {
        public Query(Query firestoreQuery)
        {
            FirestoreQuery = firestoreQuery ?? throw new ArgumentNullException(nameof(firestoreQuery));
        }

        // Gets the underlying Firestore Query object
        public Query FirestoreQuery { get; }

        public Query<T> WhereEqualTo<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
            => new (FirestoreQuery.WhereEqualTo(property.GetPropertyInfo().Name, value));

        public Query<T> WhereNotEqualTo<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
            => new (FirestoreQuery.WhereNotEqualTo(property.GetPropertyInfo().Name, value));

        public Query<T> Limit(int limit) => new (FirestoreQuery.Limit(limit));

        public async Task<QuerySnapshot> GetSnapshotAsync() => await FirestoreQuery.GetSnapshotAsync();

        public AggregateQuery Count() => FirestoreQuery.Count();
    }
}
