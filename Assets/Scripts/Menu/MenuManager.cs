using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

	/**
  * Scene:All
  * Object:Canvas
  * Description: Skripta zaduzena za hendlovanje(prikaz i sklanjanje svih Menu-ja,njihovo paljenje i gasenje, itd...)
  **/
public class MenuManager : MonoBehaviour 
{
	
	public Menu currentMenu;
	Menu currentPopUpMenu;
//	[HideInInspector]
//	public Animator openObject;
	public GameObject[] disabledObjects;
	GameObject ratePopUp, crossPromotionInterstitial;
	public bool popupOpened;
	public bool popupFullyOpened;
	private bool startInterstitialOpened;
	public GameObject soundButton;
	public GameObject loadingHolder;

	// Popup message components
	public GameObject popupMessageObject;
	public Text popupMessageText;
	public Text popupMessageHeaderText;

	public GameObject leaderboardButton;
	GameObject creditsButton;

//	public FacebookNativeAd mainSceneNativeAd;
	
	void Start () 
	{
		if(Application.loadedLevelName=="MainScene")
		{
			crossPromotionInterstitial = GameObject.Find("PopUps/PopUpInterstitial");
			ratePopUp = GameObject.Find("PopUps/PopUpRate");
		}

		if (disabledObjects!=null) {
			for(int i=0;i<disabledObjects.Length;i++)
			{
				// Debug.Log("Gasi "+disabledObjects[i].name);
				disabledObjects[i].SetActive(false);
			}
		}
		
		if(Application.loadedLevelName != "MapScene")
			ShowMenu(currentMenu.gameObject);	
		
		if(Application.loadedLevelName=="MainScene")
		{
			if(PlayerPrefs.HasKey("alreadyRated"))
			{
				Rate.alreadyRated = PlayerPrefs.GetInt("alreadyRated");
			}
			else
			{
				Rate.alreadyRated = 0;
			}
			
			if(Rate.alreadyRated==0)
			{
				Rate.appStartedNumber = PlayerPrefs.GetInt("appStartedNumber");
				Debug.Log("appStartedNumber "+Rate.appStartedNumber);
				
				if(Rate.appStartedNumber>=6)
				{
					Rate.appStartedNumber=0;
					PlayerPrefs.SetInt("appStartedNumber",Rate.appStartedNumber);
					PlayerPrefs.Save();
					ShowPopUpMenu(ratePopUp);
				}
			}
		}

		if (Application.loadedLevelName=="MainScene" && GlobalVariables.shouldShowLeaderboard)
		{
			leaderboardButton.SetActive(true);
		}

		if (Application.loadedLevelName == "MainScene")
		{
			ApplyTinyPoppersTextReplacements();
			RemoveTopTitleAndLyrics();
			EnlargeTinyPoppersLogo();
			EnsureCreditsButton();
		}

		popupOpened = false;
		popupFullyOpened = false;
		
	}
	
	/// <summary>
	/// Funkcija koja pali(aktivira) objekat
	/// </summary>
	/// /// <param name="gameObject">Game object koji se prosledjuje i koji treba da se upali</param>
	public void EnableObject(GameObject gameObject)
	{
		
		if (gameObject != null) 
		{
			if (!gameObject.activeSelf) 
			{
				gameObject.SetActive (true);
			}
		}
	}

	/// <summary>
	/// Funkcija koja gasi objekat
	/// </summary>
	/// /// <param name="gameObject">Game object koji se prosledjuje i koji treba da se ugasi</param>
	public void DisableObject(GameObject gameObject)
	{
		Debug.Log("Disable Object");
		if (gameObject != null) 
		{
			if (gameObject.activeSelf) 
			{
				gameObject.SetActive (false);
			}
		}
	}
	
