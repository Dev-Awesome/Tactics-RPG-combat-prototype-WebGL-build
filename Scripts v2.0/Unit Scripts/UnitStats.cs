using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitStats : MonoBehaviour {

	//General stats.

	public int currentHp { get; private set; }
	public int bonusMaxHp { get; private set; }
	public int baseMaxHp { get; private set; }

	public int currentAp { get; private set; }
	public int bonusAp { get; private set; }
	private int baseAp;

	public int currentMp { get; private set; }
	public int bonusMp { get; private set; }
	private int baseMp;

	public int bonusRange;
	private int baseRange;

	public int currentShield { get; private set; }

	private int baseCrit;
	public int bonusCrit;

	//Multiplier stats.

	private int baseStr;
	public int bonusStr;

	private int baseInt;
	public int bonusInt;

	private int baseAgi;
	public int bonusAgi;

	private int baseRes;
	public int bonusRes;

	//Derived stats.

	private int baseLock;
	public int bonusLock;

	private int baseDodge;
	public int bonusDodge;

	//Flat resistances.

	public int f_neutralRes { get; private set; }
	public int f_earthRes { get; private set; }
	public int f_fireRes { get; private set; }
	public int f_airRes { get; private set; }
	public int f_waterRes { get; private set; }

	//Percent resistances.

	public int p_neutralRes { get; protected set; }
	public int p_earthRes { get; protected set; }
	public int p_fireRes { get; protected set; }
	public int p_airRes { get; protected set; }
	public int p_waterRes { get; protected set; }

	//Initialise stats at the start of combat.

	public virtual void InitialiseStats()
	{
		
	}

	//Get / set methods for HP / AP / MP.

	//Signal unit death event if HP drops below 0.
	public void ModifyCurrentHp(int val)
	{
		if (currentHp + val < 0) {
			currentHp = 0;
			//Signal event.
		} else if (currentHp + val > baseMaxHp + bonusMaxHp) {
			currentHp = baseMaxHp + bonusMaxHp;
		} else {
			currentHp += val;
		}
	}

	public void ModifyBonusMaxHp(int val)
	{
		bonusMaxHp += val;

		if (val > 0) {
			currentHp += val;
		} 
		if (currentHp > baseMaxHp + bonusMaxHp) {
			currentHp = baseMaxHp + bonusMaxHp;
		}
	}

	public int GetMaxHp()
	{
		if (baseMaxHp + bonusMaxHp < 1) {
			return 1;
		} else {
			return baseMaxHp + bonusMaxHp;
		}
	}

	public void DecreaseAp(int val)
	{
		if (currentAp - val < 0) {
			currentAp = 0;
		} else {
			currentAp -= val;
		}
	}

	public void IncreaseAp(int val)
	{
		bonusAp += val;
		currentAp += val;
	}

	public void DecreaseBonusAp(int val)
	{
		bonusAp -= val;
		currentAp -= val;
	}

	public void DecreaseMp(int val)
	{
		if (currentMp - val < 0) {
			currentMp = 0;
		} else {
			currentMp -= val;
		}
	}

	public void IncreaseMp(int val)
	{
		bonusMp += val;
		currentMp += val;
	}

	public void DecreaseBonusMp(int val)
	{
		bonusMp -= val;
		currentMp -= val;
	}

	public int GetRange()
	{
		return baseRange + bonusRange;
	}

	public void ModifyShieldValue(int val)
	{
		if (currentShield + val < 0) {
			currentShield = 0;
		} else {
			currentShield += val;
		}
	}

	public int GetCritChance()
	{
		if (baseCrit + bonusCrit < 0) {
			return 0;
		} else if (baseCrit + bonusCrit > 100) {
			return 100;
		} else {
			return baseCrit + bonusCrit;
		}
	}

	//Get methods for multiplier stats.
	//Index: 1 - strength, 2 - intelligence, 3 - agility, 4 - resolve.

	public int GetStat(int i)
	{
		switch (i) {
		case 1:
			if (baseStr + bonusStr < 0) {
				return 0;
			} else {
				return baseStr + bonusStr;
			}

		case 2:
			if (baseInt + bonusInt < 0) {
				return 0;
			} else {
				return baseInt + bonusInt;
			}

		case 3:
			if (baseAgi + bonusAgi < 0) {
				return 0;
			} else {
				return baseAgi + bonusAgi;
			}

		case 4:
			if (baseRes + bonusRes < 0) {
				return 0;
			} else {
				return baseRes + bonusRes;
			}

		default:
			return 0;
		}
	}

	//Get mothods for derived stats.

	public int GetLock()
	{
		if (baseLock + bonusLock < 1) {
			return 1;
		} else {
			return baseLock + bonusLock;
		}
	}

	public int GetDodge()
	{
		if (baseDodge + bonusDodge < 1) {
			return 1;
		} else {
			return baseDodge + bonusDodge;
		}
	}

	//Set method for flat resists.
	//Index: 0 - neutral, 1 - earth, 2 - fire, 3 - air, 4 - water.

	public void ModifyFlatResist(int i, int val)
	{
		switch (i) {
		case 0:
			if (f_neutralRes + val < 0) {
				f_neutralRes = 0;
			} else {
				f_neutralRes += val;
			}
			break;

		case 1:
			if (f_earthRes + val < 0) {
				f_earthRes = 0;
			} else {
				f_earthRes += val;
			}
			break;

		case 2:
			if (f_fireRes + val < 0) {
				f_fireRes = 0;
			} else {
				f_fireRes += val;
			}
			break;

		case 3:
			if (f_airRes + val < 0) {
				f_airRes = 0;
			} else {
				f_airRes += val;
			}
			break;

		case 4:
			if (f_waterRes + val < 0) {
				f_waterRes = 0;
			} else {
				f_waterRes += val;
			}
			break;

		default:
			break;
		}
	}

	//Percent resists set in derived player and enemy unit classes - player unit %res is capped.

	public abstract void ModifyPercentResist (int i, int val);
}
