using UnityEngine;
using UnityEngine.UI;

public class DashboardSceneBuilder : MonoBehaviour
{
	Font defaultFont;
	Color backgroundColorA = new Color(0.08f, 0.16f, 0.32f, 1f); // Royal blue
	Color backgroundColorB = new Color(0.25f, 0.12f, 0.32f, 1f); // Dark violet
	Color panelColor = new Color(0.98f, 0.98f, 0.94f, 1f);
	Color accentColor = new Color(0.06f, 0.58f, 0.75f, 1f);
	Color buttonColor = new Color(1f, 0.74f, 0.21f, 1f);

	Text titleTextComponent;
	Text missionText;
	Text albumText;
	Text starsText;
	Text selectedStageText;
	Text selectedThemeText;

	// Dashboard preview elements
	Text dashboardStageTextPreview;
	Text dashboardThemeTextPreview;
	Text dashboardStarsTextPreview;
	Text dashboardToysTextPreview;
	Text dashboardCoinsTextPreview;
	Text dashboardHighScoreTextPreview;
	Text selectedToyStatusText;

	Image backgroundImageComponent;

	Text dailyStreakText;
	Text dailyRewardText;
	Button dailyClaimBtn;

	// Rect transforms for responsive layout
	RectTransform titleRect;
	RectTransform previewCardRect;
	RectTransform playButtonRect;
	RectTransform dailyTargetBtnRect;
	RectTransform toyCollectedBtnRect;
	RectTransform progressMapBtnRect;
	RectTransform themeSelectBtnRect;
	RectTransform shopBtnRect;
	RectTransform achievementsBtnRect;
	RectTransform educModeBtnRect;
	RectTransform dailyRewardBtnRect;
	RectTransform homeButtonRect;
	RectTransform resetProgressBtnRect;

	// Popup overlays
	GameObject dailyTargetPopup;
	GameObject toyCollectedPopup;
	GameObject progressMapPopup;
	GameObject themeSelectPopup;
	GameObject shopPopup;
	GameObject achievementsPopup;
	GameObject educModePopup;
	GameObject toyAlbumGalleryPopup;

	int lastScreenWidth;
	int lastScreenHeight;

	void Awake()
	{
		Time.timeScale = 1f;
		FeatureBootstrapper.EnsureManagers();
		defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
		if (defaultFont == null)
			defaultFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
		BuildDashboard();
		WireFeatureManagers();
		RefreshDashboard();

		lastScreenWidth = Screen.width;
		lastScreenHeight = Screen.height;
		ApplyResponsiveLayout();
	}

	void Update()
	{
		if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
		{
			lastScreenWidth = Screen.width;
			lastScreenHeight = Screen.height;
			ApplyResponsiveLayout();
		}
	}

