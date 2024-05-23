using UnityEngine;
using System.Collections;

public class PauserOnBios : Pauser {

	public GameObject [] disablePanels;
	public GameObject panelCharDesc;

	public UIUnlockable[] biosButtons;

	// Update is called once per frame
	protected new void Update () {
	
		//if player is alive
		if(GameControl.instance.currentHealthPoints > 0){
			//if the P or Esc key are pressed on the keyboard
			if(Input.GetKeyUp(KeyCode.P) || Input.GetKeyUp(KeyCode.Escape)){
				//pause the game
				PauseGame();
			}
			
			//if the game is paused
			if(isPaused)
				Time.timeScale = 0;	//set the time scale to zero in order to suspend all game processes
			//otherwise
			else
				Time.timeScale = 1;	//set the time scale to one in order to retake all game processes
		}
	}
}
