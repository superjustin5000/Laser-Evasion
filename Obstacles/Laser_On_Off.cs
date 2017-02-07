using UnityEngine;
using System.Collections;

public class Laser_On_Off : Obstacle {

	
	#region
	[Header("On/Off Settings")]
	[Range(0.2f, 3f)]
	public float onTime;
	[Range(0.2f, 5f)]
	public float offTime;
	[Range(0,3)]
	public float delayOnOffStart;
	public bool forceOffAfterDelay = false; //if true.. after the delay, it will be turned off reglardless of isON
	public bool forceOnAfterDelay = false; //same except for on.
	float delayTimer = 0;
	float timer = 0;
	#endregion
	#region
	public bool isOn = true;
	public bool alwaysOn = false;
	#endregion

	#region
	[Header("MovementSettings")]
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
	#endregion

	#region
	[Header("Rotation Settings")]
	[Range(0,1440)]
	public float degreesPerSec = 360;
	public bool clockWise = false;

	[Range(0,4)]
	public float rotOscPer = 0;
	[Range(0,360)]
	public float rotOscAngle = 0;
	#endregion

	#region
	[Header("Audio Files")]
	public bool playAudio = true;
	public AudioClip sfx_laserLoop;
	public AudioClip sfx_laserHit;
	#endregion


	float gameTimer = 0;


	void Start() {
		base.Start();


		//some safety checks.
		if (alwaysOn)
			isOn = true;

		if (!isOn) { ///turn the alpha off if off.
			TurnOff();
		}
		
		if (reverseXdir)
			moveOscDistX *= -1;
		if (reverseYdir)
			moveOscDistY *= -1;

		startPos = transform.localPosition;

	}


	override public void Hit() {
		gs.ac.PlaySFX(sfx_laserHit,4);
	}



	void TurnOff() {

		isOn = false;

		SpriteRenderer rend = GetComponent<SpriteRenderer>();
		Color c = rend.color;
		c.a = 0;
		rend.color = c;
		
		rend = transform.FindChild("LaserInner").GetComponent<SpriteRenderer>();
		c = rend.color;
		c.a = 0;
		rend.color = c;
		GetComponent<FadeInOut>().enabled = false;
		
		//disable collider.
		canCollide = false;
		
	}
	
	
	void TurnOn() {

		isOn = true;

		SpriteRenderer rend = GetComponent<SpriteRenderer>();
		Color c =rend.color;
		c.a = 1;
		rend.color = c;
		
		rend = transform.FindChild("LaserInner").GetComponent<SpriteRenderer>();
		c = rend.color;
		c.a = 1;
		rend.color = c;
		
		GetComponent<FadeInOut>().enabled = true;
		
		
		//enable collider..
		canCollide = true;
	}
	
	
	
	
	
	// Update is called once per frame
	void Update () {
	
		if (alive) { //dont do anything if not alive.


			gameTimer += Time.deltaTime;

			//----- O N   O F F   S E T T I N G S


			if (!alwaysOn) {

				if (delayTimer >= delayOnOffStart) {

					if (forceOnAfterDelay && !forceOffAfterDelay) {
						TurnOn();	
						forceOnAfterDelay = false;
					}
					else if (forceOffAfterDelay && !forceOnAfterDelay) {
						TurnOff();
						forceOffAfterDelay = false;
					}

					timer += Time.deltaTime;


					if (isOn) {

						if (timer >= onTime) {
							TurnOff();
							timer = 0;
						}
						else {
							//should not hit in the first x% of the timer and last x%.
							float timeSection = onTime*0.2f;
							if (timer <= timeSection)
								canCollide = false;
							else canCollide = true;

							if (playAudio)
								if (gs.frameCount%3==0)
									gs.ac.PlaySFX(sfx_laserLoop, 0.1f);
						}
					}
					else {

						if (timer >= offTime) {
							TurnOn();
							timer = 0;
						}
					} //end else if on.


				}


				else {

					delayTimer += Time.deltaTime;

				}


			} //end if !alwaysOn

			else {
				if (playAudio)
					if (gs.frameCount%3==0)
						gs.ac.PlaySFX(sfx_laserLoop, 0.1f);

			}



			///---- M O V E M E N T   S E T T I N G S
			/// 
			Vector3 pos = transform.localPosition;

			if (moveOscPerX == 0 && moveOscPerY == 0) {

				//normal movement.
				pos += new Vector3((pixelsPerSecondX*Time.deltaTime)*0.01f, (pixelsPerSecondY*Time.deltaTime)*0.01f); ///standard vector addition

			}

			else {

				if (gs.frameCount % 3 > 0) {

					if (moveOscPerX > 0) {
						if (moveOscDistX != 0) {
							float phase = Mathf.Sin(gameTimer / moveOscPerX);
							pos.x = startPos.x + (phase * moveOscDistX*0.01f);
						}
					}

					if (moveOscPerY > 0) {
						if (moveOscDistY != 0) {
							float phase = Mathf.Sin(gameTimer / moveOscPerY);
							pos.y = startPos.y + (phase * moveOscDistY*0.01f);
						}
					}

				}

			}

			transform.localPosition = pos;




			///---- R O T A T I O N   S E T T I N G S

			if (rotOscPer == 0) {
				if (degreesPerSec > 0) {
					if (gs.frameCount % 3 == 0) {
						float rotAmount = degreesPerSec * Time.deltaTime;
						if (clockWise)
							rotAmount = -rotAmount;
						float curRot = transform.localRotation.eulerAngles.z;
						transform.localRotation = Quaternion.Euler(new Vector3(0,0,curRot+rotAmount));
					}

				}
			}
			else {
				if (gs.frameCount % 3 == 0) {
					if (rotOscAngle > 0) {
						float phase = Mathf.Sin(gameTimer / rotOscPer);
						transform.localRotation = Quaternion.Euler( new Vector3(0, 0, phase * rotOscAngle));
					}
				}
			}



		}//end if alive..



	}//end update.....

}
