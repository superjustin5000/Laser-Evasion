using UnityEngine;
using System.Collections;

public class Tetra : MonoBehaviour {

	GameState gs;

	bool alive = true;
	bool canCollide = true;

	public bool killSelf = true;
	public bool soundEnabled = true;
	public bool isGold = false;
	public bool isGreen = false;

	float moveSpeed = 15f;
	// Use this for initialization

	public AudioClip sfx_pickup;
	public AudioClip sfx_enter;
	public AudioClip sfx_exit;
	public AudioClip sfx_transform;

	bool reachedGameField = false;

	GameObject square;
	float stayTimer;
	float stayTime = 5f;
	float flickerTime = 0.01f;
	float flickerTimer;
	bool squareDisabled = true;

	void Start () {
		gs = GameState.sharedGameState;

		canCollide = false;

		square = transform.FindChild("square").gameObject;
		square.SetActive(!squareDisabled);


		gameObject.layer = LayerMask.NameToLayer("3DCam");
		foreach (Transform t in transform.GetComponentsInChildren<Transform>()) {
			t.gameObject.layer = LayerMask.NameToLayer("3DCam");
		}


		int sortOrder = 501; //right above boss.
		foreach (Transform t in transform.GetComponentsInChildren<Transform>()) {
			
			Renderer r = t.GetComponent<Renderer>();
			if (r != null) {
				
				r.sortingLayerName = "Obstacles_Below_Arrow";
				r.sortingOrder = sortOrder;
				
				sortOrder += 1;
			}
			
		}



		if (isGreen) stayTime = 12f;

	}
	
	// Update is called once per frame
	void Update () {
	
		if (gs.level.ls != LevelController.levelState.paused) { //dont update if level is paused.

			//make it so the tetra's angle is always upright.
			Vector3 rot = transform.rotation.eulerAngles;

			if (rot.z != 0) {
				rot.z = 0;
				transform.rotation = Quaternion.Euler(rot);
			}
			//and their y scales should be upright.
			Vector3 scale = transform.localScale;
			if (scale.y < 0) {
				scale.y *= -1;
			}
			if (scale.x < 0) {
				scale.x *= -1;
			}
			transform.localScale = scale;


			if (!reachedGameField) {

				if (moveSpeed > 0) {

					Vector3 pos = transform.localPosition;
					pos.z -= moveSpeed * Time.deltaTime;

					if (pos.z <= 0) {
						pos.z = 0;
						ReachedGameField();
					}

					transform.localPosition = pos;

				}

			}

			else {
				if (killSelf) {

					stayTimer += Time.deltaTime;


					if (stayTimer >= stayTime - 1) {

						flickerTimer += Time.deltaTime;

						if (flickerTimer > flickerTime) {
							flickerTimer = 0;

							foreach(Transform t in square.transform.GetComponentsInChildren<Transform>()) {

								LineRenderer lr = t.GetComponent<LineRenderer>();

								if (lr != null) lr.enabled = !squareDisabled;
								squareDisabled = !squareDisabled;

							}

						}

					}
					if (stayTimer >= stayTime) {

						RemoveSelf();

					}

				}


			}

		}


	}




	void ReachedGameField() {

		if (soundEnabled) gs.ac.PlaySFX(sfx_enter);
		
		square.SetActive(true);

		gameObject.layer = LayerMask.NameToLayer("Obstacles");
		foreach (Transform t in transform.GetComponentsInChildren<Transform>()) {
			t.gameObject.layer = LayerMask.NameToLayer("Obstacles");
		}


		canCollide = true;

		moveSpeed = 0f;

		reachedGameField = true;


		StartCoroutine(MakeCircles());

	}


