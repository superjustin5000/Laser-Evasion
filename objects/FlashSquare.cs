using UnityEngine;
using System.Collections;

public class FlashSquare : MonoBehaviour {

	bool isFading = false;
	float targetScale = 5;
	float fadeScale = 3;
	float scaleVel = 8;
	// Use this for initialization
	void Start () {
		StartCoroutine(scale());
	}
	
	// Update is called once per frame
	void Update () {
	


	}


	public void SetColor(Color c) {
		SpriteRenderer s = GetComponent<SpriteRenderer>();
		Color c1 = c;
		c1.a = s.color.a;
		s.color = c1;
	}


	IEnumerator scale() {

		Vector3 scale = transform.localScale;

		while (scale.x < targetScale) {

			if (scale.x >= fadeScale) {
				if (!isFading) {
					isFading = true;
					StartCoroutine(fade());
				}
			}

			scale.x += scaleVel*Time.deltaTime;
			scale.y += scaleVel*Time.deltaTime;
			
			transform.localScale = scale;

			yield return null;
		}

		Destroy(gameObject);

	}

	IEnumerator fade() {

		Color c = GetComponent<SpriteRenderer>().color;
		while (c.a > 0) {
			c.a -= 0.025f;
			GetComponent<SpriteRenderer>().color = c;
			yield return null;
		}

	}
}
