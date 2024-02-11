using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Refit;
using LayeredWFC.addons.LayeredWFC.Scripts.DTO;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using static System.Formats.Asn1.AsnWriter;

namespace LayeredWFC
{
	[Tool]
	public partial class TileMapService : EditorScript
	{
		private string _tilesFileName, _sceneName;
		private int _sideSize;
		private IRefitModelClient _refitModelClient;

		public IEnumerable<TilesDescription> TilesDescriptions { get; set; } = null;
		public IEnumerable<SidesKind> SidesKind { get; set; } = null;

		public TileMapService(string tilesFileName, string sceneName, int sideSize, string url)
		{
			_tilesFileName = tilesFileName;
			_sceneName = sceneName;
			_sideSize = sideSize;
			_refitModelClient = RestService.For<IRefitModelClient>(url);
		}

		public void CreateTileMap()
		{
			TrainModel();
			var tileMap = new WFCTileMap();
			var tileSet = new TileSet();
			var tileSize = new Vector2I(_sideSize, _sideSize);
			tileSet.TileSize = tileSize;
			var tilesAtlas = GetTextureAtlas(tileSize, out int Cols, out int Rows);
			var tileSetId = tileSet.AddSource(tilesAtlas);
			tileMap.TileSet = tileSet;
			if (SidesKind is null || TilesDescriptions is null)
				return;
			var tileMapData = new WFCTileMapData();
			tileMapData.SidesDescription = SidesKind.ToArray();
			tileMapData.TilesDescription = TilesDescriptions.Select(x =>
			{
				GD.Print(x.Id);
				return new TilesDescriptionResource()
				{
					Id = x.Id,
					Biome = x.Biome,
					SideKind = x.SideKind.Select(y => SidesKind.First(z => y == z.Id)).ToArray(),
					CornerKind = x.CornerKind.Select(y => SidesKind.First(z => y == z.Id)).ToArray()
				};
			}).ToArray();
			var source = new SourceData()
			{
				Id = 0,
				TileCount = TilesDescriptions.ToList().Count,
				Rows = Rows,
				Cols = Cols
			};
			tileMapData.SourceData = new SourceData[] { source };
			var scenePath = GetFilePath(_sceneName, "res://", new[] { ".tscn", ".scn" });
			AddTileMapToScene(scenePath, tileMap, tileMapData);
		}

		public async Task AddTilesToMap()
		{
			var scenePath = GetFilePath(_sceneName, "res://", new[] { ".tscn", ".scn" });
			var resPath = GetFilePath(_sceneName, "res://", new[] { ".res" });
			var tileSize = new Vector2I(_sideSize, _sideSize);
			var textureAtlas = GetTextureAtlas(tileSize, out int Cols, out int Rows);
			var tileMap = GetTileMapFromScene(scenePath);
			var tileMapData = GD.Load<WFCTileMapData>(resPath);
			GD.Print(tileMap == null);
			try
			{
				var tileDescriptionDto = await RecognizeDescriptionsFromModel();
				var lastId = tileMapData.TilesDescription.Last().Id + 1;
				var tileDescriptionResources = tileDescriptionDto.Select(x =>
				{
					return new TilesDescriptionResource()
					{
						Id = lastId++,
						Biome = string.Empty,
						SideKind = x.SidesKind.Select(y => tileMapData.SidesDescription.First(z => y == z.Id)).ToArray(),
						CornerKind = x.CornersKind.Select(y => tileMapData.SidesDescription.First(z => y == z.Id)).ToArray()
					};
				}).ToList();
				var list = tileMapData.TilesDescription.ToList();
				list.AddRange(tileDescriptionResources);
				tileMapData.TilesDescription = list.ToArray();
				lastId = tileMapData.SourceData.Last().Id + 1;
				tileMap.TileSet.AddSource(textureAtlas, lastId);
				var source = new SourceData()
				{
					Id = lastId,
					TileCount = tileDescriptionDto.ToList().Count,
					Rows = Rows,
					Cols = Cols
				};
				tileMapData.SourceData = tileMapData.SourceData.Append(source).ToArray();
				UpdateTileMapToScene(scenePath, resPath, tileMap, tileMapData);
			}
			catch (Exception ex)
			{
				GD.Print("Update not possible:" + ex);
			}
		}

