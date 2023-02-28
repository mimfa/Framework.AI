using MiMFa.Model;
using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.AI.Clustering
{
    public class HierarchicalStatic : HierarchicalBase
    {
        public double Threshould { get; set; }
        public HierarchicalStatic(Func<double, double, double> distanceFunc, double minimumDistance = 0) : base(distanceFunc)
        {
            Threshould = minimumDistance;
        }
        public HierarchicalStatic(double minimumDistance = 0, Func<double, double, double> distanceFunc = null) : this(distanceFunc, minimumDistance)
        {

        }

        protected override double ComputeThreshould(double[][] matrix, int rowIndex, int colIndex)
        {
            return Threshould;
        }
    }
}
