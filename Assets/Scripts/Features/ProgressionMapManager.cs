using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ProgressionLevel
{
	public string levelName = "Level";
	public int requiredStars;
	public float minSpawnTime = 0.45f;
	public float maxSpawnTime = 0.9f;
	public float minBalloonSpeed = 180f;
	public float maxBalloonSpeed = 350f;
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
		UpdateSelectedLevelText();
	}

	public void SelectLevel(int levelIndex)
	{
		if (levels == null || levelIndex < 0 || levelIndex >= levels.Length)
			return;

		// Bypass star requirements so the player can select any stage freely
		// if (ThemeProgressionManager.Stars < levels[levelIndex].requiredStars)
		// 	return;

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

		float minTime = level.minSpawnTime > 0f ? level.minSpawnTime : 0.2f;
		float maxTime = level.maxSpawnTime > 0f ? level.maxSpawnTime : 0.4f;

		LevelManager.levelManager.minBaloonSpawnTime = minTime;
		LevelManager.levelManager.maxBaloonSpewnTime = maxTime;
		LevelManager.levelManager.minBaloonSpeed = level.minBalloonSpeed > 0f ? level.minBalloonSpeed : 180f;
		LevelManager.levelManager.maxBaloonSpeed = level.maxBalloonSpeed > 0f ? level.maxBalloonSpeed : 350f;
		LevelManager.levelManager.gameTime = 60f;
		LevelManager.levelManager.gameTimeOrig = 60f;
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
		{
			for (int i = 0; i < levels.Length; i++)
			{
				if (levels[i].maxBalloonSpeed > 0f && levels[i].maxBalloonSpeed < 50f)
				{
					levels[i].minBalloonSpeed *= 120f;
					levels[i].maxBalloonSpeed *= 120f;
				}

				levels[i].minBalloonSpeed = Mathf.Max(180f + i * 35f, levels[i].minBalloonSpeed);
				levels[i].maxBalloonSpeed = Mathf.Max(350f + i * 45f, levels[i].maxBalloonSpeed);
				levels[i].minSpawnTime = Mathf.Clamp(levels[i].minSpawnTime, 0.2f, 0.65f);
				levels[i].maxSpawnTime = Mathf.Clamp(levels[i].maxSpawnTime, 0.4f, 0.95f);

				if (levels[i].maxSpawnTime <= levels[i].minSpawnTime)
					levels[i].maxSpawnTime = levels[i].minSpawnTime + 0.35f;

				levels[i].gameTime = 60f;
			}
			return;
		}

		levels = new ProgressionLevel[5];

		for (int i = 0; i < levels.Length; i++)
		{
			levels[i] = new ProgressionLevel();
			levels[i].levelName = "Stage " + (i + 1).ToString();
			levels[i].requiredStars = i * 3;
			levels[i].minSpawnTime = Mathf.Max(0.2f, 0.45f - i * 0.04f);
			levels[i].maxSpawnTime = Mathf.Max(0.4f, 0.9f - i * 0.08f);
			levels[i].minBalloonSpeed = 180f + i * 35f;
			levels[i].maxBalloonSpeed = 350f + i * 45f;
			levels[i].gameTime = 60f;
		}
	}
}