		public string GetFilePath(string fileName, string directoryPath, IEnumerable<string> fileFilters)
		{

			var directory = DirAccess.Open(directoryPath);
			if (directory is null)
			{
				GD.PrintErr("Failed to open directory: " + directoryPath);
				return string.Empty;
			}

			string scenePath = string.Empty;

			foreach (var file in directory.GetFiles().Distinct())
			{
				if (fileFilters.Any(x => file.EndsWith(x)) && file.Contains(fileName))
					scenePath = directoryPath + file;
			}
			if (string.IsNullOrEmpty(scenePath))
			{
				foreach (var dir in directory.GetDirectories())
				{
					scenePath = GetFilePath(fileName, directoryPath + dir + "/", fileFilters);
					if (!string.IsNullOrEmpty(scenePath))
						break;
				}
			}
			return scenePath;
		}

		public void AddTileMapToScene(string scenePath, WFCTileMap tileMap, WFCTileMapData resource)
		{
			var resPath = scenePath.Replace(".tscn", ".res").Replace(".scn", ".res");
			tileMap.DataURI = resPath;
			var sceneRes = GD.Load<PackedScene>(scenePath);
			GD.Print(sceneRes.GetState());
			var sceneNode = sceneRes.Instantiate(PackedScene.GenEditState.Instance);
			sceneNode.AddChild(tileMap);
			var scene = new PackedScene();
			foreach (var x in sceneNode.GetChildren())
				x.Owner = sceneNode;
			scene.Pack(sceneNode);
			GD.Print("Save");
			DirAccess.RemoveAbsolute(scenePath);
			DirAccess.RemoveAbsolute(resPath);
			var error = ResourceSaver.Save(scene, scenePath);
			GD.Print(error);
			error = ResourceSaver.Save(resource, resPath);
			GD.Print(error);
		}

		public TileMap GetTileMapFromScene(string scenePath)
		{
			var sceneRes = GD.Load<PackedScene>(scenePath);
			var sceneNode = sceneRes.Instantiate(PackedScene.GenEditState.MainInherited);
			foreach (var child in sceneNode.GetChildren())
			{
				GD.Print(child.GetClass());
				if (child.IsClass("TileMap"))
				{
					return (TileMap)child;
				}

			}
			return null;
		}


		public void UpdateTileMapToScene(string scenePath, string resPath, TileMap tileMap, WFCTileMapData tileMapData)
		{
			var sceneRes = GD.Load<PackedScene>(scenePath);
			GD.Print(sceneRes.GetState());
			var sceneNode = sceneRes.Instantiate(PackedScene.GenEditState.Instance);

			foreach (var child in sceneNode.GetChildren())
				if (child is WFCTileMap)
					child.ReplaceBy(tileMap, true);

			var scene = new PackedScene();
			foreach (var x in sceneNode.GetChildren())
				x.Owner = sceneNode;
			scene.Pack(sceneNode);
			GD.Print("Save");
			DirAccess.RemoveAbsolute(scenePath);
			var error = ResourceSaver.Save(scene, scenePath);
			GD.Print(error);
			DirAccess.RemoveAbsolute(resPath);
			error = ResourceSaver.Save(tileMapData, resPath);
			GD.Print(error);
		}

		private TileSetAtlasSource GetTextureAtlas(Vector2I tileSize, out int Cols, out int Rows)
		{
			var tilesAtlas = new TileSetAtlasSource();
			var texture = (Texture2D)ResourceLoader.Load(_tilesFileName);
			tilesAtlas.Texture = texture;
			tilesAtlas.TextureRegionSize = tileSize;
			var atlasSize = tilesAtlas.GetAtlasGridSize();
			Cols = atlasSize.X;
			Rows = atlasSize.Y;
			for (int i = 0; i < Cols; i++)
			{
				for (int j = 0; i < Rows; i++)
				{
					tilesAtlas.CreateTile(new Vector2I(i, j), tileSize);
				}
			}
			return tilesAtlas;
		}

		private async void TrainModel()
		{
			var body = new RequestBody()
			{
				TileSize = _sideSize,
				TileDescriptions = TilesDescriptions.Select(x => new TileDescriptionDto
				{
					SidesKind = x.SideKind,
					CornersKind = x.CornerKind
				})
			};
			var file = FileAccess.GetFileAsBytes(_tilesFileName);
			try
			{
				await _refitModelClient.AddImageForTheModel(file);
				await _refitModelClient.CreateModel(body);
			}
			catch (Exception Ex)
			{
				GD.Print(Ex);
			}


		}

		private async Task<IEnumerable<TileDescriptionDto>> RecognizeDescriptionsFromModel()
		{
			var file = FileAccess.GetFileAsBytes(_tilesFileName);
			return await _refitModelClient.Recognize(file, _sideSize);
		}

		public async Task<bool> IsFreeToProcess()
		{
			return await _refitModelClient.GetIsModelTrainedNow();
		}
	}
}

