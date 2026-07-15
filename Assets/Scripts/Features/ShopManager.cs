using UnityEngine;
using UnityEngine.UI;

public enum PowerUpType
{
	ExtraTime,
	DoublePoints,
	Shield,
	SlowMotion,
	Magnet
}

[System.Serializable]
public class ShopItem
{
	public PowerUpType type;
	public string itemName;
	public string description;
	public int price;
	public int quantity;
	public Sprite iconSprite;
}

public class ShopManager : MonoBehaviour
{
	const string CoinsKey = "PlayerCoins";
	const string PowerUpPrefix = "PowerUp_";

	public static ShopManager Instance;

	public ShopItem[] shopItems;
	public Text coinsText;
	public GameObject notEnoughCoinsPopup;

	void Awake()
	{
		Instance = this;
		EnsureDefaultItems();
		UpdateCoinsUI();
	}

	void EnsureDefaultItems()
	{
		if (shopItems != null && shopItems.Length > 0)
			return;

		shopItems = new ShopItem[5];
		shopItems[0] = new ShopItem { type = PowerUpType.ExtraTime, itemName = "Extra Time", description = "+30 seconds", price = 100, quantity = 0 };
		shopItems[1] = new ShopItem { type = PowerUpType.DoublePoints, itemName = "Double Points", description = "2x points for 15s", price = 200, quantity = 0 };
		shopItems[2] = new ShopItem { type = PowerUpType.Shield, itemName = "Shield", description = "Protect from bad balloons", price = 150, quantity = 0 };
		shopItems[3] = new ShopItem { type = PowerUpType.SlowMotion, itemName = "Slow Motion", description = "Slower balloons for 10s", price = 180, quantity = 0 };
		shopItems[4] = new ShopItem { type = PowerUpType.Magnet, itemName = "Magnet", description = "Pull toys toward you", price = 250, quantity = 0 };
	}

	public static int GetCoins()
	{
		return PlayerPrefs.GetInt(CoinsKey, 0);
	}

	public static void AddCoins(int amount)
	{
		int current = GetCoins();
		PlayerPrefs.SetInt(CoinsKey, current + amount);
		PlayerPrefs.Save();
		if (Instance != null)
			Instance.UpdateCoinsUI();
		SoundManager.PlaySound("CoinCollect");
	}

	public static bool SpendCoins(int amount)
	{
		int current = GetCoins();
		if (current < amount)
		{
			if (Instance != null && Instance.notEnoughCoinsPopup != null)
				Instance.notEnoughCoinsPopup.SetActive(true);
			return false;
		}
		PlayerPrefs.SetInt(CoinsKey, current - amount);
		PlayerPrefs.Save();
		if (Instance != null)
			Instance.UpdateCoinsUI();
		return true;
	}

	public static int GetPowerUpQuantity(PowerUpType type)
	{
		return PlayerPrefs.GetInt(PowerUpPrefix + type.ToString(), 0);
	}

	public static void AddPowerUp(PowerUpType type, int amount)
	{
		string key = PowerUpPrefix + type.ToString();
		PlayerPrefs.SetInt(key, PlayerPrefs.GetInt(key, 0) + amount);
		PlayerPrefs.Save();
	}

	public static bool UsePowerUp(PowerUpType type)
	{
		string key = PowerUpPrefix + type.ToString();
		int current = PlayerPrefs.GetInt(key, 0);
		if (current <= 0)
			return false;
		PlayerPrefs.SetInt(key, current - 1);
		PlayerPrefs.Save();
		return true;
	}

	public bool BuyItem(int itemIndex)
	{
		if (shopItems == null || itemIndex < 0 || itemIndex >= shopItems.Length)
			return false;

		ShopItem item = shopItems[itemIndex];
		if (!SpendCoins(item.price))
			return false;

		AddPowerUp(item.type, 1);
		SoundManager.PlaySound("InappBought");

		int coins = GetCoins();
		AchievementManager.CheckCoinAchievements(coins);

		return true;
	}

	public void UpdateCoinsUI()
	{
		if (coinsText != null)
			coinsText.text = GetCoins().ToString();
	}
}
