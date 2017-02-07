using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine.UI;

public class AudioController : MonoBehaviour {

	GameState gs;

	private static bool created = false;

	AudioSource bgmChannel;
	//AudioSource[] sfxChannels = new AudioSource[3];
	AudioSource sfxChannel;

	public AudioClip[] bgm;
	int currentTrack;




#region
	[Header("WaveForm Capture Variables")]
	public int sampleSize = 2048;
	float [] samples;
	float [] spectrum;
	float sampleRate;
	float refValue = 0.1f; // RMS value for 0 dB
	float threshold = 0.02f; //min amplitude to extract pitch.
	[HideInInspector]
	public float dbValue;
	[HideInInspector]
	public float rmsValue;
	[HideInInspector]
	public float pitchValue;
#endregion


	LineRenderer _rms;
	LineRenderer _db;
	LineRenderer _pitch;
	GameObject visualizer;
	bool visEnabled = true;


	public bool isMute = false;


	void Awake() {

		if (!created) {

			gs = GameState.sharedGameState;
			gs.ac = this;

			DontDestroyOnLoad(this.gameObject);
			created = true;

		}

		else { ////created variable is static so every other game object that AWAKES with created = true gets destroyed.
			Destroy(this.gameObject);
		}


	}


	void Start() {

		//for (int i=0; i<sfxChannels.Length; i++) {
		//	sfxChannels[i] = gameObject.AddComponent<AudioSource>();
		//}
		sfxChannel = gameObject.AddComponent<AudioSource>();

		if (!gs.tracksUnlocked[0]) {
			gs.tracksUnlocked[0] = true;
			gs.Save();
		}



		bgmChannel = gameObject.AddComponent<AudioSource>();
		bgmChannel.loop = true;
		SetTrack(gs.currentTrackNum, false); //save track. everytime you change it saves.



		visualizer = transform.FindChild("Visualizer").gameObject;
		_rms = visualizer.transform.FindChild("RMS").GetComponent<LineRenderer>();
		_db = visualizer.transform.FindChild("dB").GetComponent<LineRenderer>();
		_pitch = visualizer.transform.FindChild("Pitch").GetComponent<LineRenderer>();
		
		ToggleVis(); //turn off to begin with.

	}




	public void PlaySFX(AudioClip clip, float volumeScale, float pitch) {

		//foreach (AudioSource a in sfxChannels) {

			//if (!a.isPlaying) {

				//Debug.Log("Playing sfx clip name = " + clip.name);
				//a.pitch = pitch;
				sfxChannel.PlayOneShot(clip, volumeScale);

				//break;

			//}

		//}

	}
	public void PlaySFX(AudioClip clip, float volumeScale) {
		
		PlaySFX(clip, volumeScale, 1.0f);
		
	}
	public void PlaySFX(AudioClip clip) {

		PlaySFX(clip, 1.0f, 1.0f);

	}



	public void PlayBGM() {
		bgmChannel.Play();
	}
	public void StopBGM() {
		bgmChannel.Stop();
	}
	IEnumerator FadeInTo(int track) {
		StopCoroutine("FadeInTo");

		float fadeSpeed = 0.05f;

		while (true) {
			bgmChannel.volume -= fadeSpeed;
			if (bgmChannel.volume <= 0) {
				bgmChannel.volume = 0;
				break;
			}
			yield return null;
		}
		StopBGM();
		bgmChannel.clip = bgm[track];
		PlayBGM();
		while (true) {
			bgmChannel.volume += fadeSpeed;
			if (bgmChannel.volume >= 1) {
				bgmChannel.volume = 1;
				break;
			}
			yield return null;
		}
	}
	IEnumerator FadeIn() {
		while (true) {
		}
	}
	public void PauseBGM() {
		bgmChannel.Pause();
	}
	public void UnPauseBGM() {
		bgmChannel.UnPause();
	}

	public void PlayBGMAtPos(int pos) {
		StopBGM();
		bgmChannel.time = 185;
		PlayBGM();
	}

