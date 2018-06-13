using UnityEngine;
using System.Collections;

public class GridPos {

	public int x { get; private set; }
	public int y { get; private set; }

	public GridPos (int x, int y) {
		this.x = x;
		this.y = y;
	}

	//Operator functions
	public static bool operator ==(GridPos first, GridPos second) {
		return first.x == second.x && first.y == second.y;
	}

	public static bool operator !=(GridPos first, GridPos second) {
		return first.x != second.x || first.y != second.y;
	}

	public static GridPos operator +(GridPos a, GridPos b)
	{
		return new GridPos(a.x + b.x, a.y + b.y);
	}

	public static GridPos operator -(GridPos p1, GridPos p2) 
	{
		return new GridPos(p1.x - p2.x, p1.y - p2.y);
	}

	public override bool Equals (object obj)
	{
		if (obj is GridPos)
		{
			GridPos p = (GridPos)obj;
			return x == p.x && y == p.y;
		}
		return false;
	}

	public bool Equals (GridPos p)
	{
		return x == p.x && y == p.y;
	}

	public override int GetHashCode ()
	{
		return x ^ y;
	}
}
