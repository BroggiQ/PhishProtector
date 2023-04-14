﻿// This file was auto-generated by ML.NET Model Builder.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.Data;
using Microsoft.ML.Vision;
using Microsoft.ML;

namespace PhishAnalyzer
{
    public partial class SiteClassification
    {



        /// <summary>
        /// Retrains model using the pipeline generated as part of the training process.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <param name="trainData"></param>
        /// <returns></returns>
        public static ITransformer RetrainModel(MLContext mlContext, IDataView trainData)
        {
            var pipeline = BuildPipeline(mlContext);
            var model = pipeline.Fit(trainData);

            var onnxPath = @"C:\Users\Akutsu\source\repos\PhishProtector\PhishExplorer\SiteClassification.onnx";

            // Convertissez le modèle ML.NET en modèle ONNX et sauvegardez-le
            using (var fileStream = new FileStream(onnxPath, FileMode.Create))
            {
                mlContext.Model.ConvertToOnnx(model, trainData, fileStream);

            }
            return model;
        }


        /// <summary>
        /// build the pipeline that is used from model builder. Use this function to retrain model.
        /// </summary>
        /// <param name="mlContext"></param>
        /// <returns></returns>
        public static IEstimator<ITransformer> BuildPipeline(MLContext mlContext)
        {
            // Data process configuration with pipeline data transformations
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName:@"Label",inputColumnName:@"Label",addKeyValueAnnotationsAsText:false)      
                                    .Append(mlContext.MulticlassClassification.Trainers.ImageClassification(labelColumnName:@"Label",scoreColumnName:@"Score",featureColumnName:@"ImageSource"))      
                                    .Append(mlContext.Transforms.Conversion.MapKeyToValue(outputColumnName:@"PredictedLabel",inputColumnName:@"PredictedLabel"));

            return pipeline;
        }
    }
 }
