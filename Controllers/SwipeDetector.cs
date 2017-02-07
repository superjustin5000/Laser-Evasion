using UnityEngine;
using System.Collections;

public class SwipeDetector : MonoBehaviour 
{
	
	public float minSwipeDistY;
	
	public float minSwipeDistX;
	
	private Vector2 startPos;
	


	GameState gs;


	void Start() {
		gs = GameState.sharedGameState;
	
	}

	
	void Update()
	{

		bool backgroundMoving = false;
		if (!gs.inMenu) {
			backgroundMoving = gs.level.backgroundMoving;
		}

		if (!backgroundMoving) {



#if (UNITY_ANDROID || UNITY_IOS)

			if (Input.touchCount > 0) 
				
			{
				
				Touch touch = Input.touches[0];
				
				switch (touch.phase) 
					
				{
				case TouchPhase.Began:
					gs.level.TouchStarted(touch);
					break;


				case TouchPhase.Moved:
					gs.level.TouchMoved(touch);
					break;
					
					
					
				case TouchPhase.Ended:
					gs.level.TouchEnded(touch);
					break;
				}
			} ///////------ end touch count > 1.


#endif



		}




	} /////////// ----- end update..





	void OnGUI() {

		bool backgroundMoving = false;
		backgroundMoving = gs.level.backgroundMoving;
		
		if (!backgroundMoving) {

#if UNITY_EDITOR
			if (Input.GetKeyDown(KeyCode.UpArrow)) {
				SwipeDetected(0);
			}
			else if (Input.GetKeyDown(KeyCode.DownArrow)) {
				SwipeDetected(2);
			}
			else if (Input.GetKeyDown(KeyCode.LeftArrow)) {
				SwipeDetected(3);
			}
			else if (Input.GetKeyDown(KeyCode.RightArrow)) {
				SwipeDetected(1);
			}
			
			
			bool mouseDown = Input.GetMouseButtonDown(0);
			bool mouseMoved = Input.GetMouseButton(0);
			bool mouseUp = Input.GetMouseButtonUp(0);
			
			Event e = Event.current;
			if (mouseDown) {
				gs.level.MouseBegan(e);
			}
			else if (mouseMoved) {
				gs.level.MouseMoved(e);
			}
			else if (mouseUp) {
				gs.level.MouseEnded(e);
			}
		
#endif

		}


	}




	void SwipeDetected(int direction) {
		gs.level.SwipeDetected(direction);
	}

}
