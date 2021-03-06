﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NET.Tools.Engines.Graphics3D.Direct3D11
{
    internal sealed class Direct3D11LayerImplementor : ILayerImplementor
    {
        private static Direct3D11LayerImplementor instance = null;
        public static Direct3D11LayerImplementor GetInstance()
        {
            if (instance == null)
            {
                instance = new Direct3D11LayerImplementor();
            }

            return instance;
        }

        private Direct3D11LayerImplementor()
        {
        }

        public IMeshImplementor MeshImplementor
        {
            get { return Direct3D11MeshImplementor.GetInstance(); }
        }


        public IMatrixImplementor MatrixImplementor
        {
            get { return Direct3D11MatrixImplementor.GetInstance(); }
        }

        public IViewportImplementor ViewportImplementor
        {
            get { return Direct3D11ViewportImplementor.GetInstance(); }
        }


        public IMaterialImplementor MaterialImplementor
        {
            get { return Direct3D11MaterialImplementor.GetInstance(); }
        }


        public ILightImplementor LightImplementor
        {
            get { return Direct3D11LightImplementor.GetInstance(); }
        }
    }
}
