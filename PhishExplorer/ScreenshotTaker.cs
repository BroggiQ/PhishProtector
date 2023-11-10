using OpenQA.Selenium;
using PhishProtector.Helper;

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
        /// <summary>
        /// This function save the screenshot in png under the format date + url + counter (for the internal link)
        /// </summary>
        /// <param name="screenshot"></param>
        /// <param name="originalSiteUrl"></param>
        /// <param name="counter"></param>
        public void SaveScreenshot(Screenshot screenshot, string originalSiteUrl, int counter)
        {

            // Extract the site name without the protocol and the extension
            string siteName = SiteHelper.GetHostFromUrl(originalSiteUrl);



 
            string screenName = string.Format("{0}_{1}_{2}.{3}", DateTime.Now.ToString("yyyy-MM-dd"), siteName, counter, "png");

            string folderSitePath = Path.Combine(_screenFolder, siteName);
            if (!Directory.Exists(folderSitePath))
                Directory.CreateDirectory(folderSitePath);
            string imagePath = Path.Combine(folderSitePath, screenName);

            screenshot.SaveAsFile(imagePath, ScreenshotImageFormat.Png);
        }

    }
}
