using UnityEngine;
using System.Collections;

public class Tirolesa : MonoBehaviour {

	public SliderJoint2D slider;

	void Start(){
		slider = GetComponent<SliderJoint2D> ();
	}

	void OnCollisionStay2D(Collision2D coll) {
		if(coll.gameObject.tag == "tirolesa")
			slider.enabled = true;
	}

	void OnCollisionExit2D(Collision2D coll) {
		if(coll.gameObject.tag == "tirolesa")
			slider.enabled = false;
	}
}
