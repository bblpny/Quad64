using System;
using System.Collections;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using BubblePony.Integers;
using BubblePony.GLHandle;

namespace Quad64
{
	public abstract class GeoParent 
	{
		/// <summary>
		/// the root. never null.
		/// </summary>
		public readonly GeoRoot Root;
		public GeoNode LastChild;
		public GeoNode FirstChild => 0 == NumImmediate ? null : LastChild.Sibling;


		public TransformI Cursor = new TransformI { scale = { Whole = 1, }, };
		public uint NumImmediate;
		public uint switchFunc, switchCount, switchPos;
		public bool callSwitch, isSwitch;
		public uint NumDescendants
		{
			get
			{
				uint Sum = NumImmediate;
				var Iter = LastChild;
				for (uint Pos = NumImmediate; 0 != Pos; --Pos, Iter = Iter.Sibling)
					Sum += Iter.NumDescendants;
				return Sum;
			}
		}
		protected GeoParent(GeoRoot Root)
		{
			this.Root = Root ?? (GeoRoot)this;
		}

		public ImmediateCollection Immediates => this;

		public struct ImmediateCollection : ICollection<GeoNode>, IEquatable<GeoParent>
		{
			public readonly GeoParent This;

			public bool Equals(GeoParent other) { return This == other; }

			private ImmediateCollection(GeoParent This) { this.This = This; }

			public static bool operator ==(ImmediateCollection L, ImmediateCollection R) { return L.This == R.This; }
			public static bool operator !=(ImmediateCollection L, ImmediateCollection R) { return L.This != R.This; }

