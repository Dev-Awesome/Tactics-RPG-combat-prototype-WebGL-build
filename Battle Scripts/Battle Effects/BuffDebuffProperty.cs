using UnityEngine;
using BattleEnums;
using System.Collections.Generic;

[System.Serializable]
public class BuffDebuffProperty {

	public bool targetCasterOverride = false;
	public bool affectsCaster = true;
	public bool affectsAllyPlayers = true;
	public bool affectsAllySummons = true;
	public bool affectsEnemyPlayers = true;
	public bool affectsEnemySummons = true;

	//Remove --
	public List<eUnitType> affectedUnits = new List<eUnitType> ();

	public eStat stat = eStat.MAX_AP;
	public MinMaxValue[] valuePerLevel = new MinMaxValue[5];
	public int[] durationPerLevel = new int[5];
	public bool resistable = false;
	public bool isResistanceModifier = false;
	public eElement resistance;
	public bool isBuff = false;
}
