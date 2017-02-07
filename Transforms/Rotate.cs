using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {

	GameState gs;

	[Range(0, 1440)]
	public float degreesPerSec = 360;
	public bool clockWise = false;
	public bool randomizeDirection = false;

	[Range(0,4)]
	public float rotOscPer = 0;
	[Range(0,360)]
	public float rotOscAngle = 0;

	[Range(1,10)]
	public int frameSkip = 1;

	public bool xAxis = false;
	public bool yAxis = false;
	public bool zAxis = true;



	float timer = 0;


	// Use this for initialization
	void Start () {
		gs = GameState.sharedGameState;

		if (randomizeDirection) {
			clockWise = (Random.Range(1,3) % 2 == 0);
		}
	}




	public void SetNoRotation(bool resetToZero) {

		rotOscAngle = 0;

		if (resetToZero)
			StartCoroutine(goToAngleZero());

	}

	IEnumerator goToAngleZero() {    /////// Swiftly rotates the object back to a 0 angle.

		float move = 0.1f;
		float dist = move*2;

		Vector3 rot = transform.localRotation.eulerAngles;

		while (true) {

			rot = transform.localRotation.eulerAngles;
			
			bool left = (rot.z >= (360-dist) && rot.z <= 360); //condition of being relatively close to 0 based on move speed.
			bool right = rot.z <= dist && rot.z >= 0;

			if (rot.z > 180 && rot.z <= 360) /// if the object is not close to 0 rotate it.
				rot.z += move;
			else if (rot.z >= 0 && rot.z <= 180)
				rot.z -= move;
			
			
			transform.localRotation = Quaternion.Euler(rot);

			if (left || right) break;

			yield return null;
		}

		rot = transform.localRotation.eulerAngles;
		rot.z = 0;
		transform.localRotation = Quaternion.Euler(rot);
		//Debug.Log("stop coroutine");
	}



	// Update is called once per frame
	void Update () {

		timer += Time.deltaTime;


		if (gs.frameCount % frameSkip == 0) {

			if (rotOscPer == 0) {
				if (degreesPerSec > 0) {
					float rotAmount = degreesPerSec * Time.deltaTime;
					if (clockWise)
						rotAmount = -rotAmount;
					float curRotZ = transform.localRotation.eulerAngles.z + rotAmount;
					float curRotX = transform.localRotation.eulerAngles.x + rotAmount;
					float curRotY = transform.localRotation.eulerAngles.y + rotAmount;
					if (!zAxis) curRotZ -= rotAmount;
					if (!xAxis) curRotX -= rotAmount;
					if (!yAxis) curRotY -= rotAmount;
					transform.localRotation = Quaternion.Euler(new Vector3(curRotX,curRotY,curRotZ));
				}
			}
			else {
				if (rotOscAngle > 0) {
					float phase = Mathf.Sin(timer / rotOscPer);
					transform.localRotation = Quaternion.Euler( new Vector3(0, 0, phase * rotOscAngle));
				}
			}

		}
	}
}
