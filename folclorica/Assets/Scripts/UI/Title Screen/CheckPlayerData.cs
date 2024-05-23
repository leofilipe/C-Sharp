using UnityEngine;
using System.Collections;
using System.Data;
using System;


/// <summary>
/// Com a desistencia do uso do sqlite, a classe nao esta sendo utilizada. Mantida para fins de pesquisa caso necessario.
/// </summary>
public class CheckPlayerData {

	/*public string dbName = "local-DB-folclorica-01.sqlite3";

	public void newPlayer(string name, string password){

		//inserir em usuario
		//inserir em aluno
		//verificar demais tabelas
		//criar esquema da tabela offline
		//ajustar esquema da tabela online
		//Na base de dados local, deve ficar salvo o usuario, 
		//o ponto do ultimo checkpoint, a pontuacao e o estado 
		//dos objetos da fase (esse precisa de pesquisa para 
		//saber como... outra opção é força-lo a reiniciar a 
		//fase do início uma vez que saia...)


	}*/

	/*public bool CheckForLocalPlayer(){

		/* Referente a classe LocalDBAccess
		 * 
		 * string sql = "SELECT * FROM USUARIO";
		LocalDBAccess localDB = new LocalDBAccess();
		localDB.OpenConnection();

		IDataReader queryResult = localDB.ExecuteQuery(sql);
		queryResult.Read().
		 */


		/*string sql = "SELECT * FROM USUARIO";


		LocalDBAccessV2 localDB = new LocalDBAccessV2(dbName);
		DataTable sqlResults = localDB.ExecuteQuery(sql);

		//se encontrou algum registro
		//OBS: sqlResults.Rows[0] correspondem aos cabecalhos da tabela
		if(sqlResults.Rows.Count > 1){

			//pegar o primeiro registro e verificar se consta na base online
			//TODO versoes futuras deverao exibir a relacao de jogadores e permitir que o usuario escolha um
			//mediante a apresentacao de uma senha

			string id = (string) sqlResults.Rows[1].ItemArray[0];
			string senha = (string) sqlResults.Rows[1].ItemArray[1];
			IntPtr onOnline = (IntPtr) sqlResults.Rows[1].ItemArray[2];

			Debug.Log("ID: " + id + " SENHA: " + senha + "ONLINE: " + sqlResults.Rows[0].ItemArray[2]);// + " ONLINE: " + onOnline);
		}else{
			Debug.Log("Nenhum registro encontrado");
			Debug.Log("Registrando novo usuario");


			string username = "leofilipe" + (int) UnityEngine.Random.Range(100, 1000);
			string password = "1234";

			sql = "INSERT INTO USUARIO VALUES (" + username + ", " + password + ", 0)";
		}

		//manter este trecho como referencia
		/*for(int i = 0; i < sqlResults.Rows.Count; i++){

			string id = (string) sqlResults.Rows[i].ItemArray[0];
			string senha = (string) sqlResults.Rows[i].ItemArray[1];

			Debug.Log("ID: " + id + " SENHA: " + senha);
		}else
			return false;*/

		/*return sqlResults.Rows.Count > 1;

	}*/

	void ConfirmationWindow(string message){

	}
}
