<?php

	$hostname = 'mysql.hostinger.com.br';
	$usernameDB = 'u109755212_admin';
	$passwordDB = 'JgXcDDWr5c4x';
	$database = 'u109755212_devel';
	$error = '';
	
	//mysql_host 
	$mysqli = new mysqli($hostname, $usernameDB, $passwordDB, $database);
	
	
	if (mysqli_connect_errno()) {
		
		$error = 'Connect failed: '. mysqli_connect_error() . '\n';
		
		printf("Connect failed: %s\n", mysqli_connect_error());
		exit();
	}
	
	$query = "SELECT ID_USUARIO, COD_LEVEL, PONTOS FROM PONTUACAO ORDER by PONTOS DESC";
	
	$result = $mysqli->query($query) or die($mysqli->error.__LINE__);

	$num_results = $result->num_rows;
	
		
?>