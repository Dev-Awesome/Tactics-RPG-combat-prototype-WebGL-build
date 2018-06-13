using System.Collections;
using UnityEngine;

namespace BattleEnums {

	public enum eStat {
		HP,
		MAX_HP,
		AP,
		MAX_AP,
		MP,
		MAX_MP,
		INIT,
		STR,
		INT,
		AGI,
		RES,
		PERCENT_DMG,
		RANGE
	}

	public enum eElement {
		NEUTRAL,
		EARTH,
		FIRE,
		AIR,
		WATER
	}

	public enum eUnitType {
		ALLY,
		ALLIED_SUMMON,
		ENEMY,
		ENEMY_SUMMON
	}

	public enum eWeaponType {
		SWORD,
		HAMMER
	}

	public enum eEffectType {
		DAMAGE,
		HEAL,
		SHIELD,
		DISPLACEMENT,
		SUMMON,
		STAT_MODIFIER
	}

	public enum eUnitState {
		DEFAULT,
		GROUNDED
	}

	public enum eAreaType {
		SINGLE,
		V_LINE,
		H_LINE,
		CROSS,
		CIRCLE
	}

	public enum eDisplacementType {
		PUSH,
		PULL,
		SWAP,
		BLINK
	}

	public enum eAnimationType {
		SPECIAL_1,
		SPECIAL_2
	}
}
