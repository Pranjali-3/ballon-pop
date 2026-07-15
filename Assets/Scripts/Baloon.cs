using UnityEngine;
using System.Collections;
using UnityEngine.UI;

///<summary>
///<para>Scene:All/NameOfScene/NameOfScene1,NameOfScene2,NameOfScene3...</para>
///<para>Object:N/A</para>
///<para>Description: Sample Description </para>
///</summary>

public class Baloon : MonoBehaviour {

	public GameObject toyObjectHolder; // Toy inside of baloon
	public float movingSpeed; // Speed that baloon is moving upwards

	public GameObject particleHolder;

	/// 0 - regular baloon, no inner object
	/// 1 - regular baloon, one inner object
	/// 2 - when this baloon pops it creates more baloons
	/// 3 - baloon that adds time
	/// 4 - baloon that substract time
	/// 5 - bomb baloon, destroyes baloons in radius
	/// 6 - double points baloon

	public int baloonType;

	void Awake()
	{
		// Set baloon type FIXME za sada stavljam da bude to random vrednost
		if (tag == "Baloon" || tag != "InnerBaloon")
		{
//			baloonType = Random.Range(0, 6);
//
//			switch(baloonType)
//			{
//			case 0:
//				// Destroying uneccessary objects
//				Destroy(transform.GetChild(0).gameObject);
//				break;
//			case 1:
//				// Destroying uneccessary objects
//				Destroy(transform.GetChild(0).gameObject);
//
//				// Create toy object
//				int r = Random.Range(0, LevelManager.levelManager.listOfToys.Length);
//
//				GameObject toy = Instantiate(LevelManager.levelManager.listOfToys[r], transform.localPosition, LevelManager.levelManager.listOfToys[r].transform.rotation) as GameObject;
//				toy.transform.SetParent(transform);
//				toy.transform.SetAsFirstSibling();
//				toy.transform.localScale = Vector3.one;
//				toy.transform.localPosition = Vector3.zero;
//				break;
//			case 2:
//				// Destroying uneccessary objects
//				break;
//			case 3:
//				// Destroying uneccessary objects
//				Destroy(transform.GetChild(0).gameObject);
//
//				GetComponent<Image>().sprite = LevelManager.levelManager.addTimeSprite;
//				GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
//				break;
//			case 4:
//				// Destroying uneccessary objects
//				Destroy(transform.GetChild(0).gameObject);
//
//				GetComponent<Image>().sprite = LevelManager.levelManager.loseTimeSprite;
//				GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
//				break;
//			case 5:
//				// Destroying uneccessary objects
//				Destroy(transform.GetChild(0).gameObject);
//
//				GetComponent<Image>().sprite = LevelManager.levelManager.bombSprite;
//				GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
//				break;
//			default:
//				break;
//			}

			int val = Random.Range(0, 60);

			if (val == 17)
			{
				// Set baloon type
				baloonType = 5; // bomb

				// Destroying uneccessary objects
				Destroy(transform.GetChild(1).gameObject);

				// Set BaloonTypeImage
				transform.GetChild(2).GetComponent<Image>().sprite = LevelManager.levelManager.bombSprite;
				transform.GetChild(2).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
			}
			else if (val == 27)
			{
				// Set baloon type
				baloonType = 3; // add time

				// Destroying uneccessary objects
				Destroy(transform.GetChild(1).gameObject);

				// Set BaloonTypeImage
				transform.GetChild(2).GetComponent<Image>().sprite = LevelManager.levelManager.addTimeSprite;
				transform.GetChild(2).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
			}
			// FIXME Double points
//			else if (val == 37)
//			{
//				// Set baloon type
//				baloonType = 6; // double coins
//
//				// Destroying uneccessary objects
//				Destroy(transform.GetChild(0).gameObject);
//
//				// Set BaloonTypeImage
//				transform.GetChild(2).GetComponent<Image>().sprite = LevelManager.levelManager.doublePointsSprite;
//				transform.GetChild(2).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
//			}
			else if (val == 4 || val == 25 || val == 43)
			{
				// Set baloon type
				baloonType = 4; // substract time

				// Destroying uneccessary objects
				Destroy(transform.GetChild(1).gameObject);

				// Set BaloonTypeImage
				transform.GetChild(2).GetComponent<Image>().sprite = LevelManager.levelManager.loseTimeSprite;
				transform.GetChild(2).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
			}
			else if (val == 5 || val == 28 || val == 41)
			{
				// Set baloon type
				baloonType = 2; // baloons inside this baloon
			}
			else if (val > 34 && val % 2 == 0)
			{
				// Set baloon type
				baloonType = 1; // toy inside this baloon

				// Destroying uneccessary objects
				Destroy(transform.GetChild(1).gameObject);

				// Create toy object
				int r = Random.Range(0, LevelManager.levelManager.listOfToys.Length);

				GameObject toy = Instantiate(LevelManager.levelManager.listOfToys[r], transform.localPosition, LevelManager.levelManager.listOfToys[r].transform.rotation) as GameObject;
				toy.transform.SetParent(transform);
				// toy.transform.SetAsFirstSibling();
				toy.transform.localScale = Vector3.one;
				toy.transform.localPosition = Vector3.zero;
				ToyCollectible collectible = toy.AddComponent<ToyCollectible>();
				collectible.toyId = r;
				collectible.toyName = LevelManager.levelManager.listOfToys[r].name;
			}
			else
			{
				// Set baloon type
				baloonType = 0; // regular baloon

				// Destroying uneccessary objects
				Destroy(transform.GetChild(1).gameObject);
			}

			float minSpeed = LevelManager.levelManager != null ? LevelManager.levelManager.minBaloonSpeed : 2.0f;
			float maxSpeed = LevelManager.levelManager != null ? LevelManager.levelManager.maxBaloonSpeed : 4.0f;
			movingSpeed = Random.Range(minSpeed, maxSpeed);
			if (movingSpeed <= 0f) movingSpeed = 2f;
		}
		else if (tag == "InnerBaloon")
		{
			baloonType = Random.Range(0, 2);

			switch(baloonType)
			{
			case 0:
				break;
			case 1:
				// Create toy object
				int r = Random.Range(0, LevelManager.levelManager.listOfToys.Length);

				GameObject toy = Instantiate(LevelManager.levelManager.listOfToys[r], Vector3.zero, LevelManager.levelManager.listOfToys[r].transform.rotation) as GameObject;
				toy.transform.SetParent(transform);
				// toy.transform.SetAsFirstSibling();
				toy.transform.localScale = Vector3.one;
				toy.transform.localPosition = Vector3.zero;
				ToyCollectible collectible = toy.AddComponent<ToyCollectible>();
				collectible.toyId = r;
				collectible.toyName = LevelManager.levelManager.listOfToys[r].name;
				break;
			default:
				break;
			}
		}
	}

