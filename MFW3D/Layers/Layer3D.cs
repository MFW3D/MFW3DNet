using MFW3D.Renderable;
using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MFW3D.Layers
{
    public class Layer3D : RenderableObject
    {
        public Layer3D(string name,
            float latitude,
            float longitude,
            float layerRadius,
            float scaleFactor,
            string meshFilePath,
            Quaternion orientation) : base(name, 
                MathEngine.SphericalToCartesian(latitude, longitude, layerRadius), orientation) 
        {

        }

        public override void Initialize(DrawArgs drawArgs)
        { }

        public override void Update(DrawArgs drawArgs)
        { }

        public override void Render(DrawArgs drawArgs)
        { }
        public override void Dispose()
        {

        }
        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            return true;
        }
    }
}
