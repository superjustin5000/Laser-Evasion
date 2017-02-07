using UnityEngine;
using System.Collections;

public class BOSS : MonoBehaviour {

	GameState gs;

	bool alive = true;

	
	[Range (3, 80)]
	public int levelDebug = 3;
	int startingLevel = 1;

	public int levelDifference = 40;
	public float scaleExponentialBase = 1.4f;

	float calculatedLevel;

	[HideInInspector]
	public bool zooming;
	float scheduledZoomTime = 3;
	Vector3 baseScale;


	long hp;
	long curHp;
	float timer;
	float time = 5f;

	GameObject bossHealthMeter;
	GameObject bossTimeMeter;
	GameObject bossHealthMeterInner;
	GameObject bossTimeMeterInner;


	public AudioClip sfx_enter;
	public AudioClip sfx_woosh;
	public AudioClip sfx_hurt1;
	public AudioClip sfx_hurt2;
	public AudioClip sfx_hurt3;
	public AudioClip sfx_hurt4;
	public AudioClip sfx_hurt5;
	public AudioClip sfx_hover;
	public AudioClip sfx_exit;

	bool playHoverLoop = false;

	// Use this for initialization
	void Start () {
	
		gs = GameState.sharedGameState;
		
		int sortOrder = 500;
		foreach (Transform t in transform.GetComponentsInChildren<Transform>()) {
			
			Renderer r = t.GetComponent<Renderer>();
			if (r != null) {
				
				r.sortingLayerName = "Obstacles_Below_Arrow";
				r.sortingOrder = sortOrder;
				
				sortOrder += 1;
			}
			
		}
		
		if (gs.level != null) {

			bossHealthMeter = GameObject.Find("BossHealthMeter");
			bossTimeMeter = GameObject.Find("BossTimeMeter");
			
			bossHealthMeterInner = bossHealthMeter.transform.FindChild("BossHealthMeterInner").gameObject;
			bossTimeMeterInner = bossTimeMeter.transform.FindChild("BossTimeMeterInner").gameObject;
			
			
			levelDebug = gs.level.currentLevel;
		}
		if (levelDebug >= 80) levelDebug = 80;
		calculatedLevel = levelDebug - levelDifference;
		
		float s = Mathf.Pow (scaleExponentialBase, calculatedLevel);
		baseScale = new Vector3(s,s,s);
		
		//CREATE ();
		
	}



	/// <summary>
	/// Any time you swipe to make the boss take damage, until it's eventual death.
	/// </summary>
	/// <param name="damage">Damage.</param>
	public void TakeDamage(long damage) {
		//can only take damage if alive is true.
		if (alive && !zooming) { //also cannot take damage while zooming in or out.
			curHp -= damage;
			
			float healthPercent = (float)curHp / (float)hp;
			if (gs.level != null)
				bossHealthMeterInner.GetComponent<LineRenderer>().SetPosition(1, new Vector3(400 * healthPercent,0,0));


			//PLAY AN AUDIO CLIP TO SIGNIFY BOSS GETTING HURT. BUT NOT IF THE DAMAGE IS 0.
			if (damage > 0) {
				//play random hurt sfx
				AudioClip hurt = sfx_hurt1;

				int rand = Random.Range(1,101);
				if (rand < 20)
					hurt = sfx_hurt2;
				else if (rand < 40)
					hurt = sfx_hurt3;
				else if (rand < 60)
					hurt = sfx_hurt4;
				else if (rand < 80)
					hurt = sfx_hurt5;

				gs.ac.PlaySFX(hurt);
			}


			if (curHp <= 0) {
				alive = false; //set alive to false, so it can't take any more damage.
				DIE(); //call this function to remove the health tand timer bars and start the shrink, then create the gold tetra.
			}

			else { //if the boss didn't die from losing hp.. make it do something.
				if (damage > 0) { //also the damage has to be greater than 0 to actually show the hit with zoom.
					// do not make the boss scale if it's zooming.
					if (!zooming) {
						foreach(Transform t in transform.GetComponentInChildren<Transform>()) {
							StartCoroutine(ScaleBossChildGotHit(t));
						}
					}

				}
			}

		}
	}


