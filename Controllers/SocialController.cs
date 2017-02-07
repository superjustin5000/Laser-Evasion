using UnityEngine;
using System.Collections;


//Game Center
using UnityEngine.SocialPlatforms;
using UnityEngine.SocialPlatforms.GameCenter;
using System.Runtime.InteropServices;


public class SocialController {

	GameState gs;

	bool attemptingToAuthenticateSocial = false;
	bool socialAuthentiacted = false;

	bool updatedSocialHighScore = false;
	

	// Use this for initialization
	public  SocialController () {

		gs = GameState.sharedGameState;
	
		AttemptAuthentication();
		GameCenterPlatform.ShowDefaultAchievementCompletionBanner(true);

	}

	public void Update() {


		/* Check whether the local user is authenticate.
		 * if not, try to authenticate
		 * other wise perform necessary authenticated updates.
		 */
		if (!socialAuthentiacted) {
			AttemptAuthentication();
		}

		else {

			if (!updatedSocialHighScore)
				AttemptToReportHighScore();

		}

	}


	/// <summary>
	/// SETS ALL BOOLS TO FALSE SO THAT EVERYTHING NEEDS TO RETRY TO CONNECT AND SUBMIT.
	/// </summary>
	public void UnAuthenticate() {
		socialAuthentiacted = false;
		updatedSocialHighScore = false;
	}

	
	
	void AttemptAuthentication() {
		if (!attemptingToAuthenticateSocial) {
			attemptingToAuthenticateSocial = true;
#if UNITY_IOS
			Social.localUser.Authenticate(AuthenticateSocial);
#endif
		}
	}
	void AuthenticateSocial(bool success) {
		#if UNITY_IOS
		if (success) {
			//if (Social.localUser.userName.Length > 0 && Social.localUser.userName != "Lerpz")
			//	gs.username = Social.localUser.userName;
			
			socialAuthentiacted = true; //indicates being logged into game center.
		}
		else {
			socialAuthentiacted = false; //unauthenticate the local user.
		}
		#endif

		attemptingToAuthenticateSocial = false;
	}
	
	public void showGameCenterScoresRequest() {
		#if UNITY_IOS
		Social.localUser.Authenticate(showGameCenterScores);
		#endif
	}
	
	void showGameCenterScores(bool success) {
		#if UNITY_IOS
		if (success) {
			Social.CreateLeaderboard();
			Social.CreateLeaderboard().id = SocialAssets.Leaderboard_HighScore;
			Social.ShowLeaderboardUI();
		}
		#endif
	}
	
	
	public void showGameCenterAchievementsRequest() {
		#if UNITY_IOS
		Social.localUser.Authenticate(showGameCenterAchievements);
		#endif
	}
	void showGameCenterAchievements(bool success) {
		#if UNITY_IOS
		if (success) {
			Social.ShowAchievementsUI();
		}
		else {
			Debug.Log("failed to show achievements ui");
		}
		#endif
	}
	
	
	
	
	
	
	
	void AttemptToReportHighScore() {
		#if UNITY_IOS
		Social.localUser.Authenticate (ReportScore);
		#endif
	}
	
	
	void ReportScore (bool success) {
		#if UNITY_IOS
		if (success) {
			//Debug.Log ("Authentication successful report score");
			Social.CreateLeaderboard();
			
			Social.CreateLeaderboard().id = SocialAssets.Leaderboard_HighScore;
			Social.ReportScore(gs.highScores[0].score, SocialAssets.Leaderboard_HighScore,  worked => {
				if (worked) {
					updatedSocialHighScore = true;
					//report the score for the current user.
					long score = gs.highScores[0].score;
					Debug.Log ("Successfully reported high score" + score); 
				}
				else {
					//Debug.Log ("Failed to report high score");});
				}
				
			});
			//if you want uncomment below to show leaderboard!  
			//Social.ShowLeaderboardUI();
		}
		else {
			//Debug.Log ("Failed to authenticate while reporting score.");
		}
		
		#endif
	}
	
	
	
	
	
	
	void AttemptToReadAchievements() {
		
		Social.localUser.Authenticate(ReadAchievements);
		
	}
	void ReadAchievements(bool success) {
		
		if (success) {
			
			Social.LoadAchievements (achievements => {
				if (achievements.Length > 0) {
					Debug.Log ("Got " + achievements.Length + " achievement instances");
					string myAchievements = "My achievements:\n";
					foreach (IAchievement achievement in achievements)
					{
						myAchievements += "\t" + 
							achievement.id + " " +
								achievement.percentCompleted + " " +
								achievement.completed + " " +
								achievement.lastReportedDate + "\n";
					}
					Debug.Log (myAchievements);
				}
				else
					Debug.Log ("No achievements returned");
			});
			
			
		}
		
	}

	/*
	 * ----- ACHIEVEMENTS
	 * 
	 * 
	 */
	
	public void AttemptToReportAchievement(string id, float percent, bool showCompletionBanner) {
		
		Social.localUser.Authenticate( success => {
			if (success) {
				//COMMENTED BELOW IS THE OLD WAY TO DO IT DIRECTLY THROUGH UNITY. NEVER WORKED.
				/*
				Social.ReportProgress( "70057410", 100, (result) => {
				Debug.Log ( result ? "Reported achievement" : "Failed to report achievement");
				});
				_ReportAchievement("70057410", 100);
				*/
#if UNITY_IOS
				//USING PLUGIN GKAchievementReporter from forum.
				GKAchievementReporter.ReportAchievement(id, percent, showCompletionBanner);
#elif UNITY_ANDROID
				//ANDROID STUFF HERE.
#endif
			}
			else
				Debug.Log ("Failed to authenticate");
		});
		
	}


	//[DllImport("__Internal")]
	//private static extern void _ReportAchievement( string achievementID, float progress );




}



public class SocialAssets {

	//LEADERBOARDS....
	public const string Leaderboard_HighScore = "LeaderBoardHighScore";


	//ACHIEVEMENTS....
	//tutorial
	public const string Achievement_ID_Tutorial = "A_Swiper_000";

	//bosses
	public const string Achievement_ID_Boss_1 = "A_Swiper_020";
	public const string Achievement_ID_Boss_3 = "A_Swiper_021";
	public const string Achievement_ID_Boss_7 = "A_Swiper_022";
	public const string Achievement_ID_Boss_11 = "A_Swiper_023";
	public const string Achievement_ID_Boss_13 = "A_Swiper_024";
	public const string Achievement_ID_Boss_14 = "A_Swiper_025";
	public const string Achievement_ID_Boss_16 = "A_Swiper_026";

	//songs
	public const string Achievement_ID_Song_Driver = "A_Swiper_001";
	public const string Achievement_ID_Song_Sentinels = "A_Swiper_003";
	public const string Achievement_ID_Song_Invaders = "A_Swiper_002";
	public const string Achievement_ID_Song_AlmostThere = "A_Swiper_004";
	//score multipliers
	public const string Achievement_ID_ScoreMult_2 = "A_Swiper_005";
	public const string Achievement_ID_ScoreMult_5 = "A_Swiper_006";
	public const string Achievement_ID_ScoreMult_20 = "A_Swiper_008";
	public const string Achievement_ID_ScoreMult_50 = "A_Swiper_007";
	//consecutive swipes
	public const string Achievement_ID_Consec_10 = "A_Swiper_010";
	public const string Achievement_ID_Consec_25 = "A_Swiper_011";
	public const string Achievement_ID_Consec_50 = "A_Swiper_009";

}
