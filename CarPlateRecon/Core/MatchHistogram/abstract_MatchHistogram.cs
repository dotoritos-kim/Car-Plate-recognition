using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarPlateRecon.Core.MatchHistogram
{
    public abstract class abstract_MatchHistogram
    {
        public abstract double MaxHist(Mat[] mats);
        public abstract double compareHistogram(Mat[] mats);
    }
}
