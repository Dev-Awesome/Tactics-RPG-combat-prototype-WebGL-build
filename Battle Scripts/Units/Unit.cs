using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleEnums;
using System;

public abstract class Unit : MonoBehaviour {

	//Icons.
	public Color unitIndicatorColour;
	private SpriteRenderer unitIndicator;

	public BattleFloatingText floatingText;

	//Unit components.
	public Stats stats { get; private set; }
	public UnitEquipment equipment { get; private set; }
	public AbilityController abilityController { get; private set; }
	public UnitController unitController { get; private set; }
	public StatusController statusEffects { get; private set; }
	public BattleInputController inputController { get; private set; }
	public BattleGUIController guiController { get; private set; }

	//Unit info.
	public string unitName { get; private set; }
	public eUnitType unitType { get; private set; }
	public GridCell currentLocation { get; private set; }
	public bool isDead { get; private set; }

	//Events.
	public event Action updateUnitState;
	public event Action<string> setBattleLogText;

	private List<AbilityHitInfo> hitInfo = new List<AbilityHitInfo> ();

	//Extra.
	protected int movementPenalty = 0;
	private List<string> textLogBuffer = new List<string> ();

	public virtual void Init(UnitData data)
	{
		unitIndicator = gameObject.transform.GetChild (1).GetComponent<SpriteRenderer> ();
		unitIndicator.color = unitIndicatorColour;
		unitIndicator.enabled = false;

		stats = GetComponent<Stats> ();
		equipment = GetComponent<UnitEquipment> ();
		abilityController = GetComponent<AbilityController> ();
		unitController = GetComponent<UnitController> ();
		statusEffects = GetComponent<StatusController> ();
		inputController = BattleController.instance.GetComponent<BattleInputController> ();
		guiController = BattleController.instance.GetComponent<BattleGUIController> ();

		Transform iconHolder = GameObject.Find ("Icon Canvas").transform;
		floatingText = Instantiate (floatingText, iconHolder).GetComponent<BattleFloatingText> ();
		floatingText.InitFloatingText (this);

		stats.InitStats (data);
		abilityController.Init (data);
		unitController.InitUnitController (data);

		unitType = data.unitType;
		unitName = data.unitName;
		isDead = false;
	}

	public virtual void StartTurn()
	{
		hitInfo.Clear ();
		movementPenalty = 0;
		unitIndicator.enabled = true;
	}

	public bool IsAlly(Unit other)
	{
		if (unitType == eUnitType.ALLY || unitType == eUnitType.ALLIED_SUMMON) {
			return other.unitType == eUnitType.ALLY || other.unitType == eUnitType.ALLIED_SUMMON;
		} else {
			return other.unitType == eUnitType.ENEMY || other.unitType == eUnitType.ENEMY_SUMMON;
		}
	}

	public abstract void MovementInterupted ();

	public virtual void EndTurn()
	{
		hitInfo.Clear ();
		stats.EndTurn ();
		statusEffects.EndTurn ();
		abilityController.EndTurn ();
		unitIndicator.enabled = false;
	}

	public void SetIndicatorEnabled(bool enabled)
	{
		unitIndicator.enabled = enabled;
	}

	public void AddToTextLogBuffer(string text)
	{
		textLogBuffer.Add (text);
	}

	public void PlaceUnitOnCell(GridCell target)
	{
		if (target.currentUnit == null) {
			if (currentLocation != null) {
				currentLocation.currentUnit = null;
			}
			currentLocation = target;
			target.currentUnit = this;
			transform.position = target.transform.position;
		} else {
			Debug.LogError ("Error: Target cell is already occupied.");
		}
	}

	public void PlaceUnitOnCell(GridCell target, string facing)
	{
		if (target.currentUnit == null) {
			if (currentLocation != null) {
				currentLocation.currentUnit = null;
			}
			currentLocation = target;
			target.currentUnit = this;
			transform.position = target.transform.position;
		} else {
			Debug.LogError ("Error: Target cell is already occupied.");
		}
	}

	public void SwapPlacesWithUnit(Unit unit)
	{
		unit.transform.position = currentLocation.transform.position;
		transform.position = unit.currentLocation.transform.position;

		GridCell myPos = currentLocation;
		currentLocation = unit.currentLocation;
		unit.currentLocation = myPos;

		currentLocation.currentUnit = this;
		unit.currentLocation.currentUnit = unit;
	}

	public void Move(Stack<GridCell> path, GridCell targetLocation, int movePenalty)
	{
		movementPenalty = movePenalty;
		unitController.MoveUnit (currentLocation, path);
	}

	public void DisplaceComplete(GridCell newLocation)
	{
		currentLocation.currentUnit = null;
		currentLocation = newLocation;
		currentLocation.currentUnit = this;
		if (updateUnitState != null) {
			updateUnitState ();
		}
		inputController.UnitPositionUpdate ();
	}

	public void MoveComplete(int totalMoved, GridCell newLocation)
	{
		int totalLossMp = totalMoved + movementPenalty;
		stats.SetMp (-totalLossMp);
		currentLocation.currentUnit = null;
		currentLocation = newLocation;
		currentLocation.currentUnit = this;
		floatingText.DisplayMpChange (-totalLossMp);
		if (updateUnitState != null) {
			updateUnitState ();
		}
	}

	//Affect unit.
	public void Damage(int val, eElement element)
	{
		hitInfo.Add (new AbilityHitInfo (eEffectType.DAMAGE, val, element));
	}

