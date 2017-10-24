using System;
using System.Collections.Generic;

namespace Quad64
{
	// implementing this on TNode makes it sortable.
	public interface IBidiNode<TNode> where TNode : class, IBidiNode<TNode>
	{
		TNode Self { get; }
		TNode Next { get; set; }
		TNode Prev { get; set; }
	}

	// implementing this makes it sortable.
	public interface IBidiNodeList<TNode> where TNode : class, IBidiNode<TNode>
	{
		TNode First { get; set; }
		TNode Last { get; set; }
		uint Count { get; }
	}

	public static partial class BidiNodeUtility<TNode> where TNode : class, IBidiNode<TNode>
	{
		public sealed class LinkNode : IBidiNode<LinkNode>, IEquatable<LinkNode>
		{
			public TNode Data;
			public LinkNode Next, Prev;
			LinkNode IBidiNode<LinkNode>.Self => this;
			LinkNode IBidiNode<LinkNode>.Next { get => Next; set => Next = value; }
			LinkNode IBidiNode<LinkNode>.Prev { get => Prev; set => Prev = value; }

			public sealed override bool Equals(object obj)
			{
				return obj == (object)this;
			}
			public sealed override int GetHashCode()
			{
				return base.GetHashCode();
			}
			public sealed override string ToString()
			{
				return "SortNode";
			}
			public bool Equals(LinkNode other)
			{
				return (object)this == (object)other;
			}
		}

		public struct Dump : IEquatable<Dump>
		{
			public object Lock;
			public LinkNode Top;
			public uint Size;

			public override int GetHashCode()
			{
				unchecked
				{
					return (int)(Size * 5u) ^ (null == Top ? 0 : Top.GetHashCode());
				}
			}
			public bool Equals(ref Dump other)
			{
				return Top == other.Top && Size == other.Size;
			}
			public bool Equals(Dump other)
			{
				return Top == other.Top && Size == other.Size;
			}
			public static bool operator ==(Dump L, Dump R)
			{
				return L.Top == R.Top && L.Size == R.Size;
			}
			public static bool operator !=(Dump L, Dump R)
			{
				return L.Top != R.Top || L.Size != R.Size;
			}
			public static bool Equals(ref Dump L, ref Dump R)
			{
				return L.Top == R.Top && L.Size == R.Size;
			}
			public static bool Inequals(ref Dump L, ref Dump R)
			{
				return L.Top != R.Top || L.Size != R.Size;
			}
			public override bool Equals(object obj)
			{
				return obj is Dump && ((Dump)obj).Equals(ref this);
			}
			public override string ToString()
			{
				return Size == 0 ? "Count=0" : string.Concat("Count=", Size.ToString());
			}
		}

		public struct LinkList : IEquatable<LinkList>
		{
			public LinkNode First, Last;
			public uint Count;

			public override int GetHashCode()
			{
				unchecked
				{
					return (int)(Count * 5u) ^ (null == First ? 0 : First.GetHashCode());
				}
			}
			public bool Equals(ref LinkList other)
			{
				return First == other.First && Count == other.Count && Last == other.Last;
			}
			public bool Equals(LinkList other)
			{
				return First == other.First && Count == other.Count && Last == other.Last;
			}
			public static bool operator ==(LinkList L, LinkList R)
			{
				return L.First == R.First && L.Count == R.Count && L.Last == R.Last;
			}
			public static bool operator !=(LinkList L, LinkList R)
			{
				return L.First != R.First || L.Count != R.Count || L.Last != R.Last;
			}
			public static bool Equals(ref LinkList L, ref LinkList R)
			{
				return L.First == R.First && L.Count == R.Count && L.Last == R.Last;
			}
			public static bool Inequals(ref LinkList L, ref LinkList R)
			{
				return L.First != R.First || L.Count != R.Count || L.Last != R.Last;
			}
			public override bool Equals(object obj)
			{
				return obj is LinkList && ((LinkList)obj).Equals(ref this);
			}
			public override string ToString()
			{
				return Count == 0 ? "Count=0" : string.Concat("Count=", Count.ToString());
			}
		}

		private static void AllocFromDump(ref Dump Dump, ref LinkList Linked, TNode First, out uint DumpAlloc, ref bool GotEnd, uint Count)
		{
			if (0 != Dump.Size)
			{
				Linked.First = Dump.Top;

				if (Count >= Dump.Size)
				{
					DumpAlloc = Dump.Size;
					Dump.Top = null;
					Dump.Size = 0;
				}
				else
				{
					for (Linked.Last = Linked.First, DumpAlloc = Count; 0 != DumpAlloc;)
					{
						Linked.Last.Data = First;
						if (0 != --DumpAlloc)
						{
							First = First.Next;
							Linked.Last.Next.Prev = Linked.Last;
							Linked.Last = Linked.Last.Next;
						}
					}
					Dump.Size -= Count;
					Dump.Top = Linked.Last.Next;
					GotEnd = true;
				}
			}
			else
			{
				DumpAlloc = 0;
			}
		}

