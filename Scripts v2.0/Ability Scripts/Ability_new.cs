using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data Objects/Abilities/Ability")]
public class Ability_new : ScriptableObject {

	public string abilityName;
	public string description;

	public Texture2D icon;

	public int[] apCostPerLvl = new int[5];
	public int[] cooldownPerLvl = new int[5];
	public int[] castsPerTurnPerLvl = new int[5];
	public int[] castsPerTargetPerLvl = new int[5];

	public int minRange;
	public int[] maxRangePerLvl = new int[5];

	public bool adjustableRange;
	public bool requiresTarget;
	public bool requiresFreeCell;
	public bool requiresLoS;
	public bool isLinear;

	public enum AreaType {
		NONE,
		CIRCLE,
		CROSS,
		H_LINE,
		V_LINE,
		CONE
	}

	public AreaType areaType;

	public enum StatScaling {
		NONE,
		STR,
		INT,
		AGI,
		RES
	}
		
	public StatScaling statScaling;

	public int[] areaSizePerLvl = new int[5];

	public TargetingModule targetingModule;

	public int GetApCost(int lvl)
	{
		return apCostPerLvl [lvl - 1];
	}

	public int GetCooldown(int lvl)
	{
		return cooldownPerLvl [lvl - 1];
	}

	public int GetTurnLimit(int lvl)
	{
		return castsPerTurnPerLvl [lvl - 1];
	}

	public int GetTargetLimit(int lvl)
	{
		return castsPerTargetPerLvl [lvl - 1];
	}

	public Vector2 GetRangeValues(int lvl)
	{
		return new Vector2 (minRange, maxRangePerLvl [lvl - 1]);
	}
}
