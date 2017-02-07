using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class ScoreAdded : MonoBehaviour {

	GameState gs;

	Vector2 vel;
	float timer;
	float lifeTime = 0.7f;

	public Color textColor;
	public Color alternate;
	Text scoreText;
	float fontScale;
	float startingSize = 1;
	public RectTransform rect;



	bool grow = false;


	public bool mainScore = true;

	// Use this for initialization
	public void Start () {

		scoreText = GetComponent<Text>();
		textColor = scoreText.color;

		textColor.a = 0;

		scoreText.color = textColor;

		
		rect = GetComponent<RectTransform>();
		fontScale = rect.localScale.x;
		startingSize = fontScale;


		gs = GameState.sharedGameState;
		vel = new Vector2(50,50);

		//set random starting position and velocity direction.
		if (Random.Range(1,3) % 2 == 0)
			vel.x = -vel.x;
		if (Random.Range(1,3) % 2 == 0)
			vel.y = -vel.y;



	}


	public void SetScore(string s, int c) {
		SetScore(s, c, lifeTime); //default lifetime.
	}

	public void SetScore(string s, int c, float t) {
		//if c does not == 0, use the normal color. otherwise use the alternate color

		if (mainScore) {
			//Debug.Log("Create Score sent by object " + gameObject.name);
			GameObject score = Instantiate(gameObject) as GameObject;
			ScoreAdded script = score.GetComponent<ScoreAdded>();
			script.Start();
			script.mainScore = false;
			script.SetScore(s, c, t);


			score.transform.SetParent(transform.parent);

			script.rect.localScale = rect.localScale;

			Vector3 pos = rect.localPosition;
			pos.x += Random.Range(-25, 25);

			script.rect.localPosition = pos;
		}

		else {

			grow = true;
			timer = 0;
			lifeTime = t;


			textColor.a = 1;

			if (c==0) {
				scoreText.color = alternate;
			
			}
			else if (c==2) {
				scoreText.color = Color.yellow;
			}
			else {
				scoreText.color = textColor;
			}

			//enable the backlight.
			rect.FindChild("Light_Streak").GetComponent<SpriteRenderer>().color = new Color(scoreText.color.r,scoreText.color.g,scoreText.color.b,0.2f);
			rect.FindChild("Light_Streak").GetComponent<FadeInOut>().enabled = true;

			scoreText.text = s;
			fontScale = startingSize;
			rect.localScale = new Vector3(fontScale, fontScale);
		}
	}

	// Update is called once per frame
	void Update () {



		if (!mainScore) {

			
			textColor = scoreText.color;
			textColor.a = 1;
			scoreText.color = textColor;


			if (grow) {
				fontScale += 0.0075f;
				timer += Time.deltaTime;
				if (timer > lifeTime) {
					timer = 0;
					grow = false;
				}
			}

			else {
				if (textColor.a > 0) {
					fontScale -= 0.25f;
					if (fontScale < 0.1) { //the text is officially GONE!!!!.
						Destroy(gameObject);
					}
				}
			}

			//set the values back.

			if (gs.frameCount % 3 == 0) {
				rect.localScale = new Vector3(fontScale, fontScale);
				Vector3 pos = rect.localPosition;
				pos.y += 20f;
				rect.localPosition = pos;
			}

		}
	}
}
