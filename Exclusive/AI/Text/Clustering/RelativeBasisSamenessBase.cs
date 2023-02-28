using MiMFa.Exclusive.AI.Clustering;
using MiMFa.Model;
using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.AI.Text.Clustering
{
    public class RelativeBasisSamenessBase : ClusterBase<string>
    {
        public double MinimumSameness { get; set; } = 0;
        public Func<string, string,double> Sameness { get; set; }

        public RelativeBasisSamenessBase(Func<string, string, double> sameness, double minimumSameness = 0)
        {
            Sameness = sameness;
            MinimumSameness = minimumSameness;
        }

        public override IEnumerable<int> Clusters(IEnumerable<string> texts)
        {
            List<List<int>> titles = new List<List<int>>();
            var ttls = Normalization(texts);
            for (int i = 0; i < ttls.Length; i++)
            {
                var item = ttls[i];
                var ind = titles.FindIndex(lst => lst.Any(v => ttls[v] == item));
                if (ind < 0)
                {
                    titles.Add(Sames(ttls, item, i).ToList().Distinct().ToList());
                    ind = titles.Count - 1;
                }
                else titles[ind] = titles[ind].Concat(Sames(ttls, item, i)).Distinct().ToList();
                yield return ind;
            }
        }


        protected IEnumerable<int> Sames(string[] ttls, string sample, int index)
        {
            yield return index;
            Dictionary<int, double> dic = new Dictionary<int, double>();
            for (int j = index + 1; j < ttls.Length; j++)
            {
                var s = Sameness(sample, ttls[j]);
                if (s > MinimumSameness) dic.Add(j, s);
            }
            if (dic.Count < 1) yield break;

            double thr = ComputeCutPoint(dic.Values.ToArray());
            foreach (var item in dic.Where(v => v.Value >= thr))
                foreach (var l in Sames(ttls, ttls[item.Key], item.Key))
                    yield return l;
        }

        public virtual double ComputeCutPoint(double[] samenesses)=> samenesses.Average();
    }
}
