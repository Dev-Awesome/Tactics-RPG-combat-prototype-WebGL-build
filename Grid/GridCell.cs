using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridCell : MonoBehaviour {

	//For serialization.
	[HideInInspector] public int x;
	[HideInInspector] public int y;

	//Pathfinding variables.
	[HideInInspector] public int g;
	[HideInInspector] public int h;
	[HideInInspector] public int f;
	[HideInInspector] public int distance;
	[HideInInspector] public GridCell parent;

	private SpriteRenderer highlight;
	private SpriteRenderer outline;
	private GridHighlighter gridHighlighter;
	private BattleInputController input;

	public GridPos gridPos { get; private set; }
	public Unit currentUnit { get; set; }

	public bool playerStart = false;
	public bool enemyStart = false;

	//Events.
	public event Action<Unit> cellHoverInfoEvent;
	public event Action cellHoverExitEvent;

	public enum eCellType {
		DEFAULT,
		IMPASSABLE,
		DISABLED,
	}

	public eCellType cellType;

	void Start()
	{
		GetComponent<SpriteRenderer> ().enabled = false;
	}

	public void PlaceTile (GridPos pos, Vector2 worldPos)
	{
		gridPos = pos;
		transform.position = worldPos;
		x = pos.x;
		y = pos.y;
	}

	public void InitCell (GridPos pos)
	{
		currentUnit = null;
		g = 0;
		h = 0;
		f = 0;
		parent = null;
		distance = int.MaxValue;
		gridPos = pos;
		outline = gameObject.transform.GetChild (0).GetComponent<SpriteRenderer> ();
		highlight = gameObject.transform.GetChild (1).GetComponent<SpriteRenderer> ();
	}

	public void EnableHighlight(Color colour)
	{
		highlight.enabled = true;
		highlight.color = colour;
	}

	public void DisableHighlight ()
	{
		highlight.enabled = false;
	}

	public void ToggleOutline(bool enabled)
	{
		if (enabled) {
			outline.enabled = true;
		} else {
			outline.enabled = false;
		}
	}

	public void SetLineColour(Color colour)
	{
		outline.color = colour;
	}
		
	public void CalcValues(GridCell parent, GridCell goal)
	{
		this.parent = parent;
		g = parent.g + 1;
		h = CalcDistance (goal);
		f = g + h;
	}

	public int CalcDistance (GridCell end)
	{
		int distance = Mathf.Abs((gridPos.x - end.gridPos.x)) + Mathf.Abs((gridPos.y - end.gridPos.y));
		return distance;
	}

	public string GetFacing (GridCell lookTo)
	{
		if (gridPos.x > lookTo.gridPos.x && gridPos.y == lookTo.gridPos.y) {
			return "NW";
		} else if (gridPos.x < lookTo.gridPos.x && gridPos.y == lookTo.gridPos.y) {
			return "SE";	
		} else if (gridPos.x == lookTo.gridPos.x && gridPos.y > lookTo.gridPos.y) {
			return "SW";
		} else if (gridPos.x == lookTo.gridPos.x && gridPos.y < lookTo.gridPos.y) {
			return "NE";	
		} else {
			return GetNearestFacing (lookTo.gridPos);
		}
	}

	private string GetNearestFacing (GridPos pos)
	{
		int dx = Mathf.Abs(pos.x - gridPos.x);
		int dy = Mathf.Abs(pos.y - gridPos.y);

		if (pos.x > gridPos.x && pos.y > gridPos.y) {
			if (dx > dy) {
				return "SE";
			} else {
				return "NE";
			}
		} else if (pos.x < gridPos.x && pos.y < gridPos.y) { 
			if (dx > dy) {
				return "NW";
			} else {
				return "SW";
			}
		} else if (pos.x > gridPos.x && pos.y < gridPos.y) { 
			if (dx > dy) {
				return "SE";
			} else {
				return "SW";
			}
		} else {							
			if (dx > dy) {
				return "NW";
			} else {
				return "NE";
			}
		}
	}

	public void SetCurrentUnit(Unit u)
	{
		if (currentUnit == null) {
			currentUnit = u;
		} else {
			Debug.Log ("Error: Current cell is already occupied.");
		}
	}
		
	public bool CanMoveThrough()
	{
		return (cellType == eCellType.DEFAULT && currentUnit == null);
	}

	public bool CanShootThrough()
	{
		return (cellType != eCellType.DISABLED && currentUnit == null);
	}

	public bool CanTarget(bool requiresFreeCell)
	{
		if (requiresFreeCell) {
			return cellType == eCellType.DEFAULT && currentUnit == null;
		}

		return (cellType == eCellType.DEFAULT);
	}

	//Hover info.
	void OnMouseEnter()
	{
		if ((BattleInputController.inputState == BattleInputController.eInputState.PLAYER_DEFAULT ||
		    BattleInputController.inputState == BattleInputController.eInputState.NPC_DEFAULT) & currentUnit != null) {

			BattleInputController.mouseHoverEvent = true;

			if (cellHoverInfoEvent != null) {
				cellHoverInfoEvent (currentUnit);
			}
			currentUnit.SetIndicatorEnabled (true);

			if (currentUnit.stats.mp > 0) {
				int mpLoss = currentUnit.MovementLockPenalty ();
				HashSet<GridCell> remainingMoveRange = Pathfinder.GetMoveRange (this, currentUnit.stats.mp - mpLoss);
				HashSet<GridCell> unitMoveRange = Pathfinder.GetMoveRange (this, currentUnit.stats.mp);

				if (gridHighlighter == null) {
					gridHighlighter = GameObject.FindGameObjectWithTag ("Battle Controller").GetComponent<GridHighlighter> ();
				}
				gridHighlighter.ClearAll ();

				if (currentUnit == BattleController.instance.currentUnit) {
					if (input == null) {
						input = GameObject.FindGameObjectWithTag ("Battle Controller").GetComponent<BattleInputController> ();
					}
					input.ShowMovementRange ();
				} else {
					gridHighlighter.HighlightNonVisibleCell (unitMoveRange);
					gridHighlighter.HighlightMoveRange (remainingMoveRange);
				}
			}
		} else if (BattleInputController.inputState == BattleInputController.eInputState.TARGET_SELECT && currentUnit != null) {
			if (cellHoverInfoEvent != null) {
				cellHoverInfoEvent (currentUnit);
			}
			currentUnit.SetIndicatorEnabled (true);
		}
	}

	void OnMouseExit()
	{
		if ((BattleInputController.inputState == BattleInputController.eInputState.PLAYER_DEFAULT ||
			BattleInputController.inputState == BattleInputController.eInputState.NPC_DEFAULT) && currentUnit != null) {

			BattleInputController.mouseHoverEvent = false;
			cellHoverExitEvent ();

			if (BattleController.instance.currentUnit != currentUnit) {
				currentUnit.SetIndicatorEnabled (false);
			}

			if (gridHighlighter == null) {
				gridHighlighter = GameObject.FindGameObjectWithTag ("Battle Controller").GetComponent<GridHighlighter> ();
			}
			gridHighlighter.ClearAll ();
		} else if (BattleInputController.inputState == BattleInputController.eInputState.TARGET_SELECT && currentUnit != null) {
			if (cellHoverInfoEvent != null) {
				cellHoverExitEvent ();
			}
			if (BattleController.instance.currentUnit != currentUnit) {
				currentUnit.SetIndicatorEnabled (false);
			}
		}
	}
}