			public static implicit operator ImmediateCollection(GeoParent parent) { return new ImmediateCollection(parent); }

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
				public Enumerator(GeoParent Parent)
				{
					Remaining = null == Parent ? 0u : Parent.NumImmediate;
					Active = (Remaining == 0) ? null : Parent.LastChild;
				}
				object System.Collections.IEnumerator.Current => Active;
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
						this = new Enumerator(Active.Parent);
				}
				public bool MoveNext()
				{
					bool Result = 0 != Remaining;
					if (!Result)
						Active = null;
					else
					{
						Active = Active.Sibling;
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
				return null != item && item.Parent == This;
			}

			void ICollection<GeoNode>.CopyTo(GeoNode[] array, int arrayIndex)
			{
				if (null == This)
					return;
				var iter = This.LastChild;
				for (uint i = This.NumImmediate; i != 0; --i)
					array[arrayIndex++] = (iter = iter.Sibling);
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

		public struct DeepCollection : ICollection<GeoNode>, IEquatable<GeoParent>
		{
			public readonly GeoParent This;

			public bool Equals(GeoParent other) { return This == other; }

			private DeepCollection(GeoParent This) { this.This = This; }

			public static bool operator ==(DeepCollection L, DeepCollection R) { return L.This == R.This; }
			public static bool operator !=(DeepCollection L, DeepCollection R) { return L.This != R.This; }

			public static implicit operator DeepCollection(GeoParent parent) { return new DeepCollection(parent); }

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

			public IEnumerator<GeoNode> GetEnumerator()
			{
				foreach (GeoNode Item in (ImmediateCollection)This)
				{
					yield return Item;
					foreach (GeoNode Child in (DeepCollection)Item)
						yield return Child;
				}
			}

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
				bool Possible = null != item && null != This;
				if (This == This.Root)
				{
					return item.Root == This;
				}
				else if (This is GeoNode)
				{
					var Depth = ((GeoNode)This).Depth;

					if (item.Depth <= Depth)
						return false;

					var Iter = item.Outer;
					while (Iter.Depth != Depth) Iter = Iter.Outer;
					return This == Iter;
				}
				else if (This != item)
				{
					for (GeoNode Iter = item;
						Iter.Outer != Iter;
						Iter = Iter.Outer)
						if (Iter.Outer == This)
							return true;
				}
				return false;
			}

			void ICollection<GeoNode>.CopyTo(GeoNode[] array, int arrayIndex)
			{
				using (var Enumerator = GetEnumerator())
					while (Enumerator.MoveNext())
						array[arrayIndex++] = Enumerator.Current;
			}

			bool ICollection<GeoNode>.Remove(GeoNode item)
			{
				throw new NotSupportedException();
			}
			

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}
	}
	public sealed class GeoMesh
	{
		public readonly GeoModel Parent;
		public readonly byte[] VertexBuffer;
		public readonly Array IndexBuffer;
		public readonly Scripts.TempMeshData Options;
		public readonly TextureFormats.Raw texture;
		public readonly int IndexCount, IndicesByteSize;
		public readonly byte IndexElementSize, DrawLayerMask;
		public readonly Scripts.Material Material;
		public GraphicsState State;
		public GeoMesh Next;
		public int VertexCount => VertexBuffer.Length >> 4;
		private static void StoreVertex(byte[] Buffer, ref Vertex128 value, int Offset)
		{
			Block128 b = (Block128)value;
			Buffer[Offset + 0] = b.A;
			Buffer[Offset + 1] = b.B;
			Buffer[Offset + 2] = b.C;
			Buffer[Offset + 3] = b.D;
			Buffer[Offset + 4] = b.E;
			Buffer[Offset + 5] = b.F;
			Buffer[Offset + 6] = b.G;
			Buffer[Offset + 7] = b.H;
			Buffer[Offset + 8] = b.I;
			Buffer[Offset + 9] = b.J;
			Buffer[Offset + 10] = b.K;
			Buffer[Offset + 11] = b.L;
			Buffer[Offset + 12] = b.M;
			Buffer[Offset + 13] = b.N;
			Buffer[Offset + 14] = b.O;
			Buffer[Offset + 15] = b.P;
		}
		public GeoMesh(GeoModel Parent, Scripts.TempMesh128 Mesh, ref Dictionary<Scripts.TempVertex128, Scripts.TempVertex128> utilize)
		{
			Scripts.TempVertex128 viter;
			uint viterpos;
			this.Parent = Parent;
			this.Options = Mesh.value;
			this.Material = this.Options.getMaterial();
			this.texture = Mesh.references.bmp;
			utilize = Scripts.TempVertex128.Process(ref Mesh.references.list, utilize);
			var VertexCount = utilize.Count;
			this.VertexBuffer = new byte[VertexCount << 4];
			Array IndexBuffer;
			var IndexSize = VertexCount > 256 ? VertexCount > (ushort.MaxValue + 1u) ? 4 : 2 : 1;
			this.IndexCount = (int)Mesh.references.list.count;
			byte IndexElementSize;
			uint NextIndex = 0;
			if (IndexSize > 1)
			{
				if (IndexSize > 2)
				{
					var UIndexBuffer = new uint[IndexCount];
					for (viterpos = Mesh.references.list.count, viter = Mesh.references.list.first; viterpos != 0; viter = viter.next, --viterpos) unchecked
						{
							UIndexBuffer[(int)(Mesh.references.list.count - viterpos)] = viter.index;
							if (NextIndex == viter.index)
								StoreVertex(VertexBuffer, ref viter.value, (int)(NextIndex++) << 4);
						}
					IndexBuffer = UIndexBuffer;
					IndexElementSize = 4;
				}
				else
				{
					var UIndexBuffer = new ushort[IndexCount];
					for (viterpos = Mesh.references.list.count, viter = Mesh.references.list.first; viterpos != 0; viter = viter.next, --viterpos) unchecked
						{
							UIndexBuffer[(int)(Mesh.references.list.count - viterpos)] = (ushort)viter.index;
							if (NextIndex == viter.index)
								StoreVertex(VertexBuffer, ref viter.value, (int)(NextIndex++) << 4);
						}
					IndexBuffer = UIndexBuffer;
					IndexElementSize = 2;
				}
			}
			else
			{
				var UIndexBuffer = new byte[IndexCount];
				for (viterpos = Mesh.references.list.count, viter = Mesh.references.list.first; viterpos != 0; viter = viter.next, --viterpos) unchecked
					{
						UIndexBuffer[(int)(Mesh.references.list.count - viterpos)] = (byte)viter.index;
						if (NextIndex == viter.index)
							StoreVertex(VertexBuffer, ref viter.value, (int)(NextIndex++) << 4);
					}
				IndexBuffer = UIndexBuffer;
				IndexElementSize = 1;
			}
			this.IndexBuffer = IndexBuffer;
			this.IndexElementSize = IndexElementSize;
			this.IndicesByteSize = 2 == IndexElementSize ? IndexCount << 1 : 4 == IndexElementSize ? IndexCount << 2 : IndexCount;
			DrawLayerMask = (byte)(1 << (Material.drawLayerBillboard & 7));
		}
		internal byte BindGL(GeoMesh prev, ref GeoMesh @this)
		{
			if (this.VertexCount != 0)
			{
				int vbo = this.State.Vertex.Gen();

				GL.BindBuffer(BufferTarget.ElementArrayBuffer, vbo);

				GL.BufferData(BufferTarget.ElementArrayBuffer,
					this.VertexBuffer.Length,
					VertexBuffer,
					BufferUsageHint.StaticDraw);

				int ibo = this.State.Index.Gen();

				GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);

				if (IndexElementSize == 4)
					GL.BufferData(
					BufferTarget.ElementArrayBuffer,
					this.IndexCount << 2,
					(uint[])this.IndexBuffer,
					BufferUsageHint.StaticDraw
					);
				else if (IndexElementSize == 2)
					GL.BufferData(
						BufferTarget.ElementArrayBuffer,
						this.IndexCount << 1,
						(ushort[])this.IndexBuffer,
						BufferUsageHint.StaticDraw);
				else
					GL.BufferData(
						BufferTarget.ElementArrayBuffer,
						this.IndexCount,
						(byte[])this.IndexBuffer,
						BufferUsageHint.StaticDraw);

				State.IndexCount = (ushort)IndexCount;
				State.IndexPrimitive = IndexPrimitive.Triangles;
				State.IndexInteger =
						IndexElementSize == 4 ? IndexInteger.Int :
						IndexElementSize == 2 ? IndexInteger.Short :
						IndexInteger.Byte;
				State.LightMode = Material.Smooth ? LightMode.Smooth : LightMode.Hard;

				State.ElementMask = (Material.HasVertexColors ?
					ElementMask.Color : ElementMask.Normal) |
					(Material.HasTexture ?
					 (null != texture && !texture.IsColorTexture) ? 
						(ElementMask.Texture | ElementMask.Texcoord) :
						ElementMask.Texcoord : (ElementMask)0) |
					(ElementMask.Index | ElementMask.Position);

				if (0!=(State.ElementMask & ElementMask.Texture))
					State.Texture = texture.GetLoadedHandle();

				State.TexturePresentation =
					(0 == ((Material.wrapModes >> 8) & 1) ?
						0 == ((Material.wrapModes >> 8) & 2) ?
							(TexturePresentation)0 :
							TexturePresentation.ClampS :
						0 == ((Material.wrapModes >> 8) & 2) ?
							TexturePresentation.MirrorS :
							(TexturePresentation)0) |
							(0 == ((Material.wrapModes) & 1) ?
						0 == ((Material.wrapModes) & 2) ?
							(TexturePresentation)0 :
							TexturePresentation.ClampT :
						0 == ((Material.wrapModes) & 2) ?
							TexturePresentation.MirrorT :
							(TexturePresentation)0);

				State.TextureScaleX = Material.texScaleX;
				State.TextureScaleY = Material.texScaleY;
				State.TextureWidth = Material.w;
				State.TextureHeight = Material.h;

				if (null != texture && 0 != (State.ElementMask & ElementMask.Texcoord))
				{
					State.Texture = texture.GetLoadedHandle();
					State.ElementMask |= ElementMask.Texture;
				}
				State.Fog = Material.fogColor;
				State.FogMultiplier = Material.fogOffset;
				State.FogOffset = Material.fogMultiplier; 
				State.FeatureMask = (FeatureMask)(Material.drawLayerBillboard & 7) |
					(this.Parent.Parent.ZTest ? FeatureMask.ZTest : (FeatureMask)0) |
					(Material.IsLit ? FeatureMask.Lit : (FeatureMask)0) |
					(Material.Fog ? FeatureMask.Fog : (FeatureMask)0);
				State.Color = Material.lightColor;
				State.LightColor = Material.lightColor;
				State.DarkColor = Material.darkColor;
				State.VertexColor = VertexColor.Smooth;
				State.Culling = Material.CullBack ? Material.CullFront ? Culling.Both : Culling.Back : Material.CullFront ? Culling.Front : Culling.Off;

				GraphicsState.Sanitize(ref State);
			}
			return DrawLayerMask;
		}		
	}

