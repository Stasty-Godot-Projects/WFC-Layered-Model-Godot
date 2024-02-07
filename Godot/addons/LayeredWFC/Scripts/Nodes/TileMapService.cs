using Godot;
using System;
using System.Collections.Generic;

namespace LayeredWFC
{
	public class TileMapService
	{
		private string _tilesFileName, _collisionsFileName, _sceneName;
		private int _sideSize;
		
		public  IEnumerable<TilesDescription> TilesDescriptions {get; set;}
		public  IEnumerable<SidesKind> SidesKind {get; set;}
		
		public TileMapService(string tilesFileName,string sceneName, int sideSize)
		{
			_tilesFileName= tilesFileName;
			_collisionsFileName = collisionsFileName;
			_sceneName = sceneName; 
			_sideSize = sideSize;
		}
		
		public void CreateTileMap(){
			var tileMap = new WFCTileMap();
			tileMap
		}
	}
}

