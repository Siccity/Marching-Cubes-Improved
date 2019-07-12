using System;
using UnityEngine;

namespace MarchingCubes {
	/// <summary> A 3-dimensional jagged array </summary>
	[Serializable] public class ChunkGrid : List3D<Chunk> {
		public ChunkGrid(int x, int y, int z) : base(x, y, z) { }

	}
}