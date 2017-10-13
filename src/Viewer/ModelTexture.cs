using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BubblePony.ExportUtility;
using Export = BubblePony.Export;
using BubblePony.GLHandle;

namespace Quad64
{
    public sealed class Texture2D : Export.Reference<Texture2D>,Export.Reference
    {
        private GraphicsHandle.Texture id;
        private int width, height;

        public int ID { get { return id; } }
        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public int TextureParamS { get; set; }
        public int TextureParamT { get; set; }

        internal Texture2D(GraphicsHandle.Texture handle, int width, int height)
        {
            this.id = handle;
            this.width = width;
            this.height = height;
        }

		public static Export.ReferenceRegister<Texture2D> ExportRegister;
		Export.TypeReference Export.Reference.API() { return ExportRegister.Singleton; }
		unsafe void Export.Reference<Texture2D>.API(Export.Exporter ex)
		{
			{
				int* members = stackalloc int[5];
				members[0] = id;
				members[1] = width;
				members[2] = height;
				members[3] = TextureParamS;
				members[4] = TextureParamT;
				ex.Value(members,5);
			}
		}
	}
}
