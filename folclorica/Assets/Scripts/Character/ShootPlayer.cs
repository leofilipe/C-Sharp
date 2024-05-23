using UnityEngine;
using System.Collections;

/// <summary>
/// Shoots the player up in the sky direction. Might be changed into a general direction class if needed.
/// </summary>
public class ShootPlayer : MonoBehaviour {

	//public bool usePlayerFeetCollider = true;

	public Collider2D target;						//object to be shot up

	public float jumpForce = 500f;						//the jump force

	public float timer = 1f;							//the time that must be waited before activating 
														//the player's colliders 2D
	public bool colliderOffOnCollision = true;			//self-explanatory


	//private GameObject _player;					//the player's game object
	private Rigidbody2D _playerRB;					//the rigid body 2D of the player's game object
	private Collider2D[] _playerColls;				//the colliders 2D of the player's game object

	protected bool shoot = false;

	protected Vector2 shootVector = Vector2.zero;

	protected bool ignoreHorizontalMovement = true;

	protected float countDown;							//the countdown to reach the waiting time
	
	/// <summary>
	/// Update method called within regular times.
	/// </summary>
	void FixedUpdate () {
	
		//if the player game object is set for this class
		if(shoot){

			//decrease the waiting time
			countDown -= Time.deltaTime;

			//if the player has a parent remove it. It means he is still attatched to a rope
			if(target.gameObject.transform.parent != null)
				//remove the parent by jumping off the rope to reset any additional parameter
				target.gameObject.GetComponent<RopeControl>().Jump();

			//set the player rigidbody velocity as the vector up
			if(!ignoreHorizontalMovement){

				Vector2 up = Vector2.up;
				up.x = _playerRB.velocity.x;

				_playerRB.velocity = up;

			}else{
				_playerRB.velocity = Vector2.up;

				_playerRB.gameObject.GetComponent<Platformer2DUserControl>().enabled = false;
				_playerRB.gameObject.GetComponent<PlatformerCharacter2D>().enabled = false;
			}
			//add a force of 500 to its vertical component. 
			//If u do not set the velocity first the player will shoot up to the skies
			_playerRB.AddForce(shootVector);
			
			//if the countdown has reached zero
			if(countDown <= 0){

				//set the player's collider back to enabled
				if(_playerColls != null)
					foreach(Collider2D collider in _playerColls){
						collider.enabled = true;
					}
				else
					Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Monsters"), false);
				//reset the player game object for this class as null 
				shoot = false;

				//reset the countdown.
				countDown = timer;

				//set the player's rigidbody velocity back to zero.
				_playerRB.velocity = Vector2.zero;

				//if the player must not bounce on reaching the ground
				//if(!bouncyOnGround)
				//Add an opposity force to the player, otherwise he will bounce
				//on reaching a platform. Bouncing migth be desireble.
				shootVector.y *= -1;

				//_playerRB.AddForce(new Vector2(0f, -jumpForce));
				_playerRB.AddForce(shootVector);

				_playerRB.gameObject.GetComponent<Platformer2DUserControl>().enabled = true;
				_playerRB.gameObject.GetComponent<PlatformerCharacter2D>().enabled = true;
			}
		}
	}

	/// <summary>
	/// Raises the collision enter2 d event.
	/// </summary>
	/// <param name="col">Col.</param>
	protected void OnCollisionEnter2D(Collision2D col){


		if(col.collider != target)
			return;

		//if should deactivate player's colliders on collision
		if(colliderOffOnCollision){

			//retrieve the player's colliders
			if(_playerColls == null)
				_playerColls = col.gameObject.GetComponents<Collider2D>();

			//set the player's colliders to inactive
			foreach(Collider2D collider in _playerColls){
				collider.enabled = false;
			}
		}

		//set the countdown time for reenabling player's colliders
		countDown = timer;

		//retrieve the player's rigidbody
		_playerRB = col.rigidbody;

		PrepareShootVector();

		//retrive the player's game object
		shoot = true;
	}

	protected void PrepareShootVector(){
		Vector2 shoot = new Vector2(0f, jumpForce);

		if(shootVector == Vector2.zero)
			shootVector = shoot;
		else if (shootVector.x == shoot.x && shootVector.y == - shoot.y)
			shootVector = shoot;

		Debug.Log("Original");
	}

	void OnCollisionStay2D(Collision2D col){

		if(col.collider != target)
			return;

		if(!shoot)
			OnCollisionEnter2D(col);
	}
}
