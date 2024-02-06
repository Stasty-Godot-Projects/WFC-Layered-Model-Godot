using Godot;
using System;

namespace LayeredWFC
{
	public class TileMapService
	{
		private string _tilesFileName, _collisionsFileName, _sceneName;
		private int _sideSize;
		public string JSONFileName {get; set;};
		public TileMapService(string tilesFileName,string collisionsFileName,string sceneName, int sideSize)
		{
			_tilesFileName= tilesFileName;
			_collisionsFileName = collisionsFileName;
			_sceneName = sceneName; 
			_sideSize = sideSize;
		}
		
		
	}
}

