using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Collections;
using UnityEngine.Events;

namespace beffio.OneMinuteGUI {

	public class MenuManager : MonoBehaviour {

		public string defaultLoadLevel = "Scene 01a - Cutscene";

		public string AnimationPropertyName {			//Name for the default animation being used
        	get {
            	return m_animationPropertyName;
        	}
			set {
            	m_animationPropertyName = value;
        	}
    	}

		public GameObject InitialScreen {				//The first menu or game object to be displayed
	        get {
	            return m_initialScreen;
	        }
	        set {
	            m_initialScreen = value;
	        }
	    }

		public List<GameObject> NavigationHistory {		//Navigation history used for the "go back" operation
	        get {
	            return m_navigationHistory;
	        }

	        set {
	            m_navigationHistory = value;
	        }
	    }
	
		[SerializeField]
		private string m_animationPropertyName;			//internal serializable value for the name of the default 
														//animation being used. Set by AnimationPropertyName

		[SerializeField]
		private GameObject m_initialScreen;				//internal serializable value for the name of the first menu 
														//or game object to be displayed. Set by InitialScreen

		[SerializeField]
		private List<GameObject> m_navigationHistory;	//internal serializable value for the first menu or game object 
														//to be displayed. Set by InitialScreen

		//METHODS --------------------------

		public void OpenURL(string url){
			Application.OpenURL(url);
		}

		/// <summary>
		/// GoBack. Return to the previous screen.
		/// </summary>
		public void GoBack(){
			//if there are entries on the navigation history
			if (m_navigationHistory.Count > 1)
			{
				//get the last index
				int index = m_navigationHistory.Count - 1;

				//display the previous screen
				Animate(m_navigationHistory[index - 1], true);

				//get the current screen
				GameObject target = m_navigationHistory[index];

				//remove it from history
				m_navigationHistory.RemoveAt(index);

				//remove it from view
				Animate(target, false);
			}
		}

		/// <summary>
		/// GoToMenu. Go to the menu indicated by target
		/// </summary>
		/// <param name="target">The game menu to go to.</param>
		public void GoToMenu(GameObject target){

			//if there is no target menu, inform it and do nothing
			if (target == null){
				Debug.Log("No target menu to go to...");
				return;
			}

			//if there are itens on the navigation history
			if (m_navigationHistory.Count > 0){
				//remove the current screen from view
				Animate(m_navigationHistory[m_navigationHistory.Count - 1], false);
			}

			//add the current screen to the navigation history
			m_navigationHistory.Add(target);
			//display it
			Animate(target, true);
		}

		public void LoadDefaultLevel(){

			Application.LoadLevel(defaultLoadLevel);
		}

		public void Quit(){
			Application.Quit();
		}

		private void Animate(GameObject target, bool direction)	{
			if (target == null)	{
				return;
			}

			target.SetActive(true);

			//Debug.Log("target --> " + target.gameObject.name);

			Canvas canvasComponent = target.GetComponent<Canvas>();
			if (canvasComponent != null){
				canvasComponent.overrideSorting = true;
				canvasComponent.sortingOrder = m_navigationHistory.Count;
			}

			Animator animatorComponent = target.GetComponent<Animator>();
			if (animatorComponent != null){
				animatorComponent.SetBool(m_animationPropertyName, direction);
			}
		}

		private void Awake(){
			m_navigationHistory = new List<GameObject>{m_initialScreen};
			//Debug.Log("Awake menu");
		}
	}
}
