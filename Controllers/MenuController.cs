//using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;




public class MenuController : MonoBehaviour {

	GameState gs;

	const int adBannerHeight = 100;


	Text highScore;
	Text scoreMult;
	float highScoreCheckTime = 1;
	float highScoreCheckTimer = 1;
	bool updatedHighScore = false;


	

	GameObject buttonSound;
	bool setSoundButton = false;

	[HideInInspector]
	public bool subMenuShowing = false;
	bool aboutMenu = false;
	bool storeMenu = false;

	Coroutine subMenuMovingIn;
	Coroutine subMenuMovingOut;




	public AudioClip sfx_buttonPress;
	public AudioClip sfx_wooshIn;
	public AudioClip sfx_wooshOut;
	public AudioClip sfx_success;



	// Use this for initialization
	void Start () {
		gs = GameState.sharedGameState;
		gs.menu = this;


		//instantiate social controller.
		gs.social = new SocialController();


		//set score.
		highScore = GameObject.Find("MenuHighScore").GetComponent<Text>();
		setHighScore();

		scoreMult = GameObject.Find("MenuScoreMult").GetComponent<Text>();
		scoreMult.text = (gs.baseMult + 1.0f).ToString("0.00");



		/// -------------------- sets the track name
		gs.ac.SetTrackName();

	}


	
	public void StartGame() {
		gs.level.StartGame();
	}
	
	




	//WHAT HAPPENS WHEN THE APPLICATION GOES TO THE BACKGROUND!!!!!!!!!!!!!
	void OnApplicationPause(bool pauseStatus) {

		if (pauseStatus) { ///goes to background.

			gs.social.UnAuthenticate();

		}

		else { ///comes back to foreground... apparently.

		}

	}




	public void showGameCenterScoresRequest() {
		gs.social.showGameCenterScoresRequest();
		gs.ac.PlaySFX(sfx_buttonPress);
	}

	public void showGameCenterAchievementsRequest() {
		gs.social.showGameCenterAchievementsRequest();
		gs.ac.PlaySFX(sfx_buttonPress);
	}




	public void toggleSound() {

		gs.ac.toggleMute();

		gs.ac.PlaySFX(sfx_buttonPress);

		setSoundButton = false;

	}


	public void nextTrack() {
		gs.ac.NextTrack();
		gs.ac.PlaySFX(sfx_buttonPress);
	}





	
	









	public void showAbout() {
		

		GameObject aboutView = GameObject.Find("AboutView");
		
		Vector3 pos = aboutView.transform.localPosition;
		
		if (subMenuMovingIn != null) StopCoroutine(subMenuMovingIn);
		subMenuMovingIn = StartCoroutine(MoveFromTo(aboutView, pos, new Vector3(pos.x, 0f - adBannerHeight/2, 0), 0.2f));
		
		subMenuShowing = true;
		aboutMenu = true;
		
		
		gs.ac.PlaySFX(sfx_wooshIn);
		gs.ac.PlaySFX(sfx_buttonPress);

		gs.LoadAdBanner();
		
	}





	public void showStore() {


		GameObject storeView = GameObject.Find("StoreView");
		
		Vector3 pos = storeView.transform.localPosition;
		
		if (subMenuMovingIn != null) StopCoroutine(subMenuMovingIn);
		subMenuMovingIn = StartCoroutine(MoveFromTo(storeView, pos, new Vector3(pos.x, 0 - adBannerHeight/2, 0), 0.2f));
		
		subMenuShowing = true;
		storeMenu = true;
		
		
		gs.ac.PlaySFX(sfx_wooshIn);
		gs.ac.PlaySFX(sfx_buttonPress);


		gs.store.OpenStore(); //makes the store do its thing when opening.

		gs.LoadAdBanner();

	}


	







	IEnumerator MoveFromTo(GameObject go, Vector3 pointA, Vector3 pointB, float time){
		float t = 0f;
		while (t < 1f){
			//Debug.Log("moving");
			t += Time.deltaTime / time; // sweeps from 0 to 1 in time seconds
			go.transform.localPosition = Vector3.Lerp(pointA, pointB, t); // set position proportional to t
			yield return 0; // leave the routine and return here in the next frame
		}
	}








	public void closeSubMenu() {
		if (subMenuShowing) {

			if (aboutMenu) {
				
				GameObject aboutView = GameObject.Find("AboutView");
				
				Vector3 pos = aboutView.transform.localPosition;
				
				if (subMenuMovingOut != null) StopCoroutine(subMenuMovingOut);
				subMenuMovingOut = StartCoroutine(MoveFromTo(aboutView, pos, new Vector3(pos.x, -1500f, 0), 0.2f));
				
			}

			else if (storeMenu) {

				GameObject storeView = GameObject.Find("StoreView");
				
				Vector3 pos = storeView.transform.localPosition;
				
				if (subMenuMovingOut != null) StopCoroutine(subMenuMovingOut);
				subMenuMovingOut = StartCoroutine(MoveFromTo(storeView, pos, new Vector3(pos.x, -1500, 0), 0.2f));

				//CLOSE THE STORE.
				gs.store.CloseStore();

			}

			subMenuShowing = false;
			aboutMenu = false;
			storeMenu = false;

			gs.ac.PlaySFX(sfx_wooshOut);


			gs.UnLoadAdBanner();

		}
	}













	void setHighScore() {
		if (gs.level != null) {
			long hs = gs.GetHighScoreForUser();
			string hsString = gs.level.formatScoreToString(hs);
			highScore.text = hsString;
		}
	}



	
	
	// Update is called once per frame
	void Update () {


	


		if (highScoreCheckTimer >= highScoreCheckTime) {

			highScoreCheckTimer = 0;

			
			//set score.
			setHighScore();

			//set mult
			scoreMult.text = (gs.baseMult + 1.0f).ToString("0.00");



			//tell the social controller to do everything it needs to.
			gs.social.Update();


		}
		else {
			highScoreCheckTimer+=Time.deltaTime;
		}







		if (!setSoundButton) {
			buttonSound = GameObject.Find("Button_Audio");
			if (buttonSound != null) {
				if (gs.ac.isMute) {
					buttonSound.GetComponent<Image>().sprite = Resources.Load<Sprite>("art/icons/Icon_AudioOff");
					setSoundButton = true;
				}
				else {
					buttonSound.GetComponent<Image>().sprite = Resources.Load<Sprite>("art/icons/Icon_AudioOn");
					setSoundButton = true;
				}
			}
		}



	
	}


	

}
