using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {

	protected GameState gs;
	protected bool canCollide = true;
	public bool alive = true;

	#region
	[Header("OBSTACLE SETTINGS")]
	public string killText = "";
	public string hintText = "";
	#endregion

	GameObject spark;

	//FOR SUBCLASSES BE SURE TO CALL THE PARENT START METHOD...

	protected void Start () {
		gs = GameState.sharedGameState;
	}
	
	// Update is called once per frame
	void Update () {
	
	}




	virtual public void Hit() {
	}


	void OnCollisionEnter2D(Collision2D coll) {
		//Debug.Log("CALLED");
		if (alive) {
			if (canCollide) {
				alive = false;

				string text = killText;
				if (Random.Range(1,3) % 2 == 0)
					text = hintText;
				
				gs.level.HitObstacle(text);
				
				Hit();
				//create spark at coll position
				spark = Instantiate(Resources.Load("Prefabs/_Collision_Laser_Spark")) as GameObject;
				spark.transform.SetParent(transform);
				spark.transform.position = coll.contacts[0].point;

			}
		}
	}
	void OnCollisionStay2D(Collision2D coll) {
		if (alive) {
			if (canCollide) {
				OnCollisionEnter2D(coll);
			}
		}
	}


	void OnCollisionExit2D(Collision2D coll) {
		if (spark != null) {
			Destroy(spark);
		}

	}

}
