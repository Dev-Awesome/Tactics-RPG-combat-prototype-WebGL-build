using UnityEditor;
using UnityEngine;
using BattleEnums;

[CustomEditor(typeof(Ability))]
public class AbilityEditor : Editor {

	Ability ability;

	//Display options.
	bool showGenericInfo = false;
	bool showUsage = false;
	bool showRange = false;
	bool showEffects = false;
	bool showCritEffects = false;
	bool showAnimation = false;

	int editEffectIndex = int.MaxValue;
	int total;
	int critTotal;
	int showLevel = 1;

	//Text colours.
	static Color32 fire;
	static Color earth;
	static Color water;
	static Color air;

	GUIStyle alignRight = new GUIStyle ();
	GUIStyle alignLeft = new GUIStyle ();
	GUIStyle highlightText = new GUIStyle ();
	GUIStyle titleStyle = new GUIStyle ();

	enum PropertyType {
		DAMAGE,
		HEAL,
		SHIELD,
		DISPLACEMENT,
		BUFF_DEBUFF,
		HP_OVER_TIME
	}

	PropertyType propertyType;

	void OnEnable()
	{
		ability = (Ability)target;
	}

	public override void OnInspectorGUI()
	{
		SetStyles();

		DrawSummaryPanel ();
		GUILayout.Space (10);

		DrawGenericInfo ();
		GUILayout.Space (5);

		DrawUsagePanel ();
		GUILayout.Space (5);

		DrawRangePanel ();
		GUILayout.Space (5);

		DrawEffectsPanel ();
		GUILayout.Space (5);

		DrawCritEffectsPanel ();
		GUILayout.Space (5);

		DrawAnimationPanel ();

		if (GUI.changed) {
			EditorUtility.SetDirty(ability);
		}
	}

	private void DrawSummaryPanel()
	{
		GUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("Ability Editor", titleStyle, GUILayout.MaxWidth(100));
		GUILayout.EndHorizontal ();
		GUILayout.Space (20);

		EditorGUILayout.LabelField ("Summary:", EditorStyles.boldLabel);
		GUILayout.BeginVertical ("box", GUILayout.MaxWidth(200));
		GUILayout.Space (5);
		EditorGUILayout.BeginHorizontal();

		string cd = ability.cooldownPerLvl [showLevel - 1] > 0 ? ability.cooldownPerLvl [showLevel - 1].ToString() : "No Cooldown";

		string range = ability.minRangePerLvl [showLevel - 1] + " - " + ability.maxRangePerLvl [showLevel - 1];

		if (ability.minRangePerLvl [showLevel - 1] == 0 && ability.maxRangePerLvl [showLevel - 1] == 0) {
			range = "Self Cast";
		}

		if (ability.displacementEffects.Count > 0) {
			foreach (DisplacementProperty dp in ability.displacementEffects) {
				if (dp.displacementType == eDisplacementType.BLINK) {
					ability.requiresFreeCell = true;
				}
			}
		}

		string ap = ability.apCostPerLvl [showLevel - 1].ToString ();

		string area = "Single Target";

		string los = showLevel >= ability.noLoSFromLvl ? "Doesn't Require Line of Sight" : "Requires Line of Sight";

		string adjustRange = ability.adjustableRange ? "Range Adjustable" : "Range Not Adjustable";

		string linear = showLevel >= ability.nonLinearFromLvl ? "Non Linear" : "Linear";

		switch (ability.areaType) {
		case eAreaType.CIRCLE:
			area = "Cirlce AoE - " + ability.areaSizePerLvl [showLevel - 1] + " cell(s)";
			break;

		case eAreaType.CROSS:
			area = "Cross AoE - " + ability.areaSizePerLvl[showLevel - 1] + " cell(s)";
			break;

		case eAreaType.H_LINE:
			area = "Horizontal Line AoE - " + ability.areaSizePerLvl[showLevel - 1] + " cell(s)";
			break;

		case eAreaType.V_LINE:
			area = "Vertical Line AoE - " + ability.areaSizePerLvl[showLevel - 1] + " cell(s)";
			break;
		}

		if (ability.abilityName == null || ability.abilityName == "") {
			ability.abilityName = "New Ability";
		}

		GUILayout.Space (5);
		EditorGUILayout.LabelField ("Ability:", GUILayout.Width(60));
		EditorGUILayout.LabelField (ability.abilityName.ToString(), EditorStyles.boldLabel, GUILayout.MaxWidth(200));
		EditorGUILayout.LabelField ("Preview Level:", GUILayout.Width(100));
		showLevel = EditorGUILayout.IntSlider (showLevel, 1, 5, GUILayout.Width(120));
		GUILayout.Space(5);
		EditorGUILayout.EndHorizontal ();

		GUILayout.Space (15);
		GUILayout.BeginHorizontal ();
		GUILayout.Space (5);
		EditorGUILayout.LabelField ("Ap Cost:", GUILayout.Width(60));
		EditorGUILayout.LabelField (ap, EditorStyles.boldLabel, GUILayout.MaxWidth(50));
		EditorGUILayout.LabelField ("Range:", GUILayout.Width(50));
		EditorGUILayout.LabelField (range, EditorStyles.boldLabel, GUILayout.MaxWidth(60));
		EditorGUILayout.LabelField (adjustRange, GUILayout.MaxWidth(180));
		EditorGUILayout.LabelField (los,alignRight,GUILayout.MaxWidth(120));
		GUILayout.EndHorizontal ();

		GUILayout.Space (10);

		GUILayout.BeginHorizontal ();
		GUILayout.Space (5);
		EditorGUILayout.LabelField ("Cooldown:", GUILayout.Width(60));
		EditorGUILayout.LabelField (cd, EditorStyles.boldLabel, GUILayout.MaxWidth(120));
		EditorGUILayout.LabelField ("Area Of Effect:", GUILayout.Width(80));
		EditorGUILayout.LabelField (area, EditorStyles.boldLabel, GUILayout.MaxWidth(160));
		EditorGUILayout.LabelField (linear, GUILayout.MaxWidth(80));
		GUILayout.EndHorizontal ();
		GUILayout.Space (5);

		if (ability.requiresTarget && ability.requiresFreeCell) {
			GUILayout.BeginHorizontal ();
			GUILayout.Space (5);
			EditorGUILayout.LabelField ("WARNING: Cannot require both target and free cell.", highlightText);
			GUILayout.EndHorizontal ();
			GUILayout.Space (5);
		} else if (ability.requiresFreeCell) {
			GUILayout.BeginHorizontal ();
			GUILayout.Space (5);
			EditorGUILayout.LabelField ("Requires Free Cell", highlightText);
			GUILayout.EndHorizontal ();
			GUILayout.Space (5);
		} else if (ability.requiresTarget) {
			GUILayout.BeginHorizontal ();
			GUILayout.Space (5);
			EditorGUILayout.LabelField ("Requires Target", highlightText);
			GUILayout.EndHorizontal ();
			GUILayout.Space (5);
		}

		GUILayout.EndVertical ();
	}