	void BuildDashboard()
	{
		GameObject eventSystemObject = new GameObject("EventSystem");
		eventSystemObject.AddComponent<UnityEngine.EventSystems.EventSystem>();
		eventSystemObject.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

		GameObject canvasObject = new GameObject("DashboardCanvas");
		Canvas canvas = canvasObject.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		
		CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
		scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(720f, 1280f);
		scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
		scaler.matchWidthOrHeight = 0.5f;

		canvasObject.AddComponent<GraphicRaycaster>();

		// 1. Background
		GameObject background = CreatePanel(canvasObject.transform, "Background", Color.white);
		backgroundImageComponent = background.GetComponent<Image>();
		SetFullStretch(background.GetComponent<RectTransform>());

		// 2. Title
		GameObject titleGo = new GameObject("Title");
		titleGo.transform.SetParent(canvasObject.transform, false);
		titleTextComponent = titleGo.AddComponent<Text>();
		titleTextComponent.font = defaultFont;
		titleTextComponent.text = "Tiny Poppers Dashboard";
		titleTextComponent.fontSize = 46;
		titleTextComponent.color = Color.white;
		titleTextComponent.alignment = TextAnchor.MiddleCenter;
		titleTextComponent.fontStyle = FontStyle.Bold;
		titleTextComponent.resizeTextForBestFit = true;
		titleTextComponent.resizeTextMinSize = 12;
		titleTextComponent.resizeTextMaxSize = 48;
		
		titleRect = titleTextComponent.rectTransform;
		Shadow titleShadow = titleGo.AddComponent<Shadow>();
		titleShadow.effectColor = new Color(0f, 0f, 0f, 0.6f);
		titleShadow.effectDistance = new Vector2(3f, -3f);

		// 3. Info / Preview Card
		GameObject previewCard = CreatePanel(canvasObject.transform, "PreviewCard", new Color(0f, 0.05f, 0.15f, 0.45f));
		previewCardRect = previewCard.GetComponent<RectTransform>();
		Outline outline = previewCard.AddComponent<Outline>();
		outline.effectColor = new Color(1f, 1f, 1f, 0.3f);
		outline.effectDistance = new Vector2(2f, 2f);

		// Inside Preview Card Texts
		int maxToys = ToyAlbumManager.Instance != null ? ToyAlbumManager.Instance.totalToys : 20;
		dashboardStageTextPreview = CreateText(previewCard.transform, "DashboardStageTextPreview", "Stage: Stage 1", 24, Color.white, TextAnchor.MiddleLeft);
		dashboardStarsTextPreview = CreateText(previewCard.transform, "DashboardStarsTextPreview", "Stars: 0", 24, Color.yellow, TextAnchor.MiddleLeft);
		dashboardToysTextPreview = CreateText(previewCard.transform, "DashboardToysTextPreview", "Toys: 0/" + maxToys, 24, new Color(0.4f, 1f, 0.4f, 1f), TextAnchor.MiddleLeft);
		dashboardCoinsTextPreview = CreateText(previewCard.transform, "DashboardCoinsTextPreview", "Coins: 0", 24, new Color(1f, 0.84f, 0f, 1f), TextAnchor.MiddleLeft);
		dashboardHighScoreTextPreview = CreateText(previewCard.transform, "DashboardHighScoreTextPreview", "High Score: 0", 24, new Color(0.85f, 0.4f, 1f, 1f), TextAnchor.MiddleLeft);

		// 4. Big Start / Play Button
		Button playButton = CreateButton(canvasObject.transform, "PlayButton", "START GAME", buttonColor);
		playButtonRect = playButton.GetComponent<RectTransform>();
		Text playBtnText = playButton.GetComponentInChildren<Text>();
		playBtnText.fontSize = 32;
		playBtnText.fontStyle = FontStyle.Bold;
		playBtnText.color = Color.white;
		Outline playBtnOutline = playButton.gameObject.AddComponent<Outline>();
		playBtnOutline.effectColor = new Color(0f, 0f, 0f, 0.4f);
		playBtnOutline.effectDistance = new Vector2(2f, -2f);
		playButton.onClick.AddListener(StartLevel);

		// 5. Grid of 4 beautiful pop-up buttons
		Button dailyTargetBtn = CreateButton(canvasObject.transform, "DailyTargetBtn", "Daily Target", accentColor);
		dailyTargetBtnRect = dailyTargetBtn.GetComponent<RectTransform>();
		SetBtnStyle(dailyTargetBtn, Color.white, 24);
		dailyTargetBtn.onClick.AddListener(delegate { OpenPopup(dailyTargetPopup); });

		Button toyCollectedBtn = CreateButton(canvasObject.transform, "ToyCollectedBtn", "Toy Collected", accentColor);
		toyCollectedBtnRect = toyCollectedBtn.GetComponent<RectTransform>();
		SetBtnStyle(toyCollectedBtn, Color.white, 24);
		toyCollectedBtn.onClick.AddListener(delegate { OpenPopup(toyCollectedPopup); });

		Button progressMapBtn = CreateButton(canvasObject.transform, "ProgressMapBtn", "Progress Map", accentColor);
		progressMapBtnRect = progressMapBtn.GetComponent<RectTransform>();
		SetBtnStyle(progressMapBtn, Color.white, 24);
		progressMapBtn.onClick.AddListener(delegate { OpenPopup(progressMapPopup); });

		Button achievementsBtn = CreateButton(canvasObject.transform, "AchievementsBtn", "Awards & Achievements", new Color(0.85f, 0.35f, 0.55f, 1f));
		achievementsBtnRect = achievementsBtn.GetComponent<RectTransform>();
		SetBtnStyle(achievementsBtn, Color.white, 22);
		achievementsBtn.onClick.AddListener(delegate { OpenPopup(achievementsPopup); RefreshAchievementsPopup(); });

		// 7. Home / Back Button
		Button homeButton = CreateButton(canvasObject.transform, "HomeButton", "Main Menu", new Color(0.9f, 0.93f, 0.95f, 1f));
		homeButtonRect = homeButton.GetComponent<RectTransform>();
		SetBtnStyle(homeButton, new Color(0.2f, 0.2f, 0.25f, 1f), 22);
		homeButton.onClick.AddListener(GoHome);

		// Reset Progress Button
		Button resetProgressBtn = CreateButton(canvasObject.transform, "ResetProgressBtn", "Reset Progress", new Color(0.85f, 0.25f, 0.25f, 1f));
		resetProgressBtnRect = resetProgressBtn.GetComponent<RectTransform>();
		SetBtnStyle(resetProgressBtn, Color.white, 20);
		resetProgressBtn.onClick.AddListener(ResetProgress);

		// 7. Pop-up Overlays Creation
		BuildPopups(canvasObject.transform);
	}

	void ApplyResponsiveLayout()
	{
		bool isLandscape = Screen.width > Screen.height;

		if (isLandscape)
		{
			SetRect(titleRect, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -55f), new Vector2(600f, 60f));
			titleTextComponent.fontSize = 34;

			SetRect(previewCardRect, new Vector2(0.18f, 0.50f), new Vector2(0.18f, 0.50f), Vector2.zero, new Vector2(280f, 280f));
			SetRect(dashboardStageTextPreview.rectTransform, new Vector2(0.5f, 0.85f), new Vector2(0.5f, 0.85f), Vector2.zero, new Vector2(240f, 32f));
			dashboardStageTextPreview.alignment = TextAnchor.MiddleCenter;
			SetRect(dashboardStarsTextPreview.rectTransform, new Vector2(0.5f, 0.68f), new Vector2(0.5f, 0.68f), Vector2.zero, new Vector2(240f, 32f));
			dashboardStarsTextPreview.alignment = TextAnchor.MiddleCenter;
			SetRect(dashboardHighScoreTextPreview.rectTransform, new Vector2(0.5f, 0.51f), new Vector2(0.5f, 0.51f), Vector2.zero, new Vector2(240f, 32f));
			dashboardHighScoreTextPreview.alignment = TextAnchor.MiddleCenter;
			SetRect(dashboardToysTextPreview.rectTransform, new Vector2(0.5f, 0.34f), new Vector2(0.5f, 0.34f), Vector2.zero, new Vector2(240f, 32f));
			dashboardToysTextPreview.alignment = TextAnchor.MiddleCenter;
			SetRect(dashboardCoinsTextPreview.rectTransform, new Vector2(0.5f, 0.17f), new Vector2(0.5f, 0.17f), Vector2.zero, new Vector2(240f, 32f));
			dashboardCoinsTextPreview.alignment = TextAnchor.MiddleCenter;

			SetRect(playButtonRect, new Vector2(0.50f, 0.76f), new Vector2(0.50f, 0.76f), Vector2.zero, new Vector2(320f, 64f));

			float btnW = 220f;
			float btnHSize = 40f;
			float col1X = 0.64f;
			float col2X = 0.88f;

			SetRect(dailyTargetBtnRect, new Vector2(col1X, 0.58f), new Vector2(col1X, 0.58f), Vector2.zero, new Vector2(btnW, btnHSize));
			SetRect(toyCollectedBtnRect, new Vector2(col2X, 0.58f), new Vector2(col2X, 0.58f), Vector2.zero, new Vector2(btnW, btnHSize));
			SetRect(progressMapBtnRect, new Vector2(col1X, 0.42f), new Vector2(col1X, 0.42f), Vector2.zero, new Vector2(btnW, btnHSize));
			SetRect(achievementsBtnRect, new Vector2(col2X, 0.42f), new Vector2(col2X, 0.42f), Vector2.zero, new Vector2(btnW, btnHSize));

			SetRect(homeButtonRect, new Vector2(0.38f, 0.04f), new Vector2(0.38f, 0.04f), Vector2.zero, new Vector2(150f, 40f));
			SetRect(resetProgressBtnRect, new Vector2(0.62f, 0.04f), new Vector2(0.62f, 0.04f), Vector2.zero, new Vector2(150f, 40f));
		}
		else
		{
			SetRect(titleRect, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -55f), new Vector2(540f, 55f));
			titleTextComponent.fontSize = 32;

