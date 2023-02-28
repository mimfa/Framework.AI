using MiMFa.Model;
using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.AI.Clustering
{
    public class HierarchicalVariance : HierarchicalBase
    {
        public HierarchicalVariance(Func<double, double, double> distanceFunc = null) :base(distanceFunc)
        {
        }

        public double LastMaximumThreshould = 0;
        public double LastMinimumThreshould = 0;
        protected override double ComputeThreshould(double[][] matrix, int rowIndex, int colIndex)
        {
            if (colIndex > 0) return LastMaximumThreshould;
            return LastMaximumThreshould = (LastMinimumThreshould = matrix[rowIndex].Min()) + MathService.Variance(matrix[rowIndex]);
        }
        protected override bool ComparerDistance(Route<int, double> parent, Route<int, double> route, double dist, double minDist)
        {
            return base.ComparerDistance(parent, route, dist, minDist) && dist <= LastMaximumThreshould && dist >= LastMinimumThreshould;
        }
    }
}
