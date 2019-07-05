using System;
using UnityEngine;

namespace MarchingCubes {
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
	public class Chunk : MonoBehaviour {
		[HideInInspector] public bool dirty;
		[HideInInspector] public Point[, , ] points;
		[HideInInspector] public int chunkSize;
		[HideInInspector] public Vector3Int position;
		public MeshFilter meshFilter;
		public MeshRenderer meshRenderer;
		public MeshCollider meshCollider;

		private float isolevel;

		private MarchingCubes marchingCubes;
		public bool initialized { get; private set; }
		public Bounds bounds { get; private set; }

		private void Awake() {
			meshFilter = GetComponent<MeshFilter>();
			meshRenderer = GetComponent<MeshRenderer>();
			meshCollider = GetComponent<MeshCollider>();
		}

		private void Update() {
			if (!initialized) return;

			if (dirty) {
				GenerateMesh();
				dirty = false;
			}
		}

		public void Initialize(Terrain world, int chunkSize, Vector3Int position) {
			this.chunkSize = chunkSize;
			this.position = position;
			isolevel = world.isolevel;

			bounds = new Bounds(position + (Vector3.one * chunkSize * 0.5f), Vector3Int.one * chunkSize);
			points = new Point[chunkSize + 1, chunkSize + 1, chunkSize + 1];

			marchingCubes = new MarchingCubes(points, isolevel);

			// Initialize all positions with 0 density
			Set(pos => 0f);
			initialized = true;
		}

		public void Set(Func<Vector3Int, float> densityFunction) {
			points = new Point[chunkSize + 1, chunkSize + 1, chunkSize + 1];
			for (int x = 0; x < points.GetLength(0); x++) {
				for (int y = 0; y < points.GetLength(1); y++) {
					for (int z = 0; z < points.GetLength(2); z++) {
						Vector3Int pos = new Vector3Int(x, y, z);
						float density = densityFunction(pos + position);
						points[x, y, z] = new Point(pos, density);
					}
				}
			}
			dirty = true;
		}

		public void Union(Func<Vector3Int, float> densityFunction) {
			for (int x = 0; x < points.GetLength(0); x++) {
				for (int y = 0; y < points.GetLength(1); y++) {
					for (int z = 0; z < points.GetLength(2); z++) {
						Vector3Int pos = new Vector3Int(x, y, z);
						points[x, y, z].density = Mathf.Max(points[x, y, z].density, densityFunction(pos));
					}
				}
			}
			dirty = true;
		}

		public void Subtract(Func<Vector3Int, float> densityFunction) {
			for (int x = 0; x < points.GetLength(0); x++) {
				for (int y = 0; y < points.GetLength(1); y++) {
					for (int z = 0; z < points.GetLength(2); z++) {
						Vector3Int pos = new Vector3Int(x, y, z);
						points[x, y, z].density = Mathf.Clamp01(points[x, y, z].density - densityFunction(pos));
					}
				}
			}
			dirty = true;
		}

		public void Intersection(Func<Vector3Int, float> densityFunction) {
			for (int x = 0; x < points.GetLength(0); x++) {
				for (int y = 0; y < points.GetLength(1); y++) {
					for (int z = 0; z < points.GetLength(2); z++) {
						Vector3Int pos = new Vector3Int(x, y, z);
						points[x, y, z].density = Mathf.Min(points[x, y, z].density, densityFunction(pos));
					}
				}
			}
			dirty = true;
		}

		public void GenerateMesh() {
			Mesh mesh = marchingCubes.CreateMeshData(points);
			meshFilter.sharedMesh = mesh;
			meshCollider.sharedMesh = mesh;
		}

		public Point GetPoint(int x, int y, int z) {
			return points[x, y, z];
		}

		public void SetDensity(float density, int x, int y, int z) {
			points[x, y, z].density = density;
		}

		public void SetDensity(float density, Vector3Int pos) {
			SetDensity(density, pos.x, pos.y, pos.z);
		}
	}
}