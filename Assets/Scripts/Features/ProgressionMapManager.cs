using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ProgressionLevel
{
	public string levelName = "Level";
	public int requiredStars;
	public float minSpawnTime = 0.6f;
	public float maxSpawnTime = 1.4f;
	public float minBalloonSpeed = 1.5f;
	public float maxBalloonSpeed = 3.2f;
	public float gameTime = 60f;
}

public class ProgressionMapManager : MonoBehaviour
{
	const string SelectedLevelKey = "SelectedProgressionLevel";

	public static ProgressionMapManager Instance;

	public ProgressionLevel[] levels;
	public Text selectedLevelText;

	void Awake()
	{
		Instance = this;
		EnsureDefaultLevels();
		ApplySelectedLevel();
		UpdateSelectedLevelText();
	}

	public void SelectLevel(int levelIndex)
	{
		if (levels == null || levelIndex < 0 || levelIndex >= levels.Length)
			return;

		if (ThemeProgressionManager.Stars < levels[levelIndex].requiredStars)
			return;

		PlayerPrefs.SetInt(SelectedLevelKey, levelIndex);
		PlayerPrefs.Save();
		ApplySelectedLevel();
		UpdateSelectedLevelText();
	}

	public void ApplySelectedLevel()
	{
		if (LevelManager.levelManager == null || levels == null || levels.Length == 0)
			return;

		int index = Mathf.Clamp(PlayerPrefs.GetInt(SelectedLevelKey, 0), 0, levels.Length - 1);
		ProgressionLevel level = levels[index];

		LevelManager.levelManager.minBaloonSpawnTime = level.minSpawnTime;
		LevelManager.levelManager.maxBaloonSpewnTime = level.maxSpawnTime;
		LevelManager.levelManager.minBaloonSpeed = level.minBalloonSpeed;
		LevelManager.levelManager.maxBaloonSpeed = level.maxBalloonSpeed;
		LevelManager.levelManager.gameTime = level.gameTime;
		LevelManager.levelManager.gameTimeOrig = level.gameTime;
	}

	void UpdateSelectedLevelText()
	{
		if (selectedLevelText == null || levels == null || levels.Length == 0)
			return;

		int index = Mathf.Clamp(PlayerPrefs.GetInt(SelectedLevelKey, 0), 0, levels.Length - 1);
		selectedLevelText.text = levels[index].levelName;
	}

	void EnsureDefaultLevels()
	{
		if (levels != null && levels.Length > 0)
			return;

		levels = new ProgressionLevel[5];

		for (int i = 0; i < levels.Length; i++)
		{
			levels[i] = new ProgressionLevel();
			levels[i].levelName = "Stage " + (i + 1).ToString();
			levels[i].requiredStars = i * 3;
			levels[i].minSpawnTime = Mathf.Max(0.25f, 0.8f - i * 0.08f);
			levels[i].maxSpawnTime = Mathf.Max(0.55f, 1.6f - i * 0.12f);
			levels[i].minBalloonSpeed = 1.3f + i * 0.25f;
			levels[i].maxBalloonSpeed = 2.8f + i * 0.35f;
			levels[i].gameTime = Mathf.Max(35f, 60f - i * 4f);
		}
	}
}
