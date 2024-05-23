using UnityEngine;
using System.Collections;

public class DriveVehicle :MonoBehaviour {

	public bool mounted= false;
	public bool jump = false;

	bool facingRight = true;	

	[SerializeField] float maxSpeed = 20f;
	[SerializeField] float jumpForce = 500f;			// Amount of force added when the player jumps.	

	private PlatformerCharacter2D player;
	void Start () {

		player = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlatformerCharacter2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		mounted = player.onVehicle;

		if(mounted)
		{
			if (Input.GetButtonDown("Jump")) jump = true;
			float speed = Input.GetAxis("Horizontal");
			Move(speed,false,jump);
			jump = false;
		}
	}


	public void Move(float move, bool crouch, bool jump){
			
			// Move the character
			GetComponent<Rigidbody2D>().velocity = new Vector2(move * maxSpeed, GetComponent<Rigidbody2D>().velocity.y);
			
			// If the input is moving the player right and the player is facing left...
			if(move > 0 && !facingRight)
				// ... flip the player.
				facingRight = Flip.HorizontalFlip(transform, facingRight);
			// Otherwise if the input is moving the player left and the player is facing right...
			else if(move < 0 && facingRight)
				// ... flip the player.
				facingRight = Flip.HorizontalFlip(transform, facingRight);

		
		// If the player should jump...
		if (jump) {
			// Add a vertical force to the player.

			GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, jumpForce));
			
		}

	}
}
