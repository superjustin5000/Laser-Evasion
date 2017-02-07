using UnityEngine;
using System.Collections;

public class GrowShrink : MonoBehaviour {


	Vector3 initialScale;
	Vector3 scale;

	float timer;
	public float growShrinkTime = 0.5f;
	public float growToScaleX = 5.0f;
	public float growToScaleY = 5.0f;


	bool active = false;
	bool growing = true;

	// Use this for initialization
	void Start () {
	
		initialScale = transform.localScale;
		scale = initialScale;

	}


	public void Activate() {
		active = true;
		growing = true;
		float timePortion = (growShrinkTime/2) * Time.deltaTime;

		scale = transform.localScale;

		float x = growToScaleX * timePortion;
		float y = growToScaleY * timePortion;

		transform.localScale = new Vector3(scale.x + x, scale.y + y, scale.z);
	}
	// Update is called once per frame
	void Update () {
	

		if (active) {

			float timePortion = (growShrinkTime/2) * Time.deltaTime;
			float x = growToScaleX * timePortion;
			float y = growToScaleY * timePortion;
			timer += Time.deltaTime;

			if (growing) {

				scale = new Vector3(scale.x + x, scale.y + y, scale.z);

				if (timer >= growShrinkTime/2) {

					growing = false;

				}

			}


			else {

				scale = new Vector3(scale.x - x, scale.y - y, scale.z);

				if (timer >= growShrinkTime) {
					scale = initialScale;
					timer = 0;
					active = false;
				}

			}

			transform.localScale = scale;

		}

	}
}
