using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.Linq;

public class LaserGrid : MonoBehaviour {


	GameObject _3dcam;

	GameObject container;

	#region
	[Header("Line Attributes")]
	public int numHLines = 50;
	public int numVLines = 20;
	public int lineDistance = 5;

	int startX = -40;

	public Color laserColor = new Color(0, 1,1, 0.5f);

	int totalLineCounter = 0;
	#endregion

	#region
	[Header("Hill Attributes")]
	public int percentChanceOfHill = 12;
	public int maxHillHeight = 7;
	int sinWaveHeight = 0;
	float sinTimer = 0;
	#endregion

	#region
	[Header("Line Attributes")]
	public float moveSpeed = 0.01f;
	float moveTimer = 0;
	float numberOfLinesMoved = 0;
	#endregion


	#region
	[Header ("Container Attributes")]
	public float startingRotation = 0;
	#endregion


	#region
	[Header ("Color Attributes")]

	//Color def
	public Color c_red = new Color(1,0,0,0.5f);
	public Color c_green = new Color(0,1,0, 0.5f);
	public Color c_blue = new Color(0,0,1, 0.5f);

	public Color c_cyan = new Color(0,1,1, 0.5f);
	public Color c_magenta = new Color(1,0,0.4f, 0.5f);
	public Color c_yellow = new Color(1,1,0, 0.5f);

	public Color c_purple = new Color(204f/255f, 46f/255f, 1f, 0.5f);
	public Color c_darkBlue = new Color(0, 0, 0.1f, 0.5f);
	public Color c_orange = new Color(1, 160f/255f, 0, 0.5f);

	public Color c_darkOrange = new Color(0.7f, 120f/255f, 0, 0.5f);

	public Color c_black = new Color(0,0,0, 0.5f);


	//CHECK LASER LINE CLASS BELOW THIS CLASS
	List<LaserLine> hLines = new List<LaserLine>();
	List<LaserLine> vLines = new List<LaserLine>();
	
	List<int[]> hillMap = new List<int[]>(); //each entry is a horizontal line of ints.
	
	GameObject initHLine;
	GameObject initVLine;

	#endregion





	// ----------------------- ======================= C O L O R   P A T T E R N S ============== ------------- 



	public enum cph {
		none,
		fadeInOut,
	}
	cph colorPatternHorizontal = cph.none;

	public enum cpv {
		none,
		fadeInOut
	}
	cpv colorPatternVertical = cpv.none;


	public void SetColorPatternHorizontal(cph pat, Color c) {
		ActivatePatternHorizontal(pat, c);
	}

	public void SetColorPatternVertical(cpv pat, Color c) {
		ActivatePatternVertical(pat, c);
	}




	//SETTING LINERENDER COLORS WITHOUT CHANGING THE COLOR VARIABLE OF LASERLINE CLASS, 
	//THUS HAVING A COLOR TO GO BACK TO WHEN CALLING UNOVERRIDe

	//--override colors.
	void OverrideColorHorizontal(int i, Color c) {
		if (i >= hLines.Count || i < 0) return;
		hLines[i].line.GetComponent<LineRenderer>().SetColors(c,c);
		hLines[i].colorOverride = true;
	}
	void UnOverrideColorHorizontal(int i) {
		if (i >= hLines.Count || i < 0) return;
		LaserLine l = hLines[i];
		l.line.GetComponent<LineRenderer>().SetColors(l.color, l.color);
		l.colorOverride = false;
	}

	void OverrideColorVertical(int i, Color c) {
		if (i >= vLines.Count || i < 0) return;
		vLines[i].line.GetComponent<LineRenderer>().SetColors(c,c);
		vLines[i].colorOverride = true;
	}
	void UnOverrideColorVertical(int i) {
		if (i >= vLines.Count || i < 0) return;
		LaserLine l = vLines[i];
		l.line.GetComponent<LineRenderer>().SetColors(l.color, l.color);
		l.colorOverride = false;
	}


	void OverrideAllColorHorizontal(Color c) {
		for (int i=0; i<numHLines; i++) {
			OverrideColorHorizontal(i, c);
		}
	}
	void UnOverrideAllColorHorizontal() {
		for (int i=0; i<numHLines; i++) {
			UnOverrideColorHorizontal(i);
		}
	}

	void OverrideAllColorVertical(Color c) {
		for (int i=0; i<numVLines; i++) {
			OverrideColorVertical(i, c);
		}
	}
	void UnOverrideAllColorVertical() {
		for (int i=0; i<numVLines; i++) {
			UnOverrideColorVertical(i);
		}
	}



