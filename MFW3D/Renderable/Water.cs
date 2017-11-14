/********************************************************************
 * Ocean/Water Rendering code obtained from MDXInfo Site
 * Code added by Tisham Dhar(aka what_nick) in hope of later inspiration
 * As this code matures specific water bodies can be added with given 
 * water level or global water data set constructed using SRTM water
 * boundaries data.
 * 
 * 
 * ******************************************************************/


using System;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Runtime.InteropServices;
using System.Drawing;

namespace MFW3D.Renderable
{
	class Water : ModelFeature
	{
		#region 成员
		private const string meshFilename = "Data/water.x";
		private const string textureFilename = "Data/water.dds";
        //The cloud file is missing
        private const string cloudFilename = "Data/cloud.dds";
		private string effectFilename = "Shaders/waterbump.fx";
		private CubeTexture texCube;
		private static float waterTime = 0;
        protected Effect effect;
        //protected Mesh mesh;
        protected Texture texture;
        private bool isBumpmapped = false;
		#endregion

		#region 方法
		public Water(string name,World parentWorld,bool isBumpmapped,float lat,float lon,float alt,float scaleFactor) : 
            base(name,parentWorld,meshFilename,lat,lon,alt,scaleFactor,90.0f,0.0f,0.0f)
		{
            this.isBumpmapped = isBumpmapped;
            if (!isBumpmapped)
                effectFilename = @"Shaders/water.fx";
		}

