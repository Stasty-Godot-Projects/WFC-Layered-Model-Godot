using Godot;
using System;

[Tool]
[GlobalClass, Icon("res://Stats/StatsIcon.svg")]
public partial class SourceData : Resource
{
	[Export]
	public int Id {get; set;}
	[Export]
	public int TileCount {get; set;}
	[Export]
	public int Rows {get; set;}
	[Export]
	public int Cols {get; set;}
}