	public void MakeEducationalBaloon(EducationalSubject subject)
	{
		baloonType = 10 + (int)subject;

		string label = "";
		switch (subject)
		{
		case EducationalSubject.Letters:
			label = ((char)('A' + Random.Range(0, 26))).ToString();
			break;
		case EducationalSubject.Numbers:
			label = Random.Range(1, 21).ToString();
			break;
		case EducationalSubject.Animals:
			label = new string[] { "CAT", "DOG", "BIRD", "FISH", "LION", "BEAR", "DUCK", "FROG", "OWL", "COW" }[Random.Range(0, 10)];
			break;
		case EducationalSubject.Colors:
			label = new string[] { "RED", "BLUE", "GREEN", "YELLOW", "ORANGE", "PURPLE", "PINK", "BROWN", "BLACK", "WHITE" }[Random.Range(0, 10)];
			break;
		case EducationalSubject.Shapes:
			label = new string[] { "CIRCLE", "SQUARE", "TRIANGLE", "STAR", "HEART", "DIAMOND", "OVAL", "RECTANGLE" }[Random.Range(0, 8)];
			break;
		}

		Text labelText = GetComponentInChildren<Text>();
		if (labelText == null)
		{
			GameObject textGo = new GameObject("EducationalText");
			textGo.transform.SetParent(transform, false);
			labelText = textGo.AddComponent<Text>();
			labelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			if (labelText.font == null)
				labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

			RectTransform rt = textGo.GetComponent<RectTransform>();
			if (rt != null)
			{
				rt.anchorMin = Vector2.zero;
				rt.anchorMax = Vector2.one;
				rt.offsetMin = Vector2.zero;
				rt.offsetMax = Vector2.zero;
			}
		}

		labelText.text = label;
		labelText.fontSize = 28;
		labelText.fontStyle = FontStyle.Bold;
		labelText.color = Color.white;
		labelText.alignment = TextAnchor.MiddleCenter;
		labelText.resizeTextForBestFit = true;
		labelText.resizeTextMinSize = 12;
		labelText.resizeTextMaxSize = 32;

		for (int i = 0; i < transform.childCount; i++)
		{
			Image img = transform.GetChild(i).GetComponent<Image>();
			if (img != null)
			{
				img.enabled = false;
			}
		}
	}

	void LateUpdate()
	{
		 transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + movingSpeed * Time.deltaTime, transform.localPosition.z);
	}
}
