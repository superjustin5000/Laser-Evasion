using UnityEngine;
using System.Collections;

public class ScanLines : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Renderer>().sortingLayerName="Slide";
		GetComponent<Renderer>().sortingOrder=51;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
