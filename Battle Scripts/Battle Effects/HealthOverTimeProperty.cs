using UnityEngine;
using BattleEnums;
using System.Collections.Generic;

[System.Serializable]
public class HealthOverTimeProperty {

	public eStat scaleFactor = eStat.INT;
	public eElement element;

	public MinMaxValue[] valuePerLevel = new MinMaxValue[5];
	public int[] durationPerLevel = new int[5];

	public List<eUnitType> affectedUnits = new List<eUnitType> ();

	public bool targetCasterInstead = false;
	public bool aoeHitsCaster = true;

	public void ApplyEffect(Unit source, GridCell target, int level, bool crit = false)
	{

	}
}
