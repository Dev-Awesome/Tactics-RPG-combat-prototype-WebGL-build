using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleEnums;

[CreateAssetMenu (menuName = "Abilities/Status Effect")]
public class StatusModifier : Effect {

	public eStat stat;
	public int[] durationPerLevel = new int[5];
	public int[] valuePerLevel = new int[5];
	public bool isResistable = false;
	public bool applyEachTurn = false;
	public eStat successScaleFactor = eStat.RES;

	public override void ApplyEffect(Unit caster, GridCell target, int level, bool crit)
	{
		if (onlyAffectCaster) {
			caster.ApplyStatModifier (caster, this, level);
		} else if (IsAffected (caster, target)) {
			target.currentUnit.ApplyStatModifier (caster, this, level);
		}
	}

	private bool IsAffected(Unit caster, GridCell target)
	{
		if (target.currentUnit != null) {
			if (affectsAll) {
				return true;
			} else if (affectedUnits.Contains (target.currentUnit.unitType)) {
				if (target.currentUnit != caster) {
					return true;
				} else if (affectsCaster) {
					return true;
				}
			}
		}
		return false;
	}

	private int CalculateApMpLossValue(Unit caster, Unit target, int val)
	{
		if (!isResistable) {
			return val;
		}
		if (stat == eStat.MAX_AP) {
			int valueLost = -BattleCalculations.CalculateApMpLoss (caster, target, val, true);
			return valueLost;
		} else if (stat == eStat.MAX_MP) {
			int valueLost = -BattleCalculations.CalculateApMpLoss (caster, target, val, false);
			return valueLost;
		} else {
			return val;
		}
	}

	public void ApplyImmediately(Unit source, Unit target, int level, StatusController.StatusEffect effectData)
	{
		int finalValue = valuePerLevel [level - 1];

		//Debuff
		if (valuePerLevel [level - 1] < 0) {
			if ((stat == eStat.MAX_AP || stat == eStat.MAX_MP) && isResistable) {
				finalValue = CalculateApMpLossValue (source, target, Mathf.Abs(valuePerLevel [level - 1]));
				int valueResisted = Mathf.Abs(valuePerLevel [level - 1]) + finalValue;
				target.stats.ModifyStatValue (stat, finalValue);
				effectData.SetValue (finalValue);
				string statName = (stat == eStat.MAX_AP) ? " Ap." : " Mp.";
				target.AddToTextLogBuffer (target.unitName + " resisted the loss of " + Mathf.Abs (valueResisted) + statName);
				target.AddToTextLogBuffer (target.unitName + " lost " + Mathf.Abs (finalValue) + statName);
			} else {
				target.stats.ModifyStatValue (stat, valuePerLevel [level - 1]);
				effectData.SetValue (valuePerLevel [level - 1]);
				target.AddToTextLogBuffer (target.unitName + " lost " + Mathf.Abs(valuePerLevel[level -1]) + stat.ToString().ToLower() + ".");
			}
		}
		//Buff
		else {
			if (stat == eStat.MAX_AP || stat == eStat.MAX_MP) {
				target.stats.ModifyStatValue (stat, valuePerLevel[level - 1]);
				effectData.SetValue (valuePerLevel [level - 1]);
				string statName = (stat == eStat.MAX_AP) ? " Ap." : " Mp.";
				target.AddToTextLogBuffer (target.unitName + " gained " + valuePerLevel[level -1] + statName);
			} else {
				target.stats.ModifyStatValue (stat, valuePerLevel [level - 1]);
				effectData.SetValue (valuePerLevel [level - 1]);
				target.AddToTextLogBuffer (target.unitName + " gained " + valuePerLevel[level -1] + stat.ToString().ToLower() + ".");
			}
		}
		if (stat == eStat.MAX_AP) {
			target.floatingText.DisplayApChange (finalValue);
		} else if (stat == eStat.MAX_MP) {
			target.floatingText.DisplayMpChange (finalValue);
		}
	}

	public void ApplyOnTurnStart(Unit source, Unit target, int level)
	{
		
	}

	public void RemoveEffect(Unit target, int val)
	{
		target.stats.ModifyStatValue (stat, -val);
	}
}
