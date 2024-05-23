using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace beffio.OneMinuteGUI {
	public class MenuManagerLevel : MenuManager {

		//public

		private GameObject lockedButton;				//A type of button that must be unlocked through ingame actions

		/// <summary>
		/// GoToLockedMenu. Used to go to specific menus that require an INGAME
		/// unlocking action. Due the MenuManager Class taking no more than one
		/// parameter on the unity editor, the lockedButton parameter is used to
		/// set the UI caller element, while target represents the screen to be
		/// displayed if the caller lock has been undonde.
		/// The [lockedButton] parameter is set as null at the end of the method  
		/// and must have a value given to it before each call to this method.
		/// </summary>
		/// <param name="target">Target. The parent game object containing all 
		/// screens corresponding to the previously set [lockedButton]</param>
		public void GoToLockedMenu(GameObject lockedParent){
			
			//if the locked button was not set for this call
			if(lockedButton == null){
				
				//display the error on the console and exit
				Debug.Log("You need first to set the caller locked button");
				return;
			}
			
			//get the UIUnlockable component of the button and check if
			
			//it has been unlocked
			if(lockedButton.GetComponent<UIUnlockable>().unlocked){
				
				//make sure that no screen with lockable information is enabled
				for(int i = 0; i < lockedParent.transform.childCount; i++)
					lockedParent.transform.GetChild(i).gameObject.SetActive(false);
				
				//retrieve the desired child by its name
				Transform lockedChild = lockedParent.transform.FindChild(lockedButton.name);
				
				//if it has been found
				if(lockedChild != null){
					
					//activate just this child on its parent
					lockedChild.gameObject.SetActive(true);
					
					//display its parent menu
					GoToMenu(lockedParent);
				}
				//if it was not found
				else{
					//display the error on the console, set the control button
					//as null and exit
					Debug.Log("Painel no menu de destino nao encontrado...");
					lockedButton = null;
					return;
				}
				
				//reach for the parent panel the UIButton is in...
				Transform parent = lockedButton.transform.parent;
				
				//... run a loop if needed be
				while(!parent.name.Contains("panel")){
					parent = parent.parent;
				}
				
				//remove the parent panel from view
				parent.gameObject.SetActive(false);
				
				//set the control button as null and exit
				lockedButton = null;
			}
		}
		
		/// <summary>
		/// Reloads the game level from the last saved checkpoint upon player's death
		/// </summary>
		public void LoadFromLastCheckpoint(){
			
			//retrieve the saving instance
			SaveAssistant saveAssistant = GameControl.instance.saveAssistant;
		
			LevelData level = saveAssistant.LoadSaveFile();
			level.PrintData();

			UITipDialog.ResetDialogs();

			Application.LoadLevel(Application.loadedLevel);

		}
		
		/// <summary>
		/// Loads the level from start. Restarts the level discarding all of the user's 
		/// saved data and replacing then.
		/// </summary>
		public void LoadRestartedLevel(){
			
			GameControl.instance.RestartLevel = true;

			UITipDialog.ResetDialogs();

			GameControl.instance.currentHealthPoints = LevelControl.instance.defaultHealthPoints;

			GameControl.instance.maxHealthPoints = LevelControl.instance.defaultMaxHealthPoints;

			Pauser.instance.ForceGameSuspension(false);

			Application.LoadLevel(Application.loadedLevel);
			
		}
		
		public void SetCallerLockedButton(GameObject lockedButton){
			this.lockedButton = lockedButton;
		}

		public void PauseGame(){
			Pauser.instance.PauseGame();
		}

		public void Wait(float duration/*, string msg, Text textObj*/){

			/*float endWait = Time.realtimeSinceStartup + duration;

			while (Time.realtimeSinceStartup < endWait){
				/*if(textObj != null && msg != null)
					textObj.text = msg;*/
			//}

			Debug.Log("End wait....");
			/*while (duration >= 0){
				duration -= Time.deltaTime;
			}*/
		}
	}
}