	public sealed class GeoModel
	{
		public readonly GeoNode Parent;
		public GeoModel Next;
		public GeoMesh First, Last;
		public uint Count;
		public byte DrawLayerMask;
		private bool Building;

		internal Scripts.TempMesh128.List TempMeshes;

		public GeoModel (GeoNode Parent)
		{
			this.Parent = Parent;
			this.Building = true;
		}
		internal Dictionary<Scripts.TempVertex128, Scripts.TempVertex128> Build(Dictionary<Scripts.TempVertex128, Scripts.TempVertex128> temp=null)
		{
			if (this.Building)
			{
				this.Building = false;
				var List = this.TempMeshes;
				this.TempMeshes = default(Scripts.TempMesh128.List);
				try
				{
					var Iter = List.first;
					var Pos = List.count;
					while (0 != Pos--)
					{
						var New = new GeoMesh(this, Iter, ref temp);
						Iter = Iter.next;

						if (0 == Count++)
						{
							New.Next = New;
							First = New;
						}
						else
						{
							Last.Next = New;
							New.Next = First;
						}
						Last = New;
						DrawLayerMask |= (byte)(1 << (New.Material.drawLayerBillboard & 7));
					}
				}
				finally
				{
					Scripts.TempMesh128.Free(ref List);
				}
			}
			return temp;
		}
		internal void AddVertex(Scripts.TempMesh mesh_non_node, ref Vertex128 vertex)
		{
			if (!Building) return;

			Scripts.TempMesh128 iter;
			uint iter_pos;
			var equator = Scripts.TempMesh.Equator;
			// note that this starts at last on purpose. the nodes are a ring.
			for (iter = TempMeshes.last, iter_pos = TempMeshes.count; 0 != iter_pos; iter = iter.next, --iter_pos)
				if (iter.hashCode == mesh_non_node.hashCode &&
					equator.Equals(ref iter.value, ref mesh_non_node.value))
					break;

			if (0 == iter_pos)
			{
				// need a new mesh.
				var references = new Scripts.TempMesh128References
				{
					bmp = mesh_non_node.references.bmp,
				};
				Scripts.TempMesh128.Add(ref TempMeshes, ref mesh_non_node.value, ref references);
				iter = TempMeshes.last;
			}
			else if (iter_pos != TempMeshes.count)
			{
				// spin it around a bit to optimize following vertices.
				TempMeshes.first = iter.next;
				TempMeshes.last = iter;
			}
			Scripts.TempVertex128.Add(ref iter.references.list, ref vertex);
		}
		internal byte BindGL(GeoModel prev, ref GeoModel @this)
		{
			GeoMesh iter,piter;
			uint iter_pos;
			for (iter = First, piter=Last, iter_pos = Count; 0 != iter_pos; piter=iter, iter=iter.Next, --iter_pos)
				DrawLayerMask |= iter.BindGL(piter, ref iter);
			return DrawLayerMask;
		}
	}
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
				object System.Collections.IEnumerator.Current => Active;
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

