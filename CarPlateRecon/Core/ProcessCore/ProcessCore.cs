using System;
using System.Drawing;
using System.Windows.Media.Imaging;
using CarPlateRecon.Core.GetPlate;
using CarPlateRecon.Core.Roi;
using OpenCvSharp;


namespace CarPlateRecon.Core.ProcessCore
{

    public class ProcessCore : abstract_ProcessCore, IConvertTools
    {
        public byte[] OriginalSource;
        public Mat OriginalImage;

        private ConvertTools convertTools = new ConvertTools();

        public ProcessCore()
        {
            
        }

        public override void SetImage(byte[] OriginalSource)
        {
            this.OriginalSource = OriginalSource;
            this.OriginalImage = Cv2.ImDecode(OriginalSource, ImreadModes.Color);
        }

        public override Mat ImageProcess()
        {
            Mat GrayImage = new Mat();
            Cv2.CvtColor(OriginalImage, GrayImage, ColorConversionCodes.RGB2GRAY);

            ProcessPlate processPlate = new ProcessPlate(GrayImage);

            using var SnakePlate = processPlate.GetSnakePlate();



            OpenCvSharp.Point[][] contours;
            HierarchyIndex[] hierarchyIndexes;

            Cv2.FindContours(
                SnakePlate,
                out contours,
                out hierarchyIndexes,
                mode: RetrievalModes.CComp,
                method: ContourApproximationModes.ApproxTC89L1);

            Region_of_Interest region = new Region_of_Interest(
                OriginalImage,
                SnakePlate,
                processPlate.SnakeRGB,
                contours
                );
            using var tmp = new Mat();

            Mat Confirmed = region.GetRegion();

            Cv2.ImShow("Result1", Confirmed);
            Cv2.CvtColor(Confirmed, tmp, ColorConversionCodes.RGB2GRAY);
            Cv2.Threshold(tmp, tmp, 100, 255, ThresholdTypes.Binary);
            Cv2.ImShow("Result?", tmp);
            Cv2.ImShow("Result2", processPlate.SnakeRGB);

            return Confirmed;
        }
        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면

        ~ProcessCore()
        {

        }


        public BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            return ((IConvertTools)convertTools).BitmapToBitmapImage(bitmap);
        }

        public byte[] BitMapToByte(Bitmap bitmap)
        {
            return ((IConvertTools)convertTools).BitMapToByte(bitmap);
        }

        public byte[] ImageToByte(Image img)
        {
            return ((IConvertTools)convertTools).ImageToByte(img);
        }

        public Bitmap MatToBitmap(Mat Image)
        {
            return ((IConvertTools)convertTools).MatToBitmap(Image);
        }
        #endregion

    }
}