			SetRect(previewCardRect, new Vector2(0.5f, 0.84f), new Vector2(0.5f, 0.84f), Vector2.zero, new Vector2(660f, 140f));
			SetRect(dashboardStageTextPreview.rectTransform, new Vector2(0.08f, 0.75f), new Vector2(0.08f, 0.75f), Vector2.zero, new Vector2(240f, 28f));
			dashboardStageTextPreview.alignment = TextAnchor.MiddleLeft;
			SetRect(dashboardStarsTextPreview.rectTransform, new Vector2(0.08f, 0.45f), new Vector2(0.08f, 0.45f), Vector2.zero, new Vector2(240f, 28f));
			dashboardStarsTextPreview.alignment = TextAnchor.MiddleLeft;
			SetRect(dashboardHighScoreTextPreview.rectTransform, new Vector2(0.08f, 0.15f), new Vector2(0.08f, 0.15f), Vector2.zero, new Vector2(240f, 28f));
			dashboardHighScoreTextPreview.alignment = TextAnchor.MiddleLeft;

			SetRect(dashboardToysTextPreview.rectTransform, new Vector2(0.52f, 0.75f), new Vector2(0.52f, 0.75f), Vector2.zero, new Vector2(240f, 28f));
			dashboardToysTextPreview.alignment = TextAnchor.MiddleLeft;
			SetRect(dashboardCoinsTextPreview.rectTransform, new Vector2(0.52f, 0.45f), new Vector2(0.52f, 0.45f), Vector2.zero, new Vector2(240f, 28f));
			dashboardCoinsTextPreview.alignment = TextAnchor.MiddleLeft;

			SetRect(playButtonRect, new Vector2(0.5f, 0.66f), new Vector2(0.5f, 0.66f), Vector2.zero, new Vector2(340f, 60f));

			float yStart = 0.52f;
			float yStep = 0.10f;
			float c1Min = 0.08f, c1Max = 0.46f;
			float c2Min = 0.54f, c2Max = 0.92f;

			SetRect(dailyTargetBtnRect, new Vector2(c1Min, yStart), new Vector2(c1Max, yStart), Vector2.zero, new Vector2(0f, 50f));
			SetRect(toyCollectedBtnRect, new Vector2(c2Min, yStart), new Vector2(c2Max, yStart), Vector2.zero, new Vector2(0f, 50f));
			SetRect(progressMapBtnRect, new Vector2(c1Min, yStart - yStep), new Vector2(c1Max, yStart - yStep), Vector2.zero, new Vector2(0f, 50f));
			SetRect(achievementsBtnRect, new Vector2(c2Min, yStart - yStep), new Vector2(c2Max, yStart - yStep), Vector2.zero, new Vector2(0f, 50f));

