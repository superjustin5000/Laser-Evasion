using UnityEngine;
using System.Collections;

public class InfiniteBackground : MonoBehaviour {

	GameState gs;

	
	GameObject bg2;
	
	public int movementType = 0; //0 is left, 1 is right.
	public float switchPos = 0;
	float moveSpeed = 0.5f;
	float xAdjust = 0.01f; //bc of the blinking gap..

	

	SpriteRenderer renderer1;
	
	void Start () {

		renderer1 = GetComponent<SpriteRenderer>();

		Vector3 pos = transform.localPosition;
		pos.x += renderer1.bounds.size.x - xAdjust;
		bg2 = Instantiate(gameObject) as GameObject;
		bg2.transform.parent = transform.parent;
		bg2.transform.localPosition = pos;
		InfiniteBackground i = bg2.GetComponent<InfiniteBackground>();
		Destroy(i);



		gs = GameState.sharedGameState;
		
	}
	
	
	
	// Update is called once per frame
	void Update () {
		
		float z = transform.localPosition.z;
		if (z < 1) z = 1;

		
		float move = moveSpeed / z;
		float halfWidth = renderer1.bounds.size.x/2;
		
		Vector3 pos = transform.localPosition;
		Vector3 pos2 = bg2.transform.localPosition;
		
		
		if (movementType == 1) {
			pos.x -= move;
			pos2.x -= move;
			
			float right = (halfWidth) + pos.x;
			float right2 = (halfWidth) + pos2.x;
			
			if (right <= (0-switchPos)) {
				pos.x = right2 + (halfWidth) - xAdjust;
			}
			if (right2 <= (0-switchPos) ) {
				pos2.x = right + (halfWidth) - xAdjust;
			}
		}
		else if (movementType == 0) {
			pos.x += move;
			pos2.x += move;
			
			float left = pos.x - (halfWidth);
			float left2 = pos2.x - (halfWidth);
			
			if (left >= (gs.winWidth + switchPos)) {
				pos.x = left2 - (halfWidth) + xAdjust;
			}
			if (left2 >= (gs.winWidth + switchPos)) {
				pos2.x = left - (halfWidth) + xAdjust;
			}
		}


		transform.localPosition = pos;
		bg2.transform.localPosition = pos2;

		
	}
	
}
