using MiMFa.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.AI.Clustering
{
    public abstract class ClusterBase<TValues>
    {
        public virtual IEnumerable<KeyValuePair<T, F>[]> Cluster<T, F>(KeyValuePair<T, F>[] data)
            => from v in Cluster(data, (from v in data select v.Value).Cast<TValues>()) select v.ToArray();
        public virtual IEnumerable<SmartKeyValue<T, F>[]> Cluster<T, F>(SmartKeyValue<T, F>[] data)
            => from v in Cluster(data, (from v in data select v.Value).Cast<TValues>()) select v.ToArray();
        public virtual IEnumerable<T[]> Cluster<T>(T[] dataArray)
            => from v in Cluster(dataArray, dataArray.Cast<TValues>()) select v.ToArray();
        public virtual IEnumerable<IList<T>> Cluster<T>(IList<T> dataArray)
            => from v in Cluster(dataArray, dataArray.Cast<TValues>()) select v.ToList();

        public virtual IEnumerable<IEnumerable<KeyValuePair<T, F>>> Cluster<T, F>(IEnumerable<KeyValuePair<T, F>> data)
            => Cluster(data, (from v in data select v.Value).Cast<TValues>());
        public virtual IEnumerable<IEnumerable<SmartKeyValue<T, F>>> Cluster<T, F>(IEnumerable<SmartKeyValue<T, F>> data)
            => Cluster(data, (from v in data select v.Value).Cast<TValues>());
        public virtual IEnumerable<IEnumerable<TValues>> Cluster(IEnumerable<TValues> dataArray)
            => Cluster(dataArray, dataArray);

        public virtual int SetCluster(int[] clustersList, int clusterIndex, int newclusterIndex)
        {
            int num = 0;
            for (int i = 0; i < clustersList.Length; i++)
                if (clustersList[i] == clusterIndex)
                {
                    clustersList[i] = newclusterIndex;
                    num++;
                }
            return num;
        }
        public virtual IEnumerable<T> GetCluster<T>(IEnumerable<T> data, int[] clustersList, int clusterIndex)
        {
            for (int i = 0; i < clustersList.Length; i++)
                if (clustersList[i] == clusterIndex)
                    yield return data.ElementAt(i);
        }
        public virtual IEnumerable<IEnumerable<T>> Cluster<T>(IEnumerable<T> data, IEnumerable<TValues> dataValues)
        {
            var clustersList = Clusters(dataValues).ToArray();
            for (int c = 0; c <= clustersList.Max(); c++)
                yield return GetCluster(data, clustersList, c);
        }
     
        public virtual IEnumerable<int> Clusters(IEnumerable<TValues> values)
        {
            throw new NotImplementedException();
        }

        public virtual TValues[] Normalization(IEnumerable<TValues> values) => values.ToArray();
    }
}
