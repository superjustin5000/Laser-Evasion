using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	Vector3 pos;
	
	GameState gs;
	

	float currentSize;
	float targetSize;
	float originalSize;
	#region
	[Header("Zoom Settings")]
	public float zoomInSizeBigArea;
	public float zoomInSizeSmallArea;
	public float zoomAccel;
	bool shouldZoom;
	#endregion

	#region
	[Header("Swing Settings")]
	public bool shouldSwing = false;
	float startTime = 0;
	public float _Period = 2;
	public float _Angle = 2;
	#endregion

	//shake
	Vector3 originalCameraPosition;
	float shakeAmt = 0;




	
	void Awake() {

		#if (UNITY_IOS)
		System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
		#endif

		//Debug.Log("camera awake first");
		///the camera will be the first thing to call game state.


		gs = GameState.sharedGameState;
		//Debug.Log("only 1 gamestate above");

		gs.mainCam = GetComponent<MainCamera> ();


		
		///camera initializes sizing variables in game state.
		/// 
		float orthoSize = GetComponent<Camera> ().orthographicSize;
		
		gs.winHeight = orthoSize * 2f;
		gs.winWidth = gs.winHeight * (9.0f / 16.0f);
		
		originalSize = orthoSize;
		targetSize = originalSize;

		
	}
	
	// Use this for initialization
	void Start () 
	{
		
		pos = transform.position;
		originalCameraPosition = pos;
		
	}
	





	
	public void zoomInBigArea() {
		zoomIn (zoomInSizeBigArea);
	}
	
	void zoomIn(float toSize) {
		if (zoomAccel > 0) zoomAccel *= -1;
		targetSize = toSize;
		shouldZoom = true;
	}
	
	public void zoomInitial() {
		if (zoomAccel < 0) zoomAccel *= -1;
		targetSize = originalSize;
		shouldZoom = true;
	}





	public void shakeWithAmount(float amt) {

		shakeAmt = amt;
		InvokeRepeating("CameraShake", 0, .01f);
		Invoke("StopShaking", 0.3f);

	}


	void CameraShake()
	{
		if(shakeAmt>0) 
		{
			float quakeAmt = Random.value*shakeAmt*2 - shakeAmt;
			Vector3 pp = transform.position;
			pp.y+= quakeAmt; // can also add to x and/or z
			transform.position = pp;
		}
	}


	void StopShaking()
	{
		CancelInvoke("CameraShake");
		transform.position = originalCameraPosition;
	}




	
	
	// Update is called once per frame
	void Update () {

		//Debug.Log("Camera width = " + gs.winWidth + ", and height = " + gs.winHeight);
		//Debug.Log("Real   width = " + Screen.width + ", and height = " + Screen.height);

		//update the gamestate frame counter.
		gs.frameCount = (short)((gs.frameCount + 1) % 100); ///100 is a good maximum.



		///cam component.
		Camera cam = GetComponent<Camera> ();
		
		
		pos = transform.position;
		

		
		
		////camera zooming....hopefully works.
		currentSize = cam.orthographicSize;
		
		
		if (shouldZoom) {
			if (zoomAccel < 0) {
				currentSize += zoomAccel;
				if (currentSize < targetSize) {
					currentSize = targetSize;
					shouldZoom = false;
				}
			}
			else if (zoomAccel > 0) {
				currentSize += zoomAccel;
				if (currentSize > targetSize) {
					currentSize = targetSize;
					shouldZoom = false;
				}
			}
		}
		
		


		if (shouldSwing) {
			//only set every 5th frame.
			if (gs.frameCount % 5 == 0) {
				float phase = Mathf.Sin((Time.time - startTime) / _Period);
				transform.localRotation = Quaternion.Euler( new Vector3(0, 0, phase * _Angle));
			}
		}
		else {
			startTime = Time.time;
		}



		cam.orthographicSize = currentSize;
		transform.position = pos;
		
	}
	
}