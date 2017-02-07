using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using System.Linq;


public class BackgroundSlide : MonoBehaviour {

	GameState gs;

	GameObject arrow;

	//obstacles..
	#region
	[Header("Obstacles")]

	static List<GameObject> obstacleList = new List<GameObject>();
	static bool loadedObstacles = false;

	GameObject obstacle = null;
	#endregion


	//END OBSTACLES..






	//random colors..
	List<Color> colorList; 
	static float previousRandom = -1; //firstslide.


	#region
	[Header("CITY COLORS")]
	public Color colorTop_Sunset = Color.white;
	public Color colorBottom_Sunset = Color.white;
	#endregion

	#region
	[Header("OCEAN COLORS")]
	public Color colorTop_Beach = Color.white;
	public Color colorBottom_Beach = Color.white;
	#endregion

	#region
	[Header("DESERT COLORS")]
	public Color colorTop_Desert = Color.white;
	public Color colorBottom_Desert = Color.white;
	#endregion

	#region
	[Header("DESERT NIGHT COLORS")]
	public Color colorTop_DesertNight = Color.white;
	public Color colorBottom_DesertNight = Color.white;
	#endregion
	
	#region
	[Header("LAVA COLORS")]
	public Color colorTop_Lava = Color.white;
	public Color colorBottom_Lava = Color.white;
	#endregion

	#region
	[Header("DARKNESS COLORS")]
	public Color colorTop_Darkness = Color.white;
	public Color colorBottom_Darkness = Color.white;
	#endregion



	void Awake() {

		/// Load the obstacles.
		if (!loadedObstacles) {

			GameObject[] allObs = Resources.LoadAll<GameObject>("Prefabs/_OBSTACLES");

			obstacleList.Clear();

			obstacleList = allObs.ToList();
			//for (int i=0; i<allObs.Length; i++) {
			//	obstacleList.Add(allObs[i]);
			//}

			loadedObstacles = true;

		}

	}



	// Use this for initialization
	void Start () {
		gs = GameState.sharedGameState;

		switch(gs.cs) {
		case GameState.ColorStyles.normal:
			colorList = getRandomColors();
			break;
		case GameState.ColorStyles.sunset:
			colorList = getSunsetColors();
			break;
		case GameState.ColorStyles.beach:
			colorList = getBeachColors();
			break;
		case GameState.ColorStyles.desert:
			colorList = getDesertColors();
			break;
		case GameState.ColorStyles.desertNight:
			colorList = getDesertNightColors();
			break;
		case GameState.ColorStyles.lava:
			colorList = getLavaColors();
			break;
		case GameState.ColorStyles.darkness:
			colorList = getDarknessColors();
			break;
		default:
			colorList = getRandomColors();
			break;
		}

		Color TopColor = colorList[0];
		Color BottomColor = colorList[1];

		if (previousRandom != -1) {

			GradientBG quad = transform.FindChild("Quad").GetComponent<GradientBG>();
			quad.SetColors(BottomColor, TopColor);

		}

	}


	List<Color> getRandomColors() {
		List<Color> cList = new List<Color>();
		//start by choosing a random HUE value.
		float randH = 0;
		if (previousRandom == -1)
			randH = Random.Range(0f, 1f);
		else
			randH = previousRandom + 0.618033988749895f;
		if (randH > 1) randH -= 1;
		previousRandom = randH;

		float degH = randH * 360;

		short degDiff = 90;
		float randDegSecondH = (degH + degDiff) % 360; //1/4 of the color wheel away.
		float randSecondH = randDegSecondH/360; //turn back into 0..1 value.

		Color c1 = hsvToRgb(randH, .6f, .17f); ///////////TOP COLOR OF GRADIENT.
		Color c3 = hsvToRgb(randSecondH, .7f, .1f); /////BOTTOM COLOR OF GRADIENT.

		cList.Add(c1);
		cList.Add(c3);

		return cList;
	}

	List<Color> getSunsetColors() {
		previousRandom = 0;
		List<Color> cList = new List<Color>();

		Color c1 = colorTop_Sunset;
		Color c3 = colorBottom_Sunset;
		cList.Add(c1);
		cList.Add(c3);

		return cList;
	}

