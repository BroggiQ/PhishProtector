
browser.browserAction.onClicked.addListener(() => {
    // The code here will be executed when the user clicks on the extension icon.
    console.log("The extension icon has been clicked!");

});

// Detects the called URL
//TODO case of an immediate redirection?
//TODO manage multi-tab icon per tab
browser.webRequest.onBeforeRequest.addListener(
    analyzeRequest,
    { urls: ['<all_urls>'], types: ['main_frame'] },
    ['blocking']
);

//Headers received, permits to get the certificate
browser.webRequest.onHeadersReceived.addListener(getCertificate,
    { urls: ['<all_urls>'], types: ['main_frame'] },
    ["blocking"]
);


//The website's certificate
var certificateInfoJson = "";

//Get the certificate from the webrequest
async function getCertificate(details) {
    try {
        let securityInfo = await browser.webRequest.getSecurityInfo(details.requestId, {});
        console.log(details.url);
        if (securityInfo.state === "secure" || securityInfo.state === "weak") {
            const cert = securityInfo.certificates[0];
            //Create a model to transfer to the Windows apps
            const certificateInfo = {
                issuer: cert.issuer,
                serialNumber: cert.serialNumber,
                subject: cert.subject,
                subjectPublicKeyInfoDigest: cert.subjectPublicKeyInfoDigest,
                validity: cert.validity,
            };
            //Convert to jon format
            certificateInfoJson = JSON.stringify(certificateInfo);
        }
    }
    catch (error) {
        console.error(error);
    }
}


//Get the url visited and take a screen of this website
async function analyzeRequest(requestDetails) {
    // The user is navigating to a new page
    console.log("Site change:", requestDetails.url);

    const baseUrl = requestDetails.url;
    console.log("Base of the URL:", baseUrl);


    // Retrieves the active tab
    browser.tabs.query({ active: true, currentWindow: true })
        .then(async (tabs) => {
            const tab = tabs[0];
            console.log(tab.url);

            // Capture the screenshot of the active tab
            const dataUrl = await browser.tabs.captureTab(tab.id);

            //Call the api of the Windows app
            sendToAntiPhishingAPI(baseUrl, dataUrl, certificateInfoJson);

        })
        .catch((error) => {
            console.error("Unable to retrieve the active tab:", error);
        });


    //Call the Windows app API
    async function sendToAntiPhishingAPI(url, imageData, certificateInfoJson) {
        try {
            console.log("sendToAntiPhishingAPI");
            //TODO move this and check if the Windows app is installed
            const apiURL = 'http://localhost:5001/api/analyze';

            // Create a FormData object to send the URL and the image
            const formData = new FormData();
            //The url of the website
            formData.append('url', url);
            //The certificate of the website
            formData.append('certificateInfoJson', certificateInfoJson);
            //The screen of the website
            formData.append('screenBytes', imageData);
            //TODO add the html code

            // Send the request to the API
            const response = await fetch(apiURL, {
                method: 'POST',
                body: formData,
            });

            // Check if the request was successful
            if (!response.ok) {
                throw new Error(`Error calling the API: ${response.statusText}`);
            }

            // Process the API response
            const result = await response.json();
            console.log('API result:', result);

            if (result) {
                browser.browserAction.setIcon({ path: { "48": "Icons/ok.png" } });

            }
            else {
                browser.browserAction.setIcon({ path: { "48": "Icons/ko.png" } });
            }

        } catch (error) {
            browser.browserAction.setIcon({ path: { "48": "Icons/alerte.png" } });
        }
    }
}
