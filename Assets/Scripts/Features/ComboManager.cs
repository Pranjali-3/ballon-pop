using UnityEngine;
using UnityEngine.UI;

public class ComboManager : MonoBehaviour
{
	public static ComboManager Instance;

	public float comboWindow = 1.4f;
	public int comboBonusStep = 5;
	public Text comboText;
	public Animator comboAnimator;
	public Image comboFillImage;
	public GameObject comboEffectPrefab;

	int comboCount;
	float lastPopTime;

	void Awake()
	{
		Instance = this;
		ResetCombo();
	}

	public static int RegisterPop(int basePoints)
	{
		if (Instance == null)
			return basePoints;

		return Instance.RegisterPopInternal(basePoints);
	}

	int RegisterPopInternal(int basePoints)
	{
		if (Time.time - lastPopTime <= comboWindow)
			comboCount++;
		else
			comboCount = 1;

		lastPopTime = Time.time;
		UpdateComboUI();

		int bonusMultiplier = 1 + (comboCount / comboBonusStep);
		return basePoints * bonusMultiplier;
	}

	void Update()
	{
		if (comboCount > 0 && Time.time - lastPopTime > comboWindow)
			ResetCombo();

		if (comboCount > 0 && comboFillImage != null)
		{
			float elapsed = Time.time - lastPopTime;
			comboFillImage.fillAmount = 1f - (elapsed / comboWindow);
		}
	}

	void UpdateComboUI()
	{
		if (comboText != null)
		{
			if (comboCount > 1)
			{
				comboText.text = comboCount + "x COMBO!";
				comboText.fontSize = Mathf.Min(36 + (comboCount / comboBonusStep) * 4, 72);
			}
			else
			{
				comboText.text = "";
			}
		}

		if (comboAnimator != null && comboCount > 1)
			comboAnimator.Play("ComboPulse", 0, 0);

		if (comboCount == comboBonusStep || comboCount == comboBonusStep * 2 || comboCount == comboBonusStep * 3)
		{
			SoundManager.PlaySound("ComboBreak");
			SoundManager.PlayMusic("Combo");

			if (comboCount >= 10)
				AchievementManager.AddProgress("combo_10");
		}
	}

	public void ResetCombo()
	{
		comboCount = 0;
		lastPopTime = -comboWindow;

		if (comboText != null)
			comboText.text = "";

		if (comboFillImage != null)
			comboFillImage.fillAmount = 0f;
	}
}
