using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldUnitController : MonoBehaviour {

	public float walkSpeed = 0.6f;
	public float runSpeed = 0.4f;

	private float inverseRunSpeed;
	private float inverseWalkSpeed;

	private bool inMotion = false;
	private bool facingLeft = true;

	private Animator animator;

	private GridCell currentLocation;
	private Stack<GridCell> path;

	void Start () {
		animator = GetComponent<Animator> ();
		inverseWalkSpeed = 1f / walkSpeed;
		inverseRunSpeed = 1f / runSpeed;
	}

	public void SetLocation(GridCell location)
	{
		gameObject.transform.position = location.transform.position;
		currentLocation = location;
	}

	public void MoveTo(GridCell targetLocation)
	{
		if (inMotion) {
			StopAllCoroutines ();
		}

		path = OverworldPathfinder.GetPath (currentLocation, targetLocation);
		StartCoroutine(Move(path, path.Count));
	}
		

	IEnumerator Move (Stack<GridCell> path, int distance) {

		inMotion = true;

		GridCell current = currentLocation;
		GridCell next = currentLocation;

		float speed = (distance > 4) ? inverseRunSpeed : inverseWalkSpeed;
		animator.SetBool ("Walking", true);

		while (path.Count > 0) {
			next = path.Peek ();
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
			path.Pop ();
		}
		animator.SetBool ("Walking", false);
		inMotion = false;
		currentLocation = current;
	}

	void Turn(GridCell start, GridCell end)
	{
		if (start != end) {
			string facing = start.GetFacing (end);
			Turn (facing);
		}
	}

	void Turn(string facing)
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

	void FlipScale () {

		Vector3 scale = new Vector3 (gameObject.transform.localScale.x * -1,
			gameObject.transform.localScale.y, gameObject.transform.localScale.z);
		gameObject.transform.localScale = scale;
	}
}
