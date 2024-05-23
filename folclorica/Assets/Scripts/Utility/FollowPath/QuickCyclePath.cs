using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Quick cycle path. 
/// When reaching a giver target in path, returns the game object 
/// to the preestablished position of the path. It also attemps to maintain
/// the specified y axis distance between its platforms
/// </summary>
public class QuickCyclePath : FollowPath {


	public Transform restartPathAt;			//the path position the game object must return too when reaching the path destiny

	public QuickCyclePath watchDistanceTo;	//the game object to which this one must keep track of its current distance on the y axis

	public float yKeepDistance;				//the y axis distance that must be kept between the two objects

	public float ignoreAfter;				//if the y axis distance if greater than this value, then do not keep track or try to correct

	public float glitchAngle = -3;			//slightly rotate platforms in by the given angle while they perform its path

	public float duration = 2f;				//duration for the rotation of the glitch angle

	private float _baseSpeed;				//original speed value for this game object
	
	private bool _restarted;				//flag if the path has rotated to its original position

	private float _startingAngle;			//initial rotation for this game object. Used to calculte _angle

	private float _angle;					//actual for rotation. Calculated from glitchAngle and _startingAngle
	
	private float elapsedTime;


	new void Start(){

		base.Start();

		//_maxDistanceY  = Mathf.Abs(transform.position.y - watchDistanceTo.transform.position.y);

		_baseSpeed = Speed;

		/*glitchAngle = Random.value > .5? glitchAngle : - glitchAngle;
		startingAngle = transform.rotation.eulerAngles.z;

		_angle = glitchAngle + startingAngle;*/

		glitchAngle = Mathf.Abs(glitchAngle);
		_startingAngle = transform.rotation.eulerAngles.z;

		_angle = _startingAngle + (UnityEngine.Random.value > .5? glitchAngle : - glitchAngle);

		SetStartingPath();
	}
	// Update is called once per frame
	new void Update () {

		//if it should not be moving, do nothing
		if(!moveNow)
			return;

		try{
			CalculateKeepDistance();

			base.WalkStep();
			base.CheckNextStep();
		}catch(Exception ex){
			Debug.Log("Em " + gameObject.name + ": " + ex.ToString() + " " + ex.StackTrace);
		}
	}

	private void CalculateKeepDistance(){

		if(currentPoint == null)
			return;
		
		if(currentPoint.Current == null)
			return;
		
		if(currentPoint.Current.position == restartPathAt.position && onTarget){
			
			transform.position = currentPoint.Current.position;
			_restarted = true;
			
		}else if (currentPoint.Current.position != restartPathAt.position){
			_restarted = false;
		}
		
		float deltaY = Mathf.Abs(transform.position.y - watchDistanceTo.transform.position.y);
		
		if(deltaY > yKeepDistance && !watchDistanceTo._restarted){
			
			if(transform.position.y > watchDistanceTo.transform.position.y){
				
				if(deltaY > 1){
					
					//divide the speed to compensate the distance
					Speed = _baseSpeed * deltaY;
				}else{
					//multiply the speed to compensate the distance
					Speed = _baseSpeed + _baseSpeed * deltaY;
				}
				
				/*Speed = _baseSpeed * 
					Mathf.Max(transform.position.y, watchDistanceTo.transform.position.y)/
						Mathf.Min(transform.position.y, watchDistanceTo.transform.position.y);
				Speed = Mathf.Abs(Speed);*/
			}else{
				Speed = _baseSpeed;
			}
		}else if (deltaY < yKeepDistance){
			
			if(deltaY > 1){
				
				//divide the speed to compensate the distance
				Speed = _baseSpeed / deltaY;
			}else{
				//multiply the speed to compensate the distance
				Speed = _baseSpeed - _baseSpeed * deltaY;
			}
			
		}
		else if (deltaY == yKeepDistance || watchDistanceTo._restarted){
			
			Speed = _baseSpeed;
		}
		
		float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, _angle, 2 * Time.deltaTime);
		
		transform.eulerAngles = new Vector3(0, 0, angle);
		
		elapsedTime += Time.deltaTime;
		
		if(elapsedTime >= duration){
			
			if(_angle != _startingAngle){
				_angle = _startingAngle;
			}else{
				_angle = _startingAngle + (UnityEngine.Random.value > .5? glitchAngle : - glitchAngle);
			}
			
			elapsedTime = 0;
			
		}
	}
}
