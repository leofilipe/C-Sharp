using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Mob first appearence. This class is used to set a path for the briefe apeareance of 
/// a character and then, make him turn and disapear. Calls to animations must be included
/// when available.
/// </summary>
public class MobFirstAppearence : FollowPath {

	public float[] pathSpeed;			//speed of the mob for each segment of the path. If left
										//empty, the default speed value is used. If less values 
										//than path nodes are provided, the last value will be 
										//used for the remaining segments.

	public float[] waitOnNode;			//time that the character must wait at each path node.
										//Uses the same rules as the above pathSpeed parameter.
										//The default wait value is zero.

	public bool disableAtTheEnd = true;

	private float _wait = 0;			//the time the character must wait at each node 
								
	private bool _facingRight;			//checks if the charecter is facing right or left

	private bool _hasFinished = false;	//checks if the whole path has been concluded

	// Use this for initialization
	new void Start () {
	
		//executes the start method of the parent class
		base.Start();

		//sets the character position to the value of the first node of the path
		transform.position = Path.Points[0].position;

		//checks if the character starts facing left or right
		_facingRight = Path.Points[1].position.x > Path.Points[0].position.x;
	}
	
	// Update is called once per frame
	new void Update () {
	
		//if should not be moving, do nothing
		if(!moveNow)
			return;

		//OBS.: DO NOT do a verification of currentPoint == null or of currentPoint.Current == null
		//and set it to return upon those cases. This will lead to the platforms never moving again.
		//Let WalkStep and CheckNextStep handle this type of problem and merely check for the existence
		//of this values before handling them.
		if(currentPoint != null && currentPoint.Current != null){

			//if there are speeds associated to different segments of the path
			if(pathSpeed != null && pathSpeed.Length > 0){

				//get the index of the current node the character is heading to
				int index = Array.IndexOf (Path.Points, currentPoint.Current);

				//if the value is within the size of the speed array set it as the 
				//current speed. Otherwise, the last valid speed value will be in  
				//use for the remaining of the path.
				if(index >= 0 && index < pathSpeed.Length)
					Speed = pathSpeed[index];
			}

			//if it is on a node position and the waiting time is greater than zero
			if(onTarget && _wait > 0){
				//decrease the waiting time and exit the method.
				_wait -= Time.deltaTime;
				return;
			}

			//if it is on a node position and has not completed the path, 
			//check if the sprite must be turned
			if(onTarget && !_hasFinished){

				//get the actual direction the character must face
				bool facing = currentPoint.Current.position.x > transform.position.x;

				//if the actual direction and the current one are different
				//then turn the character
				if(facing != _facingRight){

					_facingRight = facing;					

					Vector3 scale = transform.localScale;					
					scale.x = -scale.x;
					
					transform.localScale = scale;
				}
			}
			//if the previous is false and the character has completed the path,
			//then deactivated it
			else if (_hasFinished && disableAtTheEnd)
				gameObject.SetActive(false);
			else if (_hasFinished && !disableAtTheEnd)
				return;
		}

		//walk through the path
		WalkStep();

		//check if the current node of the path the character is heading to is the last one and if
		//it has arrived on it. 
		//Do this BEFORE calling CheckNextStep as it will change the current node of the path
		_hasFinished = (currentPoint.Current == Path.Points[Path.Points.Length - 1] && onTarget);

		//check if should move to the next node of the path
		CheckNextStep();

		//if the character is on a node position and the waiting time is zero when could
		//be a value associated to it (i.e waitOnNode exists and has values on it), then
		//find the apropriate wainting value
		if(onTarget && _wait <= 0 && waitOnNode != null && waitOnNode.Length > 0){

			//get the index of the current node the character is heading to
			int index = Array.IndexOf (Path.Points, currentPoint.Current);

			//if the value is within the size of the wait array set it as the 
			//current waiting time. Otherwise, the waiting time is set to zero.
			if(index >= 0 && index < waitOnNode.Length)
				_wait = waitOnNode[index];
			else
				_wait = 0;
		}

	}

	public bool isPathFinished(){
		return _hasFinished;
	}
}
