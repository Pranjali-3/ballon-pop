using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

///<summary>
///<para>Scene:All/NameOfScene/NameOfScene1,NameOfScene2,NameOfScene3...</para>
///<para>Object:N/A</para>
///<para>Description: Sample Description </para>
///</summary>

public class LevelManager : MonoBehaviour {

	// Spawning time
	public float minBaloonSpawnTime = 0.45f;
	public float maxBaloonSpewnTime = 0.9f;

	// Baloon speed
	public float minBaloonSpeed = 180f;
	public float maxBaloonSpeed = 350f;
	public float fallingSpeed = 200f;

	public int pointsScored;
	public Text pointsTextHolder;
	public int initialHighScore;
	private bool highScoreBannerTriggered;

	public GameObject baloonPrefab;
	public bool bossBaloonsEnabled = true;
	public float bossSpawnChance = 0.05f;
	public float bossMinSecondsBetweenSpawns = 18f;
	public float bossBaloonScale = 1.8f;
	public int bossHitPoints = 5;
	public int bossRewardPoints = 10;
	float lastBossSpawnTime;

	public Transform baloonsHolder;
	public Transform toysHolder;

	public static bool gamePaused;

	public GameObject[] baloonParticlesPool;
	public GameObject[] bombParticlesPool;
	public GameObject[] addTimeAnimationsPool;
	public GameObject[] loseTimeAnimationsPool;
	public GameObject[] addPointsAnimationsPool;

	public GameObject[] listOfToys;

	public Sprite addTimeSprite;
	public Sprite loseTimeSprite;
	public Sprite bombSprite;
	public Sprite doublePointsSprite;

	public float addTimeAmount;
	public float substractTimeAmount;

	public float gameTimeOrig;
	public float gameTime;
	public Timer timer;

	public static bool gameOver;

	Text coinsInLevelText;

	public GameObject gameOverPopup;
	public GameObject pausePopup;

	// Powerup variable
	public int pointsMultiplier;
	public float doublePointsDuration;
	public float doublePointsStartTime;
	public static bool doublePointsActive;

	// Powerup indicators
	public GameObject doubleCoinsIndicator;

	// Game panel - for animations
	public GameObject gamePanel;

	// Baloon images
	public Sprite[] baloonImages;

	// Level interface elements
	public GameObject pauseButtonHolder;
	public GameObject timerHolder;
	public GameObject pointsHolder;
	public GameObject cloudBlocker;

	public GameObject watchVideoPanelHolder;
	public static bool alreadyContinuedWithVideo;
	public static bool videoReady;

	public static LevelManager levelManager;

	public bool isEducationalMode;
	public EducationalSubject currentEducationalSubject;

	public void ActivatePowerUp(PowerUpType type)
	{
		switch (type)
		{
		case PowerUpType.ExtraTime:
			timer.AddTime(30f);
			SoundManager.PlaySound("PlusSec");
			break;
		case PowerUpType.DoublePoints:
			doublePointsActive = true;
			doublePointsStartTime = Time.time;
			pointsMultiplier = 2;
			doubleCoinsIndicator.SetActive(true);
			break;
		case PowerUpType.Shield:
			PlayerPrefs.SetInt("ShieldActive", 1);
			PlayerPrefs.Save();
			break;
		case PowerUpType.SlowMotion:
			StartCoroutine(SlowMotionCoroutine());
			break;
		case PowerUpType.Magnet:
			StartCoroutine(MagnetCoroutine());
			break;
		}
	}

	IEnumerator SlowMotionCoroutine()
	{
		float origMin = minBaloonSpeed;
		float origMax = maxBaloonSpeed;
		minBaloonSpeed *= 0.4f;
		maxBaloonSpeed *= 0.4f;
		yield return new WaitForSeconds(10f);
		minBaloonSpeed = origMin;
		maxBaloonSpeed = origMax;
	}

	IEnumerator MagnetCoroutine()
	{
		float duration = 8f;
		float elapsed = 0f;
		while (elapsed < duration)
		{
			if (toysHolder != null)
			{
				foreach (Transform toy in toysHolder)
				{
					if (toy != null)
					{
						Vector3 dir = (Camera.main.transform.position - toy.position).normalized;
						toy.position += dir * 3f * Time.deltaTime;
					}
				}
			}
			elapsed += Time.deltaTime;
			yield return null;
		}
	}

