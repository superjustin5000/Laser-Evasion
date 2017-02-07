using UnityEngine;
using System.Collections;

public class Obstacle_Container : MonoBehaviour {

	public enum obstacle_type {
		Up_Down,
		Left_Right,
		Left_Up,
		Down_Left_Up,
		Left_Up_Right,
		All_Sides
	};

	public obstacle_type obstacleType = obstacle_type.All_Sides;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
