using MiMFa.Model;
using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.AI.Clustering
{
    public class HierarchicalMean : HierarchicalBase
    {
        public HierarchicalMean(Func<double, double, double> distanceFunc = null):base(distanceFunc)
        {
        }

        public double LastThreshould = 0;
        protected override double ComputeThreshould(double[][] matrix, int rowIndex, int colIndex)
        {
            if (colIndex > 0) return LastThreshould;
            return LastThreshould = matrix[rowIndex].Average();
        }
    }
}
