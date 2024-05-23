using UnityEngine;
using System.Collections;

public class SortingLayerChange2 : MonoBehaviour {

	public string compareOnLayer;

	public string defaultSortingLayer;
	public int defaultLayerOrder;

	public string targetSortingLayer;
	public int targetLayerOrder;

	private string _currentSortingLayer;
	private int _currentLayerOrder;

	private SpriteRenderer _thisRenderer;

	// Use this for initialization
	void Start () {
	
		_thisRenderer = gameObject.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other){
		SwitchLayer(other);
	}

	void OnCollisionEnter2D(Collision2D other){
		SwitchLayer(other.collider);
	}

	void SwitchLayer(Collider2D other){

		if(LayerMask.LayerToName(other.gameObject.layer) != compareOnLayer)
			return;

		Renderer renderer = other.gameObject.GetComponentInParent<SpriteRenderer>();

		if (renderer == null)
			return;

		if(renderer.sortingLayerName != _currentSortingLayer){

			if(renderer.sortingLayerName == defaultSortingLayer){

				_currentSortingLayer = defaultSortingLayer;
				_currentLayerOrder = defaultLayerOrder;


			}
			else if(renderer.sortingLayerName == targetSortingLayer){

				_currentSortingLayer = targetSortingLayer;
				_currentLayerOrder = targetLayerOrder;

			}

			//Debug.Log(renderer.gameObject);
			_thisRenderer.sortingLayerName = _currentSortingLayer;
			_thisRenderer.sortingOrder = _currentLayerOrder;
		}
	}


}