	// S E T T I N G  N E W  C O L O R S ENTIRELY, including the color variable of laserline class 
	//if you want to preserve the original color variable, use override and unoverride.

	public void SetColorHorizontal(int i, Color c, bool setInit) {
		if (i >= hLines.Count || i < 0) return;
		
		LaserLine laser = hLines[i];

		laser.color = c;
		if (setInit) laser.initColor = c;
		LineRenderer line = laser.line.GetComponent<LineRenderer>();
		line.SetColors(c, c);

		
		hLines[i] = laser;
	}

	public void SetColorVertical(int i, Color c, bool setInit) {
		if (i >= vLines.Count || i < 0) return;
		
		LaserLine laser = vLines[i];

		laser.color = c;
		if (setInit) laser.initColor = c;
		LineRenderer line = laser.line.GetComponent<LineRenderer>();
		line.SetColors(c, c);

		vLines[i] = laser;
	}

	
	//--------------- SETT ALL LINES TO A SPECIFIC COLOR........	
	public void SetColorAllVertical(Color c, bool setInit) {
		if (vLines.Count > 0) {
			for (int i = 0; i < vLines.Count; i++)
				SetColorVertical(i, c, setInit);
		}
	}
	
	public void SetColorAllHorizontal(Color c, bool setInit) {
		if (hLines.Count > 0) {
			for (int i = 0; i < hLines.Count; i++)
				SetColorHorizontal(i, c, setInit);
		}
	}
	public void SetColorAll(Color c, bool setInit) {
		SetColorAllHorizontal(c, setInit);
		SetColorAllVertical(c, setInit);
	}
	
	

	void ResetColorAllHorizontal() {
		for (int i=0; i<hLines.Count; i++)
			SetColorHorizontal(i, hLines[i].initColor, true);
	}
	void ResetColorAllVertical() {
		for (int i=0; i<vLines.Count; i++)
			SetColorVertical(i, vLines[i].initColor, true);
	}
	
	
	
	//-------------------------------===============================





	// ----------------------- ======================= H E I G H T   A N D   M O V E   P A T T E R N S ============== ------------- 


	public enum HeightPattern {
		flat,
		normal,
		city,
		sine,
	}
	public HeightPattern php = HeightPattern.flat;
	public HeightPattern hp = HeightPattern.flat;

	public void SetHeightPattern(HeightPattern h) {
		php = hp;
		hp = h;
	}





	public void setMoveSpeed(float speed) {
		moveSpeed = speed;
	}






	//other movement variables.
	bool isSpinning = false;


	public void StartSpin(float time, bool clockwise, float finishSpeed) {
		if (!isSpinning) {
			if (gameObject.activeSelf) StartCoroutine(spin(time, clockwise, finishSpeed));
		}
	}

	IEnumerator spin(float time, bool clockwise, float finishSpeed) {

		setMoveSpeed(250);

		if (!isSpinning) {
			isSpinning = true;

			float timer = 0;

			float maxRotation = 1000f;

			if (container == null) yield return null; ///wait for next from when container should not be null.
			Rotate r = container.GetComponent<Rotate>();

			float accel = 10000*Time.deltaTime;

			r.clockWise = clockwise;

			//----------------------------- speed up.
			while(true) {
				timer += Time.deltaTime;

				r.degreesPerSec += accel;
				if (r.degreesPerSec > maxRotation) {
					r.degreesPerSec = maxRotation;
					timer = 0;
					break;
				}

				yield return null;
			}

			//--------------------- spin for time.
			while(true) {
				timer += Time.deltaTime;

				if (timer >= time) {
					timer = 0;
					break;
				}

				yield return null;
			}

			///---------------------- stop spinning.
			/// 
			Quaternion rot = container.transform.localRotation;
			rot = Quaternion.Euler(new Vector3(0, 0, startingRotation));
			container.transform.localRotation = rot;

			//------------------------- reset
			r.degreesPerSec = 0;
			r.clockWise = false;
			isSpinning = false;
		}

		setMoveSpeed(finishSpeed);

	}



