using ANN;
using CNNV2;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTester
{
    //This is for testing the network
    public class Program
    {
        public static void Main(string[] args)
        {
            //RunANN();            
            RunCNN();

            Console.Read();
        }

        public static bool OutputEpoch(string connectionId, string message, bool limit)
        {
            Console.WriteLine(message);
            return true;
        }

        public static void RunCNN()
        {
            //Get images
            var directory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            var path = Path.Combine(directory, "training-set");
            var virtualFolder = new Uri(path).LocalPath;
            string outputModelPath = Path.Combine( path, "model.mod");
            CNNManager.TrainCNN(path, outputModelPath);
        }

        public static void RunANN()
        {
            //Get the raw data
            var rawData = NetworkFactory.GetInputData();
            var ideals = NetworkFactory.GetIdealValues();
            //Format the data so it can be used in a neural network - all columns will have a value between 0-1 (floating point)
            var formattedData = NetworkFactory.FormatInputValues(rawData);
            //Create Network
            var network = NetworkFactory.CreateNetwork(formattedData, ideals, 40);
            //Train network
            var trainedNetwork = NetworkFactory.TrainNetwork(network, formattedData, ideals, 20000, 0.001, string.Empty, OutputEpoch);
            //Finally test the network
            NetworkFactory.TestNetwork(network, formattedData, ideals, string.Empty);
        }
    }
}
