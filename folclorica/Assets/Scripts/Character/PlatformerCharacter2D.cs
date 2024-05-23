using UnityEngine;

/// <summary>
/// Controls the player
/// </summary>
using System.Collections;


public class PlatformerCharacter2D : MonoBehaviour 
{
	
	//AUGUSTO
	public UIHealthMeter heartTank;
	private float lasthittime= 0;				//time of the last hit taken
	public float RepeatDamagePeriod;			//minimum time between one damage and the other

	
	/// <summary>
	/// Mass of the pushable objects.
	/// </summary>
	public int massForPushing = 3;

	public AudioSource damageAudio;
	
	[HideInInspector]
	public bool grounded = false;				//Whether or not the player is grounded. Accessible by other classes. 
	//Do not show on Unity Editor
	[HideInInspector]
	public bool facingRight = true;				//For determining which way the player is currently facing. Accessible  
	//by other classes. Do not show on Unity Editor
	[HideInInspector]
	public bool onVehicle = false;				//Indicates if the player is on a vehicle or not. Do not allow him to drive it.
	//For that, add class DriveVehicle to the vehicle game object

	[HideInInspector]
	public bool isDamaged = false;				//Indicates if the player has suffered damage
	
	[SerializeField]
	private float _maxSpeed = 10f;				//The fastest the player can travel in the x axis.
	
	[SerializeField]
	private float _jumpForce = 400f;			//Amount of force added when the player jumps.	
	
	[SerializeField] 
	private float _crouchSpeed = .36f;			//Amount of maxSpeed applied to crouching movement. 1 = 100%
	
	[SerializeField] 
	private bool _airControl = false;			//Whether or not a player can steer while jumping;
	
	[SerializeField] 
	private LayerMask _whatIsGround;			//A mask determining what is ground to the character
	
	//[Range(0, 1)] 							//lost its function?
	
	private Transform _groundCheck;				//A position marking where to check if the player is grounded.
	private Transform _ceilingCheck;			//A position marking where to check for ceilings
	
	private float _groundedRadius = .1f;		//Radius of the overlap circle to determine if grounded
	private float _ceilingRadius = .01f;		//Radius of the overlap circle to determine if the player can stand up
	
	private Animator _anim;						//Reference to the player's animator component.
	
	private bool _standardAirControlValue;				//Auxiliar variable to airControl to avoid stucking at the side 
	//of platforms that dont have a platform effector on them			
	
	private GameObject _movingPlatform;			//Stores the current moving platform the player is atop of (if any)
	//in order to keep him moving with it.
	
	private Vector3 _activeGlobalPlatformPoint;	//Stores the position the player is in at the platform in world space
	private Vector3 _activeLocalPlatformPoint;	//Stores the position the player is in at the platform converted to the

	private bool isCrouching;
	//local space of the platform.
	//private Vector3 _platformVelocity;		//moving platform current velocity {get; private set;} TODO for late implementations


	
	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake(){
		// Setting up references.
		_groundCheck = transform.Find("GroundCheck");
		_ceilingCheck = transform.Find("CeilingCheck");
		_anim = GetComponent<Animator>();
		
		_standardAirControlValue = _airControl;


		//AUGUSTO
		heartTank.StartHeartTank(GameControl.instance.currentHealthPoints, GameControl.instance.maxHealthPoints); //essas duas linhas aqui eu adicionei
		//currentHP = maximumHP;//essa e a segunda linha


		
	}
	
	/// <summary>
	/// Fixeds the update.
	/// </summary>
	void Update(){
		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		grounded = Physics2D.OverlapCircle(_groundCheck.position, _groundedRadius, _whatIsGround);
		
		//Linecast(transform.position, groundCheck.position, whatIsGround);
		
		//Physics2D.OverlapCircle(groundCheck.position, groundedRadius, whatIsGround);
		
		_anim.SetBool("Ground", grounded);
		_anim.SetBool ("Mounted", onVehicle);
		
		// Set the vertical animation
		_anim.SetFloat("vSpeed", GetComponent<Rigidbody2D>().velocity.y);

		if(isDamaged){
			if(Time.time > lasthittime + RepeatDamagePeriod)
				isDamaged = false;
		}
	}
	
