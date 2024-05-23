using UnityEngine;
using System.Collections;

public class FadeScene : MonoBehaviour {
	
	
	public float fadeInTime = 0.5f;					//default time to fade in the screen
	public float fadeOutTime = 2f;					//default time do fade out the screen
	
	public GameObject fadeTrigger;					//the object that activates the fading
	public bool RemoveFaderTransparencyWhenDone = true;

	private Collider2D _triggerSingleCollider;		//used to ensure only one of the trigger object colliders will trigger the fade.
	protected bool _isFadeOut; 						//indicates that the screen is currently fading. 
													//Must be used in conjuction with _switchAlpha
					
	protected bool _switchAlpha = false;				//acts as a trigger to avoid more than one fade at a time or in sucession
													//more than one fade at a time or in sucession

	//private FadeInOut _fadeInOut;					//The fader instance
	private Vector2 _triggerEnterPosition;			//the position of the trigger object when entering the trigger. Used together with  
													//_triggerExitPosition to check if the object did not got in and out through the same way

	private Vector2 _triggerExitPosition;			//object position when exiting the trigger.

	protected int _singleExecution = 0;				//added as a last safeguard against problems created by the rope packcage
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	protected virtual void Start(){

		//if the object trigger was not set, use the player as default.
		if(fadeTrigger == null)
			fadeTrigger = GameObject.FindGameObjectWithTag("Player");

		//there MUST be a collider
		_triggerSingleCollider = fadeTrigger.GetComponents<Collider2D>()[0];

		//Debug.Log(fadeTrigger.name + " " + colliderType.GetType());
		//gets the single instance of FadeInOut
		//_fadeInOut = FadeInOut.GetInstance();
	}
	
	protected virtual void LateUpdate(){

		//No need for this. Collision removed on editor.
		//Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("Climber"));

		//if must change the screen
		if(_switchAlpha){
			
			_singleExecution++;
			
			StartCoroutine(SwitchAlpha());
		}	
	}
	

	protected virtual void OnTriggerEnter2D(Collider2D other){
		
		//verifica se o collider eh um trigger ou de outro
		//tipo que nao BoxCollider2D. A segunda restricao
		//se deve ao player possuir dois colliders e querermos
		//reallizar a operacao apenas uma vez
		if(other != _triggerSingleCollider)
			return;
		
		if(!_switchAlpha){
			
			_triggerEnterPosition = other.gameObject.transform.localPosition;
		}
		
	}
	
	protected virtual void OnTriggerExit2D(Collider2D other){
		
		if(_singleExecution > 0)
			return;
		//verifica se o collider eh um trigger ou de outro
		//tipo que nao BoxCollider2D. A segunda restricao
		//se deve ao player possuir dois colliders e querermos
		//reallizar a operacao apenas uma vez
		if(other != _triggerSingleCollider)
			return;
		
		if(!_switchAlpha){
			_triggerExitPosition = other.gameObject.transform.position;
			
			//if SkyColor is not Black then the action is to turn it black (fadeout)
			_isFadeOut = RenderSettings.ambientSkyColor != Color.black;
			
			_switchAlpha = Vector3.Distance(_triggerEnterPosition, _triggerExitPosition) > 2f;
		}
	}
	
	public void StartFade(){
		
		_isFadeOut = RenderSettings.ambientSkyColor != Color.black;
		_switchAlpha = true;
	}

	IEnumerator SwitchAlpha(){
		
		
		//set to false as the screen is already changing
		_switchAlpha = false;
		
		//if is already executing, return
		//NA PRATICA ISTO NAO FAZ NADA, TROCAR - LEONARDO
		/*if(_singleExecution > 1)
			yield return new WaitForSeconds(0f);*/
		
		//if it is not executing yet
		if(_singleExecution <= 1){
			//if should fade out
			if(_isFadeOut){
				
				//places a black sprite in front of the camera
				FadeInOut.FadeOutMain(fadeOutTime);
				
				yield return new WaitForSeconds(fadeOutTime/4);
				//set the sky black
				RenderSettings.ambientSkyColor = Color.black;
				Camera.main.backgroundColor = Color.black;
				
				//remove the black sprite from the front of the camera
				if(RemoveFaderTransparencyWhenDone)
					FadeInOut.FadeInMain(fadeInTime);
				_singleExecution = 0;
				
			}
			//if should fade in and player has any life point
			else if (GameControl.instance.currentHealthPoints > 0){
				
				//immediatily places a black sprite in front of the camera and brings its alpha to 0
				FadeInOut.FadeInMain(fadeInTime);
				
				yield return new WaitForSeconds(fadeInTime/4);
				//set the sky to its original color
				RenderSettings.ambientSkyColor = FadeInOut.SKY_COLOR;
				Camera.main.backgroundColor = FadeInOut.CAMERA_BACKGROUND;
				_singleExecution = 0;
				
			}
		}	
		
	}
}