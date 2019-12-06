using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarPlateRecon.Core.GetPlate
{
    public class ProcessPlate : abstract_ProcessPlate
    {
        public Mat GrayImage = new Mat();
        public Mat AdaptiveImage = new Mat();
        public Mat EqualizeHist = new Mat();
        public Mat Dilate = new Mat();
        public Mat Erode = new Mat();
        public Mat SnakePlate = new Mat();
        public Mat SnakeRGB = new Mat();
        public Mat ElementSize = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(2, 2));

        public ProcessPlate(Mat GrayImage)
        {
            this.GrayImage = GrayImage;
        }

        public ProcessPlate(Mat GrayImage, Size ElementSize)
        {
            this.GrayImage = GrayImage;
            this.ElementSize = Cv2.GetStructuringElement(MorphShapes.Rect, ElementSize);
        }

        public ProcessPlate(Mat GrayImage, MorphShapes morphShapes, Size ElementSize)
        {
            this.GrayImage = GrayImage;
            this.ElementSize = Cv2.GetStructuringElement(morphShapes, ElementSize);
        }

        public override Mat GetSnakePlate()
        {
            Cv2.EqualizeHist(GrayImage, EqualizeHist);
            Cv2.FastNlMeansDenoising(EqualizeHist, GrayImage);
            Cv2.AdaptiveThreshold(GrayImage, AdaptiveImage, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 75, 3);
            Cv2.Dilate(AdaptiveImage, Dilate, ElementSize);
            Cv2.Erode(Dilate, Erode, ElementSize);
            Erode.CopyTo(SnakePlate);
            Cv2.MedianBlur(SnakePlate, SnakePlate,3);
            Cv2.CvtColor(SnakePlate, SnakeRGB, ColorConversionCodes.GRAY2BGR);
            return SnakePlate;
        }
    }
}