	public IEnumerator SpawnBaloon()
	{
		if (baloonPrefab == null)
		{
			baloonPrefab = Resources.Load<GameObject>("Baloon");
		}
		if (baloonPrefab == null)
		{
			Debug.LogWarning("LevelManager.SpawnBaloon: baloonPrefab is null! Assign it in the LevelManager inspector.");
			yield return new WaitForSeconds(2f);
			StartCoroutine("SpawnBaloon");
			yield break;
		}

		float minTime = minBaloonSpawnTime > 0f ? minBaloonSpawnTime : 0.2f;
		float maxTime = maxBaloonSpewnTime > 0f ? maxBaloonSpewnTime : 0.4f;
		float spawningTime = UnityEngine.Random.Range(minTime, maxTime);
		if (spawningTime <= 0f) spawningTime = 1.0f;

		yield return new WaitForSeconds(spawningTime);

		float xPos = UnityEngine.Random.Range(-3f, 3f);

		GameObject baloon = Instantiate(baloonPrefab, new Vector3(xPos, -6f, 90), baloonPrefab.transform.rotation) as GameObject;
		if (baloon == null)
		{
			StartCoroutine("SpawnBaloon");
			yield break;
		}
		if (baloonsHolder == null)
		{
			GameObject holder = GameObject.Find("BaloonsHolder") ?? GameObject.Find("Baloons") ?? GameObject.Find("Canvas/GamePanel") ?? GameObject.Find("Canvas");
			if (holder != null) baloonsHolder = holder.transform;
		}
		if (baloonsHolder != null)
			baloon.transform.SetParent(baloonsHolder);
		baloon.transform.localScale = Vector3.one;
		baloon.transform.SetAsFirstSibling();

		Baloon baloonScript = baloon.GetComponent<Baloon>();
		if (isEducationalMode)
		{
			if (baloonScript != null)
				baloonScript.MakeEducationalBaloon(currentEducationalSubject);
		}
		else
		{
			TryMakeBossBaloon(baloon);
		}

		if (baloonScript != null)
		{
			float speed = UnityEngine.Random.Range(minBaloonSpeed, maxBaloonSpeed);
			baloonScript.movingSpeed = speed;
		}

		StartCoroutine("SpawnBaloon");
	}

	public int coinsEarned;

	public void ScorePoint(int points)
	{
		if (gameOver)
			return;

		int pointsToAdd = points * pointsMultiplier;
		pointsToAdd = ComboManager.RegisterPop(pointsToAdd);
		pointsScored += pointsToAdd;
		if (pointsTextHolder != null)
			pointsTextHolder.text = pointsScored.ToString();

		if (pointsScored > initialHighScore && !highScoreBannerTriggered)
		{
			if (initialHighScore > 0 || pointsScored >= 10)
			{
				highScoreBannerTriggered = true;
				TriggerMilestoneBanner("New High Score!");
			}
		}

		if (MissionManager.Instance != null)
		{
			MissionManager.AddProgress(MissionType.ScorePoints, pointsToAdd);
			if (points == 1)
				MissionManager.AddProgress(MissionType.PopBalloons, 1);
		}

		int coinReward = Mathf.Max(1, pointsToAdd / 5);
		coinsEarned += coinReward;
		ShopManager.AddCoins(coinReward);

		if (coinsInLevelText != null)
			coinsInLevelText.text = "Coins: " + coinsEarned.ToString();

		AchievementManager.CheckScoreAchievement(pointsScored);
	}

	void TryMakeBossBaloon(GameObject baloon)
	{
		if (!bossBaloonsEnabled || baloon == null)
			return;

		if (Time.time < lastBossSpawnTime + bossMinSecondsBetweenSpawns)
			return;

		if (UnityEngine.Random.value > bossSpawnChance)
			return;

		lastBossSpawnTime = Time.time;

		BossBaloon boss = baloon.AddComponent<BossBaloon>();
		boss.hitPoints = bossHitPoints;
		boss.rewardPoints = bossRewardPoints;
		SoundManager.PlayMusic("Boss");

		Baloon baloonScript = baloon.GetComponent<Baloon>();
		if (baloonScript != null)
			baloonScript.baloonType = 7;

		baloon.transform.localScale = Vector3.one * bossBaloonScale;
	}

