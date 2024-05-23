using UnityEngine;
using System.Collections;

public class SpawEffectAt : MonoBehaviour {

	public ParticleSystem effect;

	public Collider2D target;

	public float offSetX;

	public float offSetY;

	public Lock optionalLock;

	private ParticleSystem _effect;

	private ParticleSystem[] _effects;

	private float _duration;

	private bool _isPlaying;
	
	// Use this for initialization
	void Start () {
	
		//_effect = effect.gameObject.GetComponentsInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
	
		if(_isPlaying){

			_effect.transform.position = SpawnAt(target.transform.position);

			_duration -= Time.deltaTime;

			int particleCount = 0;

			foreach(ParticleSystem ps in _effects)
				particleCount += ps.particleCount;

			if(_duration <= 0 && particleCount == 0){

				AudioSource audio = _effect.GetComponentInChildren<AudioSource>();

				if(audio != null && audio.isPlaying){
					audio.Stop();
				}

				Destroy(_effect);

				_isPlaying = false;
			}
		}
	}

	public void PlayEffect(){

		Debug.Log("Playing effect...");




		_effect = ((ParticleSystem)Instantiate(effect, SpawnAt(target.transform.position), effect.transform.rotation));

		_effects = _effect.gameObject.GetComponentsInChildren<ParticleSystem>();

		_effect.gameObject.SetActive(true);

		_duration = _effect.duration;

		_isPlaying = true;

	}

	Vector3 SpawnAt(Vector3 position){

		Vector3 location = new Vector3();
		
		location.x = position.x - offSetX;
		location.y = position.y - offSetY;
		location.z = position.z;

		return location;
	}

	void OnTriggerStay2D(Collider2D col){

		bool unlocked = optionalLock == null;

		if(!unlocked)
			unlocked = optionalLock.unlocked;

		if(unlocked && !_isPlaying && col == target){

			PlayEffect();
		}
	}
}
