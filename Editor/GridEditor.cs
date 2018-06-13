using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(GridManager))]
public class GridEditor : Editor {

	private int width;
	private int height;
	private Vector2 startPos;

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector ();

		GridManager grid = (GridManager)target;

		width = EditorGUILayout.IntField ("Width", width);
		height = EditorGUILayout.IntField ("Height", height);
		startPos = EditorGUILayout.Vector3Field ("Start Position", startPos);

		if (GUILayout.Button("Create Custom Grid")) {
			grid.InstantiateGrid (width, height, startPos);
		}

		if (GUILayout.Button("Fill Screen")) {
			Vector3 pos = Camera.main.ViewportToWorldPoint (new Vector3 (-0.5f, 0.5f, 0));
			Vector3 adjustPos = new Vector3 (pos.x, pos.y, 5);
			grid.InstantiateGrid (50, 50, adjustPos);
		}

		if (GUILayout.Button("Toggle Grid Indicators")) {
			grid.ToggleInspectorIndicators ();	
		}

		if (GUILayout.Button("Change Grid Outline Colour")) {
			grid.ChangeOutlineColour ();	
		}
	}
}
