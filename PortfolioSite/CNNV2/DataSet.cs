using BrightWire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CNNV2
{
    public class DataSet
    {
        public IDataSource TrainData { get; set; }
        public IDataSource TestData { get; set; }
        public int TrainingRowCount { get; set; }
        public int TestRowCount { get; set; }
    }
}
