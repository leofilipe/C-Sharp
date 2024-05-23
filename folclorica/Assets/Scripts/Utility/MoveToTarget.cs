using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class MoveToTarget : MonoBehaviour {

	public Lock unlockedBy;
	public Transform target;
	public float speed;
	public float MaxDistanceToGoal = .1f;
	public float waitBeforeStart;

	public MoveType type;

	protected bool onTarget = false;

	public enum MoveType								//an enum to define two different types of movements 
	{
		MoveTowards = 0,
		Lerp = 1
	}
	
	// Update is called once per frame
	protected void Update () {
	
		if(unlockedBy.unlocked){

			waitBeforeStart -= Time.deltaTime;

			if(waitBeforeStart > 0)
				return;

			switch (type){
				
				//if the movement is of type moving forward
			case MoveType.MoveTowards:
				transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime*speed);
				break;
				
				//if the movement is of type lerp
			case MoveType.Lerp:
				transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime*speed);
				break;
			}

			//the current distance between the object and its intended location
			var distanceSquared = (transform.position - target.position).sqrMagnitude;
			
			//the onTarget parameter indicates whether or not this distance is within the acceptable limit
			onTarget = distanceSquared < MaxDistanceToGoal * MaxDistanceToGoal;
		}
	}
}
