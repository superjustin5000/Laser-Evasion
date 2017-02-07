using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]

public class _Effect_LineRenderer_Circle : MonoBehaviour {

	float maxScale = 5;
	float scaleVel = 7;
	float scaleAccel = 6;

	LineRenderer line;
	public Color lineColor = Color.white;

	// Use this for initialization
	void Awake() {
		line = GetComponent<LineRenderer>();
		SetColors(lineColor);
	}

	void Start () {
	}

	public void SetColors(Color c) {
		lineColor = c;
		line.SetColors(c, c);
	}

	// Update is called once per frame
	void Update () {
	
		Vector3 scale = transform.localScale;

		scale.x += scaleVel * Time.deltaTime;
		if (scale.x >= maxScale) scale.x = maxScale;

		scaleVel -= scaleAccel * Time.deltaTime;
		if (scaleVel < 0.05f) scaleVel = 0.05f;

		if (scaleVel == 0.05f) {
			lineColor.a -= 0.035f;
			line.SetColors(lineColor,lineColor);
		}

		if (lineColor.a <= 0) {
			Destroy(gameObject);
		}

		scale.y = scale.x;

		transform.localScale = scale;

	}
}