	void SetStyles()
	{
		alignRight.alignment = TextAnchor.MiddleRight;
		alignRight.padding.right = 20;

		alignLeft.alignment = TextAnchor.UpperLeft;

		fire = GetColour(139f, 27f, 27f);
		earth = GetColour(131f, 91f, 6f);
		air = GetColour(28f, 98f, 14f);
		water = GetColour(22f, 109f, 144f);

		highlightText.normal.textColor = Color.red;

		titleStyle.normal.textColor = fire;
		titleStyle.fontSize = 14;
	}

	private Color GetColour(float r, float g, float b)
	{
		r = r / 255f;
		g = g / 255f;
		b = b / 255f;

		return new Color (r, g, b);
	}

	private int CountEffects(bool crit)
	{
		int count = 0;

		if (crit) {
			count = ability.critDamageEffects.Count + ability.critHealEffects.Count + ability.critDisplacementEffects.Count + ability.critBuffDebuffEffects.Count;
		} else {
			count = ability.damageEffects.Count + ability.healEffects.Count + ability.displacementEffects.Count + ability.buffDebuffEffects.Count;
		}
		return count;
	}

	void DrawEffectsPanel()
	{
		//Effects label and toggle.
		EditorGUILayout.BeginHorizontal();
		showEffects = EditorGUILayout.ToggleLeft ("Effects:", showEffects, EditorStyles.boldLabel);
		EditorGUILayout.LabelField ("Effects Total: " + CountEffects(false), alignRight);

		EditorGUILayout.EndHorizontal ();

		//Effects box.
		if (showEffects) {
			EditorGUILayout.BeginVertical ("box");

			//Add effect controls.
			GUILayout.Space(10);
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space(5);

			propertyType = (PropertyType)EditorGUILayout.EnumPopup (propertyType, GUILayout.MaxWidth(120));

			if (GUILayout.Button ("Add", EditorStyles.miniButtonRight, GUILayout.Width(50))) {
				AddProperty ();
			}

			GUILayout.Space(100);
			EditorGUILayout.LabelField ("Preview Level:", GUILayout.Width(100));
			showLevel = EditorGUILayout.IntSlider (showLevel, 1, 5, GUILayout.Width(120));
			GUILayout.Space(5);

			EditorGUILayout.EndHorizontal ();
			GUILayout.Space(10);

			if (total < 1) {
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space(5);
				EditorGUILayout.LabelField ("No Effects Added");
				EditorGUILayout.EndHorizontal ();
			}

			total = 0;
			for (int i = 0; i < ability.damageEffects.Count; i++) {
				DrawDamageProperty (ability.damageEffects [i], i, false);
				total++;
			}

			for (int i = 0; i < ability.healEffects.Count; i++) {
				DrawHealProperty (ability.healEffects [i], i, false);
				total++;
			}

			for (int i = 0; i < ability.displacementEffects.Count; i++) {
				DrawDisplacementProperty (ability.displacementEffects [i], i, false);
				total++;
			}

			for (int i = 0; i < ability.buffDebuffEffects.Count; i++) {
				DrawBuffDebuffProperty (ability.buffDebuffEffects [i], i, false);
				total++;
			}

			GUILayout.EndVertical ();
		}
	}

