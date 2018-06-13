using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityTooltip : MonoBehaviour {

	[Header("Damage text colours.")]
	public Color neutral;
	public Color earth;
	public Color fire;
	public Color air;
	public Color water;

	public Text effectTextPrefab;

//	private Text[] effectTexts = new Text[5];
//	private Text[] critEffectTexts = new Text[5];

	private Text abilityName;
	private Text apCost;
	private Text range;
	private Text aoe;
	private Text los;
	private Text linear;
	private Text cooldown;
	private Text castsPerTurn;
	private Text castsPertarget;

	void Start()
	{
		abilityName = transform.Find ("BasicInfo/AbilityName").GetComponent<Text> ();
		apCost = transform.Find ("BasicInfo/ApCost").GetComponent<Text> ();
		range = transform.GetChild (1).GetComponent<Text> ();
		aoe = transform.GetChild (2).GetComponent<Text> ();
		los = transform.GetChild (3).GetComponent<Text> ();
		linear = transform.GetChild (4).GetComponent<Text> ();
		cooldown = transform.GetChild (5).GetComponent<Text> ();
		castsPerTurn = transform.GetChild (6).GetComponent<Text> ();
		castsPertarget = transform.GetChild (7).GetComponent<Text> ();

	}

	public void ConstructTooltip(Ability ability, int level)
	{
		if (abilityName != null && abilityName.text != null) {
			abilityName.text = ability.abilityName;
			apCost.text = ability.apCostPerLvl [level - 1].ToString ();
			range.text = ability.maxRangePerLvl [level - 1] > 0 ? "Range: " + ability.minRangePerLvl [level - 1] + " - " + ability.maxRangePerLvl [level - 1] : "Self-Cast";

			if (ability.areaType != BattleEnums.eAreaType.SINGLE) {
				aoe.gameObject.SetActive (true);
				aoe.text = "Area: " + ability.areaType.ToString ().ToLower () + "  -  " + "Size: " + ability.areaSizePerLvl [level - 1];
			} else {
				aoe.gameObject.SetActive (false);
			}

			if (level < ability.noLoSFromLvl) {
				los.gameObject.SetActive (true);
			} else {
				los.gameObject.SetActive (false);
			}

			if (level < ability.nonLinearFromLvl) {
				linear.gameObject.SetActive (true);
			} else {
				linear.gameObject.SetActive (false);
			}

			if (ability.cooldownPerLvl [level - 1] != 0) {
				cooldown.gameObject.SetActive (true);
				cooldown.text = "Cooldown: " + ability.cooldownPerLvl [level - 1];
			} else {
				cooldown.gameObject.SetActive (false);
			}

			if (ability.castsPerTargetPerLvl [level - 1] != 0) {
				castsPertarget.gameObject.SetActive (true);
				castsPertarget.text = "Casts per Target: " + ability.castsPerTargetPerLvl [level - 1];
			} else {
				castsPertarget.gameObject.SetActive (false);
			}

			if (ability.castsPerTurnPerLvl [level - 1] != 0) {
				castsPerTurn.gameObject.SetActive (true);
				castsPerTurn.text = "Casts per Turn: " + ability.castsPerTurnPerLvl [level - 1];
			} else {
				castsPerTurn.gameObject.SetActive (false);
			}
		}
	}
}
