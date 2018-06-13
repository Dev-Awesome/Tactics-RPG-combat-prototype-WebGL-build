using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BattleEnums;

public static class Pathfinder {

	private static Dictionary<GridPos, GridCell> cells;

	private static void GetGridCells () {

		//Copy tiles from Grid Manager
		cells = GridManager.gridCells;
	}

	public static Stack<GridCell> GetPath (GridCell start, GridCell end)
	{
		GetGridCells ();

		//Instantiate lists
		Stack<GridCell> path = new Stack<GridCell> ();
		HashSet<GridCell> openList = new HashSet<GridCell> ();
		HashSet<GridCell> closedList = new HashSet<GridCell> ();

		GridCell currentGridCell = start;
		openList.Add (currentGridCell);

		while (openList.Count > 0) {

			//Check x axis
			for (int x = -1; x <= 1; x++) {
				GridPos pos = new GridPos (currentGridCell.gridPos.x - x, currentGridCell.gridPos.y);
				if (cells.ContainsKey (pos)) {
					GridCell nextGridCell = cells [pos];

					//Check content
					if (nextGridCell.CanMoveThrough ()) {

						//Check if already in open list, if so, check if a better parent.
						if (openList.Contains (nextGridCell)) {
							if (currentGridCell.g + 1 < nextGridCell.g) {
								nextGridCell.CalcValues (currentGridCell, end);
							}
						}

						//Check if not in closed list, if so, add to open list.
						else if (!closedList.Contains (nextGridCell)) {
							openList.Add (nextGridCell);
							nextGridCell.CalcValues (currentGridCell, end);
						}
					}
				}
			}

			//Check y axis
			for (int y = -1; y <= 1; y++) {
				GridPos pos = new GridPos (currentGridCell.gridPos.x, currentGridCell.gridPos.y - y);
				if (cells.ContainsKey (pos)) {
					GridCell nextGridCell = cells [pos];

					//Check content
					if (nextGridCell.CanMoveThrough ()) {

						//Check if already in open list, if so, check if a better parent.
						if (openList.Contains (nextGridCell)) {
							if (currentGridCell.g + 1 < nextGridCell.g) {
								nextGridCell.CalcValues (currentGridCell, end);
							}
						}

						//Check if not in closed list, if so, add to open list.
						else if (!closedList.Contains (nextGridCell)) {
							openList.Add (nextGridCell);
							nextGridCell.CalcValues (currentGridCell, end);
						}
					}
				}
			}
			//End for loops.
			//Remove current tile from open list and add it to the closed list.
			openList.Remove (currentGridCell);
			closedList.Add (currentGridCell);

			//Sort the open list by lowest F score.
			if (openList.Count > 0) {
				currentGridCell = openList.OrderBy (c => c.f).First ();
			}

			//If goal is found, add it and it's parents to the Path. Then break.
			if (currentGridCell == end) {
				while (currentGridCell.gridPos != start.gridPos) {
					path.Push (currentGridCell);
					currentGridCell = currentGridCell.parent;
				}
				break;
			}
		}
		//End while loop
		return path;
	}

	//Bresenham's line drawing algorithm
	//Checks for LoS
	private static GridCell CheckLoS(int x0, int y0, int x1, int y1)
	{
		GridPos origin = new GridPos (x0, y0);
		int dx = Mathf.Abs(x1 - x0);
		int dy = Mathf.Abs(y1 - y0);
		int x = x0;
		int y = y0;
		int n = 1 + dx + dy;
		int x_inc = (x1 > x0) ? 1 : -1;
		int y_inc = (y1 > y0) ? 1 : -1;
		int error = dx - dy;
		dx *= 2;
		dy *= 2;

		for (; n > 0; --n)
		{
			if (!CheckGridCell (new GridPos (x, y), origin)) {
				return cells [new GridPos (x, y)];
			} else {

				if (error > 0) {
					x += x_inc;
					error -= dy;
				} else {
					y += y_inc;
					error += dx;
				}
			}
		}
		return cells [new GridPos (x1, y1)];
	}

	//Check tile content.
	private static bool CheckGridCell (GridPos pos, GridPos origin)
	{
		if (cells.ContainsKey (pos)) {
			GridCell c = cells [pos];
			if (c.gridPos == origin) {
				return true;
			} else {
				bool ret = c.CanShootThrough ();
				//Debug.Log (c.name + " -> " + GridManager.gridGridCells [origin] + ": " + ret); 
				return ret;
			}
		} else {
			return false;
		}
	}