	public void PauseGame()
	{
		if (!gamePaused && !gameOver && !GameObject.Find("Canvas").GetComponent<MenuManager>().popupOpened)
		{
			GameObject.Find("Canvas").GetComponent<MenuManager>().ShowPopUpMenu(pausePopup);
			Time.timeScale = 0;
			gamePaused = true;
			timer.gamePaused = true;
		}
	}

	void OnApplicationPause(bool paused)
	{
		if (paused)
			PauseGame();
	}

	public void ContinueGame()
	{
		StartCoroutine(UnpauseGameCoroutine());
	}

	IEnumerator UnpauseGameCoroutine()
	{
		GameObject.Find("Canvas").GetComponent<MenuManager>().ClosePopUpMenu(pausePopup);

		Time.timeScale = 1;

		yield return new WaitForSeconds(0.1f);

		if (gamePaused && !gameOver)
		{
			gamePaused = false;
			timer.gamePaused = false;
		}
	}

	public void PlayParticleOnPosition(Vector3 position)
	{
		for (int i = 0; i < baloonParticlesPool.Length; i++)
		{
			if (!baloonParticlesPool[i].GetComponent<BaloonParticle>().active)
			{
				baloonParticlesPool[i].transform.localPosition = position;
				baloonParticlesPool[i].GetComponent<BaloonParticle>().PlayBaloonParticle();
				break;
			}
		}
	}

	public void PlayAddTimeOnPosition(Vector3 position)
	{
		for (int i = 0; i < addTimeAnimationsPool.Length; i++)
		{
			if (!addTimeAnimationsPool[i].GetComponent<BaloonParticle>().active)
			{
				addTimeAnimationsPool[i].transform.localPosition = position;
				addTimeAnimationsPool[i].GetComponent<BaloonParticle>().PlayAddTimeAnimation();
				break;
			}
		}
	}

	public void PlayLoseTimeOnPosition(Vector3 position)
	{
		for (int i = 0; i < loseTimeAnimationsPool.Length; i++)
		{
			if (!loseTimeAnimationsPool[i].GetComponent<BaloonParticle>().active)
			{
				loseTimeAnimationsPool[i].transform.localPosition = position;
				loseTimeAnimationsPool[i].GetComponent<BaloonParticle>().PlayLoseTimeAnimation();
				break;
			}
		}
	}

	public void PlayAddPointOnPosition(Vector3 position, int points)
	{
		for (int i = 0; i < addPointsAnimationsPool.Length; i++)
		{
			if (!addPointsAnimationsPool[i].GetComponent<BaloonParticle>().active)
			{
				addPointsAnimationsPool[i].transform.localPosition = position;
				addPointsAnimationsPool[i].GetComponent<BaloonParticle>().PlayAddPointAnimation(points);
				break;
			}
		}
	}

	public void PlayBombParticleOnPosition(Vector3 position)
	{
		for (int i = 0; i < bombParticlesPool.Length; i++)
		{
			if (!bombParticlesPool[i].GetComponent<BaloonParticle>().active)
			{
				bombParticlesPool[i].transform.localPosition = position;
				bombParticlesPool[i].GetComponent<BaloonParticle>().PlayBaloonParticle();
				break;
			}
		}
	}