	List<Color> getBeachColors() {
		previousRandom = 0;
		List<Color> cList = new List<Color>();

		Color c1 = colorTop_Beach;
		Color c3 = colorBottom_Beach;
		cList.Add(c1);
		cList.Add(c3);
		
		return cList;
	}

	List<Color> getDesertColors() {
		previousRandom = 0;
		List<Color> cList = new List<Color>();
		
		Color c1 = colorTop_Desert;
		Color c3 = colorBottom_Desert;
		cList.Add(c1);
		cList.Add(c3);
		
		return cList;
	}

	List<Color> getDesertNightColors() {
		previousRandom = 0;
		List<Color> cList = new List<Color>();
		
		Color c1 = colorTop_DesertNight;
		Color c3 = colorBottom_DesertNight;
		cList.Add(c1);
		cList.Add(c3);
		
		return cList;
	}

	List<Color> getLavaColors() {
		previousRandom = 0;
		List<Color> cList = new List<Color>();

		Color c1 = colorTop_Lava;
		Color c3 = colorBottom_Lava;
		cList.Add(c1);
		cList.Add(c3);

		return cList;
	}



	List<Color> getDarknessColors() {
		previousRandom = 0;
		List<Color> cList = new List<Color>();
		
		Color c1 = colorTop_Darkness;
		Color c3 = colorBottom_Darkness;
		cList.Add(c1);
		cList.Add(c3);
		
		return cList;
	}


	//convert an HSV defined color to an RGB defined color....

	Color hsvToRgb(float h, float s, float v) {


		float h_i = Mathf.Floor(h*6);

		float f = h*6 - h_i;

		float p = v * (1 - s);

		float q = v * (1 - (f * s));

		float t = v * (1 - ((1 - f) * s));

		Color c = new Color(0,0,0,1);

		if (h_i <= 1) {
			c.r = v; c.g = t; c.b = p;
		}
		else if (h_i <= 2) {
			c.r = q; c.g = v; c.b = p;
		}
		else if (h_i <= 3) {
			c.r = p; c.g = v; c.b = t;
		}
		else if (h_i <= 4) {
			c.r = p; c.g = q; c.b = v;
		}
		else if (h_i <= 5) {
			c.r = t; c.g = p; c.b = v;
		}
		else if (h_i <= 6) {
			c.r = v; c.g = p; c.b = q;
		}


		return c;

	}












	//------ O B S T A C L E S ----