	void DrawDamageProperty(DamageProperty prop, int index, bool crit)
	{
		int propID = crit ? critTotal + total : total;

		EditorGUILayout.BeginVertical ("box");
		GUILayout.Space(5);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Space(5);

		DisplayElementLabel (prop.element);

		EditorGUILayout.LabelField (prop.damagePerLevel [showLevel - 1].min + " - " + prop.damagePerLevel [showLevel - 1].max, EditorStyles.boldLabel, GUILayout.MaxWidth(60));
		EditorGUILayout.LabelField ("at level " + showLevel, GUILayout.MinWidth(60));

		DisplayScaleFactor (prop.scaleFactor);

		if (editEffectIndex == propID) {
			if (GUILayout.Button ("Hide", EditorStyles.miniButtonLeft, GUILayout.Width(60))) {
				editEffectIndex = int.MaxValue;
			}
		} else {
			if (GUILayout.Button ("Show", EditorStyles.miniButtonLeft, GUILayout.Width(60))) {
				editEffectIndex = propID;
			}
		}

		if (GUILayout.Button ("Remove", EditorStyles.miniButtonRight, GUILayout.Width(60))) {
			if (!crit) {
				ability.damageEffects.RemoveAt (index);
				total--;
			} else {
				ability.critDamageEffects.RemoveAt (index);
				critTotal--;
			}
			return;
		}
		GUILayout.Space(5);
		EditorGUILayout.EndHorizontal ();
		GUILayout.Space(2);

		if (editEffectIndex == propID) {

			GUILayout.Space(4);
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space(4);
			EditorGUILayout.LabelField ("Element:", GUILayout.Width(60));
			prop.element = (eElement)EditorGUILayout.EnumPopup (prop.element, GUILayout.Width(80), GUILayout.Height(15));
			GUILayout.Space(15);
			EditorGUILayout.LabelField ("Scaling:", GUILayout.Width(60));
			prop.scaleFactor = (eStat)EditorGUILayout.EnumPopup (prop.scaleFactor, GUILayout.Width(80), GUILayout.Height(15));
			EditorGUILayout.EndHorizontal ();
			GUILayout.Space(2);

			for (int i = 0; i < 5; i++) {
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space(4);
				EditorGUILayout.LabelField ("Level " + (i + 1) + ":", GUILayout.Width(60));
				GUILayout.Label ("Min Damage:");
				prop.damagePerLevel[i].min = EditorGUILayout.IntField (prop.damagePerLevel[i].min, GUILayout.Width(40));
				GUILayout.Space (60);
				GUILayout.Label ("Max Damage:");
				prop.damagePerLevel[i].max = EditorGUILayout.IntField (prop.damagePerLevel[i].max, GUILayout.Width(40));
				GUILayout.Space (60);
				EditorGUILayout.EndHorizontal ();
			}
			GUILayout.Space (5);
		}

		//Affected Units.
		DrawAffectedUnitsPanel(propID, ref prop.targetCasterOverride, ref prop.affectsCaster, ref prop.affectsAllyPlayers, ref prop.affectsAllySummons, ref prop.affectsEnemyPlayers, ref prop.affectsEnemySummons);

		EditorGUILayout.EndVertical ();
	}

	void AddProperty(bool critEffect = false)
	{
		switch (propertyType) {
		case PropertyType.DAMAGE:
			if (critEffect) {
				ability.critDamageEffects.Add(new DamageProperty());
			} else {
				ability.damageEffects.Add(new DamageProperty());
			}
			break;

		case PropertyType.HEAL:
			if (critEffect) {
				ability.critHealEffects.Add(new HealProperty());
			} else {
				ability.healEffects.Add(new HealProperty());
			}
			break;

		case PropertyType.SHIELD:

			break;

		case PropertyType.DISPLACEMENT:
			if (critEffect) {
				ability.critDisplacementEffects.Add(new DisplacementProperty());
			} else {
				ability.displacementEffects.Add(new DisplacementProperty());
			}
			break;

		case PropertyType.BUFF_DEBUFF:
			if (critEffect) {
				ability.critBuffDebuffEffects.Add(new BuffDebuffProperty());
			} else {
				ability.buffDebuffEffects.Add(new BuffDebuffProperty());
			}
			break;

		case PropertyType.HP_OVER_TIME:

			break;
		}
	}

	private void DisplayElementLabel(eElement element)
	{
		string label = element.ToString ().ToLower ();
		char[] a = label.ToCharArray();
		a[0] = char.ToUpper(a[0]);
		label = new string (a);

		GUIStyle fontColour = new GUIStyle ();
		fontColour.fontStyle = FontStyle.Bold;

		switch (element) {
		case eElement.FIRE:
			fontColour.normal.textColor = fire;
			break;

		case eElement.EARTH:
			fontColour.normal.textColor = earth;
			break;

		case eElement.AIR:
			fontColour.normal.textColor = air;
			break;

		case eElement.WATER:
			fontColour.normal.textColor = water;
			break;
		}

		EditorGUILayout.LabelField (label + " Damage:", fontColour, GUILayout.MaxWidth(100));
	}

	void DisplayScaleFactor(eStat stat)
	{
		string statName = "";

		switch (stat) {
		case eStat.STR:
			statName = "Strength";
			break;

		case eStat.INT:
			statName = "Intelligence";
			break;

		case eStat.AGI:
			statName = "Agility";
			break;

		case eStat.RES:
			statName = "Resolve";
			break;
		}

		EditorGUILayout.LabelField ("Scaling:  " + statName, GUILayout.MaxWidth(120));
	}

	//Heal property.

	void DrawHealProperty(HealProperty prop, int index, bool crit)
	{
		int propID = crit ? critTotal + total : total;

		EditorGUILayout.BeginVertical ("box");
		GUILayout.Space(5);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Space(5);

		EditorGUILayout.LabelField ("Heal:", EditorStyles.boldLabel, GUILayout.MaxWidth(100));
		EditorGUILayout.LabelField (prop.healPerLevel [showLevel - 1].min + " - " + prop.healPerLevel [showLevel - 1].max, EditorStyles.boldLabel, GUILayout.MaxWidth(60));
		EditorGUILayout.LabelField ("at level " + showLevel, GUILayout.MinWidth(60));

		DisplayScaleFactor (prop.scaleFactor);

		if (editEffectIndex == propID) {
			if (GUILayout.Button ("Hide", EditorStyles.miniButtonLeft, GUILayout.Width(60))) {
				editEffectIndex = int.MaxValue;
			}
		} else {
			if (GUILayout.Button ("Show", EditorStyles.miniButtonLeft, GUILayout.Width(60))) {
				editEffectIndex = propID;
			}
		}

		if (GUILayout.Button ("Remove", EditorStyles.miniButtonRight, GUILayout.Width(60))) {
			if (!crit) {
				ability.healEffects.RemoveAt (index);
				total--;
			} else {
				ability.critHealEffects.RemoveAt (index);
				critTotal--;
			}
			return;
		}
		GUILayout.Space(5);
		EditorGUILayout.EndHorizontal ();
		GUILayout.Space(2);

		if (editEffectIndex == propID) {

			GUILayout.Space(4);
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space(4);
			EditorGUILayout.LabelField ("Scaling:", GUILayout.Width(60));
			prop.scaleFactor = (eStat)EditorGUILayout.EnumPopup (prop.scaleFactor, GUILayout.Width(80), GUILayout.Height(15));
			EditorGUILayout.EndHorizontal ();
			GUILayout.Space(2);

			for (int i = 0; i < 5; i++) {
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space(4);
				EditorGUILayout.LabelField ("Level " + (i + 1) + ":", GUILayout.Width(60));
				GUILayout.Label ("Min Heal:");
				prop.healPerLevel[i].min = EditorGUILayout.IntField (prop.healPerLevel[i].min, GUILayout.Width(40));
				GUILayout.Space (60);
				GUILayout.Label ("Max Heal:");
				prop.healPerLevel[i].max = EditorGUILayout.IntField (prop.healPerLevel[i].max, GUILayout.Width(40));
				GUILayout.Space (60);
				EditorGUILayout.EndHorizontal ();
			}
			GUILayout.Space (5);
		}

		//Affected Units.
		DrawAffectedUnitsPanel(propID, ref prop.targetCasterOverride, ref prop.affectsCaster, ref prop.affectsAllyPlayers, ref prop.affectsAllySummons, ref prop.affectsEnemyPlayers, ref prop.affectsEnemySummons);

		EditorGUILayout.EndVertical ();
	}