			SetRect(homeButtonRect, new Vector2(0.26f, 0.05f), new Vector2(0.26f, 0.05f), Vector2.zero, new Vector2(170f, 40f));
			SetRect(resetProgressBtnRect, new Vector2(0.74f, 0.05f), new Vector2(0.74f, 0.05f), Vector2.zero, new Vector2(170f, 40f));
		}
	}

	void SetBtnStyle(Button btn, Color textColor, int fontSize)
	{
		Text t = btn.GetComponentInChildren<Text>();
		t.color = textColor;
		t.fontSize = fontSize;
		t.fontStyle = FontStyle.Bold;
		Outline o = btn.gameObject.AddComponent<Outline>();
		o.effectColor = new Color(0f, 0f, 0f, 0.25f);
		o.effectDistance = new Vector2(1.5f, -1.5f);
	}

	void BuildPopups(Transform parent)
	{
		// A. Daily Target Popup
		dailyTargetPopup = CreatePopupBlocker(parent, "DailyTargetPopup");
		GameObject targetPanel = dailyTargetPopup.transform.Find("Window").gameObject;
		Text targetTitle = CreateText(targetPanel.transform, "Title", "Daily Missions", 28, Color.black, TextAnchor.MiddleCenter);
		targetTitle.fontStyle = FontStyle.Bold;
		SetRect(targetTitle.rectTransform, new Vector2(0.5f, 0.88f), new Vector2(0.5f, 0.88f), Vector2.zero, new Vector2(460f, 50f));
		missionText = CreateText(targetPanel.transform, "MissionText", "", 24, new Color(0.15f, 0.15f, 0.15f, 1f), TextAnchor.MiddleLeft);
		SetRect(missionText.rectTransform, new Vector2(0.5f, 0.52f), new Vector2(0.5f, 0.52f), Vector2.zero, new Vector2(460f, 400f));

		// B. Toy Collected Popup
		toyCollectedPopup = CreatePopupBlocker(parent, "ToyCollectedPopup");
		GameObject toyPanel = toyCollectedPopup.transform.Find("Window").gameObject;
		Text toyTitle = CreateText(toyPanel.transform, "Title", "Toy Collection", 28, Color.black, TextAnchor.MiddleCenter);
		toyTitle.fontStyle = FontStyle.Bold;
		SetRect(toyTitle.rectTransform, new Vector2(0.5f, 0.88f), new Vector2(0.5f, 0.88f), Vector2.zero, new Vector2(460f, 50f));
		int maxToys = ToyAlbumManager.Instance != null ? ToyAlbumManager.Instance.totalToys : 20;
		albumText = CreateText(toyPanel.transform, "AlbumProgressText", "0/" + maxToys + " Toys", 36, accentColor, TextAnchor.MiddleCenter);
		albumText.fontStyle = FontStyle.Bold;
		SetRect(albumText.rectTransform, new Vector2(0.5f, 0.65f), new Vector2(0.5f, 0.65f), Vector2.zero, new Vector2(460f, 80f));
		Text albumHint = CreateText(toyPanel.transform, "AlbumHint", "Pop balloons with toys inside to collect them and fill your album!", 22, new Color(0.25f, 0.25f, 0.28f, 1f), TextAnchor.MiddleCenter);
		SetRect(albumHint.rectTransform, new Vector2(0.5f, 0.40f), new Vector2(0.5f, 0.40f), Vector2.zero, new Vector2(460f, 180f));

		// C. Progress Map Popup
		progressMapPopup = CreatePopupBlocker(parent, "ProgressMapPopup");
		GameObject progressPanel = progressMapPopup.transform.Find("Window").gameObject;
		RectTransform progressWinRect = progressPanel.GetComponent<RectTransform>();
		progressWinRect.sizeDelta = new Vector2(540f, 780f);
		Text progressTitle = CreateText(progressPanel.transform, "Title", "Stage Selection", 28, Color.black, TextAnchor.MiddleCenter);
		progressTitle.fontStyle = FontStyle.Bold;
		SetRect(progressTitle.rectTransform, new Vector2(0.5f, 0.88f), new Vector2(0.5f, 0.88f), Vector2.zero, new Vector2(460f, 50f));
		starsText = CreateText(progressPanel.transform, "StarsText", "Stars: 0", 24, new Color(0.85f, 0.65f, 0f, 1f), TextAnchor.MiddleCenter);
		starsText.fontStyle = FontStyle.Bold;
		SetRect(starsText.rectTransform, new Vector2(0.5f, 0.78f), new Vector2(0.5f, 0.78f), Vector2.zero, new Vector2(460f, 40f));
		
		selectedStageText = CreateText(progressPanel.transform, "SelectedStageText", "Tap a stage below", 22, Color.black, TextAnchor.MiddleCenter);
		SetRect(selectedStageText.rectTransform, new Vector2(0.5f, 0.70f), new Vector2(0.5f, 0.70f), Vector2.zero, new Vector2(460f, 40f));

		CreateProgressionButtons(progressPanel.transform);

		// View Gallery button in Toy Collected popup
		Button viewGalleryBtn = CreateButton(toyPanel.transform, "ViewGalleryBtn", "VIEW ALBUM", new Color(0.3f, 0.7f, 0.9f, 1f));
		SetBtnStyle(viewGalleryBtn, Color.white, 20);
		SetRect(viewGalleryBtn.GetComponent<RectTransform>(), new Vector2(0.5f, 0.15f), new Vector2(0.5f, 0.15f), Vector2.zero, new Vector2(260f, 56f));
		viewGalleryBtn.onClick.AddListener(delegate {
			SoundManager.PlaySound("ButtonClick");
			OpenPopup(toyAlbumGalleryPopup);
			BuildToyGallery();
		});

		// Toy Album Gallery Popup
		toyAlbumGalleryPopup = CreatePopupBlocker(parent, "ToyAlbumGalleryPopup");
		GameObject galleryPanel = toyAlbumGalleryPopup.transform.Find("Window").gameObject;
		Text galleryTitle = CreateText(galleryPanel.transform, "Title", "Toy Album Gallery", 28, Color.black, TextAnchor.MiddleCenter);
		galleryTitle.fontStyle = FontStyle.Bold;
		SetRect(galleryTitle.rectTransform, new Vector2(0.5f, 0.88f), new Vector2(0.5f, 0.88f), Vector2.zero, new Vector2(460f, 50f));
		GameObject galleryHolder = CreatePanel(galleryPanel.transform, "GalleryHolder", Color.clear);
		RectTransform galleryHolderRect = galleryHolder.GetComponent<RectTransform>();
		SetRect(galleryHolderRect, new Vector2(0.5f, 0.52f), new Vector2(0.5f, 0.52f), Vector2.zero, new Vector2(480f, 400f));

		selectedToyStatusText = CreateText(galleryPanel.transform, "SelectedToyStatusText", "Tap a toy to inspect it!", 20, new Color(0.2f, 0.2f, 0.25f, 1f), TextAnchor.MiddleCenter);
		selectedToyStatusText.fontStyle = FontStyle.Bold;
		SetRect(selectedToyStatusText.rectTransform, new Vector2(0.5f, 0.22f), new Vector2(0.5f, 0.22f), Vector2.zero, new Vector2(460f, 40f));
		GridLayoutGroup grid = galleryHolder.AddComponent<GridLayoutGroup>();
		grid.cellSize = new Vector2(100f, 100f);
		grid.spacing = new Vector2(8f, 8f);
		grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
		grid.constraintCount = 4;

		// F. Combined Achievements & Daily Awards Popup
		achievementsPopup = CreatePopupBlocker(parent, "AchievementsPopup");
		GameObject achPanel = achievementsPopup.transform.Find("Window").gameObject;
		RectTransform achPanelRect = achPanel.GetComponent<RectTransform>();
		achPanelRect.sizeDelta = new Vector2(540f, 780f); // Make window taller to fit both sections

		Text achTitle = CreateText(achPanel.transform, "Title", "Daily Awards & Achievements", 28, Color.black, TextAnchor.MiddleCenter);
		achTitle.fontStyle = FontStyle.Bold;
		SetRect(achTitle.rectTransform, new Vector2(0.5f, 0.93f), new Vector2(0.5f, 0.93f), Vector2.zero, new Vector2(460f, 50f));

		// Daily Rewards UI Components inside achievementsPopup
		Text dailySectionHeader = CreateText(achPanel.transform, "DailyHeader", "— Daily Reward —", 22, accentColor, TextAnchor.MiddleCenter);
		dailySectionHeader.fontStyle = FontStyle.Bold;
		SetRect(dailySectionHeader.rectTransform, new Vector2(0.5f, 0.85f), new Vector2(0.5f, 0.85f), Vector2.zero, new Vector2(460f, 40f));

		dailyStreakText = CreateText(achPanel.transform, "DailyStreakText", "Day 1 Streak!", 20, new Color(0.85f, 0.65f, 0f, 1f), TextAnchor.MiddleCenter);
		dailyStreakText.fontStyle = FontStyle.Bold;
		SetRect(dailyStreakText.rectTransform, new Vector2(0.5f, 0.79f), new Vector2(0.5f, 0.79f), Vector2.zero, new Vector2(460f, 35f));

		dailyRewardText = CreateText(achPanel.transform, "DailyRewardText", "Reward: 100 coins", 18, new Color(0.15f, 0.15f, 0.15f, 1f), TextAnchor.MiddleCenter);
		SetRect(dailyRewardText.rectTransform, new Vector2(0.5f, 0.73f), new Vector2(0.5f, 0.73f), Vector2.zero, new Vector2(460f, 35f));

		dailyClaimBtn = CreateButton(achPanel.transform, "DailyClaimBtn", "CLAIM", new Color(0.2f, 0.8f, 0.3f, 1f));
		SetRect(dailyClaimBtn.GetComponent<RectTransform>(), new Vector2(0.5f, 0.63f), new Vector2(0.5f, 0.63f), Vector2.zero, new Vector2(200f, 45f));
		SetBtnStyle(dailyClaimBtn, Color.white, 20);
		dailyClaimBtn.onClick.AddListener(ClaimDailyRewardFromPopup);

		// Achievements UI Components inside achievementsPopup
		Text achSectionHeader = CreateText(achPanel.transform, "AchievementsHeader", "— Achievements —", 22, accentColor, TextAnchor.MiddleCenter);
		achSectionHeader.fontStyle = FontStyle.Bold;
		SetRect(achSectionHeader.rectTransform, new Vector2(0.5f, 0.52f), new Vector2(0.5f, 0.52f), Vector2.zero, new Vector2(460f, 40f));

		GameObject achHolder = CreatePanel(achPanel.transform, "AchievementsHolder", Color.clear);
		RectTransform achHolderRect = achHolder.GetComponent<RectTransform>();
		SetRect(achHolderRect, new Vector2(0.5f, 0.33f), new Vector2(0.5f, 0.33f), Vector2.zero, new Vector2(480f, 240f));
		VerticalLayoutGroup achLayout = achHolder.AddComponent<VerticalLayoutGroup>();
		achLayout.spacing = 4f;
		achLayout.childAlignment = TextAnchor.MiddleCenter;
		ContentSizeFitter achFitter = achHolder.AddComponent<ContentSizeFitter>();
		achFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
	}

	GameObject CreatePopupBlocker(Transform parent, string name)
	{
		// Blocker overlay panel
		GameObject blocker = CreatePanel(parent, name, new Color(0f, 0f, 0f, 0.75f));
		SetFullStretch(blocker.GetComponent<RectTransform>());

		// Blocker Button to intercept clicks and allow closing when clicking outside the popup window
		Button blockerBtn = blocker.AddComponent<Button>();
		blockerBtn.onClick.AddListener(delegate {
			SoundManager.PlaySound("ButtonClick");
			blocker.SetActive(false);
		});

		// Popup window
		GameObject window = CreatePanel(blocker.transform, "Window", panelColor);
		SetRect(window.GetComponent<RectTransform>(), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(540f, 720f));
		Outline outline = window.AddComponent<Outline>();
		outline.effectColor = accentColor;
		outline.effectDistance = new Vector2(4f, 4f);

		// Add dummy button on window to stop click propagation to blocker
		Button windowDummyBtn = window.AddComponent<Button>();
		windowDummyBtn.transition = Selectable.Transition.None;

		// Close Button at bottom of window
		Button closeBtn = CreateButton(window.transform, "CloseButton", "CLOSE", new Color(0.9f, 0.35f, 0.35f, 1f));
		SetBtnStyle(closeBtn, Color.white, 22);
		SetRect(closeBtn.GetComponent<RectTransform>(), new Vector2(0.5f, 0.12f), new Vector2(0.5f, 0.12f), Vector2.zero, new Vector2(240f, 64f));
		closeBtn.onClick.AddListener(delegate {
			SoundManager.PlaySound("ButtonClick");
			blocker.SetActive(false);
		});

		blocker.SetActive(false); // Hide by default
		return blocker;
	}

	void OpenPopup(GameObject popup)
	{
		SoundManager.PlaySound("ButtonClick");
		if (popup != null)
		{
			popup.SetActive(true);
		}
	}

	void CreateThemeButtons(Transform parent)
	{
		string[] names = new string[] { "Classic", "Ocean", "Candy" };
		Color[] colors = new Color[] { accentColor, new Color(0.12f, 0.45f, 0.95f, 1f), new Color(0.95f, 0.25f, 0.58f, 1f) };

		for (int i = 0; i < names.Length; i++)
		{
			int index = i;
			Button button = CreateButton(parent, "ThemeButton" + i.ToString(), names[i], colors[i]);
			SetBtnStyle(button, Color.white, 20);
			SetRect(button.GetComponent<RectTransform>(), new Vector2(0.5f, 0.48f - i * 0.12f), new Vector2(0.5f, 0.48f - i * 0.12f), Vector2.zero, new Vector2(300f, 60f));
			button.onClick.AddListener(delegate {
				SoundManager.PlaySound("ButtonClick");
				SelectTheme(index);
			});
		}
	}

	void CreateProgressionButtons(Transform parent)
	{
		for (int i = 0; i < 5; i++)
		{
			int index = i;
			string label = "Stage " + (i + 1);
			Button button = CreateButton(parent, "StageButton" + i.ToString(), label, buttonColor);
			SetBtnStyle(button, Color.white, 20);
			SetRect(button.GetComponent<RectTransform>(), new Vector2(0.5f, 0.56f - i * 0.10f), new Vector2(0.5f, 0.56f - i * 0.10f), Vector2.zero, new Vector2(340f, 54f));
			button.onClick.AddListener(delegate {
				SoundManager.PlaySound("ButtonClick");
				SelectStage(index);
			});
		}
	}

	void WireFeatureManagers()
	{
		if (ToyAlbumManager.Instance != null)
			ToyAlbumManager.Instance.albumProgressText = albumText;

		if (MissionManager.Instance != null)
			MissionManager.Instance.missionText = missionText;

		if (ThemeProgressionManager.Instance != null)
			ThemeProgressionManager.Instance.progressionText = starsText;

		if (ProgressionMapManager.Instance != null)
			ProgressionMapManager.Instance.selectedLevelText = selectedStageText;

		DailyRewards dailyRewardsComp = GameObject.FindObjectOfType<DailyRewards>();
		if (dailyRewardsComp != null)
			dailyRewardsComp.dailyRewardPopup = achievementsPopup;
	}

	string GetStageChallengeDescription(int stageIndex)
	{
		switch (stageIndex)
		{
			case 0: return "Pop 20 Balloons";
			case 1: return "Score 40 Points";
			case 2: return "Collect 2 Toys";
			case 3: return "Score 60 Points";
			case 4: return "Collect 3 Toys";
			default: return "Pop 20 Balloons";
		}
	}

	void RefreshDashboard()
	{
		if (ToyAlbumManager.Instance != null)
			ToyAlbumManager.Instance.UpdateAlbumText();

		if (MissionManager.Instance != null)
			MissionManager.Instance.RefreshMissionText();

		int stars = ThemeProgressionManager.Stars;
		if (starsText != null)
			starsText.text = "Stars: " + stars.ToString();
		if (dashboardStarsTextPreview != null)
			dashboardStarsTextPreview.text = "Stars: " + stars.ToString();

		int stageIndex = Mathf.Clamp(PlayerPrefs.GetInt("SelectedProgressionLevel", 0), 0, 4);
		string stageName = "Stage " + (stageIndex + 1).ToString();
		string challengeDesc = GetStageChallengeDescription(stageIndex);
		if (selectedStageText != null)
			selectedStageText.text = stageName + " (" + challengeDesc + ")";
		if (dashboardStageTextPreview != null)
			dashboardStageTextPreview.text = "Stage: " + stageName + " (" + challengeDesc + ")";

		int toysCount = ToyAlbumManager.GetUnlockedToyCount();
		int maxToys = ToyAlbumManager.Instance != null ? ToyAlbumManager.Instance.totalToys : 20;
		if (dashboardToysTextPreview != null)
			dashboardToysTextPreview.text = "Toys: " + toysCount.ToString() + "/" + maxToys.ToString();

		int coins = ShopManager.GetCoins();
		if (dashboardCoinsTextPreview != null)
			dashboardCoinsTextPreview.text = "Coins: " + coins.ToString();

		int highScore = PlayerPrefs.GetInt("ScoreOfTheHighes", 0);
		if (dashboardHighScoreTextPreview != null)
			dashboardHighScoreTextPreview.text = "High Score: " + highScore.ToString();

		UpdateThemeTextAndBackground();
	}

	void SelectTheme(int index)
	{
		if (ThemeProgressionManager.Instance != null)
		{
			ThemeProgressionManager.Instance.SelectTheme(index);
		}
		else
		{
			PlayerPrefs.SetInt("SelectedTheme", index);
			PlayerPrefs.Save();
		}
		UpdateThemeTextAndBackground();
		RefreshDashboard();
	}

	void UpdateThemeTextAndBackground()
	{
		int index = Mathf.Clamp(PlayerPrefs.GetInt("SelectedTheme", 0), 0, 2);
		if (selectedThemeText != null)
			selectedThemeText.text = "Theme: " + (index == 0 ? "Classic" : index == 1 ? "Ocean" : "Candy");

		if (dashboardThemeTextPreview != null)
			dashboardThemeTextPreview.text = "Theme: " + (index == 0 ? "Classic" : index == 1 ? "Ocean" : "Candy");

		// Apply background dynamically from selected theme
		if (ThemeProgressionManager.Instance != null && ThemeProgressionManager.Instance.themes != null && index < ThemeProgressionManager.Instance.themes.Length)
		{
			Sprite bgSprite = ThemeProgressionManager.Instance.themes[index].backgroundSprite;
			if (bgSprite != null && backgroundImageComponent != null)
			{
				backgroundImageComponent.sprite = bgSprite;
				backgroundImageComponent.color = Color.white;
			}
		}
		else if (backgroundImageComponent != null)
		{
			// Generate a beautiful, premium dual-color gradient fallback dynamically
			backgroundImageComponent.sprite = CreateGradientSprite(backgroundColorA, backgroundColorB);
			backgroundImageComponent.color = Color.white;
		}
	}

	Sprite CreateGradientSprite(Color colorA, Color colorB)
	{
		Texture2D tex = new Texture2D(2, 2);
		tex.SetPixel(0, 0, colorA);
		tex.SetPixel(1, 0, colorA);
		tex.SetPixel(0, 1, colorB);
		tex.SetPixel(1, 1, colorB);
		tex.Apply();
		return Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f));
	}

	void SelectStage(int index)
	{
		if (ProgressionMapManager.Instance != null)
			ProgressionMapManager.Instance.SelectLevel(index);
		else
		{
			PlayerPrefs.SetInt("SelectedProgressionLevel", index);
			PlayerPrefs.Save();
		}

		if (MissionManager.Instance != null)
		{
			MissionManager.Instance.ResetMissionForSelectedStage();
		}

		RefreshDashboard();
	}

	void StartLevel()
	{
		SoundManager.PlaySound("ButtonClick");
		Application.LoadLevel("Level");
	}

	void GoHome()
	{
		SoundManager.PlaySound("ButtonClick");
		Application.LoadLevel("MainScene");
	}

	void ResetProgress()
	{
		SoundManager.PlaySound("ButtonClick");
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
		
		if (ThemeProgressionManager.Instance != null)
		{
			ThemeProgressionManager.Instance.SelectTheme(0);
		}
		if (MissionManager.Instance != null)
		{
			MissionManager.Instance.ResetMissionProgress();
		}
		
		RefreshDashboard();
	}

	GameObject CreatePanel(Transform parent, string name, Color color)
	{
		GameObject go = new GameObject(name);
		go.transform.SetParent(parent, false);
		Image image = go.AddComponent<Image>();
		image.color = color;
		return go;
	}

	Text CreateText(Transform parent, string name, string value, int size, Color color, TextAnchor anchor)
	{
		GameObject go = new GameObject(name);
		go.transform.SetParent(parent, false);
		Text text = go.AddComponent<Text>();
		text.font = defaultFont;
		text.text = value;
		text.fontSize = size;
		text.color = color;
		text.alignment = anchor;
		text.resizeTextForBestFit = true;
		text.resizeTextMinSize = 12;
		text.resizeTextMaxSize = size;
		return text;
	}

	Button CreateButton(Transform parent, string name, string label, Color color)
	{
		GameObject go = CreatePanel(parent, name, color);
		Button button = go.AddComponent<Button>();
		Text text = CreateText(go.transform, "Text", label, 22, Color.black, TextAnchor.MiddleCenter);
		SetFullStretch(text.rectTransform);
		return button;
	}

	void SetFullStretch(RectTransform rect)
	{
		rect.anchorMin = Vector2.zero;
		rect.anchorMax = Vector2.one;
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;
	}

	void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 size)
	{
		rect.anchorMin = anchorMin;
		rect.anchorMax = anchorMax;
		rect.pivot = new Vector2(0.5f, 0.5f);
		rect.anchoredPosition = anchoredPosition;
		rect.sizeDelta = size;
	}

	void ClaimDailyRewardFromPopup()
	{
		int level = Mathf.Clamp(PlayerPrefs.GetInt("LevelReward", 1), 1, 6);
		int rewardAmount = level >= 0 && level < DailyRewards.DailyRewardAmount.Length ? DailyRewards.DailyRewardAmount[level] : 0;

		SoundManager.PlaySound("ButtonClick");
		SoundManager.PlaySound("CoinCollect");

		// Give reward
		ShopManager.AddCoins(rewardAmount);

		// Save claim date
		PlayerPrefs.SetString("VremeQuit", System.DateTime.Now.ToString());
		PlayerPrefs.Save();

		// Refresh UI
		RefreshAchievementsPopup();
		RefreshDashboard();
	}

	void BuildShopPopupContent()
	{
		if (shopPopup == null || ShopManager.Instance == null || ShopManager.Instance.shopItems == null)
			return;

		GameObject shopPanel = shopPopup.transform.Find("Window").gameObject;
		Transform existingHolder = shopPanel.transform.Find("ShopItemsHolder");
		if (existingHolder == null) return;

		foreach (Transform child in existingHolder)
			Destroy(child.gameObject);

		for (int i = 0; i < ShopManager.Instance.shopItems.Length; i++)
		{
			int idx = i;
			ShopItem item = ShopManager.Instance.shopItems[i];

			GameObject row = CreatePanel(existingHolder, "ShopItem" + i, new Color(0.95f, 0.95f, 0.95f, 1f));
			RectTransform rowRect = row.GetComponent<RectTransform>();
			if (rowRect != null)
				SetRect(rowRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(440f, 64f));
			Outline rowOutline = row.AddComponent<Outline>();
			rowOutline.effectColor = new Color(0f, 0f, 0f, 0.1f);
			rowOutline.effectDistance = new Vector2(1f, 1f);

			Text nameText = CreateText(row.transform, "NameText", item.itemName + " (" + item.price + " coins)", 18, new Color(0.15f, 0.15f, 0.15f, 1f), TextAnchor.MiddleLeft);
			SetRect(nameText.rectTransform, new Vector2(0f, 0.5f), new Vector2(0.7f, 0.5f), new Vector2(12f, 0f), new Vector2(300f, 40f));

			Button buyBtn = CreateButton(row.transform, "BuyBtn", "BUY", new Color(0.2f, 0.8f, 0.3f, 1f));
			SetRect(buyBtn.GetComponent<RectTransform>(), new Vector2(0.85f, 0.5f), new Vector2(0.85f, 0.5f), Vector2.zero, new Vector2(80f, 44f));
			SetBtnStyle(buyBtn, Color.white, 16);
			buyBtn.onClick.AddListener(delegate {
				SoundManager.PlaySound("ButtonClick");
				if (ShopManager.Instance.BuyItem(idx))
					BuildShopPopupContent();
			});
		}
	}

	void RefreshAchievementsPopup()
	{
		// 1. Refresh Daily Rewards claim status
		int level = Mathf.Clamp(PlayerPrefs.GetInt("LevelReward", 1), 1, 6);
		int rewardAmount = level >= 0 && level < DailyRewards.DailyRewardAmount.Length ? DailyRewards.DailyRewardAmount[level] : 0;

		if (dailyStreakText != null)
			dailyStreakText.text = "Day " + level + " Streak!";
		if (dailyRewardText != null)
			dailyRewardText.text = "Reward: " + rewardAmount + " coins";

		bool alreadyClaimed = false;
		if (PlayerPrefs.HasKey("VremeQuit"))
		{
			string lastQuitStr = PlayerPrefs.GetString("VremeQuit");
			System.DateTime lastQuitDate;
			if (System.DateTime.TryParse(lastQuitStr, out lastQuitDate))
			{
				if (lastQuitDate.Date == System.DateTime.Now.Date)
				{
					alreadyClaimed = true;
				}
			}
		}

		if (dailyClaimBtn != null)
		{
			if (alreadyClaimed)
			{
				dailyClaimBtn.interactable = false;
				dailyClaimBtn.GetComponentInChildren<Text>().text = "CLAIMED";
				dailyClaimBtn.GetComponent<Image>().color = Color.gray;
			}
			else
			{
				dailyClaimBtn.interactable = true;
				dailyClaimBtn.GetComponentInChildren<Text>().text = "CLAIM";
				dailyClaimBtn.GetComponent<Image>().color = new Color(0.2f, 0.8f, 0.3f, 1f);
			}
		}

		// 2. Refresh Achievements list
		if (achievementsPopup == null || AchievementManager.Instance == null || AchievementManager.Instance.achievements == null)
			return;

		GameObject achPanel = achievementsPopup.transform.Find("Window").gameObject;
		Transform achHolder = achPanel.transform.Find("AchievementsHolder");
		if (achHolder == null) return;

		foreach (Transform child in achHolder)
			Destroy(child.gameObject);

		for (int i = 0; i < AchievementManager.Instance.achievements.Length; i++)
		{
			Achievement ach = AchievementManager.Instance.achievements[i];
			if (ach == null) continue;

			Color bgColor = ach.unlocked ? new Color(0.8f, 1f, 0.8f, 1f) : new Color(0.9f, 0.9f, 0.9f, 1f);
			GameObject row = CreatePanel(achHolder, "AchItem" + i, bgColor);
			RectTransform rowRect = row.GetComponent<RectTransform>();
			if (rowRect != null)
				SetRect(rowRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(440f, 44f));

			string status = ach.unlocked ? " [DONE]" : " (" + ach.currentAmount + "/" + ach.targetAmount + ")";
			Text text = CreateText(row.transform, "AchText", ach.title + status, 18, ach.unlocked ? new Color(0f, 0.5f, 0f, 1f) : new Color(0.3f, 0.3f, 0.3f, 1f), TextAnchor.MiddleLeft);
			
			// Fix alignment by stretching to fit the row and giving a nice horizontal padding
			SetFullStretch(text.rectTransform);
			text.rectTransform.offsetMin = new Vector2(15f, 0f);
			text.rectTransform.offsetMax = new Vector2(-15f, 0f);
		}
	}

	void BuildToyGallery()
	{
		if (toyAlbumGalleryPopup == null) return;

		GameObject galleryPanel = toyAlbumGalleryPopup.transform.Find("Window").gameObject;
		Transform galleryHolder = galleryPanel.transform.Find("GalleryHolder");
		if (galleryHolder == null) return;

		if (selectedToyStatusText != null)
		{
			selectedToyStatusText.text = "Tap a toy to inspect it!";
			selectedToyStatusText.color = new Color(0.2f, 0.2f, 0.25f, 1f);
		}

		foreach (Transform child in galleryHolder)
			Destroy(child.gameObject);

		int totalToys = ToyAlbumManager.Instance != null ? ToyAlbumManager.Instance.totalToys : 20;
		for (int i = 0; i < totalToys; i++)
		{
			int index = i;
			bool unlocked = ToyAlbumManager.IsToyUnlocked(index);

			GameObject cell = CreatePanel(galleryHolder, "ToyCell" + index, unlocked ? new Color(0.85f, 1f, 0.85f, 1f) : new Color(0.5f, 0.5f, 0.5f, 0.3f));
			Outline cellOutline = cell.AddComponent<Outline>();
			cellOutline.effectColor = unlocked ? new Color(0f, 0.5f, 0f, 0.5f) : new Color(0.2f, 0.2f, 0.2f, 0.3f);
			cellOutline.effectDistance = new Vector2(1f, 1f);

			Button cellBtn = cell.AddComponent<Button>();
			cellBtn.onClick.AddListener(delegate {
				SoundManager.PlaySound("ButtonClick");
				ShowToyDetails(index);
			});

			string label = unlocked ? "Toy " + (index + 1) : "???";
			Text labelText = CreateText(cell.transform, "ToyLabel", label, 16, unlocked ? new Color(0f, 0.4f, 0f, 1f) : new Color(0.5f, 0.5f, 0.5f, 1f), TextAnchor.MiddleCenter);
			SetFullStretch(labelText.rectTransform);
		}
	}

	void ShowToyDetails(int index)
	{
		if (selectedToyStatusText == null) return;

		bool unlocked = ToyAlbumManager.IsToyUnlocked(index);
		if (unlocked)
		{
			string toyName = "Toy " + (index + 1);
			if (LevelManager.levelManager != null && LevelManager.levelManager.listOfToys != null && index < LevelManager.levelManager.listOfToys.Length)
			{
				GameObject toyPrefab = LevelManager.levelManager.listOfToys[index];
				if (toyPrefab != null)
				{
					toyName = toyPrefab.name;
				}
			}
			selectedToyStatusText.text = "You obtained: " + toyName + "!";
			selectedToyStatusText.color = new Color(0f, 0.5f, 0f, 1f);
		}
		else
		{
			selectedToyStatusText.text = "This toy is locked. Keep popping!";
			selectedToyStatusText.color = new Color(0.5f, 0.2f, 0.2f, 1f);
		}
	}
}
