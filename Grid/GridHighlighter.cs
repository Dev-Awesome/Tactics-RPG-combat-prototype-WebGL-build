using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHighlighter : MonoBehaviour {

	[Header("Highlight Colours")]
	public Color selector;
	public Color movePath;
	public Color moveRange;
	public Color abilityRange;
	public Color noLineOfSight;
	public Color areaOfAffect;
	public Color playerStart;
	public Color enemyStart;
	public Color tackleArea;

	private HashSet<GridCell> gridCells;

	public void InitHighlighter()
	{
		gridCells = new HashSet<GridCell> ();
	}

	private void UpdateGrid()
	{
		foreach (GridCell c in GridManager.gridCells.Values) {
			gridCells.Add (c);
		}
	}

	public void ClearAll()
	{
		if (gridCells.Count == 0) {
			UpdateGrid ();
		}
		foreach (GridCell c in gridCells) {
			c.DisableHighlight ();
		}
	}

	public void ClearAllExcept(HashSet<GridCell> cells)
	{
		foreach (GridCell c in gridCells) {
			if (!cells.Contains (c)) {
				c.DisableHighlight ();
			}
		}
	}

	public void HighlightCell(GridCell cell)
	{
		cell.EnableHighlight (selector);
	}

	public void HighlightCell(GridCell cell, Color colour)
	{
		cell.EnableHighlight (colour);
	}

	public void HighlightMoveRange(HashSet<GridCell> cells)
	{
		foreach (GridCell c in cells) {
			c.EnableHighlight (moveRange);
		}
	}

	public void HighlightMovePath(Stack<GridCell> path, GridCell start)
	{
		start.EnableHighlight (movePath);

		foreach (GridCell c in path) {
			c.EnableHighlight (movePath);
		}
	}

	public void HighlightAbilityRange(HashSet<GridCell> cells)
	{
		foreach (GridCell c in cells) {
			c.EnableHighlight (abilityRange);
		}
	}

	public void HighlightNonVisibleCell(HashSet<GridCell> cells)
	{
		foreach (GridCell c in cells) {
			c.EnableHighlight (noLineOfSight);
		}
	}

	public void HighlightAreaOfEffect(HashSet<GridCell> cells)
	{
		foreach (GridCell c in cells) {
			c.EnableHighlight (areaOfAffect);
		}
	}

	public void HighlightPlayerStart(List<GridCell> cells)
	{
		foreach (GridCell c in cells) {
			c.EnableHighlight (playerStart);
		}
	}

	public void HighlightEnemyStart(List<GridCell> cells)
	{
		foreach (GridCell c in cells) {
			c.EnableHighlight (enemyStart);
		}
	}

	public void HighlightCells(HashSet<GridCell> cells, string cellType)
	{
		switch (cellType) {
		case "Tackle":
			foreach (GridCell c in cells) {
				c.EnableHighlight (tackleArea);
			}
			break;
		}
	}
}
