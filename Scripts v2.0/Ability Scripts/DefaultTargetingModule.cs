using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Data Objects/Targeting Modules/Default")]
public class DefaultTargetingModule : TargetingModule {

	public override void TargetSelect (GridCell origin, int level, int bonusRange, Ability_new ability)
	{
		Reset ();

		int maxRange = ability.adjustableRange ? ability.maxRangePerLvl [level - 1] + bonusRange : ability.maxRangePerLvl[level - 1];

		if (maxRange < ability.minRange) {
			maxRange = ability.minRange;
		}

		bool requiresLoS = ability.requiresLoS;

		//non linear
		if (!ability.isLinear) {
			foreach (GridCell c in grid.Values) {
				if (origin.CalcDistance (c) <= maxRange && origin.CalcDistance (c) >= ability.minRange) {
					if (!targetable.Contains (c) && !untargetable.Contains(c)) {

						if (!untargetable.Contains (c)) {
							untargetable.Add (c);
						}

						if (requiresLoS) {
							GridCell hit = CheckLoS (origin.gridPos.x, origin.gridPos.y, c.gridPos.x, c.gridPos.y);
							if (!targetable.Contains (hit)) {
								targetable.Add (hit);
							}
						} else {
							if (!targetable.Contains (c)) {
								targetable.Add (c);
							}
						}
					}
				}
			}
		} else {
			//linear
			for (int x = -maxRange; x <= maxRange; x++) {
				GridPos pos = new GridPos (origin.gridPos.x - x, origin.gridPos.y);
				if (grid.ContainsKey (pos)) {
					GridCell c = grid [pos];
					if (c.CalcDistance (origin) <= maxRange && c.CalcDistance (origin) >= ability.minRange) {
						if (!targetable.Contains (c) && !untargetable.Contains(c)) {

							if (!untargetable.Contains (c)) {
								untargetable.Add (c);
							}

							if (requiresLoS) {
								GridCell hit = CheckLoS (origin.gridPos.x, origin.gridPos.y, c.gridPos.x, c.gridPos.y);
								if (!targetable.Contains (hit)) {
									targetable.Add (hit);
								}
							} else {
								if (!targetable.Contains (c)) {
									targetable.Add (c);
								}
							}
						}
					}
				}
			}
			for (int y = -maxRange; y <= maxRange; y++) {
				GridPos pos = new GridPos (origin.gridPos.x, origin.gridPos.y - y);
				if (grid.ContainsKey (pos)) {
					GridCell c = grid [pos];
					if (c.CalcDistance (origin) <= maxRange && c.CalcDistance (origin) >= ability.minRange) {
						if (!targetable.Contains (c) && !untargetable.Contains(c)) {

							if (!untargetable.Contains (c)) {
								untargetable.Add (c);
							}

							if (requiresLoS) {
								GridCell hit = CheckLoS (origin.gridPos.x, origin.gridPos.y, c.gridPos.x, c.gridPos.y);
								if (!targetable.Contains (hit)) {
									targetable.Add (hit);
								}
							} else {
								if (!targetable.Contains (c)) {
									targetable.Add (c);
								}
							}
						}
					}
				}
			}
		}
	}

	public override List<Unit_new> GetAffectedUnits()
	{
		List<Unit_new> affectedUnits = new List<Unit_new> ();



		return affectedUnits;
	}

	void ShowArea(GridCell origin, Ability_new.AreaType type, int level)
	{
		
	}

	GridCell CheckLoS(int x0, int y0, int x1, int y1)
	{
		GridPos origin = new GridPos (x0, y0);
		int dx = Mathf.Abs(x1 - x0);
		int dy = Mathf.Abs(y1 - y0);
		int x = x0;
		int y = y0;
		int n = 1 + dx + dy;
		int x_inc = (x1 > x0) ? 1 : -1;
		int y_inc = (y1 > y0) ? 1 : -1;
		int error = dx - dy;
		dx *= 2;
		dy *= 2;

		for (; n > 0; --n)
		{
			if (!CheckGridCell (new GridPos (x, y), origin)) {
				return grid [new GridPos (x, y)];
			} else {

				if (error > 0) {
					x += x_inc;
					error -= dy;
				} else {
					y += y_inc;
					error += dx;
				}
			}
		}
		return grid [new GridPos (x1, y1)];
	}

	bool CheckGridCell (GridPos pos, GridPos origin)
	{
		if (grid.ContainsKey (pos)) {
			GridCell c = grid [pos];
			if (c.gridPos == origin) {
				return true;
			} else {
				bool ret = c.CanShootThrough ();
				return ret;
			}
		} else {
			return false;
		}
	}
}
