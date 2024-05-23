using UnityEngine;
using System.Collections;

public class TurnGameObject : MonoBehaviour {

	public GameObject target;

	// Use this for initialization
	void Start () {
	
		if(target != null){

			Vector3 scale = target.transform.localScale;
			scale.x = -scale.x;

			target.transform.localScale = scale;
		}
			//target.transform.sc = -target.transform.localScale.x;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
