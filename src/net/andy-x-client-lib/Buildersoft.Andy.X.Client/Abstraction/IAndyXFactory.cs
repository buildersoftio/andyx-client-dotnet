﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildersoft.Andy.X.Client.Abstraction
{
    public interface IAndyXFactory
    {
        AndyXClient CreateClient();
    }
}
