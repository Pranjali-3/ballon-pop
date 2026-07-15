using UnityEngine;
using UnityEngine.UI;

public class ToyAlbumManager : MonoBehaviour
{
	const string ToyPrefix = "ToyAlbum_";
	const string ToyCountKey = "ToyAlbum_Total";

	public static ToyAlbumManager Instance;

	public Text albumProgressText;
	public GameObject newToyIndicator;
	public int totalToys = 20;

	void Awake()
	{
		Instance = this;
		UpdateAlbumText();
	}

	public static bool IsToyUnlocked(int toyId)
	{
		return PlayerPrefs.GetInt(ToyPrefix + toyId, 0) == 1;
	}

	public static bool CollectToy(int toyId)
	{
		if (toyId < 0)
			return false;

		string key = ToyPrefix + toyId;
		if (PlayerPrefs.GetInt(key, 0) == 1)
			return false;

		PlayerPrefs.SetInt(key, 1);
		PlayerPrefs.SetInt(ToyCountKey, PlayerPrefs.GetInt(ToyCountKey, 0) + 1);
		PlayerPrefs.Save();

		if (Instance != null)
			Instance.OnToyCollected();

		return true;
	}

	public static int GetUnlockedToyCount()
	{
		return PlayerPrefs.GetInt(ToyCountKey, 0);
	}

	void OnToyCollected()
	{
		if (newToyIndicator != null)
			newToyIndicator.SetActive(true);

		SoundManager.PlaySound("ToyPop");
		UpdateAlbumText();

		if (GetUnlockedToyCount() >= totalToys)
			AchievementManager.AddProgress("collect_all");
	}

	public void UpdateAlbumText()
	{
		if (albumProgressText != null)
			albumProgressText.text = GetUnlockedToyCount().ToString() + "/" + totalToys.ToString();
	}
}
