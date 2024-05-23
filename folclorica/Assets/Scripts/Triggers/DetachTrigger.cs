using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Detaches a game object from a bigger "body" after a set time limit. Respaws it at the same spot once 
/// it leaves the camera view if the player has not exceeded the maximum check point to do so.
/// </summary>
public class DetachTrigger : MonoBehaviour {

	public Rigidbody2D detatchedBy;

	public float time = 2f;				//time to wait before detaching the object once the player is on it.

	public float delayRespaw = 1f;		//wait an extra time to respaw the branch if all conditions are valid.

	public float glitchAngle = -3;		//angle at which the object may tilt before detach from its main body

	public GameObject detachObject;		//the object to be detached from its main body

	public GameObject maxAxisX;			//object must be respwan in place while this checkpoint is not exceeded.

	public AudioSource detachAudio;		//audio to play when object is "breaking" (if any)
	
	private Vector3 _startPosition;		//starting positon of the object

	private Quaternion _startRotation;	//starting rotation of the object
	
	private float _duration;			//controls "time"'s elapsed time

	private float _delayRespaw;			//controls "delayRespaw"'s elapsed time

	private bool _onTrigger = false;	//controls whether or not player is on the trigger.

	private float _maxAxisX;			//as the player moves from left to right in platform games, use the X coordinate
										//to check if the player has moved beyond the target checkpoint.

	private float _playerGravityScale;	//gravity scale of the player to be set for the game object for its fall

	private Rigidbody2D _detachRB2D;	//rigidbody of the object

	// Use this for initialization
	void Start () {

		//retrieve object's starting position and rotation
		_startPosition = detachObject.transform.position;
		_startRotation = detachObject.transform.rotation;

		//set duration as time
		_duration = time;

		//set the delay respaw time
		_delayRespaw = delayRespaw;

		//retrieve the player's gravity scale
		_playerGravityScale = detatchedBy.gravityScale;

		//if the object does not have a rigid body 2d
		if(detachObject.GetComponent<Rigidbody2D>() == null)
			//add one to it
			_detachRB2D = detachObject.AddComponent<Rigidbody2D>();
		else
			//retrieve it
			_detachRB2D = detachObject.GetComponent<Rigidbody2D>();
		
		//set the gravity scale to the same as the player's
		_detachRB2D.gravityScale = _playerGravityScale;

		try{
			
			//retrieve the X coordinate of the check point and attribute it to the variable
			_maxAxisX = maxAxisX.transform.position.x;


			//Debug.Log("Ultimo checkpoint: " + LevelControl.instance.lastCheckpointIndex);

			//if the x coordinate of the last used checkpoint is bigger (to the right) of
			//the max defined checkpoint for this object, then the player has moved beyond
			//the maximum checkpoint for this object
			if(LevelControl.instance.lastCheckpoint.transform.position.x > _maxAxisX){

				//set the object as inactive
				detachObject.SetActive(false);
			}
		}
		//in case of an exception, print it
		catch(Exception ex){
			Debug.Log(ex.GetType() + " " + ex.StackTrace);
		}
	}
	
	// Update is called once per frame
	void Update () {

		//x position of the last checkpoint
		float lastCheckPointX;

		//if lastCheckPointX to the x position of the last checkpoint or 
		//to the smallest float value if there is not one.
		if(LevelControl.instance.lastCheckpoint != null)
			lastCheckPointX = LevelControl.instance.lastCheckpoint.transform.position.x;
		else
			lastCheckPointX = float.MinValue;

		//acquire the position of this object in relation to the main camera
		Vector3 onView = Camera.main.WorldToViewportPoint(transform.position);

		//if z is lower than zero it is out of the camera view
		bool outOfView = onView.z < 0;

		//if both x and y are lower than zero (down left side of camera) it is out of the camera view
		outOfView = outOfView || (onView.x < 0 && onView.y < 0); //default value is zero

		//if x is greater than zero and y are lower than zero (down right side of the camera) 
		//it is out of the camera view. As the object only falls down, upper coordinates 
		//are not tested
		outOfView = outOfView || (onView.x > 0 && onView.y < 0); //default value is zero

		//if the object has already been detached, decrement the respaw time
		if(_duration <= 0)
			_delayRespaw -= Time.deltaTime;

		//if the object is out of view, the object has moved from its original place, and the player
		//has not moved beyond the maximum checkpoint
		if(outOfView && detachObject.transform.position != _startPosition && 
		   lastCheckPointX<= _maxAxisX && _delayRespaw <= 0){

			//stop it on one place
			_detachRB2D.isKinematic = true;

			//reset the object's position and rotation
			detachObject.transform.position = _startPosition;
			detachObject.transform.rotation = _startRotation;

			//reset the object's activation trigger and timer
			_onTrigger = false;
			_duration = time;
			_delayRespaw = delayRespaw;

			//reactivate its colliders
			foreach(Collider2D col in detachObject.GetComponentsInChildren<Collider2D>()){
				col.enabled = true;
			}

		}

		//if the x coordinate of the last used checkpoint is bigger (to the right) of
		//the max defined checkpoint for this object, then the player has moved beyond
		//the maximum checkpoint for this object
		if(lastCheckPointX > _maxAxisX && _delayRespaw <= 0){

			//reset the object's position and rotation
			detachObject.transform.position = _startPosition;
			detachObject.transform.rotation = _startRotation;

			//set the object as inactive
			detachObject.SetActive(false);
		}

		//if the player is not on the trigger, exit and do nothing else. 
		//This assures the object  to tilt and get detached only while the player is upon it.
		if (!_onTrigger)
			return;

		//calculate the angle to tilt to from the current rotation and desired glitch angle
		float angle = Mathf.MoveTowardsAngle(detachObject.transform.eulerAngles.z, glitchAngle, 2 * Time.deltaTime);

		//set the angle to the object
		detachObject.transform.eulerAngles = new Vector3(0, 0, angle);

		//decrease the duration
		_duration -= Time.deltaTime;

		//if it has reached at least zero
		if(_duration <= 0){

			//disable the objects colliders so it has a clean fall and dont hit anything
			foreach(Collider2D col in detachObject.GetComponentsInChildren<Collider2D>()){
				col.enabled = false;
			}

			//let it fall
			_detachRB2D.isKinematic = false;

			//set on trigger as false
			_onTrigger = false;
		}

	}

	/// <summary>
	/// Raises the collision enter2 d event.
	/// </summary>
	/// <param name="other">Other.</param>
	void OnCollisionEnter2D(Collision2D other){

		OnCollisionStay2D(other);
	}

	/// <summary>
	/// Raises the collision stay2 d event.
	/// </summary>
	/// <param name="other">Other.</param>
	void OnCollisionStay2D(Collision2D other){

		//if there is any audio to play, then play it.
		if(detachAudio != null && !detachAudio.isPlaying)
			detachAudio.Play();

		//once turned on there is no turn back until it falls.
		_onTrigger = true;
	}

	/*
	/// <summary>
	/// Ons the trigger exit2 d.
	/// </summary>
	/// <param name="other">Other.</param>
	void OnCollisionExit2D(Collision2D other){

		_onTrigger = false;
	}*/
}