	void Pickup() {

		canCollide = false;

		GameObject f = Resources.Load<GameObject>("Prefabs/_Effect_FlashSquare");
		GameObject flash = Instantiate(f) as GameObject;
		
		flash.transform.SetParent(transform.parent);
		flash.transform.localPosition = transform.localPosition;

		



		if (!isGold) {
			gs.ac.PlaySFX(sfx_pickup); //doesn't matter if sound is enabled for this thing.


			//if (isGreen)
			//	gs.level.CollectedPowerUpItem();
			 
				gs.level.CollectedScoreMultItem();

			Destroy(gameObject);

		}

		else {

			gs.ac.PlaySFX(sfx_pickup, 2f, 0.5f);
			gs.ac.PlaySFX(sfx_transform, 0.5f);

			flash.GetComponent<SpriteRenderer>().color = new Color(1,1,0);

			StartCoroutine(CollectedGold());

		}

	}

	IEnumerator MakeCircles() {

		int circleCount = 0;
		GameObject f = Resources.Load<GameObject>("Prefabs/_Effect_LineRenderer_Circle");

		while(circleCount < 3) {

			GameObject flash = Instantiate(f) as GameObject;
			flash.transform.SetParent(transform.parent);
			flash.transform.localPosition = transform.localPosition;

			Color flashC = new Color(0,1,1, 0.5f);

			if (isGold)
				flashC = new Color(1,1,0, 0.5f);
			else if (isGreen)
				flashC = new Color(0,1,0, 0.5f);

			flash.GetComponent<_Effect_LineRenderer_Circle>().SetColors(flashC);

			circleCount ++;

			yield return new WaitForSeconds(0.1f);
		}

	}

	void RemoveSelf() {

		StartCoroutine(ScaleDownToDie());

	}
	IEnumerator ScaleDownToDie() {

		while(true) {
			Vector3 scale = transform.localScale;
			scale.x -= 0.1f;
			scale.y -= 0.1f;
			transform.localScale = scale;

			if (scale.x <= 0.1f)
				break;
			yield return null;
		}

		if (isGreen)
			GameObject.Find("_TetraGen").GetComponent<TetraGen>().generateGreen = true; //this one died so make a new one.

		if (soundEnabled) gs.ac.PlaySFX(sfx_exit);
		
		canCollide = false;
		
		Destroy(gameObject);

	}


	IEnumerator CollectedGold() {

		//tell level controller that we collected gold.
		gs.level.ActivateCollectGold();

		//move back to 3d layer.
		gameObject.layer = LayerMask.NameToLayer("3DCam");
		foreach (Transform t in transform.GetComponentsInChildren<Transform>()) {
			t.gameObject.layer = LayerMask.NameToLayer("3DCam");
		}


		//init the effect.
		Transform lasers = transform.FindChild("Lasers");
		foreach (Transform t in lasers.GetComponentInChildren<Transform>()) {
			LineRenderer r = t.GetComponent<LineRenderer>();
			if (r != null) {
				r.enabled = true;
				yield return new WaitForSeconds(0.05f);
			}

		}


		//increase velocity of spinning.
		float maxVel = 1400;
		while(true) {
			lasers.GetComponent<Rotate>().degreesPerSec += 30;

			if (lasers.GetComponent<Rotate>().degreesPerSec >= maxVel)
				break;

			yield return null;
		}

		//make lasers smaller until they disappear.
		float width=5;
		while(true) {
			width-=0.2f;

			foreach (Transform t in lasers.GetComponentInChildren<Transform>()) {
				LineRenderer r = t.GetComponent<LineRenderer>();
				if (r != null) {
					r.SetWidth(width, width);
				}
				
			}

			if (width <=0) break;
			yield return null;
		}


		Destroy(gameObject);
	}



	void OnCollisionEnter2D(Collision2D coll) {

		if (alive) {
			if (canCollide) {

				if (!gs.level.backgroundMoving) { ///don't pick up the item when background is moving, bc arrow coming in with next slide will touch them.
					Pickup();
				}
			
			}

		}


	}


	void OnCollisionStay2D(Collision2D coll) {
		if (alive) {
			if (canCollide) {
				OnCollisionEnter2D(coll);
			}
		}
	}



}
