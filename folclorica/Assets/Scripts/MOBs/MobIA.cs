using UnityEngine;
using System.Collections;
using System;

public class MobIA : MonoBehaviour {

	public Collider2D enemyTarget;
	//public variables
	public int damage; //AUGUSTO
	public float damagePushForce = 200;
	public float speed = 2f;
	public bool doDamageWhenTargetOnRope = false;
	//public float maxSpeed = 10f;
	
	/// <summary>
	/// The duration of the MOBs moviment in any direction, unless it hits something.
	/// </summary>
	public float duration = 5f;
	
	/// <summary>
	/// Sees and follows the player if he is at least at this distance.
	/// If the range is equal or lower than zero it never follows the player
	/// </summary>
	public float visionRange = -1f;
	
	public float rayCastDistance = 3f;
	
	public float rayCastInclination = 2f;

	/// <summary>
	/// Layers that the raycast should take into account.
	/// </summary>
	public LayerMask layersToCollide;

	/// <summary>
	/// Set to true if the monster should its path walking to the left.
	/// </summary>
	public bool goLeft = false;
	//if the monster should start walking at the left direction
	//public bool startToLeft = false;


	//private variables
	private float countDown;
	private Vector3 startingPosition;

	[HideInInspector]
	public bool turnSprite{get; private set;}
	private bool changeDirection;
	private bool checkDirection;
	private bool foundPlayer;
	

	private RaycastHit2D hitLowerRight, hitLowerLeft, hitLeft, hitRight;
	
	private bool grounded = false;

	public bool IsGrounded {get{return grounded;}}
	
	// Use this for initialization
	void Start () {
		
		//player = GameObject.FindGameObjectWithTag("Player");
		
		countDown = duration;
		//Debug.Log("MobIA: " + gameObject.name);
	}
	
	void OnCollisionEnter2D(Collision2D other){

		//Debug.Log(coll.gameObject.tag.ToLower() + " " + coll.gameObject.name);
		if(other.gameObject.tag.ToLower() == "ground"){
			grounded = true;
			
			if(startingPosition == Vector3.zero)
				startingPosition = this.transform.position;
			//Debug.Log(startingPosition);
		}

		//if collided with the target and it is not attatched to anything
		if(other.collider == enemyTarget && other.gameObject.transform.parent == null){//(coll.gameObject.tag == "Player" && coll.collider is BoxCollider2D){	//AUGUSTO
			other.gameObject.GetComponent<PlatformerCharacter2D>().TakeMobDamage(damage, transform, damagePushForce);
			Debug.Log("Mob collider hi 1");
		}
		//if collided with the target and it is attatched to something and it must injury it even if so
		else if (other.collider == enemyTarget && other.gameObject.transform.parent != null && doDamageWhenTargetOnRope){
			other.gameObject.GetComponent<PlatformerCharacter2D>().TakeMobDamage(damage, transform, damagePushForce);
			Debug.Log("Mob collider hi 1");
		}
		
	}
	
	void OnCollisionStay2D(Collision2D other){

		//Debug.Log(coll.gameObject.tag.ToLower() + " " + coll.gameObject.name);
		if(other.gameObject.tag.ToLower() == "ground"){
			grounded = true;
		}
		
	}
	