	public void Reset(bool deactivate) {

		//return all values back to normal values.
		StopAllCoroutines();
		RemoveStaticTempLines();

		UnOverrideAllColorHorizontal();
		UnOverrideAllColorVertical();

		SetHeightPattern(HeightPattern.flat);
		setMoveSpeed(20);

		SetColorPatternHorizontal(cph.none, laserColor);
		SetColorPatternVertical(cpv.none, laserColor);

		SetColorAll(laserColor, true);


		if (deactivate) {
			//Debug.Log("deactivating " + gameObject);
			gameObject.SetActive(false);
		}

	}



	// Use this for initialization
	void Start () {

		_3dcam = GameObject.Find("3DCam");
		container = transform.parent.gameObject;


		startX = (numVLines/2) * -lineDistance;

		float lineWidth = .5f;

		initHLine = new GameObject();
		initHLine.AddComponent<LineRenderer>();
		initHLine.layer = LayerMask.NameToLayer("3DCam");

		LineRenderer lr = initHLine.GetComponent<LineRenderer>();
		lr.useWorldSpace = false;
		lr.SetVertexCount(numVLines);

		lr.SetWidth(lineWidth, lineWidth);
		//lr.material = new Material(Shader.Find("Sprites/Default"));
		lr.material = Resources.Load<Material>("MATERIALS+SHADERS/ParticleAdditive");
		lr.SetColors(laserColor, laserColor);

		lr.transform.position = new Vector3(-10000,-10000,-10000);



		initVLine = new GameObject();
		initVLine.AddComponent<LineRenderer>();
		initVLine.layer = LayerMask.NameToLayer("3DCam");
		
		LineRenderer lr2 = initVLine.GetComponent<LineRenderer>();
		lr2.useWorldSpace = false;
		lr2.SetVertexCount(numHLines);
		
		lr2.SetWidth(lineWidth, lineWidth);
		lr2.material = Resources.Load<Material>("MATERIALS+SHADERS/ParticleAdditive");
		lr2.SetColors(laserColor, laserColor);

		lr2.transform.position = new Vector3(-10000,-10000,-10000);




		GenerateHillMap();

		GenerateHorizontal();
		GenerateVertical();


		container.transform.localRotation = Quaternion.Euler(0,0,startingRotation);

	}



	void GenerateHillMap() {

		for (int i=0; i<numHLines; i++) {

			AddHillMapLine(i);
		}

	}
	void AddHillMapLine(int i) {

		int[] line = new int[numVLines];

		totalLineCounter++; //----------- add to total lines. can be used to know which numbered line you are.


		if (hp == HeightPattern.sine) {


			float yValue = sinTimer * moveSpeed;
			float y = Mathf.Sin (yValue);
			if (y >= -1 && y <= -0.8) { ///when the wave reaches the trough.
				sinWaveHeight = Random.Range(1, maxHillHeight*2); //create new random height for next wave.
				sinTimer = 0;
			}
			for (int j=0; j<numVLines; j++) {
				float hx = Mathf.Sin(((float)j + sinTimer)/4) * sinWaveHeight;
				float hy = Mathf.Sin(yValue) * sinWaveHeight;
				int h = (int)(Mathf.Sqrt(hx*hx + hy*hy));
				//h = (int)hy;
				//h *= randomHeight;//Random.Range(0, maxHillHeight);
				line[j] = h;
			}

		}

		else if (hp == HeightPattern.city) {

			int buildingDepth = 4;
			int buildingWidth = 4;
			int buildingSpace = 3;
			int buildingHeight = 100;


			bool depthOk = false;
			int totalDepth = buildingDepth + buildingSpace;
			int mod = totalLineCounter % totalDepth;
			if (mod <= buildingDepth) depthOk = true;


			for (int j=0; j<numVLines; j++) {

				bool widthOk = false;

				int widthCounter = j+1;
				int totalWidth = buildingWidth + buildingSpace;
				int mod2 = widthCounter % totalWidth;

				if (mod2 <= buildingDepth) {
					widthOk = true;
				}

				//------------------------------- IF THE NUMBER OF LINES ARE EVEN, THERE IS NO MIDDLE, so compensate by moving the left side in and right out.
				bool evenLines = numVLines % 2 == 0;
				int numLeft = 3;
				int numRight = numLeft;
				if (evenLines) {
					numLeft = 2;
					numRight = 4;
				}

				if (j < numVLines/2 + numRight && j > numVLines/2 - numLeft) { //space in the middle to fly down.
					widthOk = false;
				}

				int height = 0;
				if (widthOk && depthOk) {
					height = buildingHeight;
				}
				line[j] = height;
			}

		}

		else {

			int[] pLine = new int[numVLines];

			bool continuePreviousLine = false;
			if (i > 0) { //generate based on previous line.
				pLine = hillMap[i-1];
				continuePreviousLine = Random.Range(1,101) <= 50;
			} 

			for (int j=0; j<numVLines; j++) {
				if (continuePreviousLine) {
					line[j] = pLine[j];
				}
				else {
					//generate some new line.
					int prevPiece = 0;
					if (j-1 >= 0) prevPiece = line[j-1];

					if(prevPiece > 0 && Random.Range(1,101) <= 50) //continue piece.
						line[j] = prevPiece;
					else { ///----------------previous piece height was 0. make random Height.
						float percent = percentChanceOfHill;
						if (Random.Range(1,101) <= percent) { //whether to make hill.
							int h = 0;
							if (hp == HeightPattern.flat) {
								h = 0;
							}
							else if (hp == HeightPattern.normal) {
								h = Random.Range(1,maxHillHeight);
							}

							line[j] = h;
						}
					}
				}
			}

		}
		
		hillMap.Add(line);
	}



