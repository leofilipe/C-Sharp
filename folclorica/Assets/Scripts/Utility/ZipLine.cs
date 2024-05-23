using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ZipLine : MonoBehaviour {

	public Lock[] locks;						//the objects that must be checked to unlock the zipline

	public GameObject zipLineRope;				//the rope object of the zipline linking the two desired areas.
	public GameObject player;
	public GameObject[] deactivateUponZiplineUse;//objects that must be deactivated while the player is on the zipline
	public Collider2D zipLineStopper;			//Collider to stop the zipline

	public UITipDialog tipLocked;				//tip dialog to be shown when the zipline is not available (if any)
	public UITipDialog tipUnlocked;				//tip dialog to be shown once the zipline is available (if any)

	public AudioSource audioUnlocked;			//audio dialog to be played once the zipline is available (if any).
												//works as an auditive tip
	public SpriteRenderer ropeSprite;

	[HideInInspector]
	public bool unlocked = false; 				//flag indicating that all locks have been unlocked and the zipline can be used

	private Collider2D[] _zipLineRopeColliders;	//stores all the zipline rope colliders. They are activated only when the player  
												//is going down the rope
	private List<Transform> _chainLinks;		//chains links of the zipline in order to check if player is on it

	private bool _playerOnZipline;				//flag to indicate that the player is holding the zipline

	private bool _zipLineReady = true;			//flag to indicate that the zipline was not used yet.

	private bool _playerCollidersOn = true;		//player colliders are turned off when goind down the zipline so to not  
												//improperly trigger events in the way. This flag indicates their state

	private float _cameraXSmooth;				//the zipline speed is much faster than regular speed. Camera values 
	private float _cameraYSmooth;				//must be adjusted to keep pace with it

	private CameraFollow _camFollow;			//required for acessing and setting camera values for following the player

	private bool _hasShownUnlockedTip = false;	//flag if the tip has been show or not
	private bool _hasPlayedAudio = false;		//flag if the tip has been played or not



	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () {
	
		//if the player has not been set, find it
		if(player == null)
			player = GameObject.FindGameObjectWithTag("Player");

		//retrieve all the colliders of the zipline and deactivate then until they're needed.
		_zipLineRopeColliders = zipLineRope.GetComponentsInChildren<Collider2D>();

		foreach(Collider2D col in _zipLineRopeColliders){
			col.enabled = false;
		}

		//retrieve the camera follow component from the main camera and store its 
		//default following values
		_camFollow = Camera.main.GetComponent<CameraFollow>();

		_cameraXSmooth = _camFollow.xSmooth;
		_cameraYSmooth = _camFollow.ySmooth;

		_chainLinks = GetComponentsInChildren<Transform>().ToList();
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update () {

		//check if the locks on the zipline have been removed
		unlocked = CheckLock();

		//check if the player is ready to go down the zipline and he is attatched to it (is its child) 
		_playerOnZipline = transform.GetComponentInChildren<PlatformerCharacter2D>() != null;

		//if the zipline is unlocked, was not used and the player is on it
		if(unlocked && _zipLineReady && _playerOnZipline){

			//disable player controllers
			player.GetComponent<Platformer2DUserControl>().enabled = false;
			player.GetComponent<PlatformerCharacter2D>().enabled = false;

			//stop player from moving on the rope
			player.GetComponent<RopeControl>().doNotMove = true;

			//get all the colliders of the player and deactivate then. 
			//the rope colliders don't interact with any relevant collider
			//so there is no need to deactivate them too.
			Collider2D[] colliders = player.GetComponentsInChildren<Collider2D>();

			//deactivate player's colliders
			foreach(Collider2D coll in colliders)
				coll.enabled = false;

			//deactivate the selected objects
			foreach(GameObject comp in deactivateUponZiplineUse)
				comp.SetActive(!comp.activeSelf);

			//activate the colliders of the zipline so the player can go down on it
			foreach(Collider2D col in _zipLineRopeColliders){
				col.enabled = true;
			}

			//enable the physics of the tack
			transform.GetComponent<Rigidbody2D>().isKinematic = false;

			//set the zipline as ready to use
			_zipLineReady = false;

			//flag the player's colliders to be deactivated
			_playerCollidersOn = false;

			//set the new camera follow values
			_camFollow.xSmooth = 3;
			_camFollow.ySmooth = 3;

			SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();

			//adjust the rope sprite in respect to the player's sorting order
			ropeSprite.sortingLayerName = playerSprite.sortingLayerName;
			ropeSprite.sortingOrder = playerSprite.sortingOrder - 2;
		}

		//if the zipline is unlocked and the player was not told it yet
		if(!_hasShownUnlockedTip && unlocked){

			//check if there is an audio tip to play
			if(audioUnlocked != null){

				//if there is, was not played yet and is not playing...
				if(!audioUnlocked.isPlaying && !_hasPlayedAudio){

					//... then play it
					audioUnlocked.PlayOneShot(audioUnlocked.clip);
					//flag it as having been played
					_hasPlayedAudio = true;
				}
				//if there is and it is not playing because it has been played
				else if(!audioUnlocked.isPlaying && _hasPlayedAudio){

					//show the tip dialog (if there is any) and flag it as shown
					if(tipUnlocked != null){
						StartCoroutine(WaitAndShowTip(1f));
					}
					_hasShownUnlockedTip = true;
				}

			}
			//if there is no audio tip to play
			else{

				//show the tip dialog (if there is any) and flag it as shown
				if(tipUnlocked != null){
					tipUnlocked.TriggerTip();
				}
				_hasShownUnlockedTip = true;
			}
		}


		//if the zipline is locked and the player is on its rope.
		//OBS: a simple check for parent != null as first done will pop up
		//every time the player grabs at any rope
		if(!unlocked && _chainLinks.Contains(player.transform.parent)){

			//then tell the player he cant use it yet
			tipLocked.TriggerTip();
		}
	}

	IEnumerator WaitAndShowTip(float wait){

		yield return new WaitForSeconds(wait);

		tipUnlocked.TriggerTip();

	}

	/// <summary>
	/// Raises the collision enter2d event.
	/// </summary>
	/// <param name="collision">Collision.</param>
	IEnumerator OnCollisionEnter2D(Collision2D collision){

		//if it has hit the zip line stoper
		if(collision.collider == zipLineStopper){

			//set the zipline tack as kinematic so it wont move anymore
			transform.GetComponent<Rigidbody2D>().isKinematic = true;


			//reenable player's controllers and its capacity to leave the rope
			player.GetComponent<Platformer2DUserControl>().enabled = true;
			player.GetComponent<PlatformerCharacter2D>().enabled = true;
			player.GetComponent<RopeControl>().doNotMove = false;

			//***************
			//active all of the player's colliders
			Collider2D[] colliders = player.GetComponentsInChildren<Collider2D>();
			
			foreach(Collider2D coll in colliders)
				coll.enabled = true;
			
			//activate the previously activated game objects
			foreach(GameObject comp in deactivateUponZiplineUse)
				comp.SetActive(!comp.activeSelf);
			
			//deactivate the rope colliders
			foreach(Collider2D col in _zipLineRopeColliders){
				col.enabled = false;
			}
			
			//reactivate the player's colliders
			_playerCollidersOn = true;

			yield return new WaitForSeconds(.5f);
			//***************
			player.GetComponent<Platformer2DUserControl>().jump = true;
			player.GetComponent<RopeControl>().Jump();

			//turn off this collider
			collision.collider.enabled = false;

			_camFollow.xSmooth = _cameraXSmooth;
			_camFollow.ySmooth = _cameraYSmooth;

			this.enabled = false;

		}
	}

	/// <summary>
	/// Checks every lock of the Lock array. If all are flaged as unlock then the zipline can be used
	/// </summary>
	/// <returns><c>true</c>, if lock was checked, <c>false</c> otherwise.</returns>
	bool CheckLock(){

		//start a flag variable with true
		bool unlock = true;

		//for each lock on the array, loop while the flag is set as true
		for(int i = 0; i < locks.Length && unlock; i++){
			//set the flag value as itsef and the value of unlock.
			//the first false ocurrence will turn it to false
			unlock = unlock && locks[i].unlocked;

		}

		return unlock;
	}
}
