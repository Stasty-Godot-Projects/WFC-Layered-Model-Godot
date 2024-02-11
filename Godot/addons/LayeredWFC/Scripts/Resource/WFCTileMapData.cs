using Godot;
using System;

[Tool]
[GlobalClass, Icon("res://Stats/StatsIcon.svg")]
public partial class WFCTileMapData : Resource
{
	[Export]
	public TilesDescriptionResource[] TilesDescription { get; set; } = null;
	[Export]
	public SidesKind[] SidesDescription { get; set; } = null;
	[Export]
	public SourceData[] SourceData {get; set;}
}