	private void DrawDisplacementProperty(DisplacementProperty prop, int index, bool crit)
	{
		int propID = crit ? critTotal + total : total;

		EditorGUILayout.BeginVertical ("box");
		GUILayout.Space(5);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Space(5);

		string type = ToUppercaseStart (prop.displacementType.ToString ());

		EditorGUILayout.LabelField ("Displace - " + type + ":", EditorStyles.boldLabel, GUILayout.MaxWidth(100));
		if (prop.displacementType == eDisplacementType.PUSH || prop.displacementType == eDisplacementType.PULL) {
			EditorGUILayout.LabelField (prop.distancePerLevel [showLevel - 1] + " cells", EditorStyles.boldLabel, GUILayout.MaxWidth (60));
			EditorGUILayout.LabelField ("at level " + showLevel, GUILayout.MinWidth (60));
		} else {
			EditorGUILayout.LabelField ("");
		}

		if (prop.displacementType == eDisplacementType.BLINK) {
			prop.targetCasterOverride = true;
		}

		if (editEffectIndex == propID) {
			if (GUILayout.Button ("Hide", EditorStyles.miniButtonLeft, GUILayout.Width(60))) {
				editEffectIndex = int.MaxValue;
			}
		} else {
			if (GUILayout.Button ("Show", EditorStyles.miniButtonLeft, GUILayout.Width(60))) {
				editEffectIndex = propID;
			}
		}

		if (GUILayout.Button ("Remove", EditorStyles.miniButtonRight, GUILayout.Width(60))) {
			if (!crit) {
				ability.displacementEffects.RemoveAt (index);
				total--;
			} else {
				ability.critDisplacementEffects.RemoveAt (index);
				critTotal--;
			}
			return;
		}
		GUILayout.Space(5);
		EditorGUILayout.EndHorizontal ();
		GUILayout.Space(2);

		if (editEffectIndex == propID) {

			GUILayout.Space(4);
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space(4);
			EditorGUILayout.LabelField ("Displacement Type:", GUILayout.Width(130));
			prop.displacementType = (eDisplacementType)EditorGUILayout.EnumPopup (prop.displacementType, GUILayout.Width(80), GUILayout.Height(15));
			EditorGUILayout.EndHorizontal ();
			GUILayout.Space(2);

			for (int i = 0; i < 5; i++) {
				if (prop.displacementType == eDisplacementType.PUSH || prop.displacementType == eDisplacementType.PULL) {
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Space (4);
					EditorGUILayout.LabelField ("Level " + (i + 1) + ":", GUILayout.Width (60));
					GUILayout.Label ("Distance:", GUILayout.MaxWidth (100));
					prop.distancePerLevel [i] = EditorGUILayout.IntField (prop.distancePerLevel [i], GUILayout.Width (40));
					EditorGUILayout.EndHorizontal ();
				}
			}
			GUILayout.Space (5);
		}

		//Affected Units.
		DrawAffectedUnitsPanel(propID, ref prop.targetCasterOverride, ref prop.affectsCaster, ref prop.affectsAllyPlayers, ref prop.affectsAllySummons, ref prop.affectsEnemyPlayers, ref prop.affectsEnemySummons);

		EditorGUILayout.EndVertical ();
	}

	private string ToUppercaseStart(string text)
	{
		string ret = text.ToLower ();
		char[] a = ret.ToCharArray();
		a[0] = char.ToUpper(a[0]);
		return new string (a);
	}

