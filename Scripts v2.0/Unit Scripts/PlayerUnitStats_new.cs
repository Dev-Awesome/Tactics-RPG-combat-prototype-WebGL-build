using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnitStats_new : UnitStats {

	//Player unit % resist cap.

	private const int playerResCap = 50;

	//Override base initialise method.

	public override void InitialiseStats()
	{
		
	}

	//Implement set method for % resists. Capped for player units.
	//Index: 0 - neutral, 1 - earth, 2 - fire, 3 - air, 4 - water.

	public override void ModifyPercentResist(int i, int val)
	{
		switch (i) {
		case 0:
			if (p_neutralRes + val > playerResCap) {
				p_neutralRes = playerResCap;
			} else {
				p_neutralRes += val;
			}
			break;

		case 1:
			if (p_earthRes + val > playerResCap) {
				p_earthRes = playerResCap;
			} else {
				p_earthRes += val;
			}
			break;

		case 2:
			if (p_fireRes + val > playerResCap) {
				p_fireRes = playerResCap;
			} else {
				p_fireRes += val;
			}
			break;

		case 3:
			if (p_airRes + val > playerResCap) {
				p_airRes = playerResCap;
			} else {
				p_airRes += val;
			}
			break;

		case 4:
			if (p_waterRes + val > playerResCap) {
				p_waterRes = playerResCap;
			} else {
				p_waterRes += val;
			}
			break;

		default:
			break;
		}
	}
}
