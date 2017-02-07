using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using System.Collections.Generic;
using System.Linq;

using UnityEngine.Advertisements;

public class LevelController : MonoBehaviour {


	GameState gs;

	public enum levelState {
		tut_1, //touch to move
		tut_2, //swipe in the correct direction
		tut_3, //watch out for lasers
		tut_4, //collect score mult, they increase score.
		tut_5, //they protect you.
		tut_6, //Gold ones permanently increase multiplier.
		tut_7, //3
		tut_8, //2
		tut_9, //1
		tut_10, //good luck.
		normal,
		paused,
		collectingGold,
		menu
	};
	[HideInInspector]
	public levelState ls = levelState.normal;
	[HideInInspector]
	public levelState pls = levelState.normal;

	bool levelStateNormalWithBoss = false;
	BOSS currentBoss = null;

	/**
	 * TOUCH STUFF
	 */
	Touch currentTouch;
	Vector2 startTouchPos;
	Vector2 lastMoveDelta;
	float deltaTouchTime;
	float lastTouchTime;
	Vector2 arrowVelocity;
	Vector2 arrowMaxVelocity = new Vector2(.7f,.7f);
	GameObject currentArrow;
	float stopVelocityTime = 0.5f;
	float touchTime;
	[HideInInspector]
	public bool isTouchingScreen = false;



	[HideInInspector]
	public bool backgroundMoving = false;
	Vector3 nextSlideStartPos;
	float _timeStartedMovingBackground;

	int curExpectedSwipeDir = -1;
	bool firstSlide = true;
	GameObject nextSlide;
	GameObject prevSlide;



	float swipeTimer = 0;
	float fastSwipeTime = 2.5f; //number of seconds before you start losing points for taking to long to swipe.
	
	[HideInInspector]
	public bool incorrectSwipe = true; ///makes it so the menu shows up first.

	int gameOverCounter = 0; //count how many times you've gotten game over.
	

	/**
	 * VARIABLES IN CONTROL OF LEVELING UP AND SCORE
	 */

	//------------------- currents scores and stats.
	[HideInInspector]
	public int correctSwipes = 0;
	int lives = 1;
	[HideInInspector]
	public long realScore = 0; 
	public int currentLevel = 0;
	public int consecutiveFastSwipes = 0; //controls multipliers and other things.
	public int consecutiveCorrectSwipes = 0;


	//------------------------ values for calulating score and score needed to level up.
	const int initialPoints = 10;
	const int scoreLevelUpMult = 10;
	long pointsToNextLevel = 0; 

	long pointsGottenThisLevel = 0;
	long pointsPerSwipeThisLevel = 10;




	//-------------------- multiplier values.
	float baseMult = 0;
	float baseMultAdded = 0.01f; //added for gold tetra.
	float scoreMult = 1.00f;
	float multiplierAdded = 0.05f; //temp score multiplier added.
	int maxMultLevels = 20;
	bool hasMultiplier = false;

	bool didCollectGold = false;

	bool firstTimeCollectingBlue = false; //so that the terrain lines dont flash red when you first press start.
	


	//------- tutorial checks.

	bool didCollectTutorialTetraBlue = false;
	bool didCollectTutoiralTetraGold = false;




	/**
	 * 
	 * game over screen variables.
	 */

	Text killText;






	

	

	/**
	 * VARIBALES FOR ADVERTISING
	 */
	bool adShown = false;
	bool showAd = false;
	bool calculatedAdShouldShow = false;

	bool watchedContinueAd = false;


	/**
	*for sound effects.
	*/
	public AudioClip sfx_success;
	public AudioClip sfx_success1;
	public AudioClip sfx_success2;
	public AudioClip sfx_success3;
	public AudioClip sfx_levelUp;
	public AudioClip sfx_fail;
	public AudioClip sfx_gameover;
	public AudioClip sfx_ActivateShield;
	public AudioClip sfx_woosh;
	public AudioClip sfx_wooshIn;
	public AudioClip sfx_button;
	public AudioClip sfx_cash;
	public AudioClip sfx_coin;

	
	GameObject buttonSound;
	bool setSoundButton=false;




	// ---------T H E   A R R O W
	
	GameObject arrow;
	GameObject shield;

	//for double swiping.
	GameObject currentSwipeDot;



	//-----------B U T T O N S  A N D   T E X T 
	GameObject resetButton;
	GameObject score;
	Text scoreText;
	Color scoreTextStartingColor;
	Text correctSwipesText;
	Text levelText;
	Text scoreMultText;
	Text scoreMultBaseText;
	Text comboText;
	//Text comboLabel;
	Text coinsText;
	Text coinsTextMain;
	Text continueNumCoinsText;

	LineRenderer levelMeter;

	Text tutText;

	BigMessage bigMessage;

	//----------- B A C K G R O U N D   O B J E C T S

	GameObject _3dcam;

	LaserGrid laserGrid;
	LaserGrid laserGrid2;

	GameObject tetraGen; //the tetra generator.
	GameObject palmGen; //palm tree generator.
	GameObject sunStreaks; //sun lights that move around.
	GameObject lavaStreaks; //red sun streaks.

	GameObject menu;

	GameObject logo;






	/// ---- S H O W I N G   S C O R E   A N D   O T H E R   M E S S A G E S 
 	 
 	GameObject scoreAddedText;



	List<messageToShow> scoreAddedQueue = new List<messageToShow>(); //queue of messages to show.
	struct messageToShow {
		public string m;
		public int color; /// 0 represents MAGENTA, 1 represents CYAN, 2 is YELLOW
	}
	public void addMessage(string m, int color) {
		messageToShow mess = new messageToShow();
		mess.m = m;
		mess.color = color;
		scoreAddedQueue.Insert(0, mess); //insert to front. so removing from end is easy in my algorithm.
	}
	void ShowMessage(string m, int color) {
		scoreAddedText.GetComponent<ScoreAdded>().SetScore(m, color);
	}
	

	float showMessageTime = 0.3f; //time between queued HUD pop up messages.
	float showMessageTimer = 0;














	// Use this for initialization
	void Start () {

		
		gs = GameState.sharedGameState;
		gs.level = this;

		if (Advertisement.isSupported) {
			Advertisement.Initialize("31511", false);
		}
		
		
		_3dcam = GameObject.Find("3DCam");
		
		menu = GameObject.Find("BackgroundSlideGameOver");



		laserGrid = GameObject.Find("LaserGrid").GetComponent<LaserGrid>();

		laserGrid2 = GameObject.Find("LaserGrid2").GetComponent<LaserGrid>();
		laserGrid2.gameObject.SetActive(false);





		tetraGen = GameObject.Find("_TetraGen");
		tetraGen.GetComponent<TetraGen>().enabled = false;

		palmGen = GameObject.Find("PalmGen");
		palmGen.GetComponent<PalmGen>().isActive = false;

		sunStreaks = GameObject.Find("Sun_Streaks");
		sunStreaks.SetActive(false);

		lavaStreaks = GameObject.Find("Lava_Streaks");
		lavaStreaks.SetActive(false);


		logo = GameObject.Find("Logo_LED");

	


		levelMeter = GameObject.Find("LevelMeterInner").GetComponent<LineRenderer>();
		levelMeter.sortingLayerName="HUD";
		levelMeter.transform.parent.gameObject.SetActive(false);

	

		scoreText = GameObject.Find("ScoreText").GetComponent<Text>();
		scoreTextStartingColor = scoreText.color;
		scoreText.enabled = false;

		//correctSwipesText = GameObject.Find("CorrectSwipes").GetComponent<Text>();
		levelText = GameObject.Find("LevelText").GetComponent<Text>();
		SetLevel(0, 0);
		levelText.enabled = false;

		scoreMultText = GameObject.Find("ScoreMult").GetComponent<Text>();
		baseMult = gs.baseMult;
		RoundMultipliers();
		scoreMultText.enabled = false;

		scoreMultBaseText = GameObject.Find("ScoreMultBase").GetComponent<Text>();
		SetScoreBaseMult(baseMult);
		scoreMultBaseText.enabled = false;

		comboText = GameObject.Find("ComboText").GetComponent<Text>();
		//comboLabel = GameObject.Find("ComboLabel").GetComponent<Text>();
		SetCombo(0);


		coinsText = GameObject.Find("CoinsText").GetComponent<Text>();
		coinsTextMain = GameObject.Find("CoinsTextMain").GetComponent<Text>();
		continueNumCoinsText = GameObject.Find("ContinueNumCoinsText").GetComponent<Text>();

		SetCoins(-1);


		killText = menu.transform.FindChild("Menu").FindChild("MainCanvas").FindChild("KillText").GetComponent<Text>();


		buttonSound = GameObject.Find("ButtonToggleSound");


		scoreAddedText = GameObject.Find("ScoreAddedCanvas").transform.FindChild("ScoreAdded").gameObject;
		//patternBG = GameObject.Find("patternBG").transform;



		//check if completed tutorials.

		tutText = GameObject.Find("Tut_Text").GetComponent<Text>();
		tutText.gameObject.SetActive(false);

		bigMessage = GameObject.Find("BigMessage").GetComponent<BigMessage>();



		NextSlide(); ////load the background slide.....

		
	}



	
	public void StartGame() {
		
		
		
		gs.ac.PlaySFX(sfx_button);

		gs.isFirstTimePlaying = false;

		if (!gs.tut_1)
			newState(levelState.tut_1);
		else if (!gs.tut_2)
			newState(levelState.tut_2);
		else if (!gs.tut_3)
			newState(levelState.tut_3);
		else if (!gs.tut_4)
			newState(levelState.tut_4);
		else if (!gs.tut_5)
			newState(levelState.tut_5);
		else if (!gs.tut_6)
			newState(levelState.tut_6);
		else if (!gs.tut_7)
			newState(levelState.tut_7);
		else if (!gs.tut_8)
			newState(levelState.tut_8);
		else if (!gs.tut_9)
			newState(levelState.tut_9);
		else if (!gs.tut_10)
			newState(levelState.tut_10);

		levelStateNormalWithBoss = false;


		//REMOVE THE BOSS
		if (currentBoss != null) {
			currentBoss.RemoveBars();
			Destroy(currentBoss.gameObject);
			currentBoss = null;
		}

		
		
		
		
		///Reinitialize HUD
		/// 
		levelMeter.transform.parent.gameObject.SetActive(true);
		levelText.enabled = true;
		scoreText.enabled = true;
		scoreMultText.enabled = true;
		scoreMultBaseText.enabled = true;
		
		logo.SetActive(false);
		
		
		
		
		//// INITIALIZE ALL GAME RELATED SCORE VARIABLES.....
		realScore = 0;
		SetScore(realScore);
		scoreText.color = scoreTextStartingColor;
		
		
		baseMultAdded = 0.01f;
		baseMult = gs.baseMult;
		RemoveScoreMultiplier();
		SetScoreBaseMult(baseMult);
		
		pointsGottenThisLevel = 0; //reset how many points you have for any level.
		pointsPerSwipeThisLevel = initialPoints;
		int debugStartLevel = 0;
		//debugStartLevel = 999; //-------- comment out this line to default back to level 0 when you restart. or change the number.
		currentLevel = debugStartLevel;
		SetLevel(currentLevel, 0);
		SetCombo(0);
		
		correctSwipes = 0;
		consecutiveFastSwipes = 0;
		consecutiveCorrectSwipes = 0;

		
		
		
		//SET LASER GRID .. AND COLORS
		laserGrid.Reset(false);
		laserGrid2.Reset(true); //true is to turn off the gameobject after.
		
		gs.SetColorStyle(GameState.ColorStyles.normal);
		
		//--------------------------------------------------------
		

		
		//LEVEL SPECIFIC GAME OBJECTS.
		
		palmGen.GetComponent<PalmGen>().isActive = false;
		
		sunStreaks.SetActive(false);
		lavaStreaks.SetActive(false);
		
		
		////empty the message queues.
		scoreAddedQueue.Clear();
		
		
		
		HideGameOver();  //----- hide the game over / main menu when playing obviously.






		//------- TOUCH VARIABELS ...
		isTouchingScreen = false;
		firstSlide = true;
		incorrectSwipe = false; // no game over. no menu.


		firstTimeCollectingBlue = false;

		watchedContinueAd = false;


		NextSlide();


	}



