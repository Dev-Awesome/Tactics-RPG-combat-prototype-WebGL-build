using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleFloatingText : MonoBehaviour {

	public Color textColour;
	public Color positiveNumberColour;
	public Color negativeNumberColour;
	public Color apColour;
	public Color mpColour;
	public float duration = 1f;
	public float apMpDuration = .3f;
	public float apMpOffsetY = 1f;
	public float hpOffsetY = 0.1f;

	private Text hpText;
	private Text apText;
	private Text mpText;
	private Transform unitPos;

	void Start()
	{
		hpText = gameObject.transform.GetChild(0).GetComponent<Text> ();
		hpText.gameObject.SetActive (false);
		apText = gameObject.transform.GetChild(1).GetComponent<Text> ();
		apText.gameObject.SetActive (false);
		mpText = gameObject.transform.GetChild(2).GetComponent<Text> ();
		mpText.gameObject.SetActive (false);
	}

	public void InitFloatingText(Unit unit)
	{
		unitPos = unit.transform;
	}

	public void DisplayNumber(int val)
	{
		if (val != 0) {
			if (val > 0) {
				hpText.text = Mathf.Abs (val).ToString ();
				hpText.color = positiveNumberColour;
			} else {
				hpText.text = "- " + Mathf.Abs (val);
				hpText.color = negativeNumberColour;
			}
			transform.position = Camera.main.WorldToScreenPoint (new Vector3 (unitPos.position.x, unitPos.position.y + hpOffsetY, 0));
			//StartCoroutine (WaitAndDisable (hpText, duration));
			hpText.gameObject.SetActive(true);
		}
	}

	public void DisplayApChange(int val)
	{
		if (val != 0) {
			if (val < 0) {
				apText.text = "- " + Mathf.Abs (val) + " Ap";
				apText.color = apColour;
			} else if (val > 0) {
				apText.text = "+ " + Mathf.Abs (val) + " Ap";
				apText.color = apColour;
			}
			transform.position = Camera.main.WorldToScreenPoint (new Vector3 (unitPos.position.x, unitPos.position.y, 0));
			//StartCoroutine (WaitAndDisable (apText, apMpDuration));
			apText.gameObject.SetActive(true);
		}
	}

	public void DisplayMpChange(int val)
	{
		if (val != 0) {
			if (val < 0) {
				mpText.text = "- " + Mathf.Abs (val) + " Mp";
				mpText.color = mpColour;
			} else if (val > 0) {
				mpText.text = "+ " + Mathf.Abs (val) + " Mp";
				mpText.color = mpColour;
			}
			transform.position = Camera.main.WorldToScreenPoint (new Vector3 (unitPos.position.x, unitPos.position.y, 0));
			//StartCoroutine (WaitAndDisable (mpText, apMpDuration));
			mpText.gameObject.SetActive(true);
		}
	}

	private IEnumerator WaitAndDisable(Text txt, float delay)
	{
		txt.gameObject.SetActive (true);
		yield return new WaitForSeconds (delay);
		txt.gameObject.SetActive (false);
	}
}
