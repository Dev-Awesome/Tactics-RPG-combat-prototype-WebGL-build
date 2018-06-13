using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleEnums;

public class AbilityController : MonoBehaviour {

	private Unit unit;
	public List<AbilityInfo> abilityInfo { get; private set; }

	public void Init(UnitData data)
	{
		unit = GetComponent<Unit> ();
		abilityInfo = new List<AbilityInfo>();

		for (int i = 0; i < data.abilities.Length; i++) {
			abilityInfo.Add (new AbilityInfo (data.abilities [i].ability, data.abilities [i].level));
		}
	}

	public bool CanUse(int index)
	{
		AbilityInfo info = abilityInfo [index];

		if (unit.stats.ap >= info.ability.apCostPerLvl [info.level - 1] && info.remainingCooldown <= 0) {
			if (info.castsThisTurn < info.ability.castsPerTurnPerLvl [info.level - 1] || info.ability.castsPerTurnPerLvl [info.level - 1] == 0) {
				return true;
			}
		} else {
			return false;
		}
		return false;
	}

	public bool CanUseOnCell(int index, GridCell target)
	{
		if (target.cellType != GridCell.eCellType.DEFAULT) {
			return false;
		}

		AbilityInfo info = abilityInfo [index];
		if (info.ability.requiresFreeCell && target.currentUnit == null && target.CanTarget (info.ability.requiresFreeCell)) {
			return true;
		}

		if (!info.ability.requiresFreeCell && target.CanTarget(info.ability.requiresFreeCell)) {
			if (target.currentUnit != null) {
				if (info.ability.castsPerTargetPerLvl [info.level - 1] != 0) {
					if (info.hitThisTurn.ContainsKey (target.currentUnit)) {
						if (info.hitThisTurn[target.currentUnit] <= info.ability.castsPerTargetPerLvl[info.level - 1]) {
							return true;
						}
					} else {
						return true;
					}
				} else {
					return true;
				}
			} else {
				return true;
			}
		}

		return false;
	}

	public int GetApCost(int index)
	{
		AbilityInfo info = abilityInfo [index];
		return info.ability.apCostPerLvl [info.level - 1];
	}

	public string GetAbilityName(int index)
	{
		AbilityInfo info = abilityInfo [index];
		return info.ability.abilityName;
	}

	public HashSet<GridCell> GetRange(int index, bool ai = false)
	{
		AbilityInfo info = abilityInfo [index];

		HashSet<GridCell> range = Pathfinder.GetAbilityRange (unit.currentLocation, info.ability.minRangePerLvl [info.level - 1],
			info.ability.maxRangePerLvl [info.level - 1], RequiresLoS (index), IsLinear (index), info.ability.requiresFreeCell, ai);

		return range;
	}

	public HashSet<GridCell> GetArea(int index, GridCell target)
	{
		AbilityInfo info = abilityInfo [index];

		HashSet<GridCell> aoe = Pathfinder.GetAoE (target, unit.currentLocation, info.ability.areaType, info.ability.areaSizePerLvl [info.level - 1]);

		return aoe;
	}

	public bool IsLinear(int index)
	{
		AbilityInfo info = abilityInfo [index];
		return (info.level >= info.ability.nonLinearFromLvl) ? false : true;
	}

	public bool RequiresLoS(int index)
	{
		AbilityInfo info = abilityInfo [index];
		return (info.level >= info.ability.noLoSFromLvl) ? false : true;
	}

	public bool HasArea(int index)
	{
		AbilityInfo info = abilityInfo [index];
		return (info.ability.areaType == eAreaType.SINGLE) ? false : true;
	}

	public bool HasCrit(int index)
	{
		AbilityInfo info = abilityInfo [index];
		if (info.ability.baseCritChance > 0) {
			return BattleCalculations.CalculateCrit (info.ability.baseCritChance);
		} else {
			return false;
		}
	}

	public void UseAbility(int index, HashSet<GridCell> cells, GridCell target, bool crit)
	{
		AbilityInfo info = abilityInfo [index];

		foreach (GridCell c in cells) {

			if (crit) {
				foreach (DamageProperty prop in info.ability.critDamageEffects) {
					EffectHandler.ApplyDamage (prop, unit, c, info.level);
				}
			} 

			else {
				foreach (DamageProperty prop in info.ability.damageEffects) {
					EffectHandler.ApplyDamage (prop, unit, c, info.level);
				}
			}

			if (c.currentUnit != null) {
				if (info.hitThisTurn.ContainsKey (c.currentUnit)) {
					info.hitThisTurn [c.currentUnit]++;
				} else {
					info.hitThisTurn.Add (c.currentUnit, 1);
				}
			}
		}

		unit.unitController.UseAbility (info.ability.animationTrigger, target);
		unit.stats.SetAp (-info.ability.apCostPerLvl [info.level - 1]);
		info.castsThisTurn++;
		info.remainingCooldown = info.ability.cooldownPerLvl [info.level - 1];
		unit.OutsideUpdateUnitState ();
		unit.floatingText.DisplayApChange (-info.ability.apCostPerLvl [info.level - 1]);

		if (info.ability.castsPerTargetPerLvl [info.level - 1] != 0 && target.currentUnit != null) {
			if (info.hitThisTurn.ContainsKey (target.currentUnit)) {
				info.hitThisTurn [target.currentUnit]++;
			} else {
				info.hitThisTurn.Add (target.currentUnit, 1);
			}
		}
	}

	public int EstimateTotalDamage(int index, Unit target)
	{
		AbilityInfo info = abilityInfo [index];

		int damage = 0;


		return damage;
	}

	public int GetAbilityIndex(AbilityInfo info)
	{
		for (int i = 0; i < abilityInfo.Count; i++) {
			if (abilityInfo [i] == info) {
				return i;
			}
		}
		return int.MaxValue;
	}

	public void EndTurn()
	{
		foreach (AbilityInfo info in abilityInfo) {
			info.EndTurn ();
		}
	}

	public HashSet<GridCell> GetRangeFrom(int index, GridCell location, bool ai = false)
	{
		AbilityInfo info = abilityInfo [index];

		return Pathfinder.GetAbilityRange (location, info.ability.minRangePerLvl [info.level - 1],
			info.ability.maxRangePerLvl [info.level - 1], RequiresLoS (index), IsLinear (index), info.ability.requiresFreeCell, ai);
		
	}

	public class AbilityInfo {

		public Ability ability;
		public int level;
		public int remainingCooldown;
		public int castsThisTurn;

		public Dictionary<Unit, int> hitThisTurn;

		public AbilityInfo(Ability a, int lvl) {

			ability = a;
			level = lvl;
			remainingCooldown = 0;
			castsThisTurn = 0;
			hitThisTurn = new Dictionary<Unit, int>();
		}

		public void EndTurn()
		{
			remainingCooldown = (remainingCooldown > 0) ? remainingCooldown - 1 : 0;
			castsThisTurn = 0;
			hitThisTurn.Clear ();
		}
	}
}
