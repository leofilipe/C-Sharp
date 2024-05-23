using UnityEngine;
using System.Collections;
using System;

public class DelayedFollowPath : FollowPath {
	
	/// <summary>
	/// If there is a game object attattched, indicates if should wait 
	/// for it at its destine or its origin.
	/// </summary>
	public bool waitAtDestine = true;
	public bool waitForTargetDestine = true;
	
	public GameObject waitObject;
	
	public float waitMidPoints = .3f;
	
	private Vector3 _vectorWaitAt;
	
	private float _duration;
	
	// Use this for initialization
	new void Start () {
		
		base.Start();
		
		//if i should wait at the starting or the final position
		_vectorWaitAt = waitAtDestine? Path.Points[Path.Points.Length - 1].position : Path.Points[0].position;
	}
	
	// Update is called once per frame
	new void Update () {

		//if it should not be moving, do nothing
		if(!moveNow)
			return;

		try{
			
			base.WalkStep();
			CheckNextStep();
			
		}catch(Exception ex){
			Debug.Log("Em " + gameObject.name + ": " + ex.ToString() + " " + ex.StackTrace);
		}
	}
	
	protected new void CheckNextStep(){
		//Debug.Log("Checking");

		if (currentPoint == null){
			
			throw new NullReferenceException(@"IEnumerator Current is null...");
		}else if (currentPoint.Current == null){
			
			throw new NullReferenceException(@"No current Transform (null) within IEnumerator...");
		}

		if (onTarget){
			_duration -= Time.deltaTime;
			
			if(_duration <= 0)
				if(currentPoint.Current.position != _vectorWaitAt){
					currentPoint.MoveNext();
				}else{
					if(!ShouldWait(waitObject))
						currentPoint.MoveNext();
				}	
		}else
			_duration = waitMidPoints;
	}
	
	private bool ShouldWait(GameObject waitObject){	
		
		if(waitObject == null)
			return false;
		
		UnityEngine.Object obj = waitObject.GetComponentInChildren<MobIA>();

		if(obj is Black_faced_ox){

			//obs: some times, the loop is interrupted at very small differences. 
			//Ex: while looking for 0.8 the loop is interrupted on 0.7999998
			//using a difference as parameter should avoid such problem.
			bool wait = waitMidPoints - ((Black_faced_ox)obj).elapsedTime >= 0.001f;

			//Debug.Log("Mid: " + waitMidPoints + " Elapsed: " + ((Black_faced_ox)obj).elapsedTime + " Wait: " + wait);
			return wait;

		}else if(obj is MobIA){
			return ((MobIA)obj).goLeft;
		}
		
		obj = waitObject.GetComponentInChildren<FollowPath>();
		
		if(obj is FollowPath){
			
			Vector3 vectorWaitFor = waitForTargetDestine? ((FollowPath)obj).Path.Points[Path.Points.Length - 1].position :
				((FollowPath)obj).Path.Points[0].position;
			
			if(((FollowPath)obj).currentPoint == null)
				return false;

			if(((FollowPath)obj).currentPoint.Current == null)
				return false;
			
			return ((FollowPath)obj).currentPoint.Current.position == vectorWaitFor;
		}
		return false;
	}
}