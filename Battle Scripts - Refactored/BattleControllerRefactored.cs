using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleControllerRefactored : Singleton {

	//Battle controller components.
	public GridManager grid { get; private set; }
	public GridHighlighter highlighter { get; private set; }
	public BattleGUIControllerRefactored guiController { get; private set; }
	public BattleInputControllerRefactored inputController { get; private set; }

	//Global battle state control.
	public enum eBattleState {
		INIT,
		UNIT_PLACEMENT,
		PLAYER_IDLE,
		PLAYER_TARGETING,
		UNIT_MOVEMENT,
		UNIT_ACTION,
		AI_CALCULATING,
		VICTORY,
		DEFEAT
	}

	public eBattleState battleState { get; private set; }

	public void InitBattleController(GridManager gridManager, List<UnitData> playerUnits, List<UnitData> enemyUnits)
	{
		SetBattleState (eBattleState.INIT);

		grid = gridManager;

		//Init controller components.
	}

	private void SetBattleState(eBattleState state)
	{
		battleState = state;

//		BattleGUIControllerRefactored.BattleStateChanged (state);
	}
}
