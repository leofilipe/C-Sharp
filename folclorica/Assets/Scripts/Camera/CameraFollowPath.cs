using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Camera follow path extends to FollowPath.
/// OBS: To ensure it works properly with StoryBoard2Step and
/// CutsceneController set the first and last Points of 
/// PathDefinition Path as the same game object
/// </summary>
public class CameraFollowPath : FollowPath {
	
	public float[] pathSpeed;			//speed of the camera for each step of the path. To ensure that 
										//the camera do not loop, set the first and last values as zero

	private int _indexSpeed;			//index of the current speed

	private bool _isStart = true;		//flag indicating it just started

	public bool hasFinished {get; private set;}	//flag to whether or not the path has been concluded


	// Use this for initialization
	new void Start () {
	
		//set the speed to the first one
		Speed = pathSpeed[0];

		//call the parent method to do the other initializations
		base.Start();

		//set moveNow as false to halt the camera's movement
		moveNow = false;

		//set hasFinished as false
		hasFinished = false;
	}
	
	// Update is called once per frame
	new void Update () {

		//if there is no current point, do nothing
		if(currentPoint == null)
			return;
		//if there is no Current set for current point, do nothing
		if(currentPoint.Current == null)
			return;
	
		//if it the camera has not concluded its path and it is moving
		if(!hasFinished && moveNow){

			//get the index of the current speed
			_indexSpeed = Array.IndexOf (Path.Points, currentPoint.Current);

			//get the current speed
			Speed = pathSpeed[_indexSpeed];

			//if the z axis is not zero, use its absolute value
			if(transform.position.z != 0)
				Camera.main.orthographicSize = Mathf.Abs(transform.position.z);
			else
				//otherwise use 10 as default
				Camera.main.orthographicSize = 10;

			//call the movement methods
			WalkStep();
			CheckNextStep();

			//if it is on the target and it is not the first time its moving
			if(onTarget && !_isStart){

				//stop the camera movement. StoryBoard2Step will decide 
				//when to turn it on again through the "next" button
				moveNow = false;
			}

			//if the current step of the path is the first again,
			//then the path as come to a circle meaning it is over
			hasFinished = currentPoint.Current == Path.Points[0];

			//once the path has started to move for the very first time, set _isStart as false
			_isStart = false;
		}
	}


	/// <summary>
	/// Updates the current speed.
	/// </summary>
	/// <param name="speed">Speed.</param>
	public void UpdateCurrentSpeed(float speed){
		pathSpeed[_indexSpeed] = speed;
	}

}
