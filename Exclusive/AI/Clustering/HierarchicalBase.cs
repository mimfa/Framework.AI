using MiMFa.Model;
using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.AI.Clustering
{
    public abstract class HierarchicalBase : EnumerableClusterBase<double>
    {
        public Func<double, double, double> ComputeDistance { get; set; } = null;
        public HierarchicalBase(Func<double, double, double> distanceFunc = null)
        {
            ComputeDistance = distanceFunc ?? new Func<double, double, double>((d1, d2) => Math.Abs(d1 - d2));
        }

        public virtual Route<int, double> Hierarchy(double[][] matrix)
        {
            var parent = new Route<int, double>(-1, 0.0d);
            for (int i = 0; i < matrix.Length; i++)
                for (int j = 0; j < matrix[i].Length; j++)
                    UpdateClusters(ComputeThreshould(matrix,i,j), parent, new Route<int, double>(i, matrix[i][j]));
            return parent;
        }
        public override IEnumerable<int> Clusters(IEnumerable<IEnumerable<double>> dataTable)
        {
            var matrix = Normalization(dataTable);
            var parent = Hierarchy(matrix);
            var f = false;
            for (int i = 0; i < matrix.Length; i++)
            {
                f = false;
                for (int c = 0; c < parent.Count; c++)
                    if (f = parent[c].Collection.Any(v => v.Key == i))
                    {
                        yield return c;
                        break;
                    }
                if (!f) yield return -1;
            }
        }
      
        public double ReduceClusters(Route<int, double> parent, double threshould )
        {
            double num = 0;
            if (parent.Count == 0) return num;
            num += ReduceClusters(parent[0], threshould) /10;
            for (int i = 1; i < parent.Count; i++)
            {
                var dist = ComputeDistance(parent[i].Value, parent[i - 1].Value);
                if (dist < threshould) threshould = dist;
                num += ReduceClusters(parent[i], threshould) /10;
            }
            for (int i = parent.Count - 1; i > 0; i--)
            {
                var dist = ComputeDistance(parent[i].Value, parent[i - 1].Value);
                if (dist <= threshould)
                {
                    parent[i].Add(parent[i - 1]);
                    parent.RemoveAt(i - 1);
                    num++;
                }
            }
            return num;
        }
        protected void UpdateClusters(double minDist, Route<int, double> parent, Route<int, double> route)
        {
            int minDistInd = -1;
            for (int i = 0; i < parent.Count; i++)
            {
                var dist = ComputeDistance(parent[i].Value, route.Value);
                if (ComparerDistance(parent, route, dist, minDist))
                {
                    minDist = dist;
                    minDistInd = i;
                }
            }
            if (minDistInd < 0) parent.Add(route);
            else UpdateClusters(minDist, parent[minDistInd], route);
        }

        protected virtual double ComputeThreshould(double[][] matrix, int rowIndex, int colIndex)
        {
            return 0;
        }
        protected virtual bool ComparerDistance(Route<int, double> parent, Route<int, double> route, double dist, double minDist)=> dist < minDist;

        public override double[][] Normalization(IEnumerable<IEnumerable<double>> rawData)
        {
            List<double[]> matrix = new List<double[]>();
            int i = 0;
            foreach (var itemi in rawData)
            {
                int j = 0;
                var ds = new List<double>();
                foreach (var itemj in itemi)
                    if (i == j++) break;
                    else ds.Add(itemj);
                matrix.Add(ds.ToArray());
                i++;
            }
            return matrix.ToArray();
        }
    }
}
