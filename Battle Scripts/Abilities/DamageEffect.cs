using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleEnums;

[CreateAssetMenu (menuName = "Abilities/Damage Effect")]
[System.Serializable]
public class DamageEffect : Effect {

	public eElement element;

	public int[] minDamagePerLevel = new int[5];
	public int[] maxDamagePerLevel = new int[5];

	[Range(1, 3)]
	public float critDamageModifier = 1.5f;

	public eStat scaleFactor = eStat.STR;

	public override void ApplyEffect(Unit caster, GridCell target, int level, bool crit)
	{
		if (target.currentUnit != null) {
			
			if (affectsAll) {
				
				int damage = BattleCalculations.CalculateDamage (caster, target.currentUnit, minDamagePerLevel [level - 1],
					maxDamagePerLevel [level - 1], element, scaleFactor);
				target.currentUnit.Damage (damage, element);
				
			} else if (affectedUnits.Contains(target.currentUnit.unitType)) {
				
				if (target.currentUnit == caster) {
					
					if (affectsCaster) {
						
						int damage = BattleCalculations.CalculateDamage (caster, target.currentUnit, minDamagePerLevel [level - 1],
							maxDamagePerLevel [level - 1], element, scaleFactor);
						target.currentUnit.Damage (damage, element);
					}
				} else {
					int damage = BattleCalculations.CalculateDamage (caster, target.currentUnit, minDamagePerLevel [level - 1],
						maxDamagePerLevel [level - 1], element, scaleFactor);
					target.currentUnit.Damage (damage, element);
				}
			}
		}
	}
}
