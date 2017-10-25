using System;
using System.Collections.Generic;


namespace Quad64
{
	using TempVertex128 = Scripts.TempVertex128;
	using TempMesh128 = Scripts.TempMesh128;
	using TempMesh128References = Scripts.TempMesh128References;

	public sealed class GeoModel
	{
		public readonly GeoNode Parent;
		public GeoModel Next;
		public GeoMesh First, Last;
		public uint Count;
		public byte DrawLayerMask;
		private bool Building;

		internal TempMesh128.List TempMeshes;

		public GeoModel(GeoNode Parent)
		{
			this.Parent = Parent;
			this.Building = true;
		}
		internal Dictionary<TempVertex128, TempVertex128> Build(Dictionary<TempVertex128, TempVertex128> temp = null)
		{
			if (this.Building)
			{
				this.Building = false;
				var List = this.TempMeshes;
				this.TempMeshes = default(TempMesh128.List);
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
					TempMesh128.Free(ref List);
				}
			}
			return temp;
		}
		internal void AddVertex(Scripts.TempMesh mesh_non_node, ref Vertex128 vertex)
		{
			if (!Building) return;

			TempMesh128 iter;
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
				var references = new TempMesh128References
				{
					bmp = mesh_non_node.references.bmp,
				};
				TempMesh128.Add(ref TempMeshes, ref mesh_non_node.value, ref references);
				iter = TempMeshes.last;
			}
			else if (iter_pos != TempMeshes.count)
			{
				// spin it around a bit to optimize following vertices.
				TempMeshes.first = iter.next;
				TempMeshes.last = iter;
			}
			TempVertex128.Add(ref iter.references.list, ref vertex);
		}
		internal byte BindGL(GeoModel prev, ref GeoModel @this)
		{
			GeoMesh iter, piter;
			uint iter_pos;
			for (iter = First, piter = Last, iter_pos = Count; 0 != iter_pos; piter = iter, iter = iter.Next, --iter_pos)
				DrawLayerMask |= iter.BindGL(piter, ref iter);
			return DrawLayerMask;
		}
	}
}
