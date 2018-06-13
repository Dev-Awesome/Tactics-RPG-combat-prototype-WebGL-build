using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleGUIPanel : MonoBehaviour {

	Text unitName;

	Text hp;
	Text ap;
	Text mp;

//	Text shield;
	Text neutRes;
	Text earthRes;
	Text fireRes;
	Text airRes;
	Text waterRes;

	PercentageFilledBar hpBar;

	void Awake()
	{
		hpBar = gameObject.transform.FindChild("HpBar").GetComponent<PercentageFilledBar> ();

		unitName = gameObject.transform.GetChild(1).GetComponent<Text> ();
		hp = gameObject.transform.FindChild("HpBar").GetComponentInChildren<Text> ();
		ap = gameObject.transform.FindChild("Ap").GetComponent<Text> ();
		mp = gameObject.transform.FindChild("Mp").GetComponent<Text> ();

//		shield = gameObject.transform.FindChild ("Resists").GetChild (0).GetComponent<Text> ();
		neutRes = gameObject.transform.FindChild ("Resists").GetChild (1).GetComponent<Text> ();
		earthRes = gameObject.transform.FindChild ("Resists").GetChild (2).GetComponent<Text> ();
		fireRes = gameObject.transform.FindChild ("Resists").GetChild (3).GetComponent<Text> ();
		airRes = gameObject.transform.FindChild ("Resists").GetChild (4).GetComponent<Text> ();
		waterRes = gameObject.transform.FindChild ("Resists").GetChild (5).GetComponent<Text> ();
	}

	public void SetUnitStats(Unit u)
	{
		unitName.text = u.unitName;
		hpBar.SetFillAmount (u.stats.hp, u.stats.maxHp);
		hp.text = u.stats.hp + " / " + u.stats.maxHp;
		ap.text = u.stats.ap.ToString ();
		mp.text = u.stats.mp.ToString ();
		neutRes.text = u.stats.neutralRes + "%";
		earthRes.text = u.stats.earthRes + "%";
		fireRes.text = u.stats.fireRes + "%";
		airRes.text = u.stats.airRes + "%";
		waterRes.text = u.stats.waterRes + "%";
	}
}
