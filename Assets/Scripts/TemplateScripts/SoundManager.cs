using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/**
  * Scene:All
  * Object:SoundManager
  * Description: Skripta zaduzena za zvuke u apliakciji, njihovo pustanje, gasenje itd...
  **/
public class SoundManager : MonoBehaviour {

	public static int musicOn = 1;
	public static int soundOn = 1;
	public static bool forceTurnOff = false;

	// FX
	public AudioSource buttonClick;
	public AudioSource popupArrive;
	public AudioSource bombSound;
	public AudioSource inappBought;
	public AudioSource minusSec;
	public AudioSource plusSec;
	public AudioSource baloonPop;
	public AudioSource toyPop;
	public AudioSource comboBreak;
	public AudioSource achievementUnlock;
	public AudioSource educCorrect;
	public AudioSource educWrong;
	public AudioSource coinCollect;

	// Music
	public AudioSource menuMusic;
	public AudioSource gameplayMusic;
	public AudioSource bossMusic;
	public AudioSource comboMusic;
	public AudioSource victoryMusic;
	public AudioSource gameOverMusic;

	public GameObject musicObjectsHolder;
	public GameObject fxObjectsHolder;

	// Music on off sprites
	public Sprite musicOffImageHolder;
	public Sprite musicOffSprite;
	public Sprite musicOnImageHolder;
	public Sprite musicOnSprite;

	static SoundManager instance;
	static string currentMusicName = "";

	public static SoundManager Instance
	{
		get
		{
			if(instance == null)
			{
				instance = GameObject.FindObjectOfType(typeof(SoundManager)) as SoundManager;
			}

			return instance;
		}
	}

	public void OnLevelWasLoaded(int level)
	{
		if (Application.loadedLevelName == "MainScene")
		{
			PlayMusic("Menu");
		}
		else if (Application.loadedLevelName == "Level")
		{
			PlayMusic("Gameplay");
		}
		else if (Application.loadedLevelName == "Dashboard")
		{
			PlayMusic("Menu");
		}
	}

	void Awake()
	{
		instance = this;
		AutoAssignAudioSources();
	}

	void AutoAssignAudioSources()
	{
		AudioSource[] allSources = GetComponentsInChildren<AudioSource>();
		foreach (AudioSource src in allSources)
		{
			switch (src.gameObject.name.ToLower())
			{
			case "buttonclick": if (buttonClick == null) buttonClick = src; break;
			case "popuparrive": if (popupArrive == null) popupArrive = src; break;
			case "bombsound": if (bombSound == null) bombSound = src; break;
			case "inappbought": if (inappBought == null) inappBought = src; break;
			case "minussec": if (minusSec == null) minusSec = src; break;
			case "plussec": if (plusSec == null) plusSec = src; break;
			case "baloonpop": if (baloonPop == null) baloonPop = src; break;
			case "toypop": if (toyPop == null) toyPop = src; break;
			case "combobreak": if (comboBreak == null) comboBreak = src; break;
			case "achievementunlock": if (achievementUnlock == null) achievementUnlock = src; break;
			case "educorrect": if (educCorrect == null) educCorrect = src; break;
			case "educwrong": if (educWrong == null) educWrong = src; break;
			case "coincollect": if (coinCollect == null) coinCollect = src; break;
			case "music": if (menuMusic == null) menuMusic = src; break;
			case "menumusic": if (menuMusic == null) menuMusic = src; break;
			case "gameplaymusic": if (gameplayMusic == null) gameplayMusic = src; break;
			case "bossmusic": if (bossMusic == null) bossMusic = src; break;
			case "combobells": if (comboMusic == null) comboMusic = src; break;
			case "victorymusic": if (victoryMusic == null) victoryMusic = src; break;
			case "gameovermusic": if (gameOverMusic == null) gameOverMusic = src; break;
			}
		}
		TryLoadMissingAudioClips();
	}

	void TryLoadMissingAudioClips()
	{
		TryLoadAudioSource(ref buttonClick, "ButtonClick");
		TryLoadAudioSource(ref popupArrive, "PopupShow");
		TryLoadAudioSource(ref bombSound, "BombSound");
		TryLoadAudioSource(ref inappBought, "InappBought");
		TryLoadAudioSource(ref minusSec, "MinusSec1");
		TryLoadAudioSource(ref plusSec, "PlusSec1");
		TryLoadAudioSource(ref baloonPop, "BaloonPop");
		TryLoadAudioSource(ref toyPop, "ToyPop");
		TryLoadAudioSource(ref menuMusic, "TheCircusBee", true);
		TryLoadAudioSource(ref gameplayMusic, "TheCircusBee", true);
		TryLoadAudioSource(ref bossMusic, "BossMusic", true);
		TryLoadAudioSource(ref comboMusic, "ComboBells", true);
		TryLoadAudioSource(ref victoryMusic, "VictoryMusic", true);
		TryLoadAudioSource(ref gameOverMusic, "GameOverMusic", true);
		if (menuMusic != null) menuMusic.loop = true;
		if (gameplayMusic != null) gameplayMusic.loop = true;
		if (bossMusic != null) bossMusic.loop = true;
		if (comboMusic != null) comboMusic.loop = false;
		if (victoryMusic != null) victoryMusic.loop = false;
		if (gameOverMusic != null) gameOverMusic.loop = false;
		TryLoadAudioSource(ref comboBreak, "InappBought");
		TryLoadAudioSource(ref achievementUnlock, "InappBought");
		TryLoadAudioSource(ref educCorrect, "PlusSec1");
		TryLoadAudioSource(ref educWrong, "MinusSec1");
		TryLoadAudioSource(ref coinCollect, "ButtonClick");
	}

