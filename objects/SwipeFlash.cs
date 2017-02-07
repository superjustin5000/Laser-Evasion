using UnityEngine;
using System.Collections;

public class SwipeFlash : MonoBehaviour {

	bool start = false;

	// Use this for initialization
	void Start () {
		start = true;
	}
	
	// Update is called once per frame
	void Update () {
	
		if (start) {

			Color c = GetComponent<SpriteRenderer>().color;
			c.a -= 0.025f;
			GetComponent<SpriteRenderer>().color = c;

			if (c.a <= 0)
				GameObject.Destroy(gameObject);

		}

	}
}
