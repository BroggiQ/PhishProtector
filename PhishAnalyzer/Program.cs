//Load sample data
using PhishAnalyzer;


class Program
{
    private static readonly string _screenFolder = @"C:\Users\Akutsu\source\repos\PhishProtector\PhishExplorer\Screens\";
    static void Main(string[] args)
    {


        var imageBytes = File.ReadAllBytes(@"C:\Users\Akutsu\source\repos\PhishProtector\PhishExplorer\Screens\fr.yahoo.com\2023-03-11_fr.yahoo.com.png");
        SiteClassification.ModelInput sampleData = new SiteClassification.ModelInput()
        {
            ImageSource = imageBytes,
        };

        //Load model and predict output
        var result = SiteClassification.Predict(sampleData);
    }
}