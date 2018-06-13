using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleActionBar : MonoBehaviour {

	public int buttonPoolSize = 7;
	public GameObject abilityButtonPrefab;

	private AbilityIcon[] abilityButtons;
	private BattleGUIController controller;

	public void UpdateAbilityBarDisplay(Unit unit)
	{
		for (int i = 0; i < unit.abilityController.abilityInfo.Count; i++) {
			abilityButtons [i].gameObject.SetActive (true);
			abilityButtons[i].SetAbilityButton (unit, i);
		}

		for (int i = 0; i < buttonPoolSize; i++) {
			if (i > unit.abilityController.abilityInfo.Count) {
				abilityButtons [i].gameObject.SetActive (false);
			} 
		}
	}

	public void InitActionBar(BattleGUIController guiController)
	{
		controller = guiController;
		abilityButtons = new AbilityIcon[buttonPoolSize];

		for (int i = 0; i < buttonPoolSize; i++) {
			int index = i;
			Button btn = Instantiate (abilityButtonPrefab, gameObject.transform).GetComponent<Button> ();
			btn.onClick.AddListener (() => {
				ButtonClicked (index);
			});
			abilityButtons [i] = btn.GetComponent<AbilityIcon>();
			btn.gameObject.SetActive (false);
		}
		GameObject.FindGameObjectWithTag ("Ability Tooltip").SetActive (false);
	}

	private void ButtonClicked(int buttonIndex)
	{
		controller.AbilityButtonClicked (buttonIndex);
	}
}
