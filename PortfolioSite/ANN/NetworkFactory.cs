using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.Neural.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Neural.NeuralData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANN
{
    public class NetworkFactory
    {
        public static string[][] GetInputData()
        {
            //Columns: |Weather (majority of day)|,|Temp (C)| 
            return new string[][]
            {
                new string[2] { "Rain", "13" },
                new string[2] { "Wind", "15" },
                new string[2] { "Sun", "21" },
                new string[2] { "Cloud/Overcast", "19" },
                new string[2] { "Snow", "-1" },
                new string[2] { "Rain", "9" },
                new string[2] { "Wind", "14" },
                new string[2] { "Sun", "25" },
                new string[2] { "Cloud/Overcast", "17" },
                new string[2] { "Snow", "2" },
                new string[2] { "Rain", "15" },
                new string[2] { "Wind", "5" },
                new string[2] { "Sun", "18" },
                new string[2] { "Cloud/Overcast", "19" },
                new string[2] { "Snow", "0" },
                new string[2] { "Rain", "12" },
                new string[2] { "Wind", "13" },
                new string[2] { "Sun", "21" },
                new string[2] { "Cloud/Overcast", "18" },
                new string[2] { "Snow", "-3" },
                new string[2] { "Rain", "12" },
                new string[2] { "Wind", "15" },
                new string[2] { "Sun", "3" },
                new string[2] { "Cloud/Overcast", "22" },
                new string[2] { "Snow", "3" },
                new string[2] { "Snow", "5" },
                new string[2] { "Sun", "-1" }
            };
        }

        public static double[][] GetIdealValues()
        {
            //0 = false & 1 = true
            return new double[][]
            {
                new double[1] { 0 },
                new double[1] { 1 },
                new double[1] { 1 },
                new double[1] { 1 },
                new double[1] { 1 },
                new double[1] { 0 },
                new double[1] { 0 },
                new double[1] { 1 },
                new double[1] { 1 },
                new double[1] { 0 },
                new double[1] { 0 },
                new double[1] { 0 },
                new double[1] { 1 },
                new double[1] { 1 },
                new double[1] { 1 },
                new double[1] { 0 },
                new double[1] { 0 },
                new double[1] { 1 },
                new double[1] { 1 },
                new double[1] { 0 },
                new double[1] { 1 },
                new double[1] { 0 },
                new double[1] { 1 },
                new double[1] { 1 },
                new double[1] { 0 },
                new double[1] { 0 },
                new double[1] { 0 },
            };
        }

        public static double[][] FormatInputValues(string[][] unformattedData)
        {
            var formattedValues = new double[unformattedData.Count()][];
            //Get distinct column name
            //Columns: |Rain|,|Wind|,|Sun|,|Cloud/Overcast|,|Snow|
            var distinctColumnNames = unformattedData.Select(x => x[0])
                .Distinct();
            //Create a row (distinct column values + 1 number field)
            double[] row = null;
            //Find min & max temp
            var minTemp = unformattedData.Select(x => x[1]).Min(x => int.Parse(x));
            var maxTemp = unformattedData.Select(x => x[1]).Max(x => int.Parse(x));
            var deviationFromZero = 0 - minTemp;
            var range = (minTemp < 0 ? 0-minTemp : 0+minTemp) + (maxTemp < 0 ? 0 - maxTemp : 0 + maxTemp);
            //Add the rows to the new formatted array
            for (int i = 0; i < unformattedData.Count(); i++)
            {
                //Init new row
                row = new double[distinctColumnNames.Count() + 1];
                //Expand columns for the weather
                for (int j = 0; j < distinctColumnNames.Count(); j++)
                {
                    if (unformattedData[i][0] == distinctColumnNames.ElementAt(j)) row[j] = 1;
                    else row[j] = 0;
                }
                //Take care of the temp
                var temp = double.Parse(unformattedData[i][1]);
                double onScale = ((((double)temp) + (((double)deviationFromZero))) / ((double)range));
                row[row.Length - 1] = onScale;
                //Set the row in the return list
                formattedValues[i] = row;
            }
            return formattedValues;
        }

        public static BasicNetwork CreateNetwork(double[][] trainingData, double[][] ideals, int? hiddenLayerCount = null)
        {
            //Make sure we have data
            if (trainingData.Any() && ideals.Any())
            {
                //Count the number of inputs (columns)
                var inputCount = trainingData[0].Count();
                //Count the number of outputs
                var outputCount = ideals[0].Count();
                //Init the basic network
                BasicNetwork network = new BasicNetwork();
                //Add layers (input, hidden & output) - Activation function is the sigmoid
                //We set true in ther layer constructor to include bias
                //Input layer has 7 total nodes 
                network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, inputCount));
                //Hidden can be as many as you want - the more you have increases propagation times 
                //Optimisation is usuall trial & error
                //This layer helps us find more complex patterns on the data
                network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, (hiddenLayerCount != null) ? hiddenLayerCount.Value : (inputCount * 3)));
                //Output layer is the count of ideal values
                network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, outputCount));
                network.Structure.FinalizeStructure();
                //Randomize the node weights to start the training
                network.Reset();
                return network;
            }
            return null;
        }

        public static BasicNetwork TrainNetwork(BasicNetwork network, double[][] trainingData, double[][] ideals, 
            int maxIterationsBeforeCompletion = 5000, double tolerance = 0.001, string connectionId = null,
            Func<string, string, bool> OutputData = null)
        {
            //Check we have data and a model to train
            if (trainingData.Any() && ideals.Any() && network != null)
            {
                //Create Dataset - data and correct classifications (matched by position)
                INeuralDataSet trainingSet = new BasicNeuralDataSet(trainingData, ideals);
                //Proagate the data through the network
                ITrain train = new ResilientPropagation(network, trainingSet);
                //Set the iteration count to 0
                var epoch = 0;
                //Train
                do
                {
                    train.Iteration();
                    //If the delegate is defined, output the progress to it
                    if (OutputData != null) OutputData(connectionId, "Epoch #" + epoch + " Error:" + train.Error);
                    epoch++;
                } while ((epoch < maxIterationsBeforeCompletion) && (train.Error > tolerance));
            }
            //Return the trained network
            return network;
        }

        public static void TestNetwork(BasicNetwork network, double[][] trainingData, double[][] ideals, 
            string connectionId, Func<string, string, bool> OutputData = null)
        {
            //Create Dataset - data and correct classifications (matched by position)
            var trainingSet = new BasicNeuralDataSet(trainingData, ideals);
            foreach (var pair in trainingSet)
            {
                var output = network.Compute(pair.Input);
                //Columns: |Rain|,|Wind|,|Sun|,|Cloud/Overcast|,|Snow|
                if (OutputData != null) OutputData(connectionId, FormatOutput(pair, output));
            }
        }

        private static string FormatOutput(IMLDataPair pair, IMLData output)
        {
            var sb = new StringBuilder();
            sb.Append("|Rain|,|Wind|,|Sun|,|Cloud/Overcast|,|Snow|" + Environment.NewLine);
            for (int i = 0; i < pair.Input.Count; i++)
            {
                sb.Append(pair.Input[i]);
                if(i < (pair.Input.Count - 1)) sb.Append(",");
            }
            sb.Append(Environment.NewLine);
            sb.Append($"Ideal Classification: {pair.Ideal[0]} ");
            sb.Append($"Prediction: {output[0]}");
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }
    }
}
