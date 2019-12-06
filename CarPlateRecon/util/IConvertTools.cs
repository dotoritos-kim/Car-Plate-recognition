using System.Drawing;
using System.Windows.Media.Imaging;
using OpenCvSharp;

namespace CarPlateRecon
{
    public interface IConvertTools
    {
        BitmapImage BitmapToBitmapImage(Bitmap bitmap);
        byte[] BitMapToByte(Bitmap bitmap);
        byte[] ImageToByte(Image img);
        Bitmap MatToBitmap(Mat Image);
    }
}