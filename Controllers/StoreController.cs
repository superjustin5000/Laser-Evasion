using System; //for try catch and exceptions.
using UnityEngine;
using System.Collections;

using UnityEngine.UI; //for ui elements.
using System.Collections.Generic; //for dictionaries.

//In App Purchases
using Soomla.Store;

[RequireComponent(typeof(AudioSource))]
public class StoreController : MonoBehaviour {

	GameState gs;

	#region
	[Header("STORE ITEMS")]
	Transform storeItem_ScoreMult025;
	Transform storeItem_ScoreMult075;
	Transform storeItem_Track2;
	Transform storeItem_Track3;
	Transform storeItem_Track4;
	Transform storeItem_Track5;
	Transform storeItem_TrackALL;
	Transform storeItem_4Coins;
	Transform storeItem_15Coins;
	Transform storeItem_50Coins;
	Transform storeItem_Restore;
	#endregion


	Transform loadingScreen;

	Transform storeItem_Previewing;
	bool previewing = false;

	AudioSource previewPlayer;
	Coroutine previewMeter;
	Sprite play;
	Sprite stop;

	#region
	[Header("SOUND EFFECTS")]
	public AudioClip sfx_buttonPress;
	public AudioClip track2;
	public AudioClip track3;
	public AudioClip track4;
	public AudioClip track5;

	public AudioClip sfx_cash;
	#endregion

	// Use this for initialization
	void Start () {
		gs = GameState.sharedGameState;

		gs.store = this;

		//store STUFF..
		if (!gs.soomlaInit) {
			
			//Allows this gameObject to remain during level loads, solving restart crashes
			StoreEvents.OnSoomlaStoreInitialized += onSoomlaStoreIntitialized;	
			
			
			StoreEvents.OnMarketPurchaseStarted += onMarketPurchaseStarted;
			StoreEvents.OnMarketPurchase += onMarketPurchase;
			StoreEvents.OnMarketPurchaseCancelled += onMarketPurchaseCancelled;
			StoreEvents.OnRestoreTransactionsStarted += onRestoreTransactionsStarted;
			StoreEvents.OnRestoreTransactionsFinished += onRestoreTransactionsFinished;
			//StoreEvents.OnBillingSupported += onBillingSupported;
			//StoreEvents.OnBillingNotSupported += onBillingNotSupported;
			StoreEvents.OnUnexpectedStoreError += onUnexpectedStoreError;
			
			//Handle the initialization of store events (calls function below - unneeded in this case)
			SoomlaStore.Initialize (new StoreAssets());
			
		}

		Transform storeItemList = transform.FindChild("ScrollView").FindChild("Store_Items");
		storeItem_ScoreMult025 = storeItemList.FindChild("Store_Item_ScoreMult_025");
		storeItem_ScoreMult075 = storeItemList.FindChild("Store_Item_ScoreMult_075");
		storeItem_Track2 = storeItemList.FindChild("Store_Item_Track_2");
		storeItem_Track3 = storeItemList.FindChild("Store_Item_Track_3");
		storeItem_Track4 = storeItemList.FindChild("Store_Item_Track_4");
		storeItem_Track5 = storeItemList.FindChild("Store_Item_Track_5");
		storeItem_4Coins = storeItemList.FindChild("Store_Item_4Coins");
		storeItem_15Coins = storeItemList.FindChild("Store_Item_15Coins");
		storeItem_50Coins = storeItemList.FindChild("Store_Item_50Coins");
		storeItem_TrackALL = storeItemList.FindChild("Store_Item_Track_ALL");

		storeItem_Restore = storeItemList.FindChild("Store_Item_Restore");

		
		
		loadingScreen = GameObject.Find("LoadingScreen").transform;
		Renderer loadingRend = loadingScreen.FindChild("Quad").GetComponent<Renderer>();
		loadingRend.sortingLayerName = "HUD";
		loadingRend.sortingOrder = 10500;
		
		
		//buttonNoAdsBuy = GameObject.Find("Button_NoAds").GetComponent<Button>();



		previewPlayer = GetComponent<AudioSource>();
		play = Resources.Load<Sprite>("art/icons/Icon_Play");
		stop = Resources.Load<Sprite>("art/icons/Icon_Stop");



		//set the line renderers of the track previews to be gone and sorting layers to be higher.
		LineRenderer Track2Line = storeItem_Track2.FindChild("TrackPreviewMeter").GetComponent<LineRenderer>();
		LineRenderer Track3Line = storeItem_Track3.FindChild("TrackPreviewMeter").GetComponent<LineRenderer>();
		LineRenderer Track4Line = storeItem_Track4.FindChild("TrackPreviewMeter").GetComponent<LineRenderer>();
		LineRenderer Track5Line = storeItem_Track5.FindChild("TrackPreviewMeter").GetComponent<LineRenderer>();

		Track2Line.SetPosition(1, new Vector3(0f, 0, 0));
		Track3Line.SetPosition(1, new Vector3(0f, 0, 0));
		Track4Line.SetPosition(1, new Vector3(0f, 0, 0));
		Track5Line.SetPosition(1, new Vector3(0f, 0, 0));

		Track2Line.sortingLayerName = "HUD";
		Track3Line.sortingLayerName = "HUD";
		Track4Line.sortingLayerName = "HUD";
		Track5Line.sortingLayerName = "HUD";

		Track2Line.sortingOrder = 12000;
		Track3Line.sortingOrder = 12000;
		Track4Line.sortingOrder = 12000;
		Track5Line.sortingOrder = 12000;


	}

