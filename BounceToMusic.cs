using UnityEngine;
using System.Collections;

public class BounceToMusic : MonoBehaviour {

	GameState gs;

	public float maxBounceScale = 1.2f;
	public float minBounceScale = 0.8f;


	// Use this for initialization
	void Start () {
		gs = GameState.sharedGameState;
	}
	
	// Update is called once per frame
	void Update () {
	
		//Make the arrow bounce with the background music.
		float invertBounceFactor = 80;

		float newScale = 1 + (gs.ac.dbValue/invertBounceFactor);

		if (gs.ac.isMute)
			newScale = 1f;

		if (newScale > maxBounceScale) newScale = maxBounceScale;
		if (newScale < minBounceScale) newScale = minBounceScale;
		
		transform.localScale = new Vector3(newScale, newScale, newScale);


	}
}