	void TryLoadAudioSource(ref AudioSource audioSource, string clipName, bool replaceExistingClip = false)
	{
		if (!replaceExistingClip && audioSource != null && audioSource.clip != null)
			return;

		if (audioSource == null)
		{
			GameObject child = new GameObject(clipName);
			child.transform.SetParent(transform, false);
			audioSource = child.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
		}

		AudioClip clip = Resources.Load<AudioClip>("BaloonPoopSounds/" + clipName);
		if (clip != null)
			audioSource.clip = clip;
	}

	void Start () 
	{
		if (instance == null) instance = this;
		if (transform.parent == null)
			DontDestroyOnLoad(this.gameObject);

		if(PlayerPrefs.HasKey("SoundOn"))
		{
			musicOn = PlayerPrefs.GetInt("MusicOn");
			soundOn = PlayerPrefs.GetInt("SoundOn");
		}
		else
		{
			PlayerPrefs.SetInt("SoundOn", 1);
			PlayerPrefs.SetInt("MusicOn", 1);
			musicOn = 1;
			soundOn = 1;
		}

		if (Application.loadedLevelName == "MainScene")
		{
			PlayMusic("Menu");
		}
		else if (Application.loadedLevelName == "Level")
		{
			PlayMusic("Gameplay");
		}
		else if (Application.loadedLevelName == "Dashboard")
		{
			PlayMusic("Menu");
		}

		Screen.sleepTimeout = SleepTimeout.NeverSleep; 
	}

	public static void ToggleSound()
	{
		if (Instance == null) return;

		if (soundOn == 0)
		{
			soundOn = 1;
			PlayerPrefs.SetInt("SoundOn", 1);

			foreach (Transform t in Instance.transform)
			{
				AudioSource src = t.GetComponent<AudioSource>();
				if (src != null)
					src.mute = false;
			}

			PlayMusic(GetSceneMusicName());
			UpdateSoundButtonSprites();
		}
		else if (soundOn == 1)
		{
			soundOn = 0;
			PlayerPrefs.SetInt("SoundOn", 0);

			foreach (Transform t in Instance.transform)
			{
				AudioSource src = t.GetComponent<AudioSource>();
				if (src != null)
					src.mute = true;
			}

			UpdateSoundButtonSprites();
		}
	}

	static void UpdateSoundButtonSprites()
	{
		GameObject canvas = GameObject.Find("Canvas");
		if (canvas == null) return;
		MenuManager menuMgr = canvas.GetComponent<MenuManager>();
		if (menuMgr == null || menuMgr.soundButton == null) return;

		Image btnImage = menuMgr.soundButton.GetComponent<Image>();
		Image childImage = menuMgr.soundButton.transform.GetChild(0).GetComponent<Image>();
		if (btnImage == null || childImage == null) return;

		if (soundOn == 1)
		{
			btnImage.sprite = Instance.musicOnImageHolder;
			childImage.sprite = Instance.musicOnSprite;
		}
		else
		{
			btnImage.sprite = Instance.musicOffImageHolder;
			childImage.sprite = Instance.musicOffSprite;
		}
	}

	public static void PlaySound(string soundName)
	{
		if (soundOn == 1 && Instance != null)
		{
			switch(soundName)
			{
				case "Music":
					PlayMusic(GetSceneMusicName());
					break;
				case "ButtonClick":
					PlayAudioSource(Instance.buttonClick);
					break;
				case "PopupArrive":
					PlayAudioSource(Instance.popupArrive);
					break;
				case "BombSound":
					PlayAudioSource(Instance.bombSound);
					break;
				case "InappBought":
					PlayAudioSource(Instance.inappBought);
					break;
				case "MinusSec":
					PlayAudioSource(Instance.minusSec);
					break;
				case "PlusSec":
					PlayAudioSource(Instance.plusSec);
					break;
				case "BaloonPop":
					PlayAudioSource(Instance.baloonPop);
					break;
				case "ToyPop":
					PlayAudioSource(Instance.toyPop);
					break;
				case "ComboBreak":
					PlayAudioSource(Instance.comboBreak);
					break;
				case "AchievementUnlock":
					PlayAudioSource(Instance.achievementUnlock);
					break;
				case "EducCorrect":
					PlayAudioSource(Instance.educCorrect);
					break;
				case "EducWrong":
					PlayAudioSource(Instance.educWrong);
					break;
				case "CoinCollect":
					PlayAudioSource(Instance.coinCollect);
					break;
				default:
					break;
			}
		}
	}

