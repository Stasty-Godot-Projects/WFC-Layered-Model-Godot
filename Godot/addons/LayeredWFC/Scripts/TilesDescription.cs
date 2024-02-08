using Godot;
using System;
using System.Collections.Generic;

namespace LayeredWFC
{
	public class TilesDescription
	{
		public int Id {get; set;}
		public string Biome {get; set;}
		public IEnumerable<int> SideKind {get; set;}
		public IEnumerable<int> CornerKind {get; set;}
	}
}

