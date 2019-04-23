using Microsoft.Win32;
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
    public partial class MainWindow : System.Windows.Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog FindImage = new OpenFileDialog();

            if (FindImage.ShowDialog() == true)
            {
                if (File.Exists(FindImage.FileName))
                {
                    using Bitmap OriginalSource = new Bitmap(FindImage.FileName);
                    using ConvertTools CVT = new ConvertTools();
                    using ProcessClass processClass = new ProcessClass(CVT.ImageToByte(OriginalSource));
                    OriginalImage.Source = new BitmapImage(new Uri(FindImage.FileName, UriKind.RelativeOrAbsolute));
                    ResultImage.Source = CVT.BitmapToBitmapImage(CVT.MatToBitmap(processClass.ImageProcess()));
                }
            }
        }
    }
}
