using Python.Runtime;
using System.Drawing;
using WFC_Godot.API.Model;
using WFC_Godot.API.Services;

namespace WFC_Godot.API.Jobs
{
    public class CreateModelJob
    {
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public async Task Exec(IFormFile file, IEnumerable<TileDescription> tilesDescriptions, int tileSize)
        {
            await semaphore.WaitAsync();

            try
            {
                var tilesSides = tilesDescriptions.Select(x=>x.SidesKind.ToArray()).ToArray();
                var tilesCorners = tilesDescriptions.Select(x=>x.CornersKind.ToArray()).ToArray();
                var image = ImageHelper.ConvertFileToImage(file);
                var imageBitMap = ImageHelper.ConvertImageToBitArrayForPython(image);
                using (Py.GIL())
                {
                    var classifier = Py.Import("ModelClassifier");
                    var pythonImage = imageBitMap.ToPython();
                    var sidesPython = tilesSides.ToPython();
                    var cornerPython = tilesCorners.ToPython();

                    var preparedData = classifier.InvokeMethod("DataPreparation", new[] { pythonImage, sidesPython, cornerPython, tileSize.ToPython() });
                    var outputString = classifier.InvokeMethod("TrainModels", preparedData);
                }
            }
            finally
            { 
               semaphore.Release();
            }
        }

        public static bool IsUnderway()
        {
           return semaphore.CurrentCount == 0;
        }


    }
}
