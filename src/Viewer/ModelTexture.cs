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
        private TextureFormats.Raw raw;
        private int width, height;

		[System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
		public int ID { get { return raw.GetLoadedHandle(); } }

        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public int TextureParamS { get; set; }
        public int TextureParamT { get; set; }
		public TextureFormats.Raw Raw => raw;

        internal Texture2D(TextureFormats.Raw handle, int width, int height)
        {
			if (null == handle) throw new ArgumentNullException("handle");

            this.raw = handle;
            this.width = width;
            this.height = height;
        }

		public static Export.ReferenceRegister<Texture2D> ExportRegister;
		Export.TypeReference Export.Reference.API() { return ExportRegister.Singleton; }
		unsafe void Export.Reference<Texture2D>.API(Export.Exporter ex)
		{
			{
				int* members = stackalloc int[4];
				members[0] = width;
				members[1] = height;
				members[2] = TextureParamS;
				members[3] = TextureParamT;
				ex.Value(members,5);
			}
		}
	}
}
