using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Switches between separete frames of a storyboard.
/// Assumes each frame as a image along the scene with a path connecting them.
/// If a same frame must have different ballon texts, each must be written on copies of the same image.
/// The "next" button will deactivate each copie on press and 
/// move to the next frame when the last of the copies is reached
/// </summary>
using System;


public class StoryBoard2Step : MonoBehaviour {


	public float frameMinWait = 3f;			//min time to wait at each frame before enabling the "next" button
	public CameraFollowPath camPath;		//the path the cam will follow
	public Button btNextFrame;				//the "next" button
	public List<PathStep> frames;			//the list of frames of the story board

	private PathStep _curFrame;				//the current in display frame of the story board
	private float _wait;					//the current waiting time
	private int frameIndex;					//the index of the current frame

	private bool isFirstFrame = true;		//flag for the first frame

	private bool canClick;

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () {

		//set the "next" button as non interactable
		btNextFrame.interactable = false;

		//set the current frame as the first one
		_curFrame = frames[0];

		//set the current waiting time as the default one
		_wait = Mathf.Max(frames[0].duration, frameMinWait);

		canClick = false;
		
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update(){

		//if the waiting time is greather than zero
		if(_wait >= 0){

			//decrement it
			_wait -= Time.deltaTime;

			//set the "next" button as non interactable
			btNextFrame.interactable = false;
		}
		//if it is not
		else
			//set the "next" button as interactable
			btNextFrame.interactable = true;

		//if the flag for enabling the click on the button is different from
		//the value of moveNow, then the cam has just stopped on a new path step.
		//Then, update the wait time for disabling the button
		if(canClick != camPath.moveNow){

			try{

				canClick = camPath.moveNow;
				_wait = Mathf.Max(frames[frameIndex].duration, frameMinWait);

			}catch(Exception ex){
				Debug.Log(ex.StackTrace);
				Debug.Log(frameIndex);
			}
		}
	}

	/// <summary>
	/// Move forward to the next frame
	/// </summary>
	public void NextFrame(){

		//set the "next" button as interactable
		btNextFrame.interactable = false;

		//if the camera is not moving along its path when the button is pressed
		if(!camPath.moveNow){

			//set the moving type as MoveTowards
			camPath.Type = FollowPath.FollowType.MoveTowards;

			//update the frame index
			frameIndex++;

			//if the frameIndex is within the bounds of the list
			if(frameIndex < frames.Count){

				//if the current frame in display has the same step as the next one
				if(_curFrame.step == frames[frameIndex].step){

					//only the text will differ, deactivate this frame to reveal the one underneath it
					_curFrame.gameObject.SetActive(false);

				}
				//if the current frame in display has not the same step as the next one
				else{

					//let the cam move to the next frame
					camPath.moveNow = true;

					//if this is the first frame
					if(isFirstFrame){

						//set the next point. OBS: doing so for other frames than 
						//the first results in skipping steps of the story board
						camPath.currentPoint.MoveNext();

						//flag the first frame as false
						isFirstFrame = false;
					}

				}

				//set the current waiting time to the default one
				_wait = Mathf.Max(frames[frameIndex].duration, frameMinWait);

				//update the current frame
				_curFrame = frames[frameIndex];

				//Debug.Log(camPath.currentPoint);
			}
			//if the frameIndex is not within the bounds of the list
			else{

				//the story board is concluding, let it always move
				camPath.moveNow = true;

				//set the waiting time to the max to avaid enabling the "next" button while the story board concludes
				_wait = float.MaxValue;
			}
		}
		//if the camera is moving along its path when the button is pressed
		else{

			//set the moving type as Lerp to go faster
			camPath.Type = FollowPath.FollowType.Lerp;

			//increase the speed to go even faster
			camPath.UpdateCurrentSpeed(10f);

			//set the current waiting time to the default one
			_wait = Mathf.Max(frames[frameIndex].duration, frameMinWait);;
		}

		//update the value for the clickable button
		canClick = camPath.moveNow;

	}


}
