using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;

public class LocalDBAccess {

	public string dbName = "local-DB-folclorica-01.sqlite3";

	private string conn;

	private IDbConnection dbconn;
	private IDbCommand dbcmd;
	private IDataReader reader;


	public void OpenConnection(){

		string connectionString = "URI=file:./Assets/DB/" + dbName;//Path to database.

		OpenConnection(connectionString);
	}

	// Use this for initialization
	public void OpenConnection (string connectionString) {


		Debug.Log (connectionString);


		dbconn = (IDbConnection) new SqliteConnection(connectionString);

		dbconn.Open(); //Open connection to the database.

		dbcmd = dbconn.CreateCommand();
	
	}

	public IDataReader ExecuteQuery(string sql){

		dbcmd.CommandText = sql;
		
		IDataReader reader = dbcmd.ExecuteReader();

		return reader;

		/*string sql = "SELECT * FROM USUARIO";
		
		dbcmd.CommandText = sql;
		
		IDataReader reader = dbcmd.ExecuteReader();
		
		while(reader.Read()) {
			string id = reader.GetString (0);
			string senha = reader.GetString (1);
			Console.WriteLine("ID: " +
			                  id + " " + senha);
			Debug.Log ("ID: " + id + " SENHA: " + senha);
		}
		
		// clean up
		reader.Close();
		reader = null;
		dbcmd.Dispose();
		dbcmd = null;
		dbconn.Close();
		dbconn = null;*/
		
		//Debug.Log (dbcon);
	}

	public void closeConnection(){

		reader.Close();
		reader = null;

		dbcmd.Dispose();
		dbcmd = null;
		dbconn.Close();
		dbconn = null;
	}
}