	//-------- I N I T I A L   G R I D   G E N E R A T I O N

	void GenerateHorizontal() {

		for (int i=0; i<numHLines; i++) {
			
			AddHorizontalLine(i); //add horizontalLine to end.
			
		}
	}

	void AddHorizontalLine(int i) {
		
		GameObject l = Instantiate(initHLine);
		
		LineRenderer line = l.GetComponent<LineRenderer>();
		line.transform.SetParent(transform);

		//----- MOVE THE LINE BACK BECAUSE THE ORIGINAL WAS MOVED 10000 Z INDEX OUT OF THE WAY
		Vector3 pos = line.transform.localPosition;
		pos.x += 10000;
		pos.y += 10000;
		pos.z += 10000;
		line.transform.localPosition = pos;

		bool lineAllFlat = true;
		for (int j=0; j<numVLines; j++) {
			int x = startX + (j * lineDistance);
			int y = hillMap[i][j];
			int z = (i * lineDistance);
			line.SetPosition(j, new Vector3(x, y, z));

			if (y > 0) lineAllFlat = false;
		}


		Color c = laserColor;
		
		LaserLine laserLine = new LaserLine();
		laserLine.color = c;
		laserLine.initColor = c;
		laserLine.completelyFlat = lineAllFlat;
		laserLine.line = l;
		
		hLines.Add(laserLine);
		
	}





	void GenerateVertical() {

		for (int i=0; i<numVLines; i++) {
			
			AddVerticalLine(i);
			
		}
	}

	void AddVerticalLine(int i) {

		GameObject l = Instantiate(initVLine);

		LineRenderer line = l.GetComponent<LineRenderer>();
		line.transform.SetParent(transform);


		Vector3 pos = line.transform.localPosition;
		pos.x += 10000;
		pos.y += 10000;
		pos.z += 10000;
		line.transform.localPosition = pos;

		bool lineAllFlat = true;
		for (int j=0; j < numHLines; j++) {
			int x = startX + (i * lineDistance);
			int y = hillMap[j][i];
			int z = ( j * lineDistance );
			line.SetPosition(j, new Vector3(x, y, z));

			if (y > 0) lineAllFlat = false;



		}

		int mid1 = numVLines/2;
		int mid2 = mid1+1;
		if (i == mid1 || i == mid2) {
			line.SetWidth(2,2);
		}


		Color c = laserColor;

		LaserLine laserLine = new LaserLine();
		laserLine.color = c;
		laserLine.initColor = c;
		laserLine.completelyFlat = lineAllFlat;
		laserLine.line = l;
		
		vLines.Add(laserLine);

	}


	// ------------------------------------- end grid generation






	//---------- G R I D  U P D A T E S



	void UpdateHorizontal() {


		for (int i=0; i<numHLines; i++) {

			LineRenderer line = hLines[i].line.GetComponent<LineRenderer>();

			bool allFlat = true;
			for (int j=0; j<numVLines; j++) {
				int x = startX + (j * lineDistance);
				int y = hillMap[i][j];
				int z = (i * lineDistance);
				line.SetPosition(j, new Vector3(x, y, z));

				if (y > 0) allFlat = false;
			}

			hLines[i].completelyFlat = allFlat;

		}

	}


