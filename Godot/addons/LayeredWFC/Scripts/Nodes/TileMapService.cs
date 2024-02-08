using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace LayeredWFC
{
	public class TileMapService
	{
		private string _tilesFileName, _sceneName;
		private int _sideSize;
		
		public  IEnumerable<TilesDescription> TilesDescriptions {get; set;} = null;
		public  IEnumerable<SidesKind> SidesKind {get; set;} = null;
		
		public TileMapService(string tilesFileName,string sceneName, int sideSize)
		{
			_tilesFileName= tilesFileName;
			_sceneName = sceneName; 
			_sideSize = sideSize;
		}
		
		public void CreateTileMap()
		{
			var tileMap = new WFCTileMap();
			var tileSet = new TileSet();
			var tileSize = new Vector2I(_sideSize,_sideSize);
			tileSet.TileSize = tileSize;
			var tilesAtlas=GetTextureAtlas(tileSize, out int Cols, out int Rows);
			tileMap.Cols = Cols;
			tileMap.Rows = Rows;
			var tileSetId = tileSet.AddSource(tilesAtlas);
			tileMap.TileSet = tileSet;
			if(SidesKind is null || TilesDescriptions is null)
				return;
			tileMap.SidesDescription = SidesKind.ToArray();
			tileMap.TilesDescription = TilesDescriptions.Select(x => {
				GD.Print(x.Id);
				return new TilesDescriptionResource(){
					Id = x.Id,
					Biome = x.Biome,
					SideKind = x.SideKind.Select(y => SidesKind.First(z => y == z.Id)).ToArray(),
					CornerKind = x.CornerKind.Select(y => SidesKind.First(z => y == z.Id)).ToArray()
			};
			}
				).ToArray();
			var scenePath = GetScenePath(_sceneName, "res://");
			AddTileMapToScene(scenePath, tileMap);
		}
		
		public string GetScenePath(string sceneName, string directoryPath)
		{

			var directory = DirAccess.Open(directoryPath);
			if (directory is null)
			{
				GD.PrintErr("Failed to open directory: " + directoryPath);
				return string.Empty;
			}
			
			string scenePath = string.Empty;
			
			foreach(var file in directory.GetFiles().Distinct())
			{
				if((file.EndsWith(".tscn") || file.EndsWith(".scn"))&&file.Contains(sceneName))
					scenePath = directoryPath+file;
			}
			if(string.IsNullOrEmpty(scenePath))
			{
				foreach(var dir in directory.GetDirectories())
				{
					scenePath = GetScenePath(sceneName,directoryPath+dir+"/");
					if(!string.IsNullOrEmpty(scenePath))
						break;
				}
			}
			return scenePath;
		}
		
		public void AddTileMapToScene(string scenePath, WFCTileMap tileMap)
		{
			var sceneRes = GD.Load<PackedScene>(scenePath);
			GD.Print(sceneRes.GetState());
			var sceneNode = sceneRes.Instantiate(PackedScene.GenEditState.Instance);
			sceneNode.AddChild(tileMap);
			var scene = new PackedScene();
			foreach(var x in sceneNode.GetChildren())
				x.Owner = sceneNode;
			scene.Pack(sceneNode);
			GD.Print("Save");
			DirAccess.RemoveAbsolute(scenePath);
			var error = ResourceSaver.Save(scene,scenePath);
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
			for(int i = 0; i<Cols;i++){
				for(int j = 0; i<Rows;i++){
					tilesAtlas.CreateTile(new Vector2I(i,j), tileSize);
				}
			}
			return tilesAtlas;
		}
	}
}

