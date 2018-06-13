using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleEnums;

public abstract class Stats : MonoBehaviour {

	public int baseMaxHp { get; private set; }
	public int baseMaxAp { get; private set; }
	public int baseMaxMp { get; private set; }

	public int hp { get; private set; }
	public int maxHp { get; private set; }
	public int ap { get; private set; }
	public int maxAp { get; private set; }
	public int mp { get; private set; }
	public int maxMp { get; private set; }

	public int initiative { get; private set; }
	public int critChance { get; private set; }

	public int neutralRes { get; protected set; }
	public int airRes { get; protected set; }
	public int fireRes { get; protected set; }
	public int earthRes { get; protected set; }
	public int waterRes { get; protected set; }

	public int strength { get; private set; }
	public int intelligence { get; private set; }
	public int agility { get; private set; }
	public int resolve { get; private set; }

	public int range { get; private set; }

	public eUnitState unitState = eUnitState.DEFAULT;
	public List<StatMod> statModifiers = new List<StatMod> ();

	public void SetHp (int val) {
		if (hp + val < 0) {
			hp = 0;
		} else if (hp + val > maxHp) {
			hp = maxHp;
		} else {
			hp += val;
		}
	}

	public void SetMaxHp (int val) {
		if (maxHp + val < 0) {
			maxHp = 1;
		} else {
			maxHp += val;
		}
	}

	public void SetAp (int val) {
		if (ap + val < 0) {
			ap = 0;
		} else if (ap + val > maxAp) {
			ap = maxAp;
		} else {
			ap += val;
		}
	}

	public void SetMaxAp (int val) {
		if (maxAp + val < 0) {
			maxAp = 0;
			ap = 0;
		} else {
			maxAp += val;
			SetAp (val);
		}
	}

	public void SetMp (int val) {
		if (mp + val < 0) {
			mp = 0;
		} else if (mp + val > maxMp) {
			mp = maxMp;
		} else {
			mp += val;
		}
	}

	public void SetMaxMp (int val) {
		if (maxMp + val < 0) {
			maxMp = 0;
			mp = 0;
		} else {
			maxMp += val;
			SetMp (val);
		}
	}

	public void ModifyStatValue(eStat stat, int amount)
	{
		switch (stat) {
		case eStat.MAX_AP:
			SetMaxAp (amount);
			break;

		case eStat.MAX_MP:
			SetMaxMp (amount);
			break;

		case eStat.STR:
			strength = (strength + amount < 0) ? 0 : strength + amount;
			break;

		case eStat.INT:
			intelligence = (intelligence + amount < 0) ? 0 : intelligence + amount;
			break;

		case eStat.AGI:
			agility = (agility + amount < 0) ? 0 : agility + amount;
			break;

		case eStat.RES:
			resolve = (resolve + amount < 0) ? 0 : resolve + amount;
			break;
		}
	}

	public void InitStats (UnitData data) {
		
		hp = data.hp;
		maxHp = data.hp;
		ap = data.ap;
		maxAp = data.ap;
		mp = data.mp;
		maxMp = data.mp;

		initiative = data.initiative;
		strength = data.strength;
		intelligence = data.intelligence;
		agility = data.agility;
		resolve = data.resolve;

		neutralRes = data.neautral;
		earthRes = data.earth;
		fireRes = data.fire;
		airRes = data.air;
		waterRes = data.water;

		baseMaxHp = maxHp;
		baseMaxAp = maxAp;
		baseMaxMp = maxMp;
	}

	public void EndTurn () {


		ap = maxAp;
		mp = maxMp;
	}

	public int GetResistValue(eElement element) {

		switch (element) {
		case eElement.NEUTRAL:
			return neutralRes;

		case eElement.FIRE:
			return fireRes;

		case eElement.EARTH:
			return earthRes;

		case eElement.AIR:
			return airRes;

		case eElement.WATER:
			return waterRes;

		default:
			return 0;
		}
	}

	public abstract void SetResistValue(eElement element, int val);

	public abstract void AdjustResistValue(eElement element, int val);

	public class StatMod {

		public bool applyEachTurn;
		public int remainingDuration;
		public eStat statToMod;
		public int val;
		public bool resistable;
		public bool wasApplied;

		public StatMod(bool constant, int dur, eStat stat, int val, bool resistable) {
			applyEachTurn = constant;
			remainingDuration = dur;
			statToMod = stat;
			this.val = val;
			this.resistable = resistable;
			wasApplied = false;
		}

		public void ApplyImmediately(Unit unit)
		{
			switch (statToMod) {
			case eStat.MAX_AP:
				unit.stats.SetMaxAp (val);
				unit.floatingText.DisplayApChange (val);
				this.wasApplied = true;
				break;

			case eStat.MAX_MP:
				unit.stats.SetMaxAp (val);
				unit.floatingText.DisplayApChange (val);
				this.wasApplied = true;
				break;
			}
		}

		public void ApplyOnTurnStart(Unit unitStats)
		{
			
		}
	}
}
