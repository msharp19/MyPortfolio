using BrightWire;
using BrightWire.ExecutionGraph;
using BrightWire.Models;
using BrightWire.TrainingData.WellKnown;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNNV2
{
    public class CNNManager
    {

        public static IDataSource BuildTensors(GraphFactory graph, IDataSource existing, IReadOnlyList<Mnist.Image> images)
        {
            var dataTable = BrightWireProvider.CreateDataTableBuilder();
            dataTable.AddColumn(ColumnType.Tensor, "Image");
            dataTable.AddColumn(ColumnType.Vector, "Target", true);
            foreach (var image in images)
            {
                var data = image.AsFloatTensor;
                dataTable.Add(data.Tensor, data.Label);
            }
            if (existing != null) return existing.CloneWith(dataTable.Build());
            else return graph.CreateDataSource(dataTable.Build());
        }

        public static float TrainCNN(string dataFolderPath, string outputModelPath)
        {
            using (var lap = BrightWireProvider.CreateLinearAlgebra(false))
            {
                var graph = new GraphFactory(lap);
                var dataset = CreateDataset(graph, dataFolderPath);
                var errorMetric = graph.ErrorMetric.OneHotEncoding;
                var config = new NetworkConfig();
                config.ERROR_METRIC = errorMetric;
                var engine = BuildNetwork(config, graph, dataset, outputModelPath);
                var bestGraph = TrainModel(engine, config, dataset, outputModelPath);
                var executionEngine = graph.CreateEngine(bestGraph ?? engine.Graph);
                var output = executionEngine.Execute(dataset.TestData);
                return output.Average(o => o.CalculateError(errorMetric));
            }
        }

        private static DataSet BuildTestSet(GraphFactory graph, Bitmap testImage)
        {
            var images = new List<Mnist.Image>();
            var bytes = ImageFunctions.ImageToRepresentativeBytes(testImage);
            var mnist = new Mnist.Image(bytes, 1);
            images.Add(mnist);
            var dataSource = BuildTensors(graph, null, images);
            return new DataSet() {
                TestData = dataSource,
                TestRowCount = dataSource.RowCount
            };
        }

        public static int TestItems(string modelPath, Bitmap testImage)
        {
            using (var lap = BrightWireProvider.CreateLinearAlgebra(false))
            {
                var graph = new GraphFactory(lap);
                DataSet testDataset = BuildTestSet(graph,testImage);
                var errorMetric = graph.ErrorMetric.OneHotEncoding;
                var config = new NetworkConfig();
                config.ERROR_METRIC = errorMetric;
                var engine = LoadTestingNetwork(modelPath, graph);
                var executionEngine = graph.CreateEngine(engine.Graph);
                var output = executionEngine.Execute(testDataset.TestData);
                return GetLargestPercent(output[0]);
            }
        }

        private static int GetLargestPercent(ExecutionResult output)
        {
            float largest = 0f;
            int maxPercent = -1;
            var labels = output.Output[0].Data;
            for(int i= 0;i<labels.Length;i++)
            {
                if (labels[i] > largest)
                {
                    largest = labels[i];
                    maxPercent = i;
                }
            }
            return maxPercent;
        }

        public static ExecutionGraph TrainModel(IGraphTrainingEngine engine, NetworkConfig config, DataSet dataset, string outputModelPath)
        {
            BrightWire.Models.ExecutionGraph bestGraph = null;
            engine.Train(config.TRAINING_ITERATIONS, dataset.TestData, config.ERROR_METRIC, model => {
                bestGraph = model.Graph;
                if (!String.IsNullOrWhiteSpace(outputModelPath))
                {
                    using (var file = new FileStream(outputModelPath, FileMode.Create, FileAccess.Write))
                    {
                        Serializer.Serialize(file, model);
                    }
                }
            });
            return bestGraph;
        }

        public static IGraphTrainingEngine BuildNetwork(NetworkConfig config, GraphFactory graph, DataSet dataset, string outputModelPath = null)
        {
            graph.CurrentPropertySet
                .Use(graph.GradientDescent.Adam)
                .Use(graph.GaussianWeightInitialisation(config.ZERO_BIAS, config.STANDARD_DEVIATION, config.GAUSSIAN_VARIANCE_CALIBRATION));
            var engine = graph.CreateTrainingEngine(dataset.TrainData, config.LEARNING_RATE, config.BATCH_SIZE);
            if (!String.IsNullOrWhiteSpace(outputModelPath) && File.Exists(outputModelPath)) engine = LoadTrainingNetwork(outputModelPath, graph, config, dataset);
            else graph = CreateStandardNetwork(engine, graph, config, dataset);
            engine.LearningContext.ScheduleLearningRate(15, config.LEARNING_RATE / 2);
            return engine;
        }

        public static GraphFactory CreateStandardNetwork(IGraphTrainingEngine engine, GraphFactory graph, NetworkConfig config, 
            DataSet dataset)
        {
            graph.Connect(engine)
                  .AddConvolutional(filterCount: 16, padding: 2, filterWidth: 5, filterHeight: 5, stride: 1, shouldBackpropagate: false)
                  .Add(graph.LeakyReluActivation())
                  .AddMaxPooling(filterWidth: 2, filterHeight: 2, stride: 2)
                  .AddConvolutional(filterCount: 32, padding: 2, filterWidth: 5, filterHeight: 5, stride: 1)
                  .Add(graph.LeakyReluActivation())
                  .AddMaxPooling(filterWidth: 2, filterHeight: 2, stride: 2)
                  .Transpose()
                  .AddFeedForward(config.HIDDEN_LAYER_SIZE)
                  .Add(graph.LeakyReluActivation())
                  .AddDropOut(dropOutPercentage: 0.5f)
                  .AddFeedForward(dataset.TrainData.OutputSize)
                  .Add(graph.SoftMaxActivation())
                  .AddBackpropagation(config.ERROR_METRIC);
            return graph;
        }

        public static IGraphTrainingEngine LoadTrainingNetwork(string path, GraphFactory graph, NetworkConfig config, 
            DataSet dataset)
        {
            IGraphTrainingEngine engine = null;
            using (var file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var model = Serializer.Deserialize<GraphModel>(file);
                engine = graph.CreateTrainingEngine(dataset.TrainData, model.Graph, config.LEARNING_RATE, config.BATCH_SIZE);
            }
            return engine;
        }

        public static IGraphEngine LoadTestingNetwork(string path, GraphFactory graph)
        {
            IGraphEngine engine = null;
            using (var file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var model = Serializer.Deserialize<GraphModel>(file);
                engine = graph.CreateEngine(model.Graph);
            }
            return engine;
        }

        public static DataSet CreateDataset(GraphFactory graph, string folderPath)
        {
            var trainLabelPath = Path.Combine(folderPath, "train-labels.idx1-ubyte");
            var trainDataPath = Path.Combine(folderPath, "train-images.idx3-ubyte");
            var testLabelPath = Path.Combine(folderPath, "t10k-labels.idx1-ubyte");
            var testDataPath = Path.Combine(folderPath, "t10k-images.idx3-ubyte");
            var trainingData = BuildTensors(graph, null, Mnist.Load(trainLabelPath, trainDataPath));
            var testData = BuildTensors(graph, trainingData, Mnist.Load(testLabelPath, testDataPath));
            return new DataSet() {
                TestData = testData,
                TrainData = trainingData,
                TrainingRowCount = trainingData.RowCount,
                TestRowCount = testData.RowCount
            };
        }
    }
}
