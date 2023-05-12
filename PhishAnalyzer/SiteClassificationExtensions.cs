using Microsoft.ML;

namespace PhishAnalyzer
{
    public partial class SiteClassification
    {
        private static PredictionEngine<ModelInput, ModelOutput> _predictionEngineInstance;

        public static PredictionEngine<ModelInput, ModelOutput> PredictionEngineInstance
        {
            get
            {
                if (_predictionEngineInstance == null)
                {
                    _predictionEngineInstance = CreatePredictEngine();
                }
                return _predictionEngineInstance;
            }
        }
    }
}
