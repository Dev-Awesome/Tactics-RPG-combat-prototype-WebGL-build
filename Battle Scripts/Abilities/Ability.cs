using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleEnums;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu (menuName = "Abilities/New Ability")]
public class Ability : ScriptableObject {

	public string abilityName;
	public string description;
	public Sprite icon;

	public eAnimationType animationTrigger = eAnimationType.SPECIAL_1;

	public int baseCritChance = 0;

	public List<DamageProperty> damageEffects = new List<DamageProperty>();
	public List<DamageProperty> critDamageEffects = new List<DamageProperty> ();

	public List<HealProperty> healEffects = new List<HealProperty>();
	public List<HealProperty> critHealEffects = new List<HealProperty>();

	public List<DisplacementProperty> displacementEffects = new List<DisplacementProperty>();
	public List<DisplacementProperty> critDisplacementEffects = new List<DisplacementProperty>();

	public List<BuffDebuffProperty> buffDebuffEffects = new List<BuffDebuffProperty>();
	public List<BuffDebuffProperty> critBuffDebuffEffects = new List<BuffDebuffProperty>();

	public int[] apCostPerLvl = new int[5];
	public int[] cooldownPerLvl = new int[5];
	public int[] castsPerTurnPerLvl = new int[5];
	public int[] castsPerTargetPerLvl = new int[5];

	public bool adjustableRange = true;
	public int[] minRangePerLvl = new int[5];
	public int[] maxRangePerLvl = new int[5];

	public bool requiresTarget = false;
	public bool requiresFreeCell = false;
	public int noLoSFromLvl = 6;
	public int nonLinearFromLvl = 0;

	public eAreaType areaType = eAreaType.SINGLE;
	public int[] areaSizePerLvl = new int[5];
}
