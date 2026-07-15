using UnityEngine;
using System.Collections;

/**
  * Scene: Sve
  * Object: Menu objekti
  * Description: Skripta zaduzena za Menu-je
  **/
public class Menu : MonoBehaviour {


	private Animator _animtor;
	private bool _hasIsOpenParam;

	public bool IsOpen
	{
		get
		{
			if (_animtor != null && _hasIsOpenParam)
				return _animtor.GetBool("IsOpen");
			return false;
		}
		set
		{
			if (_animtor != null && _hasIsOpenParam && _animtor.isActiveAndEnabled)
				_animtor.SetBool("IsOpen", value);
		}
	}

	// Use this for initialization
	public void Awake () 
	{
		_animtor = GetComponent<Animator> ();
		if (_animtor != null)
		{
			foreach (var p in _animtor.parameters)
			{
				if (p.name == "IsOpen") { _hasIsOpenParam = true; break; }
			}
		}

		var rect = GetComponent<RectTransform> ();
		rect.offsetMax = rect.offsetMin = new Vector2 (0, 0);
	}

	public void ResetObject()
	{
		gameObject.SetActive (false);
	}
	
	public void DisableObject(string gameObjectName)
	{
		GameObject gameObject= GameObject.Find (gameObjectName);
		if (gameObject != null) 
		{
			if (gameObject.activeSelf) 
			{
				gameObject.SetActive (false);
			}
		}
	}

	
}
