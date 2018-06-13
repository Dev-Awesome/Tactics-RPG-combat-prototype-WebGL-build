using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleEnums;

public class AnimationEventHandler : MonoBehaviour {

	private BattleInputController inputController;

	public void SignalHitFrame()
	{
		if (inputController == null) {
			inputController = GameObject.FindGameObjectWithTag ("Battle Controller").GetComponent<BattleInputController> ();
		}
		if (BattleController.instance.currentUnit.unitType == eUnitType.ALLY) {
			inputController.SignalUnits ();
		} else {
			inputController.AiSignalUnits (gameObject.GetComponentInParent<AiController> ().targetCells);
		}
	}
}
