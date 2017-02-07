using UnityEngine;
using System.Collections;

public class Palm : MonoBehaviour {

	public float moveSpeed = 10f;

	GameObject grid;
	GameObject gridContainer;

	// Use this for initialization
	void Start () {
	
		grid = GameObject.Find("LaserGrid");
		gridContainer = GameObject.Find("LaserGrid Container");

	}
	
	// Update is called once per frame
	void Update () {
	
		Vector3 pos = transform.localPosition;

		if (grid != null) {
			moveSpeed = grid.GetComponent<LaserGrid>().moveSpeed;
			pos.y =  5f + gridContainer.transform.localPosition.y;
		}
		else {
			grid = GameObject.Find("LaserGrid");
			gridContainer = GameObject.Find("LaserGrid Container");
		}

		pos.z -= moveSpeed * Time.deltaTime;

		transform.localPosition = pos;

		if (pos.z <= -1)
			Destroy(gameObject);

	}
}
