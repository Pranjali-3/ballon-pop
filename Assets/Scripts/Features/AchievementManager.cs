using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Achievement
{
	public string id;
	public string title;
	public string description;
	public int targetAmount;
	public bool isHidden;

	[System.NonSerialized]
	public int currentAmount;
	[System.NonSerialized]
	public bool unlocked;
}

public class AchievementManager : MonoBehaviour
{
	const string AchievementPrefix = "Ach_";
	const string AchievementCountKey = "Ach_Count";

	public static AchievementManager Instance;

	public Achievement[] achievements;
	public Text achievementNotificationText;
	public GameObject achievementPopupPrefab;
	public float notificationDuration = 3f;

	int lastUnlockedCount;

	void Awake()
	{
		Instance = this;
		EnsureDefaultAchievements();
		LoadProgress();
		lastUnlockedCount = GetUnlockedCount();
	}

	void EnsureDefaultAchievements()
	{
		if (achievements != null && achievements.Length > 0)
			return;

		achievements = new Achievement[12];
		achievements[0] = new Achievement { id = "first_pop", title = "First Pop!", description = "Pop your first balloon", targetAmount = 1 };
		achievements[1] = new Achievement { id = "pop_50", title = "Balloon Novice", description = "Pop 50 balloons", targetAmount = 50 };
		achievements[2] = new Achievement { id = "pop_500", title = "Balloon Master", description = "Pop 500 balloons", targetAmount = 500 };
		achievements[3] = new Achievement { id = "score_100", title = "Century", description = "Score 100 points in one game", targetAmount = 1, isHidden = true };
		achievements[4] = new Achievement { id = "score_500", title = "High Roller", description = "Score 500 points in one game", targetAmount = 1, isHidden = true };
		achievements[5] = new Achievement { id = "collect_10", title = "Toy Collector", description = "Collect 10 toys", targetAmount = 10 };
		achievements[6] = new Achievement { id = "collect_all", title = "Completionist", description = "Collect all 20 toys", targetAmount = 20 };
		achievements[7] = new Achievement { id = "combo_10", title = "Combo King", description = "Reach a 10x combo", targetAmount = 10 };
		achievements[8] = new Achievement { id = "boss_5", title = "Boss Slayer", description = "Defeat 5 boss balloons", targetAmount = 5 };
		achievements[9] = new Achievement { id = "coins_1000", title = "Saver", description = "Earn 1000 coins total", targetAmount = 1000 };
		achievements[10] = new Achievement { id = "daily_streak_7", title = "Dedicated", description = "7-day daily streak", targetAmount = 7 };
		achievements[11] = new Achievement { id = "educ_10", title = "Smart Kid", description = "Complete 10 educational rounds", targetAmount = 10 };
	}

	void LoadProgress()
	{
		if (achievements == null) return;
		for (int i = 0; i < achievements.Length; i++)
		{
			achievements[i].currentAmount = PlayerPrefs.GetInt(AchievementPrefix + achievements[i].id + "_progress", 0);
			achievements[i].unlocked = PlayerPrefs.GetInt(AchievementPrefix + achievements[i].id + "_unlocked", 0) == 1;
		}
	}

	void SaveProgress()
	{
		if (achievements == null) return;
		for (int i = 0; i < achievements.Length; i++)
		{
			PlayerPrefs.SetInt(AchievementPrefix + achievements[i].id + "_progress", achievements[i].currentAmount);
			PlayerPrefs.SetInt(AchievementPrefix + achievements[i].id + "_unlocked", achievements[i].unlocked ? 1 : 0);
		}
		PlayerPrefs.Save();
	}

	public static void AddProgress(string achievementId, int amount = 1)
	{
		if (Instance == null) return;
		Instance.AddProgressInternal(achievementId, amount);
	}

	void AddProgressInternal(string achievementId, int amount)
	{
		Achievement ach = FindAchievement(achievementId);
		if (ach == null || ach.unlocked) return;

		ach.currentAmount += amount;
		if (ach.currentAmount >= ach.targetAmount)
		{
			ach.currentAmount = ach.targetAmount;
			ach.unlocked = true;
			OnAchievementUnlocked(ach);
		}
		SaveProgress();
	}

	void OnAchievementUnlocked(Achievement ach)
	{
		if (SoundManager.Instance != null)
		{
			if (SoundManager.Instance.achievementUnlock != null)
				SoundManager.PlaySound("AchievementUnlock");
			else
				SoundManager.PlaySound("InappBought");
		}

		int count = GetUnlockedCount();
		PlayerPrefs.SetInt(AchievementCountKey, count);
		PlayerPrefs.Save();

		if (achievementNotificationText != null)
		{
			achievementNotificationText.text = "Achievement: " + ach.title;
			CancelInvoke("HideAchievementNotification");
			Invoke("HideAchievementNotification", notificationDuration);
		}
	}

	void HideAchievementNotification()
	{
		if (achievementNotificationText != null)
			achievementNotificationText.text = "";
	}

	public static void CheckCoinAchievements(int coins)
	{
		if (coins >= 1000)
			AddProgress("coins_1000");
	}

	public static void CheckComboAchievement(int comboCount)
	{
		if (comboCount >= 10)
			AddProgress("combo_10");
	}

	public static void CheckScoreAchievement(int score)
	{
		if (score >= 100)
			AddProgress("score_100");
		if (score >= 500)
			AddProgress("score_500");
	}

	Achievement FindAchievement(string id)
	{
		if (achievements == null) return null;
		for (int i = 0; i < achievements.Length; i++)
		{
			if (achievements[i] != null && achievements[i].id == id)
				return achievements[i];
		}
		return null;
	}

	public static int GetUnlockedCount()
	{
		if (Instance == null || Instance.achievements == null) return 0;
		int count = 0;
		for (int i = 0; i < Instance.achievements.Length; i++)
		{
			if (Instance.achievements[i] != null && Instance.achievements[i].unlocked)
				count++;
		}
		return count;
	}

	public static int GetTotalAchievements()
	{
		if (Instance == null || Instance.achievements == null) return 0;
		return Instance.achievements.Length;
	}
}
