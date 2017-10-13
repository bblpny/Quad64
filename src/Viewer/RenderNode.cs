using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Quad64
{
	public sealed class RenderObject
	{
		private System.WeakReference ListReference;
		public RenderObject Next, Prev;
		public Model3D Model;
		public object Object;
		public Color4b BoundsColor;
		public Transform Transform;
		public RenderList List => null == (object)ListReference ? null : (RenderList)ListReference.Target;
		public bool Alive => null != ListReference;
		public sealed override string ToString()
		{
			return (Object ?? string.Empty).ToString();
		}
		private void GetNodeOffset(uint Count, uint Offset, out RenderObject Out)
		{
			uint Back;

			if (Offset > Count)
				Offset %= Count;
			else if (Offset == Count)
				Offset = 0;

			Back = Count - (Offset + 1u);

			if (Back <= Offset)
			{
				Out = this;
				while (0 != Back--) Out = Out.Prev;
			}
			else
			{
				Out = this;
				while (0 != Offset--) Out = Out.Next;
			}
		}
		public RenderObject this[uint Offset]
		{
			get
			{
				var Layer = this.List;
				RenderObject Out;
				if (null == Layer || 0 == Layer.Count)
					Out = null;
				else
					GetNodeOffset(Layer.Count, Offset, out Out);
				return Out;
			}
		}
		public RenderObject this[int Offset]
		{
			get
			{
				var Layer = this.List;
				RenderObject Out;
				if (null == Layer || 0 == Layer.Count)
					Out = null;
				else
					GetNodeOffset(Layer.Count, Offset < 0 ? ((uint)((int)Layer.Count + (Offset % (int)Layer.Count))) : (uint)Offset, out Out);
				return Out;
			}
		}
		public bool IsOwnedBy(RenderList List)
		{
			return null == (object)List ?
				null == (object)ListReference :
				(object)ListReference == List.Weak;
		}


		private static class Dump
		{
			public static RenderObject Top;
			public static uint Size;
			public static object Lock = new object();
		}
		private RenderObject() { }
		private static RenderObject Alloc()
		{
			RenderObject Out = null;
			bool Got;
			lock (Dump.Lock)
				if ((Got = !(0 == Dump.Size)))
				{
					Out = Dump.Top;
					if (0 == --Dump.Size)
						Dump.Top = null;
					else
						Dump.Top = Out.Prev;
				}
			if (Got)
				Out.Prev = null;
			else
				Out = new RenderObject();
			return Out;
		}
		private static RenderObject DumpAlloc(ref uint Count)
		{
			if (0 == Count) return null;
			RenderObject Out = null, FirstOut = null;
			uint FromDump;
			lock (Dump.Lock)
				if (!(0 == (FromDump = Dump.Size)))
				{
					FirstOut = Dump.Top;
					if (Count < Dump.Size)
					{
						FromDump = Count;
						Dump.Size -= Count;
						do
						{
							Dump.Top.Next = Out;
							Out = Dump.Top;
							Dump.Top = Out.Prev;
						} while (0 != --Count);
					}
					else
					{
						Count -= Dump.Size;
						Dump.Top = null;
						Dump.Size = 0;
					}
				}

			if (FromDump != 0)
			{
				if (Count != 0)
				{
					Out = FirstOut;
					for (--FromDump; 0 != FromDump; --FromDump)
					{
						Out.Prev.Next = Out;
						Out = Out.Prev;
					}
				}
				// and make it a loop.
				FirstOut.Next = Out;
				Out.Prev = FirstOut;
			}
			return FirstOut;
		}
		private static RenderObject Alloc(uint Count)
		{
			if (0 == Count) return null;
			uint FromDump = Count;
			var Base = DumpAlloc(ref Count);
			FromDump -= Count;

			if (FromDump == 0)
			{
				Base = new RenderObject();
				Base.Next = Base;
				Base.Prev = Base;
				--Count;
			}
			while (Count != 0)
			{
				Base.Prev = new RenderObject()
				{
					Prev = Base.Prev,
					Next = Base,
				};
			}
			return Base;
		}
		internal static void Alloc(RenderList List)
		{
			var Node = Alloc();

			Node.ListReference = List.Weak;

			if (0 == List.Count++)
			{
				Node.Next = Node;
				Node.Prev = Node;

				List.First = Node;
				System.GC.ReRegisterForFinalize(List);
			}
			else
			{
				Node.Next = List.First;
				Node.Prev = List.First.Prev;
				Node.Next.Prev = Node;
				Node.Prev.Next = Node;
				//List.Start = Node;
			}
		}
		private static void Stitch(RenderList List, RenderObject NodeLoop, uint Count)
		{
			if (0 == List.Count)
			{
				List.First = NodeLoop;
				List.Count = Count;
				System.GC.ReRegisterForFinalize(List);
			}
			else
			{
				// stitch it in. there is probably a better way to do this rather than using two locals.
				var Start = NodeLoop;
				var End = NodeLoop.Prev;
				Start.Prev = List.First.Prev;
				End.Next = List.First;

				Start.Prev.Next = Start;
				End.Next.Prev = End;

				//List.Start = End;
				List.Count += Count;
			}
		}
		internal static RenderObject Alloc(RenderList List, RenderObject Node, uint Count)
		{
			if (0 != Count && null != List && null != (object)Node)
			{
				uint Adding = Count;
				var NewOnes = Alloc(Adding);

				do
				{
					NewOnes.Model = Node.Model;
					NewOnes.Object = Node.Object;
					NewOnes.Transform = Node.Transform;
					NewOnes.BoundsColor = Node.BoundsColor;
					NewOnes.ListReference = List.Weak;
					Node = Node.Next;
					NewOnes = NewOnes.Next;
				} while (0 != --Adding);
				Stitch(List, NewOnes, Count);
				return NewOnes;
			}
			return null;
		}
		internal static RenderObject Free(RenderList From, RenderList To)
		{
			RenderObject Iter;
			if ((object)From != null && From.Count != 0 && (object)From != (object)To)
			{
				if ((object)To == null)
				{
					Free(From);
					Iter = null;
				}
				else
				{
					var Num = From.Count;
					Iter = From.First;
					do
					{
						Iter.ListReference = To.Weak;
						Iter = Iter.Next;
					} while (0 != --From.Count);
					From.First = null;
					System.GC.SuppressFinalize(From);
					Stitch(To, Iter, Num);
				}
			}
			else Iter = null;
			return Iter;
		}
		internal static void Free(RenderList List)
		{
			var Last = List.First;
			var Count = List.Count;
			List.Count = 0;
			List.First = null;
			FreeLoop(Last, Count);
			if (Count != 0)
				System.GC.SuppressFinalize(List);
		}
		internal static void FreeLoop(RenderObject Last, uint Count)
		{
			if (0 != Count)
			{
				var First = Last.Next;
				{
					RenderObject Iter;
					uint IterPos;
					for (Iter = Last, IterPos = Count; 0 != IterPos; Iter = Iter.Prev, --IterPos)
					{
						Iter.Next = null;
						Iter.Object = null;
						Iter.Model = null;
						Iter.ListReference = null;
					}
				}
				lock (Dump.Lock)
				{
					if (0 == Dump.Size)
						First.Prev = null;
					else
						First.Prev = Dump.Top;
					Dump.Top = Last;
					Dump.Size += Count;
				}
			}
		}
		public bool Cancel()
		{
			bool Result = false;
			var LayerReference = this.ListReference;
			if (null != LayerReference &&
				LayerReference == System.Threading.Interlocked.CompareExchange(
					ref this.ListReference, null, LayerReference))
			{
				var List = (RenderList)LayerReference.Target;
				if (null != (object)List)
				{
					if (0 == --List.Count)
					{
						List.First = null;
						System.GC.SuppressFinalize(LayerReference);
					}
					else
					{
						if (List.First == this)
							List.First = Next;

						Prev.Next = Next;
						Next.Prev = Prev;
					}
					Result = true;
				}
				Prev = null;
				Next = null;
				Object = null;
				Model = null;
				lock (Dump.Lock)
				{
					if (0 != Dump.Size++)
						Prev = Dump.Top;
					Dump.Top = this;
				}
			}
			return Result;
		}
	}
	public sealed class RenderList : IDisposable
	{
		internal readonly WeakReference Weak;
		public RenderObject First;
		public readonly RenderLayer
			Unknown = new RenderLayer(0),
			Solid = new RenderLayer(1),
			Decal = new RenderLayer(2),
			SolidDecal = new RenderLayer(3),
			Transparent = new RenderLayer(4),
			SemiTransparent = new RenderLayer(5),
			Shadow = new RenderLayer(6),
			Unsupported = new RenderLayer(7);
		public RenderCamera Camera;
		public RenderObject Last => 0 == Count ? null : First.Next;
		public uint Count;

		public bool SetupCamera(
			ref OpenTK.Vector3 position, ref OpenTK.Quaternion rotation,
			ref OpenTK.Matrix4 view, ref OpenTK.Matrix4 proj)
		{
			bool Difference;
			int i;
			for (i = 15, Difference = false; !Difference && i >= 0; --i)
				Difference = Camera.View[i & 3, i >> 2] != view[i & 3, i >> 2] ||
					Camera.Proj[i & 3, i >> 2] != proj[i & 3, i >> 2];
			Camera.View = view;
			Camera.Proj = proj;
			if (Difference)
			{
				OpenTK.Matrix4.Mult(ref Camera.View, ref Camera.Proj, out Camera.ViewProj);

				Camera.X = (Camera.View * new OpenTK.Vector4(1, 0, 0, 1)).Xyz;
				Camera.Y = (Camera.View * new OpenTK.Vector4(0, 1, 0, 1)).Xyz;
				Camera.Z = (Camera.View * new OpenTK.Vector4(0, 0, 1, 1)).Xyz;
				Camera.X.Normalize();
				Camera.Y.Normalize();
				Camera.Z.Normalize();
			}
			Camera.Transform.translation = position;
			Camera.Transform.rotation = rotation;
			Camera.Transform.scale = 1.0f;
			return Difference;
		}
		public bool SetupCamera(
			ref OpenTK.Matrix4 view, ref OpenTK.Matrix4 proj,
			OpenTK.Vector3 position,
			float pitch, float yaw, float roll = 0f)
		{
			var rotation = OpenTK.Quaternion.FromEulerAngles(pitch, yaw, roll);
			return SetupCamera(ref position, ref rotation, ref view, ref proj);
		}
		public RenderLayer GetLayer(byte DrawLayer)
		{
			return (DrawLayer & 4) == 0 ? (DrawLayer & 2) == 0 ? (DrawLayer & 1) == 0 ? Unknown : Solid :
				(DrawLayer & 1) == 0 ? Decal : SolidDecal :
				(DrawLayer & 2) == 0 ? (DrawLayer & 1) == 0 ? Transparent : SemiTransparent :
				(DrawLayer & 1) == 0 ? Shadow : Unsupported;
		}
		public sealed override string ToString()
		{
			return string.Format("{{\"obj\":{0},\"layer\":[{1},{2},{3},{4},{5},{6},{7}]}}",
				Count == 0 ? "0" : Count.ToString(),
				Unknown, Solid, Decal, SolidDecal, Transparent, SemiTransparent, Shadow, Unsupported);
		}
		public RenderList()
		{
			System.GC.SuppressFinalize(this);
			this.Weak = new WeakReference(this);
			this.Unknown = new RenderLayer(0);
			this.Solid = new RenderLayer(1);
			this.Decal = new RenderLayer(2);
			this.SolidDecal = new RenderLayer(3);
			this.Transparent = new RenderLayer(4);
			this.SemiTransparent = new RenderLayer(5);
			this.Shadow = new RenderLayer(6);
			this.Unsupported = new RenderLayer(7);
		}

		public RenderList(RenderList CopyFrom) : this()
		{
			if (null != (object)CopyFrom && 0 != CopyFrom.Count)
			{
				RenderObject.Alloc(this, CopyFrom.First.Next, CopyFrom.Count);
			}
		}

		~RenderList()
		{
			RenderObject.FreeLoop(First, Count);
		}

		public RenderObject this[uint Offset]
		{
			get
			{
				RenderObject Out;
				uint Back;
				if (0 == Count)
					Out = null;
				else
				{
					if (Offset > Count)
						Offset %= Count;
					else if (Offset == Count)
						Offset = 0;
					Back = Count - (Offset + 1u);
					if (Back <= Offset)
					{
						Out = First.Prev;
						while (0 != Back--) Out = Out.Prev;
					}
					else
					{
						Out = First.Prev;
						do Out = Out.Next; while (0 != Offset--);
					}
				}
				return Out;
			}
		}
		public RenderObject this[int Offset]
		{
			get
			{
				return 0 == Count ? null : this[Offset < 0 ? ((uint)((int)Count + (Offset % (int)Count))) : (uint)Offset];
			}
		}
		/// <summary>
		/// adds all nodes from other list. if other list is this list, each item is duplicated once.
		/// returns null if list is null or has no nodes in it.
		/// otherwise it returns the FIRST added node in this list, where you can access the last added node in the .Last property after a non-null is returned.
		/// </summary>
		public RenderObject Render(RenderList List)
		{
			return (null == (object)List || 0 == List.Count) ?
				null : RenderObject.Alloc(this, List.First.Next, List.Count);
		}
		/// <summary>
		/// adds multiple nodes for rendering.
		/// returns null if Count is zero or the input node is dead or null.
		/// otherwise it returns the FIRST added node in this list, where you can access the last added node in the .Last property after a non-null is returned.
		/// </summary>
		public RenderObject Render(RenderObject Node, uint Count)
		{
			return (0 == Count || null == (object)Node || !Node.Alive) ?
				null : RenderObject.Alloc(this, Node, Count);
		}
		/// <summary>
		/// Adds an item for rendering. You can retrieve the node made for the item by requesting .Last after the call returns true.
		/// </summary>
		public bool Render(RenderObject Node)
		{
			if (null == (object)Node || !Node.Alive)
				return false;

			RenderObject.Alloc(this);
			First.Prev.Transform = Node.Transform;
			First.Prev.Model = Node.Model;
			First.Prev.Object = Node.Object;
			First.Prev.BoundsColor = Node.BoundsColor;
			return true;
		}
		/// <summary>
		/// Adds an item for rendering. You can retrieve the node made for the item by requesting .Last after the call.
		/// </summary>
		public void Render(
			ref Transform Transform,
			Object Object,
			Model3D Model,
			System.Drawing.Color Color)
		{
			RenderObject.Alloc(this);
			First.Prev.Transform = Transform;
			First.Prev.Object = Object;
			First.Prev.Model = Model ?? (Object as Model3D);
			First.Prev.BoundsColor = Color;
		}
		public void Render(Transform Transform, Object Object, Model3D Model, System.Drawing.Color Color)
		{
			Render(ref Transform, Object, Model, Color);
		}
		public void Render(ref Transform Transform, Object Object, Model3D Model)
		{
			Render(ref Transform, Object, Model, White);
		}
		public void Render(Transform Transform, Object Object, Model3D Model)
		{
			Render(ref Transform, Object, Model, White);
		}
		public void Render(ref Transform Transform, Object Object, System.Drawing.Color Color)
		{
			Render(ref Transform, Object, Object as Model3D, Color);
		}
		public void Render(Transform Transform, Object Object, System.Drawing.Color Color)
		{
			Render(ref Transform, Object, Object as Model3D, Color);
		}
		public void Render(ref Transform Transform, Object Object)
		{
			if (Object is System.Drawing.Color)
				Render(ref Transform, null, null, (System.Drawing.Color)Object);
			else
				Render(ref Transform, Object, Object as Model3D, White);
		}
		public void Render(Transform Transform, Object Object)
		{
			if (Object is System.Drawing.Color)
				Render(ref Transform, null, null, (System.Drawing.Color)Object);
			else
				Render(ref Transform, Object, Object as Model3D, White);
		}
		public void Render(ref Transform Transform, Model3D Model)
		{
			Render(ref Transform, Model, Model, White);
		}
		public void Render(Transform Transform, Model3D Model)
		{
			Render(ref Transform, Model, Model, White);
		}
		public void Render(ref Transform Transform, System.Drawing.Color Color)
		{
			Render(ref Transform, null, null, Color);
		}
		public void Render(Transform Transform, System.Drawing.Color Color)
		{
			Render(ref Transform, null, null, Color);
		}
		public void Render(ref Transform Transform, Model3D Model, System.Drawing.Color Color)
		{
			Render(ref Transform, Model, Model, Color);
		}
		public void Render(Transform Transform, Model3D Model, System.Drawing.Color Color)
		{
			Render(ref Transform, Model, Model, Color);
		}

		public static System.Drawing.Color White => System.Drawing.Color.White;

		public void Clear() { RenderObject.Free(this); }
		void IDisposable.Dispose() { RenderObject.Free(this); }
		/// <summary>
		/// Takes all the nodes in this list and transfers them to the target list, clearing this list.
		/// if Target is null, this list is cleared.
		/// </summary>
		/// <param name="Target">list that will take everything we have.</param>
		public void DumpTo(RenderList Target)
		{
			if (0 != Count)
				RenderObject.Free(this, Target);
		}
		/// <summary>
		/// Takes all the nodes in that list and transfers them to this list, clearing that list.
		/// null is returned if Source is null, empty or this, otherwise the first item taken from source is returned and .Last contains the last.
		/// </summary>
		/// <param name="Target">list that will take everything we have.</param>
		public RenderObject Dump(RenderList Source)
		{
			return RenderObject.Free(Source, this);
		}

		public void DrawBounds()
		{
			RenderObject Iter;
			uint IterPos;
			for (Iter = First, IterPos = Count; 0 != IterPos; Iter = Iter.Next, --IterPos)
			{
				if (Iter.BoundsColor.A != 0)
				{
					if (null == Iter.Model)
						Quad64.src.Viewer.BoundingBox.draw(Iter.Transform, Iter.BoundsColor);
					else
						Quad64.src.Viewer.BoundingBox.draw(Iter.Transform, Iter.BoundsColor, Iter.Model);
				}
			}
		}
		public void UpdateLayers(byte Mask = 255)
		{
			if (Mask == 0) return;

			RenderObject Iter;
			uint IterPos;
			if (0 != (Mask & (1 << 0))) Unknown.Clear();
			if (0 != (Mask & (1 << 1))) Solid.Clear();
			if (0 != (Mask & (1 << 2))) Decal.Clear();
			if (0 != (Mask & (1 << 3))) SolidDecal.Clear();
			if (0 != (Mask & (1 << 4))) Transparent.Clear();
			if (0 != (Mask & (1 << 5))) SemiTransparent.Clear();
			if (0 != (Mask & (1 << 6))) Shadow.Clear();
			if (0 != (Mask & (1 << 7))) Unsupported.Clear();

			for (Iter = First, IterPos = Count; 0 != IterPos; Iter = Iter.Next, --IterPos)
				if (null != Iter.Model && null != Iter.Model.root && 0 != (Iter.Model.root.DrawLayerMask & Mask))
					Iter.Model.root.render(this, Iter, Mask);

			if (0 != (Mask & (1 << 0))) Unknown.Sort();
			if (0 != (Mask & (1 << 1))) Solid.Sort();
			if (0 != (Mask & (1 << 2))) Decal.Sort();
			if (0 != (Mask & (1 << 3))) SolidDecal.Sort();
			if (0 != (Mask & (1 << 4))) Transparent.Sort(RenderLayer.MostDistanceComparer.Default);
			if (0 != (Mask & (1 << 5))) SemiTransparent.Sort(RenderLayer.MostDistanceComparer.Default);
			if (0 != (Mask & (1 << 6))) Shadow.Sort(RenderLayer.MostDistanceComparer.Default);
			if (0 != (Mask & (1 << 7))) Unsupported.Sort(RenderLayer.MostDistanceComparer.Default);
		}
		public void DrawModels()
		{
			RenderObject Iter;
			uint IterPos;
			for (Iter = First, IterPos = Count; 0 != IterPos; Iter = Iter.Next, --IterPos)
				if (null != Iter.Model)
					Iter.Model.drawModel(Iter.Transform, ref Camera);
		}
		public void DrawModels(byte drawLayers)
		{
			RenderObject Iter;
			uint IterPos;
			for (Iter = First, IterPos = Count; 0 != IterPos; Iter = Iter.Next, --IterPos)
				if (null != Iter.Model)
					Iter.Model.drawModel(Iter.Transform, drawLayers, ref Camera);
		}
	}
	public sealed class RenderModel
	{
		private System.WeakReference LayerReference;
		public RenderModel Next, Prev;
		public GeoModel Model;
		public RenderObject Object;
		public Transform Transform;
		public OpenTK.Vector4 ViewProjected;
		public RenderLayer Layer => null == (object)LayerReference ? null : (RenderLayer)LayerReference.Target;
		public bool Alive => null != LayerReference;
		public sealed override string ToString()
		{
			return null == Object ? string.Empty : Object.ToString();
		}
		private void GetNodeOffset(uint Count, uint Offset, out RenderModel Out)
		{
			uint Back;

			if (Offset > Count)
				Offset %= Count;
			else if (Offset == Count)
				Offset = 0;

			Back = Count - (Offset + 1u);

			if (Back <= Offset)
			{
				Out = this;
				while (0 != Back--) Out = Out.Prev;
			}
			else
			{
				Out = this;
				while (0 != Offset--) Out = Out.Next;
			}
		}
		public RenderModel this[uint Offset]
		{
			get
			{
				var Layer = this.Layer;
				RenderModel Out;
				if (null == Layer || 0 == Layer.Count)
					Out = null;
				else
					GetNodeOffset(Layer.Count, Offset, out Out);
				return Out;
			}
		}
		public RenderModel this[int Offset]
		{
			get
			{
				var Layer = this.Layer;
				RenderModel Out;
				if (null == Layer || 0 == Layer.Count)
					Out = null;
				else
					GetNodeOffset(Layer.Count, Offset < 0 ? ((uint)((int)Layer.Count + (Offset % (int)Layer.Count))) : (uint)Offset, out Out);
				return Out;
			}
		}
		public bool IsOwnedBy(RenderLayer Layer)
		{
			return null == (object)Layer ?
				null == (object)LayerReference :
				(object)LayerReference == Layer.Weak;
		}


		private static class Dump
		{
			public static RenderModel Top;
			public static uint Size;
			public static object Lock = new object();
		}
		private RenderModel() { }
		private static RenderModel Alloc()
		{
			RenderModel Out = null;
			bool Got;
			lock (Dump.Lock)
				if ((Got = !(0 == Dump.Size)))
				{
					Out = Dump.Top;
					if (0 == --Dump.Size)
						Dump.Top = null;
					else
						Dump.Top = Out.Prev;
				}
			if (Got)
				Out.Prev = null;
			else
				Out = new RenderModel();
			return Out;
		}
		private static RenderModel DumpAlloc(ref uint Count)
		{
			if (0 == Count) return null;
			RenderModel Out = null, FirstOut = null;
			uint FromDump;
			lock (Dump.Lock)
				if (!(0 == (FromDump = Dump.Size)))
				{
					FirstOut = Dump.Top;
					if (Count < Dump.Size)
					{
						FromDump = Count;
						Dump.Size -= Count;
						do
						{
							Dump.Top.Next = Out;
							Out = Dump.Top;
							Dump.Top = Out.Prev;
						} while (0 != --Count);
					}
					else
					{
						Count -= Dump.Size;
						Dump.Top = null;
						Dump.Size = 0;
					}
				}

			if (FromDump != 0)
			{
				if (Count != 0)
				{
					Out = FirstOut;
					for (--FromDump; 0 != FromDump; --FromDump)
					{
						Out.Prev.Next = Out;
						Out = Out.Prev;
					}
				}
				// and make it a loop.
				FirstOut.Next = Out;
				Out.Prev = FirstOut;
			}
			return FirstOut;
		}
		private static RenderModel Alloc(uint Count)
		{
			if (0 == Count) return null;
			uint FromDump = Count;
			var Base = DumpAlloc(ref Count);
			FromDump -= Count;

			if (FromDump == 0)
			{
				Base = new RenderModel();
				Base.Next = Base;
				Base.Prev = Base;
				--Count;
			}
			while (Count != 0)
			{
				Base.Prev = new RenderModel()
				{
					Prev = Base.Prev,
					Next = Base,
				};
			}
			return Base;
		}
		internal static void Alloc(RenderLayer Layer)
		{
			var Node = Alloc();

			Node.LayerReference = Layer.Weak;

			if (0 == Layer.Count++)
			{
				Node.Next = Node;
				Node.Prev = Node;

				Layer.First = Node;
				System.GC.ReRegisterForFinalize(Layer);
			}
			else
			{
				Node.Next = Layer.First;
				Node.Prev = Layer.First.Prev;
				Node.Next.Prev = Node;
				Node.Prev.Next = Node;
				//Layer.Start = Node;
			}
		}
		private static void Stitch(RenderLayer Layer, RenderModel NodeLoop, uint Count)
		{
			if (0 == Layer.Count)
			{
				Layer.First = NodeLoop;
				Layer.Count = Count;
				System.GC.ReRegisterForFinalize(Layer);
			}
			else
			{
				// stitch it in. there is probably a better way to do this rather than using two locals.
				var Start = NodeLoop;
				var End = NodeLoop.Prev;
				Start.Prev = Layer.First.Prev;
				End.Next = Layer.First;

				Start.Prev.Next = Start;
				End.Next.Prev = End;

				//Layer.Start = End;
				Layer.Count += Count;
			}
		}
		internal static RenderModel Alloc(RenderLayer Layer, RenderModel Node, uint Count)
		{
			if (0 != Count && null != Layer && null != (object)Node)
			{
				uint Adding = Count;
				var NewOnes = Alloc(Adding);

				do
				{
					NewOnes.Model = Node.Model;
					NewOnes.Object = Node.Object;
					NewOnes.Transform = Node.Transform;
					NewOnes.LayerReference = Layer.Weak;
					Node = Node.Next;
					NewOnes = NewOnes.Next;
				} while (0 != --Adding);
				Stitch(Layer, NewOnes, Count);
				return NewOnes;
			}
			return null;
		}
		internal static RenderModel Free(RenderLayer From, RenderLayer To)
		{
			RenderModel Iter;
			if ((object)From != null && From.Count != 0 && (object)From != (object)To)
			{
				if ((object)To == null)
				{
					Free(From);
					Iter = null;
				}
				else
				{
					var Num = From.Count;
					Iter = From.First;
					do
					{
						Iter.LayerReference = To.Weak;
						Iter = Iter.Next;
					} while (0 != --From.Count);
					From.First = null;
					System.GC.SuppressFinalize(From);
					Stitch(To, Iter, Num);
				}
			}
			else Iter = null;
			return Iter;
		}
		internal static void Free(RenderLayer Layer)
		{
			var Last = Layer.First;
			var Count = Layer.Count;
			Layer.Count = 0;
			Layer.First = null;
			FreeLoop(Last, Count);
			if (Count != 0)
				System.GC.SuppressFinalize(Layer);
		}
		internal static void FreeLoop(RenderModel Last, uint Count)
		{
			if (0 != Count)
			{
				var First = Last.Next;
				{
					RenderModel Iter;
					uint IterPos;
					for (Iter = Last, IterPos = Count; 0 != IterPos; Iter = Iter.Prev, --IterPos)
					{
						Iter.Next = null;
						Iter.Object = null;
						Iter.Model = null;
						Iter.LayerReference = null;
					}
				}
				lock (Dump.Lock)
				{
					if (0 == Dump.Size)
						First.Prev = null;
					else
						First.Prev = Dump.Top;
					Dump.Top = Last;
					Dump.Size += Count;
				}
			}
		}
		public bool Cancel()
		{
			bool Result = false;
			var LayerReference = this.LayerReference;
			if (null != LayerReference &&
				LayerReference == System.Threading.Interlocked.CompareExchange(
					ref this.LayerReference, null, LayerReference))
			{
				var Layer = (RenderLayer)LayerReference.Target;
				if (null != (object)Layer)
				{
					if (0 == --Layer.Count)
					{
						Layer.First = null;
						System.GC.SuppressFinalize(LayerReference);
					}
					else
					{
						if (Layer.First == this)
							Layer.First = Next;

						Prev.Next = Next;
						Next.Prev = Prev;
					}
					Result = true;
				}
				Prev = null;
				Next = null;
				Object = null;
				Model = null;
				lock (Dump.Lock)
				{
					if (0 != Dump.Size++)
						Prev = Dump.Top;
					Dump.Top = this;
				}
			}
			return Result;
		}
	}
	public sealed class RenderLayer : IDisposable
	{
		internal readonly WeakReference Weak;
		public RenderModel First;
		public RenderModel Last => 0 == Count ? null : First.Prev;
		public uint Count;
		public readonly byte DrawLayer, DrawLayerMask;

		public sealed override string ToString()
		{
			return string.Format(
				0 == (4 & DrawLayer) ?
				0 == (2 & DrawLayer) ?
				0 == (1 & DrawLayer) ? "{{\"?\":{0}}}" : "{{\"solid\":{0}}}" :
				0 == (1 & DrawLayer) ? "{{\"decal\":{0}}}" : "{{\"3\":{0}}}" :
				0 == (2 & DrawLayer) ?
				0 == (1 & DrawLayer) ? "{{\"transparent\":{0}}}" : "{{\"semitransparent\":{0}}}" :
				0 == (1 & DrawLayer) ? "{{\"shadow\":{0}}}" : "{{\"7\":{0}}}",
				Count == 0 ? "0" : Count.ToString());
		}
		public RenderLayer(byte DrawLayer)
		{
			System.GC.SuppressFinalize(this);
			if (DrawLayer != (DrawLayer & 7)) throw new System.InvalidOperationException("Cannot be more than 8 draw layers");

			this.Weak = new WeakReference(this);
			this.DrawLayer = DrawLayer;
			this.DrawLayerMask = (byte)(1 << DrawLayer);
		}

		public RenderLayer(RenderLayer CopyFrom) : this(CopyFrom.DrawLayer)
		{
			if (null != (object)CopyFrom && 0 != CopyFrom.Count)
			{
				RenderModel.Alloc(this, CopyFrom.First.Next, CopyFrom.Count);
			}
		}

		~RenderLayer()
		{
			RenderModel.FreeLoop(First, Count);
		}
		internal static void Insert(RenderList list, RenderObject obj, GeoModel model,
			ref Transform transform, ref OpenTK.Vector4 multiplied, byte layerMask) {
			RenderLayer layer;
			if (0 != (layerMask & (1 << 0))) { layer = list.Unknown; RenderModel.Alloc(layer); layer.First.Prev.Transform = transform; layer.First.Prev.Object = obj; layer.First.Prev.Model = model; layer.First.Prev.ViewProjected = multiplied; }
			if (0 != (layerMask & (1 << 1))) { layer = list.Solid; RenderModel.Alloc(layer); layer.First.Prev.Transform = transform; layer.First.Prev.Object = obj; layer.First.Prev.Model = model; layer.First.Prev.ViewProjected = multiplied; }
			if (0 != (layerMask & (1 << 2))) { layer = list.Decal; RenderModel.Alloc(layer); layer.First.Prev.Transform = transform; layer.First.Prev.Object = obj; layer.First.Prev.Model = model; layer.First.Prev.ViewProjected = multiplied; }
			if (0 != (layerMask & (1 << 3))) { layer = list.SolidDecal; RenderModel.Alloc(layer); layer.First.Prev.Transform = transform; layer.First.Prev.Object = obj; layer.First.Prev.Model = model; layer.First.Prev.ViewProjected = multiplied; }
			if (0 != (layerMask & (1 << 4))) { layer = list.Transparent; RenderModel.Alloc(layer); layer.First.Prev.Transform = transform; layer.First.Prev.Object = obj; layer.First.Prev.Model = model; layer.First.Prev.ViewProjected = multiplied; }
			if (0 != (layerMask & (1 << 5))) { layer = list.SemiTransparent; RenderModel.Alloc(layer); layer.First.Prev.Transform = transform; layer.First.Prev.Object = obj; layer.First.Prev.Model = model; layer.First.Prev.ViewProjected = multiplied; }
			if (0 != (layerMask & (1 << 6))) { layer = list.Shadow; RenderModel.Alloc(layer); layer.First.Prev.Transform = transform; layer.First.Prev.Object = obj; layer.First.Prev.Model = model; layer.First.Prev.ViewProjected = multiplied; }
			if (0 != (layerMask & (1 << 7))) { layer = list.Unsupported; RenderModel.Alloc(layer); layer.First.Prev.Transform = transform; layer.First.Prev.Object = obj; layer.First.Prev.Model = model; layer.First.Prev.ViewProjected = multiplied; }
		}
		public RenderModel this[uint Offset]
		{
			get
			{
				RenderModel Out;
				uint Back;
				if (0 == Count)
					Out = null;
				else
				{
					if (Offset > Count)
						Offset %= Count;
					else if (Offset == Count)
						Offset = 0;
					Back = Count - (Offset + 1u);
					if (Back <= Offset)
					{
						Out = First.Prev;
						while (0 != Back--) Out = Out.Prev;
					}
					else
					{
						Out = First.Prev;
						do Out = Out.Next; while (0 != Offset--);
					}
				}
				return Out;
			}
		}
		public RenderModel this[int Offset]
		{
			get
			{
				return 0 == Count ? null : this[Offset < 0 ? ((uint)((int)Count + (Offset % (int)Count))) : (uint)Offset];
			}
		}
		/// <summary>
		/// adds all nodes from other layer. if other layer is this layer, each item is duplicated once.
		/// returns null if layer is null or has no nodes in it.
		/// otherwise it returns the FIRST added node in this layer, where you can access the last added node in the .Last property after a non-null is returned.
		/// </summary>
		public RenderModel Render(RenderLayer Layer)
		{
			return (null == (object)Layer || 0 == Layer.Count) ?
				null : RenderModel.Alloc(this, Layer.First.Next, Layer.Count);
		}
		/// <summary>
		/// adds multiple nodes for rendering.
		/// returns null if Count is zero or the input node is dead or null.
		/// otherwise it returns the FIRST added node in this layer, where you can access the last added node in the .Last property after a non-null is returned.
		/// </summary>
		public RenderModel Render(RenderModel Node, uint Count)
		{
			return (0 == Count || null == (object)Node || !Node.Alive) ?
				null : RenderModel.Alloc(this, Node, Count);
		}
		/// <summary>
		/// Adds an item for rendering. You can retrieve the node made for the item by requesting .Last after the call returns true.
		/// </summary>
		public bool Render(RenderModel Node)
		{
			if (null == (object)Node || !Node.Alive)
				return false;

			RenderModel.Alloc(this);
			First.Transform = Node.Transform;
			First.Model = Node.Model;
			First.Object = Node.Object;
			return true;
		}
		/// <summary>
		/// Adds an item for rendering. You can retrieve the node made for the item by requesting .Last after the call.
		/// </summary>
		public void Render(
			ref Transform Transform,
			RenderObject Object,
			GeoModel Model)
		{
		}
		/// <summary>
		/// Adds an item for rendering. You can retrieve the node made for the item by requesting .Last after the call.
		/// </summary>
		public void Render(
			Transform Transform,
			RenderObject Object,
			GeoModel Model)
		{
			Render(ref Transform, Object, Model);
		}
		public void Clear() { RenderModel.Free(this); }
		void IDisposable.Dispose() { RenderModel.Free(this); }
		/// <summary>
		/// Takes all the nodes in this layer and transfers them to the target layer, clearing this layer.
		/// if Target is null, this layer is cleared.
		/// </summary>
		/// <param name="Target">layer that will take everything we have.</param>
		public void DumpTo(RenderLayer Target)
		{
			if (0 != Count)
				RenderModel.Free(this, Target);
		}
		/// <summary>
		/// Takes all the nodes in that layer and transfers them to this layer, clearing that layer.
		/// null is returned if Source is null, empty or this, otherwise the first item taken from source is returned and .Last contains the last.
		/// </summary>
		/// <param name="Target">layer that will take everything we have.</param>
		public RenderModel Dump(RenderLayer Source)
		{
			return RenderModel.Free(Source, this);
		}
		public sealed class LeastDistanceComparer : Comparer<RenderModel>
		{
			new public static readonly IComparer<RenderModel> Default = new LeastDistanceComparer();
			private LeastDistanceComparer() { }

			public override int Compare(RenderModel x, RenderModel y)
			{
				return x.ViewProjected.Z.CompareTo(y.ViewProjected.Z);
			}
		}
		public sealed class MostDistanceComparer : Comparer<RenderModel>
		{
			new public static readonly IComparer<RenderModel> Default = new MostDistanceComparer();
			private MostDistanceComparer() { }
			public override int Compare(RenderModel x, RenderModel y)
			{
				return y.ViewProjected.Z.CompareTo(x.ViewProjected.Z);
			}
		}
		public IComparer<RenderModel> DefaultComparer = LeastDistanceComparer.Default;
		public void Sort(IComparer<RenderModel> Comparer=null)
		{
			if (Count <= 1) return;
			else if (Count == 2)
			{
				if ((Comparer ?? DefaultComparer).Compare(First, First.Prev) > 0) First = First.Prev;
			}
			else
			{
				// TODO, not use the below!
				RenderModel[] arr = new RenderModel[(int)Count];
				uint N; int O, P;
				for (N = Count; N != 0; First = First.Prev,--N)
					arr[(int)(N - 1u)] = First;
				System.Array.Sort(arr, (Comparer ?? DefaultComparer));
				for (N = Count, O = 0, P = (int)(N - 1u); N != 0; P = O++, --N)
				{
					arr[P].Next = arr[O];
					arr[O].Prev = arr[P];
				}
				First = arr[(int)(Count - 1u)];
			}
		}
		public void Draw(GeoDrawOptions options=0)
		{
			RenderModel model;
			GeoMesh mesh;
			uint model_pos;
			uint mesh_pos;
			for (model_pos = Count, model = First; 0 != model_pos; model = model.Next, --model_pos)
			{
				OpenTK.Graphics.OpenGL.GL.PushMatrix();
				model.Transform.GL_Load();
				for (mesh_pos = model.Model.Count, mesh = model.Model.First; 0 != mesh_pos; mesh=mesh.Next, --mesh_pos)
					if (mesh.DrawLayerMask == DrawLayerMask)
						mesh.draw(options);
				OpenTK.Graphics.OpenGL.GL.PopMatrix();
			}
		}
	}
}
