using UnityEngine;

[System.Serializable]
public struct MinMaxValue {
	public int min;
	public int max;
	public MinMaxValue(int min, int max) {
		this.min = min;
		this.max = max;
	}
}

