using UnityEngine;
using System.Collections;

public class ShootPlayerOld : MonoBehaviour {

	public bool usePlayerFeetCollider = true;
	
	public float jumpForce = 500f;			//the jump force
	
	public float timer = 1f;				//the time that must be waited before activating 
	//the player's colliders 2D
	
	//public bool bouncyOnGround = true;	//if this is set to true the player will bounce 
	//when reaching the ground again
	
	private float _countDown;				//the countdown to reach the waiting time
	
	private GameObject _player;				//the player's game object
	private Rigidbody2D _playerRB;			//the rigid body 2D of the player's game object
	private Collider2D[] _playerColls;		//the colliders 2D of the player's game object
	
	/// <summary>
	/// Update method called within regular times.
	/// </summary>
	void FixedUpdate () {
		
		//if the player game object is set for this class
		if(_player != null){
			
			//decrease the waiting time
			_countDown -= Time.deltaTime;
			
			//if the player has a parent remove it. It means he is still attatched to a rope
			if(_player.transform.parent != null)
				//remove the parent by jumping off the rope to reset any additional parameter
				_player.GetComponent<RopeControl>().Jump();
			
			//set the player rigidbody velocity as the vector up
			_playerRB.velocity = Vector2.up;
			//add a force of 500 to its vertical component. 
			//If u do not set the velocity first the player will shoot up to the skies
			_playerRB.AddForce(new Vector2(0f, jumpForce));
			
			//if the countdown has reached zero
			if(_countDown <= 0){
				
				//set the player's collider back to enabled
				foreach(Collider2D collider in _playerColls){
					collider.enabled = true;
				}
				
				//reset the player game object for this class as null 
				_player = null;
				
				//reset the countdown.
				_countDown = timer;
				
				//set the player's rigidbody velocity back to zero.
				_playerRB.velocity = Vector2.zero;
				
				//if the player must not bounce on reaching the ground
				//if(!bouncyOnGround)
				//Add an opposity force to the player, otherwise he will bounce
				//on reaching a platform. Bouncing migth be desireble.
				_playerRB.AddForce(new Vector2(0f, -jumpForce));
				
			}
		}
	}
	
	/// <summary>
	/// Raises the collision enter2 d event.
	/// </summary>
	/// <param name="col">Col.</param>
	void OnCollisionEnter2D(Collision2D col){
		
		
		//if the object is not the plauer, do nothing
		if(col.gameObject.tag != "Player")
			return;
		
		if(usePlayerFeetCollider && !(col.collider is CircleCollider2D))
			return;
		else if(!usePlayerFeetCollider && col.collider is CircleCollider2D)
			return;
		
		//retrieve the player's colliders
		_playerColls = col.gameObject.GetComponents<Collider2D>();
		
		//set the player's colliders to inactive
		foreach(Collider2D collider in _playerColls){
			collider.enabled = false;
		}
		
		//set the countdown time for reenabling player's colliders
		_countDown = timer;
		
		//retrieve the player's rigidbody
		_playerRB = col.rigidbody;
		
		//retrive the player's game object
		_player = col.gameObject;
	}
}