	public static HashSet<GridCell> GetMoveRange (GridCell location, int mp)
	{
		GetGridCells ();
		//Reset tile values.
		foreach (GridCell c in cells.Values) {
			c.parent = null;
			c.distance = int.MaxValue;
		}
			
		//Create lists.
		HashSet<GridCell> reachable = new HashSet<GridCell> ();
		Queue<GridCell> toCheck = new Queue<GridCell> ();

		//Begin queue.
		GridCell start = location;
		start.distance = 0;
		toCheck.Enqueue (start);

		//Begin while loop.
		while (toCheck.Count > 0) {

			//Check for neighbours.
			GridCell current = toCheck.Dequeue();

			//Check X axis.
			for (int x = -1; x <= 1; x++) {
				GridPos check = new GridPos (current.gridPos.x - x, current.gridPos.y);
				if (cells.ContainsKey (check) && check != current.gridPos) {
					GridCell next = cells [check];

					//Check cell content.
					if (next.CanMoveThrough ()) {

						//Check if already in queue.
						if (!toCheck.Contains (next) && next.distance == int.MaxValue) {
							next.distance = current.distance + 1;
							next.parent = current;
							toCheck.Enqueue (next);

							//Check if in mp range.
							if (next.distance <= mp && !reachable.Contains (next)) {
								reachable.Add (next);
							}
						}
					}
				}
			}

			//Check Y axis.
			for (int y = - 1; y <= 1; y++) {
				GridPos check = new GridPos (current.gridPos.x, current.gridPos.y - y);
				if (cells.ContainsKey (check) && check != current.gridPos) {
					GridCell next = cells [check];

					//Check cell content.
					if (next.CanMoveThrough ()) {

						//Check if already in queue.
						if (!toCheck.Contains (next) && next.distance == int.MaxValue) {
							next.distance = current.distance + 1;
							next.parent = current;
							toCheck.Enqueue (next);

							//Check if in mp range.
							if (next.distance <= mp && !reachable.Contains (next)) {
								reachable.Add (next);
							}
						}
					}
				}
			}

			//End for loops.
		}

		//End while loop.

		return reachable;
	}

	//Ability range
	public static HashSet<GridCell> GetAbilityRange (GridCell location, int minRange, int maxRange, bool los, bool linear, bool freeCell, bool ai = false)
	{
		GetGridCells ();
		HashSet<GridCell> inRange = new HashSet<GridCell> ();
		HashSet<GridCell> canSee = new HashSet<GridCell> ();
		HashSet<GridCell> retVal = new HashSet<GridCell> ();
		//non linear
		if (!linear) {
			foreach (GridCell c in cells.Values) {
				if (location.CalcDistance (c) <= maxRange && location.CalcDistance (c) >= minRange) {
					if (!inRange.Contains (c)) {
						if (c.CanTarget (freeCell)) {
							inRange.Add (c);
						}
					}
				}
			}
		} else {
			//linear
			for (int x = -maxRange; x <= maxRange; x++) {
				GridPos pos = new GridPos (location.gridPos.x - x, location.gridPos.y);
				if (cells.ContainsKey (pos)) {
					GridCell c = cells [pos];
					if (c.CalcDistance (location) <= maxRange && c.CalcDistance (location) >= minRange) {
						if (c.CanTarget (freeCell)) {
							inRange.Add (c);
						}
					}
				}
			}
			for (int y = -maxRange; y <= maxRange; y++) {
				GridPos pos = new GridPos (location.gridPos.x, location.gridPos.y - y);
				if (cells.ContainsKey (pos)) {
					GridCell c = cells [pos];
					if (c.CalcDistance (location) <= maxRange && c.CalcDistance (location) >= minRange) {
						if (c.CanTarget (freeCell)) {
							inRange.Add (c);
						}
					}
				}
			}
		}
		//Highlight
	//	if (!ai) {
	//		GameObject.FindGameObjectWithTag ("Battle Controller").GetComponent<GridHighlighter> ().HighlightNonVisibleCell (inRange);
	//	}

		//Check LoS
		if (los) {
			foreach (GridCell c in inRange) {
				GridCell hit = CheckLoS (location.gridPos.x, location.gridPos.y, c.gridPos.x, c.gridPos.y);
				if (hit.CanTarget (freeCell)) {
					if (!canSee.Contains (hit)) {
						canSee.Add (hit);
					}
				}
			}
		} else {
			return inRange;
		}

		//Filter out range errors
		foreach (GridCell c in canSee) {
			if (location.CalcDistance (c) >= minRange) {
				retVal.Add (c);
			}
		}
		return retVal;
	}

