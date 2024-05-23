<!DOCTYPE html>
<!-- Website template by freewebsitetemplates.com -->
<html>
<head>
	<meta charset="UTF-8">
	<title>Folclórica</title>
	<link rel="stylesheet" type="text/css" href="css/style.css">
</head>
<body>
	<div id="header">
		<div class="section">
			<a href="index.html"><img src="images/logo3.png" alt="Image" height="140"></a>
            
			<!--<ul>
				<li class="current">
					<a href="index.html">Home</a>
				</li>
				<li>
					<a href="farm.html">The Farm</a>
				</li>
				<li>
					<a href="blog.html">Blog</a>
				</li>
				<li>
					<a href="about.html">About</a>
				</li>
				<li>
					<a href="contact.html">Contact</a>
				</li>
			</ul>-->
		</div>
		<div id="figure">
        	<div style="position:relative; margin-bottom:-10px; padding-left:-100px; text-align:center; z-index:4"><img src="images/main_char.png" alt="Image" height="200"/></div>
		</div>
		<span></span>
	</div>
	<div id="body">
		<div>
			<div>
				<div>
				  <div id="content">
						<h2>Quadro de classificação</h2>
						
                        <div style="color:#383838;">
                    
                            <table style="width:100%" >
                              
                                <?php include 'high_scores.php';
                                
                                    echo "<tr style=\"font-size:150%\"><th>#</th><th>Jogador</th><th>N&iacute;vel</th><th>Pontos</th></tr>";
                                    
                                    for($i = 0; $i < $num_results; $i++){
                                        
                                        if($i%2 == 0){
                                            echo "<tr id=\"table-line\"; style=\"font-size:130%;\">";
                                        }else{
                                            echo "<tr style=\"font-size:130%;\">";
                                        }
                                        
                                        $row = mysqli_fetch_array($result);
										$place = $i + 1;
                                        
                                        echo "<td style=\"text-align:right; padding-right: 5px\">". $place . "</td><td style=\"padding-left: 5px;\">" . 
												$row['ID_USUARIO'] . "</td><td style=\"text-align:center\">" . 
												$row['COD_LEVEL'] . "</td><td style=\"text-align:right; padding-right: 5px\">" . 
												$row['PONTOS'] . 
											"</td>";
                                        echo "</tr>";
                                    }
    
                                ?>
                            </table>
						</div>
					</div> <!--/content-->
				  <p>
                      <ul id="side-list">
                        <li>
                        	<a href="http://goo.gl/forms/QegRcVPqaE" target="_blank">Questionário online</a>
                       	</li>
                        <li>
                            <a href="https://drive.google.com/file/d/0B_8Uj7Dohga4eFo2bHVSVHR0MzQ/view" target="_blank">Download do jogo</a>
                        </li>
                      </ul>
                  </p>
				  
				  <p id="side-list">
						Jogo desenvolvido por:
						
						<ul id="side-list2">
							<li>Leonardo Filipe Batista Silva de Carvalho - Doutorando PPGIE UFRGS. </li>
								<ul id="side-list3">
									<li>Game design, desenvolvimento e narrativa.</li>
								</ul>
						</ul>
				</p>
				<p id="side-list" style="margin-top:0px;">
						Em parceria com:
						
						<ul id="side-list2">
							<li>PET-Inf - UFRGS</li>
								<ul id="side-list3">
									<li>Game design, desenvolvimento e narrativa.</li>
								</ul>
							<li>Patrick Pereira - UFRGS</li>
								<ul id="side-list3">
									<li>Arte.</li>
								</ul>
							<li>Vivian Albertoni - Colégio de Aplicação UFRGS.</li>
								<ul id="side-list3">
									<li>Narrativa.</li>
								</ul>
						</ul>
				  </p>
</div>
			</div>
		</div>
	</div>
	<div id="footer">
		<div>
			<div>
				<div>
					<ul>
						<li>
						</li>
						<li>
						</li>
					</ul>
				</div>
				<div>
					<div class="section">
						<span></span>
					</div>
				</div>
			</div>
		</div>
		<div>
			<p class="footnote">
				&copy; 2016 Folclórica.
			</p>
		</div>
	</div>
</body>
</html>
				