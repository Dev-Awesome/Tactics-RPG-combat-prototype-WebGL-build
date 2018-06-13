using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleEnums;

public class EnemyUnit : Unit {

	public AiController aiController { get; private set; }

	public override void StartTurn()
	{
		base.StartTurn ();
		aiController.StartTurn ();
	}

	public override void EndTurn()
	{
		base.EndTurn ();
	}

	public override void Init(UnitData data)
	{
		base.Init (data);
		aiController = GetComponent<AiController> ();
		aiController.InitAiAcontroller (data);
	}

	public override void MovementInterupted()
	{
		aiController.Interrupted ();
	}

	public override bool EnemyIsAdjacent(GridCell location)
	{
		for (int x = -1; x <= 1; x++) {
			GridPos pos = new GridPos (location.gridPos.x - x, location.gridPos.y);
			if (GridManager.gridCells.ContainsKey (pos)) {
				GridCell nextGridCell = GridManager.gridCells [pos];
				if (nextGridCell != location && nextGridCell.currentUnit != null) {
					if ((nextGridCell.currentUnit.unitType == eUnitType.ALLY 
						|| nextGridCell.currentUnit.unitType == eUnitType.ALLIED_SUMMON) && nextGridCell.currentUnit != this) {
						return true;
					}
				}
			}
		}
		for (int y = -1; y <= 1; y++) {
			GridPos pos = new GridPos (location.gridPos.x, location.gridPos.y - y);
			if (GridManager.gridCells.ContainsKey (pos)) {
				GridCell nextGridCell = GridManager.gridCells [pos];
				if (nextGridCell != location && nextGridCell.currentUnit != null) {
					if ((nextGridCell.currentUnit.unitType == eUnitType.ALLY 
						|| nextGridCell.currentUnit.unitType == eUnitType.ALLIED_SUMMON) && nextGridCell.currentUnit != this) {
						return true;
					}
				}
			}
		}
		return false;
	}

	public override int MovementLockPenalty()
	{
		List<Unit> adjacentEnemies = new List<Unit> ();
		for (int x = -1; x <= 1; x++) {
			GridPos pos = new GridPos (currentLocation.gridPos.x - x, currentLocation.gridPos.y);
			if (GridManager.gridCells.ContainsKey (pos)) {
				GridCell nextGridCell = GridManager.gridCells [pos];
				if (nextGridCell.currentUnit != null) {
					Unit unit = nextGridCell.currentUnit;
					if (unit.unitType == eUnitType.ALLY || unit.unitType == eUnitType.ALLIED_SUMMON) {
						if (unit != this) {
							adjacentEnemies.Add (unit);
						}
					}
				}
			}
		}
		for (int y = -1; y <= 1; y++) {
			GridPos pos = new GridPos (currentLocation.gridPos.x, currentLocation.gridPos.y - y);
			if (GridManager.gridCells.ContainsKey (pos)) {
				GridCell nextGridCell = GridManager.gridCells [pos];
				if (nextGridCell.currentUnit != null) {
					Unit unit = nextGridCell.currentUnit;
					if (unit.unitType == eUnitType.ALLY || unit.unitType == eUnitType.ALLIED_SUMMON) {
						if (unit != this) {
							adjacentEnemies.Add (unit);
						}
					}
				}
			}
		}
		if (adjacentEnemies.Count > 0) {
			int mpLoss = BattleCalculations.CalculateDodgeLockMpReduction (this, adjacentEnemies);
			return mpLoss;
		} else {
			return 0;
		}
	}
}
