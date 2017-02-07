using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class BigMessage : MonoBehaviour {

	GameState gs;

	[HideInInspector]
	public bool isShowing = false;

	float speed = 0.5f;
	public AudioClip sfx_woosh;

	// Use this for initialization
	void Start () {
		gs = GameState.sharedGameState;

		gameObject.SetActive(false);


	}


	public void showWithMessage(string message) {
		if (!isShowing) {
			isShowing = true;

			gameObject.SetActive(true);



			StartCoroutine(showing());
		}
	}

	IEnumerator enter() {
		Transform t = gameObject.transform;

		Vector3 p = t.position;
		p.x = 12;
		transform.position = p;
		
		gs.ac.PlaySFX(sfx_woosh, 3);

		while(true) {
			p = t.position;
			p.x -= speed;
			if (p.x <= 3.5f) p.x = 3.5f;
			t.position = p;
			if (p.x <= 3.5f)
				break;
			yield return null;
		}
		StartCoroutine(showing ());
	}

	IEnumerator showing() {
		float timer = 0;
		float time = 2f;
		while(true) {
			timer += Time.deltaTime;
			if (timer >= time)break;
			yield return null;
		}
		//StartCoroutine(exit());
		hide();
	}

	IEnumerator exit() {
		Transform t = gameObject.transform;

		
		gs.ac.PlaySFX(sfx_woosh, 3);

		while(true) {
			Vector3 p = t.position;
			p.x -= speed;
			if (p.x <= -8.5f) p.x = -8.5f;
			t.position = p;
			if (p.x <= -8.5f)
				break;
			yield return null;
		}
		hide ();
	}

	void hide() {
		gameObject.SetActive(false);
		isShowing = false;
	}

	// Update is called once per frame
	void Update () {
	
	}
}
