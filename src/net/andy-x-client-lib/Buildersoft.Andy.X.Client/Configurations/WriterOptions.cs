using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Client.Configurations
{
    public class WriterOptions
    {
        public string Component { get; set; }
        public string Book { get; set; }

        public string Name { get; set; }
        public WriterTypes WriterType { get; set; }
        public DataTypes DataType { get; set; }
        public SchemaOptions Schema { get; set; }

    }

    public enum WriterTypes
    {
        StreamAndStore = 0,
        StreamAfterStored = 1
    }

    public enum DataTypes
    {
        Json,
        Text,
        File
    }
}
