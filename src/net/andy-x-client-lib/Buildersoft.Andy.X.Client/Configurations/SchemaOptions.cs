using NJsonSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Client.Configurations
{
    public class SchemaOptions
    {
        public bool SchemaValidationStatus { get; set; }
        public string Schema { get; private set; }

        public SchemaOptions()
        {
            SchemaValidationStatus = false;
        }

        public void SetSchema<T>() where T : class
        {
            Schema = JsonSchema.FromType<T>().ToJson();
        }
    }
}
