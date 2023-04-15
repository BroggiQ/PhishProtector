using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
