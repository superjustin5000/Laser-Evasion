using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent (typeof (Shadow))]

public class TextShadowMove : MonoBehaviour {

	Shadow shad;

	public Vector2 center;
	public float distance = 10;

	float timer = 0;
	float time = 2*Mathf.PI;

	GameState gs;


	// Use this for initialization
	void Start () {
	
		gs = GameState.sharedGameState;

		shad = GetComponent<Shadow>();
		center = shad.effectDistance;

	}
	
	// Update is called once per frame
	void Update () {
	
		timer += Time.deltaTime*3;
		if (timer > time) {
			timer = 0;
		}

		if ((gs.frameCount % 5) == 0) {

			float angle = timer;

			float sin = Mathf.Sin(angle)*distance;
			float cos = Mathf.Cos(angle)*distance;

			Vector2 pos = shad.effectDistance;
			pos.x = center.x + cos;
			pos.y = center.y + sin;

			shad.effectDistance = pos;

		}

	}
}
