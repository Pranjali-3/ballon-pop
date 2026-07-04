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
