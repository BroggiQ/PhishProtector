// JavaScript source code
browser.browserAction.onClicked.addListener(() => {
    // Le code ici sera exécuté lorsque l'utilisateur cliquera sur l'icône de l'extension.
    console.log("L'icône de l'extension a été cliquée !");

    // Récupère l'onglet actif
    browser.tabs.query({ active: true, currentWindow: true })
        .then((tabs) => {
            const tab = tabs[0];
            console.log(tab.url);
			
			    // Capturer la capture d'écran de l'onglet actif
					browser.tabs.captureTab(tab.id)
					  .then((dataUrl) => {
						console.log("Capture d'écran de l'onglet :", dataUrl);
					  })
					  .catch((error) => {
						console.error("Impossible de capturer la capture d'écran de l'onglet :", error);
					  });
     
        })
        .catch((error) => {
            console.error("Impossible de récupérer l'onglet actif :", error);
        });
});