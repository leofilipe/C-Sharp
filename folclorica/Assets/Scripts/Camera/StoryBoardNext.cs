using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class StoryBoardNext : MonoBehaviour {

	public float waitToNext = 5f;	//time to wait before enabling the next button between each step.

	//public Dictionary <int, List<float>> waitText;		//how long 

	public float[][] waitText;

	public CameraFollowPath sbPath;

	public Button btNext;

	private float _waitStep;

	private float[] _waitText;

	private int _step = 0;

	private int _previousStep = -1;

	// Use this for initialization
	void Start () {
	
		btNext.interactable = false;
		_waitStep = waitToNext;


	}
	
	// Update is called once per frame
	void Update () {
	
		if (sbPath.onTarget){// && _previousStep == _step - 1){
			sbPath.Type = FollowPath.FollowType.MoveTowards;
			_waitStep = waitToNext;
			btNext.interactable = false;
			Debug.Log(_step + ": on target" + btNext.interactable);
			//_previousStep = _step;
			//_step++;
		}else if(_waitStep >= 0){
			_waitStep -= Time.deltaTime;
			btNext.interactable = false;
			Debug.Log(_step + ": duracao > 0" + btNext.interactable);
		}else if(_waitStep <= 0 && !btNext.interactable){
			btNext.interactable = true;
			Debug.Log(_step + ": duracao < 0" + btNext.interactable);
		}

	}

	public void NextFrame(){

		btNext.interactable = false;
		sbPath.Type = FollowPath.FollowType.Lerp;
		sbPath.UpdateCurrentSpeed(10f);
		_waitStep = waitToNext;
	}
}
