using UnityEngine;
using System.Collections;

public class TriggerChangeSound : MonoBehaviour {
	
	public GameObject triggeredBy;
	public AudioSource startsound;
	public AudioSource stopsound;

	public float fadeTime = 5.0F;

	enum Fade {In, Out};

	bool enterTrigger = false;
	bool changeAudio = true;
	bool changeAudio2 = true;

	bool loop = true;
	
	public bool doOnce = false;
	
	void Awake(){

	}


	void Update () {
		if(stopsound.volume == 0){
			stopsound.Stop();
			stopsound.playOnAwake = false;
		}
		if (enterTrigger && changeAudio){
			StartCoroutine(FadeAudio(fadeTime, Fade.Out, stopsound, false));

		}


		if (enterTrigger && changeAudio2){
				StartCoroutine(FadeAudio(fadeTime, Fade.In, startsound, true));
				if(!startsound.isPlaying)
				{
					startsound.Play();
					startsound.playOnAwake = true;
				}
			}

	}


	void OnTriggerEnter2D(Collider2D col){
		
		if(col.gameObject == triggeredBy && loop){

			enterTrigger = true;

			loop = !doOnce;
			Debug.Log("Disparou");
			
			
		}
	}


	IEnumerator FadeAudio (float timer, Fade fadeType, AudioSource sound, bool isStartsound) {

		//Debug.Log("Baixando audio...");

		if(! isStartsound)
			changeAudio = false;
		else
			changeAudio2 = false;

		float start = fadeType == Fade.In? 0.0F : 1.0F;
		float end = fadeType == Fade.In? 1.0F : 0.0F;
		float i = 0.0F;
		float step = 1.0F/timer;
		
		while (i <= 1.0F) {
			i += step * Time.deltaTime;
			sound.volume = Mathf.Lerp(start, end, i);


			yield return new WaitForSeconds(step * Time.deltaTime);

			if(sound.volume > 0 && fadeType == Fade.Out){
				changeAudio = true;
			}
			else if(sound.volume < 1 && fadeType == Fade.In)
				changeAudio2 = true;
		}

		enterTrigger = false;
	}
	/*
	IEnumerator FadeAudio2(float timer, Fade fadeType){
			float start2 = fadeType == Fade.In? 0.0F : 1.0F;
			float end2 = fadeType == Fade.In? 1.0F : 0.0F;
			float i2 = 0.0F;
			float step2 = 1.0F/timer;
			
			while (i2 <= 1.0F) {
				i2 += step2 * Time.deltaTime;
				stopsound.volume = Mathf.Lerp(start2, end2, i2);
				yield return new WaitForSeconds(step2 * Time.deltaTime);
			}

	
	
}*/

	}