	void DrawBuffDebuffProperty(BuffDebuffProperty prop, int index, bool crit)
	{
		int propID = crit ? critTotal + total : total;

		EditorGUILayout.BeginVertical ("box");
		GUILayout.Space(5);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Space(5);

		DisplayBuffDebuffLabel(prop);

		if (editEffectIndex == propID) {
			if (GUILayout.Button ("Hide", EditorStyles.miniButtonLeft, GUILayout.Width(60))) {
				editEffectIndex = int.MaxValue;
			}
		} else {
			if (GUILayout.Button ("Show", EditorStyles.miniButtonLeft, GUILayout.Width(60))) {
				editEffectIndex = propID;
			}
		}

		if (GUILayout.Button ("Remove", EditorStyles.miniButtonRight, GUILayout.Width(60))) {
			if (!crit) {
				ability.buffDebuffEffects.RemoveAt (index);
				total--;
			} else {
				ability.critBuffDebuffEffects.RemoveAt (index);
				critTotal--;
			}
			return;
		}
		GUILayout.Space(5);
		EditorGUILayout.EndHorizontal ();
		GUILayout.Space(2);

		if (editEffectIndex == propID) {

			if ((prop.stat != eStat.MAX_AP && prop.stat != eStat.MAX_MP) || prop.isResistanceModifier) {
				prop.resistable = false;
			}

			GUILayout.Space(4);
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space(4);
			if (!prop.isResistanceModifier) {
				EditorGUILayout.LabelField ("Affected Stat:", GUILayout.Width (100));
				prop.stat = (eStat)EditorGUILayout.EnumPopup (prop.stat, GUILayout.Width (80), GUILayout.Height (15));
			} else {
				EditorGUILayout.LabelField ("Resitance:", GUILayout.Width (100));
				prop.resistance = (eElement)EditorGUILayout.EnumPopup (prop.resistance, GUILayout.Width (80), GUILayout.Height (15));
			}
			GUILayout.Space (30);
			prop.resistable = EditorGUILayout.ToggleLeft ("Reistable", prop.resistable, GUILayout.MaxWidth(80));
			prop.isBuff = EditorGUILayout.ToggleLeft ("Buff", prop.isBuff, GUILayout.MaxWidth(80));
			prop.isResistanceModifier = EditorGUILayout.ToggleLeft ("Resist Mod", prop.isResistanceModifier, GUILayout.MaxWidth(80));
			EditorGUILayout.EndHorizontal ();
			GUILayout.Space(2);

			for (int i = 0; i < 5; i++) {
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space(4);
				EditorGUILayout.LabelField ("Level " + (i + 1) + ":", GUILayout.Width(60));
				GUILayout.Label ("Min Value:");
				prop.valuePerLevel[i].min = EditorGUILayout.IntField (prop.valuePerLevel[i].min, GUILayout.Width(40));
				GUILayout.Space (40);
				GUILayout.Label ("Max Value:");
				prop.valuePerLevel[i].max = EditorGUILayout.IntField (prop.valuePerLevel[i].max, GUILayout.Width(40));
				GUILayout.Space (40);
				GUILayout.Label ("Duration:");
				prop.durationPerLevel[i] = EditorGUILayout.IntField (prop.durationPerLevel[i], GUILayout.Width(40));
				GUILayout.Space (40);
				EditorGUILayout.EndHorizontal ();
			}
			GUILayout.Space (5);
		}

		//Affected Units.
		DrawAffectedUnitsPanel(propID, ref prop.targetCasterOverride, ref prop.affectsCaster, ref prop.affectsAllyPlayers, ref prop.affectsAllySummons, ref prop.affectsEnemyPlayers, ref prop.affectsEnemySummons);

		EditorGUILayout.EndVertical ();
	}

	private void DisplayBuffDebuffLabel(BuffDebuffProperty prop)
	{
		string buffType = prop.isBuff ? "Buff:" : "Debuff:";
		EditorGUILayout.LabelField (buffType, EditorStyles.boldLabel, GUILayout.MaxWidth(100));

		string stat = "Invalid Parameter";
		bool valid = false;
		GUIStyle fontColour = new GUIStyle ();
		fontColour.fontStyle = FontStyle.Bold;

		if (!prop.isResistanceModifier) {
			switch (prop.stat) {
			case eStat.MAX_AP:
				stat = " AP";
				valid = true;
				break;

			case eStat.MAX_MP:
				stat = " MP";
				valid = true;
				break;

			case eStat.STR:
				stat = " Strength";
				valid = true;
				break;

			case eStat.INT:
				stat = " Intelligence";
				valid = true;
				break;

			case eStat.AGI:
				stat = " Agility";
				valid = true;
				break;

			case eStat.RES:
				stat = " Resolve";
				valid = true;
				break;
			}
		} else {
			stat = ToUppercaseStart(prop.resistance.ToString ().ToLower ());

			switch (prop.resistance) {
			case eElement.FIRE:
				fontColour.normal.textColor = fire;
				break;

			case eElement.EARTH:
				fontColour.normal.textColor = earth;
				break;

			case eElement.AIR:
				fontColour.normal.textColor = air;
				break;

			case eElement.WATER:
				fontColour.normal.textColor = water;
				break;
			}
			valid = true;
			stat += " Resistance";
		}

		if (valid) {
			if (prop.valuePerLevel [showLevel - 1].min != prop.valuePerLevel [showLevel - 1].max) {
				EditorGUILayout.LabelField (prop.valuePerLevel [showLevel - 1].min + " - " + prop.valuePerLevel [showLevel - 1].max, EditorStyles.boldLabel, GUILayout.MaxWidth (50));
			} else {
				EditorGUILayout.LabelField (prop.valuePerLevel [showLevel - 1].min.ToString(), EditorStyles.boldLabel, GUILayout.MaxWidth (30));
			}
			EditorGUILayout.LabelField (stat, fontColour, GUILayout.MaxWidth (100));
			EditorGUILayout.LabelField ("at level " + showLevel, GUILayout.MinWidth (60));
		} else {
			fontColour.normal.textColor = Color.red;
			fontColour.fontStyle = FontStyle.Normal;
			EditorGUILayout.LabelField("Invalid Parameter", fontColour);
		}
	}

