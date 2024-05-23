using UnityEngine;
using System.Collections;

public class MoveLightToTarget : MoveToTarget {

	WaveringLight _light;

	IgnoreObjectCollision _ignore;

	void Start(){

		_light = GetComponent<WaveringLight>();

		_ignore = GetComponent<IgnoreObjectCollision>();
	}

	// Update is called once per frame
	new void Update () {
	
		if(unlockedBy.unlocked && !onTarget){
			_light.waveLight = false;
		}

		base.Update();

		if(onTarget && _ignore != null)
			if(_ignore.ignoreNow)
				_ignore.ignoreNow = false;

	}
}
