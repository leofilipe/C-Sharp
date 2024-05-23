using UnityEngine;
using System.Collections;

public class TriggerChangeCamFieldOfView : MonoBehaviour {

	public Collider2D triggeredBy;

	public float transitionTimer;

	public float reverseAfter;

	public float tempFOV;

	private Camera _camera;

	private float _FOV;

	private bool _changeFOV;

	public static bool IsChanging;

	// Use this for initialization
	void Start () {
	
		//get the main camera and its default field of view (FOV)
		_camera = Camera.main;
		_FOV = Camera.main.fieldOfView;
	}
	
	// Update is called once per frame
	void Update () {

		//if FOV is not changing
		if(!IsChanging){

			//if the current FOV is the new one, then it has already changed.
			//check for reversion
			if (_camera.fieldOfView == tempFOV){

				//decrement the reverse time
				reverseAfter -= Time.deltaTime;

				//if the reverse time as expired, begin reversion
				if(reverseAfter <= 0){

					//start to change the FOV from the new value to the original one
					StartCoroutine(ChangeView(transitionTimer, _camera.fieldOfView, _FOV));
				}
			}
			//else if the FOV is the original one and should change
			else if (_changeFOV){

				//start to change the FOV from the original value to the new one
				StartCoroutine(ChangeView(transitionTimer, _camera.fieldOfView, tempFOV));
			}
		}
	}

	/// <summary>
	/// Changes the view.
	/// </summary>
	/// <returns>The view.</returns>
	/// <param name="timer">Timer.</param>
	/// <param name="fromField">From field.</param>
	/// <param name="toField">To field.</param>
	IEnumerator ChangeView (float timer, float fromField, float toField) {

		//if there are several scripts of this type attatched to its object, then retrive all of them
		//and disable all but this one
		MonoBehaviour[] scripts = transform.parent.GetComponentsInChildren<TriggerChangeCamFieldOfView>();

		foreach (MonoBehaviour script in scripts)
			if(script != this)
				script.enabled = false;

		//if the current field of view is diferent from the new one
		if(fromField != toField){

			//flag it as changing
			IsChanging = true;

			//find the difference between the values and their step
			float dif = Mathf.Abs(fromField - toField);
			float step = dif/timer;
			
			float i = 0.0F;

			//while the difference has not been reached
			while (i <= dif) {

				//find the next increment
				i += step * Time.deltaTime;

				//lerp the view to it
				_camera.fieldOfView = Mathf.Lerp(fromField, toField, i);
				
				//wait before the next step
				yield return new WaitForSeconds(step * Time.deltaTime);
				
			}

			//hard set the value to get rid of small variances
			_camera.fieldOfView = toField;

			//flag it as not changing
			IsChanging = false;

			//flag that it should not change
			_changeFOV = false;

			//if has come to this point and the current field of view is the starting one,
			//then the camera has been restored to its default value, end the script.
			if(_camera.fieldOfView == _FOV)
				this.enabled = false;
		}
	}

	/// <summary>
	/// Raises the trigger stay2 d event.
	/// </summary>
	/// <param name="other">Other.</param>
	void OnTriggerStay2D(Collider2D other){

		//if the camera view is already changing, then do nothing
		if(IsChanging)
			return;
		
		//if the player was not the one who collided with 
		//the trigger
		if(other != triggeredBy)
			return;

		//if the object is the palyer and this script belongs to a rope
		if(triggeredBy.gameObject.tag == "Player" && gameObject.tag == "rope2D")
			if(triggeredBy.transform.parent == null) //do nothing unleast the player is on the rope
				return; 

		//flag that the view should change
		_changeFOV = true;

	}
}
