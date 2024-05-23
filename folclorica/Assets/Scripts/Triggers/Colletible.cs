using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Colletible : MonoBehaviour {

	public Collider2D collectedBy;

	public int points = 2;
	public bool isSpecial = false;	//set this flag to true if the colletible must account for the colleted item list
	public AudioSource audio;

	private ParticleSystem _effect;
	private Collider2D _collider;


	void Awake(){

		ParticleSystem[] effects = gameObject.GetComponentsInChildren<ParticleSystem>();

		_collider = gameObject.GetComponent<Collider2D>();

		if(effects != null && effects.Length > 0){
			_effect = effects[effects.Length - 1];

		}
		else
			_effect = null;
	}
	
	// Update is called once per frame
	void LateUpdate () {
	
		if(!_collider.enabled && _effect != null){

			if(_effect.particleCount == 0)
				gameObject.SetActive(false);
		}
	}

	void OnTriggerEnter2D(Collider2D col){

		if(col != collectedBy)
			return;

		if(audio != null)
			audio.PlayOneShot(audio.clip);
		else
			Debug.Log("No audio for game object: " + gameObject.name + " in ");
		Debug.Log(col.name);

		_collider.enabled = false;

		LevelControl.instance.AddScore(points);

		if(_effect != null){

			//gameObject.

			_effect.enableEmission = false;

			List<SpriteRenderer> renderes = new List<SpriteRenderer>();
			renderes.AddRange(GetComponents<SpriteRenderer>());
			renderes.AddRange(GetComponentsInChildren<SpriteRenderer>());

			if(renderes.Count > 0){
				foreach(SpriteRenderer render in renderes){

					if(render.gameObject != _effect){
						render.enabled = false;
					}
				}
			}else{
				List<ParticleSystemRenderer> particles = new List<ParticleSystemRenderer>();

				particles.AddRange(GetComponents<ParticleSystemRenderer>());
				particles.AddRange(GetComponentsInChildren<ParticleSystemRenderer>());

				foreach(ParticleSystemRenderer particle in particles){
					
					if(particle != _effect.GetComponent<ParticleSystemRenderer>()){
						particle.enabled = false;
					}
				}
			}
		}else{

			//adicionar pontos ao score do jogador
			gameObject.SetActive(false);
		}

	}
}
