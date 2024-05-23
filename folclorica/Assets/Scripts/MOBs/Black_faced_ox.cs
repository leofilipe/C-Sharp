using UnityEngine;
using System.Collections;

public class Black_faced_ox : MobIA {

	public float wait = .5f;
	public float elapsedTime {get; private set;}

	new void FixedUpdate () {

		if(turnSprite && elapsedTime < wait){
			elapsedTime += Time.deltaTime;
		}
		else{
			elapsedTime = 0f;
			base.FixedUpdate();
		}
	}
}
