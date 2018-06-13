using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleEnums;

public abstract class AiBehaviour : ScriptableObject {

	public float offencePriority;
	public float defencePriority;
	public float disablePriority;
	public float displacePriority;

	public abstract void AssessAction (Unit actor, AiAction action);

	public abstract GridCell FindMoveTarget (Unit actor);
}
