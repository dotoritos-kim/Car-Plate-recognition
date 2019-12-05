using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.Blob;


namespace CarPlateRecon
{
    public class ProcessClass : IDisposable
    {

        public byte[] OriginalSource;
        public Mat OriginalImage;
        public ProcessClass()
        {

        }

        public void SetImage(byte[] OriginalSource)
        {
            this.OriginalSource = OriginalSource;
            this.OriginalImage = Cv2.ImDecode(OriginalSource, ImreadModes.Color);
        }

        public Mat ImageProcess()
        {
            ConvertTools convertTools = new ConvertTools();
            Mat GrayImage = new Mat();
            Cv2.CvtColor(OriginalImage, GrayImage, ColorConversionCodes.RGB2GRAY);

            ProcessPlate processPlate = new ProcessPlate(GrayImage);

            using var SnakePlate = processPlate.GetSnakePlate();



            Point[][] contours;
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
        private void TemplateMatching(Mat src,Mat mask)
        {
            using Mat ScreenMat = src;
            using Mat FindMat = mask;
            using Mat res = ScreenMat.MatchTemplate(FindMat, TemplateMatchModes.CCoeffNormed);

            double minval, maxval = 0;
            Point minloc, maxloc;
            Cv2.MinMaxLoc(res, out minval, out maxval, out minloc, out maxloc);
            Debug.WriteLine("찾은 이미지의 유사도 : " + maxval);

            Cv2.Rectangle(src, new Rect(maxloc, mask.Size()), Scalar.Red);
            Console.WriteLine("minLoc: {0} maxLoc: {1}, maxVal: {2}", minloc, maxloc, maxval);
        }


        #region IDisposable Support
        private bool disposedValue = false; // 중복 호출을 검색하려면
        ~ProcessClass()
        {
            this.Dispose(false);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 관리되는 상태(관리되는 개체)를 삭제합니다.
                }

                // TODO: 관리되지 않는 리소스(관리되지 않는 개체)를 해제하고 아래의 종료자를 재정의합니다.
                // TODO: 큰 필드를 null로 설정합니다.

                disposedValue = true;
            }
        }

        // TODO: 위의 Dispose(bool disposing)에 관리되지 않는 리소스를 해제하는 코드가 포함되어 있는 경우에만 종료자를 재정의합니다.
        // ~ProcessClass()
        // {
        //   // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
        //   Dispose(false);
        // }

        // 삭제 가능한 패턴을 올바르게 구현하기 위해 추가된 코드입니다.
        public void Dispose()
        {
            // 이 코드를 변경하지 마세요. 위의 Dispose(bool disposing)에 정리 코드를 입력하세요.
            Dispose(true);
            // TODO: 위의 종료자가 재정의된 경우 다음 코드 줄의 주석 처리를 제거합니다.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }

}
