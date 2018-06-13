using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetingModule : ScriptableObject {

	protected Dictionary<GridPos, GridCell> grid;

	protected HashSet<GridCell> targetable;
	protected HashSet<GridCell> untargetable;
	protected HashSet<GridCell> area;

	public abstract void TargetSelect (GridCell origin, int level, int bonusRange, Ability_new ability);

	public abstract List<Unit_new> GetAffectedUnits ();

	protected void Reset()
	{
		grid = GridManager.gridCells;

		if (targetable == null) {
			targetable = new HashSet<GridCell> ();
		} else {
			targetable.Clear ();
		}

		if (untargetable == null) {
			untargetable = new HashSet<GridCell> ();
		} else {
			untargetable.Clear ();
		}

		if (area == null) {
			area = new HashSet<GridCell> ();
		} else {
			area.Clear ();
		}
	}
}
