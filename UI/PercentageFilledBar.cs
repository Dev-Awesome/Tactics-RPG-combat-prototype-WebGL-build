using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PercentageFilledBar : MonoBehaviour {

	private float fillPercent = 0;
	private Image fill;

	void Awake()
	{
		fill = GetComponent<Image> ();
	}

	public void SetFillAmount(int current, int max)
	{
		fillPercent = (float)current / (float)max;
		fill.fillAmount = fillPercent;
	}
}
