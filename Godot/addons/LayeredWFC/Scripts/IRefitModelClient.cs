using LayeredWFC.addons.LayeredWFC.Scripts.DTO;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRefitModelClient
{
	[Multipart]
	[Post("/api/Model/AddImageForTheModel")]
	public Task AddImageForTheModel([AliasAs("file")] byte[] image);
	[Post("/api/Model/CreateModel")]
	public Task CreateModel([Body] RequestBody tilesInfo);
	[Get("/api/Model/GetIsModelTrainedNow")]
	public Task<bool> GetIsModelTrainedNow();
	[Multipart]
	[Post("/api/Model/Recognize/{tileSize}")]
	public Task<IEnumerable<TileDescriptionDto>> Recognize([AliasAs("file")] byte[] image, int tileSize);
}
