using Godot;
using System;
using System.Collections.Generic;


[GlobalClass, Icon("res://Stats/StatsIcon.svg")]
public partial class TilesDescriptionResource: Resource
{
	[Export]
	public string Biome {get; set;}
	[Export]
	public SidesKind[] SideKind {get; set;}
	[Export]
	public SidesKind[] CornerKind {get; set;}
}