	void OnCollisionExit2D(Collision2D other){


		//Debug.Log(coll.gameObject.tag.ToLower() + " " + coll.gameObject.name);
		if(other.gameObject.tag.ToLower() == "ground"){
			grounded = false;
			
		}
		
	}
	void OnDrawGizmos(){
		
		if(!grounded)
			return;
		
		if(hitLowerRight == null)
			return;
		
		//lower diagonal right
		Gizmos.DrawLine(transform.position, hitLowerRight.point);
		
		//lower diagonal left
		Gizmos.DrawLine(transform.position, hitLowerLeft.point);
		
		//horizontal left
		Gizmos.DrawLine(transform.position, hitLeft.point);
		
		//horizontal right
		Gizmos.DrawLine(transform.position, hitRight.point);
	}
	// Update is called once per frame
	protected void FixedUpdate () {
		
		//Avoids the monsters to bump on each other but it does not stop raycasting to detact them.
		//Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Monsters"), LayerMask.NameToLayer("Monsters"));
		//Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Monsters"), LayerMask.NameToLayer("Player"));
		if(!grounded)
			return;
		
		//cria os raycasts na horizontal e na diagonal do inimigo
		//se nao esta indo para a direita esta voltando (backwards)
		//lower right diagonal
		hitLowerRight = Physics2D.Raycast (new Vector3 (transform.position.x + 1.2f, transform.position.y, transform.position.z), 
		                                   new Vector2 (3, -rayCastInclination), rayCastDistance, layersToCollide);
		//right
		hitRight = Physics2D.Raycast (new Vector3 (transform.position.x + 1.2f, transform.position.y, transform.position.z), 
		                              new Vector2 (3, 0), rayCastDistance, layersToCollide);	
		//lower left diagonal
		hitLowerLeft = Physics2D.Raycast (new Vector3 (transform.position.x - 1.2f, transform.position.y, transform.position.z), 
		                                  new Vector2 (-3, -rayCastInclination), rayCastDistance, layersToCollide);
		//left
		hitLeft = Physics2D.Raycast (new Vector3 (transform.position.x - 1.2f, transform.position.y, transform.position.z), 
		                             new Vector2 (-3, 0), rayCastDistance, layersToCollide);	
		
		//turns the monster sprite
		TurnSprite();
		/*if(turnSprite){
			
			Vector3 spriteScale = transform.localScale;
			spriteScale.x = -spriteScale.x;
			
			transform.localScale = spriteScale;
			turnSprite = false;
		}*/
		
		//check if player is close
		foundPlayer = HasFoundPlayer();
		
		bool pursuePlayer = foundPlayer && visionRange > 0;
		
		//if should not pursue the player
		if(!pursuePlayer)
			transform.Translate((!goLeft? Vector2.right: - Vector2.right) * speed * Time.deltaTime);
		//if player has been found and should pursue him
		else {
			
			//LEONARDO - comentario feito em PT devido a particularidade.
			//pelo editor, percebe-se que ha uma diferenca de aproximadamente 2 unidades entre o eixo y do porco do mato
			//e do jogador. Logo, em pontos com inclinacao, eh importante usar como referencia o eixo y do destino (jogador).
			//Pode ser necessario converter o valor de comparacao em uma variavel a depender do MOB em questao
			Vector3 target = new Vector3(enemyTarget.gameObject.transform.position.x, 
			                             Mathf.Abs(transform.position.y - enemyTarget.gameObject.transform.position.y) > 2f? 
			                             enemyTarget.gameObject.transform.position.y: transform.position.y,
			                             transform.position.z);
			//move the mob towards the player
			transform.position = Vector3.MoveTowards (transform.position, target, 
			                                          1.5f * speed * Time.deltaTime);
			//reset countdown to avoid bugs during chase
			countDown = duration;
		}
		
		//if the path is blocked, turn left
		if (!goLeft) {
			
			try{
				//if both rays are not hitting anything to the right, there is no path, turn
				goLeft = !hitLowerRight && !hitRight;
				
				//if hitLowerRight is hitting anything other, than ground, player or monster, turn
				if(hitLowerRight)
					goLeft = goLeft || (hitLowerRight.collider.tag != "Player" &&
					                	hitLowerRight.collider.tag != "Ground");
				
				//as hit right is not alawys in contact with something, must be ckecked separately to avoind exceptions
				if(hitRight)
					goLeft = hitRight.collider.tag != "Player" 
					 && hitRight.collider.tag != "Ground";
				
			}catch(NullReferenceException ex){
				Debug.Log(ex.StackTrace);
			}
			
			//if should go left
			if(goLeft == true){
				
				//turn the sprite
				turnSprite = true;
				
				//reset the countdown
				countDown = duration;
			}
		} else{
			
			try{
				
				//reset go left to avoid interference in the validations
				goLeft = false;
				
				//if both rays are not hitting anything to the right, there is no path, turn
				goLeft = !hitLowerLeft && !hitLeft;
				if(hitLowerLeft)
					goLeft = goLeft || (hitLowerLeft.collider.tag != "Player" &&
					                 	hitLowerLeft.collider.tag != "Ground");
				//as hit left is not alawys in contact with something, must be ckecked separately to avoind exceptions
				if(hitLeft)
					goLeft = hitLeft.collider.tag != "Player" 
						& hitLeft.collider.tag != "Ground";
				
			}catch(NullReferenceException ex){
				Debug.Log(ex.StackTrace);
			}
			//if go left is true
			if(goLeft == true){
				
				//turn the sprite
				turnSprite = true;
				
				//reset the countdown
				countDown = duration;
			}
			
			//Since we are already going left and need to turn, change the value
			goLeft = !goLeft;
			
		}
		
		//decrease countdown by elapsed time
		countDown -= Time.deltaTime;
		
		//if count down reached zero
		if (countDown <= 0) {
			//reset the value of countdown
			countDown = duration;
			//invert the value of turn to left
			goLeft = !goLeft;
			//turn the sprite to its other direction
			turnSprite = true;
		}
	}

