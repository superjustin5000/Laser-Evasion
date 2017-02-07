using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	GameState gs;

	[Range(-750,750)]
	public float pixelsPerSecondX;
	[Range(-750,750)]
	public float pixelsPerSecondY;
	
	[Range(0,4)]
	public float moveOscPerX = 0;
	[Range(0,1334)]
	public float moveOscDistX = 0;
	public bool reverseXdir = false;
	[Range(0,4)]
	public float moveOscPerY = 0;
	[Range(0,1334)]
	public float moveOscDistY = 0;
	public bool reverseYdir = false;
	Vector3 startPos;

	[Range(1,10)]
	public float frameSkip = 1;


	float timer = 0;


	// Use this for initialization
	void Start () {
	
		gs = GameState.sharedGameState;

		if (reverseXdir)
			moveOscDistX *= -1;
		if (reverseYdir)
			moveOscDistY *= -1;
		
		startPos = transform.localPosition;


	}
	
	// Update is called once per frame
	void Update () {
	

		timer += Time.deltaTime;


		Vector3 pos = transform.localPosition;
		
		if (moveOscPerX == 0 && moveOscPerY == 0) {
			
			//normal movement.
			pos += new Vector3((pixelsPerSecondX*Time.deltaTime)*0.01f, (pixelsPerSecondY*Time.deltaTime)*0.01f); ///standard vector addition
			
		}
		
		else {

			if (gs.frameCount % frameSkip == 0) {

				//Debug.Log("do it");
				
				if (moveOscPerX > 0) {
					if (moveOscDistX != 0) {
						float phase = Mathf.Sin(timer / moveOscPerX);
						pos.x = startPos.x + (phase * moveOscDistX*0.01f);
					}
				}
				
				if (moveOscPerY > 0) {
					if (moveOscDistY != 0) {
						float phase = Mathf.Sin(timer / moveOscPerY);
						pos.y = startPos.y + (phase * moveOscDistY*0.01f);
					}
				}
				
			}
			
		}
		
		transform.localPosition = pos;


	}
}