	void ShowGameOver(bool newGame) {

		//// MANIPULATE MENU ITEMS
		
		//----- pause buttons
		
		GameObject.Find("PauseButton").GetComponent<Image>().enabled = false;
		GameObject.Find("PauseButton").GetComponent<Button>().enabled = false;




		Text gameoverText = menu.transform.FindChild("Menu").FindChild("MainCanvas").FindChild("GameOverText").GetComponent<Text>();
		
		string[] gameOverTexts = {
			"Space Cadet!",
			"Radical!",
			"Schweet!",
			"Gnarly!",
			"Spectacular!",
			"Head Rush!",
			"Mental!",
			"Wicked!",
			"Finger Breaking!",
			"Deadly!",
			"Amazing!",
			"Outrageous!",
			"Unbelievable!",
			"Righteous!",
			"Flash!",
			"Eye Shattering!",
			"Stellar!",
			"Ace!",
			"Double Ace!",
			"Triple Ace!",
			"Ace plus Infinity!",
			"Ace times Infinity!",
			"Ace to the InfinityTH Power!",
			"Game Breaking!"
		};


		string[] hintTexts = {
			"The being faster nets you more points.",
			"A shield will protect you from a wrong move",
			"Tetras give you a shield, and increase multipliers.",
			"Higher level gold Tetras give more muliplier increases.",
			"Gold Tetras increase your permanent multiplier.",
			"Your permanent multiplier stays even after a Game Over.",
			"The more you play, the better you'll get.",
			"A higher permanent multiplier is key to a higher score.",
			"A higher permanent multiplier will make bosses easier.",
			"Finish bosses quickly before the timer runs out.",
			"A shield won't save you if a boss timer runs out."
		};
		
		
		int textNum = 0;

		if (newGame)
			gameoverText.text = "";
		else 
			gameoverText.text = "GAME OVER!";

		string kText = hintTexts[Random.Range(0,hintTexts.Length)];

		if (gs.tut_10 && !gs.isFirstTimePlaying) {
			SetKillText(kText);
		}
		else {
			SetKillText("");
		}

		if (gs.isFirstTimePlaying) {
			menu.transform.position = new Vector3(3.75f, 6.67f);
		}
		else {
			StopCoroutine("MoveFromTo");
			StartCoroutine(MoveFromTo(menu, new Vector3(25, 6.67f),new Vector3(3.75f, 6.67f) , 0.5f));
		}
	}


	void HideGameOver() {

		//// MANIPULATE MENU ITEMS
		
		//----- pause buttons
		
		GameObject.Find("PauseButton").GetComponent<Image>().enabled = true;
		GameObject.Find("PauseButton").GetComponent<Button>().enabled = true;

		StopCoroutine("MoveFromTo");
		StartCoroutine(MoveFromTo(menu, new Vector3(3.75f, 6.75f), new Vector3(15, 6.67f), 0.5f));
	}





	//------------------------ S E T T I N G   O F   V A R I O U S   H U D   T E X T S 


	public void SetKillText(string kText) {
		killText.text = kText;
	}

	


	void SetScore(long s) {

		string scoreString = formatScoreToString(s);
		scoreText.text = scoreString;

		scoreText.gameObject.GetComponent<GrowShrink>().Activate();
	}

	public string formatScoreToString(long s) {

		string scoreString = "";
		if (s <= 999999999 && s >= 0) { //score is less than or equal to 999 million. display normally as numbers.
			scoreString = s.ToString() + " pts";
		}
		else { ///score is 1 billion or more so display as 1.00 B 1.00 KB or 1.00 MB max is 999.99 MB.
			string quantityIdentifier = "B";
			float newScore = 1.0f;
			if (s <= 999999999999 && s >= 0) {
				newScore = (float)s / 1000000000f; //divide by one billion.
			}
			else if (s<= 999999999999999 && s >= 0) {
				quantityIdentifier = "KB";
				newScore = (float)s / 1000000000000f; //divide one trillion.
			}
			else if (s<= 999999999999999999 && s >= 0) {
				quantityIdentifier = "MB";
				newScore = (float)s / 1000000000000000f; //divide one quadrillion.
			}
			scoreString = newScore.ToString("#.00") + " " + quantityIdentifier + " pts";
		}

		return scoreString;

	}
	
	
	
	void SetScoreMultiplier(float mult) {
		scoreMultText.text = "x " + mult.ToString("#.00");
		scoreMultText.gameObject.GetComponent<GrowShrink>().Activate();


		//Check for Achievements to unlock-----------
		if (mult >= 50.0f)
			gs.social.AttemptToReportAchievement(SocialAssets.Achievement_ID_ScoreMult_50, 100, true);
		else if (mult >= 20.0f)
				gs.social.AttemptToReportAchievement(SocialAssets.Achievement_ID_ScoreMult_20, 100, true);
		else if (mult >= 5.0f)
				gs.social.AttemptToReportAchievement(SocialAssets.Achievement_ID_ScoreMult_5, 100, true);
		else if (mult >= 2.0f)
				gs.social.AttemptToReportAchievement(SocialAssets.Achievement_ID_ScoreMult_2, 100, true);
		//-------------------------------------------


		//------------------------set the shield.
		if (!incorrectSwipe) {
			if (!hasMultiplier) {
				hasMultiplier = true;
				AddShield();
			}
		}


	}
	void RemoveScoreMultiplier() {
		scoreMult = 1 + baseMult;

		RoundMultipliers();

		scoreMultText.text = "x " + scoreMult.ToString("#.00");
		scoreMultText.gameObject.GetComponent<GrowShrink>().Activate();

		if (hasMultiplier) {
			hasMultiplier = false;
			RemoveShield();
		}
	}
	void AddShield() {
		shield.SetActive(true);
		gs.ac.PlaySFX(sfx_ActivateShield);


		laserGrid.DeactivatePatternHorizontal(LaserGrid.cph.fadeInOut);
		laserGrid.DeactivatePatternVertical(LaserGrid.cpv.fadeInOut);
		
		laserGrid2.DeactivatePatternHorizontal(LaserGrid.cph.fadeInOut);
		laserGrid2.DeactivatePatternVertical(LaserGrid.cpv.fadeInOut);
	}
		
	void RemoveShield() {
		shield.SetActive(false);

		laserGrid.SetColorPatternHorizontal(LaserGrid.cph.fadeInOut, laserGrid.c_red);
		laserGrid.SetColorPatternVertical(LaserGrid.cpv.fadeInOut, laserGrid.c_red);

		laserGrid2.SetColorPatternHorizontal(LaserGrid.cph.fadeInOut, laserGrid.c_red);
		laserGrid2.SetColorPatternVertical(LaserGrid.cpv.fadeInOut, laserGrid.c_red);
	}


	void SetScoreBaseMult(float baseM) {
		if (baseM > 0) {
			scoreMultBaseText.text = "+ " + baseM.ToString("#.00");
			//scoreMultBaseText.gameObject.GetComponent<GrowShrink>().Activate();
		}
		else {
			scoreMultBaseText.text = "";
		}
	}
	
	void SetLevel(int level, float percent) {

		//Debug.Log("Level percent = " + percent);

		if (percent < 0.01f) percent = 0.01f;

		levelText.text = "L." + level.ToString();
		levelMeter.SetPosition(1, new Vector3(200*percent, 0, 0));
	}
	

	Coroutine comboCoroutine;