	public void SetTrack(int track, bool fade) {
		//check if unlocked
		if (!gs.tracksUnlocked[track]) { //check previous track if this one is not unlocked.
			SetTrack (track - 1, fade); //will call recursively until track found. At least the first one should be true by default.
			return;
		}

		currentTrack = track;

		if (fade) {
			StartCoroutine(FadeInTo(track));
		}
		else {
			StopBGM();
			bgmChannel.clip = bgm[track];
			PlayBGM();
		}

		if (track != gs.currentTrackNum) { //---- tell gamestate the current track number to save.
			gs.currentTrackNum = track;
			gs.Save();
		}

		SetTrackName(); //--set the name in the menu.
	}


	public void NextTrack() {
		int size = bgm.Length;
		int nextTrack = (currentTrack + 1) % size;

		//find the next unlocked track.
		for (int i=nextTrack; i<size; i++) {
			if (gs.tracksUnlocked[i]) { //----- found unlocked track. set it and break.
				nextTrack = i;
				break;
			}
			if (i == size-1) { //if the last track is locked. set the next track to the first track.
				nextTrack = 0;
			}
		}

		SetTrack(nextTrack, false);
	}

	public void SetTrackName() {
		GameObject track = GameObject.Find("TrackNameButton");
		if (track != null)
			track.GetComponent<Text>().text = "Track: " + (currentTrack+1).ToString() + " - " + bgmChannel.clip.name.ToUpper();
	}




	public void UnlockNextTrack(bool play) {

		for (int i=0; i<bgm.Length; i++) {
			if (!gs.tracksUnlocked[i]) { //--- loop until you find a track not unlocked.
				UnlockTrack(i, play); //call unlock.
				break; //----found the track to unlock.
			}
		} //--- if no locked tracks are found.. do nothing.

	}

	public void UnlockTrack(int track, bool play) {

		if (track >= bgm.Length || track >= gs.tracksUnlocked.Length) //--check bounds.
			return;

		if(gs.tracksUnlocked[track]) //--already unlocked.
			return;

		gs.tracksUnlocked[track] =  true;
		gs.Save();

		if (play)
			SetTrack(track, true); //true makes the song fade in and out.

	}




	public void toggleMute() {

		if (!isMute) {
			/*
			foreach (AudioSource a in sfxChannels) {
				a.mute = true;
			}
			*/
			sfxChannel.mute = true;
			bgmChannel.mute = true;
	
		}

		else {
			/*
			foreach (AudioSource a in sfxChannels) {
				a.mute = false;
			}
			*/
			sfxChannel.mute = false;
			bgmChannel.mute = false;


		}

		isMute = !isMute;

	}





	void Update() {

		samples = new float[sampleSize];

		bgmChannel.GetOutputData(samples, 0);
		float sum = 0;
		for (int i=0; i<sampleSize; i++) {
			sum+= (samples[i] * samples[i]);
		}

		rmsValue = Mathf.Sqrt(sum / sampleSize);     // rms = square root of average of samples squared.
		dbValue = 20 * Mathf.Log10(rmsValue/refValue); //calculate dB.
		if (dbValue < -160) dbValue = 160; //clamp to -160dB.

		//get sound spectrum.
		spectrum = new float[sampleSize];

		bgmChannel.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
		float maxV = 0;
		int maxN = 0;
		for (int i=0; i<sampleSize; i++) { //find max
			if (spectrum[i] > maxV && spectrum[i] > threshold) {
				maxV = spectrum[i];
				maxN = i; //maxN is index of max.
			}
		}

		float freqN = maxN; //pass the index to a float.
		if (maxN > 0 && maxN < sampleSize-1) { //interpolate index using neighbors
			float dL = spectrum[maxN-1]/spectrum[maxN];
			float dR = spectrum[maxN+1]/spectrum[maxN];
			freqN += 0.5f * (dR * dR - dL * dL);
		}
		sampleRate = AudioSettings.outputSampleRate;
		pitchValue = freqN * (sampleRate / 2) / sampleSize; //convert index to frequency.


		if (visEnabled) {
			_rms.SetPosition(1, new Vector3(0, rmsValue * 2, 1));
			_db.SetPosition(1, new Vector3(0, dbValue/10, 1));
			_pitch.SetPosition(1, new Vector3(0, pitchValue/50, 1));
		}

	}



	void ToggleVis() {
		visEnabled = !visEnabled;
		visualizer.SetActive(visEnabled);
	}



}
