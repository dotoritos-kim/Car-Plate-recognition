using OpenCvSharp;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using CarPlateRecon.Core.DataClass;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace CarPlateRecon.Core.MatchHistogram
{

    public class MatchHistogram : abstract_MatchHistogram, IConvertTools
    {

        public List<CompareHistData> CompareHist_sum = new List<CompareHistData>();
        public List<Mat> srcHistogram = new List<Mat>();

        public MatchHistogram()
        {
            String FolderName = "./";
            DirectoryInfo di = new DirectoryInfo(FolderName);
            foreach (FileInfo File in di.GetFiles())
                if (File.Extension.ToLower().CompareTo(".png") == 0)
                    srcHistogram.Add(Cv2.ImRead(File.FullName, ImreadModes.Grayscale));
        }

        public override double compareHistogram(Mat[] mats)
        {
            Parallel.For(0, srcHistogram.Count, (i) =>
            {
                CompareHistData histData = new CompareHistData();
                histData.src_img = srcHistogram[i];
                Parallel.For(0, mats.Length, (j) =>
                {
                    histData.Compare_img.Add(mats[j]);
                    histData.CompareHist_result.Add(Cv2.CompareHist(srcHistogram[i], mats[j], HistCompMethods.Bhattacharyya));
                    
                });
                CompareHist_sum.Add(histData);
            });
            return 0;
        }

        public override double MaxHist(Mat[] mats)
        {
            int Maxindex = 0;
            double Max = 0;
            for (int i = 0; i < CompareHist_sum.Count; i++)
            {
                if (Max < Convert.ToDouble(CompareHist_sum[i].CompareHist_result))
                {
                    Max = Convert.ToDouble(CompareHist_sum[i].CompareHist_result);
                    Maxindex = i;
                }
            }
            return Max;
        }

        public BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            throw new NotImplementedException();
        }

        public byte[] BitMapToByte(Bitmap bitmap)
        {
            throw new NotImplementedException();
        }

        public byte[] ImageToByte(Image img)
        {
            throw new NotImplementedException();
        }

        public Bitmap MatToBitmap(Mat Image)
        {
            throw new NotImplementedException();
        }
    }
}
