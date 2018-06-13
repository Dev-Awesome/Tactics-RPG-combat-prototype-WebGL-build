using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleEnums;

public class StatusController : MonoBehaviour {

	private Unit unit;
	public List<StatusEffect> statusEffects = new List<StatusEffect>();

	void Start()
	{
		unit = GetComponent<Unit> ();
	}

	public void AddStatusEffect(Unit source, StatusModifier mod, int level)
	{
		statusEffects.Add (new StatusEffect (mod, source, level, mod.durationPerLevel [level - 1], mod.applyEachTurn));
	}

	public void StartTurn()
	{
		foreach (StatusEffect effect in statusEffects) {
			if (effect.applyEachTurn) {
				effect.ApplyOnTurnStart (unit);
			}
		}
	}

	public void EndTurn()
	{
		for (int i = statusEffects.Count - 1; i >= 0; i--) {
			statusEffects [i].remainingDuration--;
			if (statusEffects [i].remainingDuration <= 0) {
				statusEffects [i].RemoveEffect (unit);
				statusEffects.RemoveAt (i);
			}
		}
	}

	public void ApplyQueuedEffects()
	{
		foreach (StatusEffect effect in statusEffects) {
			if (!effect.wasApplied) {
				effect.ApplyImmediately (unit);
				effect.wasApplied = true;
			}
		}
	}

	public class StatusEffect {

		public StatusModifier statMod;
		public int level;
		public Unit source;
		public bool applyEachTurn;
		public int remainingDuration;
		public bool wasApplied;
		public int val = 0;

		public StatusEffect(StatusModifier statMod, Unit caster, int level, int duration, bool constant) {
			applyEachTurn = constant;
			remainingDuration = duration;
			this.statMod = statMod;
			this.level = level;
			source = caster;
			wasApplied = false;
		}

		public void ApplyImmediately(Unit target)
		{
			statMod.ApplyImmediately (source, target, level, this);
		}

		public void ApplyOnTurnStart(Unit target)
		{
			statMod.ApplyOnTurnStart (source, target, level);
		}

		public void RemoveEffect(Unit target)
		{
			statMod.RemoveEffect (target, val);
		}

		public void SetValue(int val)
		{
			this.val = val;
		}
	}
}
