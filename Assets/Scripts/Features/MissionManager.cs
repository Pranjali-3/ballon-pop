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
				activeMissions[i].completed = true;
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

	void EnsureDefaultMissions()
	{
		if (activeMissions != null && activeMissions.Length > 0)
			return;

		activeMissions = new Mission[3];

		activeMissions[0] = new Mission();
		activeMissions[0].type = MissionType.PopBalloons;
		activeMissions[0].description = "Pop";
		activeMissions[0].targetAmount = 30;

		activeMissions[1] = new Mission();
		activeMissions[1].type = MissionType.CollectToys;
		activeMissions[1].description = "Toys";
		activeMissions[1].targetAmount = 3;

		activeMissions[2] = new Mission();
		activeMissions[2].type = MissionType.ScorePoints;
		activeMissions[2].description = "Score";
		activeMissions[2].targetAmount = 100;
	}
}
