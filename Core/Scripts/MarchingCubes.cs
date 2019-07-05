using UnityEngine;

namespace MarchingCubes {
	public class MarchingCubes {
		private Vector3[] vertices;
		private int[] triangles;
		private float isolevel;

		private int vertexIndex;

		private Vector3[] vertexList;
		private Point[] initPoints;
		private Mesh mesh;
		private int[, , ] cubeIndexes;

		private readonly Vector3 zero = Vector3.zero;

		public MarchingCubes(Point[, , ] points, float isolevel) {
			this.isolevel = isolevel;

			mesh = new Mesh();

			vertexIndex = 0;

			vertexList = new Vector3[12];
			initPoints = new Point[8];
			cubeIndexes = new int[points.GetLength(0) - 1, points.GetLength(1) - 1, points.GetLength(2) - 1];
		}

		private Vector3 VertexInterpolate(Vector3 p1, Vector3 p2, float v1, float v2) {
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

		private void March(Point[] points, int cubeIndex) {
			int edgeIndex = LookupTables.EdgeTable[cubeIndex];

			vertexList = GenerateVertexList(points, edgeIndex);

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

		private Vector3[] GenerateVertexList(Point[] points, int edgeIndex) {
			for (int i = 0; i < 12; i++) {
				if ((edgeIndex & (1 << i)) != 0) {
					int[] edgePair = LookupTables.EdgeIndexTable[i];
					int edge1 = edgePair[0];
					int edge2 = edgePair[1];

					Point point1 = points[edge1];
					Point point2 = points[edge2];

					vertexList[i] = VertexInterpolate(point1.localPosition, point2.localPosition, point1.density, point2.density);
				}
			}

			return vertexList;
		}

		private int CalculateCubeIndex(Point[] points, float iso) {
			int cubeIndex = 0;

			for (int i = 0; i < 8; i++)
				if (points[i].density < iso)
					cubeIndex |= 1 << i;

			return cubeIndex;
		}

		public Mesh CreateMeshData(Point[, , ] points) {
			cubeIndexes = GenerateCubeIndexes(points);
			int vertexCount = GenerateVertexCount(cubeIndexes);

			if (vertexCount <= 0) {
				return new Mesh();
			}

			vertices = new Vector3[vertexCount];
			triangles = new int[vertexCount];

			for (int x = 0; x < points.GetLength(0) - 1; x++) {
				for (int y = 0; y < points.GetLength(1) - 1; y++) {
					for (int z = 0; z < points.GetLength(2) - 1; z++) {
						int cubeIndex = cubeIndexes[x, y, z];
						if (cubeIndex == 0 || cubeIndex == 255) continue;

						March(GetPoints(x, y, z, points), cubeIndex);
					}
				}
			}

			vertexIndex = 0;

			mesh.Clear();

			mesh.vertices = vertices;
			mesh.SetTriangles(triangles, 0);
			mesh.RecalculateNormals();

			return mesh;
		}

		private Point[] GetPoints(int x, int y, int z, Point[, , ] points) {
			for (int i = 0; i < 8; i++) {
				Point p = points[x + CubePointsX[i], y + CubePointsY[i], z + CubePointsZ[i]];
				initPoints[i] = p;
			}

			return initPoints;
		}

		private int[, , ] GenerateCubeIndexes(Point[, , ] points) {
			for (int x = 0; x < points.GetLength(0) - 1; x++) {
				for (int y = 0; y < points.GetLength(1) - 1; y++) {
					for (int z = 0; z < points.GetLength(2) - 1; z++) {
						initPoints = GetPoints(x, y, z, points);

						cubeIndexes[x, y, z] = CalculateCubeIndex(initPoints, isolevel);
					}
				}
			}

			return cubeIndexes;
		}

		private int GenerateVertexCount(int[, , ] cubeIndexes) {
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