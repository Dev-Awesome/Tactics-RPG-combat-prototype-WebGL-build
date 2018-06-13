using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleEnums;

public class Equipment : InventoryItem {

	[HideInInspector] public int levelRequirement;
	[HideInInspector] public List<EquipmentRequirment> statRequirements;

	[System.Serializable]
	public class EquipmentRequirment {
		public eStat stat;
		public int requirement;
	}
}
