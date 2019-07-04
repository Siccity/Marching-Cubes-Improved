using UnityEngine;

namespace MarchingCubes {
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
	public class Chunk : MonoBehaviour {
		[HideInInspector] public bool readyForUpdate;
		[HideInInspector] public Point[, , ] points;
		[HideInInspector] public int chunkSize;
		[HideInInspector] public Vector3Int position;
		public MeshFilter meshFilter;
		public MeshRenderer meshRenderer;
		public MeshCollider meshCollider;

		private float isolevel;
		private int seed;

		private MarchingCubes marchingCubes;

		private DensityGenerator densityGenerator;

		private void Awake() {
			meshFilter = GetComponent<MeshFilter>();
			meshRenderer = GetComponent<MeshRenderer>();
			meshCollider = GetComponent<MeshCollider>();
		}

		private void Start() {
			Generate();
		}

		private void Update() {
			if (readyForUpdate) {
				Generate();
				readyForUpdate = false;
			}
		}

		public void Initialize(Terrain world, int chunkSize, Vector3Int position) {
			this.chunkSize = chunkSize;
			this.position = position;
			isolevel = world.isolevel;

			densityGenerator = world.densityGenerator;

			int worldPosX = position.x;
			int worldPosY = position.y;
			int worldPosZ = position.z;

			points = new Point[chunkSize + 1, chunkSize + 1, chunkSize + 1];

			seed = world.seed;
			marchingCubes = new MarchingCubes(points, isolevel, seed);

			for (int x = 0; x < points.GetLength(0); x++) {
				for (int y = 0; y < points.GetLength(1); y++) {
					for (int z = 0; z < points.GetLength(2); z++) {
						points[x, y, z] = new Point(
							new Vector3Int(x, y, z),
							densityGenerator.CalculateDensity(x + worldPosX, y + worldPosY, z + worldPosZ)
						);
					}
				}
			}
		}

		public void Generate() {
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