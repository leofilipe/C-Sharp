using UnityEngine;
using System.Collections;

public class CandleLight : TorchLight {

	public Transform collectibleEyes;
	
	private ParticleSystem particleSys;

	// Use this for initialization
	void Start () {
	
		particleSys = transform.GetComponentInChildren<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
	
		if(torchLight.enableEmission){

			Light[] eyes = collectibleEyes.GetComponentsInChildren<Light>();

			Vector3 targetEye = Vector3.zero;

			float distance = float.MaxValue;
			
			foreach(Light eye in eyes){
				
				if(eye.enabled){
					
					float aDistance = Vector3.Distance(transform.position, eye.transform.position);
					
					if(aDistance < distance){
						targetEye = eye.transform.position;
						distance = aDistance;
					}
				}
			}
			
			if(distance != float.MaxValue){

				//Vector3 targetDirection = targetEye - transform.position;


				particleSys.transform.LookAt(targetEye);
			}else{

				particleSys.transform.localRotation = Quaternion.Euler(0, 0, 0);

			}
		}
	}
}
