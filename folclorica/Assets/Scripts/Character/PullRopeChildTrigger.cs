using UnityEngine;
using System.Collections;

/// <summary>
/// This class was created because it was not possible for a child trigger to fire the 
/// OnTriggerEnter2D event on its father. Hence, each link of the chain has this class 
/// attatched to it.
/// </summary>
public class PullRopeChildTrigger : MonoBehaviour {
	
	private FollowPath _ropeDown;			//the path the chain follows while going down
	private PullRope _pullRope;				//the main class for this behavior

	// Use this for initialization
	void Start () {
	
		//Retrieve the respective objects from the parent of this object.
		if(_ropeDown == null)
			_ropeDown = gameObject.GetComponentInParent<FollowPath>();

		if(_pullRope == null)
			_pullRope = gameObject.GetComponentInParent<PullRope>();
	}

	//if the trigger was activated
	void OnTriggerStay2D(Collider2D other){

		//if the rope is already going down, then do nothing
		if(_ropeDown.moveNow)
			return;

		//if the player was not the one who collided with 
		//the chain link
		if(other.gameObject.tag != "Player")
			return;

		if(other.gameObject.transform.parent == null)
			return;

		//start the rope path
		_ropeDown.moveNow = true;
		//set the link the player is currently attatched to as this one
		_pullRope.currentLink = transform;
	}

	void OnTriggerEnter2D(Collider2D other){
		OnTriggerStay2D(other);
	}

	/*void OnTriggerStay2D(Collider2D other){
		OnTriggerEnter2D(other);
	}*/

}
