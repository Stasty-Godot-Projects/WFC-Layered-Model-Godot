using Python.Runtime;
using WFC_Godot.API.Model;
using WFC_Godot.API.Services.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WFC_Godot.API.Services
{
    public class RecognitionService : IRecognitionService
    {
        public IEnumerable<TileDescription> RecognizeTiles(IFormFile file, int tileSize)
        {
            try
            {
                var image = ImageHelper.ConvertFileToImage(file);
                var imageBitMap = ImageHelper.ConvertImageToBitArrayForPython(image);
                using (Py.GIL())
                {
                    var classifier = Py.Import("ModelClassifier");
                    var pythonImage = imageBitMap.ToPython();

                    var predictedData = classifier.InvokeMethod("LoadModelAndPeridct", new[] { pythonImage, tileSize.ToPython() });

                    var results = predictedData.As<int[,,]>();
                    return ArrayToObjectList(results);
                }
            }catch (Exception ex)
            {
                throw ex;
            }
            return new List<TileDescription>();
        }

        private IEnumerable<TileDescription> ArrayToObjectList(int[,,] data)
        {
            var tilesDescriptions = new List<TileDescription>();
            for (int i = 0; i < data.GetLength(1); i++)
            {
                var tileDescription = new TileDescription();
                tileDescription.SidesKind = GetArrayFromHigherDimennsions(data,0,i);
                tileDescription.CornersKind = GetArrayFromHigherDimennsions(data,1,i);
            }
            return tilesDescriptions;
        }

        private IEnumerable<int> GetArrayFromHigherDimennsions(int[,,] data, int firstDim, int secDim)
        {
            var array = new List<int>();
            for (int i = 0;i < data.GetLength(2); i++)
            {
                array.Add(data[firstDim, secDim, i]);
            }
            return array;
        }
    }
}
