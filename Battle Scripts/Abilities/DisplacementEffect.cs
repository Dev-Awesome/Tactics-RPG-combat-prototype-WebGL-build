using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleEnums;

[CreateAssetMenu (menuName = "Abilities/Displacement Effect")]
public class DisplacementEffect : Effect {

	public int[] distancePerLevel = new int[5];
	public float blinkDelay = 0.3f;

	public bool targetCaster = false;

	public eDisplacementType displacementType;

	public override void ApplyEffect(Unit caster, GridCell target, int level, bool crit)
	{
		switch (displacementType) {
		case eDisplacementType.PUSH:
			if (targetCaster) {
				string facing = target.GetFacing (caster.currentLocation);
				GridCell displaceLocation = Pathfinder.GetDisplacementLocation(caster.currentLocation, facing, distancePerLevel[level -1]);
				caster.ForcedDisplace (displaceLocation, displacementType);
			} else {
				if (IsAffected(target)) {
					string facing = caster.currentLocation.GetFacing (target);
					GridCell displaceLocation = Pathfinder.GetDisplacementLocation(target, facing, distancePerLevel[level -1]);
					target.currentUnit.ForcedDisplace (displaceLocation, displacementType);
				}
			}
			break;

		case eDisplacementType.PULL:
			if (targetCaster) {
				string facing = caster.currentLocation.GetFacing (target);
				GridCell displaceLocation = Pathfinder.GetDisplacementLocation(caster.currentLocation, facing, distancePerLevel[level -1]);
				caster.ForcedDisplace (displaceLocation, displacementType);
			} else {
				if (IsAffected(target)) {
					string facing = target.GetFacing (caster.currentLocation);
					GridCell displaceLocation = Pathfinder.GetDisplacementLocation(target, facing, distancePerLevel[level -1]);
					target.currentUnit.ForcedDisplace (displaceLocation, displacementType);
				}
			}
			break;

		case eDisplacementType.BLINK:
			caster.BlinkToTarget(target, blinkDelay);
			break;

		case eDisplacementType.SWAP:
			caster.ForcedDisplace (target, displacementType);
			break;
		}
	}

	private bool IsAffected(GridCell target)
	{
		if (target.currentUnit != null) {
			if (affectsAll) {
				return true;
			} else if (affectedUnits.Contains (target.currentUnit.unitType)) {
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}
	}
}