	IEnumerator ScaleBossChildGotHit(Transform t) {

		//zooming = true;

		Vector3 startScale = t.localScale;
		Vector3 varScale = new Vector3(1,1,1);

		float maxScale = 1.5f;

		while(true) { /// ------------- loop to scale big
			varScale *= 1.05f;
			if (varScale.x > maxScale) {
				t.localScale = startScale * varScale.x;
				varScale = new Vector3(maxScale, maxScale, maxScale);
				break;
			}
			t.localScale = startScale * varScale.x;

			yield return null;
		}

		while(true) { //---------------- loop to return to normal scale.
			varScale *= .95f;
			if (varScale.x < 1f) {
				varScale = new Vector3(1,1,1);
				t.localScale = startScale * varScale.x;
				break;
			}
			t.localScale = startScale * varScale.x;

			yield return null;
		}

		//zooming = false;

	}


	/// <summary>
	/// Removes the health and timer bars from the screen.
	/// </summary>
	public void RemoveBars() {
		if (gs.level != null) {
			bossHealthMeter.GetComponent<LineRenderer>().enabled = false;
			bossHealthMeterInner.GetComponent<LineRenderer>().enabled = false;
			
			bossTimeMeter.GetComponent<LineRenderer>().enabled = false;
			bossTimeMeterInner.GetComponent<LineRenderer>().enabled = false;
		}
	}



	/// <summary>
	/// Shows the health and timer bar and starts the scaling in process.
	/// </summary>
	/// <param name="h">indicates starting health for current boss.</param>
	/// <param name="t">the amount of time given to kill the boss.</param>
	public void CREATE(long h, float t) {
		if (gs == null) {
			Start();
		}

		alive = true;


		curHp = h;
		hp = h;
		TakeDamage(0); //so the meter's get set to the right size.

		time = t;
		timer = 0;

		if (gs.level != null) {
			bossTimeMeterInner.GetComponent<LineRenderer>().SetPosition(1, new Vector3(400,0,0));
			bossHealthMeterInner.GetComponent<LineRenderer>().SetPosition(1, new Vector3(400,0,0));
		}
		
		StartCoroutine(ZoomIn());

	}


	
	/// <summary>
	/// Zooms in to the scale depending on the game level. The scale is based on an expnential function since each shape is scaled 20x more than the last.
	/// </summary>
	/// <returns>yield null coroutine.</returns>
	IEnumerator ZoomIn() {
		zooming = true;

		float zoomTime = scheduledZoomTime;
		float zoomLevel = -levelDifference;//since the first instance is at level 5.
		float zoomDistance = calculatedLevel - zoomLevel; 

		float zoomRate = zoomDistance / zoomTime;

		bool playedWoosh = false;

		gs.ac.PlaySFX(sfx_enter);

		while (true) {

			float s = Mathf.Pow (scaleExponentialBase, zoomLevel);
			baseScale = new Vector3(s,s,s);

			zoomLevel += zoomRate*Time.deltaTime;

			/// THE FOLLOWING IF STATEMENT IS SOLELY FOR FIGUREING OUT WHEN TO PLAY A WOOSH SOUND AS ITS ZOOMING IN.
			int calculatedZoomLevel = (int)zoomLevel + levelDifference;
			if (calculatedZoomLevel % 10 == 0 && calculatedZoomLevel > 0) { //every multiple of 5 that's not multiple of 10.
				if (!playedWoosh) { //this boolean is here bc many floats can round to this same level over a few frames.
					playedWoosh = true;
					gs.ac.PlaySFX(sfx_woosh, 2f);
				}
			}
			else { //when the level becomes not one of the levels where the woosh should play, reset the boolean for the next level that needs to play the sfx
				if (playedWoosh)
					playedWoosh = false; 
			}
			/////// END WOOSH IF STATEMENT..................


			if (zoomLevel >= calculatedLevel) {
				break;
			}
			
			yield return null;

		}


		zooming = false;

		//play hover loop.
		playHoverLoop = true;


		//show the health and timer bar after zooming in is done.
		if (gs.level != null) {
			bossHealthMeter.GetComponent<LineRenderer>().enabled = true;
			bossHealthMeterInner.GetComponent<LineRenderer>().enabled = true;
			
			bossTimeMeter.GetComponent<LineRenderer>().enabled = true;
			bossTimeMeterInner.GetComponent<LineRenderer>().enabled = true;
		}

		//maybe play a sound indicating you can start damaging the boss.

	}




