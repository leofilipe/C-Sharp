using UnityEngine;
using System.Collections;

public class WaveringLight : TorchLight {

	public bool randomLightIntensity = true;

	[HideInInspector]
	public bool waveLight;

	Vector3 startingPosition;
	// Use this for initialization
	void Start () {
		
		SetInitialPosition();

		waveLight = this.light.enabled;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if(waveLight){

			if(randomLightIntensity)
				light.intensity = 2*Mathf.PerlinNoise(rnd_Luminosity + Time.time, rnd_Luminosity + 1 + Time.time*1);

			float x = Mathf.PerlinNoise(rnd_Luminosity + 0 + Time.time*2, rnd_Luminosity + 1 + Time.time*2) - 0.5f;
			float y = Mathf.PerlinNoise(rnd_Luminosity + 2 + Time.time*2, rnd_Luminosity + 3 + Time.time*2) - 0.5f;
			//Mathf.PerlinNoise(rnd_Luminosity + 4 + Time.time*2, rnd_Luminosity + 5 + Time.time*2) - 0.5f;
			transform.localPosition = startingPosition + Vector3.up + new Vector3(x, y, 0f)*1;
			
		}
	}

	public void SetInitialPosition(){

		rnd_Luminosity = Random.Range(light.intensity/2, light.intensity);
		
		startingPosition = transform.localPosition;
	}
}