	void SetCombo(int combo) {
		
		if (combo >= 999) combo = 999;

		//only 2 consecutive hits trigger a combo.
		string comboTextAddon = "";

		if (combo < 2) {
			Color transparent = new Color(0,0,0,0);
			comboText.color = transparent;
			//comboLabel.color = transparent;
			comboText.fontSize = 60;
			//comboLabel.fontSize = 40;
		}
		else if (combo < 10) {
			comboText.fontSize = 60;
			//comboLabel.color = new Color(1,0,100f/255f, 1);
			comboText.color = new Color(1,0,100f/255f, 1);

			if (combo >= 5)
				comboTextAddon = "PULSING!";
		}
		else if (combo < 50) {
			comboText.fontSize = 90;
			//comboLabel.color = new Color(1,0,100f/255f, 1);
			comboText.color = new Color(0,1,1,1);
			if (combo >= 10 && combo < 15)
				comboTextAddon = "INTENSE!";
			else if (combo >= 15 && combo < 20)
				comboTextAddon = "FLASHING!";
			else if (combo >= 20 && combo < 25)
				comboTextAddon = "HOT!";
			else if (combo >= 25 && combo < 30)
				comboTextAddon = "FEVERISH!";
			else if (combo >= 30 && combo < 35)
				comboTextAddon = "GLEAMING!";
			else if (combo >= 35 && combo < 40)
				comboTextAddon = "LUSTROUS!";
			else if (combo >= 40 && combo < 45)
				comboTextAddon = "BEAMING!";
			else if (combo >= 45 && combo < 50)
				comboTextAddon = "BRILLIANT!";
		}
		else {
			comboText.fontSize = 130;
			//comboLabel.color = new Color(1,0,100f/255f, 1);
			comboText.color = new Color(1,1,0,1);

			if (combo >= 50 && combo < 75)
				comboTextAddon = "RADIANT!";
			else if (combo >= 75 && combo < 100)
				comboTextAddon = "STELLAR";
			else if (combo >= 100 && combo < 150)
				comboTextAddon = "ACE!";
			else if (combo >= 150 && combo < 200)
				comboTextAddon = "DOUBLE ACE!";
			else if (combo >= 200 && combo < 300)
				comboTextAddon = "TRIPLE ACE!";
			else if (combo >= 300 && combo < 400)
				comboTextAddon = "ACE PLUS INFINITY!";
			else if (combo >= 400 && combo < 500)
				comboTextAddon = "ACE TIMES INFINITY!";
			else if (combo >= 500 && combo < 999)
				comboTextAddon = "BLACK HOLE!";
			else if (combo == 999)
				comboTextAddon = "GAME BREAKING!";
		}

		comboText.text = combo.ToString() + "\n<size=40>"+comboTextAddon+"</size>";
		comboText.gameObject.GetComponent<GrowShrink>().Activate();


		//showing different combo messages.

		string comboMessage = "COMBO " + combo;
		bool showMessage = false;
		if (combo < 10) {
			if (combo == 5)
				showMessage = true; //first show at 5.
		}
		else if (combo <= 100) {
			if (combo % 10 == 0)
				showMessage = true; //then every 10 up to 100.
		}
		else if (combo <= 1000) {
			if (combo % 25 == 0)
				showMessage = true; //every 25 up to 1000.
		}
		else {
			if (combo % 100 == 0)
				showMessage = true; //every 100 up to 9,999.
		}
		if (combo == 999 && consecutiveCorrectSwipes == 999) { //consecutive correct swipes can be bigger, but combo can only display up to 999. this way to combo displays only once.
			showMessage = true;
		}

		if (showMessage)
			addMessage(comboMessage, 0);
		//start to fade the text.
		//if (comboCoroutine != null) {
		//	StopCoroutine(comboCoroutine);
		//}
		//comboCoroutine = StartCoroutine(fadeCombo());

	}

	IEnumerator fadeCombo() {

		float fadeAmt = 0.025f;
		Color c = comboText.color;
		//Color cLabel = comboLabel.color;

		while(true) {

			c.a -= fadeAmt;

			if (c.a <= 0) c.a = 0;

			//cLabel.a = c.a;

			comboText.color = c;
			//comboLabel.color = cLabel;

			if (c.a <=0)
				break;

			yield return null;

		}

	}






	public void SetCoins(int amtAdded) {

		if (gs.numCoins >= 9999) {
			gs.numCoins = 9999;
		}

		coinsText.text = gs.numCoins.ToString();
		coinsTextMain.text = gs.numCoins.ToString();
		continueNumCoinsText.text = "You Currently Have " + gs.numCoins + " Tokens.";

		coinsText.gameObject.GetComponent<GrowShrink>().Activate();
		coinsTextMain.gameObject.GetComponent<GrowShrink>().Activate();


		if (amtAdded != -1) {
			string tokenString = " TOKEN!";
			if (amtAdded > 1) tokenString = " TOKENS!";

			addMessage("+ " + amtAdded.ToString() + tokenString, 2);
		}

	}









	//----------------------- A D V E R T I S I N G   


