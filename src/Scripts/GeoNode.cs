using System;
using System.Collections;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using BubblePony.Integers;
using BubblePony.GLHandle;
using System.Diagnostics;

namespace Quad64
{
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
				LastModel.Next = (new GeoModel(this)
				{
					Next = LastModel.Next,
				});
				LastModel = LastModel.Next;
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
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public Transform Transform { get { BuildTransform(out Transform o); return o; } }
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
