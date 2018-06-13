using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class OverworldPathfinder {

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

			for (int x = -1; x <= 1; x++) {
				for (int y = -1; y <= 1; y++) {
					GridPos pos = new GridPos (currentGridCell.gridPos.x - x, currentGridCell.gridPos.y - y);
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
}
