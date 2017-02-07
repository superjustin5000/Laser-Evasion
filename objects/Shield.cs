using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
		Transform soccerBall = transform.FindChild("_shieldModel");

		Renderer r = soccerBall.GetComponent<Renderer>();
		if (r != null) {
			r.sortingLayerName = "Flash";
		}


	}
	
	// Update is called once per frame
	void Update () {
		//transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
	}
}
