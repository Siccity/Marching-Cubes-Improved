using System;

namespace MarchingCubes {
	public struct VoxelCorners {
		public Voxel this [int i] {
			get {
				switch (i) {
					/* fixformat ignore:start */
					case 0: return lftDnBk;
					case 1: return rgtDnBk;
					case 2: return rgtDnFwd;
					case 3: return lftDnFwd;
					case 4: return lftUpBk;
					case 5: return rgtUpBk;
					case 6: return rgtUpFwd;
					case 7: return lftUpFwd;
					default: throw new IndexOutOfRangeException();
					/* fixformat ignore:end */
				}
			}
			set {
				switch (i) {
					/* fixformat ignore:start */
					case 0: lftDnBk = value; break;
					case 1: rgtDnBk = value; break;
					case 2: rgtDnFwd = value; break;
					case 3: lftDnFwd = value; break;
					case 4: lftUpBk = value; break;
					case 5: rgtUpBk = value; break;
					case 6: rgtUpFwd = value; break;
					case 7: lftUpFwd = value; break;
					default: throw new IndexOutOfRangeException();
					/* fixformat ignore:end */
				}
			}
		}

		public Voxel lftDnBk;
		public Voxel rgtDnBk;
		public Voxel rgtDnFwd;
		public Voxel lftDnFwd;
		public Voxel lftUpBk;
		public Voxel rgtUpBk;
		public Voxel rgtUpFwd;
		public Voxel lftUpFwd;

		public VoxelCorners(Voxel lftDnBk, Voxel rgtDnBk, Voxel rgtDnFwd, Voxel lftDnFwd, Voxel lftUpBk, Voxel rgtUpBk, Voxel rgtUpFwd, Voxel lftUpFwd) {
			this.lftDnBk = lftDnBk;
			this.rgtDnBk = rgtDnBk;
			this.rgtDnFwd = rgtDnFwd;
			this.lftDnFwd = lftDnFwd;
			this.lftUpBk = lftUpBk;
			this.rgtUpBk = rgtUpBk;
			this.rgtUpFwd = rgtUpFwd;
			this.lftUpFwd = lftUpFwd;
		}

		public int GetCubeIndex(float isolevel) {
			int cubeIndex = 0;
			for (int i = 0; i < 8; i++) {
				if (this [i].density < isolevel) cubeIndex |= 1 << i;
			}
			return cubeIndex;
		}
	}
}