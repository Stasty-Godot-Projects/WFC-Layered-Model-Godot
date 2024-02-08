using Godot;
using System;
using System.Collections.Generic;

public static class ArrayExtensions
{
	public static IEnumerable<T> ToEnumerable<T>(this T[,] target)
	{
		foreach (var item in target)
			yield return item;
	}
}
