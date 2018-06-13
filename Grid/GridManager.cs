using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {

	[Header("Grid Display")]
	//Inspector properties.
	public GridCell gridCell;
	public Color gridOutlineColour;

	private bool markersEnabled = true;

	//Cell size and positioning.
	public float unitsPerPixel = 100f;
	public float pixelWidth = 128f;
	private float pixelHeight { get { return pixelWidth * 0.5f; } }

	private float offsetX { get { return (pixelWidth / unitsPerPixel); } }
	private float offsetY { get { return offsetX * 0.5f; } }

	public static Dictionary<GridPos, GridCell> gridCells = new Dictionary<GridPos, GridCell> ();

	private GameObject[] cells;
	private Transform gridHolder;

	void Awake()
	{
		GetActiveGrid ();
	}

	public void InstantiateGrid(int width, int height, Vector2 startPos)
	{
		gridHolder = new GameObject ("Tile Container").transform;

		for (int y = 0; y < width; y++) {
			for (int x = 0; x < height; x++) {

				float isoX = (x * offsetX / 2) + (y * offsetX / 2);
				float isoY = (y * offsetY / 2) - (x * offsetY / 2);

				PlaceCell (startPos, isoX, isoY, x, y);
			}
		}
	}

	private void PlaceCell(Vector2 startPos, float isoX, float isoY, int x, int y)
	{
		Vector2 pos = new Vector2 (startPos.x + isoX, startPos.y + isoY);
		GridCell c = Instantiate (gridCell).GetComponent<GridCell> ();
		c.GetComponent<Transform> ().SetParent (gridHolder);
		c.PlaceTile (new GridPos (x, y), pos);
		c.name = string.Format ("{0}, {1}", Mathf.Abs (x), Mathf.Abs (y));
	}

	public void GetActiveGrid ()
	{
		cells = GameObject.FindGameObjectsWithTag ("Grid Cell");
		gridCells.Clear ();

		for (int i = 0; i < cells.Length; i++) {
			GridCell c = cells [i].GetComponent<GridCell> ();
			GridPos pos = new GridPos (c.x, c.y);
			c.InitCell (pos);
			gridCells.Add (pos, c);
	//		if (c.cellType != GridCell.eCellType.DISABLED) {
				
	//		}
		}
	}

	public List<GridCell> GetPlayerStartPos()
	{
		List<GridCell> ret = new List<GridCell> ();
		foreach (GridCell cell in gridCells.Values) {
			if (cell.playerStart) {
				ret.Add (cell);
			}
		}
		return ret;
	}

	public List<GridCell> GetEnemyStartPos()
	{
		List<GridCell> ret = new List<GridCell> ();
		foreach (GridCell cell in gridCells.Values) {
			if (cell.enemyStart) {
				ret.Add (cell);
			}
		}
		return ret;
	}

	public void ToggleInspectorIndicators()
	{
		if (markersEnabled) {
			cells = GameObject.FindGameObjectsWithTag ("Grid Cell");
			for (int i = 0; i < cells.Length; i++) {
				cells [i].GetComponent<SpriteRenderer> ().enabled = false;
			}
			markersEnabled = false;
		} else {
			cells = GameObject.FindGameObjectsWithTag ("Grid Cell");
			for (int i = 0; i < cells.Length; i++) {
				cells [i].GetComponent<SpriteRenderer> ().enabled = true;
			}
			markersEnabled = true;
		}
	}

	public void ChangeOutlineColour()
	{
		cells = GameObject.FindGameObjectsWithTag ("Grid Cell");

		for (int i = 0; i < cells.Length; i++) {
			cells [i].transform.GetChild (0).GetComponent<SpriteRenderer> ().color = gridOutlineColour;
		}
	}

	public void ShowGridLines() {
		cells = GameObject.FindGameObjectsWithTag ("Grid Cell");

		for (int i = 0; i < cells.Length; i++) {
			cells [i].transform.GetChild (0).GetComponent<SpriteRenderer> ().enabled = true;
		}
	}

	public void HideGridLines() {
		cells = GameObject.FindGameObjectsWithTag ("Grid Cell");

		for (int i = 0; i < cells.Length; i++) {
			cells [i].transform.GetChild (0).GetComponent<SpriteRenderer> ().enabled = false;
		}
	}
}
