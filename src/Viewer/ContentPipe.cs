using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using BubblePony.GLHandle;


namespace Quad64
{
    static class ContentPipe
    {
        public static Texture2D LoadTexture(string filepath)
        {
            return LoadTexture(new Bitmap(filepath));
        }

		public static Texture2D LoadTexture(Bitmap bitmap)
		{
			if (null == bitmap) return null;

			GraphicsHandle.Texture handle = default(GraphicsHandle.Texture);
			handle.Gen();
			LoadTextureHandle(bitmap, handle);
			return new Texture2D(handle, bitmap.Width, bitmap.Height);
		}
		private static void LoadTextureHandle(Bitmap bitmap, int id)
        {
            BitmapData bmpData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.BindTexture(TextureTarget.Texture2D, id);
            GL.TexImage2D(TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                bitmap.Width, bitmap.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte,
                bmpData.Scan0);
            bitmap.UnlockBits(bmpData);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter,
                (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter,
                (int)TextureMagFilter.Linear);
		}
		internal static void UpdateHandle(
			TextureFormats.Raw bitmap,
			ref GraphicsHandle.Texture handle)
		{

			using (var bmp = bitmap.ToBitmap())
				if (null == (object)bmp)
				{
					handle.Delete();
				}
				else
					LoadTextureHandle(bmp, handle);
		}
		public static Texture2D LoadTexture(TextureFormats.Raw bitmap)
		{
			if (null == bitmap)
				return null;
			else
			{
				if (bitmap.CreateHandle(out GraphicsHandle.Texture handle))
					UpdateHandle(bitmap, ref handle);

				return handle.Alive != GraphicsHandle.Null ? new Texture2D(handle, bitmap.Width, bitmap.Height) : null;
			}
		}
		public static unsafe Bitmap ToBitmap(this TextureFormats.Raw bitmap)
		{
			if (null == (object)bitmap)
				return null;

			fixed (byte* argb = bitmap.ARGB)
				return new Bitmap(
					bitmap.Width,
					bitmap.Height,
					(int)bitmap.Width << 2,
					System.Drawing.Imaging.PixelFormat.Format32bppArgb, 
					(IntPtr)argb);
		}
	}
}
