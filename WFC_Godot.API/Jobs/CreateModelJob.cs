using Python.Runtime;
using System;
using System.Diagnostics.Metrics;
using WFC_Godot.API.Model;
using WFC_Godot.API.Services;

namespace WFC_Godot.API.Jobs
{
    public class CreateModelJob
    {
        private static SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public async Task Exec(byte[] file, IEnumerable<TileDescription> tilesDescriptions, int tileSize)
        {
            await semaphore.WaitAsync();

            try
            {
                var tilesSides = tilesDescriptions.Select(x=>x.SidesKind);
                var tilesCorners = tilesDescriptions.Select(x => x.CornersKind);
                var image = ImageHelper.ConvertFileToImage(file);
                var imageBitMap = ImageHelper.ConvertImageToBitArrayForPython(image);
                var state = PythonEngine.BeginAllowThreads();
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
                            var sidesPython = new PyList();
                            foreach (var tile in tilesSides)
                            {
                                var sublist = new PyList();
                                foreach (var side in tile)
                                {
                                    sublist.Append(side.ToPython());
                                }
                                sidesPython.Append(sublist);
                            }
                            var cornerPython = new PyList();
                            foreach (var tile in tilesCorners)
                            {
                                var sublist = new PyList();
                                foreach (var corner in tile)
                                {
                                    sublist.Append(corner.ToPython());
                                }
                                cornerPython.Append(sublist);
                            }
                            var pythonImage = imageBitMap.ToPython();
                            var preparedData = classifier.InvokeMethod("DataPreparation", new[] { pythonImage, sidesPython, cornerPython, tileSize.ToPython() });
                            var outputString = classifier.InvokeMethod("TrainModels", preparedData);
                        }
                        
                    }catch(Exception ex)
                    {
                        ex.ToString();
                    }

                }
                PythonEngine.EndAllowThreads(state);
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