	/// <summary>
	/// F-ja koji poziva ucitavanje Scene
	/// </summary>
	/// <param name="levelName">Level name.</param>
	public void LoadScene(string levelName )
	{
		if (Application.loadedLevelName == "MainScene" && levelName == "Level")
			levelName = "Dashboard";

		if (levelName != "") {
			try {
//				if (levelName == "MainScene" && Application.loadedLevelName == "CharacterSelect")
//					GlobalVariables.playLoadingDepart = false;

				Application.LoadLevel (levelName);
			} catch (System.Exception e) {
				Debug.Log ("Can't load scene: " + e.Message);
			}
		} else {
			Debug.Log ("Can't load scene: Level name to set");
		}
	}
	
	/// <summary>
	/// F-ja koji poziva asihrono ucitavanje Scene
	/// </summary>
	/// <param name="levelName">Level name.</param>
	public void LoadSceneAsync(string levelName )
	{
		if (Application.loadedLevelName == "MainScene" && levelName == "Level")
			levelName = "Dashboard";

		if (levelName != "") {
			try {
				Application.LoadLevelAsync (levelName);
			} catch (System.Exception e) {
				Debug.Log ("Can't load scene: " + e.Message);
			}
		} else {
			Debug.Log ("Can't load scene: Level name to set");
		}
	}

	/// <summary>
	/// Funkcija za prikaz Menu-ja koji je pozvan kao Menu
	/// </summary>
	/// /// <param name="menu">Game object koji se prosledjuje i treba da se skloni, mora imati na sebi skriptu Menu.</param>
	public void ShowMenu(GameObject menu)
	{
		Debug.Log("ShowMwnu   " + menu);
		if (currentMenu != null)
		{
			currentMenu.IsOpen = false;
			currentMenu.gameObject.SetActive(false);
		}

		currentMenu = menu.GetComponent<Menu> ();
		menu.gameObject.SetActive (true);
		currentMenu.IsOpen = true;
		
	}

	/// <summary>
	/// Funkcija za zatvaranje Menu-ja koji je pozvan kao Meni
	/// </summary>
	/// /// <param name="menu">Game object koji se prosledjuje za prikaz, mora imati na sebi skriptu Menu.</param>
	public void CloseMenu(GameObject menu)
	{
		Debug.Log("CloseMenu");
		if (menu != null) 
		{
			menu.GetComponent<Menu> ().IsOpen = false;
			menu.SetActive (false);
		}
	}

	/// <summary>
	/// Funkcija za prikaz Menu-ja koji je pozvan kao PopUp-a
	/// </summary>
	/// /// <param name="menu">Game object koji se prosledjuje za prikaz, mora imati na sebi skriptu Menu.</param>
	public void ShowPopUpMenu(GameObject menu)
	{
//		Debug.Log(menu.name);
//		Debug.Log("popupOpened: "    + popupOpened);
//		Debug.Log("popupFullyOpened: "    + popupFullyOpened);

		if (!popupOpened && !popupFullyOpened)
		{
			popupOpened = true;

			menu.gameObject.SetActive (true);

			currentPopUpMenu = menu.GetComponent<Menu> ();
			currentPopUpMenu.IsOpen = true;

//			if (menu.name == "PopUpCrossPromotionOfferWall" || menu.name == "PopUpMessage" || menu.name == "PopUpTutorial")
//			{
//				mainSceneNativeAd.CancelLoading();
//				mainSceneNativeAd.HideNativeAd();
//			}

			SoundManager.PlaySound("PopupArrive");

			StartCoroutine(WaitForPopupToBeFullyOpened());
		}

//		if (menu.name == "VideoNotAvailable")
//		{
//			menu.gameObject.SetActive (true);
//
//			currentPopUpMenu = menu.GetComponent<Menu> ();
//			currentPopUpMenu.IsOpen = true;
//		}
	}

	IEnumerator WaitForPopupToBeFullyOpened()
	{
		yield return new WaitForSeconds(1.2f);
		
		if (popupOpened)
			popupFullyOpened = true;
	}

	/// <summary>
	/// Funkcija za zatvaranje Menu-ja koji je pozvan kao PopUp-a, poziva inace coroutine-u, ima delay zbog animacije odlaska Menu-ja
	/// </summary>
	/// /// <param name="menu">Game object koji se prosledjuje i treba da se skloni, mora imati na sebi skriptu Menu.</param>
	public void ClosePopUpMenu(GameObject menu)
	{
		Debug.Log("ClosePopUpMenu");
		StartCoroutine("HidePopUp",menu);

		if (GlobalVariables.quitGame)
			Application.Quit();
	}