		public static void Alloc(out LinkList Linked, TNode First, uint Count)
		{
			if (0 == Count)
			{
				Linked = default(LinkList);
			}
			else
			{
				for (
					Linked.First = new LinkNode() { Data = First, },
					First = First.Next,
					Linked.Last = Linked.First,
					Linked.Count = 1;
					Linked.Count < Count;
					Linked.Last = Linked.Last.Next,
					First = First.Next,
					++Linked.Count)
					Linked.Last.Next = new LinkNode() { Prev = Linked.Last, Data = First, };

				Linked.Last.Next = Linked.First;
				Linked.First.Prev = Linked.Last;
			}
		}
		public static void Alloc(ref Dump Dump, out LinkList Linked, TNode First, uint Count)
		{
			if (0 == Count)
			{
				Linked = default(LinkList);
			}
			else
			{
				Linked.First = null;
				Linked.Last = null;
				// the below is important for dump alloc.
				Linked.Count = Count;
				uint DumpAlloc;
				bool GotEnd = false;

				if (null == Dump.Lock)
					AllocFromDump(ref Dump, ref Linked, First, out DumpAlloc, ref GotEnd, Count);
				else
					lock (Dump.Lock) AllocFromDump(ref Dump, ref Linked, First, out DumpAlloc, ref GotEnd, Count);
				if (!GotEnd && 0 == DumpAlloc)
				{
					Alloc(out Linked, First, Count);
				}
				else
				{
					if (!GotEnd)
					{
						Linked.First.Data = First;
						First = First.Next;

						for (Linked.Last = Linked.First, Linked.Count = 1; Linked.Count < Count; ++Linked.Count)
						{
							if (DumpAlloc <= Linked.Count)
								Linked.Last.Next = new LinkNode();
							Linked.Last.Next.Prev = Linked.Last;
							Linked.Last = Linked.Last.Next;
							Linked.Last.Data = First;
							First = First.Next;
						}
					}
				}
				Linked.Last.Next = Linked.First;
				Linked.First.Prev = Linked.Last;
			}
		}
		public static void Assign(ref LinkList List)
		{
			LinkNode Node;
			uint Pos;
			for (Pos = List.Count, Node = List.First; 0 != Pos; Node = Node.Next, --Pos)
			{
				Node.Data.Next = Node.Next.Data;
				Node.Data.Prev = Node.Prev.Data;
			}
		}
		private static void ListFreeAssign(ref LinkList List)
		{
			LinkNode Node;
			TNode Prev;
			uint Position;
			if (List.Count != 0)
			{
				for (Node = List.First, Prev = Node.Prev.Data, Position = List.Count; 0 != Position;
					Node = Node.Next, --Position)
				{
					Prev.Next = Node.Data;
					Node.Data.Prev = Prev;
					Prev = Node.Data;
					Node.Data = null;
					Node.Prev = null;
				}
			}
		}

		private static void ListFree(ref LinkList List)
		{
			LinkNode Node;
			uint Position;
			for (Node = List.First, Position = List.Count;
				0 != Position;
				Node = Node.Next, --Position)
			{
				Node.Data = null;
				Node.Prev = null;
			}
		}
		private static void DumpAppend(ref LinkList List, ref Dump Dump)
		{
			if (0 == Dump.Size)
			{
				List.Last.Next = null;
				Dump.Size = List.Count;
			}
			else
			{
				List.Last.Next = Dump.Top;
				Dump.Size += List.Count;
			}
			Dump.Top = List.First;
		}
		private static void DumpFree(ref LinkList List, ref Dump Dump)
		{
			if (List.Count != 0)
				if (null == Dump.Lock)
					DumpAppend(ref List, ref Dump);
				else lock (Dump.Lock)
						DumpAppend(ref List, ref Dump);
			List = default(LinkList);
		}

		public static void FreeAssign(ref LinkList List)
		{
			ListFreeAssign(ref List);
			List = default(LinkList);
		}
		public static void Free(ref LinkList List)
		{
			ListFree(ref List);
			List = default(LinkList);
		}
		public static void Free(ref LinkList List, bool Assign)
		{
			if (Assign) ListFreeAssign(ref List);
			else ListFree(ref List);
			List = default(LinkList);
		}
		public static void FreeAssign(ref LinkList List, ref Dump Dump)
		{
			ListFreeAssign(ref List);
			DumpFree(ref List, ref Dump);
		}
		public static void Free(ref LinkList List, ref Dump Dump)
		{
			ListFree(ref List);
			DumpFree(ref List, ref Dump);
		}
		public static void Free(ref LinkList List, ref Dump Dump, bool Assign)
		{
			if (Assign) ListFreeAssign(ref List);
			else ListFree(ref List);
			DumpFree(ref List, ref Dump);
		}
		public static void Alloc(IBidiNodeList<TNode> NodeList, out LinkList List)
		{
			uint Count;
			if (null == NodeList || 0 == (Count = NodeList.Count))
			{
				List = default(LinkList);
			}
			else
			{
				Alloc(out List, NodeList.First, Count);
			}
		}
		public static void Alloc(IBidiNodeList<TNode> NodeList, ref Dump Dump, out LinkList List)
		{
			uint Count;
			if (null == NodeList || 0 == (Count = NodeList.Count))
			{
				List = default(LinkList);
			}
			else
			{
				Alloc(ref Dump, out List, NodeList.First, Count);
			}
		}
		public static void Alloc<TList>(ref TList NodeList, out LinkList List)
			where TList : struct, IBidiNodeList<TNode>
		{
			uint Count;
			if (0 == (Count = NodeList.Count))
			{
				List = default(LinkList);
			}
			else
			{
				Alloc(out List, NodeList.First, Count);
			}
		}
		public static void Alloc<TList>(ref TList NodeList, ref Dump Dump, out LinkList List)
			where TList : struct, IBidiNodeList<TNode>
		{
			uint Count;
			if (0 == (Count = NodeList.Count))
			{
				List = default(LinkList);
			}
			else
			{
				Alloc(ref Dump, out List, NodeList.First, Count);
			}
		}
	}
	public static partial class BidiNodeUtility
	{
	}
}
