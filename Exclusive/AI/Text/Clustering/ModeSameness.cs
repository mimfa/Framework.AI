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
    public class ModeSameness : RelativeBasisSamenessBase
    {
        public ModeSameness(Func<string, string, double> sameness, double minimumSameness = 0) :base(sameness, minimumSameness)
        {
        }

        public override double ComputeCutPoint(double[] samenesses) => MathService.Mode(samenesses);
    }
}