	/// <summary>
	/// Couorutine-a za zatvaranje Menu-ja koji je pozvan kao PopUp-a
	/// </summary>
	/// /// <param name="menu">Game object koji se prosledjuje, mora imati na sebi skriptu Menu.</param>
	IEnumerator HidePopUp(GameObject menu)
	{
//		SoundManager.Instance.PlayPopupDepartSound();

		menu.GetComponent<Menu> ().IsOpen = false;

//		if (menu.name == "PopUpCrossPromotionOfferWall" || menu.name == "PopUpMessage" || menu.name == "PopUpInterstitial" || menu.name == "PopUpRate" || menu.name == "PopUpTutorial")
//		{
//			mainSceneNativeAd.LoadAd();
//		}

		if (menu.name == "PopUpPause")
			yield return new WaitForSeconds(0.17f);
		else
			yield return new WaitForSeconds(1.2f);

		Debug.Log(menu.name);

//		popupOpened = false;
//		popupFullyOpened = false;

		// if menu is one of the popup bought menus set popup to be opened and set last opened popup to be shop popup
//		if (menu.name == "VideoNotAvailable")
//		{
////			Debug.Log("Minja close test");
////				popupOpened = true;
////				popupFullyOpened = true;
////	
//			currentPopUpMenu = GameObject.Find("PopUpShop").GetComponent<Menu> ();
//		}
//		else
//		{
			popupOpened = false;
			popupFullyOpened = false;
//		}

		if (Application.loadedLevelName == "HospitalSelect")
			GameObject.Find("BackButton").GetComponent<Button>().enabled = true;

		menu.SetActive (false);
	}

	/// <summary>
	/// Funkcija za prikaz poruke preko Log-a, prilikom klika na dugme
	/// </summary>
	/// /// <param name="message">poruka koju treba prikazati.</param>
	public void ShowMessage(string message)
	{
		Debug.Log(message);
	}

	/// <summary>
	/// Funkcija koja podesava naslov dialoga kao i poruku u dialogu i ova f-ja se poziva iz skripte
	/// </summary>
	/// <param name="messageTitleText">naslov koji treba prikazati.</param>
	/// <param name="messageText">custom poruka koju treba prikazati.</param>
	public void ShowPopUpMessage(string messageTitleText, string messageText)
	{
		popupMessageHeaderText.text=messageTitleText;
		popupMessageText.text=messageText;
		ShowPopUpMenu(popupMessageObject);

	}

	/// <summary>
	/// Funkcija koja podesava naslov CustomMessage-a, i ova f-ja se poziva preko button-a zajedno za f-jom ShowPopUpMessageCustomMessageText u redosledu: 1-ShowPopUpMessageTitleText 2-ShowPopUpMessageCustomMessageText
	/// </summary>
	/// <param name="messageTitleText">naslov koji treba prikazati.</param>
	public void ShowPopUpMessageTitleText(string messageTitleText)
	{
		popupMessageHeaderText.text=messageTitleText;
	}

	/// <summary>
	/// Funkcija koja podesava poruku CustomMessage, i poziva meni u vidu pop-upa, ova f-ja se poziva preko button-a zajedno za f-jom ShowPopUpMessageTitleText u redosledu: 1-ShowPopUpMessageTitleText 2-ShowPopUpMessageCustomMessageText
	/// </summary>
	/// <param name="messageText">custom poruka koju treba prikazati.</param>
	public void ShowPopUpMessageCustomMessageText(string messageText)
	{
		popupMessageText.text=messageText;		
		ShowPopUpMenu(popupMessageObject);
	}

