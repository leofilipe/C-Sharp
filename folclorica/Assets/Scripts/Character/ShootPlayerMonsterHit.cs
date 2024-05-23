using UnityEngine;
using System.Collections;

public class ShootPlayerMonsterHit : ShootPlayer {

	public bool ignoreIfPlayerOnRope = true;

	PlatformerCharacter2D _player;

	Collider2D _collider;

	// Use this for initialization
	void Start () {
	
		ignoreHorizontalMovement = false;

		if(!(target is CircleCollider2D)){

			Collider2D[] cols =  target.gameObject.GetComponents<Collider2D>();

			foreach(Collider2D col in cols)
				if(col is CircleCollider2D)
					target = col;
		}

		_player = target.gameObject.GetComponent<PlatformerCharacter2D>();

		_collider = GetComponent<Collider2D>();
	}

	/*void Update (){
		//base.FixedUpdate();

		if(countDown <=0)
			Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Monsters"), false);
	}*/

	new protected void OnCollisionEnter2D(Collision2D other){

		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Monsters"), true);
		//Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Monster_Hit"), true);

		if(other.collider != target)
			return;

		if(_player.grounded)
			return;

		if(_player.isDamaged)
			return;
		if(_player.transform.parent != null && ignoreIfPlayerOnRope)
			return;

		base.OnCollisionEnter2D(other);
	}

	void OnCollisionExit2D(Collision2D other){

		if(other.collider != target)
			return;
		else
			Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Monsters"), false);
	}
}
