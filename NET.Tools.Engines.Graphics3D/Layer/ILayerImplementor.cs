﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NET.Tools.Engines.Graphics3D.Layer
{
    internal interface ILayerImplementor
    {
        IMeshImplementor MeshImplementor { get; }
        IMatrixImplementor MatrixImplementor { get; }
    }
}