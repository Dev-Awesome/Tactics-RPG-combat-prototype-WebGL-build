using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryItem : ScriptableObject{

	[Header("Inventory Item")]
	public string title;
	public string description;
	public Texture2D icon;

	[Range(1, 5)]
	public int rarity = 1;
}