	/// <summary>
	/// Funkcija koja podesava naslov dialoga kao i poruku u dialogu i ova f-ja se poziva iz skripte
	/// </summary>
	/// <param name="dialogTitleText">naslov koji treba prikazati.</param>
	/// <param name="dialogMessageText">custom poruka koju treba prikazati.</param>
	public void ShowPopUpDialog(string dialogTitleText, string dialogMessageText)
	{
		popupMessageHeaderText.text=dialogTitleText;
		popupMessageText.text=dialogMessageText;
		ShowPopUpMenu(popupMessageObject);
	}

	/// <summary>
	/// Funkcija koja podesava naslov dialoga, ova f-ja se poziva preko button-a zajedno za f-jom ShowPopUpDialogCustomMessageText u redosledu: 1-ShowPopUpDialogTitleText 2-ShowPopUpDialogCustomMessageText
	/// </summary>
	/// <param name="dialogTitleText">naslov koji treba prikazati.</param>
	public void ShowPopUpDialogTitleText(string dialogTitleText)
	{
		popupMessageHeaderText.text=dialogTitleText;
	}

	/// <summary>
	/// Funkcija koja podesava poruku dialoga i poziva meni u vidu pop-upa, ova f-ja se poziva preko button-a zajedno za f-jom ShowPopUpDialogTitleText u redosledu: 1-ShowPopUpDialogTitleText 2-ShowPopUpDialogCustomMessageText
	/// </summary>
	/// <param name="dialogMessageText">custom poruka koju treba prikazati.</param>
	public void ShowPopUpDialogCustomMessageText(string dialogMessageText)
	{
		popupMessageText.text=dialogMessageText;		
		ShowPopUpMenu(popupMessageObject);
	}

	void Awake()
	{
		FeatureBootstrapper.EnsureManagers();

		// Set sound button
		if (soundButton != null && SoundManager.soundOn == 0)
		{
			if (SoundManager.Instance != null)
			{
				soundButton.GetComponent<Image>().sprite = SoundManager.Instance.musicOffImageHolder;
				soundButton.transform.GetChild(0).GetComponent<Image>().sprite = SoundManager.Instance.musicOffSprite;
			}
		}

		if (Application.loadedLevelName == "MainScene" && GlobalVariables.removeAdsOwned)
			GameObject.Find("RemoveAdsButton").SetActive(false);

		StartCoroutine(StartSceneWithLoadingHolderCoroutine());
	}

	public void ToggleSound()
	{
		SoundManager.ToggleSound();
	}

	void Update()
	{
		// Check exit interstitial
		if (Input.GetKeyDown(KeyCode.Escape) && Application.loadedLevel == 2 && !popupOpened && !startInterstitialOpened)
			Application.Quit();

		if (Input.GetKeyDown(KeyCode.Escape) && !popupOpened && Application.loadedLevel == 3)
		{

			LevelManager.levelManager.PauseGame();
		}
		else if (Input.GetKeyDown(KeyCode.Escape) && popupOpened && popupFullyOpened && Application.loadedLevel != 3)
        {
            ClosePopUpMenu(currentPopUpMenu.gameObject);
		}
		else if (Input.GetKeyDown(KeyCode.Escape) && popupOpened && Application.loadedLevel == 3)
			LevelManager.levelManager.ContinueGame();

		if (Input.GetKeyDown(KeyCode.Escape) && Application.loadedLevel == 2 && startInterstitialOpened)
		{
			startInterstitialOpened = false;
			ClosePopUpMenu(disabledObjects[2]);
		}

		if (Input.GetKeyDown(KeyCode.Escape) && GlobalVariables.quitGame)
			Application.Quit();
	}

	public void OpenLevelScene(string levelName)
	{
		if (Application.loadedLevelName == "MainScene" && levelName == "Level")
			levelName = "Dashboard";

		Application.LoadLevel(levelName);
	}

	// Play button click sound
	public void PlayButtonClickSound()
	{
		SoundManager.PlaySound("ButtonClick");
	}

	public void RateButtonClicked()
	{
		Application.OpenURL("market://details?" + GlobalVariables.applicationID);
	}

	public void LoadSceneWithLoading(string sceneName)
	{
		if (Application.loadedLevelName == "MainScene" && sceneName == "Level")
			sceneName = "Dashboard";

		StartCoroutine(LoadSceneWithLoadingCoroutine(sceneName));
	}