        public override void Initialize(DrawArgs drawArgs)
        {
            base.Initialize(drawArgs);
            Device device = drawArgs.device;
            //load the mesh

            //Vertex elements that describe the new format of the mesh
            VertexElement[] elements = new VertexElement[]
			{
				new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
				new VertexElement(0, 12, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
				new VertexElement(0, 24, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
				new VertexElement(0, 36, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Tangent, 0),
				new VertexElement(0, 48, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.BiNormal, 0),
				VertexElement.VertexDeclarationEnd,
			};
            VertexDeclaration decl = new VertexDeclaration(device, elements);
            Mesh tempMesh = m_meshElems[0].mesh.Clone(MeshFlags.Managed, elements, device);
            m_meshElems[0].mesh.Dispose();
            m_meshElems[0].mesh = tempMesh;

            /* Performs tangent frame computations on a mesh. Tangent, binormal, and optionally normal vectors are generated. 
              * Singularities are handled as required by grouping edges and splitting vertices.
             */
            m_meshElems[0].mesh.ComputeTangentFrame(0);

            //load the effect
            effect = Effect.FromFile(device, effectFilename, null, null, ShaderFlags.Debug | ShaderFlags.PartialPrecision, null);

            //load the texture
            texCube = TextureLoader.FromCubeFile(device, cloudFilename);
            texture = TextureLoader.FromFile(device, textureFilename);
            isInitialized = true;
        }
        /*
        public override void Update(DrawArgs drawArgs)
        {
            if (!isInitialized)
                this.Initialize(drawArgs);
        }
         */ 
        /*
		public void Update(Matrix matView, Matrix matProj)
		{
            /*
			matWorld.Translate(0.0f, -80.0f, 0.0f);
			this.matView = matView;
			this.matProj = matProj;
            
		}
        */

        new protected bool IsVisible(MFW3D.Camera.CameraBase camera)
        {
            if(base.IsVisible(camera))
                //donot render at high altitudes
                if (camera.Altitude > 60000)
                    return false;
            return true;
        }

		public override void Render(DrawArgs drawArgs)
		{
            if (errorMsg != null)
            {
                //System.Windows.Forms.MessageBox.Show( errorMsg, "Model failed to load.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                errorMsg = null;
                IsOn = false;
                isInitialized = false;
                return;
            }

            if (!IsVisible(drawArgs.WorldCamera))
            {
                // Mesh is not in view, unload it to save memory
                if (isInitialized)
                    Dispose();
                return;
            }

            if (!isInitialized)
                return;
            
            //Donot render for other planets
            if (!drawArgs.CurrentWorld.IsEarth)
                return;


            
            drawArgs.device.RenderState.CullMode = Cull.None;
            drawArgs.device.RenderState.Lighting = true;
            drawArgs.device.RenderState.AmbientColor = 0x808080;
            drawArgs.device.RenderState.NormalizeNormals = true;

            drawArgs.device.Lights[0].Diffuse = Color.FromArgb(255, 255, 255);
            drawArgs.device.Lights[0].Type = LightType.Directional;
            drawArgs.device.Lights[0].Direction = new Vector3(1f, 1f, 1f);
            drawArgs.device.Lights[0].Enabled = true;

            drawArgs.device.SamplerState[0].AddressU = TextureAddress.Wrap;
            drawArgs.device.SamplerState[0].AddressV = TextureAddress.Wrap;

            drawArgs.device.RenderState.AlphaBlendEnable = true;
            drawArgs.device.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
            drawArgs.device.TextureState[0].ColorOperation = TextureOperation.SelectArg1;

            // Put the light somewhere up in space
            drawArgs.device.Lights[0].Position = new Vector3(
                (float)worldXyz.X * 2f,
                (float)worldXyz.Y * 1f,
                (float)worldXyz.Z * 1.5f);
            
            Matrix currentWorld = drawArgs.device.Transform.World;
            drawArgs.device.Transform.World = Matrix.RotationX((float)MathEngine.DegreesToRadians(RotX));
            drawArgs.device.Transform.World *= Matrix.RotationZ((float)MathEngine.DegreesToRadians(RotY));
            drawArgs.device.Transform.World *= Matrix.RotationZ((float)MathEngine.DegreesToRadians(RotZ));
            drawArgs.device.Transform.World *= Matrix.Scaling(Scale, Scale, Scale);

            // Move the mesh to desired location on earth
            if (IsVertExaggerable)
                vertExaggeration = World.Settings.VerticalExaggeration;
            else vertExaggeration = 1;
            drawArgs.device.Transform.World *= Matrix.Translation(0, 0, (float)drawArgs.WorldCamera.WorldRadius + Altitude * vertExaggeration);
            drawArgs.device.Transform.World *= Matrix.RotationY((float)MathEngine.DegreesToRadians(90 - Latitude));
            drawArgs.device.Transform.World *= Matrix.RotationZ((float)MathEngine.DegreesToRadians(Longitude));


            drawArgs.device.Transform.World *= Matrix.Translation(
                (float)-drawArgs.WorldCamera.ReferenceCenter.X,
                (float)-drawArgs.WorldCamera.ReferenceCenter.Y,
                (float)-drawArgs.WorldCamera.ReferenceCenter.Z
                );

            Device device = drawArgs.device;
            // Draw the mesh with effect
            if (isBumpmapped)
                setupBumpEffect(drawArgs);
            else
                setupReflectionEffect(drawArgs);

			//render the effect
            bool alphastate = device.RenderState.AlphaBlendEnable;
			device.RenderState.AlphaBlendEnable = true;
			effect.Begin(0);
			effect.BeginPass(0);
             
            //drawArgs.device.SetTexture(0, texture);
			m_meshElems[0].mesh.DrawSubset(0);
            
			effect.EndPass();
			effect.End();
			device.RenderState.AlphaBlendEnable = alphastate;
            

            drawArgs.device.Transform.World = currentWorld;
            drawArgs.device.RenderState.Lighting = false;
		}

        private void setupBumpEffect(DrawArgs drawArgs)
        {
            float time = (float)(Environment.TickCount * 0.001);
            waterTime += 0.002f;
            //Calculate the matrices
            Matrix modelViewProj = drawArgs.device.Transform.World * drawArgs.device.Transform.View * drawArgs.device.Transform.Projection;
            Matrix modelViewIT = drawArgs.device.Transform.World * drawArgs.device.Transform.View * drawArgs.device.Transform.Projection;
            modelViewIT.Invert();
            modelViewIT = Matrix.TransposeMatrix(modelViewIT);
            //set the technique
            effect.Technique = "water";
            //set the texturs
            effect.SetValue("texture0", texture);
            effect.SetValue("texture1", texCube);
            //set the matrices
            effect.SetValue("ModelViewProj", modelViewProj);
            effect.SetValue("ModelWorld", drawArgs.device.Transform.World);
            //set eye position
            effect.SetValue("eyePos", new Vector4(450.0f, 250.0f, 750.0f, 1.0f));
            //set the light position
            effect.SetValue("lightPos", new Vector4((float)(300 * Math.Sin(time)), 40.0f, (float)(300 * Math.Cos(time)), 1.0f));
            //set the time
            effect.SetValue("time", waterTime);

        }

        private void setupReflectionEffect(DrawArgs drawArgs)
        {
            //Calculate the matrices
            Matrix worldViewProj = drawArgs.device.Transform.World * drawArgs.device.Transform.View * drawArgs.device.Transform.Projection;
            Matrix worldIT = drawArgs.device.Transform.World;
            worldIT.Invert();
            worldIT = Matrix.TransposeMatrix(worldIT);
            Matrix viewI = drawArgs.device.Transform.View;
            viewI.Invert();
            //Point3d sunPosition = SunCalculator.GetGeocentricPosition(currentTime);

            //set the technique
            effect.Technique = "Textured";
            //set the texturs
            effect.SetValue("ColorTexture", texture);
            effect.SetValue("CubeEnvMap", texCube);
            //set the matrices
            effect.SetValue("WorldViewProj", worldViewProj);
            effect.SetValue("WorldIT", worldIT);
            effect.SetValue("World", drawArgs.device.Transform.World);
            effect.SetValue("ViewI", viewI);
        }
        /*
		public void Render(Device device, Texture waterTexture)
		{
			float time = (float)(Environment.TickCount * 0.001);
			waterTime += 0.002f;
			//Calculate the matrices
			Matrix modelViewProj = matWorld * matView * matProj;
			Matrix modelViewIT = matWorld * matView * matProj;
			modelViewIT.Invert();
			modelViewIT = Matrix.TransposeMatrix(modelViewIT);
			//set the technique
			effect.Technique = "water";
			//set the texturs
			effect.SetValue("texture0", waterTexture);
			effect.SetValue("texture1", texCube);
			//set the matrices
			effect.SetValue("ModelViewProj", modelViewProj);
			effect.SetValue("ModelWorld", matWorld);
			//set eye position
			effect.SetValue("eyePos", new Vector4(450.0f, 250.0f, 750.0f, 1.0f));
			//set the light position
			effect.SetValue("lightPos", new Vector4((float)(300 * Math.Sin(time)), 40.0f, (float)(300 * Math.Cos(time)), 1.0f));
			//set the time
			effect.SetValue("time", waterTime);

			//render the effect
			//device.RenderState.AlphaBlendEnable = true;
			effect.Begin(0);
			effect.BeginPass(0);
			mesh.DrawSubset(0);
			effect.EndPass();
			effect.End();
			//device.RenderState.AlphaBlendEnable = false;
		}
        */
        public override void Dispose()
        {
            //set initialization state to false
            this.isInitialized = false;
            
            //Dispose Textures
            if (this.texCube != null)
            {
                this.texCube.Dispose();
                this.texCube = null;
            }
            if (this.texture != null)
            {
                this.texture.Dispose();
                this.texture = null;
            }
            //Dispose Effects
            if (this.effect != null)
            {
                this.effect.Dispose();
                this.effect = null;
            }
            //Dispose Mesh
            base.Dispose();
        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            return false;
        }

		#endregion
	}
}