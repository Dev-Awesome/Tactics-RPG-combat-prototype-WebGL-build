using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BattleInputController : MonoBehaviour {

	public static bool mouseHoverEvent { get; set; }

	public enum eInputState {
		NPC_DEFAULT,
		PLAYER_DEFAULT,
		TARGET_SELECT,
		UNIT_PLACEMENT,
		DISABLED
	}

	public static eInputState inputState { get; private set; }

	private Unit currentUnit;
	private GridCell target;
	private Stack<GridCell> movementPath = new Stack<GridCell>();
	private HashSet<GridCell> movementRange = new HashSet<GridCell> ();
	private HashSet<GridCell> lockedMovementIndicator = new HashSet<GridCell> ();
	private HashSet<GridCell> abilityRange = new HashSet<GridCell> ();
	private HashSet<GridCell> areaOfEffect = new HashSet<GridCell> ();
	private HashSet<GridCell> visableTackleArea = new HashSet<GridCell> ();

	private List<GridCell> playerStartPos;
	private List<GridCell> enemyStartPos;

	private GridHighlighter highlighter;
	private BattleGUIController guiController;

	//Unit placement.
	private bool holdingUnit = false;
	private Unit selectedUnit;

	private bool lockShowMoveRange = false;

	private int movementPenalty = 0;
	private int selectedAbilityIndex;
	private bool abilityHasCrit = false;

	public static void SetInputState(eInputState state)
	{
		inputState = state;
	}

	void Awake()
	{
		inputState = eInputState.DISABLED;
		GetComponent<BattleController>().stateChange += BattleStateChanged;
		highlighter = GetComponent<GridHighlighter> ();
		guiController = GetComponent<BattleGUIController> ();
	}

	void OnDestroy()
	{
		GetComponent<BattleController>().stateChange -= BattleStateChanged;
	}

	private void BattleStateChanged(BattleController.eBattleState state)
	{
		switch (state) {
		case BattleController.eBattleState.INITIALISATION:
			inputState = eInputState.DISABLED;
			highlighter.InitHighlighter ();
			mouseHoverEvent = false;
			currentUnit = null;
			break;

		case BattleController.eBattleState.UNIT_PLACEMENT:
			inputState = eInputState.UNIT_PLACEMENT;
			playerStartPos = BattleController.instance.playerStartPos;
			enemyStartPos = BattleController.instance.enemyStartPos;
			highlighter.HighlightPlayerStart (playerStartPos);
			highlighter.HighlightEnemyStart (enemyStartPos);
			currentUnit = null;
			break;

		case BattleController.eBattleState.PLAYER_TURN:
			inputState = eInputState.PLAYER_DEFAULT;
			UpdateUnitState ();
			break;

		case BattleController.eBattleState.AI_TURN:
			inputState = eInputState.NPC_DEFAULT;
			highlighter.ClearAll ();
			UpdateUnitState ();
			break;

		case BattleController.eBattleState.VICTORY:
			inputState = eInputState.DISABLED;
			currentUnit = null;
			break;

		case BattleController.eBattleState.DEFEAT:
			inputState = eInputState.DISABLED;
			currentUnit = null;
			break;
		}
	}

	void Update()
	{
		HandleKeyInput ();
		HandleMouseInput ();
	}

	private void ListenForEvents()
	{
		if (currentUnit != null) {
			currentUnit.updateUnitState += UpdateUnitState;
			currentUnit.unitController.animationCompleted += AnimationCompleted;
		}
	}

	private void StopListening()
	{
		if (currentUnit != null) {
			currentUnit.updateUnitState -= UpdateUnitState;
			currentUnit.unitController.animationCompleted -= AnimationCompleted;
		}
	}

	private void UpdateUnitState()
	{
		if (currentUnit != BattleController.instance.currentUnit) {
			StopListening ();
			currentUnit = BattleController.instance.currentUnit;
			ListenForEvents ();
		}
		movementPenalty = 0;
		GetMovementRange ();
		if (currentUnit != null) {
			guiController.UpdateCurrentUnitStats (currentUnit);
		}
	}

	private void AnimationCompleted()
	{
		if (BattleController.instance.battleState == BattleController.eBattleState.PLAYER_TURN) {
			inputState = eInputState.PLAYER_DEFAULT;
			GetMovementRange ();
		}
	}

	public void UnitPositionUpdate()
	{
		GetMovementRange ();
	}

	private void HandleKeyInput()
	{
		switch (inputState) {
		case eInputState.UNIT_PLACEMENT:
			
			break;

		case eInputState.NPC_DEFAULT:
			break;

		case eInputState.PLAYER_DEFAULT:
			HandleKeyPressPlayerDefault ();
			break;
		}
	}

	//Input.
	private void HandleMouseInput()
	{
		switch (inputState) {
		case eInputState.UNIT_PLACEMENT:
			UnitPlacementMouseInput ();
			break;

		case eInputState.NPC_DEFAULT:
			break;

		case eInputState.PLAYER_DEFAULT:
			MovementSelection ();
			HandleRightMouseButton ();
			break;

		case eInputState.TARGET_SELECT:
			TargetSelect ();
			HandleRightMouseButton ();
			break;
		}
	}

	private void HandleRightMouseButton()
	{
		if (Input.GetMouseButtonDown (1)) {
			switch (inputState) {
			case eInputState.TARGET_SELECT:
				highlighter.ClearAll ();
				SetInputState (eInputState.PLAYER_DEFAULT);
				break;
			}
		}
	}

	private void HandleKeyPressPlayerDefault()
	{
		//Move range show lock.
		if (Input.GetButtonDown("Tab Key")) {
			lockShowMoveRange = !lockShowMoveRange;
		}
	}

	private void HandeMoveRangeLockInput()
	{
		if (Input.GetMouseButtonDown (0)) {
			RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);
			if (hit.collider != null && hit.collider.gameObject.tag == "Grid Cell") {
				target = hit.collider.GetComponent<GridCell> ();
				if (target.currentUnit == currentUnit) {
					lockShowMoveRange = true;
				}
			}
		}

		if (Input.GetMouseButtonDown (1)) {
			lockShowMoveRange = false;
		}
	}

	private void UnitPlacementMouseInput()
	{
		highlighter.HighlightPlayerStart (playerStartPos);

		RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);

		if (hit.collider != null && hit.collider.gameObject.tag == "Grid Cell") {
			target = hit.collider.GetComponent<GridCell> ();
			if (playerStartPos.Contains(target)) {
				highlighter.HighlightCell (target);
				if (Input.GetMouseButtonDown (0)) {
					if (!holdingUnit) {
						if (target.currentUnit != null) {
							selectedUnit = target.currentUnit;
							selectedUnit.GetComponentInChildren<SpriteRenderer> ().color = Color.magenta;
							holdingUnit = true;
						}
					} else {
						if (target.currentUnit == null) {
							selectedUnit.PlaceUnitOnCell (target);
							selectedUnit.GetComponentInChildren<SpriteRenderer> ().color = Color.white;
							holdingUnit = false;
							selectedUnit = null;
						} else {
							selectedUnit.SwapPlacesWithUnit (target.currentUnit);
							selectedUnit.GetComponentInChildren<SpriteRenderer> ().color = Color.white;
							holdingUnit = false;
							selectedUnit = null;
						}
					}
				}
			}
		}
		if (Input.GetMouseButtonDown (1)) {
			selectedUnit.GetComponentInChildren<SpriteRenderer> ().color = Color.white;
			holdingUnit = false;
			selectedUnit = null;
		}
	}

	private void GetMovementRange()
	{
		if (currentUnit != null && currentUnit.stats.mp > 0) {
			movementPenalty = currentUnit.MovementLockPenalty ();
			movementRange = Pathfinder.GetMoveRange (currentUnit.currentLocation, currentUnit.stats.mp - movementPenalty);
			lockedMovementIndicator = Pathfinder.GetMoveRange (currentUnit.currentLocation, currentUnit.stats.mp);
		}
	}

	public void ShowMovementRange()
	{
		highlighter.HighlightNonVisibleCell (lockedMovementIndicator);
		highlighter.HighlightMoveRange (movementRange);
	}

	private void ShowTackleAreas()
	{
		visableTackleArea.Clear ();
		foreach (Unit u in BattleController.instance.units) {
			if (u != currentUnit) {
				HashSet<GridCell> area = u.GetTackleArea ();
				foreach (GridCell cell in area) {
					if (movementRange.Contains (cell)) {
						visableTackleArea.Add (cell);
					}
				}
			}
		}
		highlighter.HighlightCells (visableTackleArea, "Tackle");
	}

	private void MovementSelection()
	{
		if (currentUnit != null && currentUnit.stats.mp > 0 && mouseHoverEvent == false) {

			highlighter.ClearAll ();
			if (lockShowMoveRange) {
				ShowMovementRange ();
			}
				
			RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);

			if (hit.collider != null && hit.collider.gameObject.tag == "Grid Cell") {
				target = hit.collider.GetComponent<GridCell> ();

				if (movementRange.Contains (target)) {
					movementPath = Pathfinder.GetPath (currentUnit.currentLocation, target);
					highlighter.HighlightMovePath (movementPath, currentUnit.currentLocation);

					if (Input.GetMouseButtonDown (0)) {
						PathSelected ();
						highlighter.ClearAll ();
					}
				}
			}
		}
	}

	private void PathSelected()
	{
		inputState = eInputState.DISABLED;
		currentUnit.Move (movementPath, target, movementPenalty);
	}

	//Targeting
	public void AbilitySelected(int abilityIndex)
	{
		highlighter.ClearAll ();
		selectedAbilityIndex = abilityIndex;
		SetInputState (eInputState.TARGET_SELECT);
	}

	private void TargetSelect()
	{
		highlighter.ClearAll ();
		abilityRange = currentUnit.abilityController.GetRange (selectedAbilityIndex);
		highlighter.HighlightAbilityRange (abilityRange);

		RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);
		if (hit.collider != null && hit.collider.gameObject.tag == "Grid Cell") {
			target = hit.collider.GetComponent<GridCell> ();

			if (abilityRange.Contains (target)) {
				highlighter.HighlightCell (target);

				if (currentUnit.abilityController.HasArea (selectedAbilityIndex)) {
					areaOfEffect = currentUnit.abilityController.GetArea (selectedAbilityIndex, target);
					highlighter.HighlightAreaOfEffect (areaOfEffect);
				}

				if (Input.GetMouseButtonDown (0)) {
					if (currentUnit.abilityController.HasArea (selectedAbilityIndex)) {
						UseSelectedAbility (areaOfEffect, target);
					} else {
						areaOfEffect.Clear ();
						areaOfEffect.Add (target);
						UseSelectedAbility (areaOfEffect, target);
					}
				}
			}
		}
	}

	private void UseSelectedAbility(HashSet<GridCell> cells, GridCell target)
	{
		highlighter.ClearAll ();
		SetInputState (eInputState.DISABLED);
		abilityHasCrit = currentUnit.abilityController.HasCrit (selectedAbilityIndex);
		currentUnit.abilityController.UseAbility (selectedAbilityIndex, cells, target, abilityHasCrit);
		string text = currentUnit.unitName + " used " + currentUnit.abilityController.GetAbilityName (selectedAbilityIndex) + ".";
		guiController.ClearLogText ();
		guiController.SetBattleLogText (text);
	}

	public void SignalUnits()
	{
		if (abilityHasCrit) {
			guiController.SetBattleLogText ("Critical Hit!");
		}
		foreach (GridCell c in areaOfEffect) {
			if (c.currentUnit != null) {
				c.currentUnit.PlayHitAnimation ();
			}
		}
		currentUnit.PlayHitAnimation ();
		abilityHasCrit = false;
	}

	public void AiSignalUnits(HashSet<GridCell> targetCells)
	{
		foreach (GridCell c in targetCells) {
			if (c.currentUnit != null) {
				c.currentUnit.PlayHitAnimation ();
			}
		}
		currentUnit.PlayHitAnimation ();
	}

	public void AbilityAnimationComplete()
	{
		if (BattleController.instance.battleState == BattleController.eBattleState.PLAYER_TURN) {
			SetInputState (eInputState.PLAYER_DEFAULT);
		} else if (BattleController.instance.battleState == BattleController.eBattleState.AI_TURN) {
			SetInputState (eInputState.NPC_DEFAULT);
		}
	}
}