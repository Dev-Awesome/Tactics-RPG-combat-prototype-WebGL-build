using UnityEngine;

public static class EffectHandler {

	public static void ApplyDamage(DamageProperty prop, Unit source, GridCell target, int level)
	{
		if (prop.targetCasterOverride) {
			int damage = BattleCalculations.CalculateDamage (source, source, prop.damagePerLevel [level].min, prop.damagePerLevel [level].max, prop.element, prop.scaleFactor);
			source.Damage (damage, prop.element);
		}

		else if (target.currentUnit != null) {

			if (target.currentUnit == source) {

				if (prop.affectsCaster) {
					int damage = BattleCalculations.CalculateDamage (source, source, prop.damagePerLevel [level].min, prop.damagePerLevel [level].max, prop.element, prop.scaleFactor);
					source.Damage (damage, prop.element);
				}
			}

			else if (prop.affectedUnits.Contains (target.currentUnit.unitType)) {
				int damage = BattleCalculations.CalculateDamage (source, target.currentUnit, prop.damagePerLevel [level].min, prop.damagePerLevel [level].max, prop.element, prop.scaleFactor);
				target.currentUnit.Damage (damage, prop.element);
			}
		}
	}
}
