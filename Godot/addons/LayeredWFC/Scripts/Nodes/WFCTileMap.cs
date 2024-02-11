using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class WFCTileMap : TileMap
{
	[Export]
	public WFCTileMapData Data {get; set;} = null;
	[Export]
	public string DataURI {get; set;} = "";
	[Export]
	private Vector2I _startingCell = new Vector2I(0, 0);
	[Export]
	private Vector2I _size = new Vector2I(0, 0);
	[Export]
	private int _seed = -1;

	private KernelElement[] sides = new KernelElement[4] { new KernelElement(0, -1, 0), new KernelElement(1, 0, 1), new KernelElement(0, 1, 2), new KernelElement(-1, 0, 3) };
	private KernelElement[] corners = new KernelElement[4] { new KernelElement(-1, -1, 0), new KernelElement(1, -1, 1), new KernelElement(1, 1, 2), new KernelElement(-1, 1, 3) };
	private RandomNumberGenerator _random;
	public override void _Ready()
	{
		if(Data is null)
		{
			var resource = ResourceLoader.Load(DataURI) as WFCTileMapData;
			Data = resource;
		}
		_random = new RandomNumberGenerator();
		if (_seed > -1)
			_random.Seed = Convert.ToUInt64(_seed);
		else
			_random.Randomize();
		var tileMap = GenerateWFCTerrain();
		Populate(tileMap);
	}


	public override void _Process(double delta)
	{
	}

	public int?[,] GenerateWFCTerrain()
	{
		var map = Initialize();
		while (map.ToEnumerable().Any(x => x.Count > 1))
		{
			var cell = Choose(map);
			Fill(map, cell);
			Collapse(map, cell);
		}
		var tileMap = new int?[_size.X, _size.Y];
		for (int i = 0; i < _size.X; i++)
		{
			for (int j = 0; j < _size.Y; j++)
			{
				if (map[i, j].Count > 0)
					tileMap[i, j] = map[i, j].First();
				else
					tileMap[i, j] = null;
			}
		}
		return tileMap;
	}

	private List<int>[,] Initialize()
	{
		var map = new List<int>[_size.X, _size.Y];
		for (int i = 0; i < _size.X; i++)
		{
			for (int j = 0; j < _size.Y; j++)
			{
				map[i, j] = Data.TilesDescription.Select(x => x.Id).ToList(); ;
			}
		}
		return map;
	}

	private Vector2I Choose(List<int>[,] map)
	{
		var cellCord = new Vector2I(0, 0);
		var minNum = Data.TilesDescription.Length;
		if (!map.ToEnumerable().Any(x => x.Count < minNum))
			return new Vector2I(_random.RandiRange(0, _size.X - 1), _random.RandiRange(0, _size.Y - 1));
		for (int i = 0; i < _size.X; i++)
		{
			for (int j = 0; j < _size.Y; j++)
			{
				if (map[i, j].Count < minNum && map[i, j].Count > 1)
				{
					minNum = map[i, j].Count;
					cellCord = new Vector2I(i, j);
				}
			}
		}
		return cellCord;
	}

	private void Fill(List<int>[,] map, Vector2I kernelCenter)
	{
		var cell = map[kernelCenter.X, kernelCenter.Y];
		var tileId = _random.RandiRange(0, cell.Count - 1);
		map[kernelCenter.X, kernelCenter.Y] = new List<int>() { cell[tileId] };
	}

	private void Collapse(List<int>[,] map, Vector2I kernelCenter)
	{
		var cell = map[kernelCenter.X, kernelCenter.Y];
		//GD.Print(cell.First());
		var tile = Data.TilesDescription.First(x => x.Id == cell.First());
		foreach (var side in sides)
		{
			var kernelX = kernelCenter.X + side.X;
			var kernelY = kernelCenter.Y + side.Y;
			if (kernelX >= 0 && kernelX < _size.X
				&& kernelY >= 0 && kernelY < _size.Y)
			{

				var targetCell = map[kernelX, kernelY];
				if (targetCell.Count > 1)
				{
					var adjancecies = tile.SideKind[side.ArrEl].Adjacencies;
					map[kernelX, kernelY] = Data.TilesDescription.Where(x => targetCell.Any(y => y == x.Id) && adjancecies.Any(z => x.SideKind[(side.ArrEl + 2) % 4].Id == z)).Select(x => x.Id).ToList();
				}

			}
		}
		foreach (var corner in corners)
		{
			var kernelX = kernelCenter.X + corner.X;
			var kernelY = kernelCenter.Y + corner.Y;
			if (kernelX >= 0 && kernelX < _size.X
				&& kernelY >= 0 && kernelY < _size.Y)
			{
				var targetCell = map[kernelX, kernelY];
				if (targetCell.Count > 1)
				{
					var adjancecies = tile.CornerKind[corner.ArrEl].Adjacencies;
					map[kernelX, kernelY] = Data.TilesDescription.Where(x => targetCell.Any(y => y == x.Id) && adjancecies.Any(z => x.CornerKind[(corner.ArrEl + 2) % 4].Id == z)).Select(x => x.Id).ToList();
				}

			}
		}
	}

	private void Populate(int?[,] tileMap)
	{
		for (int i = 0; i < _size.X; i++)
		{
			for (int j = 0; j < _size.Y; j++)
			{
				var cell = tileMap[i, j];
				if (cell != null)
				{
					var atlasCords = IntToAtlasCords(cell ?? 0, out int sourceId);
					GD.Print(atlasCords);
					this.SetCell(0, new Vector2I(i + _startingCell.X, j + _startingCell.Y), sourceId, atlasCords);
				}
			}
		}
	}

	private Vector2I IntToAtlasCords(int i, out int sourceId)
	{
		GD.Print(i);
		if (Data.TilesDescription.First().Id == 1)
			i--;
		foreach(var source in Data.SourceData)
		{
			if(i >= source.TileCount)
				i -= source.TileCount;
			else
			{
				sourceId = source.Id;
				var X = i % source.Cols;
				var Y = (i - X) / source.Cols;
				return new Vector2I(X, Y);
			}
		}
		sourceId = 0;
		return new Vector2I(0,0);
	}
	
	
}
