using UnityEngine;
using System.Collections;

public class TetraGen : MonoBehaviour {

	GameState gs;

	int tetraCounter = 0;
	int maxTetras = 3;

	float timer = 0;
	float time = 4;

	float powerUpTimer = 0;
	float powerUpTime = 45; //40 seconds offsetting the 12 second stay time of the green tetra.

	bool generateTetras = true;
	public bool generateGreen = false;//don't make green tetras. for now there will be no powerups.

	// Use this for initialization
	void Start () {
	
		gs = GameState.sharedGameState;

	}


	public void GenerateTetra(bool withSound) {

		//pick random x and y cooridinate.
		int x = (int)(Random.Range(100f,651f) / 100f);  //border of 100 pixels that the tetra cant be in.
		int y = (int)(Random.Range(100f, 1235f) / 100f);

		///now pick the starting z point. something far from the screen.
		int z = 50;

		GameObject t = Resources.Load<GameObject>("Prefabs/_COLLECTABLES/_tetra");
		GameObject tetra = Instantiate(t) as GameObject;
		tetra.transform.localPosition = new Vector3(x, y, z);

		tetra.GetComponent<Tetra>().soundEnabled = withSound;

	}

	public void GeneratePowerUpTetra() {

		generateGreen = false; //so that none get generated until this one is either gone or collected. 
		//gets set to true in the tetra class under kill,
		//gets set to true in levelcontroller class under collectedpowerupitem.

		//pick random x and y cooridinate.
		int x = (int)(Random.Range(100f,651f) / 100f);  //border of 100 pixels that the tetra cant be in.
		int y = (int)(Random.Range(100f, 1235f) / 100f);

		///now pick the starting z point. something far from the screen.
		int z = 50;

		GameObject t = Resources.Load<GameObject>("Prefabs/_COLLECTABLES/_tetra_green");
		GameObject tetra = Instantiate(t) as GameObject;
		tetra.transform.localPosition = new Vector3(x, y, z);
		
		tetra.GetComponent<Tetra>().soundEnabled = true; //just incase it gets false by default.
	}

	
	// Update is called once per frame
	void Update () {
	
		if (generateTetras && !(gs.level.ls == LevelController.levelState.paused)) { ///if should make tetras, and level not paused.
		
			timer += Time.deltaTime;

			if (timer >= time) {
				timer = 0;
				tetraCounter += 1;
				//if (tetraCounter >= maxTetras)
				//	generateTetras = false;

				//int numTetras = Random.Range(1,4); //generate 1 to n-1 tetras.
				int numTetras = 1;
				for (int i=0; i<numTetras; i++) {
					bool sound = true;
					if (i>0) sound = false; ///if there are multiple tetras dont let them play sound, otherwise the combined sound will be very loud.
					GenerateTetra(sound);
				}

				//generate new random time for the next set of tetras.
				time = Random.Range(2.75f, 5.25f);

			}


			//no power ups for now.
			/*
			if (gs.level.currentLevel >= 10) { //introduce powerups at level 10.
				if (generateGreen) {
					powerUpTimer += Time.deltaTime;

					if (powerUpTimer >= powerUpTime) {
						powerUpTimer = 0;

						GeneratePowerUpTetra();

					}
				}
			}
			*/

		}

	}
}
