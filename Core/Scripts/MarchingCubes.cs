using UnityEngine;

namespace MarchingCubes {
	public static class MarchingCubes {
		private static Vector3[] vertices;
		private static int[] triangles;

		private static int vertexIndex;

		private static Vector3[] vertexList = new Vector3[12];
		private static Voxel[] initPoints = new Voxel[8];
		private static int[, , ] cubeIndexes;

		public static Mesh CreateMeshData(VoxelList voxels, float isolevel) {
			cubeIndexes = new int[voxels.X - 1, voxels.Y - 1, voxels.Z - 1];
			cubeIndexes = GenerateCubeIndexes(voxels, isolevel);
			int vertexCount = GenerateVertexCount(cubeIndexes);

			if (vertexCount <= 0) {
				return new Mesh();
			}

			vertices = new Vector3[vertexCount];
			triangles = new int[vertexCount];

			for (int x = 0; x < voxels.X - 1; x++) {
				for (int y = 0; y < voxels.Y - 1; y++) {
					for (int z = 0; z < voxels.Z - 1; z++) {
						int cubeIndex = cubeIndexes[x, y, z];
						if (cubeIndex == 0 || cubeIndex == 255) continue;

						March(GetPoints(x, y, z, voxels), cubeIndex, isolevel);
					}
				}
			}

			vertexIndex = 0;

			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.SetTriangles(triangles, 0);
			mesh.RecalculateNormals();

			return mesh;
		}

		private static Vector3 VertexInterpolate(Vector3 p1, Vector3 p2, float v1, float v2, float isolevel) {
			if (Utils.Abs(isolevel - v1) < 0.000001f) {
				return p1;
			}
			if (Utils.Abs(isolevel - v2) < 0.000001f) {
				return p2;
			}
			if (Utils.Abs(v1 - v2) < 0.000001f) {
				return p1;
			}

			float mu = (isolevel - v1) / (v2 - v1);

			Vector3 p = p1 + mu * (p2 - p1);

			return p;
		}

		private static void March(Voxel[] points, int cubeIndex, float isolevel) {
			int edgeIndex = LookupTables.EdgeTable[cubeIndex];

			vertexList = GenerateVertexList(points, edgeIndex, isolevel);

			int[] row = LookupTables.TriangleTable[cubeIndex];

			for (int i = 0; i < row.Length; i += 3) {
				vertices[vertexIndex] = vertexList[row[i + 0]];
				triangles[vertexIndex] = vertexIndex;
				vertexIndex++;

				vertices[vertexIndex] = vertexList[row[i + 1]];
				triangles[vertexIndex] = vertexIndex;
				vertexIndex++;

				vertices[vertexIndex] = vertexList[row[i + 2]];
				triangles[vertexIndex] = vertexIndex;
				vertexIndex++;
			}
		}

		private static Vector3[] GenerateVertexList(Voxel[] points, int edgeIndex, float isolevel) {
			for (int i = 0; i < 12; i++) {
				if ((edgeIndex & (1 << i)) != 0) {
					int[] edgePair = LookupTables.EdgeIndexTable[i];
					int edge1 = edgePair[0];
					int edge2 = edgePair[1];

					Voxel point1 = points[edge1];
					Voxel point2 = points[edge2];

					vertexList[i] = VertexInterpolate(point1.localPosition, point2.localPosition, point1.density, point2.density, isolevel);
				}
			}

			return vertexList;
		}

		private static int CalculateCubeIndex(Voxel[] points, float isolevel) {
			int cubeIndex = 0;

			for (int i = 0; i < 8; i++)
				if (points[i].density < isolevel)
					cubeIndex |= 1 << i;

			return cubeIndex;
		}

		private static Voxel[] GetPoints(int x, int y, int z, VoxelList voxels) {
			for (int i = 0; i < 8; i++) {
				Voxel p = voxels[x + CubePointsX[i], y + CubePointsY[i], z + CubePointsZ[i]];
				initPoints[i] = p;
			}

			return initPoints;
		}

		private static int[, , ] GenerateCubeIndexes(VoxelList voxels, float isolevel) {
			for (int x = 0; x < voxels.X - 1; x++) {
				for (int y = 0; y < voxels.Y - 1; y++) {
					for (int z = 0; z < voxels.Z - 1; z++) {
						initPoints = GetPoints(x, y, z, voxels);

						cubeIndexes[x, y, z] = CalculateCubeIndex(initPoints, isolevel);
					}
				}
			}

			return cubeIndexes;
		}

		private static int GenerateVertexCount(int[, , ] cubeIndexes) {
			int vertexCount = 0;

			for (int x = 0; x < cubeIndexes.GetLength(0); x++) {
				for (int y = 0; y < cubeIndexes.GetLength(1); y++) {
					for (int z = 0; z < cubeIndexes.GetLength(2); z++) {
						int cubeIndex = cubeIndexes[x, y, z];
						int[] row = LookupTables.TriangleTable[cubeIndex];
						vertexCount += row.Length;
					}
				}
			}

			return vertexCount;
		}

		public static readonly Vector3Int[] CubePoints = {
			new Vector3Int(0, 0, 0),
			new Vector3Int(1, 0, 0),
			new Vector3Int(1, 0, 1),
			new Vector3Int(0, 0, 1),
			new Vector3Int(0, 1, 0),
			new Vector3Int(1, 1, 0),
			new Vector3Int(1, 1, 1),
			new Vector3Int(0, 1, 1)
		};

		public static readonly int[] CubePointsX = {
			0,
			1,
			1,
			0,
			0,
			1,
			1,
			0
		};

		public static readonly int[] CubePointsY = {
			0,
			0,
			0,
			0,
			1,
			1,
			1,
			1
		};

		public static readonly int[] CubePointsZ = {
			0,
			0,
			1,
			1,
			0,
			0,
			1,
			1
		};
	}
}