using Godot;
using System;


[GlobalClass, Icon("res://Stats/StatsIcon.svg")]
public partial class SidesKind: Resource
{
	[Export]
	public int Id {get; set;}
	[Export]
	public string ReadAbleName {get; set;}
	[Export]
	 public float Propability {get; set;}
}


