using System.Drawing;

namespace WFC_Godot.API.Services
{
    public class ImageHelper
    {
        public static Image ConvertFileToImage(IFormFile image)
        {
            using (var memoryStream = new MemoryStream())
            {
                image.CopyTo(memoryStream);

                var processedImage = Image.FromStream(memoryStream);
                return processedImage;
            }
        }

        public static byte[,,] ConvertImageToBitArrayForPython(Image image)
        {
            Bitmap bitmap = new Bitmap(image);

            byte[,,] pixelData = new byte[bitmap.Width, bitmap.Height, 4];

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color pixelColor = bitmap.GetPixel(x, y);


                    pixelData[x, y, 0] = pixelColor.R;
                    pixelData[x, y, 1] = pixelColor.G;
                    pixelData[x, y, 2] = pixelColor.B;
                    pixelData[x, y, 3] = pixelColor.A;
                }
            }

            return pixelData;
        }
    }
}
