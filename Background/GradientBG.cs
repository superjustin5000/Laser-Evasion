using UnityEngine;
using System.Collections;

public class GradientBG : MonoBehaviour {
	//#pragma strict
	
	public Color startColor = Color.red;
	public Color endColor = Color.blue;

	#region
	[Header("MESH SORTING")]
	public string sortingLayerName;
	public int sortOrder;
	#endregion
	// Use this for initialization

	MeshRenderer rend;

	bool isFlashing = false;
	float flashTimer = 0;


	void Start () {
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Color[] colors = new Color[mesh.vertices.Length];
		colors[0] = startColor;
		colors[1] = endColor;
		colors[2] = startColor;
		colors[3] = endColor;
		mesh.colors = colors;

		rend = GetComponent<MeshRenderer>();
		//rend.sortingLayerName = "Slide";
		//rend.sortingOrder = 50;
	}

	public void SetColors(Color start, Color end) {

		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Color[] colors = new Color[mesh.vertices.Length];
		colors[0] = start;
		colors[1] = end;
		colors[2] = start;
		colors[3] = end;
		mesh.colors = colors;

		startColor = start;
		endColor = end;
	}
	public void SetColorsKeepOriginal(Color start, Color end) {
		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Color[] colors = new Color[mesh.vertices.Length];
		colors[0] = start;
		colors[1] = end;
		colors[2] = start;
		colors[3] = end;
		mesh.colors = colors;
	}


	public void SendRenderToBack() {
		rend.sortingOrder = 0;
	}



	public void Flash(Color c) {
		if (!isFlashing) {
			SetColorsKeepOriginal(c, c);
			isFlashing = true;
		}
	}


	public void FlashRed() {
		Flash (Color.red);
	}



	// Update is called once per frame
	void Update () {

		if (rend != null) {
			if (rend.sortingLayerName != sortingLayerName && sortingLayerName != null && sortingLayerName != "")
				rend.sortingLayerName = sortingLayerName;
			if (rend.sortingOrder != sortOrder && sortOrder != 0)
				rend.sortingOrder = sortOrder;

			//Debug.Log(gameObject + " sorting layer name = " + rend.sortingLayerName + " and order = " + rend.sortingOrder);
		}



		if (isFlashing) {
			flashTimer += Time.deltaTime;
			if (flashTimer >= .125f) {
				isFlashing = false;
				flashTimer = 0;
				SetColors(startColor, endColor); ///reset to original colors after flash is done.
			}
		}
	}
}
