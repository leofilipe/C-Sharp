using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIArea : MonoBehaviour {

	//the text component on the HUD
	public Text label{get; private set;}

	// Use this for initialization
	void Start () {
		
		//get the Text component
		label = GetComponent<Text>();

		//Update's the name of the area of the game that this level takes place.
		//retrieve the value from the level control class and set it as the field's value
		label.text = "Região: " + LevelControl.instance.levelName;
	}
}
