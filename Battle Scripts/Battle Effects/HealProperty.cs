using UnityEngine;
using BattleEnums;
using System.Collections.Generic;

[System.Serializable]
public class HealProperty {

	public bool targetCasterOverride = false;
	public bool affectsCaster = true;
	public bool affectsAllyPlayers = true;
	public bool affectsAllySummons = true;
	public bool affectsEnemyPlayers = true;
	public bool affectsEnemySummons = true;

	//Remove --
	public List<eUnitType> affectedUnits = new List<eUnitType> ();

	public MinMaxValue[] healPerLevel = new MinMaxValue[5];
	public eStat scaleFactor = eStat.INT;
}