	public sealed class GeoNode : GeoParent
	{

		public uint ramAddress;

		public short MinDistance, MaxDistance;
		public bool IsTop => 0 == Depth;
		public bool forceBillboard;
		public byte SelfDrawLayerMask;
		public byte DrawLayerMask;
		public byte ChildrenDrawLayerMask;
		public bool ZTest;
		public bool IsDistanceBased => MinDistance != MaxDistance;
		public bool isBillboard
		{
			get
			{
				var value = forceBillboard;
				var iter = Outer;
				for (var i = Depth; i != 0; iter = iter.Outer, --i)
					if (iter.forceBillboard)
						value = !value;
				return value;
			}
		}
		/// <summary>
		/// the number of parents of this node. zero when this is a direct child of root.
		/// </summary>
		public readonly uint Depth;

		/// <summary>
		/// the index of this child relative to either the parent when depth is non zero or the root when depth is zero.
		/// </summary>
		public readonly uint ChildIndex;

		/// <summary>
		/// a unique identifier to objects of the same root.
		/// </summary>
		public readonly uint ID;

		public uint ModelCount;

		/// <summary>
		/// either the owning node of this one or this. never null.
		/// </summary>
		public readonly GeoNode Outer;

		/// <summary>
		/// the next child of the parent of this child, when this is the newest sibling the sibling points to the oldest.
		/// </summary>
		public GeoNode Sibling;

