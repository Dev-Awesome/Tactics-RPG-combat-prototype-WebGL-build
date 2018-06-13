using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AbilityIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

//	private string reasonForDisable;
	private float fillPercent;

	private AbilityTooltip tooltip;
	private Ability currentAbility;
	private int currentLevel;

	private Image cooldownFill;
	private Image abilityIcon;
	private Text apCost;
	private Text cooldown;
	private Button abilityButton;

	public void Awake()
	{
		tooltip = GameObject.FindGameObjectWithTag ("Ability Tooltip").GetComponent<AbilityTooltip> ();
		cooldownFill = gameObject.transform.GetChild (3).GetComponent<Image> ();
		apCost = gameObject.transform.GetChild (1).GetComponentInChildren<Text> ();
		abilityIcon = gameObject.transform.GetChild (2).GetComponent<Image> ();
		cooldown = gameObject.transform.GetChild (4).GetComponent<Text> ();
		abilityButton = GetComponent<Button> ();
		cooldownFill.fillAmount = 0;
	}

	public void OnPointerEnter(PointerEventData data)
	{
		ShowAbilityTooltip ();
	}

	public void OnPointerExit(PointerEventData data)
	{
		HideAbilityTooltip ();
	}

	public void SetAbilityButton(Unit unit, int index)
	{
		AbilityController.AbilityInfo info = unit.abilityController.abilityInfo [index];

		currentAbility = info.ability;
		currentLevel = info.level;

		apCost.text = unit.abilityController.GetApCost (index).ToString ();;
		abilityIcon.sprite = info.ability.icon;
		abilityButton.interactable = true;
		cooldownFill.fillAmount = 0;
		cooldown.text = "";

		//Check cooldown.
		if (info.remainingCooldown != 0) {
			SetCooldown (info.remainingCooldown, info.ability.cooldownPerLvl [info.level - 1]);
			abilityButton.interactable = false;
	//		reasonForDisable = "On Cooldown";
		}

		//Check Ap.
		else if (unit.stats.ap < info.ability.apCostPerLvl [info.level - 1]) {
			abilityButton.interactable = false;
	//		reasonForDisable = "Not Enough AP";
		}

		//Check cast limit.
		else if (info.ability.castsPerTurnPerLvl[info.level-1] != 0 && info.castsThisTurn >= info.ability.castsPerTurnPerLvl [info.level - 1]) {
			abilityButton.interactable = false;
	//		reasonForDisable = "Cast Per Turn Limit Reached";
		}
	}

	public void ShowAbilityTooltip()
	{
		tooltip.gameObject.SetActive (true);
		tooltip.ConstructTooltip (currentAbility, currentLevel);
	}

	public void HideAbilityTooltip()
	{
		tooltip.gameObject.SetActive (false);
	}

	private void SetCooldown(int remainingCooldown, int maxCooldown)
	{
		fillPercent = (float)remainingCooldown / (float)maxCooldown;
		cooldownFill.fillAmount = fillPercent;
		cooldown.text = remainingCooldown.ToString ();
	}
}
