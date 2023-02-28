using MiMFa.Exclusive.AI.Clustering;
using MiMFa.Model;
using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.AI.Clustering
{
    public class RelativeBasisBase<TValue> : EnumerableClusterBase<TValue> where TValue : IEquatable<TValue>, IComparable<TValue>
    {
        public RelativeBasisBase()
        {
        }

        public override IEnumerable<int> Clusters(IEnumerable<IEnumerable<TValue>> values)
        {
            List<List<int>> titles = new List<List<int>>();
            var ttls = (from v in Normalization(values) select v.ToArray()).ToArray();
            for (int i = 0; i < ttls.Length; i++)
            {
                var item = ttls[i];
                var ind = titles.FindIndex(lst => lst.Any(v => ttls[v] == item));
                if (ind < 0)
                {
                    titles.Add(Sames(ttls[i], i).ToList().Distinct().ToList());
                    ind = titles.Count - 1;
                }
                else titles[ind] = titles[ind].Concat(Sames(ttls[i], i)).Distinct().ToList();
                yield return ind;
            }
        }


        protected IEnumerable<int> Sames(TValue[] ttls, int index)
        {
            yield return index;

            TValue thr = ComputeCutPoint(ttls);
            for (int i = index+1; i < ttls.Length; i++)
                if (ttls[i].CompareTo(thr) >= 0)
                    foreach (var l in Sames(ttls, i))
                        yield return l;
        }

        public virtual TValue ComputeCutPoint(TValue[] samenesses)=> samenesses.Min();
    }
}
