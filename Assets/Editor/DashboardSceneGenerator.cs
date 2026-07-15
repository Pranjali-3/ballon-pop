#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class DashboardSceneGenerator
{
	const string DashboardScenePath = "Assets/Scenes/Dashboard.unity";

	static DashboardSceneGenerator()
	{
		EditorApplication.delayCall += EnsureDashboardScene;
	}

	[MenuItem("Tools/Tiny Poppers/Rebuild Dashboard Scene")]
	public static void EnsureDashboardScene()
	{
		if (EditorApplication.isPlayingOrWillChangePlaymode)
			return;

		if (!File.Exists(DashboardScenePath))
			CreateDashboardScene();

		EnsureSceneInBuildSettings();
	}

	static void CreateDashboardScene()
	{
		var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

		GameObject cameraObject = new GameObject("Main Camera");
		Camera camera = cameraObject.AddComponent<Camera>();
		camera.clearFlags = CameraClearFlags.SolidColor;
		camera.backgroundColor = new Color(0.08f, 0.11f, 0.18f, 1f);
		cameraObject.AddComponent<AudioListener>();
		cameraObject.tag = "MainCamera";

		GameObject dashboardObject = new GameObject("DashboardSceneBuilder");
		dashboardObject.AddComponent<DashboardSceneBuilder>();

		EditorSceneManager.SaveScene(scene, DashboardScenePath);
		AssetDatabase.Refresh();
	}

	static void EnsureSceneInBuildSettings()
	{
		string[] desiredScenes = new string[]
		{
			"Assets/Scenes/Splash.unity",
			"Assets/Scenes/MainScene.unity",
			DashboardScenePath,
			"Assets/Scenes/Level.unity"
		};

		EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[desiredScenes.Length];
		for (int i = 0; i < desiredScenes.Length; i++)
			scenes[i] = new EditorBuildSettingsScene(desiredScenes[i], true);

		EditorBuildSettings.scenes = scenes;
	}
}
#endif
