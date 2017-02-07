using UnityEngine;
using System.Collections;


public class FadeInOut_Mesh : MonoBehaviour {
	
	/**
	 * LENGTH OF FADE OUT, AND FADE IN, BUT NOT BOTH
	 */
	[Range(0.001f, 6f)]
	public float period  = 2f;
	[Range(0f,1f)]
	public float min_opacity = 0f;
	[Range(0f,1f)]
	public float max_opacity = 1f;
	Color color;
	
	GameState gs;
	
	// Use this for initialization
	void Start () {
		
		gs = GameState.sharedGameState;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if (gs.frameCount % 3 == 0) {
			
			
			float range = max_opacity - min_opacity;
			float phase = Mathf.Sin(Time.time / period);
			
			float newA = (min_opacity + range/2) + (phase * (range/2));
			
			color.a = newA;
			//color = Color.red;
			
			foreach(Transform t in GetComponentsInChildren<Transform>() ) {

				Renderer r = t.GetComponent<Renderer>();
				if (r != null) {
					Color c = r.material.color;
					c.a = newA;
					r.material.SetColor("_Color", c);
				}
				
			}
		}
		
	}
}
