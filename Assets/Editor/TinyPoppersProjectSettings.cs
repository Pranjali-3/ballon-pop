using UnityEditor;

[InitializeOnLoad]
public static class TinyPoppersProjectSettings
{
	const string ProductName = "Tiny Poppers";

	static TinyPoppersProjectSettings()
	{
		if (PlayerSettings.productName != ProductName)
		{
			PlayerSettings.productName = ProductName;
			AssetDatabase.SaveAssets();
		}
	}
}
