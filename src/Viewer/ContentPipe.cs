using System;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using BubblePony.GLHandle;


namespace Quad64
{
    static class ContentPipe
	{
		
        public static Texture2D LoadTexture(string filepath, int s, int t, object tag = null)
        {
			using(var bitmap= new Bitmap(filepath))
	            return LoadTexture(bitmap,s,t,tag);
        }
		public unsafe static Texture2D LoadTexture(Bitmap bitmap, int s, int t, object tag=null)
		{
			return null == (object)bitmap ? null : LoadTexture(
				TextureFormats.FromBitmap(bitmap, tag),
				s, t);
		}
		private static void LoadTextureHandlePostTexImage2D()
		{
			GL.TexParameter(TextureTarget.Texture2D,
				TextureParameterName.TextureMinFilter,
				(int)TextureMinFilter.Linear);
			GL.TexParameter(TextureTarget.Texture2D,
				TextureParameterName.TextureMagFilter,
				(int)TextureMagFilter.Linear);
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
			LoadTextureHandlePostTexImage2D();
		}
		private unsafe static void LoadTextureHandle(TextureFormats.Raw bitmap, int id)
		{
			GL.BindTexture(TextureTarget.Texture2D, id);
			fixed(byte *buf=bitmap.ARGB)
			GL.TexImage2D(TextureTarget.Texture2D,
				0,
				PixelInternalFormat.Rgba,
				bitmap.Width, bitmap.Height, 0,
				OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
				PixelType.UnsignedByte,
				(IntPtr)buf);
			LoadTextureHandlePostTexImage2D();
		}
		internal static void UpdateHandle(
			TextureFormats.Raw bitmap,
			ref GraphicsHandle.Texture handle)
		{
			/*
			using (var bmp = bitmap.ToBitmap())
				if (null == (object)bmp)
				{
					handle.Delete();
				}
				else*/
			LoadTextureHandle(/*bmp*/ bitmap, handle);
		}
		public static Texture2D LoadTexture(TextureFormats.Raw bitmap, int s, int t)
		{
			if (null == bitmap)
				return null;
			else
			{
				if (bitmap.CreateHandle(out GraphicsHandle.Texture handle))
					UpdateHandle(bitmap, ref handle);

				return handle.Alive != GraphicsHandle.Null ? 
					new Texture2D(bitmap, bitmap.Width, bitmap.Height) { TextureParamS = s, TextureParamT = t, } : 
					null;
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
