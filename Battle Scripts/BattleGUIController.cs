using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class BattleGUIController : MonoBehaviour {

	[SerializeField]
	private Canvas battleGUI;

	//Buttons.
	private Button endTurnButton;
	private Button startBattleButton;
//	private Button forceTurnEnd;

	//Panels.
	private BattleGUIPanel currentUnitStats;
	private BattleActionBar abilityPanel;
	private Text battleLog;

	private GridHighlighter highlighter;
	private BattleInputController inputController;

	void Awake()
	{
		GetComponent<BattleController>().stateChange += BattleStateChanged;
		highlighter = GetComponent<GridHighlighter> ();
		inputController = GetComponent<BattleInputController> ();
	}

	void OnDestroy()
	{
		GetComponent<BattleController>().stateChange -= BattleStateChanged;
	}

	private void BattleStateChanged(BattleController.eBattleState state)
	{
		switch (state) {
		case BattleController.eBattleState.INITIALISATION:
			InstantiateUiElements ();
			break;

		case BattleController.eBattleState.UNIT_PLACEMENT:
			SubsribeListenersToCells ();
			SubscribeListenersToUnits ();
			startBattleButton.interactable = true;
			break;

		case BattleController.eBattleState.PLAYER_TURN:
			endTurnButton.gameObject.SetActive (true);
			break;

		case BattleController.eBattleState.AI_TURN:
			endTurnButton.gameObject.SetActive (false);
			break;

		case BattleController.eBattleState.VICTORY:
			ShowEndBattleGUI (true);
			break;

		case BattleController.eBattleState.DEFEAT:
			ShowEndBattleGUI (false);
			break;
		}
	}

	private void InstantiateUiElements()
	{
		Instantiate (battleGUI);
		abilityPanel = GameObject.FindGameObjectWithTag ("Ability Panel").GetComponent<BattleActionBar> ();
		abilityPanel.InitActionBar (this);

		endTurnButton = GameObject.FindGameObjectWithTag ("End Turn Button").GetComponent<Button> ();
		endTurnButton.onClick.AddListener (() => {
			EndTurnPressed ();
		});
		abilityPanel.gameObject.SetActive (false);

		startBattleButton = GameObject.FindGameObjectWithTag ("Start Battle Button").GetComponent<Button> ();
		startBattleButton.onClick.AddListener (() => {
			StartBattlePressed ();
		});
		startBattleButton.interactable = false;

//		forceTurnEnd = GameObject.FindGameObjectWithTag ("DevEndTurn").GetComponent<Button> ();
//		forceTurnEnd.onClick.AddListener (() => {
//			ForceTurnEnd ();
//		});

		battleLog = GameObject.Find ("Battle Log").GetComponentInChildren<Text> ();
		currentUnitStats = GameObject.Find ("CurrentUnitStatsPanel").GetComponent<BattleGUIPanel> ();
		currentUnitStats.gameObject.SetActive (false);
	}

	//Debugging
	private void ForceTurnEnd()
	{
		highlighter.ClearAll ();
		BattleController.instance.EndTurn ();
	}

	private void EndTurnPressed()
	{
		if (BattleInputController.inputState == BattleInputController.eInputState.PLAYER_DEFAULT ||
		    BattleInputController.inputState == BattleInputController.eInputState.TARGET_SELECT) {
			BattleController.instance.EndTurn ();
			highlighter.ClearAll ();
		}
	}

	private void StartBattlePressed()
	{
		startBattleButton.gameObject.SetActive (false);
		currentUnitStats.gameObject.SetActive (true);
		abilityPanel.gameObject.SetActive (true);
		BattleController.instance.StartBattle ();
	}

	private void SubsribeListenersToCells()
	{
		GameObject[] cells = GameObject.FindGameObjectsWithTag ("Grid Cell");
		for (int i = 0; i < cells.Length; i++) {
			cells [i].GetComponent<GridCell> ().cellHoverInfoEvent += CellHoverEvent;
			cells [i].GetComponent<GridCell> ().cellHoverExitEvent += CellHoverExit;
		}
	}

	private void SubscribeListenersToUnits()
	{
		foreach (Unit u in BattleController.instance.units) {
			u.setBattleLogText += SetBattleLogText;
		}
	}

	public void SetBattleLogText(string log)
	{
		battleLog.text += (battleLog.text.Length > 0) ? "\n" + log : log;
	}

	public void ClearLogText()
	{
		battleLog.text = "";
	}

	private void CellHoverEvent(Unit unit)
	{
		UpdateCurrentUnitStats (unit);
	}

	private void CellHoverExit()
	{
		UpdateCurrentUnitStats (BattleController.instance.currentUnit);
	}

	public void UpdateCurrentUnitStats(Unit unit)
	{
		if (currentUnitStats == null) {
			currentUnitStats = GameObject.Find ("CurrentUnitStatsPanel").GetComponent<BattleGUIPanel> ();
		}

		currentUnitStats.SetUnitStats (unit);
		abilityPanel.UpdateAbilityBarDisplay (unit);
		if (unit == BattleController.instance.currentUnit) {

		} else {

		}
	}

	//Ability buttons.
	public void AbilityButtonClicked(int buttonIndex)
	{
		if (BattleInputController.inputState == BattleInputController.eInputState.PLAYER_DEFAULT ||
			BattleInputController.inputState == BattleInputController.eInputState.TARGET_SELECT) {
			inputController.AbilitySelected (buttonIndex);
		}
	}

	public void ShowEndBattleGUI(bool victory)
	{
		Destroy (GameObject.FindGameObjectWithTag("BattleGUI"));
	}
}
