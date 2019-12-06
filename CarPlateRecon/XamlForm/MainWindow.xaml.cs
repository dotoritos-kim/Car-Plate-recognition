using CarPlateRecon.Core.MatchHistogram;
using CarPlateRecon.Core.ProcessCore;
using Microsoft.Win32;
using OpenCvSharp;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CarPlateRecon
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : System.Windows.Window, IConvertTools
    {
        Mat Original = new Mat();
        Mat Result = new Mat();
        ConvertTools CVT = new ConvertTools();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog FindImage = new OpenFileDialog();
            FindImage.Multiselect = true;
            if (FindImage.ShowDialog() == true)
            {
                if (File.Exists(FindImage.FileName))
                {
                    processCall(FindImage.FileName);

                    foreach (string x in FindImage.FileNames)
                    {
                        PictureList.Items.Add(x);
                    }
                }
            }
        }

        private void processCall(string FileName)
        {

            Bitmap OriginalSource = new Bitmap(FileName);

            ProcessCore processClass = new ProcessCore();
            processClass.SetImage(CVT.ImageToByte(OriginalSource));

            MatchHistogram matchHistogram = new MatchHistogram();

            OriginalImage.Source = new BitmapImage(new Uri(FileName, UriKind.RelativeOrAbsolute));

            Result = processClass.ImageProcess();
            ResultImage.Source = CVT.BitmapToBitmapImage(CVT.MatToBitmap(Result));

            Original = processClass.OriginalImage;
        }

        private void OriginalImage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Cv2.ImShow("OriginalSource", Original);
        }

        private void ResultImage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Cv2.ImShow("Result", Result);
        }

        private void PictureList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (PictureList.Items.Count > 0)
            {
                processCall((string)PictureList.Items[PictureList.SelectedIndex]);
            }
        }


        public BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            return ((IConvertTools)CVT).BitmapToBitmapImage(bitmap);
        }

        public byte[] BitMapToByte(Bitmap bitmap)
        {
            return ((IConvertTools)CVT).BitMapToByte(bitmap);
        }

        public byte[] ImageToByte(Image img)
        {
            return ((IConvertTools)CVT).ImageToByte(img);
        }

        public Bitmap MatToBitmap(Mat Image)
        {
            return ((IConvertTools)CVT).MatToBitmap(Image);
        }
    }
}
