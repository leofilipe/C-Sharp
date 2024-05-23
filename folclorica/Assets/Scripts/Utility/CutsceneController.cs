using UnityEngine;
using System.Collections;

public class CutsceneController : MonoBehaviour {

	public CameraFollowPath camPath;//the path the cam will follow

	public StoryBoard2Step storyBoard;	//the controller for the frames of the storyboard

	public string nextLevelToLoad = "Scene 01 - South Level";

	private float _fadeWait;		//the waiting time for the fade transitions

	private bool starting = true;	//flag that this instance is on its first update

	// Use this for initialization
	void Start () {
	
		//get the time for the fading
		_fadeWait = FadeInOut.GetInstance().fadeDuration;

		//disable the story board controller for now
		storyBoard.enabled = false;

		//disable the pause menu for now
		if(Pauser.instance != null)
			Pauser.instance.enabled = false;
	}
	
	// Update is called once per frame
	void LateUpdate () {
	
		if (starting){

			//set the camera on its starting position
			Camera.main.transform.position = camPath.Path.Points[0].position;
			Camera.main.orthographicSize = Mathf.Abs(Camera.main.transform.position.z);

			//set starting as false
			starting = false;

			//get the time for the fading
			_fadeWait = FadeInOut.GetInstance().fadeDuration;

		}
		//if the faiding time is greater than zero
		else if(_fadeWait > 0){

			//decrement the fading time
			_fadeWait -= Time.deltaTime;

			//if it the fading time has reached at least zero and 
			//the camera has finished its path
			if(_fadeWait <= 0 && camPath.hasFinished)
				//load the first level
				Application.LoadLevel(nextLevelToLoad);

		}
		//if the camera has finished its path
		else if (camPath.hasFinished){

			//start fading the scene out
			FadeInOut.GetInstance().FadeOut();

			//get the time for the fading
			_fadeWait = FadeInOut.GetInstance().fadeDuration;
		}
		//if the fading time has reached at least zero and the story board
		//is not enable, enable it
		else if (_fadeWait <= 0 && !storyBoard.enabled){
			storyBoard.enabled = true;
		}
	}
}