	void UpdateVertical() {


		for (int i=0; i<numVLines; i++) {

			LineRenderer line = vLines[i].line.GetComponent<LineRenderer>();

			bool allFlat = true;
			for (int j=0; j<numHLines; j++) {
				int x = startX + (i * lineDistance);
				int y = hillMap[j][i];
				int z = ( j * lineDistance );
				line.SetPosition(j, new Vector3(x, y, z));

				if (y > 0) allFlat = false;
			}

			vLines[i].completelyFlat = allFlat;

		}

	}



	void RemoveLine() {

		transform.localPosition = new Vector3(0,-6.67f,0);

		hillMap.RemoveAt(0);

		AddHillMapLine(hillMap.Count);
	
		UpdateHorizontal();
		UpdateVertical();

	}



	void FixedUpdate() {

		if (moveSpeed > 0) {
			
			Vector3 pos = transform.localPosition;
			
			pos.z -= moveSpeed*Time.fixedDeltaTime;
			
			transform.localPosition = pos;
			
			
			moveTimer += Time.deltaTime;
			if (transform.localPosition.z <= -lineDistance) {
				moveTimer = 0;
				RemoveLine();
			}
			
		}

	}






	void Update() {


		// ------------------------------ WHAT TO DO DEPENDING ON DIFFERENT PATTERNS.

		//---- HEIGHT PATTERNS.


		if (hp == HeightPattern.sine) {
			//also add to the sin timer.....
			sinTimer += Time.deltaTime;
			container.transform.localPosition = new Vector3(3.75f, 4.67f + Mathf.Sin(Time.time)*10, -10);
		}
		else if (hp == HeightPattern.city) {
			for (int i=0; i<numHLines; i++) {
				LaserLine l = hLines[i];
				if (l.completelyFlat) {
					OverrideColorHorizontal(i, c_yellow);
				}
				else {
					UnOverrideColorHorizontal(i);
				}
			}

			int mid1 = numVLines/2; //the middle 2 vertical lines.
			int mid2 = mid1+1;

			for (int i=0; i<numVLines; i++) { // NOT WORKING .. makes all vert lines yellow.
				LaserLine l = vLines[i];
				if (l.completelyFlat) {
					if (i != mid1 && i != mid2) //======== dont let it be the middle 2 those should be white.
						OverrideColorVertical(i, c_yellow);
				}
				else  {
					UnOverrideColorVertical(i); //its not completely flat, unoverride the color.
				}
			}
			Color w = Color.white;
			OverrideColorVertical(mid1, w); //override the middle 2 lines to white.
			OverrideColorVertical(mid2, w);


		}
		else {
			container.transform.localPosition = new Vector3(3.75f, 6.67f, -10);

			UnOverrideAllColorHorizontal(); //go back to base colors and patterns.
			UnOverrideAllColorVertical();

		}


	}





	//-----------------------------------
	//-------------------------------------------    P A T T E R N S   F O R   T H E   C O L O R S ----------------------
	//-------------------------------------------
	//-------------------------------------------
	void ActivatePatternHorizontal(cph pat, Color c) {
		if (colorPatternHorizontal != pat) {
			colorPatternHorizontal = pat;
			switch(pat) {
			case cph.fadeInOut:
				if (gameObject.activeSelf) StartCoroutine(PATTERN_Horizontal_FadeInOut(c));
				break;

			default:
				break;
			}
		}
	}
	public void DeactivatePatternHorizontal(cph pat) {
		if (colorPatternHorizontal == pat) {
			colorPatternHorizontal = cph.none;
			switch(pat) {
			case cph.fadeInOut:
				if (gameObject.activeSelf) { 
					StopCoroutine(PATTERN_Horizontal_FadeInOut(c_cyan));
					ResetColorAllHorizontal();
				}
				break;

			default:
				break;
			}
		}
	}

	void ActivatePatternVertical(cpv pat, Color c) {
		if (colorPatternVertical != pat) {
			colorPatternVertical = pat;
			switch(pat) {
			case cpv.fadeInOut:
				if (gameObject.activeSelf) StartCoroutine(PATTERN_Veritcal_FadeInOut(c));
				break;

			default:
				break;
			}
		}
	}
	public void DeactivatePatternVertical(cpv pat) {
		if (colorPatternVertical == pat) {
			colorPatternVertical = cpv.none;
			switch(pat) {
			case cpv.fadeInOut:
				if (gameObject.activeSelf) {
					StopCoroutine(PATTERN_Veritcal_FadeInOut(c_cyan));
					ResetColorAllVertical();
				}
				break;
				
			default:
				break;
			}
		}
	}



