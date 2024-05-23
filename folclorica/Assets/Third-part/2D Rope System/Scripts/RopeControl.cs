using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RopeControl : MonoBehaviour {
	
	public float climbUpInterval = 0.05f;			//interval between climbing up
	public float climbDownInterval = 0.04f;			//interval between climbing down
	public float swingForce = 10.0f;				//force which will be added to connected chain when left/right buttons will be pressed
	public float delayBeforeSecondHang = 0.4f;		//delay after jump before player will be able to hang on another rope

	[HideInInspector]
	public bool doNotMove = false;
	
	private static Transform collidedChain;			//saves transform on which player is connected
	private static List<Transform> chains;			//saves connected rope's chain objects
	
	private Transform playerTransform;				//saves player's transform
	private int chainIndex = 0;						//saves chains index on which player is connected
	private Collider2D[] colliders;					//saves all colliders of player
	private PlatformerCharacter2D playerControl;	//used for enabling/disabling PlayerControl script
	private Platformer2DUserControl jumpControl; 	//used to get the jump component of Platformer2DUserControl
	private Animator _anim;							//used for playing animations
	
	public bool onRope {private set; get;}			//wheter or not player is attatched to the rope (default is false)

	public AudioSource audioRope;
	public AudioSource audioJump;
	public float audioTimer = 0.3f;

	private float climbTimer = 0.0f;
	private float waitAudio = 0.0f;

	//private float audioClimbInterval;
	
	// Use this for initialization
	void Start (){
		//get player's components
		playerTransform = transform;
		colliders = GetComponentsInChildren<Collider2D>();
		playerControl = GetComponent<PlatformerCharacter2D>();
		jumpControl = GetComponent<Platformer2DUserControl> ();
		_anim = GetComponent<Animator>();
	}
	
	
	// Update is called once per frame
	void Update (){
		try{

			if(onRope)	{
				//make player's position and rotation same as connected chain's
				playerTransform.position = collidedChain.position;
				//stop's player from tilting to the sides while on the rope
				playerTransform.rotation = Quaternion.identity;
				
				//if player is moving up
				if(!doNotMove & Input.GetAxisRaw ("Vertical") > 0) {

					climbTimer += Time.deltaTime;
					waitAudio += Time.deltaTime;

					//if chain index is greater than 2, then  there is an acessible link above the player, 
					//climb up a rope link within the time interval
					if(chainIndex > 2){
						if(climbTimer > climbUpInterval)	{
							ClimbUp ();
							climbTimer = 0.0f;

							/*if(audioRope != null)
								audioRope.PlayOneShot(audioRope.clip);*/
						}
					}
					//else, release the rope
					else{
						Jump();
						if(audioJump != null)
							audioJump.PlayOneShot(audioJump.clip);
					}
				}
				
				//if player is moving down
				if(!doNotMove & Input.GetAxisRaw ("Vertical") < 0) {
					//if chain index is lower than the max number minus one, then  there is an acessible link 
					//bellow the player, climb down a rope link within the time interval
					if(chainIndex < chains.Count - 1){

						climbTimer += Time.deltaTime;
						waitAudio += Time.deltaTime;
						
						if(climbTimer > climbUpInterval)
						{
							ClimbDown ();
							climbTimer = 0.0f;

							/*if(audioRope != null)
								audioRope.PlayOneShot(audioRope.clip);*/
						}
					}
					//else, release the rope
					else{
						Jump(); //if there isn't chain below player, jump from rope
						if(audioJump != null)
							audioJump.PlayOneShot(audioJump.clip);
					}
				}
				
				//if jump button is pressed, jump from rope
				if(!doNotMove & Input.GetButtonDown ("Jump"))
				{
					Jump();
					jumpControl.jump = true;
				}
				
				//Cache the horizontal input.
				float H = Input.GetAxis("Horizontal");
				
				if(H > 0 && !playerControl.facingRight)
					// ... flip the player.
					playerControl.facingRight = Flip.HorizontalFlip(transform, playerControl.facingRight);
				// Otherwise if the input is moving the player left and the player is facing right...
				else if(H < 0 && playerControl.facingRight)
					// ... flip the player.
					playerControl.facingRight = Flip.HorizontalFlip(transform, playerControl.facingRight);
				
				//add swing force to connected chain
				collidedChain.GetComponent<Rigidbody2D>().AddForce (Vector2.right * H * swingForce);
			}

		}catch (Exception ex){
			Debug.Log("Erro em " + gameObject.name + ": " + ex.ToString() + " " + ex.StackTrace);
		}
	}
	
	/// <summary>
	/// Climbs down the rope.
	/// </summary>
	public void ClimbDown(){
		//get all HingeJoint2D components from chain below the player
		var joint = chains[chainIndex + 1].GetComponent<HingeJoint2D>();

		//if chain has HingeJoint2D but isn't enabled jump from rope
		if(joint && !joint.enabled)	{
			Jump();
			return;
		}

		PlayRopeAudio();
		
		//connect player to below chain
		collidedChain = chains[chainIndex + 1];
		playerTransform.parent = collidedChain;
		chainIndex ++;
	}
	
	/// <summary>
	/// Climbs up the rope.
	/// </summary>
	public void ClimbUp(){
		//get all HingeJoint2D components from chain above the player
		var joint = chains[chainIndex - 1].GetComponent<HingeJoint2D>();

		//if chain has HingeJoint2D but isn't enabled don't do anything
		if(joint && !joint.enabled)
			return;

		PlayRopeAudio();

		//connect player to above chain
		collidedChain = chains[chainIndex - 1];
		playerTransform.parent = collidedChain;
		chainIndex --;
	}

	/// <summary>
	/// Returns the index of the chain the player is currently attached to.
	/// </summary>
	/// <returns>The index.</returns>
	/// <param name="link">Link.</param>
	public int ChainIndex(Transform link){

		return chains.IndexOf(link);
	}

	/// <summary>
	/// Returns the transform of the chain link the player is currently attached to.
	/// </summary>
	/// <returns>The link.</returns>
	/// <param name="index">Index.</param>
	public Transform ChainLink(int index){
		return chains[index];
	}

	public void Jump(){
		StartCoroutine(JumpOff());
	}

	/// <summary>
	/// Grabs the rope.
	/// </summary>
	/// <param name="coll">Coll.</param>
	private void GrabRope(Collider2D other){
		
		//stop walking animation (if activated)
		_anim.SetFloat ("Speed", 0);	
		
		//ignore collisions between the player and the rope layers
		foreach(var col in colliders )//col.enabled = false;
			Physics2D.IgnoreLayerCollision(col.gameObject.layer, LayerMask.NameToLayer("Climber"));
		
		var chainsParent = other.transform.parent;	//get collided object's parent
		chains = new List<Transform>();
		
		//fill chains list 
		foreach (Transform child in chainsParent)
			chains.Add(child);
		
		//connect player to collided object
		collidedChain = other.transform;
		chainIndex = chains.IndexOf (collidedChain);
		playerTransform.parent = collidedChain;
		onRope = true;

		_anim.SetBool("OnRope", onRope);
		
		//do this to avoid gravity from affecting player and mess with its position in
		//respect to the chain link he is currently attached to.
		playerTransform.GetComponentInChildren<Rigidbody2D>().isKinematic = true;
		
	}

	/// <summary>
	/// Determines whether this instance is able to grab the rope collider corresponing to "other"
	/// </summary>
	/// <returns><c>true</c> if this instance can grab the rope collider specified by other;
	/// otherwise, <c>false</c>.</returns>
	/// <param name="other">Other.</param>
	private bool IsGrabRope(Collider2D other){

		//if the collider is not tagged as rope, return false
		if(other.gameObject.tag != "rope2D")
			return false;

		//if player is not grounded, check if the ctrl key is pressed
		//bool grabRope = !playerControl.grounded && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));
		
		bool grabRope = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
		grabRope = grabRope || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
		
		return grabRope;
	}

	/// <summary>
	/// Jumps off the rope.
	/// </summary>
	/// <returns>The off.</returns>
	private IEnumerator JumpOff(){   

		//there is no need to call jump sound here as it is normally called by the button press

		//reset player's velocity
		GetComponent<Rigidbody2D>().velocity = Vector2.zero;	
		//detach player from chain
		playerTransform.parent = null;
		//corrects player's rotation
		transform.rotation = Quaternion.identity;

		//flag player as not on the rope
		onRope = false;

		_anim.SetBool("OnRope", onRope);
		
		//when on the rope, player is not affect by gravity in order for it do not mess 
		//with its position in respect to the chain link he is currently attached to.
		//When leaving the rope, he must be affected by gravity again
		playerTransform.GetComponentInChildren<Rigidbody2D>().isKinematic = false;

		//wait the specified amount of seconds
		yield return new WaitForSeconds(delayBeforeSecondHang);	
		
		//reestablishes collisions between the player and the rope layers
		foreach(var col in colliders ){
			Physics2D.IgnoreLayerCollision(col.gameObject.layer, LayerMask.NameToLayer("Climber"), false);
		}
		
	}

	private void PlayRopeAudio(){

		if(waitAudio >= audioTimer){

			waitAudio = 0f;

			if(audioRope != null)
				audioRope.PlayOneShot(audioRope.clip);
		}
	}

	/*private void PlayClimbAudio(){
		if(audioRope != null){
			
			//if(rigidBody.velocity != Vector2.zero){
				
			if(audioClimbInterval <= 0){
				audioClimbInterval = jumpControl.walkInterval;
				audioRope.PlayOneShot(audioRope.clip);
			}/*else{
				audioClimbInterval -= Time.deltaTime;
			//}
				
			//}else{
			//	wait = 0;
			//}
			
		}
	}*/

	/// <summary>
	/// Raises the trigger enter2 d event.
	/// </summary>
	/// <param name="other">Other.</param>
	private void OnTriggerEnter2D(Collider2D other){

		if (IsGrabRope(other))
			GrabRope(other);
	}

	/// <summary>
	/// Raises the trigger stay2 d event.
	/// </summary>
	/// <param name="other">Other.</param>
	private void OnTriggerStay2D(Collider2D other){

		if (IsGrabRope(other))
			GrabRope(other);
	}
}
