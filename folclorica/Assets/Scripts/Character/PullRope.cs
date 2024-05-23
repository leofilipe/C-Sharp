using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

/// <summary>
/// This class makes the player pull down the rope or chain he is attateched to.
/// At the same time, it pulls up an additional rope previously set to the class (if any).
/// </summary>
public class PullRope : MonoBehaviour {

	// Use this for initialization
	public Transform playerTransform;						//the player game object.
	public FollowPath pathRopeUp;							//the path for the rope going up

	[HideInInspector]
	public Transform currentLink;							//the chain link the player is currently attatched to.

	[HideInInspector]
	public bool pulled = false;								//indicates if the rope has been pulled all the way

	private Transform _nextLink;							//the next link the player must go to.

	private FollowPath _pathRopeDown;						//the path for the rope going down. Must come from
															//the same GameObject as the one from this class

	private PlatformerCharacter2D _playerControl;			//the player's controller
	private Platformer2DUserControl _playerJumpControl;		//the player's jump controller
	private RopeControl _playerRopeControl;					//the player's rope controller

	private float _climbTimer = 1.5f;						//the time the player will wait before climbing the next link of the chain
	
	private float _countDown;								//the time elapsed since the player climbed a link.

	private bool fromSaveFile = false; 						//indicates if the puzzle was loaded as completed from a save

	void Start () {
	
		//Debug.Log("Classe para string: " + this.GetType().Name);
		//fazer isso p monobehaviours, locks and colliders de triggers;

		//retrive all the needed objects from the player...
		_playerControl = playerTransform.gameObject.GetComponent<PlatformerCharacter2D>();
		_playerJumpControl = playerTransform.gameObject.GetComponent<Platformer2DUserControl>();
		_playerRopeControl = playerTransform.gameObject.GetComponent<RopeControl>();

		//... and from this game object.
		_pathRopeDown = GetComponent<FollowPath>();

		//set the countdown time
		_countDown = _climbTimer;

		//check if the position was loaded from the save file
		var distanceSquared = (transform.position - _pathRopeDown.Path.Points[_pathRopeDown.Path.Points.Length - 1].position).sqrMagnitude;

		fromSaveFile = distanceSquared < _pathRopeDown.MaxDistanceToGoal * _pathRopeDown.MaxDistanceToGoal;
		//if it was, then reset them and run it faster so to properly set the visibility and colliders of the chain links
		if(fromSaveFile){

			//Vector3 playerRopePos = transform.position;

			//change the player jail chain to its new position
			transform.position = _pathRopeDown.Path.Points[0].position;

			//retrieve the links of the player jail chain
			CircleCollider2D[] playerChainJoints = GetComponentsInChildren<CircleCollider2D>();

			//enable the view of all its links
			foreach(CircleCollider2D joint in playerChainJoints){
				joint.gameObject.GetComponent<SpriteRenderer>().enabled = true;
			}

			//get the jail chain starting postition
			Vector3 jailChainPos = pathRopeUp.Path.Points[0].position;

			//retrieve the links of the jail chain
			CircleCollider2D[] jaiChainJoints = pathRopeUp.gameObject.GetComponentsInChildren<CircleCollider2D>();

			//for each link
			foreach(CircleCollider2D joint in jaiChainJoints){

				//if the links is above the starting position of the first link
				if(joint.transform.position.y > jailChainPos.y){

					//disable its sprite
					joint.gameObject.GetComponent<SpriteRenderer>().enabled = false;
					Debug.Log("Desligando sprite de: " + joint.gameObject);
				}
			}

		}

	} 
	