		/// <summary>
		/// the next created geo node on the root.
		/// </summary>
		public GeoNode Next;

		public GeoModel LastModel, FirstModel;
		public Transform BuiltTransform;
		public TransformI Local;
		public TransformI Accumulated;
		private TransformI LocalCached;
		private bool ExplicitlyDirty;


		internal GeoModel StartModel()
		{
			if (0 == ModelCount++)
			{
				FirstModel = new GeoModel(this);
				FirstModel.Next = FirstModel;
				LastModel = FirstModel;
			}
			else
			{
				var NewModel = new GeoModel(this);
				NewModel.Next = LastModel.Next;
				LastModel.Next = NewModel;
				LastModel = NewModel;
			}
			return LastModel;
		}
		internal void render(RenderList list, RenderObject obj, ref Transform parent, byte drawLayers = 255)
		{
			Transform trs = Local * parent;
			uint pos;
			if (this.forceBillboard)
			{
				list.Camera.MakeBillboard(ref trs);
			}
			if (0 != (drawLayers & ChildrenDrawLayerMask))
			{
				GeoNode node;
				for (pos = NumImmediate, node = FirstChild; 0 != pos; node = node.Sibling, --pos)
					if (0 != (drawLayers & node.DrawLayerMask))
						node.render(list, obj, ref trs, drawLayers);
			}
			if (0 != (drawLayers & SelfDrawLayerMask))
			{
				OpenTK.Vector4 viewProj = list.Camera.ViewProj * new OpenTK.Vector4(trs.translation, 1f);
				GeoModel model;
				for (pos = ModelCount, model = FirstModel; 0 != pos; model = model.Next, --pos)
					if (0 != (drawLayers & model.DrawLayerMask))
						RenderLayer.Insert(list, obj, model, ref trs, ref viewProj, (byte)(drawLayers & model.DrawLayerMask));
			}
		}
		public void draw(GraphicsInterface gi,ref Transform transform, byte drawLayers, ref RenderCamera camTrs, DrawOptions options=0)
		{
			GeoNode NodeIter;
			GeoModel ModelIter;
			GeoMesh MeshIter;
			uint NodeIterPos;
			uint ModelIterPos;
			uint MeshIterPos;
			var new_transform = Local * transform;
			GL.PushMatrix();
			Local.GL_Load();
			if (this.forceBillboard)
				camTrs.MakeBillboardAndRotateGL(ref transform);
			if (0 != (SelfDrawLayerMask & drawLayers))
				for (ModelIter = FirstModel, ModelIterPos = ModelCount; 0 != ModelIterPos; ModelIter = ModelIter.Next, --ModelIterPos)
					if (0 != (ModelIter.DrawLayerMask & drawLayers))
						for (MeshIter = ModelIter.First, MeshIterPos = ModelIter.Count; 0 != MeshIterPos; MeshIter = MeshIter.Next, --MeshIterPos)
							if (0 != (MeshIter.DrawLayerMask & drawLayers))
								gi.Draw(ref MeshIter.State, Options:options);

			for (NodeIter = FirstChild, NodeIterPos = NumImmediate; 0 != NodeIterPos; NodeIter = NodeIter.Sibling, --NodeIterPos)
				if (0 != (NodeIter.DrawLayerMask & drawLayers))
					NodeIter.draw(gi, ref new_transform, drawLayers, ref camTrs, options);

			GL.PopMatrix();
		}

		public void BuildTransform(out TransformI Transform, bool SkipCheck = false)
		{
			if (!SkipCheck)
				Update();
			Transform = Accumulated;
		}
		public void BuildTransform(out Transform Transform, bool SkipCheck=false)
		{
			if(!SkipCheck)
				Update();

			Transform.translation.X = this.BuiltTransform.translation.X;
			Transform.translation.Y = this.BuiltTransform.translation.Y;
			Transform.translation.Z = this.BuiltTransform.translation.Z;
			Transform.scale = this.BuiltTransform.scale;
			Transform.rotation = this.BuiltTransform.rotation;
		}
		public Transform Transform { get { BuildTransform(out Transform o); return o; } }
		public Transform UncheckedTransform { get { BuildTransform(out Transform o, true); return o; } }


