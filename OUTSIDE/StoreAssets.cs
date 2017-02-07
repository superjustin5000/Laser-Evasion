using UnityEngine;
using System.Collections;

using Soomla.Store;

public class StoreAssets : IStoreAssets {

	public int GetVersion() {														// Get Current Version
		return 0;
	}
	
	public VirtualCurrency[] GetCurrencies() {										// Get/Setup Virtual Currencies
		return new VirtualCurrency[]{};
	}
	
	public VirtualGood[] GetGoods() {												// Add Producs to goods.
		return new VirtualGood[]{TRACK_2, TRACK_3, TRACK_4, TRACK_5, TRACK_ALL, COINS_4, COINS_15, COINS_50};
	}
	
	public VirtualCurrencyPack[] GetCurrencyPacks() {								// Get/Setup Currency Packs
		return new VirtualCurrencyPack[]{};
	}
	
	public VirtualCategory[] GetCategories() {										// Get/ Setup Categories (for In App Purchases)
		return new VirtualCategory[]{};
	}



	//****************************BOILERPLATE ABOVE(modify as you see fit/ if nessisary)***********************
	
	public const string NO_ADS_PRODUCT_ID = "IAPNoAds";  //SAME AS APPLE PRODUCT ID FOR IAP
	public const string TRACK_2_PRODUCT_ID = "IAPTrack2";  //SAME AS APPLE PRODUCT ID FOR IAP
	public const string TRACK_3_PRODUCT_ID = "IAPTrack3";  //SAME AS APPLE PRODUCT ID FOR IAP
	public const string TRACK_4_PRODUCT_ID = "IAPTrack4";  //SAME AS APPLE PRODUCT ID FOR IAP
	public const string TRACK_5_PRODUCT_ID = "IAPTrack5";  //SAME AS APPLE PRODUCT ID FOR IAP
	public const string TRACK_ALL_PRODUCT_ID = "IAPTrackALL";  //SAME AS APPLE PRODUCT ID FOR IAP
	//public const string SCORE_MULT_025_PRODUCT_ID = "IAPScoreMult025";  //SAME AS APPLE PRODUCT ID FOR IAP
	//public const string SCORE_MULT_075_PRODUCT_ID = "IAPScoreMult075";  //SAME AS APPLE PRODUCT ID FOR IAP
	public const string COINS_4_PRODUCT_ID = "IAP4Coins";
	public const string COINS_15_PRODUCT_ID = "IAP15Coins";
	public const string COINS_50_PRODUCT_ID = "IAP50Coins";


	/*
	public static VirtualGood NO_ADS = new LifetimeVG(
		"No Ads", //name
		"Removes and stops showing ads forever", //description
		"IAPNoAds", //id for soomla store.
		
		new PurchaseWithMarket(NO_ADS_PRODUCT_ID, 1.99) //create purchase with apple id and price.
		);

	*/
	public static VirtualGood TRACK_2 = new LifetimeVG(
		"Track 2", //name
		"Removes and stops showing ads forever", //description
		"IAPTrack2", //id for soomla store.
		
		new PurchaseWithMarket(TRACK_2_PRODUCT_ID, 0.99) //create purchase with apple id and price.
		);

	public static VirtualGood TRACK_3 = new LifetimeVG(
		"Track 2", //name
		"Unlocks Track 2 - The Driver, by Night Runner.", //description
		"IAPTrack3", //id for soomla store.
		
		new PurchaseWithMarket(TRACK_3_PRODUCT_ID, 0.99) //create purchase with apple id and price.
		);

	public static VirtualGood TRACK_4 = new LifetimeVG(
		"Track 3", //name
		"Unlocks Track 3 - The Sentinels, by Night Runner.", //description
		"IAPTrack4", //id for soomla store.
		
		new PurchaseWithMarket(TRACK_4_PRODUCT_ID, 0.99) //create purchase with apple id and price.
		);

	public static VirtualGood TRACK_5 = new LifetimeVG(
		"Track 4", //name
		"Unlocks Track 4 - Invaders, by Night Runner.", //description
		"IAPTrack5", //id for soomla store.
		
		new PurchaseWithMarket(TRACK_5_PRODUCT_ID, 0.99) //create purchase with apple id and price.
		);

	public static VirtualGood TRACK_ALL = new LifetimeVG(
		"Track 5", //name
		"Unlocks Track 5 - Almost There, by Night Runner.", //description
		"IAPTrackALL", //id for soomla store.
		
		new PurchaseWithMarket(TRACK_ALL_PRODUCT_ID, 1.99) //create purchase with apple id and price.
		);

	public static VirtualGood COINS_4 = new SingleUseVG(
		"4 Coins",
		"Adds 4 Coins",
		"IAP4Coins",

		new PurchaseWithMarket(COINS_4_PRODUCT_ID, 0.99)
		);

	public static VirtualGood COINS_15 = new SingleUseVG(
		"15 Coins",
		"Adds 15 Coins",
		"IAP15Coins",
		
		new PurchaseWithMarket(COINS_15_PRODUCT_ID, 2.99)
		);

	public static VirtualGood COINS_50 = new SingleUseVG(
		"50 Coins",
		"Adds 50 Coins",
		"IAP50Coins",
		
		new PurchaseWithMarket(COINS_50_PRODUCT_ID, 3.99)
		);

	/*
	public static VirtualGood SCORE_MULT_025 = new SingleUseVG(
		"Multiplier Up + 0.25", //name
		"Increases the permanent base score multiplier by an additional 0.75.", //description
		"IAPScoreMult025", //id for soomla store.
		
		new PurchaseWithMarket(SCORE_MULT_025_PRODUCT_ID, 0.99) //create purchase with apple id and price.
		);

	public static VirtualGood SCORE_MULT_075 = new SingleUseVG(
		"Multiplier Up + 0.75", //name
		"Increases the permanent base score multiplier by an additional 0.75.", //description
		"IAPScoreMult075", //id for soomla store.
		
		new PurchaseWithMarket(SCORE_MULT_075_PRODUCT_ID, 1.99) //create purchase with apple id and price.
		);
*/

}
