using UnityEngine;
using System.Collections;



/// <summary>
/// NOTE : DO NOT ATTACH TO THE OBJECT YOU WANT TO BLINK.. THIS IS A SEPARATE GAME OBJECt.
/// </summary>


public class Blink : MonoBehaviour {

	GameState gs;

	public GameObject blinkObject;

	public float blinksPerSec = 2;
	public bool isOff = false;

	float blinkTime;
	float blinkTimer;

	// Use this for initialization
	void Start () {
	
		if (blinkObject != null)
			if (isOff)
				blinkObject.SetActive(false);

	}


	public void BlinkOn() {
		blinkObject.SetActive(true);
		isOff = false;
	}
	public void BlinkOff() {
		blinkObject.SetActive(false);
	}
	public void TurnOff() {
		isOff = true;
	}
	
	// Update is called once per frame
	void Update () {
	
		if (blinkObject != null) {

			if (!isOff) {

				blinkTime = 1/blinksPerSec;

				blinkTimer+=Time.deltaTime;

				if (blinkTimer >= blinkTime) {
					blinkTimer = 0;

					if (blinkObject.activeSelf) {
						BlinkOff();
					}
					else {
						BlinkOn();
					}
				}

			}

		}

	}
}
