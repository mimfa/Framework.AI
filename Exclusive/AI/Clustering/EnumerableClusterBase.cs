using MiMFa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.AI.Clustering
{
    public abstract class EnumerableClusterBase<TValue> : ClusterBase<IEnumerable<TValue>>
    {
        public virtual IEnumerable<KeyValuePair<T, F[]>[]> Cluster<T, F>(KeyValuePair<T, F[]>[] data)
            => from v in Cluster(data, from v in data select v.Value.Cast<TValue>()) select v.ToArray();
        public virtual IEnumerable<IList<KeyValuePair<T, IList<F>>>> Cluster<T, F>(IList<KeyValuePair<T, IList<F>>> data)
            => from v in Cluster(data, from v in data select v.Value.Cast<TValue>()) select v.ToList();
        public virtual IEnumerable<T[][]> Cluster<T, F>(T[][] data)
            => from v in Cluster(data.AsEnumerable(), from v in data select v.Cast<TValue>()) select v.ToArray();
        public virtual IEnumerable<IList<IList<T>>> Cluster<T, F>(IList<IList<T>> data)
            => from v in Cluster(data.AsEnumerable(), from v in data select v.Cast<TValue>()) select v.ToList();


        public virtual IEnumerable<IEnumerable<Route<T, F>>> Cluster<T, F>(IEnumerable<Route<T, F>> route)
            => Cluster(route,from v in route select v.Values.Cast<TValue>());
        public virtual IEnumerable<IEnumerable<KeyValuePair<T, IEnumerable<F>>>> Cluster<T, F>(IEnumerable<KeyValuePair<T, IEnumerable<F>>> data)
            => Cluster(data,from v in data select v.Value.Cast<TValue>());
        public virtual IEnumerable<IEnumerable<IEnumerable<T>>> Cluster<T>(IEnumerable<IEnumerable<T>> dataTable)
            => Cluster(dataTable, from v in dataTable select v.Cast<TValue>());

        public new virtual TValue[][] Normalization(IEnumerable<IEnumerable<TValue>> rawData)
        {
            Matrix<TValue> result = new Matrix<TValue>();
            var maxind = 0;
            foreach (var item in rawData)
            {
                var arr = item.ToArray();
                maxind = Math.Max(maxind, arr.Length-1);
                result.AddY(arr);
            }
            while(maxind >= result.Count) result.Add(new SmartList<TValue>());
            return (from v in result let m = v[maxind] select v.ToArray() ).ToArray();
        }
    }
}
