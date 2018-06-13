using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTester : MonoBehaviour {

	public bool fullTestMode = false;

	[Header("Projectile Properties")]
	public GameObject mainProjectile;
	public ParticleSystem mainParticleSystem;
	public float time = 1f;

	[Header("Dummy Units")]
	public Animator testUnit;
	public Animator targetDummy;

	private GridManager gridManager;

	private bool isPlaying = false;

	void Start()
	{
		if (fullTestMode) {
			gridManager = GetComponent<GridManager> ();
			testUnit = Instantiate (testUnit).GetComponent<Animator> ();
			targetDummy = Instantiate (targetDummy).GetComponent<Animator> ();

			gridManager.GetActiveGrid ();
			targetDummy.transform.position = GridManager.gridCells [new GridPos (4, 2)].transform.position;
			testUnit.transform.position = GridManager.gridCells [new GridPos (4, 7)].transform.position;
			targetDummy.SetBool ("FacingFront", false);
			targetDummy.GetComponent<SpriteRenderer> ().flipX = true;
		} else {
			GameObject.Find ("Tile Container").SetActive (false);
			GameObject.Find ("background00").SetActive (false);
		}

		mainProjectile.SetActive (false);
	}

	void Update()
	{
		HandleMouseClicks ();
	}

	public void SetActiveAnimation(int index)
	{
		
	}

	void Activate()
	{
		if (!isPlaying) {
			Vector3 target = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			target.z = 1f;
			StartCoroutine (Simulate (target));
		}
	}

	void HandleMouseClicks()
	{
		if (Input.GetMouseButtonDown (0)) {
			Activate ();
		}
	}

	IEnumerator Simulate(Vector3 target)
	{
		mainProjectile.SetActive (true);
		isPlaying = true;
		iTween.MoveTo (mainProjectile, target, time);
		while (mainParticleSystem.IsAlive()) {
			yield return null;
		}
		isPlaying = false;
		mainProjectile.SetActive (false);

	}
}
