using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class GameTheme
{
	public string themeName;
	public Sprite backgroundSprite;
	public Sprite[] balloonSprites;
	public int requiredStars;
}

public class ThemeProgressionManager : MonoBehaviour
{
	const string SelectedThemeKey = "SelectedTheme";
	const string StarsKey = "ProgressStars";

	public static ThemeProgressionManager Instance;

	public GameTheme[] themes;
	public Image backgroundImage;
	public Text progressionText;

	void Awake()
	{
		Instance = this;
		ApplySelectedTheme();
		UpdateProgressionText();
	}

	public static int Stars
	{
		get { return PlayerPrefs.GetInt(StarsKey, 0); }
	}

	public static void AddStars(int amount)
	{
		PlayerPrefs.SetInt(StarsKey, Stars + amount);
		PlayerPrefs.Save();

		if (Instance != null)
			Instance.UpdateProgressionText();
	}

	public void SelectTheme(int themeIndex)
	{
		if (themes == null || themeIndex < 0 || themeIndex >= themes.Length)
			return;

		if (Stars < themes[themeIndex].requiredStars)
			return;

		PlayerPrefs.SetInt(SelectedThemeKey, themeIndex);
		PlayerPrefs.Save();
		ApplySelectedTheme();
	}

	public void ApplySelectedTheme()
	{
		if (themes == null || themes.Length == 0)
			return;

		int index = Mathf.Clamp(PlayerPrefs.GetInt(SelectedThemeKey, 0), 0, themes.Length - 1);

		if (backgroundImage != null && themes[index].backgroundSprite != null)
			backgroundImage.sprite = themes[index].backgroundSprite;

		if (LevelManager.levelManager != null && themes[index].balloonSprites != null && themes[index].balloonSprites.Length > 0)
			LevelManager.levelManager.baloonImages = themes[index].balloonSprites;
	}

	void UpdateProgressionText()
	{
		if (progressionText != null)
			progressionText.text = Stars.ToString();
	}
}
