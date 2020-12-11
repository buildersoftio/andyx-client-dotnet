using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Client.Configurations
{
    public class WriterOptions
    {
        public string Name { get; set; }
        public WriterTypes WriterType { get; set; }
        public SchemaTypes SchemaType { get; set; }
    }

    public enum WriterTypes
    {
        StreamAndStore = 0,
        StreamAfterStored = 1
    }

    public enum SchemaTypes
    {
        Json,
        Text,
        File
    }
}
