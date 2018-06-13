using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverWorldMob : MonoBehaviour {

	public List<UnitData> units = new List<UnitData>();

	public float triggerRange = 1f;

	public Text mobText;

	private GameObject player;
	private GameObject mobInfo;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		mobInfo = GameObject.Find ("MobTooltip");
		mobInfo.SetActive (false);
	}

	void FixedUpdate()
	{
		CheckPlayerDistance ();
	}

	void OnMouseEnter()
	{
		for (int i = 0; i < units.Count; i++) {
			Text mobName = Instantiate (mobText, mobInfo.transform).GetComponent<Text> ();
			mobName.text = units [i].unitName;
			mobName.transform.GetChild(0).GetComponent<Text> ().text = "LVL: " + units [i].level;
		}
		Vector3 pos = Camera.main.WorldToScreenPoint (transform.position);
		mobInfo.transform.position = pos;
		mobInfo.SetActive (true);
	}

	void OnMouseExit()
	{
		for (int i = mobInfo.transform.childCount - 1; i >= 0; i--) {
			Destroy (mobInfo.transform.GetChild (i).gameObject);
		}
		mobInfo.SetActive (false);
	}

	void CheckPlayerDistance()
	{
		if (player != null) {
			float distance = Vector3.Distance (transform.position, player.transform.position);

			if (distance < triggerRange) {
				GameManager.instance.StartBattle (units);
			}
		} else {
			player = GameObject.FindGameObjectWithTag ("Player");
		}
	}
}