	//only gets called when not game over....
	public void ActivateObstacles(int level, int swipeDir) {


		Quaternion obRot = Quaternion.identity;
		Vector3 obPos = Vector3.zero;
		Vector3 obScale = Vector3.one;



		/*
		 * The difficulty algorithm
		 * Levels 0-4 No lasers. Just getting used to the controls, and having fun making correct swipes.
		 * Introduce percentage chance of having no lasers.
		 * Array of percents that decreases up until level 35.
		 * 
		 * if there is a laser do the following:
		 * 1.Convert Level to float. multiply by ratio of obstacle types to 80 levels.
		 * 2.Convert that answer to int.
		 * 3.Take that number +/- the range to get min and max.
		 * 4.Get a random between min and max.
		 * 5.As levels increase, so should the range.
		 */

 

		int percentChanceOfNoLasers = 62; //the highest possible chance of no lasers
		//an array of percentage chances for levels 5-34
		int[] percentages = {
			62, 55, 48, 41, 34,
			62, 51, 40, 29, 18,
			62, 47, 32, 17, 2,
			62, 43, 24, 5, 0
		};
		int levelToUse = level;
		if (level > 19) levelToUse = (14 + (level%5)); 
		/*lets us not go passed level 35 so that levelToUse-5 <=30.
		adding level%5 above means that no matter what level your at, in between two levels that are multiples of 5, it will know.
		for example lvl 336 % 5 = 1. 30 + 1 = 31. 31 -5 = 26. array pos 26 = 45%
		*/
		int percentagesArrayPos = levelToUse; 
		//minus 5 due to the first 5 levels not being included in this.
		//get the percentage chance from the array.
		percentChanceOfNoLasers = percentages[percentagesArrayPos];

		//create random between 1 and 100 and test against percentage chance.
		int randomChance = Random.Range(1,101);

		/*
		 * if true
		 * Do everything necessary to create obstacles for this level.
		 */
		if (randomChance > percentChanceOfNoLasers) {

			int numObstacles = obstacleList.Count;

			if (numObstacles > 1) {

				//the last obstacle is the size of the list - 1.
				int theLastObstacleInTheList = numObstacles-1;

				/*mult level * ratio
				 * ratio should be number of obstacles / number of levels
				 * number of levels being 80 since that's when spiritually the last boss is.
				 * 
				 */
				float obstacleToMaxLevelRatio = (float)numObstacles / 80.0f;


				/*
				 * Calculation of how to increase the range as the level increases.
				 * Create range multiplier, based on level/10 as well as ratio of objects to 80 levels.
				 * if multiplier is less than 1, set to 1. (or if final range is less than 3 set to 3).
				 */
				int startingRange = 3;

				float rangeMult = ((float)level/10.0f) * obstacleToMaxLevelRatio;
				if (rangeMult < 1.0f) rangeMult = 1.0f;
				Debug.Log("Range Mult = " + rangeMult);

				float rangeFloat = (float)startingRange * rangeMult;
				Debug.Log("Range Float = " + rangeFloat);

				int range = (int)rangeFloat;
				Debug.Log("Range = " + range);

				int rangeBack = range;
				int rangeForward = range;


				float currentPosFloat = (float)(level) * obstacleToMaxLevelRatio; // -5 for starting at level 5.
				Debug.Log("current pos float = " + currentPosFloat);
				//convert to int... add range.
				int currentPos = (int)currentPosFloat; // + range so that currentPos - range will be 0. at the first level. 
				Debug.Log("current pos = " + currentPos);

				if (currentPos > theLastObstacleInTheList)
					currentPos = theLastObstacleInTheList;


				int minObstacle = currentPos - rangeBack;
				if (minObstacle < 0) minObstacle = 0;

				int maxObstacle = currentPos + rangeForward;
				if (maxObstacle > theLastObstacleInTheList)
					maxObstacle = theLastObstacleInTheList;

				int randomObstacle = 0;



				while (true) {
					randomObstacle = Random.Range(minObstacle, maxObstacle); //pick random obstacle.
					Debug.Log("Obstcle chosen = " + randomObstacle);
					GameObject o = obstacleList[randomObstacle];

					if (o == null)
						continue;

					Obstacle_Container theObstacle = o.GetComponent<Obstacle_Container>();

					if (theObstacle.obstacleType == Obstacle_Container.obstacle_type.Up_Down) {
						if (swipeDir == 1 || swipeDir == 3)
							continue;
					}
					else if (theObstacle.obstacleType == Obstacle_Container.obstacle_type.Left_Right) {
						if (swipeDir == 0 || swipeDir == 2)
							continue;
					}


					obstacle = obstacleList[randomObstacle];
					break;

				}
			}
			else {
				if (obstacleList[0] != null)
					obstacle = obstacleList[0];
			}


			if (obstacle != null) {

				obstacle = Instantiate(obstacle) as GameObject;
				Obstacle_Container.obstacle_type type = obstacle.GetComponent<Obstacle_Container>().obstacleType;

				obstacle.transform.SetParent(transform);
				obRot = obstacle.transform.localRotation;

				obPos = obstacle.transform.localPosition;



				
				bool randFlipX = false;
				bool randFlipY = false;

				switch(type) {
				case Obstacle_Container.obstacle_type.Up_Down:
					if (swipeDir == 2) //down instead of up.
						obScale.y = -1;
					break;
				case Obstacle_Container.obstacle_type.Left_Right:
					if (swipeDir == 3) //left instead of right.
						obScale.x = -1;
					if (obstacle.gameObject.name == "Laser_LR_002(Clone)") {
						obPos.y += Random.Range(-3, 3);
					}
					break;
				case Obstacle_Container.obstacle_type.Left_Up:
					if (swipeDir == 0)
						randFlipX = true;
					else if (swipeDir == 1) {
						obScale.x = -1;
						randFlipY = true;
					}
					else if (swipeDir == 2) {
						obScale.y = -1;
						randFlipX = true;
					}
					else if (swipeDir == 3)
						randFlipY = true;
					break;
				case Obstacle_Container.obstacle_type.Down_Left_Up:
					if (swipeDir == 3)
						obScale.x = -1;
					randFlipY = true;
					break;
				case Obstacle_Container.obstacle_type.Left_Up_Right:
					if (swipeDir == 2)
						obScale.y = -1;
					randFlipX = true;
					break;
				case Obstacle_Container.obstacle_type.All_Sides: //original direction is 1.
					if (swipeDir == 0) {
						obRot = Quaternion.Euler(new Vector3(0,0,90));
					}
					else if (swipeDir == 3) {
						obRot = Quaternion.Euler(new Vector3(0,0,180));
					}
					else if (swipeDir == 2) {
						obRot = Quaternion.Euler(new Vector3(0,0,-90));
					}
					break;
				}

				if (randFlipX)
					if (Random.Range(1,3) % 2 == 0)
						obScale.x = -1;
				if (randFlipY)
					if (Random.Range(1,3) % 2 == 0)
						obScale.y = -1;

				//obPos.x *= obScale.x;
				//obPos.y *= obScale.y;

				obstacle.transform.localPosition = obPos;
				obstacle.transform.localRotation = obRot;
				obstacle.transform.localScale = obScale;
				obstacle.layer = LayerMask.NameToLayer("Obstacles");
			
			} //end if obstacle is not null.

		} // end if percent chance to show no lasers.


	}