	/// <summary>
	/// DYING .... ZOOMING OUT ... what happens before and after the zoom process.
	/// </summary>
	public void DIE() {
		playHoverLoop = false;

		gs.ac.PlaySFX(sfx_exit);

		//--tell the level controller that the boss has been killed.
		gs.level.BossDestroyed();

		StartCoroutine(ZoomOut());
	}

	/// <summary>
	/// What happens before the zooming out has started.
	/// </summary>
	void PRE_DESTROY() {

		RemoveBars();
		
	}

	/// <summary>
	/// What happens after the zooming out has completed.
	/// </summary>
	void POST_DESTROY() {
		// -------------------  CREATE  A GOLD TETRA AFTER DYING.
		GameObject gold = Resources.Load<GameObject>("Prefabs/_COLLECTABLES/_tetra_gold");
		GameObject g = Instantiate(gold) as GameObject;

		//-------- THIS BOSS IS NO LONGER NEEDED. EACH BOSS INSTANCE IS UNIQUE BASED ON THE GAME's CURRENT LEVEL.
		//Destroy(gameObject);
	}


	/// <summary>
	/// Zooms out to normal scale, then shrinks, then completes the death process.
	/// </summary>
	/// <returns>yield null enumerator.</returns>
	IEnumerator ZoomOut() {

		PRE_DESTROY();

		zooming = true;

		float zoomTime = scheduledZoomTime;
		float zoomLevel = calculatedLevel;
		float zoomDistance = calculatedLevel + levelDifference;

		float zoomRate = zoomDistance / zoomTime;

		while(true) {

			float s = Mathf.Pow(scaleExponentialBase, zoomLevel);
			baseScale = new Vector3(s,s,s);

			zoomLevel -= zoomRate * Time.deltaTime;

			if (zoomLevel <= (-levelDifference + startingLevel)) { /// +5 to account for the starting level being 5 when you first encounter one of these.
				break;
			}

			yield return null;
		}

		/*
		//AFTER ZOOMED OUT .. SRETCH THE X SCALE AND SHRINK THE Y SCALE SO IT LOOKS LIKE ITS SQUISHING.
		Vector3 startScale = transform.FindChild("Layer8_O").localScale;
		Vector3 scale = new Vector3(1,1,1);
		while(true) {

			scale.y *= 0.99f;

			transform.FindChild("Layer8_O").localScale = new Vector3(startScale.x, scale.y * startScale.y, startScale.z);

			if (scale.y <= 0.1f) break;

			yield return null;
		}
		*/

		POST_DESTROY();


	}






	
	// Update is called once per frame
	void Update () {

		transform.localScale = baseScale;

		if (gs.level != null) {
			levelDebug = gs.level.currentLevel;
			if (levelDebug >= 80) levelDebug = 80;
		}
		calculatedLevel = levelDebug - levelDifference;

		if (!zooming) {

			//if (playHoverLoop)
			//	gs.ac.PlaySFX(sfx_hover);
			if (gs.level != null) {


				/*This if statement is here incase you press pause during a boss,
				 * or if you die during a boss and the continue menu pops up, or you go to the menu.
				 * in all cases we don't want the boss timer to be going. because when it runs out
				 * the failedtokillbossintime() message gets sent, which can break the game during
				 * pause or waiting for continue, or during a game over screen.
				 * incorrectSwipe takes care of the continue menu as well.
				 */
				if (gs.level.ls != LevelController.levelState.paused && !gs.level.incorrectSwipe) {

					/*
					 * This section is for the main boss timer. when it runs out, you die
					 * It also updates the meter on the screen based on how much time is left.
					 */
					timer += Time.deltaTime;
					if (timer >= time) {
						gs.level.FailedToKillBossInTime();
					}
					else {
						float width = 1 - timer/time;
						if (width <= 0.01f) width = 0.01f;
						bossTimeMeterInner.GetComponent<LineRenderer>().SetPosition(1, new Vector3((400 * width),0,0));
					}

				}

			}

			/*
			 * Not sure if this is necessary anymore, but it makes the scale of the boss be 
			 * calculated by the calculated level of the game you're at.
			 * This way the boss gets bigger then higher level you are.
			 */
			float newS = Mathf.Pow(scaleExponentialBase, calculatedLevel); //some number to the power of the level.
			Vector3 newBaseScale = new Vector3(newS, newS, newS);

			baseScale = newBaseScale;

		}

	}
}