	void DrawCritEffectsPanel()
	{
		//Effects label and toggle.
		EditorGUILayout.BeginHorizontal();
		showCritEffects = EditorGUILayout.ToggleLeft ("Crit Effects:", showCritEffects, EditorStyles.boldLabel);
		EditorGUILayout.LabelField ("Crit Effects Total: " + CountEffects(true), alignRight);

		EditorGUILayout.EndHorizontal ();

		//Effects box.
		if (showCritEffects) {
			EditorGUILayout.BeginVertical ("box");

			//Add effect controls.
			GUILayout.Space(10);
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space(5);

			propertyType = (PropertyType)EditorGUILayout.EnumPopup (propertyType, GUILayout.MaxWidth(120));

			if (GUILayout.Button ("Add", EditorStyles.miniButtonRight, GUILayout.Width(50))) {
				AddProperty (true);
			}

			if (GUILayout.Button ("Duplicate", EditorStyles.miniButtonRight, GUILayout.Width(60))) {
				DuplicateEffects ();
			}

			GUILayout.Space(100);
			EditorGUILayout.LabelField ("Preview Level:", GUILayout.Width(100));
			showLevel = EditorGUILayout.IntSlider (showLevel, 1, 5, GUILayout.Width(120));
			GUILayout.Space(5);

			EditorGUILayout.EndHorizontal ();
			GUILayout.Space(10);

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space(5);
			EditorGUILayout.LabelField ("Base Crit Chance:", GUILayout.MaxWidth(120));
			ability.baseCritChance = EditorGUILayout.IntSlider (ability.baseCritChance, 0, 100, GUILayout.MaxWidth(120));
			EditorGUILayout.EndHorizontal ();

			if (ability.baseCritChance == 0) {
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space(5);
				EditorGUILayout.LabelField ("Cannot Crit");
				EditorGUILayout.EndHorizontal ();
				GUILayout.Space(5);
			} else if (critTotal < 1) {
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space(5);
				EditorGUILayout.LabelField ("No Crit Effects Added", highlightText);
				EditorGUILayout.EndHorizontal ();
				GUILayout.Space(5);
			}

			critTotal = 0;

			if (ability.baseCritChance > 0) {
				for (int i = 0; i < ability.critDamageEffects.Count; i++) {
					DrawDamageProperty (ability.critDamageEffects [i], i, true);
					critTotal++;
				}

				for (int i = 0; i < ability.critHealEffects.Count; i++) {
					DrawHealProperty (ability.critHealEffects [i], i, true);
					critTotal++;
				}

				for (int i = 0; i < ability.critDisplacementEffects.Count; i++) {
					DrawDisplacementProperty (ability.critDisplacementEffects [i], i, true);
					critTotal++;
				}

				for (int i = 0; i < ability.critBuffDebuffEffects.Count; i++) {
					DrawBuffDebuffProperty (ability.critBuffDebuffEffects [i], i, true);
					critTotal++;
				}
			}

			GUILayout.EndVertical ();
		}
	}

	private void DrawAffectedUnitsPanel(int id, ref bool targetCaster, ref bool affectCaster, ref bool allies, ref bool allySummon, ref bool enemies, ref bool enemySummon)
	{
		GUILayout.Space (5);
		EditorGUILayout.BeginHorizontal();
		GUILayout.Space (4);
		EditorGUILayout.LabelField("Affected Units:", GUILayout.MaxWidth(100));
		string affectedUnitsLabel = "";

		if (affectCaster) {
			affectedUnitsLabel += "Caster    ";
		}
		if (allies) {
			affectedUnitsLabel += "Ally Players    ";
		}
		if (allySummon) {
			affectedUnitsLabel += "Ally Summons    ";
		}
		if (enemies) {
			affectedUnitsLabel += "Enemy Players    ";
		}
		if (enemySummon) {
			affectedUnitsLabel += "Enemy Summons";
		}

		if (affectCaster && allies && allySummon && enemies && enemySummon) {
			affectedUnitsLabel = " Affects all";
		}

		if (targetCaster) {
			GUIStyle fontColour = new GUIStyle ();
			fontColour.normal.textColor = Color.red;
			EditorGUILayout.LabelField("Target Caster Override Enabled", fontColour);
		} else {
			EditorGUILayout.LabelField(affectedUnitsLabel);
		}
		EditorGUILayout.EndHorizontal ();
		GUILayout.Space (5);

		if (editEffectIndex == id) {
			GUILayout.Space (5);
			EditorGUILayout.BeginHorizontal ();
			targetCaster = EditorGUILayout.ToggleLeft ("Target Caster", targetCaster, GUILayout.MaxWidth(110));
			affectCaster = EditorGUILayout.ToggleLeft ("Affects Caster", affectCaster, GUILayout.MaxWidth(210));
			if (GUILayout.Button("Affects All", EditorStyles.miniButton, GUILayout.MaxWidth(70))) {
				affectCaster = true;
				allies = true;
				allySummon = true;
				enemies = true;
				enemySummon = true;
			}
			GUILayout.Space (10);
			EditorGUILayout.EndHorizontal ();

			GUILayout.Space (5);
			EditorGUILayout.BeginHorizontal ();
			allies = EditorGUILayout.ToggleLeft ("Ally Players", allies, GUILayout.MaxWidth(110));
			allySummon = EditorGUILayout.ToggleLeft ("Ally Summons", allySummon, GUILayout.MaxWidth(110));
			enemies = EditorGUILayout.ToggleLeft ("Enemy Players", enemies, GUILayout.MaxWidth(110));
			enemySummon = EditorGUILayout.ToggleLeft ("Enemy Summons", enemySummon, GUILayout.MaxWidth(110));
			EditorGUILayout.EndHorizontal ();
		}
	}

