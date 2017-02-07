using UnityEngine;
using System.Collections;

public class ScaleRandom : MonoBehaviour {

	float baseScale;

	// Use this for initialization
	void Start () {
		baseScale = transform.localScale.x;
	}
	
	// Update is called once per frame
	void Update () {
	
		Vector3 scale = transform.localScale;

		scale.x = baseScale * Random.Range(0.1f, 2f);
		scale.y = baseScale * Random.Range(0.1f, 2f);
		scale.z = baseScale * Random.Range(0.1f, 2f);

		transform.localScale = scale;


	}
}
