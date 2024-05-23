using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

public class WebServerConnection {

	private string secretKey = "I8vjPJ9ke2"; // Edit this value and make sure it's the same as the one stored on the server

	public string targetURL; //be sure to add a ? to your url
	public string returnMsg;
	public bool connError;
	//public string highscoreURL = "http://localhost/unity_test/display.php";
	

	public WebServerConnection(string targetURL){
		this.targetURL = targetURL;
	}

	// remember to use StartCoroutine when calling this function!
	public IEnumerator InsertUser(string name, string password, string email, int cod_escola, string turma, int serie){

		connError = false;
		returnMsg = null;

		name = name.Trim();

		string[] initials = name.Split(' ');

		string username = "";

		foreach(string s in initials){

			username += s.ToLower()[0];
		}

		//This connects to a server side php script that will add the name and score to a MySQL DB.
		// Supply it with a string representing the players name and the players score.
		string hash = Md5.Md5Sum(username + password + name + cod_escola + serie + turma + secretKey);//name + password + email + cod_escola + turma + serie + secretKey);
		
		string post_url = targetURL + 	"user_id=" + WWW.EscapeURL(username) + 
				"&password=" + WWW.EscapeURL(password) + 
				"&user_name=" + WWW.EscapeURL(name) +
				"&cod_escola=" + cod_escola +
				"&serie=" + serie +
				"&turma=" + WWW.EscapeURL(turma) +
				"&hash=" + hash;

			//+ "&spassword=" + WWW.EscapeURL(password) + "&hash=" + hash;
		
		// Post the URL to the site and create a download object to get the result.
		WWW hs_post = new WWW(post_url);

		//hs_post.responseHeaders
		yield return hs_post; // Wait until the download is done

		returnMsg = hs_post.text;

		if(!returnMsg.Contains("sucesso") || returnMsg == null){
			connError = true;
			returnMsg = "Ocorreu um erro durante a criação do usuário... " + returnMsg;
		}else{
			returnMsg = "Usuário criado com sucesso...\nNome de usuário: " + returnMsg.Split(' ')[1];
		}
	}

	// remember to use StartCoroutine when calling this function!
	public IEnumerator RetrieveUser(string username, string password){
		
		connError = false;
		returnMsg = null;
		

		//This connects to a server side php script that will add the name and score to a MySQL DB.
		// Supply it with a string representing the players name and the players score.
		string hash = Md5.Md5Sum(username + password + secretKey);//name + password + email + cod_escola + turma + serie + secretKey);
		
		string post_url = targetURL + 	"user_id=" + WWW.EscapeURL(username) + 
			"&password=" + WWW.EscapeURL(password) + "&hash=" + hash;

		Debug.Log(post_url);
		
		// Post the URL to the site and create a download object to get the result.
		WWW hs_post = new WWW(post_url);
		
		//hs_post.responseHeaders
		yield return hs_post; // Wait until the download is done
		
		returnMsg = hs_post.text;
		
		if(!returnMsg.Contains("sucesso") || returnMsg == null){
			connError = true;
			//returnMsg = "Ocorreu um erro durante a criação do usuário... " + returnMsg;
		}else
			returnMsg = "Usuário válidado com o servidor, iniciando nova sessão de jogo...";
	}

	//was not executing properly here. See UpdateUserServerData class for final version.
	// remember to use StartCoroutine when calling this function!
	/*public IEnumerator UpdateScore(string username, string level_id, int score){
		
		connError = false;
		returnMsg = null;
		
		//This connects to a server side php script that will add the name and score to a MySQL DB.
		// Supply it with a string representing the players name and the players score.
		string hash = Md5.Md5Sum(username + level_id + score + secretKey);//name + password + email + cod_escola + turma + serie + secretKey);
		
		string post_url = targetURL + 	"user_id=" + WWW.EscapeURL(username) + 
			"&level_id=" + level_id + "&score=" + score + "&hash=" + hash;
		
		// Post the URL to the site and create a download object to get the result.
		WWW hs_post = new WWW(post_url);

		//hs_post.responseHeaders
		//yield return hs_post; // Wait until the download is done

		//hs_post
		
		returnMsg = hs_post.text;
		
		if(!returnMsg.Contains("sucesso") || returnMsg == null){
			connError = true;
			returnMsg = "Ocorreu um erro durante o salvamento dos dados... " + returnMsg;
		}else{
			returnMsg = "Pontuação salva com sucesso. Carregando próxima fase...";
		}
	}*/
}
