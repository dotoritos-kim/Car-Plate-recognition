using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarPlateRecon
{
    public class Region_of_Interest
    {
        public Mat SnakePlate = new Mat();
        public Mat OriginalImage = new Mat();
        public Mat SnakeRGB = new Mat();
        List<Rect> SortPoint = new List<Rect>();
        List<Rect> FindRect = new List<Rect>();
        Point[][] contours;

        public Region_of_Interest(
            Mat OriginalImage, 
            Mat SnakePlate, 
            Mat SnakeRGB,
            Point[][] contours
            )
        {
            this.OriginalImage = OriginalImage;
            this.SnakePlate = SnakePlate;
            this.SnakeRGB = SnakeRGB;
            this.contours = contours;
        }
        public Mat GetRegion()
        {
            setContours();
            snakeGame();
            return setInterestRegion();
        }



        private void setContours()
        {
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
                boundingRect.Width > 3 &&
                boundingRect.Height > 3 &&
                boundingRect.Width < 300 &&
                boundingRect.Height < 200 &&
                boundingRect.Width * boundingRect.Height > 400
                )
                {
                    SortPoint.Add(boundingRect);
                    Cv2.Rectangle(SnakeRGB,
                        new Point(boundingRect.X, boundingRect.Y),
                        new Point(boundingRect.X + boundingRect.Width, boundingRect.Y + boundingRect.Height),
                        new Scalar(0, 0, 255),
                        2);
                }
            }
        }

        private void snakeGame()
        {
            int count = 0;
            int friend_count = 0;
            
            for (int i = 0; i < SortPoint.Count; i++)
            {
                for (int j = 0; j < (SortPoint.Count - 1) - i; j++)
                {
                    var temp_rect = SortPoint[j];
                    SortPoint[j] = SortPoint[j + 1];
                    SortPoint[j + 1] = temp_rect;
                }
            }

            
            for (int i = 0; i < SortPoint.Count; i++)
            {
                for (int j = i + 1; j < SortPoint.Count; j++)
                {
                    int Delta_x = Math.Abs(SortPoint[j].TopLeft.X - SortPoint[i].TopLeft.X);

                    if (Delta_x > 300)
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

                    double gradient = (double)Delta_y / Delta_x;

                    if (gradient < 0.15)
                    {
                        count += 1;
                    }
                    if (count > friend_count)
                    {
                        int selected = i;
                        friend_count = count;
                        Cv2.Rectangle(SnakeRGB, SortPoint[selected], new Scalar(0, 0, 255), 2);
                        int plate_width = Delta_x;
                        Cv2.Line(SnakeRGB,
                            new Point(SortPoint[selected].TopLeft.X, SortPoint[selected].TopLeft.Y),
                            new Point(SortPoint[selected].TopLeft.X + plate_width, SortPoint[selected].TopLeft.Y),
                            new Scalar(255, 0, 0),
                            2
                            );
                        FindRect.Add(new Rect(SortPoint[selected].TopLeft.X - 35, SortPoint[selected].TopLeft.Y, plate_width + 105, SortPoint[selected].Height));
                    }
                }
            }
        }

        private Mat setInterestRegion()
        {
            int frch = 0;
            Mat interestRegion = new Mat(OriginalImage.Rows, OriginalImage.Cols, OriginalImage.Type()).EmptyClone();
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
                    var roi = new Mat(interestRegion, new Rect(tmp.X, tmp.Y, ss.Width, ss.Height));
                    ss.CopyTo(roi);
                    frch++;
                }
            }
            return interestRegion;
        }
    }
}
