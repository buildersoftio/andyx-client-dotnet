using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Client.Configurations
{
    public class ReaderOptions
    {
        public string Component { get; set; }
        public string Book { get; set; }

        public string Name { get; set; }
        public ReaderTypes ReaderType { get; set; }
        public ReaderAs ReaderAs { get; set; }
        public SchemaOptions Schema { get; set; }
    }

    public enum ReaderTypes
    {
        /// <summary>
        /// Only one reader
        /// </summary>
        Exclusive,
        /// <summary>
        /// One reader with one backup
        /// </summary>
        Failover,
        /// <summary>
        /// Shared to more than one reader.
        /// </summary>
        Shared
    }

    public enum ReaderAs
    {
        Subscription = 0,
        Consumer = 1,
    }

}
