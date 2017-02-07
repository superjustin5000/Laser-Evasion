using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class NeonBuzz : MonoBehaviour {

	AudioSource sound;

	float timer = 0;
	float time = 0.5f;

	bool begin = false;
	bool steady = false;
	bool fading = false;

	public Transform[] flickerSprites;

	// Use this for initialization
	void Start () {
		sound = GetComponent<AudioSource>();
		//sound.Play();
	}

	public void Begin() {
		begin = true;
		sound.Play ();
	}

	public void Steady() {
		steady = true;
	}

	public void Fade() {
		fading = true;
	}

	// Update is called once per frame
	void Update () {
	
		if (begin) {

			if (fading) {

				sound.volume -= 0.01f;

			}


			if (steady) {

				if (!sound.isPlaying) {
					sound.Play();
				}

			}

			else {

				timer += Time.deltaTime;
				if (timer >= time) {

					timer = 0;
					time = Random.Range(0.0025f, 0.2f);

					if (sound.isPlaying) {
						sound.Pause();
						foreach (Transform t in flickerSprites) {
							SpriteRenderer s = t.GetComponent<SpriteRenderer>();
							if (s != null) {
								Color c = s.color;
								c.a = 0.4f;
								s.color = c;	
							}
						}
					}
					else {
						sound.UnPause();
						foreach (Transform t in flickerSprites) {
							SpriteRenderer s = t.GetComponent<SpriteRenderer>();
							if (s != null) {
								Color c = s.color;
								c.a = 1f;
								s.color = c;	
							}
						}
					}
				}

			}

		}

	}
}