	public void Heal(int val)
	{
		hitInfo.Add(new AbilityHitInfo (eEffectType.HEAL, val, eElement.NEUTRAL));
	}

	public void ApplyStatModifier(Unit source, StatusModifier statMod, int level)
	{
		statusEffects.AddStatusEffect (source, statMod, level);
	}

	public void ForcedDisplace(GridCell target, eDisplacementType dType)
	{
		hitInfo.Add (new AbilityHitInfo (eEffectType.DISPLACEMENT, target, dType));
	}

	public void BlinkToTarget(GridCell target, float delay)
	{
		StartCoroutine (DelayedBlink (target, delay));	
	}

	private IEnumerator DelayedBlink(GridCell target, float delay)
	{
		yield return new WaitForSeconds (delay);
		PlaceUnitOnCell (target);
	}

	public void PlayHitAnimation()
	{
		if (BattleController.instance.currentUnit != this) {
			unitIndicator.enabled = false;
		}

		int netHpChange = 0;

		if (hitInfo.Count > 0) {
			foreach (AbilityHitInfo info in hitInfo) {
				switch (info.effectType) {

				case eEffectType.DAMAGE:
					netHpChange -= info.val;
					SetElementalLogString (info.val, info.element);
					break;

				case eEffectType.HEAL:
					netHpChange += info.val;
					SetLogText (unitName + " recovered " + info.val + " Hp.");
					break;

				case eEffectType.DISPLACEMENT:
					switch (info.dType) {
					case eDisplacementType.PUSH:
						unitController.PushUnit (info.location);
						break;

					case eDisplacementType.PULL:
						unitController.PushUnit (info.location);
						break;
					}
					break;
				}
			}
			stats.SetHp (netHpChange);
			floatingText.DisplayNumber (netHpChange);
			hitInfo.Clear ();
		}
		statusEffects.ApplyQueuedEffects ();

		if (textLogBuffer.Count > 0) {
			for (int i = 0; i < textLogBuffer.Count; i++) {
				SetLogText (textLogBuffer [i]);
			}
			textLogBuffer.Clear ();
		}

		if (netHpChange < 0) {
			unitController.HitAnimation ();
		}

		if (updateUnitState != null) {
			updateUnitState ();
		}

		if (stats.hp <= 0) {
			SetLogText (unitName + " has died.");
			BattleController.instance.UnitDied (this);
		}
	}

	private void SetLogText(string log)
	{
		if (setBattleLogText != null) {
			setBattleLogText (log);
		}
	}

	private void SetElementalLogString(int damage, eElement element)
	{
		switch (element) {
		case eElement.NEUTRAL:
			SetLogText (unitName + " was hit for <color=#CACACAFF>" + damage + " " + element.ToString ().ToLower ()
			+ "</color> damage.");
			break;

		case eElement.EARTH:
			SetLogText (unitName + " was hit for <color=#BF860BFF>" + damage + " " + element.ToString ().ToLower ()
				+ "</color> damage.");
			break;

		case eElement.FIRE:
			SetLogText (unitName + " was hit for <color=#FF0000FF>" + damage + " " + element.ToString ().ToLower ()
				+ "</color> damage.");
			break;

		case eElement.AIR:
			SetLogText (unitName + " was hit for <color=#20902AFF>" + damage + " " + element.ToString ().ToLower ()
				+ "</color> damage.");
			break;

		case eElement.WATER:
			SetLogText (unitName + " was hit for <color=#00ACE5FF>" + damage + " " + element.ToString ().ToLower ()
				+ "</color> damage.");
			break;
		}
	}

	public abstract int MovementLockPenalty ();

	public HashSet<GridCell> GetTackleArea()
	{
		HashSet<GridCell> tackleArea = new HashSet<GridCell> ();
		for (int x = -1; x <= 1; x++) {
			GridPos pos = new GridPos (currentLocation.gridPos.x - x, currentLocation.gridPos.y);
			if (GridManager.gridCells.ContainsKey (pos)) {
				GridCell nextGridCell = GridManager.gridCells [pos];
				if (nextGridCell != currentLocation) {
					tackleArea.Add (nextGridCell);
				}
			}
		}
		for (int y = -1; y <= 1; y++) {
			GridPos pos = new GridPos (currentLocation.gridPos.x, currentLocation.gridPos.y - y);
			if (GridManager.gridCells.ContainsKey (pos)) {
				GridCell nextGridCell = GridManager.gridCells [pos];
				if (nextGridCell != currentLocation) {
					tackleArea.Add (nextGridCell);
				}
			}
		}

		return tackleArea;
	}

	public void OutsideUpdateUnitState()
	{
		if (updateUnitState != null) {
			updateUnitState ();
		}
	}

	public abstract bool EnemyIsAdjacent (GridCell location);

	public struct AbilityHitInfo {

		public eEffectType effectType;
		public int val;
		public eElement element;
		public GridCell location;
		public eDisplacementType dType;

		public AbilityHitInfo(eEffectType et, int v, eElement e) {
			val = v;
			element = e;
			effectType = et;
			location = null;
			dType = eDisplacementType.BLINK;
		}

		public AbilityHitInfo(eEffectType et, GridCell target, eDisplacementType type) {
			location = target;
			dType = type;
			val = 0;
			element = eElement.NEUTRAL;
			effectType = et;
		}
	}
}