	public void DeactivateObstacles(bool gameOver) {

		if (obstacle != null) {
			foreach(Transform o in obstacle.GetComponentsInChildren<Transform>()) {
				Obstacle ob = o.GetComponent<Obstacle>();
				if (ob != null) {
					ob.alive = false;

					if (!gameOver) { //don't send the objects to the background if it's game over.

						SpriteRenderer sprite = o.GetComponent<SpriteRenderer>();
						if (sprite != null) {
							o.GetComponent<SpriteRenderer>().sortingLayerName = "Slide";
						}



						foreach(Transform s in o.GetComponentInChildren<Transform>()) {
							SpriteRenderer sp = s.GetComponent<SpriteRenderer>();
							if (sp != null)
								s.GetComponent<SpriteRenderer>().sortingLayerName = "Slide";
						}
					}
				}
				else {
					Rotate rot = o.GetComponent<Rotate>();
					if (rot != null)
						rot.enabled = false;

					Move mov = o.GetComponent<Move>();
					if (mov != null)
						mov.enabled = false;
				}
			}
		}

	}


	public void ReactivateObstacles() {

		if (obstacle != null) {
			foreach(Transform o in obstacle.GetComponentsInChildren<Transform>()) {
				Obstacle ob = o.GetComponent<Obstacle>();
				if (ob != null) {
					ob.alive = true;

					SpriteRenderer sprite = o.GetComponent<SpriteRenderer>();
					if (sprite != null) {
						o.GetComponent<SpriteRenderer>().sortingLayerName = "Obstacles_Above_Arrow";
					}

					
					foreach(Transform s in o.GetComponentInChildren<Transform>()) {
						SpriteRenderer sp = s.GetComponent<SpriteRenderer>();
						if (sp != null)
							s.GetComponent<SpriteRenderer>().sortingLayerName = "Obstacles_Above_Arrow";
					}
				}
				else {
					Rotate rot = o.GetComponent<Rotate>();
					if (rot != null)
						rot.enabled = true;

					Move mov = o.GetComponent<Move>();
					if (mov != null)
						mov.enabled = true;
				}
			}
		}

	}
















	// Update is called once per frame
	void Update () {
		
	}






}
