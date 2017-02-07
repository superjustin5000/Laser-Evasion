using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

	GameState gs;

	public bool makeShadows = true;
	public bool isShadow;

	public Color shadowColor = new Color(0,1,1,0.3f);

	float fadeCounter;

	// Use this for initialization
	void Start () {
		gs = GameState.sharedGameState;



	}
	
	// Update is called once per frame
	void Update () {
		
		if (gs.frameCount % 1 == 0) { //only update every nth frame.
			//Debug.Log("Arrow");
			if (!isShadow) {


				if (makeShadows) { ////whether or not the arrow should make shadows.
					//Debug.Log("Not Shadow");
					if (gs.level != null) {
						if (gs.level.isTouchingScreen) {
							//Debug.Log("touching screen");
							GameObject shadow = Instantiate(gameObject) as GameObject;
							shadow.GetComponent<Arrow>().isShadow = true;
							shadow.transform.SetParent(transform.parent);
							shadow.transform.localPosition = transform.localPosition;
							shadow.GetComponent<SpriteRenderer>().color = shadowColor;
							shadow.GetComponent<SpriteRenderer>().sortingOrder -= 1;
							foreach (Transform t in shadow.transform.GetComponentInChildren<Transform>()) {
								Destroy(t.gameObject);
							}

						}
					}
				}
			}
			else {

				///do what the shadow does.
				Color c = GetComponent<SpriteRenderer>().color;
				if (c.a <= 0) {
					Destroy(gameObject);
				}
				else {
					c.a -= 0.025f;
					GetComponent<SpriteRenderer>().color = c;
				}

			}
		}

	}

}
