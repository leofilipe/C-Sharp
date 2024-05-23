<?php 
        $hostname = 'mysql.hostinger.com.br';
        $usernameDB = 'u109755212_admin';
        $passwordDB = 'JgXcDDWr5c4x';
        $database = 'u109755212_devel';
		
		//mysql_host 
		$mysqli = new mysqli($hostname, $usernameDB, $passwordDB, $database);
		
		if (mysqli_connect_errno()) {
			printf("Connect failed: %s\n", mysqli_connect_error());
			exit();
		}
	
		$secretKey = 'I8vjPJ9ke2'; # Change this value to match the value stored in the client javascript below 

		
		$user_id = $mysqli->real_escape_string($_GET['user_id']); 
        $level_id = $mysqli->real_escape_string($_GET['level_id']); 
		$score = $mysqli->real_escape_string($_GET['score']); 
		$bonus = $mysqli->real_escape_string($_GET['bonus']); 
		

		$hash = $_GET['hash']; 

        //$real_hash = md5($user_id + $password + $user_name + $cod_escola + $serie + $turma + $secretKey); 
		$real_hash = md5($user_id . $level_id . $score . $bonus . $secretKey);
		
		
		if($real_hash == $hash) { 
			
			//check if this user exists
			$query = "SELECT PONTOS FROM PONTUACAO WHERE ID_USUARIO = '$user_id'";
			$result = $mysqli->query($query) or die($mysqli->error.__LINE__);
			
			$score = $score + $bonus;
			
			//if it does exist
			if($result->num_rows > 0){
				//check if this user exists
				//$query = "SELECT ID FROM USUARIO WHERE ID = '$random_id'";
				$query = "UPDATE PONTUACAO SET PONTOS = '$score', BONUS = '$bonus' WHERE ID_USUARIO = '$user_id' AND COD_LEVEL = '$level_id' AND PONTOS <= '$score'";
			}else{
				//try to insert the students grade into the database	
				$query = "INSERT INTO PONTUACAO VALUES ('$user_id', '$level_id', '$score', '$bonus');";
			}
			
			$result = $mysqli->query($query) or die($mysqli->error.__LINE__);
			
			echo("sucesso... ". $user_id);
        } else{
			echo("Hash incompativel...");

		}
		
		mysqli_close($mysqli);
		
?>