using System;

namespace MFW3D.Renderable
{
	/// <summary>
	/// ����Ⱦ����
	/// </summary>
	interface IRenderable : IDisposable
	{
		void Initialize(DrawArgs drawArgs);
		void Update(DrawArgs drawArgs);
		void Render(DrawArgs drawArgs);
	}
}
