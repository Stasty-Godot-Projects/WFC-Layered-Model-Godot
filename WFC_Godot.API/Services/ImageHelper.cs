using Python.Runtime;
using System.Drawing;

namespace WFC_Godot.API.Services
{
    public class ImageHelper
    {
        public static Image ConvertFileToImage(byte[] image)
        {
            using (var memoryStream = new MemoryStream(image))
            {

                var processedImage = Image.FromStream(memoryStream);
                return processedImage;
            }
        }

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

        public static PyList ConvertImageToPython(byte[,,] image)
        {
            var pythonImage = new PyList();
            for (int i = 0; i < image.GetLength(0); i++)
            {
                var sublist = new PyList();
                for (int j = 0; j < image.GetLength(1); j++)
                {
                    var subSubList = new PyList();
                    for (int z = 0; z < 4; z++)
                    {
                        sublist.Append(image[i, j, z].ToPython());
                    }
                    sublist.Append(subSubList);
                }
                pythonImage.Append(sublist);
            }
            return pythonImage;
        }
    }
}
