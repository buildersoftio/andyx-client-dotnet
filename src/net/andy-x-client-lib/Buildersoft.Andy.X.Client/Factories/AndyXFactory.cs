using Buildersoft.Andy.X.Client.Abstraction;
using Buildersoft.Andy.X.Client.Builders;
using Buildersoft.Andy.X.Client.Configurations;
using System;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Client.Factories
{
    public class AndyXFactory : AndyXBuilder, IAndyXFactory
    {
        private readonly AndyXBuilder _andyXBuilder;

        public AndyXFactory(AndyXBuilder andyXBuilder)
        {
            _andyXBuilder = andyXBuilder;
        }

        public AndyXClient CreateClient()
        {
            return InitializeBuilder() as AndyXClient;
        }
    }
}
