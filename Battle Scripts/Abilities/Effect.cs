using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleEnums;

[System.Serializable]
public abstract class Effect : ScriptableObject {

	public eEffectType effectType;
	public List<eUnitType> affectedUnits;
	public bool affectsCaster;
	public bool affectsAll = true;
	public bool onlyAffectCaster = false;

	public abstract void ApplyEffect (Unit caster, GridCell target, int level, bool crit);
}
