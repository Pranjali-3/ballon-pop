using UnityEngine;
using UnityEngine.UI;

public class ComboManager : MonoBehaviour
{
	public static ComboManager Instance;

	public float comboWindow = 1.4f;
	public int comboBonusStep = 5;
	public Text comboText;
	public Animator comboAnimator;

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
		UpdateComboText();

		int bonusMultiplier = 1 + (comboCount / comboBonusStep);
		return basePoints * bonusMultiplier;
	}

	void Update()
	{
		if (comboCount > 0 && Time.time - lastPopTime > comboWindow)
			ResetCombo();
	}

	void UpdateComboText()
	{
		if (comboText != null)
			comboText.text = comboCount > 1 ? "x" + comboCount.ToString() : "";

		if (comboAnimator != null && comboCount > 1)
			comboAnimator.Play("TextAnimationActive", 0, 0);
	}

	public void ResetCombo()
	{
		comboCount = 0;
		lastPopTime = -comboWindow;

		if (comboText != null)
			comboText.text = "";
	}
}
