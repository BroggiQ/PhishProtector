
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


browser.webRequest.onHeadersReceived.addListener(getCertificate,
    { urls: ['<all_urls>'], types: ['main_frame'] },
    ["blocking"]
);



var certificateInfoJson = "";

async function getCertificate(details) {
    try {
        let securityInfo = await browser.webRequest.getSecurityInfo(details.requestId, {});
        console.log(details.url);
        if (securityInfo.state === "secure" || securityInfo.state === "weak") {
            const cert = securityInfo.certificates[0];

            const certificateInfo = {
                issuer: cert.issuer,
                serialNumber: cert.serialNumber,
                subject: cert.subject,
                subjectPublicKeyInfoDigest: cert.subjectPublicKeyInfoDigest,
                validity: cert.validity,
            };
            certificateInfoJson = JSON.stringify(certificateInfo);
        }
    }
    catch (error) {
        console.error(error);
    }
}



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


            sendToAntiPhishingAPI(baseUrl, dataUrl, certificateInfoJson);

        })
        .catch((error) => {
            console.error("Unable to retrieve the active tab:", error);
        });



    async function sendToAntiPhishingAPI(url, imageData, certificateInfoJson) {
        try {
            console.log("sendToAntiPhishingAPI");
            const apiURL = 'http://localhost:5001/api/analyze';

            // Create a FormData object to send the URL and the image
            const formData = new FormData();
            formData.append('url', url);
            formData.append('certificateInfoJson', certificateInfoJson);

            // Convert the imageData to Blob
            //const imageBlob = new Blob([new Uint8Array(imageData)], { type: 'image/png' });
            //formData.append('screenBytes', imageBlob);

            formData.append('screenBytes', imageData);

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
