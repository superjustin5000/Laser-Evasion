using UnityEngine;
using System.Collections;

using UnityEngine.UI;

[RequireComponent(typeof(Image))]

public class FadeInOutUIIMAGE : MonoBehaviour {
	
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
	
	public bool dontFade = false;
	
	
	GameState gs;
	
	// Use this for initialization
	void Start () {
		
		gs = GameState.sharedGameState;
		
		
		color = GetComponent<Image>().color;
		color.a = min_opacity;
		GetComponent<Image>().color = color;
		
	}
	
	
	public void FadeInTo(float alpha0To1) {
		StartCoroutine(_FadeIn(alpha0To1));
	}
	public void FadeIn() {
		StartCoroutine(_FadeIn(1f));
	}
	IEnumerator _FadeIn(float a) {
		color.a = 0.1f;
		GetComponent<Image>().color = color;
		
		while(true) {
			min_opacity += 0.01f;
			max_opacity += 0.01f;
			
			bool min = min_opacity >= a;
			bool max = max_opacity >= a;
			
			if (min) min_opacity = a;
			if (max) max_opacity = a;
			
			if (min && max) break;
			
			yield return null;
		}
	}
	
	
	
	// Update is called once per frame
	void Update () {
		
		if (!dontFade) {
			//if (gs.frameCount % 3 == 0) {
			
			float range = max_opacity - min_opacity;
			float phase = Mathf.Sin(Time.time / period);
			
			float newA = (min_opacity + range/2) + (phase * (range/2));
			
			color.a = newA;
			
			GetComponent<Image>().color = color;
			
			foreach(Transform t in GetComponentsInChildren<Transform>() ) {
				Image s = t.GetComponent<Image>();
				if (s != null) {
					Color c = s.color;
					c.a = newA;
					s.color = c;			
				}
			}
			//}
		}
		
	}
}

