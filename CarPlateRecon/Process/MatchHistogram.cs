using OpenCvSharp;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.Collections;
using System.Globalization;

namespace CarPlateRecon.Process
{
    class MatchHistogram
    {
        ConvertTools ConvertTool = new ConvertTools();
        Mat Histogram = new Mat();
        List<double> CompareHist_sum = new List<double>();
        public MatchHistogram()
        {
            Histogram = Mat.FromImageData(ConvertTool.BitMapToByte(Properties.Resources.Plate1));
        }
        public double compareHistogram(Mat[] mats)
        {
            Parallel.For(0, mats.Length, (i) => {
                CompareHist_sum.Add(Cv2.CompareHist(Histogram,mats[i],HistCompMethods.Bhattacharyya));
            });
            return 0;
        }
    }
}
