using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BattleEnums;
using System;

public class BattleController : MonoBehaviour {

	//Singleton pattern.
	private static BattleController _instance;
	public static BattleController instance { get { return _instance; } }

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

	//Actions.
	public event Action<eBattleState> stateChange;

	//Battle controller.
	public PlayerUnit basePlayerUnit;
	public EnemyUnit baseEnemyUnit;

	public int turnNumber { get; private set; }

	private int unitIndexCounter = 0;
	public List<Unit> units { get; private set; }
	private List<Unit> playerUnits;
	private List<Unit> enemyUnits;
	private GridManager grid;
	private Transform unitHolder;
	private int playerCount = 0;
	private int enemyCount = 0;

	public List<GridCell> playerStartPos { get; private set; }
	public List<GridCell> enemyStartPos { get; private set; }

	public Unit currentUnit { get; private set; }

	public enum eBattleState {
		INITIALISATION,
		UNIT_PLACEMENT,
		PLAYER_TURN,
		AI_TURN,
		VICTORY,
		DEFEAT
	}

	public eBattleState battleState { get; private set; }

	private void ChangeState(eBattleState state)
	{
		battleState = state;
		if (stateChange != null) {
			stateChange (battleState);
		} else {
			Debug.Log ("Event is null");
		}
	}

	public void InitBattle(List<UnitData> unitData, GridManager gridManager)
	{
		ChangeState (eBattleState.INITIALISATION);
		grid = gridManager;
		grid.GetActiveGrid ();

		unitHolder = new GameObject ("Unit Holder").transform;

		List<Unit> temp = new List<Unit> ();
		playerUnits = new List<Unit> ();
		enemyUnits = new List<Unit> ();
		for (int i = 0; i < unitData.Count; i++) {
			if (unitData [i].unitType == eUnitType.ALLY) {
				Unit p = Instantiate (basePlayerUnit, unitHolder);
				p.Init (unitData [i]);
				temp.Add (p);
				playerUnits.Add (p);
			} else {
				Unit p = Instantiate (baseEnemyUnit, unitHolder);
				p.Init (unitData [i]);
				temp.Add (p);
				enemyUnits.Add (p);
			}
		}

		units = temp.OrderByDescending(x => x.stats.initiative).ToList();
		turnNumber = 0;

		playerCount = playerUnits.Count;
		enemyCount = enemyUnits.Count;

		PlaceEnemyUnits ();
		PlacePlayerUnits ();

		ChangeState (eBattleState.UNIT_PLACEMENT);
	}

	private void PlaceEnemyUnits()
	{
		enemyStartPos = grid.GetEnemyStartPos ();

		foreach (Unit unit in enemyUnits) {
			bool found = false;
			while (!found) {
				int index = UnityEngine.Random.Range (0, enemyStartPos.Count - 1);
				GridCell cell = enemyStartPos [index];
				if (cell.currentUnit == null) {
					unit.PlaceUnitOnCell (cell);
					found = true;
				}
			}
		}
	}

	private void PlacePlayerUnits()
	{
		playerStartPos = grid.GetPlayerStartPos ();

		foreach (Unit unit in playerUnits) {
			bool found = false;
			while (!found) {
				int index = UnityEngine.Random.Range (0, playerStartPos.Count - 1);
				GridCell cell = playerStartPos [index];
				if (cell.currentUnit == null) {
					unit.PlaceUnitOnCell (cell);
					found = true;
				}
			}
		}
	}

	private void NextUnit () {

		int unitIndex = unitIndexCounter % units.Count;
		Unit u = units [unitIndex];

		if (unitIndex == 0) {
			turnNumber++;
		}
		unitIndexCounter++;

		currentUnit = u;
		currentUnit.StartTurn ();

		switch (currentUnit.unitType) {
		case eUnitType.ALLY:
			ChangeState (eBattleState.PLAYER_TURN);
			break;

		case eUnitType.ENEMY:
			ChangeState (eBattleState.AI_TURN);
			break;

		case eUnitType.ALLIED_SUMMON:
			ChangeState (eBattleState.AI_TURN);
			break;

		case eUnitType.ENEMY_SUMMON:
			ChangeState (eBattleState.AI_TURN);
			break;
		}
	}

	public void UnitDied(Unit unit)
	{
		units.Remove (unit);
		unit.currentLocation.currentUnit = null;

		if (unit.unitType == eUnitType.ALLY) {
			playerCount--;
		} else if (unit.unitType == eUnitType.ENEMY) {
			enemyCount--;
		}

		unit.gameObject.SetActive (false);

		if (enemyCount <= 0) {
			ChangeState (eBattleState.VICTORY);
			EndBattle ();
		} else if (playerCount <= 0) {
			ChangeState (eBattleState.DEFEAT);
			EndBattle ();
		}

		if (unit == currentUnit) {
			NextUnit ();
		}
	}

	public void StartBattle()
	{
		NextUnit ();
	}

	private void EndBattle()
	{
		print ("END");
		foreach (Unit u in units) {
			u.gameObject.SetActive (false);
		}
		Destroy (unitHolder.gameObject, .2f);
	}

	public void EndTurn()
	{
		currentUnit.EndTurn ();
		NextUnit ();
	}
}