	/// <summary>
	/// Checks the purchase status.
	/// Blocks purchases buy adding an unlocked image over top, if they have been unlocked or reached max.
	/// </summary>
	void CheckPurchaseStatus() {

		Debug.Log("CHECKING PURCHASE STATUS");

		bool purchasedTrack2 = (StoreInventory.GetItemBalance("IAPTrack2") >= 1) || gs.tracksUnlocked[1];
		bool purchasedTrack3 = (StoreInventory.GetItemBalance("IAPTrack3") >= 1) || gs.tracksUnlocked[2];
		bool purchasedTrack4 = (StoreInventory.GetItemBalance("IAPTrack4") >= 1) || gs.tracksUnlocked[3];
		bool purchasedTrack5 = (StoreInventory.GetItemBalance("IAPTrack5") >= 1) || gs.tracksUnlocked[4];
		bool allTracks = purchasedTrack2 && purchasedTrack3 && purchasedTrack4 && purchasedTrack5;
		bool purchasedTrackALL = (StoreInventory.GetItemBalance("IAPTrackALL") >= 1) || allTracks;

		bool limitBaseMult = gs.baseMult >= 999f;
		bool limitCoins = gs.numCoins >= 9999f;

		//prevents from buying extra scrore multipliers when the base multiplier is at it's limit.
		if (!limitBaseMult) {
			storeItem_ScoreMult025.FindChild("Unlocked").gameObject.SetActive(false);
			storeItem_ScoreMult075.FindChild("Unlocked").gameObject.SetActive(false);
		}
		else {
			storeItem_ScoreMult025.FindChild("Unlocked").gameObject.SetActive(true);
			storeItem_ScoreMult075.FindChild("Unlocked").gameObject.SetActive(true);
		}

		if (!limitCoins) {
			if (gs.numCoins >= 4 && gs.numCoins < 8) {
				storeItem_ScoreMult025.FindChild("Buy_Button").GetComponent<Button>().interactable = true;
				storeItem_ScoreMult075.FindChild("Buy_Button").GetComponent<Button>().interactable = false;
			}
			else if (gs.numCoins >= 8) {
				storeItem_ScoreMult025.FindChild("Buy_Button").GetComponent<Button>().interactable = true;
				storeItem_ScoreMult075.FindChild("Buy_Button").GetComponent<Button>().interactable = true;
			}
			else {
				storeItem_ScoreMult025.FindChild("Buy_Button").GetComponent<Button>().interactable = false;
				storeItem_ScoreMult075.FindChild("Buy_Button").GetComponent<Button>().interactable = false;
			}

			storeItem_4Coins.FindChild("Unlocked").gameObject.SetActive(false);
			storeItem_15Coins.FindChild("Unlocked").gameObject.SetActive(false);
			storeItem_50Coins.FindChild("Unlocked").gameObject.SetActive(false);
		}
		else {
			storeItem_4Coins.FindChild("Unlocked").gameObject.SetActive(true);
			storeItem_15Coins.FindChild("Unlocked").gameObject.SetActive(true);
			storeItem_50Coins.FindChild("Unlocked").gameObject.SetActive(true);
		}




		if (!purchasedTrack2) {
			storeItem_Track2.FindChild("Unlocked").gameObject.SetActive(false);
		}
		else { //set it to unlocked, and make sure it is in the code.
			storeItem_Track2.FindChild("Unlocked").gameObject.SetActive(true);
			PurchasedTrack2();
		}

		if (!purchasedTrack3) {
			storeItem_Track3.FindChild("Unlocked").gameObject.SetActive(false);
		}
		else {
			storeItem_Track3.FindChild("Unlocked").gameObject.SetActive(true);
			PurchasedTrack3();
		}

		if (!purchasedTrack4) {
			storeItem_Track4.FindChild("Unlocked").gameObject.SetActive(false);
		}
		else {
			storeItem_Track4.FindChild("Unlocked").gameObject.SetActive(true);
			PurchasedTrack4();
		}

		if (!purchasedTrack5) {
			storeItem_Track5.FindChild("Unlocked").gameObject.SetActive(false);
		}
		else {
			storeItem_Track5.FindChild("Unlocked").gameObject.SetActive(true);
			PurchasedTrack5();
		}

		if (!purchasedTrackALL) {
			storeItem_TrackALL.FindChild("Unlocked").gameObject.SetActive(false);
		}
		else {
			storeItem_TrackALL.FindChild("Unlocked").gameObject.SetActive(true);
			PurchasedTrackALL();
		}

		/*
		if (StoreInventory.GetItemBalance("IAPNoAds") >= 1) 
		{
			gs.iap_NoAds = true;		// check if the non-consumable in app purchase has been bought or not
		}
		*/
		//buttonNoAdsBuy.interactable = !gs.iap_NoAds;

	}



