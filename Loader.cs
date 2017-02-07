using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour {

	public float waitTime = 3f;
	float waitTimer = 0;
	bool doneWaiting = false;

	GameState gs;


	bool playedAudio = false;
	bool didTouchScreen = false;

	public AudioClip sfx_start;


	// Use this for initialization
	void Start () {

		gs = GameState.sharedGameState;


	}
	
	// Update is called once per frame
	void Update () {
	
		if (!doneWaiting) {
			waitTimer+=Time.deltaTime;

			if (waitTimer >= waitTime) {
				doneWaiting = true;
				StartCoroutine(gs.restartLevel());

			}
		}


		if (!didTouchScreen) {
			bool touched = false;
			#if UNITY_IOS
			touched = Input.touchCount > 0;
			#endif
			#if UNITY_EDITOR
			touched = Input.GetMouseButtonDown(0);
			#endif
			
			if (touched) {
				didTouchScreen = true; //stop polling for touch.
				
				//tell loader to stop waiting.
				GameObject.Find("Loader").GetComponent<Loader>().waitTime = 0;
			}
		}




	}
}
