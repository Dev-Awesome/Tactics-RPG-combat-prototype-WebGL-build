using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleEnums;

[CreateAssetMenu (menuName = "Abilities/Heal Effect")]
public class HealEffect : Effect {

	public int[] minHealPerLevel = new int[5];
	public int[] maxHealPerLevel = new int[5];

	[Range(1, 3)]
	public float critHealModifier = 1.5f;

	public eStat scaleFactor = eStat.INT;

	public override void ApplyEffect(Unit caster, GridCell target, int level, bool crit)
	{
		int heal = BattleCalculations.CalculateHeal (caster, minHealPerLevel [level - 1], maxHealPerLevel [level - 1]);
		
		if (target.currentUnit != null) {

			if (affectsAll) {

				//Hit
				target.currentUnit.Heal(heal);
				

			} else if (affectedUnits.Contains(target.currentUnit.unitType)) {

				if (target.currentUnit == caster) {

					if (affectsCaster) {

						//Hit
						target.currentUnit.Heal(heal);
					}
				} else {

					//Hit
					target.currentUnit.Heal(heal);
				}
			}
		}
	}
}
