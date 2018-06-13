//using UnityEditor;
//using UnityEngine;
//using BattleEnums;
//
//[CustomEditor(typeof(Weapon))]
//public class WeaponEditor : Editor {
//
//	Weapon weapon;
//
//	void OnEnable()
//	{
//		weapon = (Weapon)target;
//	}
//
//	public override void OnInspectorGUI()
//	{
//		GUILayout.Label ("Weapon Editor", EditorStyles.boldLabel);
//
//		DrawDefaultInspector ();
//
//		DrawDamageProperties ();
//		DrawHealProperties ();
//	}
//
//	void AddDamageProperty()
//	{
//		weapon.damageProperties.Add (new DamageProperty ());
//	}
//
//	void RemoveDamageProperty(int index)
//	{
//		weapon.damageProperties.RemoveAt (index);
//	}
//
//	void AddHealProperty()
//	{
//		weapon.healProperties.Add (new HealProperty ());
//	}
//
//	void RemoveHealProperty(int index)
//	{
//		weapon.healProperties.RemoveAt (index);
//	}
//
//	void DrawDamageProperties()
//	{
//		GUILayout.Space (20);
//		GUILayout.BeginHorizontal ();
//		GUILayout.Label ("Damage Properties", EditorStyles.boldLabel);
//		if (GUILayout.Button ("Add", GUILayout.Width(40))) {
//			AddDamageProperty ();
//		}
//		GUILayout.EndHorizontal ();
//
//		GUILayout.BeginVertical("box");
//
//		if (weapon.damageProperties.Count == 0) {
//			GUILayout.Label ("No Damage Properties Added");
//		}
//
//		for (int i = 0; i < weapon.damageProperties.Count; i++) {
//
//			//Start damage effect info.
//			GUILayout.BeginVertical("box");
//			GUILayout.Space (5);
//			GUILayout.BeginHorizontal ();
//
//			GUILayout.Label ("Element:");
//			weapon.damageProperties [i].element = (eElement)EditorGUILayout.EnumPopup (weapon.damageProperties [i].element, GUILayout.Width(60));
//
//			GUILayout.Label ("Min Damage:");
//			weapon.damageProperties [i].damagePerLevel[0].min = EditorGUILayout.IntField (weapon.damageProperties [i].damagePerLevel[0].min, GUILayout.Width(40));
//
//			GUILayout.Label ("Max Damage:");
//			weapon.damageProperties [i].damagePerLevel[0].max = EditorGUILayout.IntField (weapon.damageProperties [i].damagePerLevel[0].max, GUILayout.Width(40));
//
//			if (GUILayout.Button ("X", GUILayout.Height(16))) {
//				RemoveDamageProperty (i);
//				return;
//			}
//
//			GUILayout.EndHorizontal ();
//
//			GUILayout.Label ("Stat Scale Factor:");
//			weapon.damageProperties [i].scaleFactor = (eStat)EditorGUILayout.EnumPopup (weapon.damageProperties [i].scaleFactor, GUILayout.Width(40));
//
//			//Affected units.
//			GUILayout.Space (10);
//			GUILayout.BeginHorizontal();
//			GUILayout.Label ("Affected Units:");
//
//			if (GUILayout.Button ("Add", GUILayout.Width(40))) {
//				if (weapon.damageProperties [i].affectedUnits.Count == 0) {
//					weapon.damageProperties [i].affectedUnits.Add (eUnitType.ENEMY);
//				}
//				else if (!weapon.damageProperties [i].affectedUnits.Contains (eUnitType.ENEMY)) {
//					weapon.damageProperties [i].affectedUnits.Add(eUnitType.ENEMY);
//				}
//				else if (!weapon.damageProperties [i].affectedUnits.Contains (eUnitType.ENEMY_SUMMON)) {
//					weapon.damageProperties [i].affectedUnits.Add(eUnitType.ENEMY_SUMMON);
//				}
//				else if (!weapon.damageProperties [i].affectedUnits.Contains (eUnitType.ALLY)) {
//					weapon.damageProperties [i].affectedUnits.Add(eUnitType.ALLY);
//				}
//				else if (!weapon.damageProperties [i].affectedUnits.Contains (eUnitType.ALLIED_SUMMON)) {
//					weapon.damageProperties [i].affectedUnits.Add(eUnitType.ALLIED_SUMMON);
//				}
//
//			}
//			if (GUILayout.Button ("Add All", GUILayout.Width(60))) {
//				weapon.damageProperties [i].affectedUnits.Add(eUnitType.ENEMY);
//				weapon.damageProperties [i].affectedUnits.Add(eUnitType.ALLY);
//				weapon.damageProperties [i].affectedUnits.Add(eUnitType.ENEMY_SUMMON);
//				weapon.damageProperties [i].affectedUnits.Add(eUnitType.ALLIED_SUMMON);
//			}
//			if (GUILayout.Button ("Remove", GUILayout.Width(60))) {
//				if (weapon.damageProperties [i].affectedUnits.Count > 0) {
//					weapon.damageProperties [i].affectedUnits.RemoveAt (weapon.damageProperties [i].affectedUnits.Count - 1);
//				}
//			}
//			GUILayout.EndHorizontal();
//			weapon.damageProperties [i].targetCasterInstead = EditorGUILayout.Toggle ("Target Caster:", weapon.damageProperties [i].targetCasterInstead);
//			GUILayout.Space (8);
//
//			if (weapon.damageProperties [i].targetCasterInstead) {
//				GUILayout.Label ("Target Caster Override Enabled", EditorStyles.boldLabel);
//			}
//
//			GUILayout.BeginHorizontal();
//
//			if (weapon.damageProperties [i].affectedUnits.Count == 0 && !weapon.damageProperties [i].targetCasterInstead) {
//				GUILayout.Label ("No affected Units Added");
//			}
//			for (int j = 0; j < weapon.damageProperties[i].affectedUnits.Count; j++) {
//				
//				weapon.damageProperties[i].affectedUnits[j] = (eUnitType)EditorGUILayout.EnumPopup (weapon.damageProperties[i].affectedUnits[j], GUILayout.Width(100));
//			}
//			GUILayout.EndHorizontal();
//			GUILayout.EndVertical ();
//			//End damage effect info.
//		}
//		GUILayout.EndVertical ();
//	}
//
//	void DrawHealProperties()
//	{
//		GUILayout.Space (20);
//		GUILayout.BeginHorizontal ();
//		GUILayout.Label ("Healing Properties", EditorStyles.boldLabel);
//		if (GUILayout.Button ("Add", GUILayout.Width(40))) {
//			AddHealProperty ();
//		}
//		GUILayout.EndHorizontal ();
//
//		GUILayout.BeginVertical("box");
//		GUILayout.Space (5);
//
//		if (weapon.healProperties.Count == 0) {
//			GUILayout.Label ("No Healing Properties Added");
//		}
//
//		for (int i = 0; i < weapon.healProperties.Count; i++) {
//
//			//Start heal effect info.
//			GUILayout.BeginVertical("box");
//
//			GUILayout.BeginHorizontal ();
//
//			GUILayout.Label ("Min Heal:");
//			weapon.healProperties [i].healPerLevel[0].min = EditorGUILayout.IntField (weapon.healProperties [i].healPerLevel[0].min, GUILayout.Width(100));
//
//			GUILayout.Label ("Max Heal:");
//			weapon.healProperties [i].healPerLevel[0].max = EditorGUILayout.IntField (weapon.healProperties [i].healPerLevel[0].max, GUILayout.Width(100));
//
//			if (GUILayout.Button ("X", GUILayout.Height(16), GUILayout.Width(25))) {
//				RemoveHealProperty (i);
//				return;
//			}
//
//			GUILayout.EndHorizontal ();
//
//			GUILayout.Label ("Stat Scale Factor:");
//			weapon.healProperties [i].scaleFactor = (eStat)EditorGUILayout.EnumPopup (weapon.healProperties [i].scaleFactor, GUILayout.Width(40));
//
//			//Affected units.
//			GUILayout.Space (10);
//			GUILayout.BeginHorizontal();
//			GUILayout.Label ("Affected Units:");
//			if (GUILayout.Button ("Add", GUILayout.Width(40))) {
//				if (weapon.healProperties [i].affectedUnits.Count == 0) {
//					weapon.healProperties [i].affectedUnits.Add (eUnitType.ALLY);
//				}
//				else if (!weapon.healProperties [i].affectedUnits.Contains (eUnitType.ALLY)) {
//					weapon.healProperties [i].affectedUnits.Add(eUnitType.ALLY);
//				}
//				else if (!weapon.healProperties [i].affectedUnits.Contains (eUnitType.ALLIED_SUMMON)) {
//					weapon.healProperties [i].affectedUnits.Add(eUnitType.ALLIED_SUMMON);
//				}
//				else if (!weapon.healProperties [i].affectedUnits.Contains (eUnitType.ENEMY)) {
//					weapon.healProperties [i].affectedUnits.Add(eUnitType.ENEMY);
//				}
//				else if (!weapon.healProperties [i].affectedUnits.Contains (eUnitType.ENEMY_SUMMON)) {
//					weapon.healProperties [i].affectedUnits.Add(eUnitType.ENEMY_SUMMON);
//				}
//
//			}
//			if (GUILayout.Button ("Add All", GUILayout.Width(60))) {
//				weapon.healProperties [i].affectedUnits.Add(eUnitType.ENEMY);
//				weapon.healProperties [i].affectedUnits.Add(eUnitType.ALLY);
//				weapon.healProperties [i].affectedUnits.Add(eUnitType.ENEMY_SUMMON);
//				weapon.healProperties [i].affectedUnits.Add(eUnitType.ALLIED_SUMMON);
//			}
//			if (GUILayout.Button ("Remove", GUILayout.Width(60))) {
//				if (weapon.healProperties [i].affectedUnits.Count > 0) {
//					weapon.healProperties [i].affectedUnits.RemoveAt (weapon.healProperties [i].affectedUnits.Count - 1);
//				}
//			}
//			GUILayout.EndHorizontal();
//			weapon.healProperties [i].targetCasterInstead = EditorGUILayout.Toggle ("Target Caster:", weapon.healProperties [i].targetCasterInstead);
//			GUILayout.Space (8);
//
//			if (weapon.healProperties [i].targetCasterInstead) {
//				GUILayout.Label ("Target Caster Override Enabled", EditorStyles.boldLabel);
//			}
//			GUILayout.BeginHorizontal();
//
//			if (weapon.healProperties [i].affectedUnits.Count == 0 && !weapon.healProperties [i].targetCasterInstead) {
//				GUILayout.Label ("No affected Units Added");
//			}
//			for (int j = 0; j < weapon.healProperties[i].affectedUnits.Count; j++) {
//
//				weapon.healProperties[i].affectedUnits[j] = (eUnitType)EditorGUILayout.EnumPopup (weapon.healProperties[i].affectedUnits[j], GUILayout.Width(100));
//			}
//			GUILayout.EndHorizontal();
//
//			GUILayout.EndVertical ();
//			//End heal effect info.
//		}
//		GUILayout.EndVertical ();
//
//	}
//}
