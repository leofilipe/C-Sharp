using UnityEngine;
using System.Collections;

public class ResetWhenLost : MonoBehaviour {

	public Transform targetReferencial;

	public bool useY = true;

	private Vector3 _startPosition;
	private Quaternion _startRotation;

	// Use this for initialization
	void Start () {
	
		_startPosition = gameObject.transform.position;
		_startRotation = gameObject.transform.rotation;


	}
	
	// Update is called once per frame
	void Update () {
	
		if(useY){
			if(targetReferencial.position.y > transform.position.y){

				Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
				rb.isKinematic = true;
				rb.velocity = Vector2.zero;

				gameObject.transform.rotation = _startRotation;
				gameObject.transform.position = _startPosition;

				rb.isKinematic = false;
			}
		}else{
			//TODO this part needs testing
			if(targetReferencial.position.x < transform.position.x){

				Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
				rb.isKinematic = true;
				rb.velocity = Vector2.zero;
				
				gameObject.transform.rotation = _startRotation;
				gameObject.transform.position = _startPosition;
				
				rb.isKinematic = false;
			}
		}
	}


}
