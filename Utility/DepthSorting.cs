using UnityEngine;
using System.Collections;
using System;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class DepthSorting : MonoBehaviour {

	private const int IsometricRangePerYUnit = 100;

	void Update ()
	{
		SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
		sRenderer.sortingOrder = -(int)(transform.position.y * IsometricRangePerYUnit);
	}
}