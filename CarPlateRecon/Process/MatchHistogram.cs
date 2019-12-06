using OpenCvSharp;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Resources;
using System.Collections;
using System.Globalization;
using System.Diagnostics;

namespace CarPlateRecon.Process
{
    public class MatchHistogram
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

        public void test()
        {
            ResourceSet rsrcSet = CarPlateRecon.Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentCulture, false, true);

            foreach (DictionaryEntry entry in rsrcSet)
            {
                Object name = entry.Key;
                Object resource = entry.Value;
                Debug.WriteLine(name + " [  ] " + resource);
            }
        }
    }
}