	private void AdCallback (ShowResult result) { //ad callback for normal, non-reward ads.
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log("The ad was successfully shown.");
			break;
		case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			break;
		}
	}



	private void ContinueAdCallback(ShowResult result) { //ad callback for reward ads.
		bool complete = false;

		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log("The ad was successfully shown.");

			//player finished watching ad.
			complete = true;

			break;
		case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");

			//give player benefit of the doubt.
			complete = true;

			break;
		}

		if (complete) {

			watchedContinueAd = true;

			incorrectSwipe = false;

		}

		
		HideContinue(); //--calls the next slide. and game continues.
	}



	private void PowerUpAdCallback(ShowResult result) {

		bool complete = false;

		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log("The ad was successfully shown.");
			complete = true;
			
			break;
		case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			
			//give player benefit of the doubt.
			complete = true;
			
			break;
		}

		HidePowerUpMessage();

		if (complete) {
			RewardPowerUp();
		}

	}




	void RewardPowerUp() {
		IncreaseBaseMult(0.03f, true);
	}


	void CheckToShowPowerUpAd() {
		//increase the counter, because this method was called because you got game over.
		gameOverCounter++;
		//either randomize it or do it every so many game overs.	
		//must at least have made it to the second boss so level 10.. this way wen people are learning, there aren't ads jumping in their face.
		if (gs.highestLevel >= 10) {
			if (gameOverCounter >= 4) {
				gameOverCounter = 0;
				ShowPowerUpMessage();
			}
		}
	}

	public void ShowPowerUpMessage() {


		GameObject powerUpMenu = null;
		powerUpMenu = GameObject.Find("PowerUpCanvas");
		Vector3 pos = powerUpMenu.transform.localPosition;
		
		StopCoroutine("MoveFromTo");
		StartCoroutine(MoveFromTo(powerUpMenu, pos, new Vector3(3.75f, pos.y), 0.2f));
		
		//gs.ac.PauseBGM();
		gs.ac.PlaySFX(sfx_wooshIn);

	}

	public void ClosePowerUpMessage() {
		gs.ac.PlaySFX(sfx_button);
		HidePowerUpMessage();
	}

	public void HidePowerUpMessage() {

		GameObject powerUpMenu = null;
		powerUpMenu = GameObject.Find("PowerUpCanvas");
		Vector3 pos = powerUpMenu.transform.localPosition;
		
		StopCoroutine("MoveFromTo");
		StartCoroutine(MoveFromTo(powerUpMenu, pos, new Vector3(15f, pos.y), 0.2f));
		
		//gs.ac.PauseBGM();
		gs.ac.PlaySFX(sfx_woosh);
		
		
	}

	public void WatchPowerUpAd() {

		gs.ac.PlaySFX(sfx_button);

		ShowOptions options = new ShowOptions();
		options.resultCallback = PowerUpAdCallback;

		Advertisement.Show("rewardedVideoZone", options);

	}









	public void WatchContinueAd() {
		
		gs.ac.PlaySFX(sfx_button);

		ShowOptions options = new ShowOptions();
		options.resultCallback = ContinueAdCallback;
		
		Advertisement.Show("rewardedVideoZone", options);
		
	}


	public void TokenContinueAd() {

		gs.ac.PlaySFX(sfx_cash);

		watchedContinueAd = true;
		
		incorrectSwipe = false;


		gs.numCoins -= 5;
		gs.Save();
		SetCoins(-1);

		addMessage("-5 TOKENS", 2);


		HideContinue();

	}

	
	
	public void CloseContinue() {

		gs.ac.PlaySFX(sfx_button);

		HideContinue();

	}


	IEnumerator StartContinue() {
		
		deactivatePauseButton();

		float timer = 0;
		while(timer < 1) {
			timer += Time.deltaTime;
			yield return null;
		}
		ShowContinue();
	}

	public void ShowContinue() {


		GameObject continueMenu = null;
		continueMenu = GameObject.Find("ContinueCanvas");
		Vector3 pos = continueMenu.transform.localPosition;


		//Disable pay with tokens button.
		if (gs.numCoins < 5)
			GameObject.Find("TokensContinueButton").GetComponent<Button>().interactable = false;
		else 
			GameObject.Find("TokensContinueButton").GetComponent<Button>().interactable = true;

		
		StopCoroutine("MoveFromTo");
		StartCoroutine(MoveFromTo(continueMenu, pos, new Vector3(3.75f, pos.y), 0.2f));
		
		//gs.ac.PauseBGM();
		gs.ac.PlaySFX(sfx_wooshIn);
		
	}
	
	
	public void HideContinue() {

		activatePauseButton();

		GameObject continueMenu = null;
		continueMenu = GameObject.Find("ContinueCanvas");
		Vector3 pos = continueMenu.transform.localPosition;
		
		StopCoroutine("MoveFromTo");
		StartCoroutine(MoveFromTo(continueMenu, pos, new Vector3(15f, pos.y), 0.2f));
		
		//gs.ac.PauseBGM();
		gs.ac.PlaySFX(sfx_woosh);


		if (levelStateNormalWithBoss) { //if you were fighting boss.. remove bars.
			currentBoss.RemoveBars();
			CreateBoss();
		}
		
		NextSlide();


		
	}














	//---------------- T O G G L E S   /   S T A T E S   /   P A U S I N G  




	public void newState(levelState s) {

		pls = ls;

		ls = s;

	}




	public void toggleSound() {
		
		gs.ac.toggleMute();
		
		
		gs.ac.PlaySFX(sfx_button);
		
		setSoundButton = false;
		
	}


	public void TogglePause() {
		bool paused = ls == levelState.paused;
		if (paused)
			unPause(true);
		else
			pause (true);
	}

	void deactivatePauseButton() {
		GameObject.Find("PauseButton").GetComponent<Image>().enabled = false;
		GameObject.Find("PauseButton").GetComponent<Button>().enabled = false;
	}
	void activatePauseButton() {
		GameObject.Find("PauseButton").GetComponent<Image>().enabled = true;
		GameObject.Find("PauseButton").GetComponent<Button>().enabled = true;
	}

	public void pause(bool showMenu) {


		newState(levelState.paused);
		
		
		deactivatePauseButton();


		nextSlide.GetComponent<BackgroundSlide>().DeactivateObstacles(false); ///stop those obstacles. @param false=not gameOver


		if (showMenu) {
			///save if they pause the game so if they go back to the menu or reset.. the game has the latest values.
			gs.Save();

			GameObject pausemenu = null;
			pausemenu = GameObject.Find("PauseCanvas");
			Vector3 pos = pausemenu.transform.localPosition;
			
			StopCoroutine("MoveFromTo");
			StartCoroutine(MoveFromTo(pausemenu, pos, new Vector3(pos.x, 6.67f), 0.2f));

			//gs.ac.PauseBGM();
			gs.ac.PlaySFX(sfx_button);
			gs.ac.PlaySFX(sfx_wooshIn);

		}

	}


	public void unPause(bool hideMenu) {

		newState(pls); ///go to the previous level state when unpausing.

		activatePauseButton();

		nextSlide.GetComponent<BackgroundSlide>().ReactivateObstacles(); //reactivate any deactivated obstacles.


		
		if (hideMenu) {

			GameObject pausemenu = null;
			
			pausemenu = GameObject.Find("PauseCanvas");
			
			Vector3 pos = pausemenu.transform.localPosition;
			
			StopCoroutine("MoveFromTo");
			StartCoroutine(MoveFromTo(pausemenu, pos, new Vector3(pos.x, -6.67f), 0.2f));

			
			//gs.ac.UnPauseBGM();
			gs.ac.PlaySFX(sfx_button);
			gs.ac.PlaySFX(sfx_woosh);

		}

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






	public void MainMenu() {
		ForceQuitGame();
	}


	/// <summary>
	/// Forces the quitting of the game.
	/// done buy pressing the home button at the pause menu.
	/// or when a boss kills you.
	/// </summary>
	void ForceQuitGame() {

		//so the pause menu goes away if it's up.
		if (ls == levelState.paused) {
			unPause(true);
		}
		//if after unpausing it's now the collecting gold state.
		if (ls == levelState.collectingGold) {
			newState(levelState.normal);
			//find the gold tetra and destroy it.
			GameObject tetraGold = GameObject.Find("_tetra_gold(Clone)");
			if (tetraGold != null)
				Destroy(tetraGold);
		}

		//incase you're in a tutorial.
		newState(levelState.normal);

		//so the continue menu doesn't show.
		watchedContinueAd = true;

		//if you have a multiplier, you have a shield as well, destory it.
		if (hasMultiplier) {
			RemoveScoreMultiplier();
		}

		/*
		 * if there is a background currently moving in.. promply remove it. it should be known as the next slide.
		 */
		if (backgroundMoving){
			Destroy(nextSlide);
		}

		//die..
		SwipeDetected(100);
		
	}







	//-------------------
	//-------------------
	//-------------------
	//--------------------=========================   T O U C H I N G   S C R E E N ===========----------------
	//-------------------
	//-------------------
	//-------------------

	public void TouchStarted(Touch t) {
		isTouchingScreen = true;
		touchTime = Time.time;
		lastTouchTime = Time.time;
		deltaTouchTime = 0;
		
		if (!incorrectSwipe && !(ls == levelState.paused)) {
			
			if (currentArrow == null && arrow != null) {
				currentArrow = arrow;
				arrowVelocity = Vector2.zero;
			}
		}
	}

	public void TouchMoved(Touch t) {
		if  (!incorrectSwipe && !(ls == levelState.paused)) {
			
			if (currentArrow != null && !backgroundMoving) {
				
				deltaTouchTime = Time.time - lastTouchTime;
				lastTouchTime = Time.time;
				
				
				Vector2 aPos = currentArrow.transform.localPosition;
				Vector2 vel = Vector2.zero;

				Vector2 normalizedVel = Vector2.zero;
				/*
				 * Using Screen.width/height to actually normalize the velocity
				 * gs.winWidth/winHeight is set to the camera size which is always the same,
				 * no matter what your screen size is.
				 * However touch inputs are on the physical screen size, so that's what we need to get here.
				 * Then multiplying by our desired screen size. And that will map the physical screen size
				 * to the camera size.
				 */
				normalizedVel.x = t.deltaPosition.x / (Screen.width);
				normalizedVel.y = t.deltaPosition.y / (Screen.width);

				vel.x = normalizedVel.x * 7.5f; //intended resolution width.
				vel.y = normalizedVel.y * 13.34f; //intended resolution height.

				//vel = t.deltaPosition / 100f;
				//vel.y = -vel.y;
				vel *= 1.7f; //some velocity multiplier to make it that much faster than normal.
				
				if (vel.x > arrowMaxVelocity.x)
					vel.x = arrowMaxVelocity.x;
				else if (vel.x < -arrowMaxVelocity.x)
					vel.x = -arrowMaxVelocity.x;
				if (vel.y > arrowMaxVelocity.y)
					vel.y = arrowMaxVelocity.y;
				else if(vel.y < -arrowMaxVelocity.y)
					vel.y = -arrowMaxVelocity.y;
				
				aPos.x += vel.x;
				aPos.y += vel.y;
				currentArrow.transform.localPosition = aPos;
				
				float moveDist = Mathf.Sqrt((t.deltaPosition.x * t.deltaPosition.x) + (t.deltaPosition.y * t.deltaPosition.y));
				float maxDist = 15f;
				
				if (moveDist > maxDist) { //enough movement.. reset the timer for touching.
					touchTime = 0;
				} 

				if (moveDist > 0)
					arrowVelocity = vel;
				
			}
		}
	}


	public void TouchEnded(Touch t) {
		if (!incorrectSwipe && !(ls == levelState.paused)) {
		}
		isTouchingScreen = false;
		touchTime = 0;
		lastMoveDelta = Vector2.zero;
	}
	





	public void MouseBegan(Event e) {
		isTouchingScreen = true;
		touchTime = Time.time;
		lastTouchTime = Time.time;
		deltaTouchTime = 0;
		
		if (!incorrectSwipe && !(ls == levelState.paused)) {

			if (currentArrow == null && arrow != null) {
				currentArrow = arrow;
				arrowVelocity = Vector2.zero;
			}
		}
	}

	public void MouseMoved(Event e) {
		if  (!incorrectSwipe && !(ls == levelState.paused)) {
			
			if (currentArrow != null && !backgroundMoving) {

				deltaTouchTime = Time.time - lastTouchTime;
				lastTouchTime = Time.time;


				Vector2 aPos = currentArrow.transform.localPosition;
				Vector2 vel = Vector2.zero;

				if (e.isMouse) {
					Vector2 normalizedVel = Vector2.zero;
					normalizedVel.x = e.delta.x / (Screen.width);
					normalizedVel.y = e.delta.y / (Screen.height);
					//Debug.Log(e.delta.x);
					vel.x = normalizedVel.x * 7.50f;
					vel.y = normalizedVel.y * 13.34f;
				//	vel = e.delta / 100;
					lastMoveDelta = e.delta;
				}
				vel.y = -vel.y;

				vel *= 1.7f;

				if (vel.x > arrowMaxVelocity.x)
					vel.x = arrowMaxVelocity.x;
				else if (vel.x < -arrowMaxVelocity.x)
					vel.x = -arrowMaxVelocity.x;
				if (vel.y > arrowMaxVelocity.y)
					vel.y = arrowMaxVelocity.y;
				else if(vel.y < -arrowMaxVelocity.y)
					vel.y = -arrowMaxVelocity.y;
				
				aPos.x += vel.x;
				aPos.y += vel.y;
				currentArrow.transform.localPosition = aPos;
				
				float moveDist = Mathf.Sqrt((e.delta.x * e.delta.x) + (e.delta.y * e.delta.y));
				float maxDist = 15f;
				
				if (moveDist > maxDist) { //enough movement.. reset the timer for touching.
					touchTime = 0;
				} 

				if (moveDist > 0)
					arrowVelocity = vel;

			}
		}
	}

	public void MouseEnded(Event e) {
		
		if (!incorrectSwipe && !(ls == levelState.paused)) {
		}
		isTouchingScreen = false;
		touchTime = 0;
		lastMoveDelta = Vector2.zero;

	}


	






	//// ///////////////   --------------- ============  C O L L E C T I N G   G O L D 
	public void ActivateCollectGold() {

		if (ls != levelState.tut_6) { //if you're not grabbing the tutorial gold tetra.

			//START INTERMISSION TIMER BETWEEN COLLECTING THE GOLD TETRA AND CHANGING TO NEXT SLIDE.
			float intermissionTime = 2;
			StartCoroutine(intermissionToDeactivate(intermissionTime));
			

			float finishSpeed = 10; //--set finish speed of terrain after warp.


			//REMOVE STUFF THAT MIGHTNT BE THERE....
			laserGrid.Reset(false);
			laserGrid2.Reset(true); //true is to turn off the gameobject after.

			sunStreaks.SetActive(false);
			lavaStreaks.SetActive(false);



			int levelTypeToSelect = currentLevel;
			if (currentLevel >= 85) {
				//pick a random 1. every 5 levels up to 80 = 16 types.
				//random from 1-16. then multiply by 5.
				int rand = Random.Range(1,17);
				levelTypeToSelect = rand * 5; // 5 * 5 = 25, 8 * 5 = 40, 16=80, 1=5 //SHOULD WORK
			}

			//pick what type of terrain will be generated next. --- before the warp is done.
			/*
			 * Levels 0-5 is flat, and blue, dark background
			 */
			switch(levelTypeToSelect) {


				/*levels 5-10 has hills, 
				 * and is just like levels 0-5
				 */
			case 5:
				gs.social.AttemptToReportAchievement(SocialAssets.Achievement_ID_Boss_1, 100, true);

				laserGrid.SetHeightPattern(LaserGrid.HeightPattern.normal);

				break;



				/*
				 * Levels 10-15 are the city.
				 */
			case 10:
				if (!gs.tracksUnlocked[1]) {
					gs.ac.UnlockTrack(1, true); //the method saves the game.
				}
				gs.social.AttemptToReportAchievement(SocialAssets.Achievement_ID_Song_Driver, 100, true);



				laserGrid.SetHeightPattern(LaserGrid.HeightPattern.city);
				finishSpeed = 100;

				//add some temp lines.
				float depth = (laserGrid.numHLines * laserGrid.lineDistance) / 5;
				float xpos = 3.75f;

				//right side lasers
				for (int i=0; i<6; i++) {
					Vector3 l1p1 = new Vector3(xpos, ((i)*1.67f), 0); //start.
					Vector3 l1p2 = new Vector3(xpos, ((i)*1.67f), depth); //end.
					
					Vector3 l2p1 = new Vector3(xpos, .33f + ((i)*1.67f), 0);
					Vector3 l2p2 = new Vector3(xpos, .33f + ((i)*1.67f), depth);

					Vector3[] l1 = {l1p1, l1p2};
					Vector3[] l2 = {l2p1, l2p2};

					laserGrid.AddStaticTempLine(laserGrid.c_green, l1, 1); 
					laserGrid.AddStaticTempLine(laserGrid.c_green, l2, 1); 
				}
				//left side lasers.
				for (int i=0; i<6; i++) {
					Vector3 l1p1 = new Vector3(-xpos, ((i)*1.67f), 0); //start.
					Vector3 l1p2 = new Vector3(-xpos, ((i)*1.67f), depth); //end.
					
					Vector3 l2p1 = new Vector3(-xpos, .33f + ((i)*1.67f), 0);
					Vector3 l2p2 = new Vector3(-xpos, .33f + ((i)*1.67f), depth);
					
					Vector3[] l1 = {l1p1, l1p2};
					Vector3[] l2 = {l2p1, l2p2};
					
					laserGrid.AddStaticTempLine(laserGrid.c_green, l1, 1); 
					laserGrid.AddStaticTempLine(laserGrid.c_green, l2, 1); 
				}

				gs.SetColorStyle(GameState.ColorStyles.sunset);
				break;



				/*
				 * 15 - 25 are the ocean.
				 */
			case 15:
				
				gs.social.AttemptToReportAchievement(SocialAssets.Achievement_ID_Boss_3, 100, true);



				laserGrid.SetHeightPattern(LaserGrid.HeightPattern.sine);
				laserGrid.SetColorAll(laserGrid.c_darkBlue, true);
				finishSpeed = 20;

				sunStreaks.SetActive(true);

				gs.SetColorStyle(GameState.ColorStyles.beach);
				break;
			case 20:
				laserGrid.SetHeightPattern(LaserGrid.HeightPattern.sine);
				laserGrid.SetColorAll(laserGrid.c_darkBlue, true);
				finishSpeed = 20;

				sunStreaks.SetActive(true);
				
				gs.SetColorStyle(GameState.ColorStyles.beach);
				break;


				/*
				 * 25 - 35 are the desert. light yellow colors. and the lasers be yellowish.
				 */
			case 25:

				if (!gs.tracksUnlocked[2]) {
					gs.ac.UnlockTrack(2, true); //the method saves the game.
				}
				gs.social.AttemptToReportAchievement(SocialAssets.Achievement_ID_Song_Sentinels, 100, true);


				laserGrid.SetHeightPattern(LaserGrid.HeightPattern.normal);
				gs.SetColorStyle(GameState.ColorStyles.desert);

				laserGrid.SetColorAll(laserGrid.c_orange, true);

				finishSpeed = 10;
				break;
			case 30:
				laserGrid.SetHeightPattern(LaserGrid.HeightPattern.normal);
				gs.SetColorStyle(GameState.ColorStyles.desert);
				
				laserGrid.SetColorAll(laserGrid.c_orange, true);
				
				finishSpeed = 50;
				break;



				/*
				 * Levels 35 - 45
				 * Desert at night. yellow lasers but dark background. maybe stars.
				 */
			case 35:
				gs.social.AttemptToReportAchievement(SocialAssets.Achievement_ID_Boss_7, 100, true);


				laserGrid.SetHeightPattern(LaserGrid.HeightPattern.normal);
				gs.SetColorStyle(GameState.ColorStyles.desertNight);
				
				laserGrid.SetColorAll(laserGrid.c_darkOrange, true);
				
				finishSpeed = 75;

				break;
			case 40:

				laserGrid.SetHeightPattern(LaserGrid.HeightPattern.normal);
				gs.SetColorStyle(GameState.ColorStyles.desertNight);
				
				laserGrid.SetColorAll(laserGrid.c_darkOrange, true);
				
				finishSpeed = 100;
				break;



				/*
				 * 45 - 55 Are LAVA.
				 */
			case 45:
				if (!gs.tracksUnlocked[3]) {
					gs.ac.UnlockTrack(3, true); //the method saves the game.
				}
				gs.social.AttemptToReportAchievement(SocialAssets.Achievement_ID_Song_Invaders, 100, true);

				laserGrid.SetHeightPattern(LaserGrid.HeightPattern.sine);
				laserGrid.SetColorAll(laserGrid.c_red, true);
				gs.SetColorStyle(GameState.ColorStyles.lava);

				lavaStreaks.SetActive(true);
				
				finishSpeed = 20;
				break;
			case 50:
				laserGrid.SetHeightPattern(LaserGrid.HeightPattern.sine);
				laserGrid.SetColorAll(laserGrid.c_red, true);
				gs.SetColorStyle(GameState.ColorStyles.lava);

				lavaStreaks.SetActive(true);

				finishSpeed = 20;
				break;


				/*
				 * 55 - 65 are Darkness.
				 */
			case 55:
				
				gs.social.AttemptToReportAchievement(SocialAssets.Achievement_ID_Boss_11, 100, true);

				laserGrid.SetHeightPattern(LaserGrid.HeightPattern.normal);
				laserGrid.SetColorAll(laserGrid.c_black, true);
				gs.SetColorStyle(GameState.ColorStyles.darkness);
				break;
			case 60:
				laserGrid.SetHeightPattern(LaserGrid.HeightPattern.normal);
				laserGrid.SetColorAll(laserGrid.c_black, true);
				gs.SetColorStyle(GameState.ColorStyles.darkness);
				break;


				/*
				 * 65 - 70 are Normal.
				 */
			case 65:
				
				gs.social.AttemptToReportAchievement(SocialAssets.Achievement_ID_Boss_13, 100, true);

				laserGrid.SetHeightPattern(LaserGrid.HeightPattern.normal);
				laserGrid.SetColorAll(laserGrid.c_cyan, true);
				gs.SetColorStyle(GameState.ColorStyles.normal);
				break;


				/*
				 *  70 - 85 are double flat.
				 */
			case 70:
				
				gs.social.AttemptToReportAchievement(SocialAssets.Achievement_ID_Boss_14, 100, true);


				laserGrid.SetHeightPattern(LaserGrid.HeightPattern.flat);
				laserGrid.SetColorAll(laserGrid.c_cyan, true);
				gs.SetColorStyle(GameState.ColorStyles.normal);
				
				//create double of lasergrid.
				laserGrid2.gameObject.SetActive(true);
				laserGrid2.SetHeightPattern(LaserGrid.HeightPattern.flat);
				laserGrid2.SetColorAll(laserGrid.c_cyan, true);

				finishSpeed = 50;
				break;
			case 75:
				if (!gs.tracksUnlocked[4]) {
					gs.ac.UnlockTrack(4, true); //the method saves the game.
				}
				gs.social.AttemptToReportAchievement(SocialAssets.Achievement_ID_Song_AlmostThere, 100, true);


				laserGrid.SetHeightPattern(LaserGrid.HeightPattern.flat);
				laserGrid.SetColorAll(laserGrid.c_cyan, true);
				gs.SetColorStyle(GameState.ColorStyles.normal);
				
				//create double of lasergrid.
				laserGrid2.gameObject.SetActive(true);
				laserGrid2.SetHeightPattern(LaserGrid.HeightPattern.flat);
				laserGrid2.SetColorAll(laserGrid.c_cyan, true);

				finishSpeed = 80;
				break;
			
			case 80:
				
				gs.social.AttemptToReportAchievement(SocialAssets.Achievement_ID_Boss_16, 100, true);


				laserGrid.SetHeightPattern(LaserGrid.HeightPattern.flat);
				laserGrid.SetColorAll(laserGrid.c_cyan, true);
				gs.SetColorStyle(GameState.ColorStyles.normal);
				
				//create double of lasergrid.
				laserGrid2.gameObject.SetActive(true);
				laserGrid2.SetHeightPattern(LaserGrid.HeightPattern.flat);
				laserGrid2.SetColorAll(laserGrid.c_cyan, true);

				finishSpeed = 120;
				break;

			default:
				break;
			}

			
			laserGrid.StartSpin(intermissionTime, false, finishSpeed);
			if (laserGrid2.gameObject.activeSelf) laserGrid2.StartSpin(intermissionTime, false, finishSpeed);
		}

		CollectedGoldMultItem();


	}


	IEnumerator intermissionToDeactivate(float time) {
		float timer = 0;
		while (timer < time) {
			timer += Time.deltaTime;
			yield return null;
		}
		DeactivateCollectGold();
	}

	public void DeactivateCollectGold() {
		
		newState(levelState.normal);

		//DEACTIVATE BOSS MODE.
		levelStateNormalWithBoss = false;

		NextSlide();

	}



	//--------------------
	//--------------------
	//--------------------
	//---------------------=============================  S W I P I N G   D E T E C T I O N ===============------------------
	//--------------------
	//--------------------
	//--------------------


	public void HitObstacle(string newKillText) {

		nextSlide.transform.FindChild("Quad").GetComponent<GradientBG>().FlashRed(); ///call before detecting swipe so next slide is the current slide and not game over slide


		SwipeDetected(100);

	}



	float RoundToHundredth(float num) {
		//Debug.Log("ROUNDING START!!!! for num = " + num);
		num *= 100;
		//Debug.Log("Num x 100 = " + num);
		num = Mathf.Round(num);
		//Debug.Log("Num rounded = " + num);
		num /= 100;
		//Debug.Log("Num / 100 = " + num);

		//Debug.Log("ROUNDING END!!!!");
		return num;

	}


	void RoundMultipliers() {
		scoreMult = RoundToHundredth(scoreMult);
		multiplierAdded = RoundToHundredth(multiplierAdded);

		baseMult = RoundToHundredth(baseMult);
		baseMultAdded = RoundToHundredth(baseMultAdded);
	}


	public void CollectedScoreMultItem() {

		RoundMultipliers();

		scoreMult += multiplierAdded;
		if ((scoreMult - (1 + baseMult)) > multiplierAdded*maxMultLevels) {
			scoreMult -= multiplierAdded;
		}
		SetScoreMultiplier(scoreMult);
		addMessage("<size=70>MULTIPLIER + " + multiplierAdded + "</size>", 0);


		if (ls == levelState.tut_4) {
			didCollectTutorialTetraBlue = true;
		}

		firstTimeCollectingBlue = true;
	}





	void CollectedGoldMultItem() {
		
		RoundMultipliers(); //should be done before any multiplier math is done.
		
		scoreMult -= baseMult; //remove the original base mult from the multiplier

		/// ---------------- This method does the wrok of increassing the base multiplier as well as adjusting the labels.
		IncreaseBaseMult(baseMultAdded, false); 

		// readjust the score multiplier with the new base multiplier.
		scoreMult += baseMult; //add the new base mult.
		SetScoreMultiplier(scoreMult); //set text.


		//if you're in the 6th tutorila, make sure to say that you collected the tutorial tetra, otherwise the tutorial 6 will repeat.
		if (ls == levelState.tut_6) {
			didCollectTutoiralTetraGold = true;
		}

	}

	public void IncreaseBaseMult(float amt, bool setScoreMult) {
		RoundMultipliers();
		
		addMessage("<size=70>PERMANENT + " + amt + "</size>", 2);
		
		baseMult += amt;
		if (baseMult >= 999f) baseMult = 999f;
		gs.baseMult = baseMult;
		gs.Save();
		
		SetScoreBaseMult(baseMult);


		if (setScoreMult)
			SetScoreMultiplier(1.0f + baseMult);
	}



	// --------------- 
	// --------------- 
	// --------------- 
	// --------------- ===============  B O S S    M E T H O D S
	// --------------- 
	// --------------- 
	// --------------- 



	void CreateBoss() {
		//calculate new bossHealth.
		
		//The estimated base multiplier you will need at this point to beat the boss in time.
		float multNeededToKillBossInTime = 1f;
		multNeededToKillBossInTime = Mathf.Pow(((float)currentLevel / 10f), 0.9f);
		if (multNeededToKillBossInTime > 999f) multNeededToKillBossInTime = 999f;

		int swipesNeededToKillBoss = (int)Mathf.Pow(((float)currentLevel), 0.9f);

		long hp = (long)((float)swipesNeededToKillBoss * (float)((float)pointsPerSwipeThisLevel * multNeededToKillBossInTime));
		
		
		//calculate how long the boss should take to kill.
		//how long should one swipe take?
		float timePerSwipe = 0.8f;
		//how much health should you take down per swipe.
		float healthBossShouldLosePerSwipe = (float)pointsPerSwipeThisLevel * multNeededToKillBossInTime;
		//divide by how much health the boss has to get the number of swipes needed to kill it.
		float numberOfSwipesToKillBoss = (float)hp / healthBossShouldLosePerSwipe;
		//multiply number of swipes to kill by estimated time per swipe to get how much time all the swipes should take.
		float timeNeededToKillBoss = timePerSwipe * numberOfSwipesToKillBoss;
		
		currentBoss.CREATE(hp, timeNeededToKillBoss); 


		//show message
		bigMessage.showWithMessage("BOSS");
	}
	
	public void BossDestroyed() {
		
		newState(levelState.collectingGold);

		gs.numCoins += 1;
		gs.Save();
		SetCoins(1);

		GameObject.Find("CoinsCollectText").GetComponent<CoinsCollect>().ShowCoinCollect();

		//NextSlide();

	}
	public void FailedToKillBossInTime() {

		//so you die even if you have a shield.
		ForceQuitGame();

	}






	// --------------- 
	// --------------- 
	// --------------- 
	// --------------- ===============  S W I P E   D E T  E C T I O N    S U C C E S S    F A I L     M E T H O D S
	// --------------- 
	// --------------- 
	// --------------- 


	/// <summary>
	/// Swipes the detected.
	/// </summary>
	/// <param name="direction">Direction.
	/// 0 = up
	/// 1 = right
	/// 2 = down
	/// 3 = left
	/// -1 = tap.
	/// 100 = forced incorrect swipe
	/// 10 = forced correct swipe
	/// </param>
	public void SwipeDetected(int direction) {

		if (ls != levelState.collectingGold) { //if you're collecting.. swipes are meaningless. until you collect it.

			isTouchingScreen = false; ///every swipe creates a new screen so you're not touching it.

			currentArrow = null;

			arrowVelocity = Vector2.zero;
			/// 
			///// this will have reset all touching variables.!!!! once and for all.


			//----------------------------------

			if (!Advertisement.isShowing) { ///only allow input if and AD is not showing.

				if (!incorrectSwipe) { ///only get swipes if it's not game over.


					bool gameOver = direction != curExpectedSwipeDir; //game over condition.

					//before doing anything. tell currect background to deactivate obstacles. and send them to the background.
					nextSlide.GetComponent<BackgroundSlide>().DeactivateObstacles(gameOver);


					if (!gameOver) {
						successSwipe(direction);
					}

					else {
						failedSwipe();
					}

				} ///end if not game over. incorrect swipe


			} ///end if ad is not showing.

		}//end if not collecting gold tetra.


	}


	/// <summary>
	/// IF you failed a swipe. Called when swiping in the wrong direction, or when hitting an obstacle.
	/// </summary>
	void failedSwipe() {

		//reset consecutive correct swipes.
		consecutiveCorrectSwipes = 0;
		
		SetCombo(consecutiveCorrectSwipes);


		//You can only lose the level state is correct.
		if (ls == levelState.normal) { 
			if (hasMultiplier) { ///you currently have multipliers.
				//lose the multiplier.
				RemoveScoreMultiplier();

				gs.ac.PlaySFX(sfx_fail);

				NextSlide ();

			}
			else { /// you fail a swipe without a multiplier.. so game over.


				gs.ac.PlaySFX(sfx_gameover);

				incorrectSwipe = true;

				/*
				 * You should only watch the continue ad if:
				 * 1. you have not previously watched the continue ad.
				 * 2. you are above level 10.
				 */
				bool shouldWatchContinueAd = (!watchedContinueAd && currentLevel > 10);


				// check whether to show the continue message or not for watching an ad.
				if (shouldWatchContinueAd) { //if you've already watched the continue ad this game.
					StartCoroutine(StartContinue()); ///continue decides whether or not to show the game over screen or reset incorrect swipe to false and continue playing.
				}
				else {
					NextSlide();
				}


			}
		}

		else { //the level state is not normal... so check which tutorial and what to do.

			if (ls == levelState.tut_5) { // ------ if you mess up in tutorial 5, you're supposed to hit the laser. go on.
				gs.tut_5 = true;
				newState(levelState.tut_6);
				//lose the multiplier.
				RemoveScoreMultiplier();
			}
			if (ls == levelState.tut_1) {
				gs.tut_1 = true;
				newState(levelState.tut_2); ///even if you mess up on tut 1, go to tutorial 2.
			}
			gs.ac.PlaySFX(sfx_fail);

			NextSlide ();

		}



		///other effects.
		gs.screenShakeLarge();


	}
	


	/// <summary>
	/// When you swipe correctly, as well as successfully having dodged any lasers.
	/// </summary>
	/// <param name="direction">Direction.</param>
	void successSwipe(int direction) {
		
		//only add scores when not in tutorials, otherwise when in a tutorial, check which tutorial and check them off appropriately.
		if (ls != levelState.tut_1 && ls != levelState.tut_2 && ls != levelState.tut_3 && ls != levelState.tut_4 && ls != levelState.tut_5 && ls != levelState.tut_6 && ls != levelState.tut_7 && ls != levelState.tut_8 && ls != levelState.tut_9 && ls != levelState.tut_10)
			AddScore();
		else
			CheckTutorials();


		//VARIOUS SPECIAL EFFECTS AND SOUNDS WHEN GETTING CORRECT SWIPE
		AudioClip success = sfx_success1;
		int rand = Random.Range(1,101);
		if (rand >= 33 && rand <= 66)
			success = sfx_success2;
		else if (rand > 66)
			success = sfx_success3;
		gs.ac.PlaySFX(success);

		//create circle effect.
		StartCoroutine(MakeSuccessSwipeCircles());

		//make swipe flash effect.
		GameObject bar;
		GameObject clone;
		
		if (curExpectedSwipeDir == 0 || curExpectedSwipeDir == 2) {
			bar = GameObject.Find("swipeTall");
			clone = Instantiate(bar, new Vector3(arrow.transform.position.x, 6.67f, 0), Quaternion.identity) as GameObject;
		}
		else {
			bar = GameObject.Find("swipeLong");
			clone = Instantiate(bar, new Vector3(3.75f, arrow.transform.position.y, 0), bar.transform.localRotation) as GameObject;
		}
		clone.GetComponent<SwipeFlash>().enabled = true;

		//----==== shake the screen.
		gs.screenShakeNormal();


		//--- LASTLY !
		//--------------- SEND IN THE NEXT SLIDE...		
		NextSlide();
	}

	//Makes it so 3 success circles show up, wherever the arrow hit the side.
	IEnumerator MakeSuccessSwipeCircles() { 
		GameObject f = Resources.Load<GameObject>("Prefabs/_Effect_Linerenderer_Circle");
		if (levelStateNormalWithBoss && !currentBoss.zooming)
			f = Resources.Load<GameObject>("Prefabs/_Effect_Linerenderer_Circle_RED");
		Vector3 pos = arrow.transform.position;
		for (int i=0; i<3; i++) {
			GameObject circleFlash = Instantiate(f, pos, Quaternion.identity) as GameObject;
			yield return new WaitForSeconds(0.1f);
		}
	}


	
	/// <summary>
	/// Adds score to your total, checks if records are broken.
	/// Takes care of the leveling up system as well as score multipliers
	/// And how much to increase score multiplier collections.
	/// </summary>
	void AddScore() {


		//add a consecutive swipe.
		consecutiveCorrectSwipes += 1;
		//check achievements -------------
		if (consecutiveCorrectSwipes == 10) {
			gs.social.AttemptToReportAchievement(SocialAssets.Achievement_ID_Consec_10, 100, true);
		}
		else if (consecutiveCorrectSwipes == 25) {
			gs.social.AttemptToReportAchievement(SocialAssets.Achievement_ID_Consec_25, 100, true);
		}
		else if (consecutiveCorrectSwipes == 50) {
			gs.social.AttemptToReportAchievement(SocialAssets.Achievement_ID_Consec_50, 100, true);
		}
		//------------------
		
		SetCombo(consecutiveCorrectSwipes);





		float pointsAdded = (float)pointsPerSwipeThisLevel * scoreMult;
		/// NOW SCORE TO ADD IS REPRESENTATIVE OF THE CURRENT LEVEL AND THE CURRENT SCORE MULTIPLIER.


		///------ SEE WHAT SCORE IS ADDED BASED ON SWIPE TIME...
		//if you score in the less than the fast swipe time, you  get full points.
		if (swipeTimer > fastSwipeTime) {
			//calculate how long since fastswipetime it's been.
			float newTime = swipeTimer - fastSwipeTime;
			pointsAdded /= (1 + newTime); //otherwise get points equal to the output of score/1+time .. which is a 1/x graph.
			if (pointsAdded <= initialPoints)
				pointsAdded = initialPoints; ///can't get less than initial points.
		}
		else { //got the swipe fast enough... increase consecutive swipes.
			consecutiveFastSwipes += 1;
		}
		//THIS WAY: FASTER PLAYERS LEVEL UP FASTER...

		///SHOW THE SCORE ON SCREEN.
		/// CHECK HOW TO SHOW THE SCORE BASED ON HOW BIG THE NUMBER IS
		long pointsAddedLong = (long)pointsAdded;

		string scoreMessageString = formatScoreToString(pointsAddedLong);

		addMessage("+" + scoreMessageString, 1);

		/// -----


		/// ---------- ADD TO TOTAL SCORE....


		realScore += pointsAddedLong;
		if (realScore >= 999999999999999999)
			realScore = 999999999999999999;

		if (realScore > gs.highScores[0].score) { //---- check i fyou beat your score.
			scoreText.color = Color.yellow;
			gs.AddHighScore(realScore); //-------- Add high score
		}

		SetScore(realScore);




		//---- C O R R E E C T   S W I P E S
		
		correctSwipes += 1;

		//MAKE CALLS TO GAME STATE TO SEE IF NEW RECORD NEEDS TO BE ADDED.
		if (correctSwipes > gs.numSwipesOneGame) {
			gs.numSwipesOneGame = correctSwipes;
		}

		gs.numSwipes += 1;




		if (!levelStateNormalWithBoss) { ///level up possible when not in boss fight.

			//------------------=============================  L E V E L I N G   U P 

			//initialize points

			pointsToNextLevel = 0;
			int screensForNextLevel = 0;


			//Accumulated points for this current level --- gets reset when level up.
			pointsGottenThisLevel += pointsAddedLong;
				
			//how many FAST screens/swipes needed to reach the next level
			screensForNextLevel = (currentLevel+1); 

			//points needed to level up again.
			pointsToNextLevel = pointsPerSwipeThisLevel * (long)screensForNextLevel;

			//check progress of current level, range 0 to 1.
			float percentOfLevel = (float)pointsGottenThisLevel / (float)pointsToNextLevel;
			if (percentOfLevel >= 1f) percentOfLevel = 1f;
			SetLevel(currentLevel, percentOfLevel); //----- set the level meter to reflect the percentage.

			
			//// L E V E L   U P ! ! ! ! 
			
			if (pointsGottenThisLevel >= pointsToNextLevel) { //when you've reached enough points.
				
				//increase level... set text.
				
				currentLevel++;
				if (currentLevel > 9999) currentLevel = 9999;

				SetLevel(currentLevel, 0);
				
				if (currentLevel > gs.highestLevel) {
					gs.highestLevel = currentLevel;
					gs.Save();
				}

				pointsGottenThisLevel = 0; ///reset how many points got every level.
				//---calculate score gotten per swipe for this NEW level
				int oneLess = currentLevel-1;
				if (oneLess < 1) oneLess = 1;
				pointsPerSwipeThisLevel = (long)((currentLevel * oneLess * scoreLevelUpMult) + initialPoints); ///------the points you get at this level.

				
				//play level up sound
				gs.ac.PlaySFX(sfx_levelUp, 1f);
				
				
				//show message
				addMessage("LEVEL UP!", 2);
				
				
				/**
					 * 
					 * 
					 * CHECK TO INCREASE MULTIPLIERS FOR GOLD TETRAS !!
					 * 
				*/
				if (currentLevel % 10 == 5) { //every (multiple of 5 and not 10) levels , double the multiplier added.
					if (currentLevel == 5)
						baseMultAdded = 0.01f;
					else 
						baseMultAdded *= 2;
					if (baseMultAdded >= 10.24f)
						baseMultAdded = 10.24f;
				}
			}

		} /// -- end if it was not a boss phase.

		else { // it is a boss phase. can't level up.. but points damage boss

			currentBoss.TakeDamage(pointsAddedLong);

		}


	}




	void CheckTutorials() {

		//-------- C H E C K   W H E T H E R   A   T U T O R I A L  W A S  B E A T E N
		
		switch(ls) {
		case levelState.tut_1:
			newState(levelState.tut_2);
			gs.tut_1 = true;
			break;
			
		case levelState.tut_2:
			newState(levelState.tut_3);
			gs.tut_2 = true;
			
			break;
			
		case levelState.tut_3:
			newState(levelState.tut_4);
			gs.tut_3 = true;
			break;
			
		case levelState.tut_4:
			if (didCollectTutorialTetraBlue) {
				newState(levelState.tut_5);
				gs.tut_4 = true;
			}
			break;
			
		case levelState.tut_5:
			newState(levelState.tut_6);
			gs.tut_5 = true;
			break;
			
		case levelState.tut_6:
			if (didCollectTutoiralTetraGold) {
				newState(levelState.tut_7);
				gs.tut_6 = true;
			}
			else {
				newState(levelState.tut_6); ///set this so that the previous state is tut_6 as well. so not to spawn another gold tetra.
			}
			break;
			
		case levelState.tut_7:
			newState(levelState.tut_8);
			gs.tut_7 = true;
			break;
			
		case levelState.tut_8:
			newState(levelState.tut_9);
			gs.tut_8 = true;
			break;
			
		case levelState.tut_9:
			newState(levelState.tut_10);
			gs.tut_9 = true;
			break;
			
		case levelState.tut_10:
			newState(levelState.normal);
			gs.tut_10 = true;
			tutText.text = "";
			tutText.gameObject.SetActive(false);

			gs.social.AttemptToReportAchievement(SocialAssets.Achievement_ID_Tutorial, 100, true);

			break;
		}
	}




	//----------------
	//----------------
	//----------------
	//----------------========================  B A C K G R O U N D   A D D I N G   M E T H O D S   ========--------------
	//----------------
	//----------------
	//----------------





	void NextSlide() {
		if (!incorrectSwipe) {
			///generate new background.
			AddBackground();
			MoveBackground();
		}
		else {
			//check to show the power up ad.
			CheckToShowPowerUpAd();

			newState(levelState.normal);
			ShowGameOver(gs.isFirstTimePlaying);
		}
	}


	
	public void MoveBackground() {
		
		_timeStartedMovingBackground = Time.time;

		backgroundMoving = true;

	}



	
	void NextSlideDoneEntering() {
		//cleanup ... remove slide, reset timer, and say the background isn't moving...
		backgroundMoving = false;
		swipeTimer = 0.0f; //---- reset the swipe timer every time a new slide comes in.
		if (!incorrectSwipe) { //if the game isn't over.. destroy the previous slide...... and activate obstacles on the next slide.
			GameObject.Destroy(prevSlide);


			if (ls == levelState.normal) {

				if (currentLevel % 5 == 0 && currentLevel > 0 && !didCollectGold) { //level is multiple of 5,
					didCollectGold = true;

					//GameObject g = Resources.Load<GameObject>("Prefabs/_Collectables/_tetra_gold");
					//GameObject gold = Instantiate(g, new Vector3(3.75f, 9, 20), Quaternion.identity) as GameObject;
					tetraGen.GetComponent<TetraGen>().enabled = false;

					//newState(levelState.collectingGold);
					levelStateNormalWithBoss = true;


					//add a boss to the screen. //the boss class does what it needs to do.

					if (currentBoss == null) { //creates the boss for the first time.
						GameObject boss = Resources.Load<GameObject>("Prefabs/_BOSS/BOSS_SPHERE");
						GameObject b = Instantiate(boss) as GameObject;
						currentBoss = b.GetComponent<BOSS>();
						CreateBoss();
					}
					else {
						CreateBoss();
					}

					//when boss is killed, immediatey change level state to collecting gold, while the boss is zooming out to death.


				}
				else { //normal level not multiple of 5 or did already collect gold tetra.
					if (currentLevel % 5 != 0) didCollectGold = false; //reset to false once level is not multiple of 5.

					if (!levelStateNormalWithBoss) { //if there is no boss, ok to put obstacles and tetras.
						tetraGen.GetComponent<TetraGen>().enabled = true;
						nextSlide.GetComponent<BackgroundSlide>().ActivateObstacles(currentLevel, curExpectedSwipeDir);
					}
				}
			}
			else {
				if (ls == levelState.tut_1) {
					tutText.gameObject.SetActive(true);
					tutText.text = "Touch anywhere to start moving.";
				}
				else if (ls == levelState.tut_2) {
					tutText.gameObject.SetActive(true);
					tutText.text = "Move the arrow to the side it's pointing.";
				}
				else if (ls == levelState.tut_3) {
					gs.Save(); ////-------------------------- S A V E
					tutText.gameObject.SetActive(true);
					tutText.text = "Be careful of lasers!";
					GameObject t = Resources.Load<GameObject>("Prefabs/Tutorial_Laser");
					GameObject tut = Instantiate(t);
					tut.transform.SetParent(nextSlide.transform);
					if (curExpectedSwipeDir == 3)
						tut.transform.localScale = new Vector2(-tut.transform.localScale.x, tut.transform.localScale.y);
				}
				else if (ls == levelState.tut_4) {
					tutText.gameObject.SetActive(true);
					tutText.text = "Blue Tetras increase your game score multiplier, and give you a shield.";
					GameObject t = Resources.Load<GameObject>("Prefabs/Tutorial_Tetra");
					GameObject tut = Instantiate(t);
					tut.transform.SetParent(nextSlide.transform);
				}
				else if (ls == levelState.tut_5) {
					tutText.gameObject.SetActive(true);
					tutText.text = "Breaking your shield will drop your score multiplier.";
					GameObject t = Resources.Load<GameObject>("Prefabs/Tutorial_Laser_Tetra");
					GameObject tut = Instantiate(t);
					tut.transform.SetParent(nextSlide.transform);
					if (curExpectedSwipeDir == 3) {
						tut.transform.localScale = new Vector2(-tut.transform.localScale.x, tut.transform.localScale.y);
					}
					GameObject tet = Resources.Load<GameObject>("Prefabs/_COLLECTABLES/_tetra");
					Instantiate(tet, new Vector3(3.75f, 2.5f, 10), Quaternion.identity);
				}
				else if (ls == levelState.tut_6) {
					tutText.gameObject.SetActive(true);
					tutText.text = "Gold Tetras increase your permanent score multiplier. This multiplier never goes down, even after restarting.";
					if (pls != levelState.tut_6) {
						GameObject tet = Resources.Load<GameObject>("Prefabs/_COLLECTABLES/_tetra_gold");
						Instantiate(tet, new Vector3(3.75f, 2.5f, 10), Quaternion.identity);
					}
				}
				else if (ls == levelState.tut_7) {
					tutText.gameObject.SetActive(true);
					tutText.text = "Get Ready!";
				}
				else if (ls == levelState.tut_8) {
					tutText.gameObject.SetActive(true);
					tutText.fontSize = 80;
					tutText.text = "3";
				}
				else if (ls == levelState.tut_9) {
					tutText.gameObject.SetActive(true);
					tutText.fontSize = 80;
					tutText.text = "2";
				}
				else if (ls == levelState.tut_10) {
					gs.Save(); //---------------------------- S A V E  
					tutText.gameObject.SetActive(true);
					tutText.fontSize = 80;
					tutText.text = "1";
				}
			}
		
		}
		
	}





	public void AddBackground() {

		if (nextSlide) {
			prevSlide = nextSlide;
			prevSlide.transform.position = new Vector3(prevSlide.transform.position.x, prevSlide.transform.position.y, 10);
		}


		Vector3 nextBackgroundPos = new Vector3(0,0,0);

		
		switch(curExpectedSwipeDir) {  /////move the next incoming background based on the current swipe direction.
			
		case 0:
			nextBackgroundPos = new Vector3(gs.winWidth/2, -(gs.winHeight/2), 0);
			break;
			
		case 1:
			nextBackgroundPos = new Vector3(-(gs.winWidth/2), gs.winHeight/2, 0);
			break;
			
		case 2:
			nextBackgroundPos = new Vector3(gs.winWidth/2, (gs.winHeight/2)*3, 0);
			break;
			
		case 3:
			nextBackgroundPos = new Vector3((gs.winWidth/2)*3, gs.winHeight/2, 0);
			break;
			
			
		}

		
		GameObject clone = null;






		///create a normal background bc the game is still going on.


		/**
		 * 
		 * S E T U P    O F    T H E    S L I D E    C O M I N G   I N 
		 * 
		 */

		//// set the new expected direction. and set up the next slide below.


		
		
		int nextSwipeDirection = Random.Range(0,4); // 0 = up, 1 = right, 2 = down, 3 = left.


		if (ls == levelState.tut_3 || ls == levelState.tut_5) {
			nextSwipeDirection = 1;
			if (Random.Range(1,3) % 2 == 0)
				nextSwipeDirection = 3;
		}


		curExpectedSwipeDir = nextSwipeDirection;



		if (firstSlide) {
			firstSlide = false;
			nextBackgroundPos = new Vector3(gs.winWidth/2, gs.winHeight/2, 0f);
			//Debug.Log("creating background at " + gs.winWidth/2 + " and y= " + gs.winHeight/2);
		}
		else { //this coming slide is not the first one.
			nextSlide.transform.FindChild("Quad").GetComponent<GradientBG>().SendRenderToBack(); ///send the slide backwards so the incoming one can be seen.
		}

		GameObject bgSlide = GameObject.Find("BackgroundSlide");

		clone = Instantiate(bgSlide, nextBackgroundPos, Quaternion.identity) as GameObject;

		nextSlideStartPos = nextBackgroundPos;
		nextSlide = clone;

		//arrow = nextSlide.transform.FindChild("ArrowDown").gameObject;
		arrow = Instantiate(Resources.Load<GameObject>("Prefabs/ArrowDown")) as GameObject;
		arrow.transform.SetParent(nextSlide.transform);
		arrow.transform.localPosition = new Vector3(0,0,0);

		//init the shield, whether or not to show it.
		shield = arrow.transform.FindChild("_shield").gameObject;
		if (!hasMultiplier) {
			RemoveShield();
		}


			
		Vector3 lightPos = Vector3.zero;
		GameObject newLight = null;

		if (nextSlide != null) {

			GameObject sidelight = GameObject.Find("SideLight");
			Quaternion lightRot = Quaternion.identity;

			switch(nextSwipeDirection) { ////dont worry about reverse swipes here. since these should be to fool you in that situatoin.

			case 0:
				arrow.transform.localRotation = Quaternion.Euler(0,0,180);
				lightPos.x = 0;
				lightPos.y = 6.67f;
				lightRot = Quaternion.Euler(new Vector3(0,0,90));
				break;
				
			case 1:
				arrow.transform.localRotation = Quaternion.Euler(0,0,90);
				lightPos.x = 3.75f;
				lightPos.y = 0f;
				break;

			case 2:
				lightPos.x = 0f;
				lightPos.y = -6.67f;
				lightRot = Quaternion.Euler(new Vector3(0,0,90));
				break;
				
			case 3:
				arrow.transform.localRotation = Quaternion.Euler(0,0,-90);
				lightPos.x = -3.75f;
				lightPos.y = 0f;
				break;


			

			}

			//create the blinking side light.
			newLight = Instantiate(sidelight, lightPos, lightRot) as GameObject;
			newLight.transform.SetParent(nextSlide.transform);
			newLight.transform.localPosition = lightPos;
			newLight.GetComponent<FadeInOut>().enabled = true; //enable the script. since the side light had it disabled.






		}//// end if next slide != null..


		///make a noise for the background entering...


		if (!firstSlide)
			gs.ac.PlaySFX(sfx_woosh, 1.0f);




	}



	//---------------------------
	//---------------------------
	//---------------------------
	//-----------------------------------------==========   U P D A T E ========----------------------------------
	//---------------------------
	//---------------------------
	//---------------------------



	void Update() {

		if (!Advertisement.isShowing) {


			if (!(ls == levelState.paused)) { //// IF THE GAME IS NOT PAUSED....


				// - - - - - -- - - - - - - - - - - - - - - SHOW THE LAST MESSAGE IN THE QUEUE, THEN REMOVE IT. SET TO TIMERS.

				showMessageTimer += Time.deltaTime;
				if (showMessageTimer >= showMessageTime) {
					showMessageTimer = 0;
					if (scoreAddedQueue.Count > 0) {
						messageToShow m = scoreAddedQueue.Last();
						ShowMessage(m.m, m.color);
						scoreAddedQueue.Remove(m);
					}

				}
				
				//-------------------------------------------------------------------------------------------


				if (backgroundMoving) {
					
					float timeSinceStarted = Time.time - _timeStartedMovingBackground;
					float percentageComplete = timeSinceStarted / 0.25f; //the denominator here is how long it should take.
					
					
					Vector3 nextSlideEndPos = new Vector3(gs.winWidth/2,gs.winHeight/2,0);
					nextSlide.transform.position = Vector3.Lerp(nextSlideStartPos, nextSlideEndPos, percentageComplete);
					
					if (nextSlide.transform.position == nextSlideEndPos) {
						
						NextSlideDoneEntering();
						
					}
				}
				
				
				else { ////what to do only when the slide is in the frame and not moving in or out.

					if (!incorrectSwipe) {


						/*-----------------------------------------------
						// MAKING THE ARROW BOUNCE! ...... AND TESTING MOVEMENT
						
						-----------------------------------------------
						* CHECKINT THE ARROW HITTING THE SIDES
						* create a small buffer for when the arrow hit the wrong side.
						* it should be able to go slightly further into the wrong side to give the
						* player the benefit of the doubt when the arrow touches the corners.
						 */
						if (arrow != null) { ///arrow is the actual arrow for this slide.
							
							Vector2 pos = arrow.transform.localPosition;

							float bufferZoneForPlayerError = 0.5f;
							float rightSide = gs.winWidth/2 + bufferZoneForPlayerError;
							float leftSide = -gs.winWidth/2 - bufferZoneForPlayerError;
							float topSide = gs.winHeight/2 + bufferZoneForPlayerError;
							float bottomSide = -gs.winHeight/2 - bufferZoneForPlayerError;

							switch(curExpectedSwipeDir) { ///which side to take the buffer off.
							case 1:
								rightSide -= bufferZoneForPlayerError;
								break;
							case 2:
								bottomSide += bufferZoneForPlayerError;
								break;
							case 3:
								leftSide += bufferZoneForPlayerError;
								break;
							case 0:
								topSide -= bufferZoneForPlayerError;
								break;
							default:
								break;
							}


							if (pos.x > rightSide) {
								pos.x = rightSide;
								SwipeDetected(1);
							}
							else if (pos.y > topSide) {
								pos.y = topSide;
								SwipeDetected(0);
							}
							else if (pos.y < bottomSide) {
								pos.y = bottomSide;
								SwipeDetected(2);
							}
							else if (pos.x < leftSide) {
								pos.x = leftSide;
								SwipeDetected(3);
							}

							if (ls == levelState.collectingGold) arrow.transform.localPosition = pos;
							
						}
						if (currentArrow != null) { //current arrow is the arrow recognized by the last touch began.
							
							
							/// ------------------------------ IF YOURE TOUCHING THE SCREEN AND NOT MOVING.. SET VELOCITY TO 0.
							if (isTouchingScreen) {
								touchTime += Time.deltaTime;
								
								if (touchTime > stopVelocityTime) { /// this should work for standing still bc if they move large enough amount. it will set the velocity and this timer.
									arrowVelocity = Vector2.zero;
									lastMoveDelta = Vector2.zero;
								}
							}
							
							else { ///only when not touching screen.
								// ----------------------------- ADD ARROWS VELOCITY TO ITS POSITION
								Vector2 pos = currentArrow.transform.localPosition;
								pos += arrowVelocity;

								//slow down velocity.
								float accel = 0.01f;

								if (arrowVelocity.x <= accel && arrowVelocity.x >= -accel)
									arrowVelocity.x = 0;
								else {
									if (arrowVelocity.x > accel)
										arrowVelocity.x -= accel;
									else if (arrowVelocity.x < -accel)
										arrowVelocity.x += accel;
								}

								if (arrowVelocity.y <= accel && arrowVelocity.y >= -accel)
									arrowVelocity.y = 0;
								else {
									if (arrowVelocity.y > accel)
										arrowVelocity.y -= accel;
									else if (arrowVelocity.y < -accel)
										arrowVelocity.y += accel;
								}



								currentArrow.transform.localPosition = pos;

							}
							
						}


						//////- - - - - - - - - - - - -  T I M E R   S T U F F 
						swipeTimer += Time.deltaTime;

					}

					else { ////INCORRECT SWIPE WAS SET TO TRUE BY SWIPING WRONG OR FROM HITTING AN OBSTACLE.
					

						
					}//end if game over.

			
				} //// end if background is not moving.


			}//end if not paused.


			else { ///// IF THE GAME IS PAUSED.......


				if (!setSoundButton) {


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

		} //// END IF AD IS NOT SHOWING....

	} //-- END UPDATE METHOD..

} //-- END CLASS
