//using UnityEditor;
//using UnityEngine;
//using BattleEnums;
//
//[CustomEditor(typeof(Ability))]
//public class AbilityEditorOld : Editor {
//
//	Ability ability;
//
//	enum PropertyType {
//		DAMAGE,
//		HEAL,
//		SHIELD,
//		DISPLACEMENT,
//		BUFF_DEBUFF,
//		HP_OVER_TIME
//	}
//
//	PropertyType propertyType;
//
//	void OnEnable()
//	{
//		ability = (Ability)target;
//	}
//
//	public override void OnInspectorGUI()
//	{
//		GUILayout.Label ("Ability Editor", EditorStyles.boldLabel);
//
//		//DrawDefaultInspector ();
//
//		DrawEffectsPanel ();
//	}
//
//	void AddProperty()
//	{
//		switch (propertyType) {
//		case PropertyType.DAMAGE:
//			ability.damageProperties.Add (new DamageProperty ());
//			break;
//
//		case PropertyType.HEAL:
//
//			break;
//
//		case PropertyType.SHIELD:
//	
//			break;
//
//		case PropertyType.DISPLACEMENT:
//	
//			break;
//
//		case PropertyType.BUFF_DEBUFF:
//
//			break;
//
//		case PropertyType.HP_OVER_TIME:
//
//			break;
//		}
//	}
//
//	void DrawEffectsPanel()
//	{
//		GUILayout.BeginVertical("box");
//		GUILayout.BeginHorizontal();
//
//		GUILayout.Label ("Property Type:");
//		propertyType = (PropertyType)EditorGUILayout.EnumPopup (propertyType, GUILayout.Height(15));
//
//		if (GUILayout.Button ("Add")) {
//			AddProperty ();
//		}
//
//		GUILayout.EndHorizontal ();
//
//		for (int i = 0; i < ability.damageProperties.Count; i++) {
//
//			DrawDamageProperty (ability.damageProperties [i]);
//		}
//
//		GUILayout.EndVertical ();
//	}
//
//	void DrawDamageProperty(DamageProperty prop)
//	{
//		GUILayout.BeginHorizontal ();
//
//		GUILayout.Label ("Element:");
//		prop.element = (eElement)EditorGUILayout.EnumPopup (prop.element, GUILayout.Width(60));
//
//		GUILayout.Label ("Min Damage:");
//		prop.damagePerLevel[0].min = EditorGUILayout.IntField (prop.damagePerLevel[0].min, GUILayout.Width(40));
//
//		GUILayout.Label ("Max Damage:");
//		prop.damagePerLevel[0].max = EditorGUILayout.IntField (prop.damagePerLevel[0].max, GUILayout.Width(40));
//
//		if (GUILayout.Button ("X", GUILayout.Height(16))) {
//			
//			//return;
//		}
//
//		GUILayout.EndHorizontal ();
//	}
//}
