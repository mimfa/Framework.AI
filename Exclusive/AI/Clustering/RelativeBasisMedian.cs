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
    public class RelativeBasisMedian : RelativeBasisBase<double>
    {
        public RelativeBasisMedian() : base()
        {

        }

        public override double ComputeCutPoint(double[] samenesses) => (samenesses.Max() + samenesses.Min()) / 2d;
    }
}
