using UnityEngine;
using UnityEngine.UI;

public class BossBaloon : MonoBehaviour
{
	public int hitPoints = 5;
	public int rewardPoints = 10;
	public Text hitPointsText;

	void Awake()
	{
		UpdateHitPointsText();
	}

	public bool Hit()
	{
		hitPoints--;
		UpdateHitPointsText();

		if (hitPoints <= 0)
		{
			LevelManager.levelManager.ScorePoint(rewardPoints);
			MissionManager.AddProgress(MissionType.PopBosses, 1);
			LevelManager.levelManager.PlayParticleOnPosition(transform.localPosition);
			Destroy(gameObject);
			return true;
		}

		return false;
	}

	void UpdateHitPointsText()
	{
		if (hitPointsText != null)
			hitPointsText.text = hitPoints.ToString();
	}
}
