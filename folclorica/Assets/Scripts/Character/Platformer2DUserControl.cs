using UnityEngine;

[RequireComponent(typeof(PlatformerCharacter2D))]
public class Platformer2DUserControl : MonoBehaviour 
{
	private PlatformerCharacter2D character;
	private Rigidbody2D rigidBody;
	private float wait = 0;

	[HideInInspector]	
    public bool jump;

	public AudioSource audioJump;
	public AudioSource audioWalk;

	public float walkInterval = .5f;
	

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		character = GetComponent<PlatformerCharacter2D>();
		rigidBody = GetComponent<Rigidbody2D>();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
    void Update ()
    {
        // Read the jump input in Update so button presses aren't missed.
		if (Input.GetButtonDown("Jump")) jump = true;


    }

	/// <summary>
	/// Fixeds the update.
	/// </summary>
	void FixedUpdate()
	{
		if(!character.onVehicle){
			// Read the inputs.
			bool crouch = character.grounded && (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)); 
			//(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));
			float h = Input.GetAxis("Horizontal");

			// Pass all parameters to the character control script.
			character.Move( h, crouch , jump );

			if(jump){
				if(audioJump != null)
					audioJump.PlayOneShot(audioJump.clip);

			}else if (character.grounded /* && rigidBody.velocity != Vector2.zero && wait <= 0*/){


				if(audioWalk != null){

					if(rigidBody.velocity != Vector2.zero){

						if(wait <= 0){
							wait = walkInterval;
							audioWalk.PlayOneShot(audioWalk.clip);
						}else{
							wait -= Time.deltaTime;
						}

					}else{
						wait = 0;
					}
					
				}


				/*if(rigidBody.velocity == Vector2.zero){
					wait = 0;
				}else{



					if(wait <= 0){
						wait = walkInterval;
						audioWalk.PlayOneShot(audioWalk.clip);
					}else{
						wait -= Time.deltaTime;
					}
				}*/



					/*if(rigidBody.velocity != Vector2.zero){
						
						if(wait <= 0){
							audioWalk.PlayOneShot(audioWalk.clip);
							wait = walkInterval;
						}else{
							wait -= Time.deltaTime;
						}
					}else{
						wait = walkInterval;
					}*/
			} else if (!character.grounded){
				wait = 0;
			}
	        // Reset the jump input once it has been used.
		    jump = false;
		}

	}
}
