using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OpenCvSharp;


namespace CarPlateRecon
{
    public class ProcessClass : IDisposable
    {

        public byte[] OriginalSource;
        public Mat OriginalImage;
        public ProcessClass(byte[] OriginalSource)
        {
            this.OriginalSource = OriginalSource;
            this.OriginalImage = Cv2.ImDecode(OriginalSource, ImreadModes.Color);
        }

        public Mat ImageProcess()
        {

            Mat GrayImage = new Mat();
            using Mat AdaptiveImage = new Mat();
            Mat PlateTmpImage = new Mat(OriginalImage.Size(), MatType.CV_8UC1);
            using Mat Dilate = new Mat();
            using Mat Erode = new Mat();
            using Mat SnakePlate = new Mat();
            using var ElementSize = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(1, 1));

            Cv2.CvtColor(OriginalImage, GrayImage, ColorConversionCodes.RGB2GRAY);
            Cv2.FastNlMeansDenoising(GrayImage, GrayImage);
            Cv2.AdaptiveThreshold(GrayImage, AdaptiveImage, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.BinaryInv, 25, -1);
            Cv2.Dilate(AdaptiveImage, Dilate, ElementSize);
            Cv2.Erode(Dilate, Erode, ElementSize);

            Erode.CopyTo(SnakePlate);

            Point[][] contours;
            HierarchyIndex[] hierarchyIndexes;
            Cv2.FindContours(
                SnakePlate,
                out contours,
                out hierarchyIndexes,
                mode: RetrievalModes.CComp,
                method: ContourApproximationModes.ApproxSimple);
            List<Rect> SortPoint = new List<Rect>();


            using Mat SnakePlateRGB = new Mat();
            Cv2.CvtColor(SnakePlate, SnakePlateRGB, ColorConversionCodes.GRAY2BGR);


            for (int i = 0; i < contours.Length; i++)
            {
                var contour = contours[i];
                var boundingRect = Cv2.BoundingRect(contour);
                if (
                boundingRect.X > (SnakePlate.Width / 20) * 3 &&
                boundingRect.Y > (SnakePlate.Height / 20) * 6 &&
                boundingRect.X < (SnakePlate.Width / 20) * 17 &&
                boundingRect.Y < (SnakePlate.Height / 20) * 14 &&
                (SnakePlate.Width / 20) * 3 < boundingRect.X + boundingRect.Width &&
                (SnakePlate.Height / 20) * 6 < boundingRect.Y + boundingRect.Height &&
                (SnakePlate.Width / 20) * 17 > boundingRect.X + boundingRect.Width &&
                (SnakePlate.Height / 20) * 14 > boundingRect.Y + boundingRect.Height &&
                boundingRect.Width < 100 &&
                boundingRect.Height < 200 &&
                boundingRect.Width * boundingRect.Height > 400 &&
                boundingRect.Width * boundingRect.Height < 10000
                )
                {
                    SortPoint.Add(boundingRect);
                    Cv2.Rectangle(SnakePlateRGB,
                        new Point(boundingRect.X, boundingRect.Y),
                        new Point(boundingRect.X + boundingRect.Width, boundingRect.Y + boundingRect.Height),
                        new Scalar(0, 0, 255),
                        2);
                }
            }

            for (int i = 0; i < SortPoint.Count; i++)
            {
                for (int j = 0; j < (SortPoint.Count - 1) - i; j++)
                {
                    var temp_rect = SortPoint[j];
                    SortPoint[j] = SortPoint[j + 1];
                    SortPoint[j + 1] = temp_rect;
                }
            }
            int count = 0;
            double gradient = 0;
            int friend_count = 0;
            int selected = 0;
            List<Rect> FindRect = new List<Rect>();
            for (int i = 0; i < SortPoint.Count; i++)
            {
                for (int j = i + 1; j < SortPoint.Count; j++)
                {
                    int Delta_x = Math.Abs(SortPoint[j].TopLeft.X - SortPoint[i].TopLeft.X);

                    if (Delta_x > 200)
                        break;

                    int Delta_y = Math.Abs(SortPoint[j].TopLeft.Y - SortPoint[i].TopLeft.Y);

                    if (Delta_x == 0)
                    {
                        Delta_x = 1;
                    }
                    if (Delta_y == 0)
                    {
                        Delta_y = 1;
                    }

                    gradient = Delta_y / Delta_x;

                    if (gradient < 0.15)
                    {
                        count += 1;
                    }
                    if (count > friend_count)
                    {

                        selected = i;
                        friend_count = count;
                        Cv2.Rectangle(SnakePlateRGB, SortPoint[selected], new Scalar(0, 0, 255), 2);
                        int plate_width = Delta_x;
                        Cv2.Line(SnakePlateRGB,
                            new Point(SortPoint[selected].TopLeft.X, SortPoint[selected].TopLeft.Y),
                            new Point(SortPoint[selected].TopLeft.X + plate_width, SortPoint[selected].TopLeft.Y),
                            new Scalar(255, 0, 0),
                            2
                            );
                        FindRect.Add(new Rect(SortPoint[selected].TopLeft.X, SortPoint[selected].TopLeft.Y, plate_width - (SortPoint[selected].Width), SortPoint[selected].Height));
                    }
                }
            }

            Mat temp1 = new Mat(OriginalImage.Rows, OriginalImage.Cols, OriginalImage.Type()).EmptyClone();
            int frch = 0;

            foreach (Rect tmp in FindRect)
            {
                if (
                       0 <= tmp.X
                       && 0 <= tmp.Width
                       && tmp.X + tmp.Width <= OriginalImage.Cols
                       && 0 <= tmp.Y
                       && 0 <= tmp.Height
                       && tmp.Y + tmp.Height <= OriginalImage.Rows
                       )
                {
                    var ss = new Mat(OriginalImage, tmp);
                    var roi = new Mat(temp1, new Rect(tmp.X, tmp.Y, ss.Width, ss.Height));
                    ss.CopyTo(roi);

                    frch++;
                }
            }

            Cv2.ImShow("ㅁㄴㅇㄹ2", temp1);
            Cv2.ImShow("ㅁㄴㅇㄹ3", GrayImage);
           



            Mat ReturnMat = new Mat();
            SnakePlateRGB.CopyTo(ReturnMat);
            return ReturnMat;
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
