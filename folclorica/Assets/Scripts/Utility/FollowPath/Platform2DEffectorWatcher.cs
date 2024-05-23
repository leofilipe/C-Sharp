using UnityEngine;
using System.Collections;

/// <summary>
/// Platform2 D effector watcher. When using moving platforms and platform effectors, 
/// there is a high risk of the player falling from the platform. To avoid that, disable 
/// the effector's "one way" property while player is on it.
/// </summary>
public class Platform2DEffectorWatcher : MonoBehaviour {

	private BoxCollider2D platformCollider;	//the collider of this platform's game object
	private PlatformEffector2D effector;	//the effector of this platform's game object

	private bool _standingHere;				//flag indicanting that the player is standing on this platform
	private float _duration;				//elapsed time since player left the platform

	public Rigidbody2D targetRigidBody;		//the rigid body 2d of the target game object (usually he player's)

	public float wait = 0.3f;				//waiting time for when the player leaves the platform
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start(){

		//find the collider and effector for this platform
		platformCollider = transform.parent.GetComponent<BoxCollider2D>();
		effector = platformCollider.GetComponent<PlatformEffector2D>();

		//disable the collider and set the one way property of the effector as true
		platformCollider.enabled = false;
		effector.useOneWay = true;

	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update(){

		//if the player is not standing on this platform and the duration 
		//is greater than zero, then, decrease it
		if(!_standingHere && _duration > 0)
			_duration -= Time.deltaTime;

		//if the duration is equal or lower than zero
		if(_duration <= 0){

			//disable this platform's collider and set the one way 
			//property of the effector as true
			platformCollider.enabled = false;
			effector.useOneWay = true;
		}
	}

	/// <summary>
	/// Raises the trigger enter2d event.
	/// </summary>
	/// <param name="col">Col.</param>
	void OnTriggerEnter2D(Collider2D col){

		//if it is not the target game object, do nothing
		if(col.gameObject != targetRigidBody.gameObject)
			return;

		//if the rigidbody is moving up
		if(targetRigidBody.velocity.y > 0){

			//check to see if its the feet collider (CircleCollider2D) and it is bellow this platform
			if(col is CircleCollider2D && col.transform.position.y < platformCollider.transform.position.y){

				//enable the platform collider and reset the duration
				platformCollider.enabled = true;
				_duration = wait;
			}

		}
		//if the rigidbody is moving down (falling)
		else if (targetRigidBody.velocity.y < 0){

			//no further checks are needed. Enable the platform collider and reset the duration
			platformCollider.enabled = true;
			_duration = wait;
		}

	}

	/// <summary>
	/// Raises the trigger stay2d event.
	/// </summary>
	/// <param name="col">Col.</param>
	void OnTriggerStay2D(Collider2D col){

		//if it is not the target game object, do nothing
		if(col.gameObject != targetRigidBody.gameObject)
			return;

		//check to see if its the feet collider (CircleCollider2D) and it is on top of this platform 
		//(i.e. standing on it)
		if(col is CircleCollider2D && col.transform.position.y > platformCollider.transform.position.y){

			//set the stading here flag as true and disable the effector. Otherwise,
			//the target may fall from the platform as it moves.
			_standingHere = true;
			effector.useOneWay = false;

			//reset the duration
			_duration = wait;
		}
	}

	/// <summary>
	/// Raises the trigger exit2d event.
	/// </summary>
	/// <param name="col">Col.</param>
	void OnTriggerExit2D(Collider2D col){

		//if it is not the target game object, do nothing
		if(col.gameObject != targetRigidBody.gameObject)
			return;

		//check to see if its the feet collider (CircleCollider2D)
		//if it is, then the player is not stading on this platform
		// anymore. change the flag to false.
		if(col is CircleCollider2D)
			_standingHere = false;
	}	
}
