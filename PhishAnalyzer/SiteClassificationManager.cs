
namespace PhishAnalyzer
{
    public class SiteClassificationManager
    {
        public static SiteClassification.ModelOutput Predict(SiteClassification.ModelInput input)
        {
            return SiteClassification.PredictionEngineInstance.Predict(input);
        }

    }
}
