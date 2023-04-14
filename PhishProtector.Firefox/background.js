// JavaScript source code
browser.browserAction.onClicked.addListener(() => {
    // Le code ici sera exécuté lorsque l'utilisateur cliquera sur l'icône de l'extension.
    console.log("L'icône de l'extension a été cliquée !");


});

browser.webNavigation.onBeforeNavigate.addListener((details) => {
  // L'utilisateur est en train de naviguer vers une nouvelle page
  console.log("Changement de site :", details.url);
  
      // Récupère l'onglet actif
    browser.tabs.query({ active: true, currentWindow: true })
        .then((tabs) => {
            const tab = tabs[0];
            console.log(tab.url);
			const baseUrl = tab.url.origin;
			console.log("Base de l'URL :", baseUrl);
			
			
			fetch('https://mon-api.com/path', {
				  method: 'POST',
				  headers: {
					'Content-Type': 'application/json'
				  },
				  body: JSON.stringify({
					targetUrl: baseUrl
				  })
				})
				  .then(response => {
					if (!response.ok) {
					  throw new Error('Réponse de l\'API invalide');
					}
					return response.json();
				  })
				  .then(data => {
					  if(data == true){
						 console.log('Site valide');
					  }
					  else{
						console.log('Site invalide');
					  }
					
					
				  })
				  .catch(error => {
					console.error('Erreur lors de l\'appel de l\'API :', error);
				  });
							
			
			
			
			
			
			    // Capturer la capture d'écran de l'onglet actif
					/*browser.tabs.captureTab(tab.id)
					  .then((dataUrl) => {
						console.log("Capture d'écran de l'onglet :", dataUrl);
					  })
					  .catch((error) => {
						console.error("Impossible de capturer la capture d'écran de l'onglet :", error);
					  });
					  */
     
        })
        .catch((error) => {
            console.error("Impossible de récupérer l'onglet actif :", error);
        });
});