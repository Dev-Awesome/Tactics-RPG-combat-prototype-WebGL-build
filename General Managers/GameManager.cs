using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	//Singleton pattern.
	private static GameManager _instance;
	public static GameManager instance { get { return _instance; } }

	void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(this.gameObject);
		} else {
			_instance = this;
		}
	}

	void OnDestroy()
	{
		if (this == _instance) { 
			_instance = null;
		}
	}

	public OverworldUnitController player;
	public Color indicator;
	public List<UnitData> playerUnits;

	public BattleController battleController;

	private bool inCombat = false;

	private GridManager gridManager;

	private Dictionary<GridPos, GridCell> cells;

	void Start()
	{
		gridManager = GetComponent<GridManager> ();
		gridManager.HideGridLines ();
		cells = GridManager.gridCells;
		player = Instantiate (player, GameObject.Find("OverWorld Units").transform).GetComponent<OverworldUnitController> ();
		player.SetLocation (cells [new GridPos (2, 2)]);
	}

	public void StartBattle(List<UnitData> enemyUnits)
	{
		inCombat = true;

		GameObject.Find ("OverWorld Units").SetActive (false);
		List<UnitData> units = new List<UnitData> ();

		for (int i = 0; i < playerUnits.Count; i ++) {
			units.Add (playerUnits [i]);
		}

		for (int i = 0; i < enemyUnits.Count; i ++) {
			units.Add (enemyUnits [i]);
		}

		battleController = Instantiate (battleController);
		battleController.InitBattle (units, gridManager);
		gridManager.ShowGridLines ();
	}

	void Update()
	{
		if (!inCombat) {
			if (Input.GetMouseButtonDown (0)) {
				MouseClick ();
			}
		}
	}

	void MouseClick()
	{
		RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);
		if (hit.collider != null && hit.collider.gameObject.tag == "Grid Cell") {
			GridCell target = hit.collider.GetComponent<GridCell> ();
			StartCoroutine (IndicateCell (target));
			player.MoveTo (target);
		}
	}

	IEnumerator IndicateCell(GridCell cell)
	{
		cell.EnableHighlight (indicator);
		yield return new WaitForSeconds (.3f);
		cell.DisableHighlight ();
	}
}
