using UnityEngine;
using System.Collections;

public class Respawn : MonoBehaviour {
	
	// Back delegate
	public delegate bool BackEvent();
	public event BackEvent OnBack;
	
	// Settings
	[HideInInspector]
	public Vector3 backLocation;
	[HideInInspector]
	public Quaternion backRotation;
	[HideInInspector]
	public bool goBack = false;

	public bool backable;
	
	public ParticleSystem respawnEffect;
	private ParticleSystem _effect;
	
	void Start () {
		backLocation = transform.position;
		backRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void LateUpdate () {

		if(!FadeInOut.IS_FADED_OUT && LevelControl.instance.playerDead){
			if (backable && goBack) {

				goBack = false;

				if (OnBack == null || OnBack()) {
					
					transform.position = backLocation;
					transform.rotation = backRotation;
				}
				
				if(gameObject.tag == "Player")
					StartCoroutine(RespawnPlayer());
			}

			if(_effect != null && !LevelControl.instance.playerDead){


				if(_effect.particleCount == 0)
					Destroy(_effect);
			}
		}else{

		}
	}

	public void GoBack(){

		gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
		gameObject.GetComponent<SpriteRenderer>().enabled = false;

		// .. stop the camera tracking the player
		if(gameObject.tag == "Player"){
			Camera.main.GetComponent<CameraFollow>().enabled = false;

			LevelControl.instance.playerDead = true;

			Debug.Log(GameControl.instance.currentHealthPoints);
		}

		goBack = true;
	}
	
	private IEnumerator RespawnPlayer(){

		// ... instantiate the splash where the player falls in.
		_effect = ((ParticleSystem)Instantiate(respawnEffect, backLocation, respawnEffect.transform.rotation));
		
		_effect.gameObject.SetActive(true);
		
		//GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		Vector3 resetCameraPos = new Vector3 (backLocation.x, backLocation.y + 2f, Camera.main.transform.position.z);
		
		Camera.main.transform.position = resetCameraPos;
	
		yield return new WaitForSeconds(_effect.duration/1.5f);
	
		gameObject.GetComponent<Rigidbody2D>().isKinematic = false;
		gameObject.GetComponent<SpriteRenderer>().enabled = true;

		Camera.main.GetComponent<CameraFollow>().enabled = true;

		LevelControl.instance.playerDead = false;
	}
}
