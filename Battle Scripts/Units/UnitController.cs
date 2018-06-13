using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BattleEnums;

public class UnitController : MonoBehaviour {

	public float walkSpeed = 0.4f;
	public float runSpeed = 0.6f;

	private float inverseRunSpeed;
	private float inverseWalkSpeed;
	private Animator animator;
	private bool facingLeft = true;
	private Unit unit;

	private const float pushSpeed = 4f;

	public event Action animationCompleted;

	void Awake () {
		animator = gameObject.transform.GetChild (0).GetComponent<Animator> ();
		unit = GetComponent<Unit> ();
		inverseWalkSpeed = 1f / walkSpeed;
		inverseRunSpeed = 1f / runSpeed;
	}

	public void InitUnitController(UnitData data)
	{
		animator.runtimeAnimatorController = data.animatorController;
	}

	public void MoveUnit (GridCell start, Stack<GridCell> path) {
		BattleInputController.SetInputState (BattleInputController.eInputState.DISABLED);
		StartCoroutine (Move (path, start, path.Count));
	}

	public void Turn(string facing)
	{
		switch (facing) {
		case "SW":
			animator.SetBool ("FacingFront", true);
			if (!facingLeft) {
				FlipScale ();
				facingLeft = true;
			}
			break;

		case "SE":
			animator.SetBool ("FacingFront", true);
			if (facingLeft) {
				FlipScale ();
				facingLeft = false;
			}
			break;

		case "NW":
			animator.SetBool ("FacingFront", false);
			if (!facingLeft) {
				FlipScale ();
				facingLeft = true;
			}
			break;

		case "NE":
			animator.SetBool ("FacingFront", false);
			if (facingLeft) {
				FlipScale ();
				facingLeft = false;
			}
			break;
		}
	}

	public void Turn(GridCell start, GridCell end)
	{
		if (start != end) {
			string facing = start.GetFacing (end);
			Turn (facing);
		}
	}

	public void PushUnit(GridCell target)
	{
		StartCoroutine (Push (target));
	}

	private IEnumerator Push(GridCell target)
	{
		BattleInputController.SetInputState (BattleInputController.eInputState.DISABLED);
		while (transform.position != target.transform.position) {

			float sqrRemainingDistance = (transform.position - target.transform.position).sqrMagnitude;

			while (sqrRemainingDistance > float.Epsilon) {

				transform.position = Vector3.MoveTowards (transform.position, target.transform.position, pushSpeed * Time.deltaTime);
				sqrRemainingDistance = (transform.position - target.transform.position).sqrMagnitude;
				yield return null;
			}
		}

		unit.DisplaceComplete (target);

		if (BattleController.instance.battleState == BattleController.eBattleState.PLAYER_TURN) {
			BattleInputController.SetInputState (BattleInputController.eInputState.PLAYER_DEFAULT);
		} else {
			BattleInputController.SetInputState (BattleInputController.eInputState.NPC_DEFAULT);
		}
	}
		
	private IEnumerator Move (Stack<GridCell> path, GridCell start, int distance) {

		GridCell current = start;
		GridCell next = start;
		int totalDistance = 0;
		float speed = (distance > 4) ? inverseRunSpeed : inverseWalkSpeed;
		animator.SetBool ("Walking", true);

		while (path.Count > 0) {
			
			next = path.Peek ();

			//Set animations.
			Turn(current, next);

			while (transform.position != next.transform.position) {
				
				float sqrRemainingDistance = (transform.position - next.transform.position).sqrMagnitude;

				while (sqrRemainingDistance > float.Epsilon) {

					transform.position = Vector3.MoveTowards (transform.position, next.transform.position, speed * Time.deltaTime);
					sqrRemainingDistance = (transform.position - next.transform.position).sqrMagnitude;
					yield return null;
				}

			}
				
			current = next;
			totalDistance++;
			if (unit.EnemyIsAdjacent (current)) {
				ForceStop (totalDistance, current);
				yield break;
			}

			path.Pop ();
		}

		animator.SetBool ("Walking", false);
		if (animationCompleted != null) {
			animationCompleted ();
		}
		unit.MoveComplete (totalDistance, next);
	}

	private void ForceStop(int distance, GridCell newPos)
	{
		animator.SetBool ("Walking", false);
		if (animationCompleted != null) {
			animationCompleted ();
		}
		unit.MoveComplete (distance, newPos);
		unit.MovementInterupted ();
	}

	public void UseAbility(eAnimationType animationType, GridCell target)
	{
		Turn (unit.currentLocation, target);

		switch (animationType) {
		case eAnimationType.SPECIAL_1:
			animator.SetTrigger ("Special 1");
			break;
		}
	}

	public void HitAnimation()
	{
		animator.SetTrigger ("Hurt");
	}

	void FlipScale () {

		Vector3 scale = new Vector3 (gameObject.transform.localScale.x * -1,
			gameObject.transform.localScale.y, gameObject.transform.localScale.z);
		gameObject.transform.localScale = scale;
	}
}