	private void DrawGenericInfo()
	{
		GUILayout.BeginHorizontal ();
		showGenericInfo = EditorGUILayout.ToggleLeft ("Generic Information:", showGenericInfo, EditorStyles.boldLabel);
		GUILayout.EndHorizontal ();

		if (showGenericInfo) {

			EditorGUILayout.BeginVertical ("box");
			GUILayout.Space (5);

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Space (5);
			EditorGUILayout.BeginVertical ();
			EditorGUILayout.LabelField("Ability Name:", GUILayout.MaxWidth(160));
			ability.abilityName = GUILayout.TextField (ability.abilityName, GUILayout.MaxWidth(280));
			EditorGUILayout.EndVertical ();

			GUILayout.Space (100);

			EditorGUILayout.BeginVertical ();
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField("Ability Icon:", GUILayout.MaxWidth(80));
//			ability.icon = (Sprite)EditorGUILayout.ObjectField(ability.icon, typeof(Sprite), GUILayout.MinHeight(64), GUILayout.MaxWidth(64));
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();

			GUILayout.Space (5);
			EditorGUILayout.LabelField("Ability Description:");
			ability.description = EditorGUILayout.TextArea (ability.description, GUILayout.MinHeight(50));
			GUILayout.Space (5);
			EditorGUILayout.EndVertical ();
		}
	}

	private void DuplicateEffects()
	{
		ability.critDamageEffects.Clear ();
		foreach (DamageProperty p in ability.damageEffects) {
			CopyDamageProperty (p);
		}

		ability.critHealEffects.Clear ();
		foreach (HealProperty p in ability.healEffects) {
			CopyHealProperty (p);
		}

		ability.critDisplacementEffects.Clear ();
		foreach (DisplacementProperty p in ability.displacementEffects) {
			CopyDisplaceProperty (p);
		}

		ability.critBuffDebuffEffects.Clear ();
		foreach (BuffDebuffProperty p in ability.buffDebuffEffects) {
			CopyBuffDebuffProperty (p);
		}
	}

	private void CopyDamageProperty(DamageProperty prop)
	{
		eElement element = new eElement ();
		element = prop.element;

		eStat scaling = new eStat ();
		scaling = prop.scaleFactor;

		MinMaxValue[] values = new MinMaxValue[5];

		for (int i = 0; i < 5; i++) {
			MinMaxValue val = new MinMaxValue (prop.damagePerLevel [i].min, prop.damagePerLevel [i].max);
			values [i] = val;
		}

		bool targetCaster = prop.targetCasterOverride;
		bool affectCaster = prop.affectsCaster;
		bool allies = prop.affectsAllyPlayers;
		bool allySummon = prop.affectsAllySummons;
		bool enemies = prop.affectsEnemyPlayers;
		bool enemySummon = prop.affectsAllySummons;

		DamageProperty newProp = new DamageProperty ();

		newProp.element = element;
		newProp.scaleFactor = scaling;
		newProp.damagePerLevel = values;
		newProp.targetCasterOverride = targetCaster;
		newProp.affectsCaster = affectCaster;
		newProp.affectsAllyPlayers = allies;
		newProp.affectsAllySummons = allySummon;
		newProp.affectsEnemyPlayers = enemies;
		newProp.affectsEnemySummons = enemySummon;

		ability.critDamageEffects.Add (newProp);
	}

	private void CopyHealProperty(HealProperty prop)
	{
		eStat scaling = new eStat ();
		scaling = prop.scaleFactor;

		MinMaxValue[] values = new MinMaxValue[5];

		for (int i = 0; i < 5; i++) {
			MinMaxValue val = new MinMaxValue (prop.healPerLevel [i].min, prop.healPerLevel [i].max);
			values [i] = val;
		}

		bool targetCaster = prop.targetCasterOverride;
		bool affectCaster = prop.affectsCaster;
		bool allies = prop.affectsAllyPlayers;
		bool allySummon = prop.affectsAllySummons;
		bool enemies = prop.affectsEnemyPlayers;
		bool enemySummon = prop.affectsAllySummons;

		HealProperty newProp = new HealProperty ();

		newProp.scaleFactor = scaling;
		newProp.healPerLevel = values;
		newProp.targetCasterOverride = targetCaster;
		newProp.affectsCaster = affectCaster;
		newProp.affectsAllyPlayers = allies;
		newProp.affectsAllySummons = allySummon;
		newProp.affectsEnemyPlayers = enemies;
		newProp.affectsEnemySummons = enemySummon;

		ability.critHealEffects.Add (newProp);
	}

	private void CopyDisplaceProperty(DisplacementProperty prop)
	{
		eDisplacementType type = new eDisplacementType ();
		type = prop.displacementType;

		int[] values = new int[5];

		for (int i = 0; i < 5; i++) {
			int val = prop.distancePerLevel [i];
			values [i] = val;
		}

		bool targetCaster = prop.targetCasterOverride;
		bool affectCaster = prop.affectsCaster;
		bool allies = prop.affectsAllyPlayers;
		bool allySummon = prop.affectsAllySummons;
		bool enemies = prop.affectsEnemyPlayers;
		bool enemySummon = prop.affectsAllySummons;

		DisplacementProperty newProp = new DisplacementProperty ();

		newProp.displacementType = type;
		newProp.distancePerLevel = values;
		newProp.targetCasterOverride = targetCaster;
		newProp.affectsCaster = affectCaster;
		newProp.affectsAllyPlayers = allies;
		newProp.affectsAllySummons = allySummon;
		newProp.affectsEnemyPlayers = enemies;
		newProp.affectsEnemySummons = enemySummon;

		ability.critDisplacementEffects.Add (newProp);
	}

