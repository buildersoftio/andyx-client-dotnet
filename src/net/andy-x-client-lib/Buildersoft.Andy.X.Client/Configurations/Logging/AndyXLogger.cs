using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Buildersoft.Andy.X.Client.Configurations.Logging
{
    public class AndyXLogger
    {
        private readonly ILoggerFactory _loggerFactory = null;

        public AndyXLogger()
        {
            _loggerFactory = new LoggerFactory();
        }

        public AndyXLogger(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public ILoggerFactory GetLoggerFactory()
        {
            return _loggerFactory;
        }
    }
}
