using UnityEngine;
using System.Collections;

public class PlaySoundWhenNear : MonoBehaviour {

	public GameObject target;
	public AudioSource audioSource;

	public float minDistance;
	public float maxDistance;
	public float maxVolume = 1f;

	public float loopDelay;

	float _wait;

	bool isChanging;

	Vector2 targetPos;
	Vector2 objPos;

	//private float
	// Use this for initialization
	void Start () {
	
		objPos = new Vector2();
		targetPos = new Vector2();

		if(maxVolume > 1)
			maxVolume = 1;
		else if (maxVolume < 0)
			maxVolume = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
		targetPos.x = target.transform.position.x;
		targetPos.y = target.transform.position.y;

		objPos.x = transform.position.x;
		objPos.y = transform.position.y;

		float distance = (targetPos - objPos).sqrMagnitude;
		//Debug.Log(distance);

		float volume = 0;

		if(distance > maxDistance){
			volume = 0;
		}else if (distance < maxDistance && distance > minDistance){//if (distance <= minDistance){
			volume = minDistance/ Mathf.Abs(distance - minDistance);
			//Debug.Log(volume);
		}else {
			volume = maxVolume;
		}

		if (!isChanging && _wait <= 0){

			StartCoroutine(ChangeVolume(audioSource.volume, volume));
			_wait = loopDelay;
		}

		_wait -= Time.deltaTime;

	}

	IEnumerator ChangeVolume(float fromVolume, float toVolume){

		if(!audioSource.isPlaying)
			audioSource.Play();


		if(fromVolume != toVolume){
			isChanging = true;

			float dif = Mathf.Abs(fromVolume - toVolume);
			float step = dif/.3f;//toVolume > 0? toVolume/fromVolume : 0;

			float i = 0.0F;

			while (i <= dif) {
				i += step * Time.deltaTime;
				audioSource.volume = Mathf.Lerp(fromVolume, toVolume, i);
				
				
				yield return new WaitForSeconds(step * Time.deltaTime);

			}

			isChanging = false;
		}
	}
}
