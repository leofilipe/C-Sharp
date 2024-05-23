using UnityEngine;
using System.Collections;

public class FadeSceneDueHeight : FadeScene {
	
	// Update is called once per frame
	protected override void LateUpdate () {
		HeightFaderCheck();
		base.LateUpdate();

	}

	//keep it empty to prevent parent's execution
	/*protected override void OnTriggerEnter2D(Collider2D other){

	}
	//keep it empty to prevent parent's execution
	protected override void OnTriggerExit2D(Collider2D other){

	}*/

	/// <summary>
	/// Fades to black or clear's the screen depending on target's position in comparison
	/// to the fader's position
	/// </summary>
	private void HeightFaderCheck(){

		//if it is not switching right now
		if(_singleExecution > 0)
			return;

		//if is is not set to switch
		if(!_switchAlpha){

			//if SkyColor is not Black then the action is to turn it black (fadeout)
			_isFadeOut = RenderSettings.ambientSkyColor != Color.black;

			//if target position is greater than the object's, then fade to white
			if(fadeTrigger.transform.position.y > transform.position.y){

				//set true if sky is black
				_switchAlpha = RenderSettings.ambientSkyColor == Color.black;
			}
			//if target position is lower than the object's, then fade to black
			else {
				//set true if sky is not black 
				_switchAlpha = RenderSettings.ambientSkyColor != Color.black;
			}
		}
	}
}
