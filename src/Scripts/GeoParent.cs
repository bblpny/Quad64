using System;
using System.Collections;
using System.Collections.Generic;

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
#if DEBUG
		public ushort JumpCall;
		public string PipeString => 0 == JumpCall ? "| " : "|:";
#endif
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
}
