using UnityEngine;
using System.Collections;

public class TorchLight : MonoBehaviour {

	/// <summary>
	/// The light object for the torch.
	/// </summary>
	public new Light light;

	/// <summary>
	/// The particle system for the torch.
	/// </summary>
	public ParticleSystem torchLight;

	/// <summary>
	/// Whether or not to use a time limit for the light emission.
	/// </summary>
	public bool timerEnabled = true;

	/// <summary>
	/// The duration for the light emission.
	/// </summary>
	public float lightOffTimer = 3f;

	//random value to dim the light and create a live fire light effect
	protected float rnd_Luminosity;

	//The elapsed time of light emission for when using a timer
	private float countDown;

	//The trigger that sets the light on and off when the timer is at use
	private bool onTrigger = false;

	private float _minIntensity;



	/*
		Configuracoes interessantes da tocha
			- Transform/Scale - x 1.5, y 6 , z 1 (para uso com no puzzle da fase 01)
			- Particle System - Start size: 1; Shape - Sphere - Radious: 0.08

			- Transform/Scale - x 2, y 5 , z 1 
			- Particle System - Start size: 0.8; Shape - Sphere - Radious: 0.07
	 */
	// Use this for initialization
	void Start () {
	
		this.light.enabled = !timerEnabled;
		torchLight.enableEmission = !timerEnabled;
		countDown = lightOffTimer;
		_minIntensity = light.intensity / 3;
		//rnd_Luminosity = Random.Range(_minIntensity, light.intensity);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
		if(timerEnabled){

			if(!onTrigger && this.light.enabled)
				countDown -= Time.deltaTime;
			
			//se a contagem regressiva for menor ou igual a duracao do
			//efeito de particula
			if(countDown <= torchLight.duration)
				//desative a emissao de particulas
				torchLight.enableEmission = false;
			
			//se a contagem regressiva for menor ou igual a zero
			if(countDown <= 0)
				//desative a fonte de luz
				this.light.enabled = false;
		}

		if(this.light.enabled){
			light.intensity = _minIntensity * Mathf.PerlinNoise(_minIntensity + Time.time, _minIntensity + 1 + Time.time*1) // may range from 0 to _halfIntensity
				+ _minIntensity; //min secure luminosity;

		}
	}
	
	
	void OnTriggerStay2D(Collider2D coll){

		if(timerEnabled && coll.tag == "Player"){

			this.light.enabled = true;
			torchLight.enableEmission = true;
			onTrigger = true;
			
			countDown = lightOffTimer;
		}

	}

	void OnTriggerExit2D(Collider2D coll){

		if(timerEnabled && coll.tag == "Player")
			onTrigger = false;
	}
}