	/// <summary>
	/// IN APP PURCHASE METHODS
	/// </summary>
	/// 
	//this is likely unnecessary, but may be required depending on how you plan on doing IAPS

	public void onSoomlaStoreIntitialized(){
		gs.soomlaInit = true;
	}


	public void AttemptToPurchase_NoAds() {

		gs.ac.PlaySFX(sfx_buttonPress);

		AttemptToPurchase("IAPNoAds");

	}


	public void AttemptToPurchase_Track2() {
		gs.ac.PlaySFX(sfx_buttonPress);
		
		AttemptToPurchase("IAPTrack2");
	}
	public void AttemptToPurchase_Track3() {
		gs.ac.PlaySFX(sfx_buttonPress);
		
		AttemptToPurchase("IAPTrack3");
	}
	public void AttemptToPurchase_Track4() {
		gs.ac.PlaySFX(sfx_buttonPress);
		
		AttemptToPurchase("IAPTrack4");
		
	}
	public void AttemptToPurchase_Track5() {
		gs.ac.PlaySFX(sfx_buttonPress);
		
		AttemptToPurchase("IAPTrack5");
	}

	public void AttemptToPurchase_TrackALL() {
		gs.ac.PlaySFX(sfx_buttonPress);
		
		AttemptToPurchase("IAPTrackALL");
	}

	public void AttemptToPurchase_ScoreMult025() {
		gs.ac.PlaySFX(sfx_buttonPress);
		
		AttemptToBuyWithCoins("IAPScoreMult025");

	}
	
	public void AttemptToPurchase_ScoreMult075() {
		gs.ac.PlaySFX(sfx_buttonPress);
		
		AttemptToBuyWithCoins("IAPScoreMult075");
	}


	public void AttemptToPurchase_4Coins() {
		gs.ac.PlaySFX(sfx_buttonPress);
		
		AttemptToPurchase("IAP4Coins");
	}
	public void AttemptToPurchase_15Coins() {
		gs.ac.PlaySFX(sfx_buttonPress);
		
		AttemptToPurchase("IAP15Coins");
	}
	public void AttemptToPurchase_50Coins() {
		gs.ac.PlaySFX(sfx_buttonPress);
		
		AttemptToPurchase("IAP50Coins");
	}


	void AttemptToPurchase(string id) {

		try {
			Debug.Log("attempt to purchase");
			
			StoreInventory.BuyItem (id);// if the purchases can be completed sucessfully
		

		} 
		catch (Exception e) 
		{																						// if the purchase cannot be completed trigger an error message connectivity issue, IAP doesnt exist on ItunesConnect, etc...)
			Debug.Log ("SOOMLA/UNITY" + e.Message);							
		}

	}


	public void RestorePurchases() {

		gs.ac.PlaySFX(sfx_buttonPress);

		try 
		{
			Debug.Log("Attempting to Restore");
			SoomlaStore.RestoreTransactions();													// restore purchases if possible
		} 
		catch (Exception e) 
		{
			Debug.Log ("SOOMLA/UNITY" + e.Message);												// if restoring purchases fails (connectivity issue, IAP doesnt exist on ItunesConnect, etc...)
		}


	}




