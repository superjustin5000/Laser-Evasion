using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class CoinsCollect : MonoBehaviour {


	float fadeTime = 2f;
	float stayTime = 3f;

	bool isFading = false;

	GameState gs;

	public AudioClip sfx_coin;

	// Use this for initialization
	void Start () {
		gs = GameState.sharedGameState;
	}


	public void ShowCoinCollect() {
		if (!isFading) {
			StartCoroutine(doEverything());
		}
	}

	IEnumerator doEverything() {

		isFading = true;

		float timer = 0;
		float fadeAmount = Time.deltaTime / fadeTime;

		GetComponent<Text>().text = ((gs.numCoins-1).ToString() + " TOKENS");

		///////------ FADE THE TEXT IN.
		while (true) {

			Color c = GetComponent<Text>().color;
			c.a +=fadeAmount;
			GetComponent<Text>().color = c;

			if (c.a >= 1) {
				c.a = 1;
				GetComponent<Text>().color = c;
				break;
			}


			yield return null;

		}

		bool addedCoin =false;
		while (true) {

			timer += Time.deltaTime;

			//increment halfway
			if (!addedCoin) {
				if (timer >= stayTime/2) {
					addedCoin = true;
					GetComponent<Text>().text = (gs.numCoins.ToString() + " TOKENS");
					GetComponent<GrowShrink>().Activate();
					gs.ac.PlaySFX(sfx_coin);
				}
			}

			if (timer >= stayTime)
				break;

			yield return null;

		}


		//// ---------- FADE THE TEXT BACK OUT.
		while (true) {
			Color c = GetComponent<Text>().color;
			c.a -=fadeAmount;
			GetComponent<Text>().color = c;
			
			if (c.a <= 0) {
				c.a = 0;
				GetComponent<Text>().color = c;
				break;
			}
			
			yield return null;
		}

		isFading = false;

	}


	// Update is called once per frame
	void Update () {
	
	}
}
