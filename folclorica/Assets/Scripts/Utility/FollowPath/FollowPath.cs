using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Makes a game object follow a previously set path. 
/// Usually used for moving platforms but can be used 
/// with any game objecct
/// </summary>
public class FollowPath : MonoBehaviour {
	
	public enum FollowType								//an enum to define two different types of movements 
	{
		MoveTowards = 0,
		Lerp = 1
	}
	
	public FollowType Type = FollowType.MoveTowards;	//the default movement type

	public PathDefinition Path;							//the points of the path that will be followed

	public float Speed = 1;								//the default speed for crossing the path
	public float MaxDistanceToGoal = .1f;

	public bool startFromCurrentLocation = false;		//a flag to indicate if the object must be placed on the default 
														//starting position or if it can start its path from its current position.

	public bool moveNow = true;							//begins to move the platform whenever the value is true. The default value  
														//is true so the platform begins to move as soon as the game starts.

	[HideInInspector]
	public IEnumerator<Transform> currentPoint;		//the point of the path that the object is currently moving to.
	
	public bool onTarget {get; private set;}					//whether or not the game object has reached one of the points of its path

	public bool displaySpeed = false;

	private int currentPathIndex;

	private int previousPathIndex;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	public void Start(){

		//if there is no path, do nothing
		if (Path == null){
			Debug.LogError("Path cannot be null", gameObject);
			return;
		}

		//retrieve the points of the path
		currentPoint = Path.GetPathEnumerator();
		currentPathIndex = -1;

		//if should start moving from now (from game start)
		if(moveNow){

			//move to the next point
			currentPoint.MoveNext();
			UpdatePathIndex();

			//if there is yet no current point, then there are no points in the path.
			//Do nothing and end the method
			if (currentPoint.Current == null) {
				return;
			}

			//if it cant start its path from its current position
			if(!startFromCurrentLocation)
				//set the current position of the object as the current position of the path (first position)
				transform.position = currentPoint.Current.position;
			else {
				SetStartingPath();
			}
		}
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	public void Update(){

		//if it should not be moving, do nothing
		if(!moveNow)
			return;

		try{
			
			WalkStep();			//perform the next step of the movement
			CheckNextStep();	//sets the next step to perform, if there is any
			
		}catch(Exception ex){ 	//catch any exception and write them on the console
			Debug.Log(ex.StackTrace);
		}


		
	}

	/// <summary>
	/// Performs the movement.
	/// </summary>
	public void WalkStep(){

		//if there is no current point, print an error message and do nothing

		//OBS: The IEnumerator class has often displayed an incosistent behavior where currentPoint or currentPoint.Current
		//suddenly become null. When reaching this point currentPoint should not be null. If it is, then an incosistence has
		//happended. This aims to fix it so the platforms keep moving.
		if (currentPoint == null){

			//restart the value of currentPoint
			currentPoint = Path.GetPathEnumerator();
			currentPathIndex = -1;

			//try to find its current place and destiny
			SetStartingPath();

			throw new NullReferenceException(@"IEnumerator Current is null...");
		}

		//if there is no value for this current point
		if (currentPoint.Current == null){
			//try to find the next value
			currentPoint.MoveNext();
			UpdatePathIndex();
		}
		
		switch (Type){

			//if the movement is of type moving forward
			case FollowType.MoveTowards:
				transform.position = Vector3.MoveTowards(transform.position, currentPoint.Current.position, Time.deltaTime*Speed);
				break;

			//if the movement is of type lerp
			case FollowType.Lerp:
				transform.position = Vector3.Lerp(transform.position, currentPoint.Current.position, Time.deltaTime*Speed);
				break;
		}

		//the current distance between the object and its intended location
		var distanceSquared = (transform.position - currentPoint.Current.position).sqrMagnitude;

		//the onTarget parameter indicates whether or not this distance is within the acceptable limit
		onTarget = distanceSquared < MaxDistanceToGoal * MaxDistanceToGoal;
		
	}

	
	/// <summary>
	/// Sets the next step to perform, if there is any. This method is 
	/// overwritten by the child classes
	/// </summary>
	protected void CheckNextStep(){
		
		//if there is no current point, print an error message and do nothing
		if (currentPoint == null){
			
			currentPoint = Path.GetPathEnumerator();
			currentPathIndex = -1;

			throw new NullReferenceException(@"IEnumerator Current is null...");
		}
		
		//if the object is on its intended location
		if (onTarget){
			//move to the next one
			currentPoint.MoveNext();
			UpdatePathIndex();

			if(displaySpeed)
				Debug.Log("Mudando de secao: " + GetComponent<Rigidbody2D>().velocity);
		}
		
	}

	/// <summary>
	/// Checks if the object is on its intended location and 
	/// if this is the last point of its path. If it is, it will be
	/// returning to the previous position
	/// </summary>
	public bool OnDestiny(){

		//if the object is not on its intended target, return false
		if(!onTarget)
			return false;

		//check if the current index of the path is lower than the previous one.
		//if it is, the path has been concluded and will be retracing
		return currentPathIndex < previousPathIndex; //currentPoint.Current.position == Path.Points[Path.Points.Length - 2].position;
	}

	/*public bool OnStep(){

		return onTarget;
	}*/

	protected void SetStartingPath(){

		if(!moveNow)
			return;

		try{

			Transform[] path = Path.Points;
			
			Vector3 currentPosition = transform.position;
			
			Vector3 _startAt = Vector3.zero;
			
			float distance = float.MaxValue;
			
			foreach(Transform step in path){
				
				float aDistance = (currentPosition - step.position).magnitude; //Vector3.Distance(currentPosition, step.position);
				
				//Debug.Log("Passo: " + step.transform.name + " " + step.position );
				if(aDistance < distance && step.position.y <= transform.position.y){
					
					distance = aDistance;
					_startAt = step.position;
				}
			}

			//OBS: The IEnumerator class has often displayed incosistent behavior where currentPoint or currentPoint
			//suddenly become null. The timer in this loop aims to avoid an infinity loop by interrupting it if reached.
			float _duration = 3f;

			if(currentPoint.Current == null){
				currentPoint.MoveNext();
				UpdatePathIndex();
			}

			while(currentPoint.Current.position != _startAt && _duration > 0){
				currentPoint.MoveNext();
				UpdatePathIndex();

				_duration -= Time.deltaTime;
			}

		}catch(Exception ex){

			Debug.Log(ex.ToString() + " " + ex.StackTrace);
			WalkStep();
			SetStartingPath();
		}
	}

	private void UpdatePathIndex(){

		previousPathIndex = currentPathIndex;

		currentPathIndex++;

		if(currentPathIndex >= Path.Points.Length)
			currentPathIndex = 0;
	}
}