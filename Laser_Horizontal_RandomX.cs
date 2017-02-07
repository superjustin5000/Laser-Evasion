using UnityEngine;
using System.Collections;

public class Laser_Horizontal_RandomX : MonoBehaviour {

	// Use this for initialization
	void Start () {
		float x = Random.Range(-2.4f, 2.4f);
		Vector3 pos = transform.localPosition;
		pos.x = x;
		transform.localPosition = pos;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
