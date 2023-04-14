//Load sample data
using Microsoft.ML;
using PhishAnalyzer;
using Tensorflow.Contexts;
 using Microsoft.ML.Data;
using System.IO;
using Tensorflow.Keras.Engine;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.ML.Trainers;

class Program
{
    private static readonly string _screenFolder = @"C:\Users\Akutsu\source\repos\PhishProtector\PhishExplorer\Screens\";
    static void Main(string[] args)
    {


        /*  var imageBytes = File.ReadAllBytes(@"C:\Users\Akutsu\source\repos\PhishProtector\PhishExplorer\Screens\fr.yahoo.com\2023-03-11_fr.yahoo.com.png");
          SiteClassification.ModelInput sampleData = new SiteClassification.ModelInput()
          {
              ImageSource = imageBytes,
          };

          //Load model and predict output
          var result = SiteClassification.Predict(sampleData);
        */


        /*    // Charger le modèle actuel
              var mlContext = new MLContext();
                 ITransformer currentModel = mlContext.Model.Load(@"./SiteClassification.mlnet", out var modelSchema);



                 // Charger les images à partir du répertoire
                 string imagesDirectory = @"C:\Users\Akutsu\source\repos\PhishProtector\PhishExplorer\Screens";
                 IEnumerable<SiteClassification.ModelInput> images = Directory.GetFiles(imagesDirectory, "*.png", SearchOption.AllDirectories)
                     .Select(imagePath => new SiteClassification.ModelInput
                     {
                         ImageSource = ImageToByteArray(imagePath) 
                     });


                 // Créez un IDataView à partir des images chargées
                 IDataView imageData = mlContext.Data.LoadFromEnumerable(images);
            //IDataView transformedNewData = currentModel.Transform(imageData);

            var modelSchema2 = currentModel.GetOutputSchema(imageData.Schema);


                 var onnxPath = @"C:\Users\Akutsu\source\repos\PhishProtector\PhishExplorer\SiteClassification.onnx";


                 // Convertissez le modèle ML.NET en modèle ONNX et sauvegardez-le
                 using (var fileStream = new FileStream(onnxPath, FileMode.Create))
                 {
                     mlContext.Model.ConvertToOnnx(currentModel, imageData, fileStream);

                 } 
     */


 


    }
    public static byte[] ImageToByteArray(string imagePath)
    {
        using (Image image = Image.FromFile(imagePath))
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }

    public static void SaveAsOnnx(MLContext mlContext, ITransformer model, Stream stream)
    {
        // Créez un échantillon de données pour aider à déterminer les dimensions d'entrée
        var sampleData = mlContext.Data.LoadFromEnumerable(new[] { new SiteClassification.ModelInput() });
        var transformedData = model.Transform(sampleData);

        // Utilisez la méthode SaveOnnxCommand pour sauvegarder le modèle au format ONNX
       mlContext.Model.ConvertToOnnx(model, transformedData, stream);
     }


    public class ImageInput
    {
        public string ImagePath { get; set; }
    }


}