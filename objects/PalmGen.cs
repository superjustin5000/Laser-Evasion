using UnityEngine;
using System.Collections;

public class PalmGen : MonoBehaviour {

	string palmPrefab = "Prefabs/Palm";

	float time = 1;
	float timer = 0;

	public bool isActive = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (isActive){
			timer += Time.deltaTime;

			if (timer >= time) {
				timer = 0;
				time = Random.Range(6f, 12.5f);
				GenPalm();
			}
		}

	}


	void GenPalm() {

		GameObject palm = Resources.Load<GameObject>(palmPrefab);

		GameObject p = Instantiate(palm) as GameObject;

		Vector3 randScale = Vector3.zero;
		randScale.x = randScale.y = randScale.z = Random.Range(0.8f, 1.2f);
		p.transform.localScale = new Vector3(randScale.x, randScale.y, randScale.z);

		//random rotation on y axis.
		float randYRot = Random.Range(0,361);
		p.transform.localRotation = Quaternion.Euler(new Vector3(0,randYRot,0));

		float randX = Random.Range(-20f, 23.75f);

		p.transform.localPosition = new Vector3(randX, 0, 100);

	}

}
