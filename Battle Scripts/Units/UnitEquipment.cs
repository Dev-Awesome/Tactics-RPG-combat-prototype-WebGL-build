using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEquipment : MonoBehaviour {

//	private PlayerUnit owner;

	public Weapon weapon { get; private set; }

	void Start()
	{
	//	owner = GetComponent<PlayerUnit> ();
	}

	public void SetWeapon(Weapon weapon)
	{
		this.weapon = weapon;
	}
}
