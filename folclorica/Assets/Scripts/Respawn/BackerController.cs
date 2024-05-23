using UnityEngine;
using System.Collections;

/*
 * Created by Joao Paulo T Ruschel in 
 * */
public class BackerController : MonoBehaviour {

	//limita problemas caso haja mais de um colisor no obejto
	//public static int BACKER_EXECUTION = 0;

	public Collider2D triggeredBy;

	private FadeScene fade;

	// Use this for initialization
	void Start () {
		fade = GetComponent<FadeScene>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// When a element hits 
	/*void OnCollisionEnter2D (Collision2D coll) {

		StartCoroutine(CollisionBack (coll.collider));
	}*/
	void OnTriggerEnter2D (Collider2D coll) {

		if(coll != triggeredBy)
			return;

		StartCoroutine(CollisionBack (coll.gameObject));
	}

	// Callback from Collision or Trigger enter
	private IEnumerator CollisionBack (GameObject _gameObject) {

		Respawn dec = triggeredBy.gameObject.GetComponent<Respawn>();

		if (dec != null) {

			yield return new WaitForSeconds(fade.fadeOutTime);

			fade.StartFade();

			PlatformerCharacter2D player = triggeredBy.gameObject.GetComponent<PlatformerCharacter2D>();

			if(player != null)
				player.TakeObjDamage(2, transform);

			if(GameControl.instance.currentHealthPoints > 0)
				dec.GoBack();

		}
	}
}