	public void AttemptToBuyWithCoins(string id) {

		int price = 0;

		bool success = true;

		if (id == "IAPScoreMult025") {
			price = 4;
		}
		else if (id == "IAPScoreMult075") {
			price = 8;
		}
		else {
			success = false;
		}


		if (success) {

			gs.ac.PlaySFX(sfx_cash);

			if (gs.numCoins >= price) {

				gs.numCoins -= price;

				gs.Save();
			}

			if (id == "IAPScoreMult025") {
				PurchasedScoreMult025();
			}
			else if (id == "IAPScoreMult075") {
				PurchasedScoreMult075();
			}


			CheckPurchaseStatus();

		}

	}







	//What to do for each item that was successfully purchased.


	void PurchasedTrack2 () {
		gs.ac.UnlockTrack(1, false);
	}
	void PurchasedTrack3 () {
		gs.ac.UnlockTrack(2, false);
	}

	void PurchasedTrack4 () {
		gs.ac.UnlockTrack(3, false);
	}

	void PurchasedTrack5 () {
		gs.ac.UnlockTrack(4, false);
	}

	void PurchasedTrackALL () {
		PurchasedTrack2();
		PurchasedTrack3();
		PurchasedTrack4();
		PurchasedTrack5();
	}


	void Purchased4Coins () {
		gs.numCoins += 4;
		gs.Save();
		gs.level.SetCoins(4);
	}

	void Purchased15Coins() {
		gs.numCoins += 15;
		gs.Save();
		gs.level.SetCoins(15);
	}

	void Purchased50Coins() {
		gs.numCoins += 50;
		gs.Save();
		gs.level.SetCoins(50);
	}



	void PurchasedScoreMult025 () {
		gs.level.addMessage("-1 TOKENS", 2);
		gs.level.SetCoins(-1);
		gs.level.IncreaseBaseMult(0.25f, true);
	}
	
	void PurchasedScoreMult075 () {
		gs.level.addMessage("-2 TOKENS", 2);
		gs.level.SetCoins(-1);
		gs.level.IncreaseBaseMult(0.75f, true);
	}



	/// -------------










	//--------------------
	//--------------------
	//--------------------
	// ----------------------------  CALL BACK METHODS FROM SOOMLA STORE -----------------
	//--------------------
	//--------------------
	//--------------------

	public void onMarketPurchaseStarted(PurchasableVirtualItem pvi) {
		// pvi - the PurchasableVirtualItem whose purchase operation has just started
		
		// ... your game specific implementation here ...

		showLoadingScreenWithMessage("Purchasing Item");
	}


	public void onMarketPurchase(PurchasableVirtualItem pvi, string payload,
	                             Dictionary<string, string> extra) {
		// pvi - the PurchasableVirtualItem that was just purchased
		// payload - a text that you can give when you initiate the purchase operation and
		//    you want to receive back upon completion
		// extra - contains platform specific information about the market purchase
		//    Android: The "extra" dictionary will contain: 'token', 'orderId', 'originalJson', 'signature', 'userId'
		//    iOS: The "extra" dictionary will contain: 'receiptUrl', 'transactionIdentifier', 'receiptBase64', 'transactionDate', 'originalTransactionDate', 'originalTransactionIdentifier'
		
		// ... your game specific implementation here ...

		bool success = true; //assume success unless id not found.

		switch(pvi.ItemId) {
		case "IAPTrack2":
			PurchasedTrack2();
			break;
		case "IAPTrack3":
			PurchasedTrack3();
			break;
		case "IAPTrack4":
			PurchasedTrack4();
			break;
		case "IAPTrack5":
			PurchasedTrack5();
			break;
		case "IAPTrackALL":
			PurchasedTrackALL();
			break;
		case "IAP4Coins":
			Purchased4Coins();
			break;
		case "IAP15Coins":
			Purchased15Coins();
			break;
		case "IAP50Coins":
			Purchased50Coins();
			break;
		

		default:
			success = false;
			Debug.Log("ERROR _ NO ITEM WAS PURCHASED OR ID TRIED TO RETRIEVE WAS WRONG!!!! STORE CONTROLLER onMarketPurchase.");
			break;

		}

		if (success) gs.ac.PlaySFX(sfx_cash);

		
		CheckPurchaseStatus(); //after every purchase, recheck the purchases statuses to see if any are unlocked or full.
		
		hideLoadingScreen();
	}

	
	public void onMarketPurchaseCancelled(PurchasableVirtualItem pvi) {
		// pvi - the PurchasableVirtualItem whose purchase operation was cancelled
		
		// ... your game specific implementation here ...
		hideLoadingScreen();
	}

	
	public void onRestoreTransactionsStarted() {
		// ... your game specific implementation here ...

		showLoadingScreenWithMessage("Restoring Purchases");
	}


	
	public void onRestoreTransactionsFinished(bool success) {
		// success - true if the restore transactions operation has succeeded

		CheckPurchaseStatus(); //does all the work to make all the menus correct and set all other game values correctly.

		// ... your game specific implementation here ...
		hideLoadingScreen();
	}


/*
	public void onBillingSupported() {
		// ... your game specific implementation here ...
	}


	public void onBillingNotSupported() {
		// ... your game specific implementation here ...
		hideLoadingScreen();
	}
*/

