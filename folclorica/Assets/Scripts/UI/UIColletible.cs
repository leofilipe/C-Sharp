using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIColletible : MonoBehaviour {

	//list of colletibles to be acounted for the HUD
	private List<GameObject> _colletibles;

	//the text component on the HUD
	public Text label{get; private set;}

	// Use this for initialization
	void Start () {
	
		//get the Text component
		label = GetComponent<Text>();
		//get the list of colletibles flaged as special
		_colletibles = LevelControl.instance.specialColletibles;
	}
	
	// Update is called once per frame
	void LateUpdate () {
	
		label.text = "Itens Coletados: ";

		int count = 0;

		foreach(GameObject colletible in _colletibles){

			if(!colletible.GetComponent<Collider2D>().enabled)
				count++;

		}

		label.text += count > 10? count.ToString() : ("0" + count.ToString());

		label.text += " de " + _colletibles.Count;
	}
}
