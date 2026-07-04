using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
//using UnityEngine.SocialPlatforms;


///<summary>
///<para>Scene:All/NameOfScene/NameOfScene1,NameOfScene2,NameOfScene3...</para>
///<para>Object:N/A</para>
///<para>Description: Sample Description </para>
///</summary>
public class GlobalVariables : MonoBehaviour {

	public static string applicationID;

	public static bool quitGame;

	public static bool removeAdsOwned = false;

	public static bool shouldShowLeaderboard = false;

	public static bool startInterstitialShown;

	public static GlobalVariables globalVariables;

	void Awake()
	{
        applicationID = "com.Test.Package.Name";
		startInterstitialShown = false;

		shouldShowLeaderboard = false;

		if (!PlayerPrefs.HasKey("ScoreOfTheHighes"))
		{
			PlayerPrefs.SetInt("ScoreOfTheHighes", 0);
			PlayerPrefs.Save();
		}

//		// Start services
//		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
//			// enables saving game progress.
//			.EnableSavedGames()
//			// registers a callback to handle game invitations received while the game is not running.
//			.WithInvitationDelegate()
//			// registers a callback for turn based match notifications received while the
//			// game is not running.
//			.WithMatchDelegate()
//			// require access to a player's Google+ social graph (usually not needed)
//			.Build();
//
//		PlayGamesPlatform.InitializeInstance(config);
//		// recommended for debugging:
//		PlayGamesPlatform.DebugLogEnabled = true;
//		// Activate the Google Play Games platform
//		PlayGamesPlatform.Activate();

//		PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
			// enables saving game progress.
//			.EnableSavedGames()
			// registers a callback to handle game invitations received while the game is not running.
			//.WithInvitationDelegate(delegate() {})
			// registers a callback for turn based match notifications received while the
			// game is not running.
			//.WithMatchDelegate(delegate() {})
			// require access to a player's Google+ social graph (usually not needed)
			// .RequireGooglePlus()
//			.Build();

//		PlayGamesPlatform.InitializeInstance(config);
		// recommended for debugging:
		//FIXME UPDATE NEMA PLAYSERVISA
//		#if UNITY_ANDROID
//		PlayGamesPlatform.DebugLogEnabled = true;
//		// Activate the Google Play Games platform
//		PlayGamesPlatform.Activate();
//		#endif

		globalVariables = this;
		DontDestroyOnLoad(this);
	}

	void Start()
	{
//		PlayGamesPlatform.DebugLogEnabled = true;

//		GooglePlayGames.PlayGamesPlatform.Activate ();

		//FIXME UPDATE NEMA PLAYSERVISA
//		#if UNITY_ANDROID
//		// Sign in
//		Social.localUser.Authenticate((bool success) => {
//			// handle success or failure
//			if (success)
//				shouldShowLeaderboard = true;
//			else
//				shouldShowLeaderboard = false;
//		});
//		#endif
	}

	//FIXME UPDATE NEMA PLAYSERVISA
	public void PostScoreToLeaderboard(int score)
	{
//		#if UNITY_ANDROID
//		if (PlayGamesPlatform.Instance.IsAuthenticated())
//		{
//			Social.ReportScore(score, "CgkI48GU27cFEAIQAA", (bool success) => {
//				// handle success or failure
//			});
//		}
//		#endif
	}

	//FIXME UPDATE NEMA PLAYSERVISA
	public void ShowLeaderboard()
	{
//		#if UNITY_ANDROID
//		if (PlayGamesPlatform.Instance.IsAuthenticated())
//		{
//			// Ako hocemo ceo UI
////			Social.ShowLeaderboardUI();
//
//			// Ako hocemo samo za ovaj leaderboard
//			PlayGamesPlatform.Instance.ShowLeaderboardUI("CgkI48GU27cFEAIQAA");
//		}
//		#endif
	}

	public void DisableLog(string msg)
	{
		Debug.unityLogger.logEnabled = false;
	}
}