	public void onUnexpectedStoreError(int errorCode) {
		// message - the description of the error
		
		// ... your game specific implementation here ...
		
		hideLoadingScreen();
	}


	
	
	




	void showLoadingScreenWithMessage(string m) {
		
		gs.soomlaBusy = true;
		
		Text message = loadingScreen.FindChild("Canvas").FindChild("LoadMessage").GetComponent<Text>();
		message.text = m;
		
		Vector3 p = loadingScreen.localPosition;
		p.x = 0;
		loadingScreen.localPosition = p;
		
	}
	
	void hideLoadingScreen() {
		
		gs.soomlaBusy = false;
		
		if (loadingScreen != null) {
			Vector3 p = loadingScreen.localPosition;
			p.x = 1500;
			loadingScreen.localPosition = p;
		}
	}




	public void OpenStore() {
		CheckPurchaseStatus();
	}


	public void CloseStore() {
		if (previewing) {
			PreviewTrack(storeItem_Previewing, track2, 0); //sending the same track to stop the previewing.
		}
	}



	public void PreviewTrack2() {
		PreviewTrack(storeItem_Track2, track2, 45f);
	}
	public void PreviewTrack3() {
		PreviewTrack(storeItem_Track3, track3, 114f);
	}
	public void PreviewTrack4() {
		PreviewTrack(storeItem_Track4, track4, 166.5f);
	}
	public void PreviewTrack5() {
		PreviewTrack(storeItem_Track5, track5, 16f);
	}

	void PreviewTrack(Transform track, AudioClip trackClip, float trackPos) {
		bool startNewTrack = true;
		if (previewing && storeItem_Previewing != null) { //if theres a store item being previewed. then make sure to cancel it.
			//if they press the same track button that is currently previewing.
			//stop the preview and do nothing else.
			if (storeItem_Previewing == track) {
				startNewTrack = false; //don't start another track.
			}
			LineRenderer line = storeItem_Previewing.FindChild("TrackPreviewMeter").GetComponent<LineRenderer>();
			Image button = storeItem_Previewing.FindChild("TrackPreviewPlay").GetComponent<Image>();
			EndPreview(line, button);
		}

		if (!startNewTrack) return; //break the method here so no new music plays.


		//STARTING THE NEXT PREVIEW TRACK THAT WAS PRESSED.

		gs.ac.PauseBGM(); ///pause the current background music.

		previewing = true;
		storeItem_Previewing = track;

		LineRenderer meter = storeItem_Previewing.FindChild("TrackPreviewMeter").GetComponent<LineRenderer>();
		meter.SetPosition(1, new Vector3(200,0,0));

		Image buttonImage = storeItem_Previewing.FindChild("TrackPreviewPlay").GetComponent<Image>();
		buttonImage.sprite = stop;

		previewPlayer.clip = trackClip;
		previewPlayer.time = trackPos;
		previewPlayer.Play();
		previewMeter = StartCoroutine(TrackPreviewMeter(meter, buttonImage));
	}

	IEnumerator TrackPreviewMeter(LineRenderer meter, Image buttonImage) {
		float time = 30f;
		float timer = time;
		while(true) {
			timer -= Time.deltaTime;

			meter.SetPosition(1, new Vector3(200f * (timer / time), 0,0));

			if (timer <= 0) break;
			yield return null;
		}
		EndPreview(meter, buttonImage);
	}

	void EndPreview(LineRenderer meter, Image buttonImage) {
		
		StopCoroutine(previewMeter);

		previewPlayer.Stop();
		previewing = false;
		meter.SetPosition(1, new Vector3(0f, 0, 0));
		buttonImage.sprite = play;
		storeItem_Previewing = null;

		//reset the background music and play from that.
		gs.ac.UnPauseBGM();
	}






	// Update is called once per frame
	void Update () {
	


		/// CHECK IN APP PURCHASE STATUS.
		/// 
		/// 
		/*
		if (StoreInventory.GetItemBalance("IAPNoAds") >= 1) 
		{
			gs.iap_NoAds = true;		// check if the non-consumable in app purchase has been bought or not
		}
		else {
			gs.iap_NoAds = false;
		}

		buttonNoAdsBuy.interactable = !gs.iap_NoAds;
		*/




	}
}
