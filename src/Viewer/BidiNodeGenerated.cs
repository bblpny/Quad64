using System;
using System.Collections.Generic;

namespace Quad64 {
	partial class BidiNodeUtility {

		private struct SortComparer<TNode, TComparer>
			where TNode : class, IBidiNode<TNode> where TComparer : struct, IComparer<TNode> {
			public TNode X, Swap;
			public BidiNodeUtility<TNode>.LinkNode RealFirst, RealLast, I, J;
			public TComparer Comparer;
			private void Partition(
				BidiNodeUtility<TNode>.LinkNode First,
				BidiNodeUtility<TNode>.LinkNode Last)
			{
				for (X = Last.Data, I = First.Prev, J = First; J != Last; J = J.Next)
					if ((object)J.Data == (object)X || 0 >= Comparer.Compare(J.Data,X)){
						I = I.Next;
						Swap=J.Data;
						J.Data=I.Data;
						I.Data=Swap;
					}
				I=I.Next;
				Swap=I.Data;I.Data=Last.Data;
				Last.Data=Swap;
			}
			private void Sort(
				BidiNodeUtility<TNode>.LinkNode First,
				BidiNodeUtility<TNode>.LinkNode Last)
			{
				BidiNodeUtility<TNode>.LinkNode Split;
				if (First != Last && First != Last.Next)
				{
					Partition(First, Last);
					Split = I;

					if(Split!=RealFirst)
						Sort(First, Split.Prev);

					if(Split!=RealLast)
						Sort(Split.Next, Last);
				}
			}

			public bool EnsureSorted()
			{
				for (BidiNodeUtility<TNode>.LinkNode A = RealFirst, 
					B = A.Next; B != RealFirst; 
					A = B, B = B.Next)
					if (Comparer.Compare(A.Data,B.Data) > 0)
						return false;
				return true;
			}

			public void Sort()
			{
				BidiNodeUtility<TNode>.LinkNode Split;
				Partition(RealFirst, RealLast);
				Split = I;

				if(Split!=RealFirst)
					Sort(RealFirst, Split.Prev);

				if(Split!=RealLast)
					Sort(Split.Next, RealLast);
#if DEBUG
				if (!EnsureSorted())
					throw new System.InvalidOperationException("Sort failed");
#endif
			}
		}
		private struct SortComparer<TNode>
			where TNode : class, IBidiNode<TNode> {
			public TNode X, Swap;
			public BidiNodeUtility<TNode>.LinkNode RealFirst, RealLast, I, J;
			public IComparer<TNode> Comparer;
			private void Partition(
				BidiNodeUtility<TNode>.LinkNode First,
				BidiNodeUtility<TNode>.LinkNode Last)
			{
				for (X = Last.Data, I = First.Prev, J = First; J != Last; J = J.Next)
					if ((object)J.Data == (object)X || 0 >= Comparer.Compare(J.Data,X)){
						I = I.Next;
						Swap=J.Data;
						J.Data=I.Data;
						I.Data=Swap;
					}
				I=I.Next;
				Swap=I.Data;I.Data=Last.Data;
				Last.Data=Swap;
			}
			private void Sort(
				BidiNodeUtility<TNode>.LinkNode First,
				BidiNodeUtility<TNode>.LinkNode Last)
			{
				BidiNodeUtility<TNode>.LinkNode Split;
				if (First != Last && First != Last.Next)
				{
					Partition(First, Last);
					Split = I;

					if(Split!=RealFirst)
						Sort(First, Split.Prev);

					if(Split!=RealLast)
						Sort(Split.Next, Last);
				}
			}

			public bool EnsureSorted()
			{
				for (BidiNodeUtility<TNode>.LinkNode A = RealFirst, 
					B = A.Next; B != RealFirst; 
					A = B, B = B.Next)
					if (Comparer.Compare(A.Data,B.Data) > 0)
						return false;
				return true;
			}

			public void Sort()
			{
				BidiNodeUtility<TNode>.LinkNode Split;
				Partition(RealFirst, RealLast);
				Split = I;

				if(Split!=RealFirst)
					Sort(RealFirst, Split.Prev);

				if(Split!=RealLast)
					Sort(Split.Next, RealLast);
#if DEBUG
				if (!EnsureSorted())
					throw new System.InvalidOperationException("Sort failed");
#endif
			}
		}
		private struct SortComparison<TNode>
			where TNode : class, IBidiNode<TNode> {
			public TNode X, Swap;
			public BidiNodeUtility<TNode>.LinkNode RealFirst, RealLast, I, J;
			public Comparison<TNode> Comparison;
			private void Partition(
				BidiNodeUtility<TNode>.LinkNode First,
				BidiNodeUtility<TNode>.LinkNode Last)
			{
				for (X = Last.Data, I = First.Prev, J = First; J != Last; J = J.Next)
					if ((object)J.Data == (object)X || 0 >= Comparison(J.Data,X)){
						I = I.Next;
						Swap=J.Data;
						J.Data=I.Data;
						I.Data=Swap;
					}
				I=I.Next;
				Swap=I.Data;I.Data=Last.Data;
				Last.Data=Swap;
			}
			private void Sort(
				BidiNodeUtility<TNode>.LinkNode First,
				BidiNodeUtility<TNode>.LinkNode Last)
			{
				BidiNodeUtility<TNode>.LinkNode Split;
				if (First != Last && First != Last.Next)
				{
					Partition(First, Last);
					Split = I;

					if(Split!=RealFirst)
						Sort(First, Split.Prev);

					if(Split!=RealLast)
						Sort(Split.Next, Last);
				}
			}

			public bool EnsureSorted()
			{
				for (BidiNodeUtility<TNode>.LinkNode A = RealFirst, 
					B = A.Next; B != RealFirst; 
					A = B, B = B.Next)
					if (Comparison(A.Data,B.Data) > 0)
						return false;
				return true;
			}

			public void Sort()
			{
				BidiNodeUtility<TNode>.LinkNode Split;
				Partition(RealFirst, RealLast);
				Split = I;

				if(Split!=RealFirst)
					Sort(RealFirst, Split.Prev);

				if(Split!=RealLast)
					Sort(Split.Next, RealLast);
#if DEBUG
				if (!EnsureSorted())
					throw new System.InvalidOperationException("Sort failed");
#endif
			}
		}
		private struct SortComparable<TNode>
			where TNode : class, IBidiNode<TNode> , IComparable<TNode> {
			public TNode X, Swap;
			public BidiNodeUtility<TNode>.LinkNode RealFirst, RealLast, I, J;
			private void Partition(
				BidiNodeUtility<TNode>.LinkNode First,
				BidiNodeUtility<TNode>.LinkNode Last)
			{
				for (X = Last.Data, I = First.Prev, J = First; J != Last; J = J.Next)
					if ((object)J.Data == (object)X || 0 >= J.Data.CompareTo(X)){
						I = I.Next;
						Swap=J.Data;
						J.Data=I.Data;
						I.Data=Swap;
					}
				I=I.Next;
				Swap=I.Data;I.Data=Last.Data;
				Last.Data=Swap;
			}
			private void Sort(
				BidiNodeUtility<TNode>.LinkNode First,
				BidiNodeUtility<TNode>.LinkNode Last)
			{
				BidiNodeUtility<TNode>.LinkNode Split;
				if (First != Last && First != Last.Next)
				{
					Partition(First, Last);
					Split = I;

					if(Split!=RealFirst)
						Sort(First, Split.Prev);

					if(Split!=RealLast)
						Sort(Split.Next, Last);
				}
			}

			public bool EnsureSorted()
			{
				for (BidiNodeUtility<TNode>.LinkNode A = RealFirst, 
					B = A.Next; B != RealFirst; 
					A = B, B = B.Next)
					if (A.Data.CompareTo(B.Data) > 0)
						return false;
				return true;
			}

			public void Sort()
			{
				BidiNodeUtility<TNode>.LinkNode Split;
				Partition(RealFirst, RealLast);
				Split = I;

				if(Split!=RealFirst)
					Sort(RealFirst, Split.Prev);

				if(Split!=RealLast)
					Sort(Split.Next, RealLast);
#if DEBUG
				if (!EnsureSorted())
					throw new System.InvalidOperationException("Sort failed");
#endif
			}
		}
		public static void Sort<TNode, TComparer>(ref BidiNodeUtility<TNode>.LinkList List, ref TComparer Comparer)
			where TNode : class, IBidiNode<TNode> where TComparer : struct, IComparer<TNode>{
			TNode Swap;
			if (List.Count > 1u)
			{
				if (List.Count == 2)
				{
					if (Comparer.Compare(List.First.Data,List.Last.Data) > 0)
					{
						Swap = List.First.Data;
						List.First.Data = List.Last.Data;
						List.Last.Data = Swap;
					}
				}
				else
				{
					new SortComparer<TNode, TComparer>
					{
						RealFirst = List.First,
						RealLast = List.Last,
						Comparer = Comparer,
					}.Sort();
				}
			}
		}
		public static void Sort<TNode, TComparer>(
			IBidiNodeList<TNode> List, ref TComparer Comparer)
			where TNode : class, IBidiNode<TNode> where TComparer : struct, IComparer<TNode>{
			BidiNodeUtility<TNode>.LinkList Link;
			BidiNodeUtility<TNode>.Alloc(List, out Link);
			Sort(ref Link, Comparer);
			List.First = Link.First.Data;
			List.Last = Link.Last.Data;
			BidiNodeUtility<TNode>.Assign(ref Link);
		}
		public static void Sort<TNode, TComparer, TNodeList>(
			ref TNodeList List, ref TComparer Comparer)
			where TNode : class, IBidiNode<TNode> where TComparer : struct, IComparer<TNode> where TNodeList : struct, IBidiNodeList<TNode> {
			BidiNodeUtility<TNode>.LinkList Link;
			BidiNodeUtility<TNode>.Alloc(ref List, out Link);
			Sort(ref Link, Comparer);
			List.First = Link.First.Data;
			List.Last = Link.Last.Data;
			BidiNodeUtility<TNode>.Assign(ref Link);
		}
		public static void Sort<TNode, TComparer>(
			ref BidiNodeUtility<TNode>.Dump Dump, IBidiNodeList<TNode> List, ref TComparer Comparer)
			where TNode : class, IBidiNode<TNode> where TComparer : struct, IComparer<TNode>{
			BidiNodeUtility<TNode>.LinkList Link;
			BidiNodeUtility<TNode>.Alloc(List, ref Dump, out Link);
			Sort(ref Link, Comparer);
			List.First = Link.First.Data;
			List.Last = Link.Last.Data;
			BidiNodeUtility<TNode>.FreeAssign(ref Link, ref Dump);
		}
		public static void Sort<TNode, TComparer, TNodeList>(
			ref BidiNodeUtility<TNode>.Dump Dump, ref TNodeList List, ref TComparer Comparer)
			where TNode : class, IBidiNode<TNode> where TComparer : struct, IComparer<TNode> where TNodeList : struct, IBidiNodeList<TNode> {
			BidiNodeUtility<TNode>.LinkList Link;
			BidiNodeUtility<TNode>.Alloc(ref List, ref Dump, out Link);
			Sort(ref Link, Comparer);
			List.First = Link.First.Data;
			List.Last = Link.Last.Data;
			BidiNodeUtility<TNode>.FreeAssign(ref Link, ref Dump);
		}
		public static void Sort<TNode>(ref BidiNodeUtility<TNode>.LinkList List, IComparer<TNode> Comparer)
			where TNode : class, IBidiNode<TNode>{
			TNode Swap;
			if (List.Count > 1u)
			{
				if (List.Count == 2)
				{
					if (Comparer.Compare(List.First.Data,List.Last.Data) > 0)
					{
						Swap = List.First.Data;
						List.First.Data = List.Last.Data;
						List.Last.Data = Swap;
					}
				}
				else
				{
					new SortComparer<TNode>
					{
						RealFirst = List.First,
						RealLast = List.Last,
						Comparer = Comparer,
					}.Sort();
				}
			}
		}
		public static void Sort<TNode>(
			IBidiNodeList<TNode> List, IComparer<TNode> Comparer)
			where TNode : class, IBidiNode<TNode>{
			BidiNodeUtility<TNode>.LinkList Link;
			BidiNodeUtility<TNode>.Alloc(List, out Link);
			Sort(ref Link, Comparer);
			List.First = Link.First.Data;
			List.Last = Link.Last.Data;
			BidiNodeUtility<TNode>.Assign(ref Link);
		}
		public static void Sort<TNode, TNodeList>(
			ref TNodeList List, IComparer<TNode> Comparer)
			where TNode : class, IBidiNode<TNode> where TNodeList : struct, IBidiNodeList<TNode> {
			BidiNodeUtility<TNode>.LinkList Link;
			BidiNodeUtility<TNode>.Alloc(ref List, out Link);
			Sort(ref Link, Comparer);
			List.First = Link.First.Data;
			List.Last = Link.Last.Data;
			BidiNodeUtility<TNode>.Assign(ref Link);
		}
		public static void Sort<TNode>(
			ref BidiNodeUtility<TNode>.Dump Dump, IBidiNodeList<TNode> List, IComparer<TNode> Comparer)
			where TNode : class, IBidiNode<TNode>{
			BidiNodeUtility<TNode>.LinkList Link;
			BidiNodeUtility<TNode>.Alloc(List, ref Dump, out Link);
			Sort(ref Link, Comparer);
			List.First = Link.First.Data;
			List.Last = Link.Last.Data;
			BidiNodeUtility<TNode>.FreeAssign(ref Link, ref Dump);
		}
		public static void Sort<TNode, TNodeList>(
			ref BidiNodeUtility<TNode>.Dump Dump, ref TNodeList List, IComparer<TNode> Comparer)
			where TNode : class, IBidiNode<TNode> where TNodeList : struct, IBidiNodeList<TNode> {
			BidiNodeUtility<TNode>.LinkList Link;
			BidiNodeUtility<TNode>.Alloc(ref List, ref Dump, out Link);
			Sort(ref Link, Comparer);
			List.First = Link.First.Data;
			List.Last = Link.Last.Data;
			BidiNodeUtility<TNode>.FreeAssign(ref Link, ref Dump);
		}
		public static void Sort<TNode>(ref BidiNodeUtility<TNode>.LinkList List, Comparison<TNode> Comparison)
			where TNode : class, IBidiNode<TNode>{
			TNode Swap;
			if (List.Count > 1u)
			{
				if (List.Count == 2)
				{
					if (Comparison(List.First.Data,List.Last.Data) > 0)
					{
						Swap = List.First.Data;
						List.First.Data = List.Last.Data;
						List.Last.Data = Swap;
					}
				}
				else
				{
					new SortComparison<TNode>
					{
						RealFirst = List.First,
						RealLast = List.Last,
						Comparison = Comparison,
					}.Sort();
				}
			}
		}
		public static void Sort<TNode>(
			IBidiNodeList<TNode> List, Comparison<TNode> Comparison)
			where TNode : class, IBidiNode<TNode>{
			BidiNodeUtility<TNode>.LinkList Link;
			BidiNodeUtility<TNode>.Alloc(List, out Link);
			Sort(ref Link, Comparison);
			List.First = Link.First.Data;
			List.Last = Link.Last.Data;
			BidiNodeUtility<TNode>.Assign(ref Link);
		}
		public static void Sort<TNode, TNodeList>(
			ref TNodeList List, Comparison<TNode> Comparison)
			where TNode : class, IBidiNode<TNode> where TNodeList : struct, IBidiNodeList<TNode> {
			BidiNodeUtility<TNode>.LinkList Link;
			BidiNodeUtility<TNode>.Alloc(ref List, out Link);
			Sort(ref Link, Comparison);
			List.First = Link.First.Data;
			List.Last = Link.Last.Data;
			BidiNodeUtility<TNode>.Assign(ref Link);
		}
		public static void Sort<TNode>(
			ref BidiNodeUtility<TNode>.Dump Dump, IBidiNodeList<TNode> List, Comparison<TNode> Comparison)
			where TNode : class, IBidiNode<TNode>{
			BidiNodeUtility<TNode>.LinkList Link;
			BidiNodeUtility<TNode>.Alloc(List, ref Dump, out Link);
			Sort(ref Link, Comparison);
			List.First = Link.First.Data;
			List.Last = Link.Last.Data;
			BidiNodeUtility<TNode>.FreeAssign(ref Link, ref Dump);
		}
		public static void Sort<TNode, TNodeList>(
			ref BidiNodeUtility<TNode>.Dump Dump, ref TNodeList List, Comparison<TNode> Comparison)
			where TNode : class, IBidiNode<TNode> where TNodeList : struct, IBidiNodeList<TNode> {
			BidiNodeUtility<TNode>.LinkList Link;
			BidiNodeUtility<TNode>.Alloc(ref List, ref Dump, out Link);
			Sort(ref Link, Comparison);
			List.First = Link.First.Data;
			List.Last = Link.Last.Data;
			BidiNodeUtility<TNode>.FreeAssign(ref Link, ref Dump);
		}
		public static void Sort<TNode>(ref BidiNodeUtility<TNode>.LinkList List)
			where TNode : class, IBidiNode<TNode> , IComparable<TNode>{
			TNode Swap;
			if (List.Count > 1u)
			{
				if (List.Count == 2)
				{
					if (List.First.Data.CompareTo(List.Last.Data) > 0)
					{
						Swap = List.First.Data;
						List.First.Data = List.Last.Data;
						List.Last.Data = Swap;
					}
				}
				else
				{
					new SortComparable<TNode>
					{
						RealFirst = List.First,
						RealLast = List.Last,
					}.Sort();
				}
			}
		}
		public static void Sort<TNode>(
			IBidiNodeList<TNode> List)
			where TNode : class, IBidiNode<TNode> , IComparable<TNode>{
			BidiNodeUtility<TNode>.LinkList Link;
			BidiNodeUtility<TNode>.Alloc(List, out Link);
			Sort(ref Link);
			List.First = Link.First.Data;
			List.Last = Link.Last.Data;
			BidiNodeUtility<TNode>.Assign(ref Link);
		}
		public static void Sort<TNode, TNodeList>(
			ref TNodeList List)
			where TNode : class, IBidiNode<TNode> , IComparable<TNode> where TNodeList : struct, IBidiNodeList<TNode> {
			BidiNodeUtility<TNode>.LinkList Link;
			BidiNodeUtility<TNode>.Alloc(ref List, out Link);
			Sort(ref Link);
			List.First = Link.First.Data;
			List.Last = Link.Last.Data;
			BidiNodeUtility<TNode>.Assign(ref Link);
		}
		public static void Sort<TNode>(
			ref BidiNodeUtility<TNode>.Dump Dump, IBidiNodeList<TNode> List)
			where TNode : class, IBidiNode<TNode> , IComparable<TNode>{
			BidiNodeUtility<TNode>.LinkList Link;
			BidiNodeUtility<TNode>.Alloc(List, ref Dump, out Link);
			Sort(ref Link);
			List.First = Link.First.Data;
			List.Last = Link.Last.Data;
			BidiNodeUtility<TNode>.FreeAssign(ref Link, ref Dump);
		}
		public static void Sort<TNode, TNodeList>(
			ref BidiNodeUtility<TNode>.Dump Dump, ref TNodeList List)
			where TNode : class, IBidiNode<TNode> , IComparable<TNode> where TNodeList : struct, IBidiNodeList<TNode> {
			BidiNodeUtility<TNode>.LinkList Link;
			BidiNodeUtility<TNode>.Alloc(ref List, ref Dump, out Link);
			Sort(ref Link);
			List.First = Link.First.Data;
			List.Last = Link.Last.Data;
			BidiNodeUtility<TNode>.FreeAssign(ref Link, ref Dump);
		}
	}
}