	void Awake()
	{
		FeatureBootstrapper.EnsureManagers();

		levelManager = this;
		Time.timeScale = 1f;
		initialHighScore = PlayerPrefs.GetInt("ScoreOfTheHighes", 0);

		// Force speed values (overrides stale scene serialized values)
		minBaloonSpawnTime = 0.45f;
		maxBaloonSpewnTime = 0.9f;
		minBaloonSpeed = 180f;
		maxBaloonSpeed = 350f;
		if (fallingSpeed >= 0f) fallingSpeed = 200f;
		highScoreBannerTriggered = false;

		// Recover missing reference holders dynamically
		if (baloonsHolder == null)
		{
			GameObject holder = GameObject.Find("BaloonsHolder") ?? GameObject.Find("Baloons") ?? GameObject.Find("Canvas/GamePanel") ?? GameObject.Find("Canvas");
			if (holder != null) baloonsHolder = holder.transform;
		}

		if (toysHolder == null)
		{
			GameObject holder = GameObject.Find("ToysHolder") ?? GameObject.Find("Toys") ?? GameObject.Find("Canvas/GamePanel") ?? GameObject.Find("Canvas");
			if (holder != null) toysHolder = holder.transform;
		}

		if (timer == null)
		{
			timer = GameObject.FindObjectOfType<Timer>();
			if (timer == null)
			{
				GameObject timerObj = GameObject.Find("TimerText");
				if (timerObj != null) timer = timerObj.GetComponent<Timer>();
			}
		}

		if (pointsTextHolder == null)
		{
			GameObject pointsObj = GameObject.Find("PointsText");
			if (pointsObj != null) pointsTextHolder = pointsObj.GetComponent<Text>();
		}

		if (pointsHolder == null)
		{
			GameObject pHolder = GameObject.Find("PointsHolder") ?? GameObject.Find("PointsText");
			if (pHolder != null) pointsHolder = pHolder;
		}

		if (pauseButtonHolder == null) pauseButtonHolder = GameObject.Find("PauseButton");
		if (timerHolder == null) timerHolder = GameObject.Find("TimerText");
		if (gameOverPopup == null) gameOverPopup = GameObject.Find("Canvas/Popups/GameOverPopup");
		if (pausePopup == null) pausePopup = GameObject.Find("Canvas/Popups/PausePopup");
		if (watchVideoPanelHolder == null) watchVideoPanelHolder = GameObject.Find("Canvas/Popups/WatchVideoPanel");

		// Resource loading fallback for baloonPrefab
		if (baloonPrefab == null)
		{
			baloonPrefab = Resources.Load<GameObject>("Baloon");
			if (baloonPrefab == null)
			{
				Debug.LogError("LevelManager: Baloon prefab could not be loaded from Resources!");
			}
		}

		// Resource loading fallback for listOfToys
		if (listOfToys == null || listOfToys.Length == 0)
		{
			listOfToys = new GameObject[20];
			for (int i = 0; i < 20; i++)
			{
				listOfToys[i] = Resources.Load<GameObject>("Toys/Toy" + (i + 1));
				if (listOfToys[i] == null)
				{
					Debug.LogError("LevelManager: Toy" + (i + 1) + " could not be loaded from Resources!");
				}
			}
		}

		if (ThemeProgressionManager.Instance != null)
			ThemeProgressionManager.Instance.ApplySelectedTheme();

		if (ProgressionMapManager.Instance != null)
			ProgressionMapManager.Instance.ApplySelectedLevel();

		NormalizeBalloonTuning();

		// Force the game duration to be 60 seconds only
		gameTime = 60f;
		gameTimeOrig = 60f;

		if (ToyAlbumManager.Instance != null && listOfToys != null && listOfToys.Length > 0)
		{
			ToyAlbumManager.Instance.totalToys = listOfToys.Length;
			ToyAlbumManager.Instance.UpdateAlbumText();
		}

		if (PlayerPrefs.HasKey("EducationalSubject") && PlayerPrefs.GetInt("EducationalSubject", -1) >= 0)
		{
			isEducationalMode = true;
			currentEducationalSubject = (EducationalSubject)PlayerPrefs.GetInt("EducationalSubject", 0);
			PlayerPrefs.DeleteKey("EducationalSubject");
		}
		else
		{
			isEducationalMode = false;
		}

		pointsScored = 0;
		pointsMultiplier = 1;
		fallingSpeed = -fallingSpeed;
		if (timer != null)
		{
			timer.minutes = Mathf.Round(gameTime / 60);
			timer.seconds = gameTime % 60;
		}

		doublePointsActive = false;
		doublePointsStartTime = -doublePointsDuration - 1f;
		lastBossSpawnTime = -bossMinSecondsBetweenSpawns;

		gamePaused = false;

		gameOver = false;
		AdsManager.videoReady = false;

		alreadyContinuedWithVideo = false;

		if (GlobalVariables.removeAdsOwned)
		{
			if (pauseButtonHolder != null) pauseButtonHolder.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-90f, -90f);
			if (timerHolder != null) timerHolder.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, 118f);
			if (pointsHolder != null) pointsHolder.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(22f, -142f);
			if (cloudBlocker != null) Destroy(cloudBlocker);
		}

		CreateCoinsInLevelText();

		// Reset missions for the current level start
		if (MissionManager.Instance != null)
		{
			MissionManager.Instance.ResetMissionProgress();
		}
	}

	void CreateCoinsInLevelText()
	{
		GameObject canvasObj = GameObject.Find("Canvas");
		if (canvasObj == null) return;

		GameObject coinObj = new GameObject("CoinsInLevelText");
		coinObj.transform.SetParent(pointsHolder != null ? pointsHolder.transform : canvasObj.transform, false);
		coinsInLevelText = coinObj.AddComponent<Text>();
		coinsInLevelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		if (coinsInLevelText.font == null)
			coinsInLevelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
		coinsInLevelText.fontSize = 20;
		coinsInLevelText.color = new Color(1f, 0.84f, 0f, 1f);
		coinsInLevelText.alignment = TextAnchor.MiddleLeft;
		coinsInLevelText.text = "Coins: 0";
		RectTransform rt = coinObj.GetComponent<RectTransform>();
		rt.anchorMin = new Vector2(0f, 0f);
		rt.anchorMax = new Vector2(0f, 0f);
		rt.anchoredPosition = new Vector2(10f, -30f);
		rt.sizeDelta = new Vector2(200f, 30f);
	}

	void Start()
	{
		SoundManager.PlayMusic("Gameplay");
		StartCoroutine("SpawnBaloon");
		AddDashboardButtonToGameOverPopup();

		if (isEducationalMode && EducationalModeManager.Instance != null)
		{
			EducationalModeManager.Instance.StartEducationalMode(currentEducationalSubject);
		}
	}

	public void RestartGame()
	{
		if (pausePopup.activeInHierarchy)
			GameObject.Find("Canvas").GetComponent<MenuManager>().ClosePopUpMenu(pausePopup);
		else if (gameOverPopup.activeInHierarchy)
			GameObject.Find("Canvas").GetComponent<MenuManager>().ClosePopUpMenu(gameOverPopup);

		StartCoroutine(RestartGameCoroutine());
	}

	IEnumerator RestartGameCoroutine()
	{
		Time.timeScale = 1f;

		// Stop spawning baloons
		StopCoroutine("SpawnBaloon");
		//		StopAllCoroutines();

		// Clear lists for fingerpower elements
		if (Camera.main != null && Camera.main.GetComponent<FingerPower>() != null)
			Camera.main.GetComponent<FingerPower>().ClearAdditionalLists();

		// Destroy all gameobjects
		if (baloonsHolder != null)
		{
			foreach (Transform t in baloonsHolder.transform)
			{
				Destroy(t.gameObject);
			}
		}

		if (toysHolder != null)
		{
			foreach (Transform t in toysHolder.transform)
			{
				Destroy(t.gameObject);
			}
		}

		yield return new WaitForSeconds(0.1f);

        if(!GlobalVariables.removeAdsOwned && AdsManager.Instance != null)
		    AdsManager.Instance.ShowInterstitial();

		yield return new WaitForSeconds(0.5f);

		gameTime = gameTimeOrig;

		gamePaused = false;
		if (timer != null)
			timer.gamePaused = false;
		AdsManager.videoReady = false;

		pointsScored = 0;
		coinsEarned = 0;
		if (coinsInLevelText != null)
			coinsInLevelText.text = "Coins: 0";
		if (pointsTextHolder != null)
			pointsTextHolder.text = pointsScored.ToString();

		if (timer != null)
		{
			timer.minutes = Mathf.Round(gameTime / 60);
			timer.seconds = gameTime % 60;
			timer.gameoverCalled = false;
			timer.gamePaused = false;
		}

		gameOver = false;
		SoundManager.PlayMusic("Gameplay");

		alreadyContinuedWithVideo = false;

		GameObject timerTextObject = GameObject.Find("TimerText");
		if (timerTextObject != null && timerTextObject.GetComponent<Timer>() != null)
			timerTextObject.GetComponent<Timer>().videoAvailableChecked = false;

		StartCoroutine("SpawnBaloon");
	}

	public void HomeButtonPressed()
	{
		// Set always scale to 1 so if game is paused it will be unpaused
//		if (gamePaused)
		Time.timeScale = 1f;

		if (!GlobalVariables.removeAdsOwned && AdsManager.Instance != null)
			AdsManager.Instance.ShowInterstitial();

		Application.LoadLevel("MainScene");
	}

	void NormalizeBalloonTuning()
	{
		if (maxBaloonSpeed > 0f && maxBaloonSpeed < 50f)
		{
			minBaloonSpeed *= 120f;
			maxBaloonSpeed *= 120f;
		}

		minBaloonSpeed = Mathf.Clamp(minBaloonSpeed, 180f, 520f);
		maxBaloonSpeed = Mathf.Clamp(maxBaloonSpeed, 300f, 700f);

		if (maxBaloonSpeed <= minBaloonSpeed)
			maxBaloonSpeed = minBaloonSpeed + 120f;

		minBaloonSpawnTime = Mathf.Clamp(minBaloonSpawnTime, 0.2f, 0.65f);
		maxBaloonSpewnTime = Mathf.Clamp(maxBaloonSpewnTime, 0.4f, 0.95f);

		if (maxBaloonSpewnTime <= minBaloonSpawnTime)
			maxBaloonSpewnTime = minBaloonSpawnTime + 0.35f;
	}

	public void WatchVideoForContinue()
	{
		if (AdsManager.videoReady)
			AdsManager.Instance.ShowVideoReward();
	}

	public void ContinueGameAfterVideoIsWatched()
	{
		StartCoroutine("ContinueCoroutine");
	}

	IEnumerator ContinueCoroutine()
	{
		// Close popup menu
		GameObject.Find("Canvas").GetComponent<MenuManager>().ClosePopUpMenu(gameOverPopup);
		Time.timeScale = 1f;

		alreadyContinuedWithVideo = true;

		yield return new WaitForSeconds(0.4f);

		// Hide watch video panel
		watchVideoPanelHolder.SetActive(false);

		// Comtinue game
		gameOver = false;
		gamePaused = false;
		SoundManager.PlayMusic("Gameplay");
		gameTime = 20f;
		timer.minutes = Mathf.Round(gameTime / 60);
		timer.seconds = gameTime % 60;
		timer.gameoverCalled = false;
		timer.gamePaused = false;

		StartCoroutine("SpawnBaloon");
	}

	public void DashboardButtonPressed()
	{
		Time.timeScale = 1f;

		if (gameOverPopup != null && gameOverPopup.activeInHierarchy)
		{
			GameObject.Find("Canvas").GetComponent<MenuManager>().ClosePopUpMenu(gameOverPopup);
		}

		if (!GlobalVariables.removeAdsOwned && AdsManager.Instance != null)
			AdsManager.Instance.ShowInterstitial();

		Application.LoadLevel("Dashboard");
	}

	void AddDashboardButtonToGameOverPopup()
	{
		if (gameOverPopup == null)
			return;

		Button[] buttons = gameOverPopup.GetComponentsInChildren<Button>(true);
		if (buttons.Length == 0)
			return;

		foreach (Button btn in buttons)
		{
			if (btn.name == "DashboardButton")
				return;
		}

		Button homeButton = null;
		Button restartButton = null;

		foreach (Button btn in buttons)
		{
			string nameLower = btn.name.ToLower();
			if (nameLower.Contains("home") || nameLower.Contains("menu"))
			{
				homeButton = btn;
			}
			else if (nameLower.Contains("restart") || nameLower.Contains("retry") || nameLower.Contains("rewind") || nameLower.Contains("play"))
			{
				restartButton = btn;
			}
		}

		if (homeButton == null && buttons.Length > 0) homeButton = buttons[0];
		if (restartButton == null && buttons.Length > 1) restartButton = buttons[1];

		if (homeButton == null) return;

		GameObject dashboardBtnObj = Instantiate(homeButton.gameObject) as GameObject;
		dashboardBtnObj.name = "DashboardButton";
		dashboardBtnObj.transform.SetParent(homeButton.transform.parent, false);

		Button dashboardButton = dashboardBtnObj.GetComponent<Button>();
		dashboardButton.onClick.RemoveAllListeners();
		dashboardButton.onClick.AddListener(delegate {
			SoundManager.PlaySound("ButtonClick");
			DashboardButtonPressed();
		});

		Text btnText = dashboardBtnObj.GetComponentInChildren<Text>(true);
		if (btnText != null)
		{
			btnText.text = "Dashboard";
		}

		RectTransform homeRect = homeButton.GetComponent<RectTransform>();
		RectTransform dashboardRect = dashboardButton.GetComponent<RectTransform>();

		if (restartButton != null)
		{
			RectTransform restartRect = restartButton.GetComponent<RectTransform>();
			float xDiff = Mathf.Abs(homeRect.anchoredPosition.x - restartRect.anchoredPosition.x);
			float yDiff = Mathf.Abs(homeRect.anchoredPosition.y - restartRect.anchoredPosition.y);

			if (xDiff > yDiff)
			{
				Button leftBtn = homeRect.anchoredPosition.x < restartRect.anchoredPosition.x ? homeButton : restartButton;
				Button rightBtn = leftBtn == homeButton ? restartButton : homeButton;

				RectTransform leftRect = leftBtn.GetComponent<RectTransform>();
				RectTransform rightRect = rightBtn.GetComponent<RectTransform>();

				float leftX = leftRect.anchoredPosition.x;
				float rightX = rightRect.anchoredPosition.x;
				float yPos = (leftRect.anchoredPosition.y + rightRect.anchoredPosition.y) / 2f;

				leftRect.anchoredPosition = new Vector2(leftX - 90f, leftRect.anchoredPosition.y);
				rightRect.anchoredPosition = new Vector2(rightX + 90f, rightRect.anchoredPosition.y);
				dashboardRect.anchoredPosition = new Vector2((leftX + rightX) / 2f, yPos);
			}
			else
			{
				Button topBtn = homeRect.anchoredPosition.y > restartRect.anchoredPosition.y ? homeButton : restartButton;
				Button bottomBtn = topBtn == homeButton ? restartButton : homeButton;

				RectTransform topRect = topBtn.GetComponent<RectTransform>();
				RectTransform bottomRect = bottomBtn.GetComponent<RectTransform>();

				float topY = topRect.anchoredPosition.y;
				float bottomY = bottomRect.anchoredPosition.y;
				float xPos = (topRect.anchoredPosition.x + bottomRect.anchoredPosition.x) / 2f;

				topRect.anchoredPosition = new Vector2(topRect.anchoredPosition.x, topY + 70f);
				bottomRect.anchoredPosition = new Vector2(bottomRect.anchoredPosition.x, bottomY - 70f);
				dashboardRect.anchoredPosition = new Vector2(xPos, (topY + bottomY) / 2f);
			}
		}
		else
		{
			Vector2 homePos = homeRect.anchoredPosition;
			homeRect.anchoredPosition = new Vector2(homePos.x - 100f, homePos.y);
			dashboardRect.anchoredPosition = new Vector2(homePos.x + 100f, homePos.y);
		}
	}

	public void TriggerMilestoneBanner(string message)
	{
		StartCoroutine(MilestoneCoroutine(message));
	}

	IEnumerator MilestoneCoroutine(string message)
	{
		float oldTimeScale = Time.timeScale;
		Time.timeScale = 0f;

		SoundManager.PlaySound("AchievementUnlock");

		GameObject bannerCanvas = new GameObject("MilestoneBannerCanvas");
		Canvas canvas = bannerCanvas.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		bannerCanvas.AddComponent<CanvasScaler>();
		bannerCanvas.AddComponent<GraphicRaycaster>();
		canvas.sortingOrder = 999;

		GameObject bg = new GameObject("Background");
		bg.transform.SetParent(bannerCanvas.transform, false);
		Image bgImg = bg.AddComponent<Image>();
		bgImg.color = new Color(0f, 0f, 0f, 0.4f);
		RectTransform bgRect = bg.GetComponent<RectTransform>();
		bgRect.anchorMin = Vector2.zero;
		bgRect.anchorMax = Vector2.one;
		bgRect.offsetMin = Vector2.zero;
		bgRect.offsetMax = Vector2.zero;

		GameObject banner = new GameObject("Banner");
		banner.transform.SetParent(bannerCanvas.transform, false);
		Image bannerImg = banner.AddComponent<Image>();
		bannerImg.color = new Color(0.1f, 0.15f, 0.3f, 0.95f);
		Outline outline = banner.AddComponent<Outline>();
		outline.effectColor = new Color(1f, 0.84f, 0f, 1f);
		outline.effectDistance = new Vector2(3f, 3f);
		RectTransform bannerRect = banner.GetComponent<RectTransform>();
		bannerRect.anchorMin = new Vector2(0.5f, 0.5f);
		bannerRect.anchorMax = new Vector2(0.5f, 0.5f);
		bannerRect.anchoredPosition = Vector2.zero;
		bannerRect.sizeDelta = new Vector2(500f, 160f);

		GameObject textGo = new GameObject("Text");
		textGo.transform.SetParent(banner.transform, false);
		Text text = textGo.AddComponent<Text>();
		Font defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
		if (defaultFont == null) defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
		text.font = defaultFont;
		text.text = message;
		text.fontSize = 28;
		text.color = Color.white;
		text.fontStyle = FontStyle.Bold;
		text.alignment = TextAnchor.MiddleCenter;
		Shadow shadow = textGo.AddComponent<Shadow>();
		shadow.effectColor = Color.black;
		shadow.effectDistance = new Vector2(2f, -2f);
		RectTransform textRect = text.GetComponent<RectTransform>();
		textRect.anchorMin = Vector2.zero;
		textRect.anchorMax = Vector2.one;
		textRect.offsetMin = new Vector2(20f, 20f);
		textRect.offsetMax = new Vector2(-20f, -20f);

		int particleCount = 30;
		GameObject[] particles = new GameObject[particleCount];
		Vector2[] directions = new Vector2[particleCount];
		float[] speeds = new float[particleCount];
		Color[] colors = new Color[] { Color.yellow, Color.red, Color.green, Color.cyan, new Color(1f, 0.5f, 0f) };

		for (int i = 0; i < particleCount; i++)
		{
			GameObject p = new GameObject("Particle" + i);
			p.transform.SetParent(bannerCanvas.transform, false);
			Image pImg = p.AddComponent<Image>();
			pImg.color = colors[UnityEngine.Random.Range(0, colors.Length)];
			RectTransform pRect = p.GetComponent<RectTransform>();
			pRect.anchorMin = new Vector2(0.5f, 0.5f);
			pRect.anchorMax = new Vector2(0.5f, 0.5f);
			pRect.anchoredPosition = new Vector2(UnityEngine.Random.Range(-50f, 50f), UnityEngine.Random.Range(-50f, 50f));
			pRect.sizeDelta = new Vector2(UnityEngine.Random.Range(10f, 20f), UnityEngine.Random.Range(10f, 20f));

			particles[i] = p;
			float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
			directions[i] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
			speeds[i] = UnityEngine.Random.Range(100f, 400f);
		}

		float duration = 1.5f;
		float elapsed = 0f;
		while (elapsed < duration)
		{
			float dt = Time.unscaledDeltaTime;
			elapsed += dt;

			for (int i = 0; i < particleCount; i++)
			{
				if (particles[i] != null)
				{
					RectTransform pRect = particles[i].GetComponent<RectTransform>();
					pRect.anchoredPosition += directions[i] * speeds[i] * dt;
					Image pImg = particles[i].GetComponent<Image>();
					Color c = pImg.color;
					c.a = Mathf.Lerp(1f, 0f, elapsed / duration);
					pImg.color = c;
				}
			}

			if (elapsed > 1.2f)
			{
				float bannerAlpha = Mathf.Lerp(1f, 0f, (elapsed - 1.2f) / 0.3f);
				if (bannerImg != null)
				{
					Color c = bannerImg.color;
					c.a = bannerAlpha * 0.95f;
					bannerImg.color = c;
				}
				if (text != null)
				{
					Color c = text.color;
					c.a = bannerAlpha;
					text.color = c;
				}
				if (bgImg != null)
				{
					Color c = bgImg.color;
					c.a = bannerAlpha * 0.4f;
					bgImg.color = c;
				}
			}

			yield return null;
		}

		Destroy(bannerCanvas);
		Time.timeScale = oldTimeScale;
	}
}