	IEnumerator LoadSceneWithLoadingCoroutine(string sceneName)
	{
		if (loadingHolder != null)
		{
			loadingHolder.GetComponent<CanvasGroup>().blocksRaycasts = true;

			// Play loading scene arrive animation
			loadingHolder.transform.GetChild(0).GetComponent<Animator>().Play("LoadingArriving", 0, 0);

			yield return new WaitForSeconds (2f);

			Application.LoadLevel(sceneName);
		}
	}

	IEnumerator StartSceneWithLoadingHolderCoroutine()
	{
		yield return null;
	}

	public void HomeButtonPressed()
	{
//		GlobalVariables.playLoadingDepart = true;

		StartCoroutine(LoadSceneWithLoadingCoroutine("MainScene"));
	}

	public void NextPatientPressed()
	{

		StartCoroutine(LoadSceneWithLoadingCoroutine("CharacterSelect"));
	}

	public void PlayButtonPressed()
	{
		GlobalVariables.startInterstitialShown = true;

		if (loadingHolder != null)
			loadingHolder.GetComponent<CanvasGroup>().blocksRaycasts = true;

		Application.LoadLevel("Dashboard");
	}

	void ApplyTinyPoppersTextReplacements()
	{
		Text[] labels = Resources.FindObjectsOfTypeAll<Text>();
		foreach (Text label in labels)
		{
			if (label == null || string.IsNullOrEmpty(label.text))
				continue;

			if (label.text == "Balloon Popping" || label.text == "Balloon Pop" || label.text == "BaloonPoper")
				label.text = "Tiny Poppers";
		}
	}

	void RemoveTopTitleAndLyrics()
	{
		Text[] labels = Resources.FindObjectsOfTypeAll<Text>();
		foreach (Text label in labels)
		{
			if (label == null || string.IsNullOrEmpty(label.text))
				continue;

			string lower = label.text.ToLower();
			bool isGeneratedTitle = label.gameObject.name == "TinyPoppersTitle";
			bool isRhyme = label.gameObject.name == "RainbowBalloonsRhyme"
				|| lower.Contains("rainbow balloons")
				|| lower.Contains("red and yellow")
				|| lower.Contains("purple, orange")
				|| lower.Contains("happy smiles")
				|| lower.Contains("pop the balloons")
				|| lower.Contains("hear them cheer")
				|| lower.Contains("green and blue")
				|| lower.Contains("pink ones too");

			if (isGeneratedTitle || isRhyme)
				Destroy(label.gameObject);
		}
	}

	void EnlargeTinyPoppersLogo()
	{
		Image[] images = GetComponentsInChildren<Image>(true);
		foreach (Image image in images)
		{
			if (image == null || image.sprite == null || image.sprite.texture == null)
				continue;

			if (image.sprite.texture.name != "Interface")
				continue;

			Rect spriteRect = image.sprite.rect;
			bool looksLikeLogoSprite = spriteRect.width > 400f && spriteRect.height > 200f;
			if (!looksLikeLogoSprite)
				continue;

			RectTransform rect = image.GetComponent<RectTransform>();
			if (rect == null)
				continue;

			rect.localScale = new Vector3(1.18f, 1.18f, 1f);
			rect.SetAsLastSibling();
		}
	}

