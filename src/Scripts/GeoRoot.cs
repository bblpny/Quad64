using System;
using System.Collections;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Quad64
{
	public sealed class GeoRoot : GeoParent
	{
		internal uint IDCounter;
		public uint Num;
		public readonly Model3D mdl;
		public GeoNode Last;
		public GeoNode Shadow;
		public GeoParent Background;
		public uint BackgroundRam;
		public uint FrustumAsm;
		public Color4b BackgroundColor;
		public Vector3s Pivot;
		public ushort BackgroundImageIndex;
		public ushort Near, Far;
		public ushort FOVInt;



		public ushort ShadowSize, DrawDistance;
		public byte ShadowShape, ShadowTransparency;
		public byte Code;
		public byte DrawLayerMask;

		public GeoNode First => 0 == Num ? null : Last.Next;
		public GlobalCollection Nodes => this;

		public bool draw(
			GraphicsInterface gi,
			Transform transform, 
			byte drawLayers,
			ref RenderCamera camTrs)
		{
			GeoNode NodeIter;
			uint NodeIterPos;
			for (NodeIterPos = NumImmediate,NodeIter=FirstChild;0!=NodeIterPos; NodeIter=NodeIter.Sibling,--NodeIterPos)
			{
				if (0 == (NodeIter.DrawLayerMask&drawLayers))
					continue;

				GL.PushMatrix();
				transform.GL_Load();
				do
				{
					NodeIter.draw(gi, ref transform, drawLayers, ref camTrs);
					while (0 != --NodeIterPos &&
						0 == ((NodeIter = NodeIter.Sibling).DrawLayerMask & drawLayers))
						continue;
				} while (0 != NodeIterPos);
				GL.DisableClientState(ArrayCap.ColorArray);
				GL.DisableClientState(ArrayCap.NormalArray);
				GL.DisableClientState(ArrayCap.TextureCoordArray);
				GL.DisableClientState(ArrayCap.VertexArray);
				GL.PopMatrix();
				return true;
			}
			return false;
		}
		internal void render(RenderList list, RenderObject obj, byte drawLayers=255)
		{
			GeoNode node;
			var trs = obj.Transform;
			uint node_pos;
			for (node_pos = NumImmediate, node = FirstChild; 0 != node_pos; node = node.Sibling, --node_pos)
				if (0 == (node.DrawLayerMask & drawLayers))
					continue;
				else
					node.render(list, obj, ref trs, drawLayers);
		}
		public struct GlobalCollection : ICollection<GeoNode>, IEquatable<GeoParent>
		{
			public readonly GeoRoot This;

			public bool Equals(GeoParent other) { return This == other; }

			private GlobalCollection(GeoRoot This) { this.This = This; }

			public static implicit operator GlobalCollection(GeoRoot parent) { return new GlobalCollection(parent); }
			public static implicit operator GlobalCollection(GeoParent node) { return new GlobalCollection(null == node ? (GeoRoot)null : node.Root); }

			public static bool operator ==(GlobalCollection L, GlobalCollection R) { return L.This == R.This; }
			public static bool operator !=(GlobalCollection L, GlobalCollection R) { return L.This != R.This; }

			public override int GetHashCode()
			{
				return null == (object)This ? 0 : This.GetHashCode();
			}
			public override bool Equals(object obj)
			{
				return null == obj ?
					This == null :
					obj is GeoParent ?
					This == obj :
					(obj is IEquatable<GeoParent>) && ((IEquatable<GeoParent>)obj).Equals(This);
			}
			int ICollection<GeoNode>.Count => null == This ? 0 : unchecked((int)This.NumImmediate);

			bool ICollection<GeoNode>.IsReadOnly => true;

			public struct Enumerator : IEnumerator<GeoNode>
			{
				public GeoNode Active;
				public GeoNode Current => Active;
				public uint Remaining;
				public Enumerator(GeoRoot Parent)
				{
					Remaining = null == Parent ? 0u : Parent.Num;
					Active = (Remaining == 0) ? null : Parent.Last;
				}
				object IEnumerator.Current => Active;
				public void Dispose()
				{
					Remaining = 0;
					Active = null;
				}
				public void Reset()
				{
					if (null == (object)Active)
						this = default(Enumerator);
					else
						this = new Enumerator(Active.Root);
				}
				public bool MoveNext()
				{
					bool Result = 0 != Remaining;
					if (!Result)
						Active = null;
					else
					{
						Active = Active.Next;
						--Remaining;
					}
					return Result;
				}
			}

			public Enumerator GetEnumerator() { return new Enumerator(This); }

			void ICollection<GeoNode>.Add(GeoNode item)
			{
				throw new NotSupportedException();
			}

			void ICollection<GeoNode>.Clear()
			{
				throw new NotSupportedException();
			}

			bool ICollection<GeoNode>.Contains(GeoNode item)
			{
				return null != item && item.Root == This;
			}

			void ICollection<GeoNode>.CopyTo(GeoNode[] array, int arrayIndex)
			{
				if (null == This)
					return;
				var iter = This.Last;
				for (uint i = This.Num; i != 0; --i)
					array[arrayIndex++] = (iter = iter.Next);
			}

			bool ICollection<GeoNode>.Remove(GeoNode item)
			{
				throw new NotSupportedException();
			}

			IEnumerator<GeoNode> IEnumerable<GeoNode>.GetEnumerator()
			{
				return GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		public GeoRoot(Model3D mdl) : base(null)
		{
			this.mdl = mdl;
		}
		internal void Bind()
		{
			GeoNode Iter, ParentIter;
			GeoModel ModelIter,ModelPrevIter;

			uint IterPos, ParentIterPos, ModelIterPos;
			byte Accum;

			for (Iter = First, IterPos = Num; 0 != IterPos; Iter = Iter.Next, --IterPos)
			{
				Iter.DrawLayerMask =
				Iter.SelfDrawLayerMask = 0;
				for (ModelIterPos = Iter.ModelCount, ModelPrevIter = Iter.LastModel, ModelIter = Iter.FirstModel; ModelIterPos != 0; ModelPrevIter= ModelIter, ModelIter = ModelIter.Next, --ModelIterPos)
					Iter.SelfDrawLayerMask |= ModelIter.BindGL(ModelPrevIter, ref ModelIter);
			}

			for (Iter = First, IterPos = Num; 0 != IterPos; Iter = Iter.Next, --IterPos)
				for (
					Accum = (byte)(Iter.SelfDrawLayerMask | Iter.ChildrenDrawLayerMask),
					Iter.DrawLayerMask |= Accum,
					ParentIter = Iter.Outer,
					ParentIterPos = Iter.Depth;
					0 != ParentIterPos;
					ParentIter = ParentIter.Outer, --ParentIterPos)
					ParentIter.DrawLayerMask |= (Accum = (ParentIter.ChildrenDrawLayerMask |= Accum));

			for (Iter = FirstChild, IterPos = NumImmediate; 0 != IterPos; Iter = Iter.Sibling, --IterPos)
				DrawLayerMask |= Iter.DrawLayerMask;
		}
	}
}
