using WFC_Godot.API.Model;

namespace WFC_Godot.API.Services.Interfaces
{
    public interface IRecognitionService
    {
        IEnumerable<TileDescription> RecognizeTiles(IFormFile file, int tileSize);
    }
}
