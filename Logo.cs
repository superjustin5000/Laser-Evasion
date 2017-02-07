using UnityEngine;
using System.Collections;

public class Logo : MonoBehaviour {

	GameState gs;

	public bool animated = true;

	Animator anim;
	string anim_trigger_Fall1 = "Fall1";
	string anim_trigger_Fall2 = "Fall2";
	string anim_trigger_OFall1 = "OFall1";
	string anim_trigger_OFall2 = "OFall2";
	string anim_trigger_Bright = "Bright";
	string anim_STATIC = "Static";

	public AudioClip sfx_fall1;
	public AudioClip sfx_fall2;
	public AudioClip sfx_ofall;
	public AudioClip sfx_ofall2;


	bool didTouchScreen = false;


	// Use this for initialization
	void Start () {

		gs = GameState.sharedGameState;
	
		anim = GetComponent<Animator>();

		if (animated)
			StartCoroutine(animationControl());
		else
			Anim6();

	}


	IEnumerator animationControl() {
		yield return new WaitForSeconds(2.2f); //-- wait to start first anim.

		//transform.FindChild("Letter_O").GetComponent<FadeInOut>().enabled = true;
		//transform.FindChild("Word_Hell").GetComponent<FadeInOut>().enabled = true;
		//----------start sound
		transform.FindChild("NeonBuzz").GetComponent<NeonBuzz>().Begin();


		yield return new WaitForSeconds(1.5f); //-- wait to start first anim.
		anim.SetTrigger(anim_trigger_Fall1); //----------------------------------   start animation, and then start flickering, and then sound.
		//--play sound for initial fall.
		gs.ac.PlaySFX(sfx_fall1);

	}

	public void Anim2() {
		anim.SetTrigger(anim_trigger_Fall2);
		gs.ac.PlaySFX(sfx_fall2);
	}
	public void Anim3() {
		anim.SetTrigger(anim_trigger_OFall1);
		gs.ac.PlaySFX(sfx_ofall);
	}
	public void Anim4() {
		anim.SetTrigger(anim_trigger_OFall2);
		gs.ac.PlaySFX(sfx_ofall2);
	}
	public void Anim5() {
		anim.SetTrigger(anim_trigger_Bright);
	}

	public void Anim6() {
		anim.SetTrigger (anim_STATIC);
		transform.FindChild("Light_Streak").GetComponent<FadeInOut>().min_opacity = 0.2f;
		transform.FindChild("Light_Streak").GetComponent<FadeInOut>().max_opacity = 0.26f;
	}


	public void Steady() {
		transform.FindChild("NeonBuzz").GetComponent<NeonBuzz>().Steady();
		transform.FindChild("Word_Hell").GetComponent<FadeInOut>().dontFade = false;
		transform.FindChild("Word_Hell").GetComponent<FadeInOut>().FadeIn();
	}

	public void Brighten() {
		transform.FindChild("Light_Streak").GetComponent<FadeInOut>().FadeInTo(60f/255f);
	}

	public void Fade() {
		transform.FindChild("NeonBuzz").GetComponent<NeonBuzz>().Fade();
	}






	// Update is called once per frame CHECK FOR INTERRUPTION FROM TOUCH.
	void Update () {
		if (animated) {
			if (!didTouchScreen) {
				bool touched = false;
#if UNITY_IOS
				touched = Input.touchCount > 0;
#endif
#if UNITY_EDITOR
				touched = Input.GetMouseButtonDown(0);
#endif

				if (touched) {
					didTouchScreen = true; //stop polling for touch.

					//tell loader to stop waiting.
					GameObject.Find("Loader").GetComponent<Loader>().waitTime = 0;
				}
			}
		}
	}
}
