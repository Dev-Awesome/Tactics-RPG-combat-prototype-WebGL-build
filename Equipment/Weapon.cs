using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleEnums;

[CreateAssetMenu(menuName = "Items/Equipment/Weapon")]
public class Weapon : Equipment {

	[HideInInspector] public eWeaponType weaponType;
	[HideInInspector] public int apCost;

	[HideInInspector] public List<DamageProperty> damageProperties = new List<DamageProperty> ();
	[HideInInspector] public List<HealProperty> healProperties = new List<HealProperty> ();
	[HideInInspector] public List<ShieldProperty> shieldProperties = new List<ShieldProperty> ();

	[HideInInspector] public int minRange = 1;
	[HideInInspector] public int maxRange = 1;
	[HideInInspector] public bool linear = false;
	[HideInInspector] public bool requiresLoS = true;
}
