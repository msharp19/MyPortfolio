using Accord.Imaging;
using Accord.Imaging.Converters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNNV2
{
    public class ImageFunctions
    {
        public static Bitmap ToBitmap(double[,] rawImage)
        {
            MatrixToImage mtrxToImage = new MatrixToImage();
            UnmanagedImage outputImage = null;
            mtrxToImage.Convert(rawImage, out outputImage);
            return outputImage.ToManagedImage();
        }

        public static double[,] ConvertFlatArrayToMatrix(IList<int> rawValues, int width, int height)
        {
            double[,] multiArray = new double[width, height];
            int flatCounter = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    multiArray[i, j] = rawValues[flatCounter];
                    flatCounter++;
                }
            }
            return multiArray;
        }

        public static byte[] ImageToByte(System.Drawing.Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        public static Bitmap Base64ToImage(string base64String)
        {
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
                return new Bitmap(image);
            }
        }

        public static byte[] ImageToRepresentativeBytes(Bitmap img)
        {
            var bytes = new byte[img.Height*img.Width];
            int flatCount = 0;
            for(int i=0;i< img.Height;i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    var a = img.GetPixel(j, i).A > 0 ? 255 : 0;
                    bytes[flatCount] = ((byte)(a));
                    flatCount++;
                }
            }
            return bytes;
        }

        public static Bitmap Resize(Bitmap image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }
    }
}