	void PATTERN_Horizontal_Solid_Color(Color c) {
		for (int i=0; i<numHLines; i++) {
			LaserLine line = hLines[i];

			if (!line.colorOverride) SetColorHorizontal(i, c, false);
		}
	}
	void PATTERN_Vertical_Solid_Color(Color c) {
		for (int i=0; i<numVLines; i++) {
			LaserLine line = vLines[i];
			
			if (!line.colorOverride) SetColorVertical(i, c, false);
		}
	}





	//------------------------------------------- FADE IN AND OUT A SPECIFIC COLOR ALL THE LINES

	IEnumerator PATTERN_Horizontal_FadeInOut(Color c) {

		colorPatternHorizontal = cph.fadeInOut;
	
		while(colorPatternHorizontal == cph.fadeInOut) { ///keep running the coroutine forever, untile the color pattern changes.

			//--------------------------------------init setup
			Color initC = hLines[0].initColor;
			
			int iterations = 10;
			float it = (float)iterations;


			//----------------------------------- loop fade in then out.
			for (int i=1; i<=iterations; i++) {


				if (i<5) {
					float progress = (float)(i*2) / it;

					for (int j=0; j<numHLines; j++) {

						LaserLine l = hLines[j];

						Color newColor = Color.Lerp(l.color, c, progress);

						if (!l.colorOverride) SetColorHorizontal(j, newColor, false); //-- only if not color overrided by something else.

					}
				}
				else {
					float progress = (float)(i-(it/2)) / (it/2);

					for (int j=0; j<numHLines; j++) {

						LaserLine l = hLines[j];

						Color newColor = Color.Lerp(l.color, initC, progress);

						if (!l.colorOverride) SetColorHorizontal(j, newColor, false);

					}
				}

				yield return new WaitForSeconds(0.1f); //-- wait for .1 seconds then run the next iteration.
			}

		}

	}


	IEnumerator PATTERN_Veritcal_FadeInOut(Color c) {
		
		colorPatternVertical = cpv.fadeInOut;
		
		while(colorPatternVertical == cpv.fadeInOut) { ///keep running the coroutine forever, untile the color pattern changes.
			
			//--------------------------------------init setup
			Color initC = vLines[0].initColor;
			
			int iterations = 10;
			float it = (float)iterations;
			
			
			//----------------------------------- loop fade in then out.
			for (int i=1; i<=iterations; i++) {
				
				
				if (i<5) {
					float progress = (float)i / (it/2);
					
					for (int j=0; j<numVLines; j++) {
						
						LaserLine l = vLines[j];
						
						Color newColor = Color.Lerp(l.color, c, progress);
						
						if (!l.colorOverride) SetColorVertical(j, newColor, false); //-- only if not color overrided by something else.
						
					}
				}
				else {
					float progress = (float)(i-(it/2)) / (it/2);
					
					for (int j=0; j<numVLines; j++) {
						
						LaserLine l = vLines[j];
						
						Color newColor = Color.Lerp(l.color, initC, progress);
						
						if (!l.colorOverride) SetColorVertical(j, newColor, false);

					}
				}
				
				yield return new WaitForSeconds(0.1f); //-- wait for .1 seconds then run the next iteration.
			}
			
		}
		
	}



	//------------ ======================   GENERATING STATIC TEMPORARY LINES  ================= ---------------


	List<GameObject> stLines = new List<GameObject>();


	public void AddStaticTempLine(Color c, Vector3[] positions, float width) {

		GameObject line = Instantiate (initHLine) as GameObject;
		LineRenderer l = line.GetComponent<LineRenderer>();

		l.transform.SetParent(GameObject.Find("StaticTempLasers").transform);
		l.transform.localPosition = new Vector3(0,0,0);

		l.SetVertexCount(positions.Length);

		for (int i=0; i<positions.Length; i++) {
			l.SetPosition(i, positions[i]);
		}

		l.SetColors(c,c);

		l.SetWidth(width,width);

		stLines.Add(line);

	}

	public void RemoveStaticTempLines() { //all removed at once.

		foreach(GameObject line in stLines) {
			Destroy(line);
		}
		stLines.Clear();

	}
	

}










//------- LASER LINE CLASS

class LaserLine {
	public Color initColor; //starting color without patterns.
	public Color color;     //current color with patterns, but not including any overrided colors.
	public bool colorOverride = false;
	public GameObject line;
	
	public bool completelyFlat = false;
}






