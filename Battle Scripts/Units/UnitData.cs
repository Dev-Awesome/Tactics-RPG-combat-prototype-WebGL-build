using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleEnums;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu (menuName = "Units/Unit Data")]
public class UnitData : ScriptableObject {

	[Header("Info")]
	public string unitName;
	public string classification;
	public int level;
	public eUnitType unitType;

	public AnimatorOverrideController animatorController;

	[Header("Stats")]
	public int hp;
	public int ap;
	public int mp;
	public int initiative;

	public int strength;
	public int intelligence;
	public int agility;
	public int resolve;

	[Header("Resistances")]
	public int neautral;
	public int earth;
	public int fire;
	public int air;
	public int water;

	[Header("AI")]
	public AiBehaviour aiBehaviour;

	[Header("Abilities")]
	public AbilityContainer[] abilities;

	[System.Serializable]
	public struct AbilityContainer {
		public Ability ability;
		public int level;
	}
}
