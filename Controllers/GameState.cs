using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;



#if UNITY_IOS
//using ADBannerView = UnityEngine.iOS.ADBannerView;
#endif


public class GameState {
	
	private static GameState gs;

	[HideInInspector]
	public MainCamera mainCam;
	[HideInInspector]
	public short frameCount = 1;
	
	[HideInInspector]
	public float winWidth, winHeight;
	
	[HideInInspector]
	public bool isPaused = false;
	

	//GAME VARIABLES -------------


	public LevelController level;
	public MenuController menu;
	public SocialController social;
	public StoreController store;

	public AudioController ac;


	[HideInInspector]
	public bool inMenu = false;
	public bool isFirstTimePlaying = true;

	public bool isSoundEnabled = true;

	[HideInInspector]
	public bool soomlaInit = false;
	public bool soomlaBusy = false;


	#if UNITY_IOS || UNITY_EDITOR
	private UnityEngine.iOS.ADBannerView banner = null;
	#endif
	public bool adBannerShow = false;



	int maxHighScores = 20;

	public int definiteHighScore;
	public int definiteNumSwipes;



	/// <summary>
	/// //////// ---- BELOW ARE ALL VARIABLES TO BE SAVED.
	/// BE SURE TO ADD THEM TO THE SERIALIZED DATA CLASS AT THE BOTTOM TOO!!!!!
	/// </summary>


	public string username = "New Player";
	public List<HighScore> highScores = new List<HighScore>();
	public int numSwipes;
	public int numSwipesOneGame;
	public int highestLevel;
	public int playsBeforeVideoAd;
	public bool iap_NoAds;

	public int numCoins;

	//audio
	public int currentTrackNum;

	public bool[] tracksUnlocked = new bool[50];

	public float baseMult;


	//tutorials.
	public bool tut_1 = false;
	public bool tut_2 = false;
	public bool tut_3 = false;
	public bool tut_4 = false;
	public bool tut_5 = false;
	public bool tut_6 = false;
	public bool tut_7 = false;
	public bool tut_8 = false;
	public bool tut_9 = false;
	public bool tut_10 = false;
	//-------------------------------






	//------------ BACKGROUND COLOR CONTROLLER 

	//COLORS 
	public enum ColorStyles {
		normal, 
		sunset, 
		beach, 
		desert, 
		desertNight, 
		lava,    
		darkness, 
	};
	public ColorStyles cs; //static so it effects every instance the same.

	public void SetColorStyle(GameState.ColorStyles c) {
		gs.cs = c;
	}


	//------------- END BACKGROUND COLORS



	
	private GameState() {
		
		Debug.Log ("Game State init");

		//uncomment the following line to delete player prefs. 
		//don't forget to recomment, or else saving will have no effect the next time you start.
		//PlayerPrefs.DeleteAll(); //comment this line out after every use.
	

		//Screen.orientation = ScreenOrientation.Portrait;
		//init game manager... don't reference game objects here bc game manager will be created first.
		//print (winWidth);

		Application.targetFrameRate = 60; 


		Load(); ///start by loading values.


		if (username == null)
			username = "Player 1";


		if (highScores.Count < maxHighScores) { ////if the high score has less than 20 elements.. add 0's and save.
			for (int i=highScores.Count; i<maxHighScores; i++) {
				HighScore h = new HighScore();
				h.name = "--------";
				h.score = 0;
				highScores.Add(h);
			}
			Save();
		}




		#if UNITY_IOS || UNITY_EDITOR
		//LoadAdBanner();
		#endif


	}


	public static GameState sharedGameState {
		get {
			if (gs == null) {
				gs = new GameState();
			}

			//Debug.Log(gs.numDouble);
			return gs;
		}
	}
	
	




	//---------- I A D    S E C T I O N


	#if UNITY_IOS || UNITY_EDITOR
	void OnBannerClicked()
	{
		Debug.Log("Clicked!\n");
	}
	void OnBannerLoaded()
	{
		/*
		 * Prevents the ad from showing when it shouldn't
		 */
		if (!adBannerShow) { 
			UnLoadAdBanner();
			return;
		}

		if (!iap_NoAds) {
			Debug.Log("Loaded!\n");
			banner.visible = true;
		}
		
	}


	/// <summary>
	/// Removes the banner ad.
	/// If for some reason the ad is still loading. OnBannerLoaded should check if the ad should be there then delete itself.
	/// </summary>
	public void UnLoadAdBanner() {
		Debug.Log("AdbannerUnload");
		adBannerShow = false;
		if (banner != null) {
			banner.visible = false;
			banner = null;
			UnityEngine.iOS.ADBannerView.onBannerWasLoaded  -= OnBannerLoaded; // Very important that this is called
			UnityEngine.iOS.ADBannerView.onBannerWasClicked -= OnBannerClicked;
		}
	}

	public void LoadAdBanner() {
		Debug.Log("AdbannerLoad");
		adBannerShow = true;
		if (!iap_NoAds) {
			if (banner == null) {
				banner = new UnityEngine.iOS.ADBannerView(UnityEngine.iOS.ADBannerView.Type.Banner, UnityEngine.iOS.ADBannerView.Layout.Top);
				UnityEngine.iOS.ADBannerView.onBannerWasClicked += OnBannerClicked;
				UnityEngine.iOS.ADBannerView.onBannerWasLoaded  += OnBannerLoaded;
			}
		}
	}
	#endif


	
	//-------------------------------===============


	
	public void pause() {
		
		//beforePauseTimeScale = Time.timeScale;
		Time.timeScale = 0;
		isPaused = true;
		
	}
	
