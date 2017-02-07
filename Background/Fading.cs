using UnityEngine;
using System.Collections;

public class Fading : MonoBehaviour {

	public Texture2D fadeText;
	float fadeSpeed = 0.1f;
	//float fadeDuration = 0.5f;

	int drawDepth = -1000;
	float alpha = 1.0f;
	int fadeDir = -1;


	void OnGUI() {

		alpha += fadeDir * fadeSpeed;
		alpha = Mathf.Clamp01(alpha);


		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
		GUI.depth = drawDepth;
		GUI.DrawTexture(new Rect(0,0,Screen.width,Screen.height), fadeText);



	}


	public float BeginFade(int direction) {

		fadeDir = direction;
		return(1.0f/fadeSpeed);

	}


	void OnLevelWasLoaded() {

		BeginFade(-1);
	}
}
