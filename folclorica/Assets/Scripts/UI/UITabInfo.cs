using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Linq;

public class UITabInfo : MonoBehaviour {

	public Text player;
	public Text health;
	public Text colletibles;
	public Text level;
	public Text score;
	public Text area;

	/// <summary>
	/// Updates the data of the fields of the "Resumo" tab of the pause menu.
	/// </summary>
	public void UpdateFields(){

		try{

			//Update player's username on the "Resumo" tab in the pause menu
			player.text = "Jogador(a): " + GameControl.instance.username;

			//Update player's current number of hearts on the "Resumo" tab in the pause menu
			health.text = "Vidas: " + (float)GameControl.instance.currentHealthPoints / 2; 

			//Update player's total number of colletibles flaged as "special"
			//and display's it on the "Resumo" tab in the pause menu.		
			//retrieve the colletible text from the HUD
			string label = GameObject.FindObjectOfType<UIColletible>().label.text;
			
			//set all its characters to lowcase
			label = label.ToLower();
			
			//set its first character to uppercase
			label = label.First().ToString().ToUpper() + label.Substring(1);
			
			//set its value as the field's text
			colletibles.text = label;

			//Update player's current level on the "Resumo" tab in the pause menu.			
			//retrieve the level text from the HUD
			level.text = GameObject.FindObjectOfType<UILevel>().label.text;

			//Update player's current level on the "Resumo" tab in the pause menu.
			//retrieve the value from the UIScore class and add the numerical value to this field
			score.text = "Pontuação: " + GameObject.FindObjectOfType<UIScore>().label.text.Split(' ')[1];

			//Update's the name of the area of the game that this level takes place.
			//retrieve the level text from the HUD
			area.text = GameObject.FindObjectOfType<UIArea>().label.text;
		}
		//if anything wrong happens, print the exception.
		catch(Exception ex){
			Debug.Log("Erro em " + gameObject.name + ": " + ex.ToString() + " " + ex.StackTrace);
		}
	}
}