	// Update is called once per frame
	void Update () {

		//to prevent erros, before anything, check if the player is 
		//attatched to the rope, if not, do nothing.
		if(!_playerRopeControl.onRope)// || playerTransform.parent == null)
			return;

		pulled = false;

		//if the down rope is on its destiny but has not stoped, stop it.
		if(_pathRopeDown.OnDestiny() && _pathRopeDown.moveNow){

			//set the the rope movement to false and disable it
			_pathRopeDown.moveNow = false;
			_pathRopeDown.enabled = false;

			//reenable the player controllers
			_playerControl.enabled = true;
			_playerJumpControl.enabled = true;

			//set the flag for the pull chain behavior as false
			_playerRopeControl.doNotMove = false;

			//retrieve the hingejoint objects (exclusive to the chain links)
			//from this object's children and set them movable and as triggers
			MontionlessChainLinks(GetComponentsInChildren<HingeJoint2D>(), false, true);	

			//set the rope as pulled all the way
			pulled = true;

			Lock locked = GetComponent<Lock>();

			if(locked != null)
				locked.unlocked = true;

			Debug.Log("Finished... " + locked.unlocked);
		}

		//if the down rope is on its destiny but has not stoped, stop it.
		if(pathRopeUp.OnDestiny() && pathRopeUp.moveNow){
			
			//set the the rope movement to false and disable it
			pathRopeUp.moveNow = false;
			pathRopeUp.enabled = false;

			//retrieve the hingejoint objects (exclusive to the chain links)
			//from this object's children and set them movable
			MontionlessChainLinks(pathRopeUp.gameObject.GetComponentsInChildren<HingeJoint2D>(), true);	

			Debug.Log("At the end...");
			//leave the return method
			return;
		}
	
		//if the moving down chain is moving and the player is attatched to it
		if(_pathRopeDown.moveNow && !fromSaveFile){

			//if there is no next link, then it just started moving. 
			//Initialize the parameters for this behavior
			if(_nextLink == null){

				//start to move the rope going up.
				pathRopeUp.moveNow = true;

				//disable the player controllers
				_playerControl.enabled = false;
				_playerJumpControl.enabled = false;

				//set the flag for the pull chain behavior as true
				_playerRopeControl.doNotMove = true;

				//retrieve the hingejoint objects (exclusive to the chain links)
				//from this object's children and set them montionless, then...
				MontionlessChainLinks(GetComponentsInChildren<HingeJoint2D>(), true);

				//... do the same for the rope going up
				MontionlessChainLinks(pathRopeUp.gameObject.GetComponentsInChildren<HingeJoint2D>(), true);	

				//set the next link the player should climb
				_nextLink = _playerRopeControl.ChainLink(_playerRopeControl.ChainIndex(currentLink) - 1);

			}

			//try to climb a link
			ClimbLink();

			//if the puzzle was not loaded...
			if(!fromSaveFile)
				//decrease the countdown
				_countDown -= Time.deltaTime;
		}
	}

	/// <summary>
	/// Set the RigidBody2D of all the links of a chain to kinematic or not, 
	/// depending on the enable parameter
	/// </summary>
	/// <param name="joints">Joints.</param>
	/// <param name="enabled">If set to <c>true</c> kinematic.</param>
	void MontionlessChainLinks(HingeJoint2D[] joints, bool enabled){
		MontionlessChainLinks(joints, enabled, false);
	}
	/// <summary>
	/// Set the RigidBody2D of all the links of a chain to kinematic or not, 
	/// depending on the enable parameter
	/// </summary>
	/// <param name="joints">Joints.</param>
	/// <param name="enabled">If set to <c>true</c> kinematic.</param>
	/// <param name="asTriggers"> If the object must be set as a trigger </param>
	void MontionlessChainLinks(HingeJoint2D[] joints, bool enabled, bool asTriggers){

		//for each of these objects
		foreach(HingeJoint2D body in joints){
			
			//set their rigid body as kinematic
			body.connectedBody.isKinematic = enabled;
			EnableCollider(body.gameObject.GetComponent<Collider2D>(), asTriggers);
		}
	}

	void EnableCollider(Collider2D col, bool asTriggers){
		if(!col.enabled)
			col.enabled = true;
		if(asTriggers)
			col.isTrigger = true;
	}
	/// <summary>
	/// Tries to climb a link of the chain.
	/// </summary>
	void ClimbLink(){

		//if the countdown has not reached zero, 
		//do nothing and leave the method
		if(_countDown > 0)
			return;
		//climb up a chain link
		_playerRopeControl.ClimbUp();
		//set the current chain link as the next one
		currentLink = _nextLink;
		//set a new next link
		_nextLink = _playerRopeControl.ChainLink(_playerRopeControl.ChainIndex(_nextLink) - 1);
		//reset the countdown
		_countDown = _climbTimer;

	}


}
