using Python.Runtime;
using System.Collections.Generic;
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

                var state = PythonEngine.BeginAllowThreads();
                IEnumerable<TileDescription> tiles;
                using (Py.GIL())
                {
                    try
                    {
                        var importOs = Py.Import("os");
                        var path = importOs.InvokeMethod("getcwd");

                        using (PyModule scope = Py.CreateScope())
                        {
                            scope.Set("path", path);

                            scope.Exec("import sys;sys.path.append(path)");
                            var classifier = Py.Import("ModuleClassifier");
                            var pythonImage = imageBitMap.ToPython();


                            var predictedData = classifier.InvokeMethod("LoadModelAndPeridct", new[] { pythonImage, tileSize.ToPython() });

                            var results = predictedData.As<int[][][]>();
                            tiles = ArrayToObjectList(results);
                        }

                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                        PythonEngine.EndAllowThreads(state);
                        return new List<TileDescription>();
                    }

                }
                PythonEngine.EndAllowThreads(state);
                return tiles;
            }catch (Exception ex)
            {
                throw ex;
            }
        }

        private IEnumerable<TileDescription> ArrayToObjectList(IEnumerable<IEnumerable<int>>[] data)
        {
            var tilesDescriptions = new List<TileDescription>();
            var sides = data[0].ToList();
            var cosners = data[1].ToList();
            for (var i = 0; i < sides.Count; i++) 
            {
                var tile = new TileDescription()
                {
                    SidesKind = sides[i],
                    CornersKind = cosners[i]
                };
                tilesDescriptions.Add(tile);
            }
            return tilesDescriptions;
        }

    }
}
