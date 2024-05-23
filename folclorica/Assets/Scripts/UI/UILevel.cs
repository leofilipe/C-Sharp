using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UILevel : MonoBehaviour {

	//the text component on the HUD
	public Text label{get; private set;}

	// Use this for initialization
	void Start () {

		//get the Text component
		label = GetComponent<Text>();

		//Update player's current level on the "Resumo" tab in the pause menu.			
		//retrieve the value from the level control class and set it as the field's value
		label.text = "Fase: " + LevelControl.instance.levelID;
	}
}
