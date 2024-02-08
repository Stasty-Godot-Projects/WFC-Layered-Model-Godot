using Godot;
using System;

public partial class KernelElement
{
	public int X {get; set;}
	public int Y {get; set;}
	public int ArrEl {get; set;}
	
	public KernelElement(int X, int Y, int ArrEl)
	{
		this.X=X;
		this.Y=Y;
		this.ArrEl=ArrEl;
	}
}