	void EnsureCreditsButton()
	{
		if (creditsButton == null)
			creditsButton = GameObject.Find("CreditsButton");

		Button privacyButton = FindButtonByText("privacy");
		if (privacyButton != null && creditsButton == null)
		{
			creditsButton = Instantiate(privacyButton.gameObject) as GameObject;
			creditsButton.name = "CreditsButton";
			creditsButton.transform.SetParent(privacyButton.transform.parent, false);
		}
		else if (creditsButton == null)
		{
			creditsButton = CreateBasicMenuButton("CreditsButton", "Credits", new Vector2(0f, 134f));
		}

		if (privacyButton != null)
		{
			RectTransform creditsRect = creditsButton.GetComponent<RectTransform>();
			RectTransform privacyRect = privacyButton.GetComponent<RectTransform>();
			if (creditsRect != null && privacyRect != null)
			{
				float verticalSpacing = Mathf.Max(privacyRect.rect.height, privacyRect.sizeDelta.y, 44f) + 14f;
				creditsRect.anchorMin = privacyRect.anchorMin;
				creditsRect.anchorMax = privacyRect.anchorMax;
				creditsRect.pivot = privacyRect.pivot;
				creditsRect.sizeDelta = privacyRect.sizeDelta;
				creditsRect.anchoredPosition = privacyRect.anchoredPosition + new Vector2(0f, verticalSpacing);
			}
		}

		Text buttonText = creditsButton.GetComponentInChildren<Text>(true);
		if (buttonText != null)
			buttonText.text = "Credits";

		Button button = creditsButton.GetComponent<Button>();
		if (button != null)
		{
			button.onClick = new Button.ButtonClickedEvent();
			button.onClick.AddListener(new UnityAction(ShowCredits));
		}
	}

	Button FindButtonByText(string text)
	{
		Text[] labels = GetComponentsInChildren<Text>(true);
		foreach (Text label in labels)
		{
			if (label != null && label.text != null && label.text.ToLower().Contains(text))
			{
				Button button = label.GetComponentInParent<Button>();
				if (button != null)
					return button;
			}
		}

		return null;
	}

	GameObject CreateBasicMenuButton(string objectName, string label, Vector2 anchoredPosition)
	{
		GameObject buttonObject = new GameObject(objectName);
		buttonObject.transform.SetParent(transform, false);

		Image image = buttonObject.AddComponent<Image>();
		image.color = new Color(1f, 0.85f, 0.25f, 0.95f);
		Button button = buttonObject.AddComponent<Button>();
		button.targetGraphic = image;

		RectTransform rect = buttonObject.GetComponent<RectTransform>();
		rect.anchorMin = new Vector2(0.5f, 0f);
		rect.anchorMax = new Vector2(0.5f, 0f);
		rect.pivot = new Vector2(0.5f, 0f);
		rect.anchoredPosition = anchoredPosition;
		rect.sizeDelta = new Vector2(220f, 44f);

		GameObject textObject = new GameObject("Text");
		textObject.transform.SetParent(buttonObject.transform, false);
		Text textComponent = textObject.AddComponent<Text>();
		textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
		if (textComponent.font == null)
			textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
		textComponent.text = label;
		textComponent.fontSize = 22;
		textComponent.fontStyle = FontStyle.Bold;
		textComponent.alignment = TextAnchor.MiddleCenter;
		textComponent.color = Color.white;
		RectTransform textRect = textObject.GetComponent<RectTransform>();
		textRect.anchorMin = Vector2.zero;
		textRect.anchorMax = Vector2.one;
		textRect.offsetMin = Vector2.zero;
		textRect.offsetMax = Vector2.zero;

		return buttonObject;
	}

	public void ShowCredits()
	{
		SoundManager.PlaySound("ButtonClick");
		ShowPopUpMessage(
			"Credits",
			"Original Base by: Krish Gupta and Umesh Rajput\n\nModifications & Features by: Pranjali Srivastava"
		);
	}

	public void AtemptToBuyInapp(string sku)
	{
	}

	public void ShowLeaderboard()
	{
		GlobalVariables.globalVariables.ShowLeaderboard();
	}

	public void BuyInappTestis()
	{
	}

    public void OpenPrivacyPolicyLink()
    {
        if (AdsManager.Instance != null && !string.IsNullOrEmpty(AdsManager.Instance.privacyPolicyLink))
        {
            try
            {
                Application.OpenURL(AdsManager.Instance.privacyPolicyLink);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Failed to open privacy policy URL: " + ex.Message);
            }
        }
        else
        {
            Debug.LogWarning("Privacy policy URL is not configured.");
            ShowPopUpMessage("Privacy Policy", "Privacy Policy is not available right now.");
        }
    }

}
