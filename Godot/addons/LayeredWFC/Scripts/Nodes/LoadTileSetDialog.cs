#if TOOLS
using Godot;
using System;
using LayeredWFC.Plugin;

[Tool]
public partial class LoadTileSetDialog : FileDialog
{
	public CallerEnum DialogCaller { get; set; } = CallerEnum.Sprites;
}
#endif
