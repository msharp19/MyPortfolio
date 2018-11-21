﻿using ANN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkTester
{
    //This is for testing the network
    public class Program
    {
        public static void Main(string[] args)
        {
            //Get the raw data
            var rawData = NetworkFactory.GetInputData();
            var ideals = NetworkFactory.GetIdealValues();
            //Format the data so it can be used in a neural network - all columns will have a value between 0-1 (floating point)
            var formattedData = NetworkFactory.FormatInputValues(rawData);
            //Create Network
            var network = NetworkFactory.CreateNetwork(formattedData, ideals, 40);
            //Train network
            var trainedNetwork = NetworkFactory.TrainNetwork(network, formattedData, ideals, 20000, 0.001, OutputEpoch);
            //Finally test the network
            NetworkFactory.TestNetwork(network, formattedData, ideals);
            Console.Read();
        }

        public static bool OutputEpoch(string message)
        {
            Console.WriteLine(message);
            return true;
        }


        /*
         * pair.Input[0] + "," + pair.Input[1]
                + ", actual=" + output[0] + ",ideal=" + pair.Ideal[0]
         */
    }
}
