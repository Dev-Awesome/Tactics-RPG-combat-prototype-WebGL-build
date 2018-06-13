 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleEnums;
using System.Linq;

public class AiController : MonoBehaviour {

	//Cell highlighting
	public Color moveIndicator;
	public Color targetIndicator;

	private Unit actor;
	private AiBehaviour aiBehaviour;

	private List<AiAction> actions = new List<AiAction> ();
	public HashSet<GridCell> targetCells = new HashSet<GridCell> ();
	private HashSet<GridCell> movementRange = new HashSet<GridCell> ();
	private Stack<GridCell> path = new Stack<GridCell>();

	private AiAction currentAction;
	private bool actionsRemaining;
	private bool inAction;

	private int movementPenalty;

	public void InitAiAcontroller(UnitData data)
	{
		actor = GetComponent<Unit> ();
		aiBehaviour = data.aiBehaviour;
	}

	public void StartTurn()
	{
		movementPenalty = 0;
		currentAction = null;
		path.Clear ();
		actionsRemaining = true;
		inAction = false;
		if (actor.stats.ap > 0) {
			StartCoroutine (ExecuteTurn ());
		}
	}

	public void Interrupted()
	{
		StopAllCoroutines ();
		StartTurn ();
	}

	IEnumerator ExecuteTurn()
	{
		while (actionsRemaining) {

			StartCoroutine (AssessActions ());

			if (currentAction != null) {
				inAction = true;

				if (currentAction.moveTarget != actor.currentLocation) {
					path = Pathfinder.GetPath (actor.currentLocation, currentAction.moveTarget);
					movementPenalty = actor.MovementLockPenalty ();
					actor.Move (path, currentAction.moveTarget, movementPenalty);
					yield return new WaitForSeconds (2f);
				}

				string text = actor.unitName + " used " + actor.abilityController.GetAbilityName (currentAction.abilityIndex) + ".";
				actor.guiController.ClearLogText ();
				actor.guiController.SetBattleLogText (text);

				bool crit = actor.abilityController.HasCrit (currentAction.abilityIndex);
				targetCells = actor.abilityController.GetArea (currentAction.abilityIndex, currentAction.abilityTarget);
				actor.abilityController.UseAbility (currentAction.abilityIndex, targetCells, currentAction.abilityTarget, crit);

				currentAction = null;
				inAction = false;

				yield return new WaitForSeconds (2f);
			}
		}
	}

	private IEnumerator AssessActions()
	{
		if (!inAction) {
			actions.Clear ();
			movementPenalty = actor.MovementLockPenalty ();
			movementRange = Pathfinder.GetMoveRange (actor.currentLocation, actor.stats.mp - movementPenalty);
			movementRange.Add (actor.currentLocation);

			for (int i = 0; i < actor.abilityController.abilityInfo.Count; i++) {
				if (actor.abilityController.CanUse (i)) {

					foreach (GridCell moveCell in movementRange) {
						HashSet<GridCell> abilityRange = actor.abilityController.GetRangeFrom (i, moveCell, true);

						foreach (GridCell rangeCell in abilityRange) {
							if (actor.abilityController.CanUseOnCell(i, rangeCell)) {
								if (actor.abilityController.HasArea (i)) {
									targetCells = actor.abilityController.GetArea (i, rangeCell);
									//		foreach (GridCell areaCell in targetCells) {
									//			if (areaCell.currentUnit != null) {
									//				if (!actor.IsAlly (areaCell.currentUnit)) {
									//					AddAction (i, moveCell, rangeCell, targetCells);
									//				}
									//			}
									//		}
								} else {
									if (rangeCell.currentUnit != null) {
										AddAction (i, moveCell, rangeCell);
									}
								}
							}
						}
					}
				}
			}
			//Debug.Log ("Assesment Complete - Action count: " + actions.Count);
			if (actions.Count > 0) {
				actions = actions.OrderByDescending(x => x.actionScore).ToList();
				currentAction = actions [0];
			} else {
				actionsRemaining = false;
				CheckFinalMovementOptions ();
			}
		}
		yield return null;
	}

	private void AddAction(int index, GridCell move, GridCell target, HashSet<GridCell> area = null)
	{
		int mpCost = 0;
		if (move != actor.currentLocation) {
			mpCost = Pathfinder.GetPath (actor.currentLocation, move).Count ();
		}

		AiAction act = new AiAction (index, move, actor.abilityController.GetApCost (index),
			               mpCost, target, area);
		
		aiBehaviour.AssessAction (actor, act);
		actions.Add (act);
	}

	private void CheckFinalMovementOptions()
	{
		if (actor.stats.mp - actor.MovementLockPenalty () > 0) {
			GridCell moveTarget = aiBehaviour.FindMoveTarget (actor);
			if (moveTarget != actor.currentLocation) {
				StartCoroutine (FinalMovement (moveTarget));
			} else {
				BattleController.instance.EndTurn ();
			}
		} else {
			BattleController.instance.EndTurn ();
		}
	}

	IEnumerator FinalMovement(GridCell target)
	{
		path = Pathfinder.GetPath (actor.currentLocation, target);
		movementPenalty = actor.MovementLockPenalty ();
		actor.Move (path, target, movementPenalty);
		yield return new WaitForSeconds (3f);

		BattleController.instance.EndTurn ();
	}
}