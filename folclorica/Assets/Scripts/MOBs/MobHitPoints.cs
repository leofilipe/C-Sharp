using UnityEngine;
using System.Collections;

public class MobHitPoints : MonoBehaviour {

	public int hitPoints = 1;
	public int score = 1;
	public Collider2D attackerCollider;
	public bool isPlayer = true;
	public AudioSource audioHit;


	private MobIA _mobIA;
	private PlatformerCharacter2D _player;

	//private PlatformerCharacter2D _player;
	private Collider2D _collider;

	void Start(){

		if(isPlayer && !(attackerCollider is CircleCollider2D)){
			
			Collider2D[] cols =  attackerCollider.gameObject.GetComponents<Collider2D>();
			
			foreach(Collider2D col in cols)
				if(col is CircleCollider2D)
					attackerCollider = col;

			_player = attackerCollider.gameObject.GetComponent<PlatformerCharacter2D>();
		}
		

		_collider = GetComponent<Collider2D>();


		_mobIA = gameObject.GetComponentInParent<MobIA>();
	}

	void Update(){

		//if the mob has hit points, do nothing
		if(hitPoints > 0)
			return;

		//if the mob is moving
		if(_mobIA.speed > 0){

			//stop it
			gameObject.GetComponentInParent<MobIA>().speed = 0;

			//sets the mob's hit area as imovable and disables its collider
			gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
			gameObject.GetComponent<Collider2D>().enabled = false;
			
			
			
			
			Vector3 scale = transform.parent.localScale;
			scale.y = - scale.y;
			
			transform.parent.localScale = scale;
		}else if (_mobIA.IsGrounded){

			transform.parent.GetComponent<Rigidbody2D>().isKinematic = true;	
			transform.parent.GetComponent<Collider2D>().enabled = false;
			this.enabled = false;
		}
	}

	void OnCollisionEnter2D(Collision2D col){

		if(!this.enabled)
			return;

		//se nao eh o colisor
		if(col.collider != attackerCollider)
			return;

		//if player was not above the Mob.
		/*Vector3 targetColPos = col.transform.position;
		targetColPos.y += Mathf.Abs(col.transform.lossyScale.y) * col.collider.offset.y;
		
		Vector3 mobColPos = _collider.transform.position;
		mobColPos.y += Mathf.Abs(_collider.transform.lossyScale.y) *_collider.offset.y;
		
		
		Debug.Log(targetColPos +  " " + _collider.gameObject.name + " :" + mobColPos);
		
		if(targetColPos.y < mobColPos.y)
			return;*/

		//scripts desligados seguem executando operacoes da fisica, logo,
		//eh interessante evitar novas execucoes caso o script esteja desligado

		//if(_player == null)
		//	_player = col.gameObject.GetComponent<PlatformerCharacter2D>();

		//avoid player incorrectly kills mob while on rope
		if(isPlayer){
			if(attackerCollider.transform.parent != null)
				return;
			if(_player.isDamaged)
				return;
		}

		hitPoints--;

		audioHit.PlayOneShot(audioHit.clip);

		if(hitPoints <= 0){
			//adicione os pontos do jogador

			LevelControl.instance.AddScore(score);

		}
	}
}