		public bool MarkedDirty => ExplicitlyDirty && 0 != Depth;
		public bool LocalModification => TransformI.Inequals(ref Local, ref LocalCached);
		public bool PendingModification => MarkedDirty || LocalModification;

		public GeoNode DirtyOuter
		{
			get
			{
				GeoNode Iter,Furthest=null;
				{
					uint Position;
					for (Position = Depth, Iter = this; 0 != Position; Iter = Iter.Outer, --Position)
						if (Iter.PendingModification)
							Furthest = Iter;
				}
				return Furthest;
			}
		}
		public bool Dirty => null != DirtyOuter;

		public GeoParent Parent => 0 == Depth ? (GeoParent)Root : Outer;

		static short Mult(Fixed16_16 scale, short value)
		{
			if (value < 0)
				return (short)(-((int)((scale * new Fixed16_16 { Whole = (ushort)(-((int)value)), })).Whole));
			else
				return (short)((scale * new Fixed16_16 { Whole = (ushort)value, }).Whole);
		}
		private void Integrate()
		{
			var OldTransform = BuiltTransform;
			var OldAccumulated = Accumulated;
			BuiltTransform = Local;

			if (0 != Depth)
			{
				BuiltTransform *= Outer.BuiltTransform;
				Accumulated = Outer.Accumulated;
				Accumulated.translation.X += Mult(Accumulated.scale, Local.translation.X);
				Accumulated.translation.Y += Mult(Accumulated.scale, Local.translation.Y);
				Accumulated.translation.Z += Mult(Accumulated.scale, Local.translation.Z);
				Accumulated.scale *= Local.scale;
				Accumulated.rotation += Local.rotation;
				//BuiltTransform = Accumulated;
			}

			LocalCached = Local;

			ExplicitlyDirty = false;

			// when the transform actually changed all immediate children need to be marked explicitly.
			if (OldTransform != BuiltTransform || OldAccumulated != Accumulated)
			{
				GeoNode Current;
				uint Pos;
				for (Pos = NumImmediate, Current = LastChild; 0 != Pos; Current = Current.Sibling, --Pos)
					Current.ExplicitlyDirty = true;
			}

		}

		public void Update()
		{
			GeoNode Current;
			for (Current = DirtyOuter; null != Current && this != Current; Current = DirtyOuter)
				if (Current != this)
					if (Current.Depth == 0)
						Current.Integrate();
					else
						Current.Update();

			if (this == Current)
				Integrate();
		}
		private static GeoRoot EnsureParentGetRoot(GeoParent Parent)
		{
			if (null == Parent) throw new ArgumentNullException("Parent");
			if (Parent != Parent.Root && !(Parent is GeoNode)) throw new ArgumentException("Parent must be either a GeoRoot or GeoNode!", "Parent");
			if (null == Parent.Root) throw new ArgumentException("Parent does not reference a valid root", "Parent");
			return Parent.Root;
		}

		public GeoNode(GeoParent Parent) : base(EnsureParentGetRoot(Parent))
		{
			this.BuiltTransform = Transform.Identity;
			this.Local = TransformI.Identity;
			this.LocalCached = TransformI.Identity;

			this.Outer = (Root == Parent) ? this : (GeoNode)Parent;
			this.Depth = this.Outer == this ? 0u : (this.Outer.Depth + 1u);

			if (0 == this.Root.Num++)
			{
				this.Next = this;
			}
			else
			{
				this.Next = this.Root.Last.Next;
				this.Root.Last.Next = this;
			}
			this.Root.Last = this;

			if (0 == (this.ChildIndex = Parent.NumImmediate++))
			{
				this.Sibling = this;
			}
			else
			{
				this.Sibling = Parent.LastChild.Sibling;
				Parent.LastChild.Sibling = this;
			}
			Parent.LastChild = this;

			this.ID = this.Root.IDCounter++;
			this.ExplicitlyDirty = 0 != Depth;
		}

