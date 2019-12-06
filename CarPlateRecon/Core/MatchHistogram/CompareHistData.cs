using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarPlateRecon.Core.DataClass
{
    public class CompareHistData
    {
        public Mat src_img = new Mat();
        public List<Mat> Compare_img = new List<Mat>();
        public List<double> CompareHist_result = new List<double>();
    }
}