	static void PlayAudioSource(AudioSource audioSource)
	{
		if (audioSource != null)
			audioSource.Play();
	}

	public static void PlayMusic(string musicName)
	{
		if (soundOn != 1 || Instance == null)
			return;

		if (musicName == "Combo")
		{
			PlayAudioSource(Instance.comboMusic);
			return;
		}

		AudioSource nextMusic = GetMusicSource(musicName);
		if (nextMusic == null)
			return;

		if (currentMusicName == musicName && nextMusic.isPlaying)
			return;

		StopMusicSource(Instance.menuMusic);
		StopMusicSource(Instance.gameplayMusic);
		StopMusicSource(Instance.bossMusic);
		StopMusicSource(Instance.victoryMusic);
		StopMusicSource(Instance.gameOverMusic);

		nextMusic.volume = 1f;
		nextMusic.Play();
		currentMusicName = musicName;
	}

	static AudioSource GetMusicSource(string musicName)
	{
		switch (musicName)
		{
		case "Menu": return Instance.menuMusic;
		case "Gameplay": return Instance.gameplayMusic;
		case "Boss": return Instance.bossMusic;
		case "Victory": return Instance.victoryMusic;
		case "GameOver": return Instance.gameOverMusic;
		default: return Instance.menuMusic;
		}
	}

	static void StopMusicSource(AudioSource audioSource)
	{
		if (audioSource != null && audioSource.isPlaying)
			audioSource.Stop();
	}

	static string GetSceneMusicName()
	{
		if (Application.loadedLevelName == "Level")
			return "Gameplay";

		return "Menu";
	}

	public static void StopSound(string soundName)
	{
		if (soundOn == 1)
		{
			switch(soundName)
			{
			case "Music":
				Instance.StartCoroutine(Instance.FadeOut(Instance.menuMusic, 0.03f));
				break;
			default:
				break;
			}
		}
	}

	public static bool IsSoundPlaying(string soundName)
	{
		if (soundOn == 1)
		{
			switch(soundName)
			{
			case "Music":
				if (Instance.menuMusic.isPlaying)
					return true;
				break;
			case "ButtonClick":
				if (Instance.buttonClick.isPlaying)
					return true;
				break;
			case "PopupArrive":
				if (Instance.popupArrive.isPlaying)
					return true;
				break;
			case "BombSound":
				if (Instance.bombSound.isPlaying)
					return true;
				break;
			case "InappBought":
				if (Instance.inappBought.isPlaying)
					return true;
				break;
			case "MinusSec":
				if (Instance.minusSec.isPlaying)
					return true;
				break;
			case "PlusSec":
				if (Instance.plusSec.isPlaying)
					return true;
				break;
			case "BaloonPop":
				if (Instance.baloonPop.isPlaying)
					return true;
				break;
			case "ToyPop":
				if (Instance.toyPop.isPlaying)
					return true;
				break;
			case "ComboBreak":
				if (Instance.comboBreak.isPlaying)
					return true;
				break;
			case "AchievementUnlock":
				if (Instance.achievementUnlock.isPlaying)
					return true;
				break;
			case "EducCorrect":
				if (Instance.educCorrect.isPlaying)
					return true;
				break;
			case "EducWrong":
				if (Instance.educWrong.isPlaying)
					return true;
				break;
			case "CoinCollect":
				if (Instance.coinCollect.isPlaying)
					return true;
				break;
			default:
				break;
			}
		}

		return false;
	}

	/// <summary>
	/// Corutine-a koja za odredjeni AudioSource, kroz prosledjeno vreme, utisava AudioSource do 0, gasi taj AudioSource, a zatim vraca pocetni Volume na pocetan kako bi AudioSource mogao opet da se koristi
	/// </summary>
	/// <param name="sound">AudioSource koji treba smanjiti/param>
	/// <param name="time">Vreme za koje treba smanjiti Volume/param>
	IEnumerator FadeOut(AudioSource sound, float time)
	{
		float originalVolume = 1f;

		while(sound.volume != 0)
		{
			sound.volume = Mathf.MoveTowards(sound.volume, 0, time);
			yield return null;
		}

		sound.Stop();
		sound.volume = originalVolume;
	}
	
}
