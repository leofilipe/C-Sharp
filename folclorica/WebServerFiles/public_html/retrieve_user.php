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
        $password = $mysqli->real_escape_string($_GET['password']); 

		$hash = $_GET['hash']; 

		$real_hash = md5($user_id . $password . $user_name . $secretKey);
		
		
		if($real_hash == $hash) { 
		
			
			//check if this user exists
			$query = "SELECT ID, SENHA FROM USUARIO WHERE ID = '$user_id'";
			$result = $mysqli->query($query) or die($mysqli->error.__LINE__);
			
			if($result->num_rows > 0){
				
				 $row = $result->fetch_assoc();
				 
				 if($row['SENHA'] != $password){
					 echo("Senha incorreta: ". $row['SENHA'] . " ". $password);
				 }else{
					 echo("sucesso...");
				 }
				 
			}else
				echo("Usuário não encontrado...");
        } else{
			echo("Hash incompativel...");
		}
		
		mysqli_close($mysqli);

?>