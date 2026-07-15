using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum EducationalSubject
{
	Letters,
	Numbers,
	Animals,
	Colors,
	Shapes
}

[System.Serializable]
public class EducationalQuestion
{
	public string displayChar;
	public string correctAnswer;
	public List<string> wrongAnswers;
	public Sprite displaySprite;
}

public class EducationalModeManager : MonoBehaviour
{
	public static EducationalModeManager Instance;

	public bool isEducationalModeActive;
	public EducationalSubject currentSubject;
	public Text questionText;
	public Text scoreText;
	public GameObject[] answerButtons;
	public Text[] answerButtonTexts;
	public GameObject educationalPanel;

	int currentScore;
	int currentRound;

	Dictionary<EducationalSubject, List<EducationalQuestion>> questionBank;

	void Awake()
	{
		Instance = this;
		InitializeQuestionBank();
	}

	void InitializeQuestionBank()
	{
		questionBank = new Dictionary<EducationalSubject, List<EducationalQuestion>>();

		List<EducationalQuestion> letters = new List<EducationalQuestion>();
		for (char c = 'A'; c <= 'Z'; c++)
		{
			List<string> wrongs = new List<string>();
			char wrong1 = (char)('A' + Random.Range(0, 26));
			char wrong2 = (char)('A' + Random.Range(0, 26));
			while (wrong1 == c) wrong1 = (char)('A' + Random.Range(0, 26));
			while (wrong2 == c || wrong2 == wrong1) wrong2 = (char)('A' + Random.Range(0, 26));
			wrongs.Add(wrong1.ToString());
			wrongs.Add(wrong2.ToString());
			letters.Add(new EducationalQuestion { displayChar = c.ToString(), correctAnswer = c.ToString(), wrongAnswers = wrongs });
		}
		questionBank[EducationalSubject.Letters] = letters;

		List<EducationalQuestion> numbers = new List<EducationalQuestion>();
		for (int i = 1; i <= 20; i++)
		{
			List<string> wrongs = new List<string>();
			int w1 = Random.Range(1, 21);
			int w2 = Random.Range(1, 21);
			while (w1 == i) w1 = Random.Range(1, 21);
			while (w2 == i || w2 == w1) w2 = Random.Range(1, 21);
			wrongs.Add(w1.ToString());
			wrongs.Add(w2.ToString());
			numbers.Add(new EducationalQuestion { displayChar = i.ToString(), correctAnswer = i.ToString(), wrongAnswers = wrongs });
		}
		questionBank[EducationalSubject.Numbers] = numbers;

		List<EducationalQuestion> animals = new List<EducationalQuestion>();
		string[] animalNames = new string[] { "CAT", "DOG", "BIRD", "FISH", "LION", "BEAR", "DUCK", "FROG", "OWL", "COW" };
		for (int i = 0; i < animalNames.Length; i++)
		{
			List<string> wrongs = new List<string>();
			for (int j = 0; j < 2; j++)
			{
				int r = Random.Range(0, animalNames.Length);
				while (r == i || wrongs.Contains(animalNames[r])) r = Random.Range(0, animalNames.Length);
				wrongs.Add(animalNames[r]);
			}
			animals.Add(new EducationalQuestion { displayChar = animalNames[i], correctAnswer = animalNames[i], wrongAnswers = wrongs });
		}
		questionBank[EducationalSubject.Animals] = animals;

		List<EducationalQuestion> colors = new List<EducationalQuestion>();
		string[] colorNames = new string[] { "RED", "BLUE", "GREEN", "YELLOW", "ORANGE", "PURPLE", "PINK", "BROWN", "BLACK", "WHITE" };
		for (int i = 0; i < colorNames.Length; i++)
		{
			List<string> wrongs = new List<string>();
			for (int j = 0; j < 2; j++)
			{
				int r = Random.Range(0, colorNames.Length);
				while (r == i || wrongs.Contains(colorNames[r])) r = Random.Range(0, colorNames.Length);
				wrongs.Add(colorNames[r]);
			}
			colors.Add(new EducationalQuestion { displayChar = colorNames[i], correctAnswer = colorNames[i], wrongAnswers = wrongs });
		}
		questionBank[EducationalSubject.Colors] = colors;

		List<EducationalQuestion> shapes = new List<EducationalQuestion>();
		string[] shapeNames = new string[] { "CIRCLE", "SQUARE", "TRIANGLE", "STAR", "HEART", "DIAMOND", "OVAL", "RECTANGLE" };
		for (int i = 0; i < shapeNames.Length; i++)
		{
			List<string> wrongs = new List<string>();
			for (int j = 0; j < 2; j++)
			{
				int r = Random.Range(0, shapeNames.Length);
				while (r == i || wrongs.Contains(shapeNames[r])) r = Random.Range(0, shapeNames.Length);
				wrongs.Add(shapeNames[r]);
			}
			shapes.Add(new EducationalQuestion { displayChar = shapeNames[i], correctAnswer = shapeNames[i], wrongAnswers = wrongs });
		}
		questionBank[EducationalSubject.Shapes] = shapes;
	}

