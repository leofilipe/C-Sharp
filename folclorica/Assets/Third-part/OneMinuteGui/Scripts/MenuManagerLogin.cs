using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace beffio.OneMinuteGUI {

	public class MenuManagerLogin : MenuManager {

		//PARAMETERS --------------------------
		public GameObject UnableConnectMenu;			//Unable to connect game object menu
		
		public GameObject SystemMsgMenu;				//System message menu
		public Text SystemMsgMenuText;					//System message menu text
		public Text SystemMsgMenuTitile;				//System message menu title
		public Button[] SystemMsgMenuButtons;		//System message menu bt ok


		public void FillUser(GameObject fieldContainer){
			
			//retrieve the input fields containing the user's login name and his password...
			InputField[] fields = fieldContainer.GetComponentsInChildren<InputField>();

			string username = SystemMsgMenuText.text.Substring(SystemMsgMenuText.text.LastIndexOf(" ") + 1);

			//... set these values to the GameControl object
			fields[0].text = username;
		}

		/// <summary>
		/// Shows the message menu with an ok button using the first available button
		/// </summary>
		/// <param name="parentWindow">Parent window.</param>
		/// <param name="title">Title of the message.</param>
		/// <param name="msg">Message to been show.</param>
		public void ShowMsgMenu(GameObject parentWindow, string title, string msg){
			
			ShowMsgMenu(parentWindow, title, msg, 0);
		}

		/// <summary>
		/// Shows the message menu with an ok button using the button specified by the index parameter
		/// </summary>
		/// <param name="parentWindow">Parent window.</param>
		/// <param name="title">Title.</param>
		/// <param name="msg">Message.</param>
		/// <param name="btIndex">Bt index.</param>
		public void ShowMsgMenu(GameObject parentWindow, string title, string msg, int btIndex){
			
			//set the text of the message
			SystemMsgMenuTitile.text = title;
			SystemMsgMenuText.text = msg;
			
			//deactivate the buttons
			foreach(Button bt in SystemMsgMenuButtons){
				
				bt.gameObject.SetActive(false);
			}
			
			//activate just the button specified by the index parameter
			SystemMsgMenuButtons[btIndex].gameObject.SetActive(true);
			
			//if the screen is deactivated, show it
			if(!SystemMsgMenu.activeSelf){
				GoToMenu(SystemMsgMenu);
				parentWindow.SetActive(false);
			}
		}
		
		/// <summary>
		/// Shows the message menu with no button.
		/// </summary>
		/// <param name="parentWindow">Parent window.</param>
		/// <param name="title">Title of the message.</param>
		/// <param name="msg">Message to been show.</param>
		public void ShowMsgMenuNoButton(GameObject parentWindow, string title, string msg){
			
			//set the text of the message
			SystemMsgMenuTitile.text = title;
			SystemMsgMenuText.text = msg;
			
			//deactivate the buttons
			foreach(Button bt in SystemMsgMenuButtons){
				
				bt.gameObject.SetActive(false);
			}
			
			//display it
			GoToMenu(SystemMsgMenu);

			if(parentWindow != null)
				parentWindow.SetActive(false);
		}

		private IEnumerator FindUserInServer(GameObject parentWindow){
			
			ShowMsgMenuNoButton(parentWindow, "Conectando com o servidor", "Usuário não encontrado, verificando no servidor...");
			
			string url = "http://folclorica2.esy.es/retrieve_user.php?";
			WebServerConnection conn = new WebServerConnection(url);
			
			yield return StartCoroutine(conn.RetrieveUser(GameControl.instance.username, GameControl.instance.password));
			
			string msgTitle;
			
			if(!conn.connError){
				
				msgTitle = "Sucesso!!";
				
				ShowMsgMenuNoButton(parentWindow, msgTitle, conn.returnMsg);
				
				yield return new WaitForSeconds(1f);
				
				LoadDefaultLevel();
			}else{

				msgTitle = "Erro!!";

				msgTitle = "Erro!!";
				
				string msgError = conn.returnMsg;
				
				if(msgError == null || msgError == ""){

					msgError = "Não foi possível conectar-se ao servidor...";
				}

				ShowMsgMenu(null, msgTitle, msgError);
			}
			
		}

		/// <summary>
		/// Adds a user to the online database, enabling access to the game
		/// </summary>
		/// <param name="fieldContainer">Field container. The Add user screen</param>
		public void AddUser(GameObject fieldContainer){
			
			//retrieve the input fields containing the user's data
			InputField[] fields = fieldContainer.GetComponentsInChildren<InputField>();
			
			//check if all of them have beel fulfilled and displays an error message otherwise
			foreach(InputField field in fields){
				
				if(field.text == ""){
					ShowMsgMenu(fieldContainer, "Erro", "Todos os campos devem ser preenchidos...");
					return;
				}
			}
			
			//retrieve the alphanumeric fields
			string name = fields[0].text;
			string email = fields[1].text;
			string password = fields[2].text;
			string passwordCheck = fields[5].text;
			
			//check if password was properly typed. Display an error message and exit otherwise
			if(password != passwordCheck){
				
				ShowMsgMenu(fieldContainer, "Erro", "Senhas não conferem...");
				
				fields[2].text = "";
				fields[5].text = "";
				
				return;
			}
			
			//instantiate a non numeric check for the numeric fields
			Regex nonNumericRegex = new Regex(@"\D");
			
			//check if user's grade is a number. Display an error message and exit otherwise
			if (nonNumericRegex.IsMatch(fields[3].text)){
				
				ShowMsgMenu(fieldContainer, "Erro", "O campo Série deve ser um valor númerico...");
				
				fields[3].text = "";
				
				return;
			}
			
			//set the user's grade (serie) and group (turma)
			int grade = int.Parse(fields[3].text);
			
			string group = fields[4].text;
			
			//try to add it to online database
			StartCoroutine(AddUserIntoServer(fieldContainer, name, email, password, group, grade));
		}

		private IEnumerator AddUserIntoServer(GameObject parentWindow, string name, string email, string password, string group, int grade){
			
			ShowMsgMenuNoButton(parentWindow, "Conectando com o servidor", "Salvando dados...");
			
			string url = "http://folclorica2.esy.es/update_user.php?";
			WebServerConnection conn = new WebServerConnection(url);
			
			yield return StartCoroutine(conn.InsertUser(name.ToUpper(), password, email, 1, group.ToUpper(), grade));
			
			string msgTitle = conn.connError? "Erro!!" : "Sucesso!!";
			
			ShowMsgMenu(null, msgTitle, conn.returnMsg, 1);
		}

		/// <summary>
		/// Loads from login. Loads the last saved game once the user performs his login.
		/// It is possible to recognize an user account but not to retrieve its saved data.
		/// For that, all user data would have to be saved at least at the end of each level.
		/// </summary>
		/// <param name="fieldContainer">Field container. The login screen</param>
		public void LoadFromLogin(GameObject fieldContainer){
			
			//retrieve the input fields containing the user's login name and his password...
			InputField[] fields = fieldContainer.GetComponentsInChildren<InputField>();
			
			//... set these values to the GameControl object
			GameControl.instance.username = fields[0].text;
			GameControl.instance.password = fields[1].text;
			
			//Debug.Log (fields[0].text + " " + fields[1].text);
			
			//retrieve the saving instance
			SaveAssistant saveAssistant = GameControl.instance.saveAssistant;
			
			//retrieve the level data from the save file
			LevelData level = saveAssistant.LoadSaveFile();
			
			//if data was found
			if(level != null){
				
				//Debug.Log("Carregar level: " + level.LevelName());
				
				//if the  saved data says that the player has reached at least 
				//the second checkoint on this level, then load the level
				//Application.LoadLevel(level.LevelName());				

				if(level.lastCheckpointIndex > 1){
					Application.LoadLevel(level.LevelName());
				}
				else if (level.lastCheckpointIndex <= 1 && level.LevelScore(int.Parse(level.LevelID()) - 1) > 0){
					Application.LoadLevel(level.LevelName());
				}
				else{
					
					/*string scene = "Scene XX - Cutscene";
					
					string cutSceneId = saveAssistant.LevelID() + "a";
					
					scene = scene.Replace("XX", cutSceneId);
					
					Application.LoadLevel(scene);*/
					LoadDefaultLevel();
				}
				
				
			}
			//if no data was found, try validating player with server
			else{
				StartCoroutine(FindUserInServer(fieldContainer));
			}
		}
	}


}
