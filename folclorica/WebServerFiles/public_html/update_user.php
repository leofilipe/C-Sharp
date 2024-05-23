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
		$user_name = $mysqli->real_escape_string($_GET['user_name']); 
		$cod_escola = $mysqli->real_escape_string($_GET['cod_escola']); 
		$serie = $mysqli->real_escape_string($_GET['serie']); 
		$turma = $mysqli->real_escape_string($_GET['turma']); 

		$hash = $_GET['hash']; 

        //$real_hash = md5($user_id + $password + $user_name + $cod_escola + $serie + $turma + $secretKey); 
		$real_hash = md5($user_id . $password . $user_name . $cod_escola . $serie . $turma . $secretKey);
		
		
		if($real_hash == $hash) { 
		
			//create a random id and store the user  id in it.
			$random_id = $user_id;
			
			//check if this user exists
			$query = "SELECT ID FROM USUARIO WHERE ID = '$user_id'";
			$result = $mysqli->query($query) or die($mysqli->error.__LINE__);
			
			//while it does exist
			while($result->num_rows > 0){
				
				//set the random id as the user id and a random number
				$random_id = $user_id . rand(100, 999);
				
				//check if this user exists
				$query = "SELECT ID FROM USUARIO WHERE ID = '$random_id'";
				$result = $mysqli->query($query) or die($mysqli->error.__LINE__);
			}
			
			//set the user id as the random id so the value is correct whether or not 
			//it went through the while
			$user_id = $random_id;
			
			//try to insert the students grade into the database	
			$query = "INSERT IGNORE INTO SERIE VALUES ('$serie', '$cod_escola', '$turma', '$user_id');";
			
			$result = $mysqli->query($query) or die($mysqli->error.__LINE__);
			
			//try to insert the new user into the database
			$query = "INSERT IGNORE INTO  USUARIO (ID, SENHA) VALUES ('$user_id', '$password');"; 
            $result = $mysqli->query($query) or die($mysqli->error.__LINE__);
			
			//try to insert the new student into the database
			$query = "INSERT INTO ALUNO VALUES ('$user_id', '$cod_escola', '$serie', '$user_name');"; 
			$result = $mysqli->query($query) or die($mysqli->error.__LINE__);
			//NOTE: WOULD BE BETTER TO CHECK IF STUDENT AND USER ARE YET NOT IN THE TABLE
			//BUT THIS IS NOT BEING DONE NOW
			
			echo("sucesso... ". $user_id);
        } else{
			echo("Hash incompativel...");
			//echo("Hash ". $hash);
			//echo("Real Hash ". $real_hash);
		}
		
		mysqli_close($mysqli);
		
		/*$query = "SELECT MAX(ID) FROM USUARIO";
		$result =$mysqli->query($query) or die($mysqli->error.__LINE__);
		$id;
		
		if($result->num_rows > 0){
			 //$id = mysqli_fetch_array($result)['ID'] + 1;
			 $row = $result->fetch_assoc();
			 $id = $row['ID'] + 1;
			 
			 //printf("Printing... Data found on table id set to %d\n", $id );
			 //echo "Echoing.. Data found on table id set to ". $id; 
		}else{
			$id = 1;
			//printf("Printing... No data found on table id set to %d\n", $id );
			//echo "Echoing.. No data found on table id set to ". $id; 
		}*/
		
		//$query = "SELECT MAX(ID) FROM USUARIO"
		
		
        // Strings must be escaped to prevent SQL injection attack. 
        /*$name = mysql_real_escape_string($_GET['name'], $db); 
        $score = mysql_real_escape_string($_GET['score'], $db); 
        $hash = $_GET['hash']; 
 
        $secretKey="mySecretKey"; # Change this value to match the value stored in the client javascript below 

        $real_hash = md5($name . $score . $secretKey); 
        if($real_hash == $hash) { 
            // Send variables for the MySQL database class. 
            $query = "insert into scores values (NULL, '$name', '$score');"; 
            $result = mysql_query($query) or die('Query failed: ' . mysql_error()); 
        } */
?>