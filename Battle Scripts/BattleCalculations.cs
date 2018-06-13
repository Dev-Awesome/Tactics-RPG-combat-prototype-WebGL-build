using System.Collections;
using UnityEngine;
using BattleEnums;
using System.Collections.Generic;

public static class BattleCalculations {

	public static int CalculateDodgeLockMpReduction(Unit movingUnit, List<Unit> adjacentUnits)
	{
		int highestAgi = 0;
		foreach (Unit u in adjacentUnits) {
			if (u.stats.agility > highestAgi) {
				highestAgi = u.stats.agility;
			}
		}
		float percent = ((float)movingUnit.stats.agility + 1f) / (2 * (float)highestAgi + 1f);
		percent = (percent > 1f) ? 1f : percent;
		int mpLost = Mathf.RoundToInt ((float)movingUnit.stats.mp * percent);
		return movingUnit.stats.mp - mpLost;
	}

	public static bool CalculateCrit(int critChance)
	{
		int roll = Random.Range (1, 100);
		return (roll <= critChance) ? true : false;
	}

	public static int CalculateDamage(Unit source, Unit target, int minDmg, int maxDmg, eElement element, eStat scaleFactor)
	{
		int roll = Random.Range (minDmg, maxDmg);
		float damage = 0;

		switch (scaleFactor) {
		case eStat.STR:
			damage = roll + (roll * (source.stats.strength / 100f));
			break;

		case eStat.INT:
			damage = roll + (roll * (source.stats.intelligence / 100f));
			break;

		case eStat.AGI:
			damage = roll + (roll * (source.stats.agility / 100f));
			break;

		case eStat.RES:
			damage = roll + (roll * (source.stats.resolve / 100f));
			break;

		default:
			damage = roll + (roll * (source.stats.strength / 100f));
			break;
		}
			
		float resistMultiplier = (float)target.stats.GetResistValue (element) / 100f;

		int finalValue = Mathf.RoundToInt (damage - (damage * resistMultiplier));

		return finalValue;
	}

	public static int CalculateHeal(Unit source, int minHeal, int maxHeal)
	{
		int roll = Random.Range (minHeal, maxHeal);
		float heal = roll + (roll * (source.stats.intelligence / 100f));

		int finalValue = Mathf.RoundToInt (heal);
		return finalValue;
	}

	public static int CalculateApMpLoss(Unit source, Unit target, int amount, bool isAp)
	{
		int totalLost = 0;
		for (int i = 0; i < amount; i++) {
			
			float percentRemaining = (isAp) ? (((float)target.stats.ap / (float)target.stats.baseMaxAp)) * 100f : ((float)target.stats.mp / (float)target.stats.baseMaxMp) * 100f;

			float percentChance = Mathf.Clamp((((float)source.stats.resolve + 1f) / ((float)target.stats.resolve + 1f)) * (percentRemaining / 2), 10, 90);

			int roll = Random.Range (1, 100);
			if (roll <= percentChance) {
				totalLost++;
			}
		}
		return totalLost;
	}

	public static Vector2 GetMinMaxDamageValues(Unit source, Unit target, eStat scaleFactor, int min, int max)
	{
		float minDamage = min;
		float maxDamage = max;

		switch (scaleFactor) {
		case eStat.STR:
			minDamage = minDamage + (minDamage * (source.stats.strength / 100f));
			maxDamage = maxDamage + (maxDamage * (source.stats.strength / 100f));
			break;

		case eStat.INT:
			minDamage = minDamage + (minDamage * (source.stats.intelligence / 100f));
			maxDamage = maxDamage + (maxDamage * (source.stats.intelligence / 100f));
			break;

		case eStat.AGI:
			minDamage = minDamage + (minDamage * (source.stats.agility / 100f));
			maxDamage = maxDamage + (maxDamage * (source.stats.agility / 100f));
			break;

		case eStat.RES:
			minDamage = minDamage + (minDamage * (source.stats.resolve / 100f));
			maxDamage = maxDamage + (maxDamage * (source.stats.resolve / 100f));
			break;

		default:
			minDamage = minDamage + (minDamage * (source.stats.strength / 100f));
			maxDamage = maxDamage + (maxDamage * (source.stats.strength / 100f));
			break;
		}

		return new Vector2 (Mathf.RoundToInt (minDamage), Mathf.RoundToInt (maxDamage));
	}

	public static int GetAverageDamageValue(Unit source, Unit target, eElement element, eStat scaleFactor, int min, int max)
	{
		float minDamage = min;
		float maxDamage = max;

		switch (scaleFactor) {
		case eStat.STR:
			minDamage = minDamage + (minDamage * (source.stats.strength / 100f));
			maxDamage = maxDamage + (maxDamage * (source.stats.strength / 100f));
			break;

		case eStat.INT:
			minDamage = minDamage + (minDamage * (source.stats.intelligence / 100f));
			maxDamage = maxDamage + (maxDamage * (source.stats.intelligence / 100f));
			break;

		case eStat.AGI:
			minDamage = minDamage + (minDamage * (source.stats.agility / 100f));
			maxDamage = maxDamage + (maxDamage * (source.stats.agility / 100f));
			break;

		case eStat.RES:
			minDamage = minDamage + (minDamage * (source.stats.resolve / 100f));
			maxDamage = maxDamage + (maxDamage * (source.stats.resolve / 100f));
			break;

		default:
			minDamage = minDamage + (minDamage * (source.stats.strength / 100f));
			maxDamage = maxDamage + (maxDamage * (source.stats.strength / 100f));
			break;
		}
			
		float avg = minDamage + maxDamage / 2;
		float resistMultiplier = (float)target.stats.GetResistValue (element) / 100f;
		int finalValue = Mathf.RoundToInt (avg - (avg * resistMultiplier));
		return finalValue;
	}
}
