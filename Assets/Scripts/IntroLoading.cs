using UnityEngine;
using System.Collections;
using UnityEngine.UI;

///<summary>
///<para>Scene:All/NameOfScene/NameOfScene1,NameOfScene2,NameOfScene3...</para>
///<para>Object:N/A</para>
///<para>Description: Sample Description </para>
///</summary>

public class IntroLoading : MonoBehaviour {

	public void LoadSpashScreen()
	{
		// First case 9:16, second 10:16, third 3:4
		if (Screen.width > 900f && Camera.main.aspect < 0.6f)
			Screen.SetResolution(720, 1280, true);
		else if (Screen.width > 850f && Camera.main.aspect > 0.6f && Camera.main.aspect < 0.7f)
			Screen.SetResolution(800, 1280, true);
		else if (Screen.width > 1000f && Camera.main.aspect > 0.7f && Camera.main.aspect < 0.8f)
			Screen.SetResolution(960, 1280, true);

		Application.LoadLevel("Splash");
	}
}
