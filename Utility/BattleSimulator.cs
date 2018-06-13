using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSimulator : MonoBehaviour {

	[Header("Units")]
	public List<UnitData> playerUnits;
	public List<UnitData> enemyUnits;

	void Start()
	{
		List<UnitData> units = new List<UnitData> ();

		for (int i = 0; i < playerUnits.Count; i ++) {
			units.Add (playerUnits [i]);
		}

		for (int i = 0; i < enemyUnits.Count; i ++) {
			units.Add (enemyUnits [i]);
		}

		BattleController.instance.InitBattle (units, GameObject.FindGameObjectWithTag("Game Manager").GetComponent<GridManager>());
	}
}
