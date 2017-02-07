using UnityEngine;
using System.Collections;

public class Tetra_Maker : MonoBehaviour {

	public GameObject tetra;

	public bool start = false;

	// Use this for initialization
	void Start () {
		if (start)
			CreateTetra();
	}


	public void CreateTetra() {

		GameObject t = Instantiate(tetra) as GameObject;
		
		t.transform.SetParent(transform.parent);

		t.transform.localPosition = transform.localPosition;

		Destroy(gameObject);

	}

	public void KillSelf() {
		Destroy(gameObject);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
