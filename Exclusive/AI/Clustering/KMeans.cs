using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.AI.Clustering
{
    public class KMeans : KBase
    {
        public KMeans(int clustersNumber=2) : base(clustersNumber)
        {
        }

        public override double[][] Normalization(IEnumerable<IEnumerable<double>> rawData)
        {
            double[][] result = base.Normalization(rawData);

            for (int j = 0; j < result[0].Length; ++j)
            {
                double colSum = 0.0;
                for (int i = 0; i < result.Length; ++i)
                    colSum += result[i][j];
                double mean = colSum / result.Length;
                double sum = 0.0;
                for (int i = 0; i < result.Length; ++i)
                    sum += (result[i][j] - mean) * (result[i][j] - mean);
                double sd = sum / result.Length;
                for (int i = 0; i < result.Length; ++i)
                    result[i][j] = (result[i][j] - mean) / sd;
            }
            return result;
        }
        protected override bool UpdateCenters(double[][] data, int[] clustering, double[][] centers)
        {
            int numClusters = centers.Length;
            int[] clusterCounts = new int[numClusters];
            for (int i = 0; i < data.Length; ++i)
            {
                int cluster = clustering[i];
                ++clusterCounts[cluster];
            }

            for (int k = 0; k < numClusters; ++k)
                if (clusterCounts[k] == 0)
                    return false;

            for (int k = 0; k < centers.Length; ++k)
                for (int j = 0; j < centers[k].Length; ++j)
                    centers[k][j] = 0.0;

            for (int i = 0; i < data.Length; ++i)
            {
                int cluster = clustering[i];
                for (int j = 0; j < data[i].Length; ++j)
                    centers[cluster][j] += data[i][j]; // accumulate sum
            }

            for (int k = 0; k < centers.Length; ++k)
                for (int j = 0; j < centers[k].Length; ++j)
                    centers[k][j] /= clusterCounts[k]; // danger of div by 0
            return true;
        }
    }
}
