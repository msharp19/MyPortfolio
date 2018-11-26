using BrightWire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BrightWire.ExecutionGraph.GraphFactory;

namespace CNNV2
{

    public class NetworkConfig
    {
        public float LEARNING_RATE { get; set; }
        public int BATCH_SIZE { get; set; }
        public int HIDDEN_LAYER_SIZE { get; set; }
        public int TRAINING_ITERATIONS { get; set; }
        public IErrorMetric ERROR_METRIC { get; set; }
        public bool ZERO_BIAS { get; set; }
        public float STANDARD_DEVIATION { get; set; }
        public GaussianVarianceCalibration GAUSSIAN_VARIANCE_CALIBRATION { get; set; }

        public NetworkConfig()
        {
            HIDDEN_LAYER_SIZE = 1024;
            BATCH_SIZE = 64;
            TRAINING_ITERATIONS = 20;
            LEARNING_RATE = 0.05f;
            ZERO_BIAS = false;
            STANDARD_DEVIATION = 0.1f;
            GAUSSIAN_VARIANCE_CALIBRATION = GaussianVarianceCalibration.SquareRoot2N;
        }
    }
}