	private void CopyBuffDebuffProperty(BuffDebuffProperty prop)
	{
		eStat stat = new eStat ();
		stat = prop.stat;

		eElement res = new eElement ();
		res = prop.resistance;

		MinMaxValue[] values = new MinMaxValue[5];

		int[] durations = new int[5];

		for (int i = 0; i < 5; i++) {
			MinMaxValue val = new MinMaxValue (prop.valuePerLevel [i].min, prop.valuePerLevel [i].max);
			values [i] = val;
		}

		for (int i = 0; i < 5; i++) {
			int val = prop.durationPerLevel [i];
			durations [i] = val;
		}

		bool isResist = prop.isResistanceModifier;
		bool resistable = prop.resistable;
		bool isBuff = prop.isBuff;

		bool targetCaster = prop.targetCasterOverride;
		bool affectCaster = prop.affectsCaster;
		bool allies = prop.affectsAllyPlayers;
		bool allySummon = prop.affectsAllySummons;
		bool enemies = prop.affectsEnemyPlayers;
		bool enemySummon = prop.affectsAllySummons;

		BuffDebuffProperty newProp = new BuffDebuffProperty ();

		newProp.isResistanceModifier = isResist;
		newProp.isBuff = isBuff;
		newProp.resistable = resistable;

		newProp.resistance = res;
		newProp.stat = stat;
		newProp.valuePerLevel = values;
		newProp.targetCasterOverride = targetCaster;
		newProp.affectsCaster = affectCaster;
		newProp.affectsAllyPlayers = allies;
		newProp.affectsAllySummons = allySummon;
		newProp.affectsEnemyPlayers = enemies;
		newProp.affectsEnemySummons = enemySummon;

		ability.critBuffDebuffEffects.Add (newProp);
	}

	private void DrawUsagePanel()
	{
		GUILayout.BeginHorizontal ();
		showUsage = EditorGUILayout.ToggleLeft ("Usage:", showUsage, EditorStyles.boldLabel);
		GUILayout.EndHorizontal ();

		if (showUsage) {

			GUILayout.BeginVertical ("box");
			GUILayout.Space (5);

			for (int i = 0; i < 5; i++) {
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space(4);
				EditorGUILayout.LabelField ("Level " + (i + 1) + ":", GUILayout.Width(60));

				GUILayout.Label ("AP Cost:");
				ability.apCostPerLvl[i] = EditorGUILayout.IntField (ability.apCostPerLvl[i], GUILayout.Width(30));

				GUILayout.Label ("Cooldown:");
				ability.cooldownPerLvl[i] = EditorGUILayout.IntField (ability.cooldownPerLvl[i], GUILayout.Width(30));

				GUILayout.Label ("Casts / Target:");
				ability.castsPerTargetPerLvl[i] = EditorGUILayout.IntField (ability.castsPerTargetPerLvl[i], GUILayout.Width(30));

				GUILayout.Label ("Casts / Turn:");
				ability.castsPerTurnPerLvl[i] = EditorGUILayout.IntField (ability.castsPerTurnPerLvl[i], GUILayout.Width(30));

				EditorGUILayout.EndHorizontal ();
			}
			GUILayout.Space (5);
			GUILayout.EndVertical ();
		}
	}

	private void DrawRangePanel()
	{
		GUILayout.BeginHorizontal ();
		showRange = EditorGUILayout.ToggleLeft ("Range:", showRange, EditorStyles.boldLabel);
		GUILayout.EndHorizontal ();

		if (showRange) {

			GUILayout.BeginVertical ("box");
			GUILayout.Space (5);

			GUILayout.BeginHorizontal ();
			ability.adjustableRange = EditorGUILayout.ToggleLeft ("Adjustable Range", ability.adjustableRange, GUILayout.MaxWidth(120));

			GUILayout.Space (20);

			ability.requiresFreeCell = EditorGUILayout.ToggleLeft ("Requires Free Cell", ability.requiresFreeCell, GUILayout.MaxWidth(120));

			ability.requiresTarget = EditorGUILayout.ToggleLeft ("Requires Target", ability.requiresTarget, GUILayout.MaxWidth(120));
			GUILayout.Space (5);

			GUILayout.EndHorizontal ();
			GUILayout.Space (10);

			GUILayout.BeginHorizontal ();
		
			EditorGUILayout.LabelField ("Area of Effect Type:", GUILayout.Width (140));
			ability.areaType = (eAreaType)EditorGUILayout.EnumPopup (ability.areaType, GUILayout.Width (80), GUILayout.Height (15));

			GUILayout.Space (20);


			GUILayout.Label ("No LoS Unlock lvl:");
			ability.noLoSFromLvl = EditorGUILayout.IntField (ability.noLoSFromLvl, GUILayout.Width(30));
			GUILayout.Space (20);

			GUILayout.Label ("Non Linear Unlock lvl:");
			ability.nonLinearFromLvl = EditorGUILayout.IntField (ability.nonLinearFromLvl, GUILayout.Width(30));
			GUILayout.Space (20);
			GUILayout.EndHorizontal ();

			GUILayout.Space (5);

			for (int i = 0; i < 5; i++) {
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Space(4);
				EditorGUILayout.LabelField ("Level " + (i + 1) + ":", GUILayout.Width(60));

				GUILayout.Label ("Min Range:");
				ability.minRangePerLvl[i] = EditorGUILayout.IntField (ability.minRangePerLvl[i], GUILayout.Width(30));
				GUILayout.Space (10);

				GUILayout.Label ("Max Range:");
				ability.maxRangePerLvl[i] = EditorGUILayout.IntField (ability.maxRangePerLvl[i], GUILayout.Width(30));
				GUILayout.Space (10);

				GUILayout.Label ("Area Size:");
				ability.areaSizePerLvl[i] = EditorGUILayout.IntField (ability.areaSizePerLvl[i], GUILayout.Width(30));
				GUILayout.Space (10);

				EditorGUILayout.EndHorizontal ();
			}
			GUILayout.Space (5);
			GUILayout.EndVertical ();
		}
	}

	private void DrawAnimationPanel()
	{
		GUILayout.BeginHorizontal ();
		showAnimation = EditorGUILayout.ToggleLeft ("Animation Effects:", showAnimation, EditorStyles.boldLabel);
		GUILayout.EndHorizontal ();

		if (showAnimation) {

		}
	}
}