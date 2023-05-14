using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.AI.Text.Processing
{
    public class English : Language
    {
        public English(string separator = "¶", string includePattern = "\\.pt$", string excludePattern = null) 
            : base(separator, includePattern, excludePattern)
        { }
    }
}
