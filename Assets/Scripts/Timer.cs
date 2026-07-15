using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Scene: MiniGame_3
/// Object:N/A
/// Description: Used for counting down time in pop the cat min game
/// </summary>

public class Timer : MonoBehaviour {

	public Text timer;
	public float minutes = 0;
	public float seconds = 20;
	float miliseconds = 0;

	string secondsString;
	string milisecondsString;

	public bool gameoverCalled;

	public bool gamePaused;

	public bool videoAvailableChecked;

	public static Timer instance;

	Text gameOverCoinText;
	Text gameOverStarText;

	void Awake()
	{
		timer = GetComponent<Text>();

		videoAvailableChecked = false;

		gameoverCalled = false;

		gamePaused = false;

		instance = this;
	}

	void FindOrCreateGameOverTexts()
	{
		GameObject popup = LevelManager.levelManager.gameOverPopup;
		if (popup == null) return;

		gameOverCoinText = popup.transform.Find("CoinText")?.GetComponent<Text>();
		gameOverStarText = popup.transform.Find("StarText")?.GetComponent<Text>();

		if (gameOverCoinText == null)
		{
			GameObject coinObj = new GameObject("CoinText");
			coinObj.transform.SetParent(popup.transform, false);
			gameOverCoinText = coinObj.AddComponent<Text>();
			gameOverCoinText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			if (gameOverCoinText.font == null)
				gameOverCoinText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
			gameOverCoinText.fontSize = 22;
			gameOverCoinText.color = new Color(1f, 0.84f, 0f, 1f);
			gameOverCoinText.alignment = TextAnchor.MiddleCenter;
			RectTransform rt = coinObj.GetComponent<RectTransform>();
			rt.anchorMin = new Vector2(0.5f, 0.3f);
			rt.anchorMax = new Vector2(0.5f, 0.3f);
			rt.anchoredPosition = Vector2.zero;
			rt.sizeDelta = new Vector2(400f, 40f);
		}

		if (gameOverStarText == null)
		{
			GameObject starObj = new GameObject("StarText");
			starObj.transform.SetParent(popup.transform, false);
			gameOverStarText = starObj.AddComponent<Text>();
			gameOverStarText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			if (gameOverStarText.font == null)
				gameOverStarText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
			gameOverStarText.fontSize = 22;
			gameOverStarText.color = Color.yellow;
			gameOverStarText.alignment = TextAnchor.MiddleCenter;
			RectTransform rt = starObj.GetComponent<RectTransform>();
			rt.anchorMin = new Vector2(0.5f, 0.24f);
			rt.anchorMax = new Vector2(0.5f, 0.24f);
			rt.anchoredPosition = Vector2.zero;
			rt.sizeDelta = new Vector2(400f, 40f);
		}
	}

	public void AddTime(float sec)
	{
		seconds += sec;

		if (seconds >= 60)
		{
			minutes += Mathf.Round(seconds / 60f);
			seconds = seconds % 60;
		}
		else if (seconds <= 0)
		{
			minutes = 0;
			seconds = 0;
		}
	}

	void Update(){

		if (!gamePaused)
		{
			if(miliseconds <= 0){
				if(seconds <= 0){
					minutes--;
					seconds = 59;
				}
				else if(seconds >= 0){
					seconds--;
				}
				
				miliseconds = 100;
			}
			
			miliseconds -= Time.deltaTime * 100;

			if (seconds < 10)
				secondsString = "0" + seconds.ToString();
			else
				secondsString = seconds.ToString();

			// If seconds less than 5 colour lettrers in red, else white
			if (seconds < 5 && minutes < 1)
			{
				Color c = GetComponent<Text>().color;

				c.r = 1f;
				c.g = 0;
				c.b = 0;

				GetComponent<Text>().color = c;

				if (!videoAvailableChecked)
				{
					videoAvailableChecked = true;

					if (!LevelManager.alreadyContinuedWithVideo && AdsManager.Instance != null)
						AdsManager.Instance.IsVideoRewardAvailable();
				}

				// If beeping is not playing play
//				if (!SoundManager.Instance.beepSound.isPlaying)
//					SoundManager.Instance.Play_BeepSound();
			}
			else
			{
				Color c = GetComponent<Text>().color;
				
				c.r = 1f;
				c.g = 1f;
				c.b = 1f;
				
				GetComponent<Text>().color = c;

//				SoundManager.Instance.Stop_BeepSound();
			}

			if (miliseconds < 10)
			{
				int ms;
				ms = (int)miliseconds;
				milisecondsString = "0" + ms.ToString();
			}
			else
			{
				int ms;
				ms = (int)miliseconds;
				milisecondsString = ms.ToString();
			}

			if (minutes >= 0)
			{
				timer.text = minutes.ToString() + ":" + secondsString + ":" + milisecondsString;
			}
			else
			{
				timer.text = "0:00";

				if (!gameoverCalled)
				{
					LevelManager.gameOver = true;

					Time.timeScale = 0;

					// Show gameover popup
					GameObject.Find("Canvas").GetComponent<MenuManager>().ShowPopUpMenu(LevelManager.levelManager.gameOverPopup);

					if (AdsManager.videoReady && !LevelManager.alreadyContinuedWithVideo)
						LevelManager.levelManager.watchVideoPanelHolder.SetActive(true);

					gameoverCalled = true;

					// Check if its highscore
					if (LevelManager.levelManager.pointsScored > PlayerPrefs.GetInt("ScoreOfTheHighes"))
					{
						PlayerPrefs.SetInt("ScoreOfTheHighes", LevelManager.levelManager.pointsScored);
						if (GlobalVariables.globalVariables != null)
							GlobalVariables.globalVariables.PostScoreToLeaderboard(LevelManager.levelManager.pointsScored);
					}

					int starsEarned = Mathf.Clamp(LevelManager.levelManager.pointsScored / 50, 1, 3);
					ThemeProgressionManager.AddStars(starsEarned);
					if (starsEarned >= 2)
						SoundManager.PlayMusic("Victory");
					else
						SoundManager.PlayMusic("GameOver");

					int baseCoinBonus = Mathf.Clamp(LevelManager.levelManager.pointsScored / 10, 1, 50);
					ShopManager.AddCoins(baseCoinBonus);

					FindOrCreateGameOverTexts();
					if (gameOverCoinText != null)
						gameOverCoinText.text = "Coins earned: " + (LevelManager.levelManager.coinsEarned + baseCoinBonus).ToString();
					if (gameOverStarText != null)
						gameOverStarText.text = "Stars earned: " + starsEarned.ToString() + " / 3";
				}
			}
		}
	}
}