	/// <summary>
	/// Turns the sprite.
	/// </summary>
	protected void TurnSprite(){

		/*if(ignoreTurn)
			return;*/
		//turns the monster sprite
		if(turnSprite){
			
			Vector3 spriteScale = transform.localScale;
			spriteScale.x = -spriteScale.x;
			
			transform.localScale = spriteScale;
			turnSprite = false;
		}
	}
	/// <summary>
	/// Determines whether this instance has found player. Returns true if the player is at 
	/// a distance iguals or lower to Vision Range
	/// </summary>
	/// <returns><c>true</c> if this instance has found player; otherwise, <c>false</c>.</returns>
	private bool HasFoundPlayer() {
		
		//if the range is not greater than zero do not look for the player
		if(visionRange <= 0)
			return false;
		
		//indicates if the player has been found
		bool foundPlayer = false;
		
		//indicates direction of the mob.
		//start it with the value of backwards
		bool facingLeft = goLeft;
		
		//verifica se raycasts horizontais estao pegando outra coisa alem do personagem
		if (hitLeft){
			if (hitLeft.collider.tag == "Player"){
				foundPlayer = true;
				facingLeft = true;
			}else{
				//Debug.Log("Found on left");
				foundPlayer = false;
			}
		}
		
		if (hitRight){
			if (hitRight.collider.tag == "Player"){
				foundPlayer = true;
				facingLeft = false;
			}
			else{
				foundPlayer = false;
			}
		}
		
		
		if (Vector3.Distance(transform.position, enemyTarget.gameObject.transform.position) <= visionRange){
			foundPlayer = true;
			
			facingLeft = enemyTarget.gameObject.transform.position.x > transform.position.x? false: true;
		}
		
		//if facing left is diferent of backwards, then there was a change in the direction
		//the sprite must be turned
		turnSprite = facingLeft != goLeft;
		
		//after the comparison, set the value of backwards as the same of facing left
		goLeft = facingLeft;
		
		//return the value of foundPlayer
		return foundPlayer;
	}
	
	void OnTriggerEnter2D(Collider2D other) { //Colocar um collider como trigger em cima dos inimigos para quando personagem pular em cima, desativar seu movimento
		/*if (other.tag == "Player") {
			this.speed = 0;
			this.elapsedTime = 0;
			this.changeDirection = changeDirection;
		}*/
	}
	
	bool isNearStartPosition(Vector3 point, Vector3 position, float range){  //verifica se as posicoes x de dois vetores estao dentro da margem estabelecida
		if (Vector2.Distance(point, position) <= range)
			return true;
		else
			return false;
	}
}
