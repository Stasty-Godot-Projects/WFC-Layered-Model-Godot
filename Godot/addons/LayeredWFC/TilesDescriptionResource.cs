using Godot;
using System;
using System.Collections.Generic;

namespace LayeredWFC
{
	[GlobalClass, Icon("res://Stats/StatsIcon.svg")]
	public class TilesDescription: Resource
	{
		[Export]
		public string Biome {get; set;}
		[Export]
		public IEnumerable<SidesKind> SideKind {get; set;}
		[Export]
		public IEnumerable<SidesKind> CornerKind {get; set;}
	}
}