	/// <summary>
	/// Move the character by the specified move, wheter he is walking crouching or jumping.
	/// </summary>
	/// <param name="move">Move the character by the specified move value.</param>
	/// <param name="crouch">If set to <c>true</c> player is crouching. Movement must adapt to it</param>
	/// <param name="jump">If set to <c>true</c> player is jumping. Movement must adapt to it</param>
	public void Move(float move, bool crouch, bool jump){

		isCrouching = crouch;

		// If crouching, check to see if the character can stand up
		if(!crouch && _anim.GetBool("Crouch")){
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if( Physics2D.OverlapCircle(_ceilingCheck.position, _ceilingRadius, _whatIsGround)){
				crouch = true;
				
				//Debug.Log("Keep Crounching...");
			}//else
			//	Debug.Log("End Crounching..." + crouch);
		}
		
		// Set whether or not the character is crouching in the animator
		_anim.SetBool("Crouch", crouch);
		
		//calls the method responsible for keeping the playr on the platform
		HandleMovingPlatform();
		
		//only control the player if grounded or if airControl is turned on and the player is not touching the ground in anyway
		if(grounded || _airControl){
			// Reduce the speed if crouching by the crouchSpeed multiplier
			move = (crouch ? move * _crouchSpeed : move);
			
			// The Speed animator parameter is set to the absolute value of the horizontal input.
			_anim.SetFloat("Speed", Mathf.Abs(move));
			
			// Move the character
			GetComponent<Rigidbody2D>().velocity = new Vector2(move * _maxSpeed, GetComponent<Rigidbody2D>().velocity.y);
			
			// If the input is moving the player right and the player is facing left...
			if(move > 0 && !facingRight)
				// ... flip the player.
				facingRight = Flip.HorizontalFlip(transform, facingRight);
			// Otherwise if the input is moving the player left and the player is facing right...
			else if(move < 0 && facingRight)
				// ... flip the player.
				facingRight = Flip.HorizontalFlip(transform, facingRight);

			//Debug.Log("movendo...");
		}
		
		//if the player is mounted
		if(onVehicle == true){
			//desmontar apertando up & jump
			//LEONARDO - DEVE HAVER UM AVISO NA TELA PARA QUE O JOGADOR SAIBA DISSO
			if (Input.GetKey(KeyCode.UpArrow)){
				if (Input.GetKeyDown(KeyCode.Space)){
					onVehicle = false;
				}
			}
		}
		
		//if the player is over a moving platform
		if(_movingPlatform != null){
			SetMovingPlatform();
		}
		
		// If the player should jump...
		if (grounded && jump) {
			// Add a vertical force to the player.
			_anim.SetBool("Ground", false);
			GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, _jumpForce));
			
		}
	}
	
	/// <summary>
	/// Starts the variables needed for maintaining the player of the moving platform
	/// </summary>
	void SetMovingPlatform(){
		//stores the player position in the world space
		_activeGlobalPlatformPoint = transform.position;
		
		//convertes the player position from world space to the local space of the 
		//requesting game object (moving platform) and stores it
		_activeLocalPlatformPoint = _movingPlatform.transform.InverseTransformPoint(transform.position);
	}
	
	/// <summary>
	/// Handles the platform.
	/// </summary>
	void HandleMovingPlatform(){
		
		//if the player is over a moving platform
		if (_movingPlatform != null){
			
			//convertes the platform local space position to world space position
			var newGlobalPlatformPoint = _movingPlatform.transform.TransformPoint(_activeLocalPlatformPoint);
			
			//calculates the difference between the moving platform and the player's position in world space values
			var moveDistance = newGlobalPlatformPoint - _activeGlobalPlatformPoint;
			
			//prints a series of coordinates on the console log
			/*Debug.Log("Player position: " + transform.position);
			Debug.Log("Platform position: " + MovingPlatform.transform.position);
			Debug.Log("_activeLocalPlatformPoint position: " + _activeLocalPlatformPoint);
			Debug.Log(" --> newGlobalPlatformPoint position: " + newGlobalPlatformPoint);
			Debug.Log("_activeGlobalPlatformPoint position: " + _activeGlobalPlatformPoint);
			Debug.Log("Distance: " + moveDistance);*/
			
			//if the distance is not zero (the platform has moved)
			if(moveDistance != Vector3.zero){
				
				//translate the player's position by the difference between his and 
				//the platform's position in world space coordinates
				transform.Translate(moveDistance, Space.World);
			}
			
			//calculates the platform velocity (future use)
			//_platformVelocity = moveDistance/Time.deltaTime;
			
		}
		//if not
		/*else{
			//the velocity is zero
			_platformVelocity = Vector3.zero;
		}*/
		
		//avoids keep using last update's position
		//obs: the code urges to keep this line but it does not work with it as it is
		//MovingPlatform = null;
	}
	
	/*/// <summary>
	/// Raises the collision enter2 d event.
	/// </summary>
	/// <param name="collision">Collision.</param>
	void OnCollisionEnter2D(Collision2D collision){
		
		checkMount(collision);
	}
	
	void checkMount(Collision2D coll){
		
		//checks whether or not the player must be set as mounted
		//if true
		/*if (coll.gameObject.tag.ToLower() == "mount") {
			
			coll.gameObject.GetComponent<HingeJoint2D>().enabled = true;
			
			coll.gameObject.transform.SetParent(gameObject.transform, true);

			DriveVehicle drive = coll.gameObject.GetComponent<DriveVehicle>();

			onVehicle = true;

			if(drive != null){
				drive.mounted = onVehicle;
			}
		}	*/
		
	//}
	
	/// <summary>
	/// Raises the collision stay2 d event.
	/// </summary>
	/// <param name="coll">Coll.</param>
	void OnCollisionStay2D(Collision2D other) {
		
		//Debug.Log(coll.gameObject.tag.ToLower() + " " + coll.gameObject.name);
		
		//if the player is colliding with something and he is not grounded 
		//air control should not be possible. Unleast he was intentionally shot up
			if(!grounded && LayerMask.LayerToName(other.gameObject.layer) != "Monsters" &&
			   (other.gameObject.tag != "Ground" && other.gameObject.tag != "MovingPlatform")){

			   //other.gameObject.GetComponentInChildren<ShootPlayer>() == null){
			_airControl = false;

			//Debug.Log("Desligando air control...");
			}
			
			//if the player is in contact with a box
			if(other.gameObject.tag.ToLower() == "box"){
				
				//if the player is holding the left control key
				if (isCrouching) {
					//sets the box mass to 8 so he can push it
					other.gameObject.GetComponent<Rigidbody2D>().mass = massForPushing;
				} 
				//else
				else {
					//sets the box mass to 1000 so it stays in place
					other.gameObject.GetComponent<Rigidbody2D>().mass = 1000;
				}
			}
			
			//if the player is over a moving platform
			if (other.gameObject.tag.ToLower() == "movingplatform") {
				
				
				//if there is no moving platform defined
				if(_movingPlatform == null){
					
					//set the moving platform as the collider's game object
					_movingPlatform = other.gameObject;
					
					//stores the player position in world space cordinates
					_activeGlobalPlatformPoint = transform.position;
					
					//stores the player position in respect to the moving platform local space coordinates
					_activeLocalPlatformPoint = _movingPlatform.transform.InverseTransformPoint(transform.position);
					
				}
			}
		
		//checkMount(coll);
	}
	
	/// <summary>
	/// Raises the collision exit2 d event.
	/// </summary>
	/// <param name="coll">Coll.</param>
	void OnCollisionExit2D(Collision2D coll){
		
		//if the player has lost contact with a moving platform
		if (coll.gameObject.tag.ToLower() == "movingplatform") {
			
			//there is no moving platform
			_movingPlatform = null;
			
		}
		
		//if the player is not colliding with something air control should be set 
		//to its original value regardless of whether the player is grounded or not
		_airControl = _standardAirControlValue;
	}


	//AUGUSTO
	public void TakeMobDamage(int dano, Transform enemy, float damageForce){	//essa funçao inteira aqui
		
		TakeDamage(dano, enemy, true, damageForce);
		
	}

	public void TakeObjDamage(int dano, Transform enemy){	//essa funçao inteira aqui
		
		TakeDamage(dano, enemy, false, 0);
		
	}

	//AUGUSTO
	private void TakeDamage(int dano, Transform enemy, bool push, float damageForce){	//essa funçao inteira aqui
		
		if(Time.time > lasthittime + RepeatDamagePeriod){

			GameControl.instance.currentHealthPoints -= dano;

			lasthittime = Time.time;

			isDamaged = true;

			GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);

			if(push){
				Vector2 hurtvector =  damageForce*2*(transform.position - enemy.position) + Vector3.up * 2 * damageForce; 
				GetComponent<Rigidbody2D>().AddForce(hurtvector, ForceMode2D.Force);
			}
			
			StartCoroutine(UnableUserForTime(RepeatDamagePeriod));

			heartTank.HeartUpdate(GameControl.instance.currentHealthPoints);

			if(damageAudio != null)
				damageAudio.PlayOneShot(damageAudio.clip);

		}
		
		
	}

	/// <summary>
	/// Unables the user for time.
	/// LEONARDO
	/// TODO nao seria mais interessante desabilita-lo enquanto estiver no ar ao 
	/// inves de por um periodo? (grounded == false)?
	/// 
	/// </summary>
	/// <returns>The user for time.</returns>
	/// <param name="espera">Espera.</param>
	IEnumerator UnableUserForTime(float espera){	//essa aqui tambem adicionei

			SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
			
			
			//Color originalColor = renderer.color;

			renderer.color = GameControl.instance.PlayerHurtColor;

			transform.gameObject.GetComponent<Platformer2DUserControl>().enabled = false;
			yield return new WaitForSeconds(espera);
			transform.gameObject.GetComponent<Platformer2DUserControl>().enabled = true;

			renderer.color = LevelControl.instance.PlayerStartingRendererColor;
	}

}