	public void StartEducationalMode(EducationalSubject subject)
	{
		isEducationalModeActive = true;
		currentSubject = subject;
		currentScore = 0;
		currentRound = 0;

		if (educationalPanel != null)
			educationalPanel.SetActive(true);

		if (scoreText != null)
			scoreText.text = "Score: 0";

		ShowNextQuestion();
	}

	public void StopEducationalMode()
	{
		isEducationalModeActive = false;
		if (educationalPanel != null)
			educationalPanel.SetActive(false);
	}

	void ShowNextQuestion()
	{
		if (!questionBank.ContainsKey(currentSubject))
			return;

		List<EducationalQuestion> questions = questionBank[currentSubject];
		int index = Random.Range(0, questions.Count);
		EducationalQuestion q = questions[index];

		if (questionText != null)
			questionText.text = "Find: " + q.displayChar;

		List<string> allAnswers = new List<string>(q.wrongAnswers);
		allAnswers.Add(q.correctAnswer);

		for (int i = allAnswers.Count - 1; i > 0; i--)
		{
			int j = Random.Range(0, i + 1);
			string tmp = allAnswers[i];
			allAnswers[i] = allAnswers[j];
			allAnswers[j] = tmp;
		}

		for (int i = 0; i < answerButtons.Length && i < allAnswers.Count; i++)
		{
			int capturedIndex = i;
			string answer = allAnswers[i];
			if (answerButtonTexts != null && i < answerButtonTexts.Length && answerButtonTexts[i] != null)
				answerButtonTexts[i].text = answer;

			if (answerButtons[i] != null)
			{
				Button btn = answerButtons[i].GetComponent<Button>();
				if (btn != null)
				{
					btn.onClick.RemoveAllListeners();
					btn.onClick.AddListener(() => OnAnswerSelected(answer, q.correctAnswer));
				}
			}
		}
	}

	void OnAnswerSelected(string selected, string correct)
	{
		if (selected == correct)
		{
			currentScore += 10;
			currentRound++;
			SoundManager.PlaySound("EducCorrect");

			if (scoreText != null)
				scoreText.text = "Score: " + currentScore;

			if (currentRound >= 10)
			{
				AchievementManager.AddProgress("educ_10");
				ShopManager.AddCoins(50);
				if (questionText != null)
					questionText.text = "Complete! +50 coins!";
				Invoke("StopEducationalMode", 2f);
				return;
			}

			ShowNextQuestion();
		}
		else
		{
			SoundManager.PlaySound("EducWrong");
			if (questionText != null)
				questionText.text = "Try again!";
			Invoke("RestoreQuestion", 1f);
		}
	}

	void RestoreQuestion()
	{
		ShowNextQuestion();
	}

	public static bool IsEducationalBaloonType(int baloonType)
	{
		return baloonType >= 10;
	}
}
