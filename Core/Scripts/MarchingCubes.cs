using UnityEngine;

namespace MarchingCubes {
	public static class MarchingCubes {
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
		public static readonly int[] CubePointsX = { 0, 1, 1, 0, 0, 1, 1, 0 };
		public static readonly int[] CubePointsY = { 0, 0, 0, 0, 1, 1, 1, 1 };
		public static readonly int[] CubePointsZ = { 0, 0, 1, 1, 0, 0, 1, 1 };

		private static Vector3[] vertexList = new Vector3[12];
		private static int[, , ] cubeIndexes;

		public static Mesh CreateMeshData(VoxelGrid voxels, float isolevel) {
			cubeIndexes = new int[voxels.X - 1, voxels.Y - 1, voxels.Z - 1];
			cubeIndexes = GenerateCubeIndexes(voxels, isolevel);
			int vertexCount = GenerateVertexCount(cubeIndexes);

			if (vertexCount <= 0) {
				return new Mesh();
			}

			Vector3[] vertices = new Vector3[vertexCount];
			int[] triangles = new int[vertexCount];

			int vertexIndex = 0;

			for (int x = 0; x < voxels.X - 1; x++) {
				for (int y = 0; y < voxels.Y - 1; y++) {
					for (int z = 0; z < voxels.Z - 1; z++) {
						int cubeIndex = cubeIndexes[x, y, z];
						if (cubeIndex == 0 || cubeIndex == 255) continue;

						March(voxels.GetCorners(x, y, z), vertices, triangles, ref vertexIndex, cubeIndex, isolevel);
					}
				}
			}

			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.SetTriangles(triangles, 0);
			mesh.RecalculateNormals();

			return mesh;
		}

		private static int[, , ] GenerateCubeIndexes(VoxelGrid voxels, float isolevel) {
			for (int x = 0; x < voxels.X - 1; x++) {
				for (int y = 0; y < voxels.Y - 1; y++) {
					for (int z = 0; z < voxels.Z - 1; z++) {
						VoxelCorners corners = voxels.GetCorners(x,y,z);

						cubeIndexes[x, y, z] = corners.GetCubeIndex(isolevel);
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

		private static void March(VoxelCorners corners, Vector3[] vertices, int[] triangles, ref int vertexIndex, int cubeIndex, float isolevel) {
			int edgeIndex = LookupTables.EdgeTable[cubeIndex];

			vertexList = GenerateVertexList(corners, edgeIndex, isolevel);

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

		private static Vector3[] GenerateVertexList(VoxelCorners corners, int cornerIndex, float isolevel) {
			for (int i = 0; i < 12; i++) {
				if ((cornerIndex & (1 << i)) != 0) {
					LookupTables.Edge edge = LookupTables.EdgeIndexTable[i];
					int corner1 = edge.a;
					int corner2 = edge.b;

					Voxel point1 = corners[corner1];
					Voxel point2 = corners[corner2];

					vertexList[i] = VertexInterpolate(point1.localPosition, point2.localPosition, point1.density, point2.density, isolevel);
				}
			}
			return vertexList;
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
	}
}