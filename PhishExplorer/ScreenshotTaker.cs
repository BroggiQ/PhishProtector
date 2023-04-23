using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhishExplorer
{
    class ScreenshotTaker
    {
        private string _screenFolder;

        public ScreenshotTaker()
        {

#if DEBUG
            _screenFolder = @"C:\Users\Akutsu\Desktop\Phishing\Screens\";
#else
            _screenFolder = Path.Combine(Directory.GetCurrentDirectory(),"Screens");

#endif
        }

            public void SaveScreenshot(Screenshot screenshot, string originalSiteUrl, int counter)
        {

            // extraire le nom du site sans le protocole et sans l'extension
            var uriSite = new Uri(originalSiteUrl);
            string siteName = uriSite.Host;
 
            string screenName = string.Format("{0}_{1}_{2}.{3}", DateTime.Now.ToString("yyyy-MM-dd"), siteName, counter, "png");

            string folderSitePath = Path.Combine(_screenFolder, siteName);
            if (!Directory.Exists(folderSitePath))
                Directory.CreateDirectory(folderSitePath);
            string imagePath = Path.Combine(folderSitePath, screenName);

            screenshot.SaveAsFile(imagePath, ScreenshotImageFormat.Png);
        }
  
    }
}
