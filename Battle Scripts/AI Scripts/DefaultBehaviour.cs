using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleEnums;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu (menuName = "AI/Default Ability Beheaviour")]
public class DefaultBehaviour : AiBehaviour {

	public override void AssessAction (Unit actor, AiAction action)
	{
		float score = action.actionScore;



		int actionCostPenalty = (action.apCost * 2) + (action.mpCost * 2);
		score -= actionCostPenalty;

		action.actionScore = Mathf.RoundToInt (score);
	}

	private int EstimateDamage(Unit actor, AiAction action)
	{
		int damage = actor.abilityController.EstimateTotalDamage (action.abilityIndex, action.abilityTarget.currentUnit);
		//float percentOfRemainingHealth = ((float)damage / (float)action.abilityTarget.currentUnit.stats.hp) * 100f;
		//int score = Mathf.RoundToInt (percentOfRemainingHealth * 0.5f);
		return damage;
	}

	private int EstimateDamageInArea(Unit actor, AiAction action)
	{
		int allyDamage = 0;
		int enemyDamage = 0;

		foreach (GridCell cell in action.area) {
			if (cell.currentUnit != null) {
				if (cell.currentUnit.IsAlly(actor)) {
					allyDamage += EstimateDamage (actor, action);
				} else {
					enemyDamage += EstimateDamage (actor, action);
				}
			}
		}
		//Debug.Log (enemyDamage + " - " + allyDamage);
		return enemyDamage - allyDamage;
	}

	public override GridCell FindMoveTarget(Unit actor)
	{
		if (actor.stats.mp - actor.MovementLockPenalty () <= 0) {
			return actor.currentLocation;
		}

		Unit target = FindNearestEnemy (actor);

		int movePenalty = actor.MovementLockPenalty ();
		HashSet<GridCell> moveRange = Pathfinder.GetMoveRange (actor.currentLocation, actor.stats.mp - movePenalty);

		GridCell targetLocation = actor.currentLocation;
		int shortestDistance = int.MaxValue;

		foreach (GridCell cell in moveRange) {
			int distance = cell.CalcDistance (target.currentLocation);
			if (distance < shortestDistance) {
				shortestDistance = distance;
				targetLocation = cell;
			}
		}

		return targetLocation;
	}

	private Unit FindNearestEnemy(Unit actor)
	{
		int shortestDistance = int.MaxValue;
		Unit targetUnit = null;

		foreach (Unit unit in BattleController.instance.units) {
			if (unit.unitType != eUnitType.ENEMY && unit.unitType != eUnitType.ENEMY_SUMMON) {
				int distance = unit.currentLocation.CalcDistance (actor.currentLocation);
				if (distance < shortestDistance) {
					shortestDistance = distance;
					targetUnit = unit;
				}
			}
		}
		return targetUnit;
	}
}