	public void resume() {
		
		//Time.timeScale = beforePauseTimeScale;
		isPaused = false;
		
	}






	public void screenShakeNormal() {

		mainCam.shakeWithAmount(0.05f);

	}


	public void screenShakeLarge() {
		
		mainCam.shakeWithAmount(0.15f);
		
	}





	
	public IEnumerator restartLevel() {

		float time = mainCam.GetComponent<Fading>().BeginFade(1);
		yield return new WaitForSeconds(time*Time.deltaTime + 0.01f);
		Application.LoadLevel ("Main");

	}
	public IEnumerator mainMenu() {

		
		float time = mainCam.GetComponent<Fading>().BeginFade(1);
		yield return new WaitForSeconds(time*Time.deltaTime + 0.01f);

		isFirstTimePlaying = true;
		Application.LoadLevel("Menu");
	}
	
	
	





	public void AddHighScore(long score) {

		int numitems = highScores.Count;

		if (score <= highScores[numitems-1].score) //if its less than the last element don't add it.
			return;

		Debug.Log("BEGINNING INSERT OF SCORE");
		for (int i=0; i<numitems; i++) {
			if (score > highScores[i].score) {
				HighScore h = new HighScore();
				Debug.Log("INSERT SCORE ONCE IN POSITION .. " + i);
				h.name = username;
				h.score = score;
				highScores.Insert(i, h);
				break;
			}
		}


		while (highScores.Count > maxHighScores) {
			highScores.RemoveAt(maxHighScores);
		}


		Save ();
	}

	public long GetHighScoreForUser() {
		if (highScores.Count > 0) {
			foreach (HighScore s in highScores) {
				if (s.name == username)
					return s.score;
			}
			return 0;
		}
		else {
			return 0;
		}

	}







	public void Save() {

		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath+"/playerInfo.dat");

		PlayerData data = new PlayerData();



		data.username = username;
		if (highScores != null)
			data.highScores = new List<HighScore>(highScores);
		data.numSwipes = numSwipes;
		data.numSwipesOneGame = numSwipesOneGame;
		data.highestLevel = highestLevel;
		data.playsBeforeVideoAd = playsBeforeVideoAd;
		data.iap_NoAds = iap_NoAds;

		data.numCoins = numCoins;

		data.curentTrackNum = currentTrackNum;
		//which track are unlocked.
		for (int i=0; i<tracksUnlocked.Length; i++) {
			data.tracksUnlocked[i] = tracksUnlocked[i];
		}

		data.baseMult = baseMult;

		data.tut_1 = tut_1;
		data.tut_2 = tut_2;
		data.tut_3 = tut_3;
		data.tut_4 = tut_4;
		data.tut_5 = tut_5;
		data.tut_6 = tut_6;
		data.tut_7 = tut_7;
		data.tut_8 = tut_8;
		data.tut_9 = tut_9;
		data.tut_10 = tut_10;



		bf.Serialize(file, data);
		file.Close();
	}





	public void Load() {

		if (File.Exists(Application.persistentDataPath+"/playerInfo.dat")) {

			BinaryFormatter bf = new BinaryFormatter();

			FileStream file = File.Open(Application.persistentDataPath+"/playerInfo.dat", FileMode.Open);
			PlayerData data = (PlayerData)bf.Deserialize(file);


			if (data.username.Length > 0)
				username = data.username;

			if (data.highScores != null)
				highScores = new List<HighScore>(data.highScores);
			
			numSwipes = data.numSwipes;
			numSwipesOneGame = data.numSwipesOneGame;
			highestLevel = data.highestLevel;
			playsBeforeVideoAd = data.playsBeforeVideoAd;
			iap_NoAds = data.iap_NoAds;

			numCoins = data.numCoins;

			currentTrackNum = data.curentTrackNum;
			for (int i=0; i<tracksUnlocked.Length; i++) {
				tracksUnlocked[i] = data.tracksUnlocked[i];
			}

			baseMult = data.baseMult;


			tut_1 = data.tut_1;
			tut_2 = data.tut_2;
			tut_3 = data.tut_3;
			tut_4 = data.tut_4;
			tut_5 = data.tut_5;
			tut_6 = data.tut_6;
			tut_7 = data.tut_7;
			tut_8 = data.tut_8;
			tut_9 = data.tut_9;
			tut_10 = data.tut_10;
	

		}

	}


	
}






[Serializable]
class PlayerData {


	public string username;
	public List<HighScore> highScores;
	public int numSwipes;
	public int numSwipesOneGame;
	public int highestLevel;
	public int playsBeforeVideoAd;
	public bool iap_NoAds;

	public int numCoins;

	//music
	public int curentTrackNum;
	public bool[] tracksUnlocked = new bool[50];

	public float baseMult;


	//tutorials.
	public bool tut_1 = false;
	public bool tut_2 = false;
	public bool tut_3 = false;
	public bool tut_4 = false;
	public bool tut_5 = false;
	public bool tut_6 = false;
	public bool tut_7 = false;
	public bool tut_8 = false;
	public bool tut_9 = false;
	public bool tut_10 = false;


}





[Serializable]
public class HighScore {
	
	public string name;
	public long score;

}


