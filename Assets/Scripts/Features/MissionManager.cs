using UnityEngine;
using UnityEngine.UI;

public enum MissionType
{
	PopBalloons,
	CollectToys,
	ScorePoints,
	PopBosses,
	AvoidBadBalloons
}

[System.Serializable]
public class Mission
{
	public MissionType type;
	public string description;
	public int targetAmount = 10;
	public int currentAmount;
	public bool completed;
}

public class MissionManager : MonoBehaviour
{
	public static MissionManager Instance;

	public Mission[] activeMissions;
	public Text missionText;

	void Awake()
	{
		Instance = this;
		EnsureDefaultMissions();
		RefreshMissionText();
	}

	public static void AddProgress(MissionType type, int amount)
	{
		if (Instance != null)
			Instance.AddProgressInternal(type, amount);
	}

	void AddProgressInternal(MissionType type, int amount)
	{
		if (activeMissions == null)
			return;

		for (int i = 0; i < activeMissions.Length; i++)
		{
			if (activeMissions[i] == null || activeMissions[i].completed || activeMissions[i].type != type)
				continue;

			activeMissions[i].currentAmount += amount;
			if (activeMissions[i].currentAmount >= activeMissions[i].targetAmount)
			{
				activeMissions[i].currentAmount = activeMissions[i].targetAmount;
				if (!activeMissions[i].completed)
				{
					activeMissions[i].completed = true;
					if (LevelManager.levelManager != null)
					{
						LevelManager.levelManager.TriggerMilestoneBanner("Daily Target Achieved!");
					}
				}
			}
		}

		RefreshMissionText();
	}

	public void RefreshMissionText()
	{
		if (missionText == null || activeMissions == null || activeMissions.Length == 0)
			return;

		string text = "";
		for (int i = 0; i < activeMissions.Length; i++)
		{
			if (activeMissions[i] == null)
				continue;

			if (text != "")
				text += "\n";

			string label = activeMissions[i].description;
			if (label == null || label == "")
				label = activeMissions[i].type.ToString();

			text += label + " " + activeMissions[i].currentAmount.ToString() + "/" + activeMissions[i].targetAmount.ToString();
		}

		missionText.text = text;
	}

	public void EnsureDefaultMissions(bool forceReset = false)
	{
		if (!forceReset && activeMissions != null && activeMissions.Length > 0)
			return;

		int selectedStage = PlayerPrefs.GetInt("SelectedProgressionLevel", 0);

		activeMissions = new Mission[1];
		activeMissions[0] = new Mission();

		switch (selectedStage)
		{
			case 0:
				activeMissions[0].type = MissionType.PopBalloons;
				activeMissions[0].description = "Pop Balloons:";
				activeMissions[0].targetAmount = 20;
				break;
			case 1:
				activeMissions[0].type = MissionType.ScorePoints;
				activeMissions[0].description = "Score Points:";
				activeMissions[0].targetAmount = 40;
				break;
			case 2:
				activeMissions[0].type = MissionType.CollectToys;
				activeMissions[0].description = "Collect Toys:";
				activeMissions[0].targetAmount = 2;
				break;
			case 3:
				activeMissions[0].type = MissionType.ScorePoints;
				activeMissions[0].description = "Score Points:";
				activeMissions[0].targetAmount = 60;
				break;
			case 4:
				activeMissions[0].type = MissionType.CollectToys;
				activeMissions[0].description = "Collect Toys:";
				activeMissions[0].targetAmount = 3;
				break;
			default:
				activeMissions[0].type = MissionType.PopBalloons;
				activeMissions[0].description = "Pop Balloons:";
				activeMissions[0].targetAmount = 20;
				break;
		}

		activeMissions[0].currentAmount = 0;
		activeMissions[0].completed = false;
	}

	public void ResetMissionProgress()
	{
		EnsureDefaultMissions(true);
		RefreshMissionText();
	}

	public void ResetMissionForSelectedStage()
	{
		EnsureDefaultMissions(true);
		RefreshMissionText();
	}
}
