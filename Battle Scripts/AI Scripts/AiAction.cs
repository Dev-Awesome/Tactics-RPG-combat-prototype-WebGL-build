using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleEnums;

public class AiAction {

	public GridCell moveTarget { get; private set; }
	public GridCell abilityTarget { get; private set; }
	public HashSet<GridCell> area { get; private set; }
	public int abilityIndex { get; private set; }
	public int apCost { get; private set; }
	public int mpCost { get; private set; }
	public int actionScore;

	public AiAction(int index, GridCell moveTo, int apC, int mpC, GridCell target = null, HashSet<GridCell> area = null) {
		moveTarget = moveTo;
		abilityTarget = target;
		abilityIndex = index;
		actionScore = 0;
		apCost = apC;
		mpCost = mpC;
		this.area = area;
	}
}