	public static HashSet<GridCell> GetAoE (GridCell target, GridCell start, eAreaType aoe, int size) {

		HashSet<GridCell> area = new HashSet<GridCell> ();

		GetGridCells ();

		string facing = start.GetFacing (target);

		switch (aoe) {
		case eAreaType.SINGLE:
			area.Add (target);
			return area;

		case eAreaType.CIRCLE:
			for (int x = -size; x <= size; x++) {
				for (int y = -size; y <= size; y++) {
					GridPos p = new GridPos (target.gridPos.x - x, target.gridPos.y - y);
					if (cells.ContainsKey (p)) {
						if (target.CalcDistance (cells [p]) <= size) {
							if (!area.Contains(cells[p])) {
								area.Add (cells [p]);
							}
						}
					}
				}
			}
			return area;

		case eAreaType.CROSS:
			for (int x = -size; x <= size; x++) {
				GridPos p = new GridPos (target.gridPos.x - x, target.gridPos.y);
				if (cells.ContainsKey (p)) {
					if (!area.Contains (cells [p])) {
						area.Add (cells [p]);
					}
				}
			}
			for (int y = -size; y <= size; y++) {
				GridPos p = new GridPos (target.gridPos.x, target.gridPos.y - y);
				if (cells.ContainsKey (p)) {
					if (!area.Contains (cells [p])) {
						area.Add (cells [p]);
					}
				}
			}
			return area;

		case eAreaType.V_LINE:
			if (facing == "NE") {
				for (int y = 0; y <= size; y++) {
					GridPos p = new GridPos (target.gridPos.x, target.gridPos.y + y);
					if (cells.ContainsKey (p)) {
						if (!area.Contains (cells [p])) {
							area.Add (cells [p]);
						}
					}
				}
			} else if (facing == "SE") {
				for (int x = 0; x <= size; x++) {
					GridPos p = new GridPos (target.gridPos.x + x, target.gridPos.y);
					if (cells.ContainsKey (p)) {
						if (!area.Contains (cells [p])) {
							area.Add (cells [p]);
						}
					}
				}
			} else if (facing == "SW") {
				for (int y = 0; y <= size; y++) {
					GridPos p = new GridPos (target.gridPos.x, target.gridPos.y - y);
					if (cells.ContainsKey (p)) {
						if (!area.Contains (cells [p])) {
							area.Add (cells [p]);
						}
					}
				}
			} else if (facing == "NW") {
				for (int x = 0; x <= size; x++) {
					GridPos p = new GridPos (target.gridPos.x - x, target.gridPos.y);
					if (cells.ContainsKey (p)) {
						if (!area.Contains (cells [p])) {
							area.Add (cells [p]);
						}
					}
				}
			}
			return area;

		case eAreaType.H_LINE:

			return area;

		}

		return area;
	}

	public static GridCell GetDisplacementLocation(GridCell start, string facing, int distance)
	{
		GridCell targetLocation = start;
		switch (facing) {

		case "NE":
			for (int y = 0; y <= distance; y++) {
				GridPos p = new GridPos (start.gridPos.x, start.gridPos.y + y);
				if (cells.ContainsKey (p)) {
					if (cells [p].cellType == GridCell.eCellType.DEFAULT) {
						if (cells [p].currentUnit != null) {
							if (cells [p].currentUnit == start.currentUnit) {
								targetLocation = cells [p];
							} else {
								return targetLocation;
							}
						} else {
							targetLocation = cells [p];
						}
					} else {
						return targetLocation;
					}
				}
			}
			return targetLocation;

		case "SE":
			for (int x = 0; x <= distance; x++) {
				GridPos p = new GridPos (start.gridPos.x + x, start.gridPos.y);
				if (cells.ContainsKey (p)) {
					if (cells [p].cellType == GridCell.eCellType.DEFAULT) {
						if (cells [p].currentUnit != null) {
							if (cells [p].currentUnit == start.currentUnit) {
								targetLocation = cells [p];
							} else {
								return targetLocation;
							}
						} else {
							targetLocation = cells [p];
						}
					} else {
						return targetLocation;
					}
				}
			}
			return targetLocation;

		case "SW":
			for (int y = 0; y <= distance; y++) {
				GridPos p = new GridPos (start.gridPos.x, start.gridPos.y - y);
				if (cells.ContainsKey (p)) {
					if (cells [p].cellType == GridCell.eCellType.DEFAULT) {
						if (cells [p].currentUnit != null) {
							if (cells [p].currentUnit == start.currentUnit) {
								targetLocation = cells [p];
							} else {
								return targetLocation;
							}
						} else {
							targetLocation = cells [p];
						}
					} else {
						return targetLocation;
					}
				}
			}
			return targetLocation;

		case "NW":
			for (int x = 0; x <= distance; x++) {
				GridPos p = new GridPos (start.gridPos.x - x, start.gridPos.y);
				if (cells.ContainsKey (p)) {
					if (cells [p].cellType == GridCell.eCellType.DEFAULT) {
						if (cells [p].currentUnit != null) {
							if (cells [p].currentUnit == start.currentUnit) {
								targetLocation = cells [p];
							} else {
								return targetLocation;
							}
						} else {
							targetLocation = cells [p];
						}
					} else {
						return targetLocation;
					}
				}
			}
			return targetLocation;

		default:
			Debug.LogError ("Error: Invalid direction.");
			return start;
		}
	}
}
