using UnityEngine;
using BattleEnums;
using System.Collections.Generic;

[System.Serializable]
public class DisplacementProperty {

	public bool targetCasterOverride = false;
	public bool affectsCaster = true;
	public bool affectsAllyPlayers = true;
	public bool affectsAllySummons = true;
	public bool affectsEnemyPlayers = true;
	public bool affectsEnemySummons = true;

	//Remove --
	public List<eUnitType> affectedUnits = new List<eUnitType> ();

	//For animating - refactor later.
	public float blinkDelay = 0.3f;

	public eDisplacementType displacementType = eDisplacementType.PUSH;
	public int[] distancePerLevel = new int[5];
}
