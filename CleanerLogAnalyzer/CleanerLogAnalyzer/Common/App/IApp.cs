﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public interface IApp
    {
        ILogger Logger { get; }
    }
}