		/// <summary>
		/// iterates from the immediate parent of this to the least immediate parent.
		/// (excludes the root)
		/// </summary>
		public OuterCollection Outers => this;

		/// <summary>
		/// iterates from the least immediate parent of this to the most immediate parent of this.
		/// (excludes the root)
		/// </summary>
		public AscendantCollection Ascendants => this;

		public struct OuterCollection : ICollection<GeoNode>, IEquatable<GeoParent>
		{
			public readonly GeoNode This;

			public bool Equals(GeoParent other) { return This == other; }

			private OuterCollection(GeoNode This) { this.This = This; }

			public static bool operator ==(OuterCollection L, OuterCollection R) { return L.This == R.This; }
			public static bool operator !=(OuterCollection L, OuterCollection R) { return L.This != R.This; }

			public static implicit operator OuterCollection(GeoNode parent) { return new OuterCollection(parent); }

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

			int ICollection<GeoNode>.Count => null == This ? 0 : unchecked((int)This.Depth);

			bool ICollection<GeoNode>.IsReadOnly => true;

			public struct Enumerator : IEnumerator<GeoNode>
			{
				public GeoNode Active;
				public GeoNode Current => Active;
				public uint Remaining;
				public Enumerator(GeoNode Child)
				{
					Remaining = null == Child ? 0u : Child.Depth;
					Active = (Remaining == 0) ? null : Child;
				}
				object System.Collections.IEnumerator.Current => Active;
				public void Dispose()
				{
					Remaining = 0;
					Active = null;
				}
				public void Reset()
				{
				}
				public bool MoveNext()
				{
					bool Result = 0 != Remaining;
					if (!Result)
						Active = null;
					else
					{
						Active = Active.Outer;
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

			public bool Contains(GeoNode item)
			{
				return null != item && item.Parent == This;
			}

			void ICollection<GeoNode>.CopyTo(GeoNode[] array, int arrayIndex)
			{
				if (null == This)
					return;

				var Iter = This;
				for (uint Left = This.Depth; Left != 0; Iter = Iter.Outer, --Left)
					array[arrayIndex++] = (Iter = Iter.Outer);
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

		public struct AscendantCollection : ICollection<GeoNode>, IEquatable<GeoParent>
		{
			public readonly GeoNode This;

			public bool Equals(GeoParent other) { return This == other; }

			private AscendantCollection(GeoNode This) { this.This = This; }

			public static bool operator ==(AscendantCollection L, AscendantCollection R) { return L.This == R.This; }
			public static bool operator !=(AscendantCollection L, AscendantCollection R) { return L.This != R.This; }

			public static implicit operator AscendantCollection(GeoNode parent) { return new AscendantCollection(parent); }

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

			int ICollection<GeoNode>.Count => null == This ? 0 : unchecked((int)This.Depth);

			bool ICollection<GeoNode>.IsReadOnly => true;

			public struct Enumerator : IEnumerator<GeoNode>
			{
				public GeoNode Bottom;
				public GeoNode Current {
					get
					{
						var Iter = Bottom;
						for (uint Pos = Remaining; Pos != 0; --Pos)
							Iter = Iter.Outer;
						return Iter;
					}
				}
				public uint Remaining;
				public Enumerator(GeoNode Child)
				{
					Remaining = null == Child ? 0u : (1u+Child.Depth);
					Bottom = (Remaining == 0) ? null : Child;
				}
				object System.Collections.IEnumerator.Current => Current;
				public void Dispose()
				{
					Remaining = 0;
					Bottom = null;
				}
				public void Reset()
				{
					if (null == Bottom)
						Remaining = 0;
					else
						Remaining = 1u + Bottom.Depth;
				}
				public bool MoveNext()
				{
					bool Result = 0 != Remaining && 0 != --Remaining;
					if (!Result)
						Bottom = null;
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
				return ((OuterCollection)This).Contains(item);
			}

			void ICollection<GeoNode>.CopyTo(GeoNode[] array, int arrayIndex)
			{
				if (null == This)
					return;

				var Iter = This;
				for (uint Left = This.Depth; Left != 0; Iter = Iter.Outer, --Left)
					array[arrayIndex + (int)(Left - 1u)] = (Iter = Iter.Outer);
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

	}
}
