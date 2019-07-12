using System;
using UnityEngine;

namespace MarchingCubes {
	/// <summary> A 3-dimensional jagged array </summary>
	[Serializable] public class VoxelGrid : List3D<Voxel> {
		public VoxelGrid(int x, int y, int z) : base(x, y, z) { }

		public static readonly int[] CubePointsX = { 0, 1, 1, 0, 0, 1, 1, 0 };
		public static readonly int[] CubePointsY = { 0, 0, 0, 0, 1, 1, 1, 1 };
		public static readonly int[] CubePointsZ = { 0, 0, 1, 1, 0, 0, 1, 1 };

		public Voxel GetCorner(int x, int y, int z, int index) {
			return this [x + CubePointsX[index], y + CubePointsY[index], z + CubePointsZ[index]];
		}

		public VoxelCorners GetCorners(int x, int y, int z) {
			Voxel lftDnBk = this [x + 0, y + 0, z + 0];
			Voxel rgtDnBk = this [x + 1, y + 0, z + 0];
			Voxel rgtDnFwd = this [x + 1, y + 0, z + 1];
			Voxel lftDnFwd = this [x + 0, y + 0, z + 1];
			Voxel lftUpBk = this [x + 0, y + 1, z + 0];
			Voxel rgtUpBk = this [x + 1, y + 1, z + 0];
			Voxel rgtUpFwd = this [x + 1, y + 1, z + 1];
			Voxel lftUpFwd = this [x + 0, y + 1, z + 1];
			return new VoxelCorners(lftDnBk, rgtDnBk, rgtDnFwd, lftDnFwd, lftUpBk, rgtUpBk, rgtUpFwd, lftUpFwd);
		}

		public void Subtract(Func<Vector3Int, float, float> densityFunction) {
			int count = 0;
			for (int z = 0; z < this.z; z++) {
				for (int y = 0; y < this.y; y++) {
					for (int x = 0; x < this.x; x++) {
						Voxel voxel = list[count];
						voxel.density = densityFunction(new Vector3Int(x,y,z), voxel.density);
						list[count] = voxel;
						count++;
					}
				}
			}
		}

		public int GetCubeIndex(int x, int y, int z, float isolevel) {
			int cubeIndex = 0;
			for (int i = 0; i < 8; i++) {
				if (this [x + CubePointsX[i], y + CubePointsY[i], z + CubePointsZ[i]].density > isolevel) cubeIndex |= 1 << i;
			}
			return cubeIndex;
		}
	}
}