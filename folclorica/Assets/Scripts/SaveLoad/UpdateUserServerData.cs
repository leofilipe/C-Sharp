using UnityEngine;
using System.Collections;
using beffio.OneMinuteGUI;
using System.Collections.Generic;

public class UpdateUserServerData : MenuManagerLogin {

	//public FadeInOut waitFader;					//Fade-in fader to this scene that should be waited for before starting the data transfer

	public float _duration = 1.5f;

	//public string nextLevelToLoad = "Scene 00 - Title Screen";

	private string secretKey = "I8vjPJ9ke2";	//hash secret key

	private string opStatus = "";				//status of the operation: error - empty

	private string[] keys;						//user save file keys within the file

	private bool allChecked = false;			//flag to indicate that the coroutines for all keys have been concluded.


	/*void Start(){

		//get the fader duration and wait the scene to fade in
		float _duration = waitFader.fadeDuration;
		while(_duration > 0)
			_duration -= Time.deltaTime;

		//start the update
		//StartCoroutine(UpdateUserScore());

	}*/

	public void StartUpdateRoutine(){

		//disable the pause menu for now
		if(Pauser.instance != null)
			Pauser.instance.enabled = false;

		while(_duration > 0)
			_duration -= Time.deltaTime;

		StartCoroutine(UpdateUserScore());

		Debug.Log("Click");
	}

	/// <summary>
	/// Updates the user score.
	/// </summary>
	/// <returns>The user score.</returns>
	private IEnumerator UpdateUserScore(){

		//retrieve the saved data
		SavedData _savedData = SavedData.GetCurrentGameData();

		//retrieve saved data user keys'
		keys = _savedData.SavedKeys();

		//start a counter
		int count = 0;

		//while the counter's value is lower than the keys length
		for (count = 0; count < keys.Length; count++){
			
			//if the key belongs to this user
			if(keys[count].Contains(GameControl.instance.username)){
				
				//retrieve its level data
				LevelData aLevel = _savedData.GetSavedData(keys[count]);

				//starts the couroutine for sending user data about this level
				yield return StartCoroutine(UpdateUserLevelScore(GameControl.instance.username, aLevel, count));

				//if an error ocurred during the sending stop the loop.
				if(opStatus.Contains("Erro"))
					break;

			}
		}

		//in case the threads within the for get out of control, this forces them to wait until all 
		//have been concluded (allChecked is true) or that an error was caught (opStatus is not empty)
		while(opStatus == "" && !allChecked){

			//wait for one second before intarating
			yield return new WaitForSeconds(1f);
		}

		//if there was an error during the transfer of data
		if(opStatus.Contains("Erro")){

			Debug.Log("opStatus " + opStatus);

			//keep the message on the screen for the user for a period of time
			yield return new WaitForSeconds(6f);

			//update the message on the screen to say the user has concluded the level
			ShowMsgMenuNoButton(null, "Parabéns!!!", "Você concluiu esta parte do jogo. Em breve estaremos trazendo a continuação para você. Obrigado!!;)");

			//keep the message on the screen for the user for a period of time
			yield return new WaitForSeconds(6f);

			//go back to login screen
			Application.LoadLevel(defaultLoadLevel);
		}else{

			Debug.Log("opStatus " + opStatus);

			//update msg status
			opStatus = "Sucesso!!";

			//update the message on the screen to say the data have been successfully saved
			ShowMsgMenuNoButton(null, opStatus, "Dados salvos com sucesso no servidor!");

			//keep the message on the screen for the user for a period of time
			yield return new WaitForSeconds(4f);

			//update the message on the screen to say the user has concluded the level
			ShowMsgMenuNoButton(null, "Parabéns!!!", "Você concluiu esta parte do jogo. Em breve estaremos trazendo a continuação para você. Obrigado!!;)");

			//keep the message on the screen for the user for a period of time
			yield return new WaitForSeconds(6f);

			//go to the next screen
			Application.LoadLevel(defaultLoadLevel);
		}


	}

	/// <summary>
	/// Updates the user score for the specified level.
	/// </summary>
	/// <returns>The user level score.</returns>
	/// <param name="username">Username.</param>
	/// <param name="aLevel">A level.</param>
	/// <param name="saveIndex">Save index.</param>
	private IEnumerator UpdateUserLevelScore(string username, LevelData aLevel, int saveIndex){

		if(aLevel.IsLevelFinished()){

			//get its level id and score
			string levelID = aLevel.LevelID();					
			int score = aLevel.LevelScore(int.Parse(levelID)  - 1);
			int bonus = aLevel.levelBonus;

			//set the url to where post the data
			string url = "http://folclorica2.esy.es/update_user_score.php?";

			//create the hash sum
			string hash = Md5.Md5Sum(username + levelID + score + bonus + secretKey);

			//prepare the posting url
			string post_url = url + 	"user_id=" + WWW.EscapeURL(username) + 
				"&level_id=" + levelID + "&score=" + score + "&bonus=" + bonus + "&hash=" + hash;

			Debug.Log(post_url);

			// Posts the URL to the site and create a download object to get the result.
			WWW hs_post = new WWW(post_url);
			
			yield return hs_post; // Wait until the download is done

			//get the returing message
			string returnMsg = hs_post.text;

			//if it has no sucesso in it or is empty
			if(!returnMsg.Contains("sucesso") || returnMsg == null){
				
				Debug.Log("If --> pontuacao nao foi salva com sucesso");

				//set an error message
				returnMsg = "Ocorreu um erro durante o salvamento dos dados... " + returnMsg;

				//set an error message title
				opStatus = "Erro!!";

				//set an error message in case there is no content on the return message
				if(returnMsg == null || returnMsg == ""){
					
					returnMsg = "Não foi possível conectar-se ao servidor... A pontuação será enviada em outro momento. Carregando próxima fase...";
				}
				//set a different error message in case there is a returning message
				else{
					returnMsg = "Não foi possível conectar-se ao servidor... A pontuação será enviada em outro momento... "+ returnMsg +"... Carregando próxima fase...";
				}

				//show the error message
				ShowMsgMenuNoButton(null, opStatus, returnMsg);
				
				Debug.Log("msgError " + returnMsg);
				
			}else{
				//IMPORTANT - DO NOT set a success status for opStatus as it can prematurally interrupt the caller loop

				Debug.Log("Else --> pontuacao foi salva com sucesso");
			}
			
			Debug.Log("Retorno do servidor: " + hs_post.text);
		}

		//if all data has been checked, flag it as so
		if(saveIndex == keys.Length - 1)
			allChecked = true;

		//return on the next frame
		yield return null;
		
	}
}
