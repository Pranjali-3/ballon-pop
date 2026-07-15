using UnityEngine;

public static class FeatureBootstrapper
{
	public static void EnsureManagers()
	{
		EnsureManager<ComboManager>("ComboManager");
		EnsureManager<MissionManager>("MissionManager");
		EnsureManager<ToyAlbumManager>("ToyAlbumManager");
		EnsureManager<ThemeProgressionManager>("ThemeProgressionManager");
		EnsureManager<ProgressionMapManager>("ProgressionMapManager");
		EnsureManager<ShopManager>("ShopManager");
		EnsureManager<AchievementManager>("AchievementManager");
		EnsureManager<EducationalModeManager>("EducationalModeManager");
		EnsureSoundManager();
	}

	static void EnsureSoundManager()
	{
		SoundManager existing = GameObject.FindObjectOfType<SoundManager>() as SoundManager;
		if (existing != null)
			return;
		GameObject holder = new GameObject("SoundManager");
		GameObject.DontDestroyOnLoad(holder);
		holder.AddComponent<SoundManager>();
	}

	static T EnsureManager<T>(string objectName) where T : Component
	{
		T existing = GameObject.FindObjectOfType(typeof(T)) as T;
		if (existing != null)
			return existing;

		GameObject holder = new GameObject(objectName);
		GameObject.DontDestroyOnLoad(holder);
		return holder.AddComponent<T>();
	}
}
