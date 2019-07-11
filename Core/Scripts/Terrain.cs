using System;
using System.Collections.Generic;
using UnityEngine;

namespace MarchingCubes {
	public class Terrain : MonoBehaviour {
		public Material material;

		[SerializeField] public ChunkList chunks;

		public bool initialized { get; private set; }
		public int chunkSize { get; private set; }
		public Bounds bounds { get; private set; }

		/// <summary> A 3-dimensional jagged array </summary>
		[Serializable] public class ChunkList : List3D<Chunk> { public ChunkList(int x, int y, int z) : base(x, y, z, null) { } }

		private void OnDrawGizmos() {
			if (initialized) {
				Gizmos.color = new Color(1f, 1f, 1f, 0.05f);
				EnumerateChunks(chunk => Gizmos.DrawWireCube(chunk.bounds.center, chunk.bounds.size));
				Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
				Gizmos.DrawWireCube(bounds.center, bounds.size);
			}
		}

		public void Initialize(Vector3Int size, int chunkSize = 8, float isolevel = 0.5f) {
			if (initialized) {
				Debug.LogWarning("Terrain already initialized");
				return;
			}
			CreateChunks(size, chunkSize, isolevel);
			initialized = true;
		}

		[ContextMenu("Deinitialize")]
		public void Deinitialize() {
			if (!initialized) {
				Debug.LogWarning("Terrain not initialized");
				return;
			}
			if (chunks != null) {
				EnumerateChunks(x => DestroyImmediate(x.gameObject));
				chunks = null;
			}
			initialized = false;
		}

		public void Generate(Func<Vector3Int, float> densityFunction, bool updateMesh = true) {
			EnumerateChunks(x => { x.Set(densityFunction); if (updateMesh) x.UpdateMesh(); });
		}

		public void Union(Func<Vector3Int, float> densityFunction, bool updateMesh = true) {
			EnumerateChunks(x => { x.Union(densityFunction); if (updateMesh) x.UpdateMesh(); });
		}

		public void Subtract(Func<Vector3Int, float> densityFunction, bool updateMesh = true) {
			EnumerateChunks(x => { x.Subtract(densityFunction); if (updateMesh) x.UpdateMesh(); });
		}

		public void Intersection(Func<Vector3Int, float> densityFunction, bool updateMesh = true) {
			EnumerateChunks(x => { x.Intersection(densityFunction); if (updateMesh) x.UpdateMesh(); });
		}

		private void CreateChunks(Vector3Int size, int chunkSize, float isolevel) {
			Debug.Log("Create chunks");
			this.chunkSize = chunkSize;
			chunks = new ChunkList(size.x, size.y, size.z);
			for (int x = 0; x < chunks.X; x++) {
				for (int y = 0; y < chunks.Y; y++) {
					for (int z = 0; z < chunks.Z; z++) {
						Vector3Int pos = new Vector3Int(x, y, z) * chunkSize;
						Chunk chunk = CreateChunk(pos, isolevel);
						chunks[x, y, z] = chunk;
					}
				}
			}
			UpdateBounds();
		}

		private void EnumerateChunks(Action<Chunk> onChunk) {
			if (!initialized) {
				Debug.LogError("Terrain not initialized!");
				return;
			}
			for (int x = 0; x < chunks.X; x++) {
				for (int y = 0; y < chunks.Y; y++) {
					for (int z = 0; z < chunks.Z; z++) {
						onChunk(chunks[x, y, z]);
					}
				}
			}
		}

		private Chunk GetChunk(Vector3Int pos) {
			return GetChunk(pos.x, pos.y, pos.z);
		}

		public Chunk GetChunk(int x, int y, int z) {
			x /= chunkSize;
			y /= chunkSize;
			z /= chunkSize;
			return chunks[x, y, z];
		}

		public float GetDensity(int x, int y, int z) {
			Point p = GetPoint(x, y, z);

			return p.density;
		}

		public float GetDensity(Vector3Int pos) {
			return GetDensity(pos.x, pos.y, pos.z);
		}

		public Point GetPoint(int x, int y, int z) {
			Chunk chunk = GetChunk(x, y, z);

			Point p = chunk.GetPoint(x.Mod(chunkSize),
				y.Mod(chunkSize),
				z.Mod(chunkSize));

			return p;
		}

		public void SetDensity(float density, int worldPosX, int worldPosY, int worldPosZ, bool setReadyForUpdate, Chunk[] initChunks) {
			Vector3Int dp = new Vector3Int(worldPosX, worldPosY, worldPosZ);

			Vector3Int lastChunkPos = dp.FloorToNearestX(chunkSize);

			for (int i = 0; i < 8; i++) {
				Vector3Int chunkPos = (dp - MarchingCubes.CubePoints[i]).FloorToNearestX(chunkSize);

				if (i != 0 && chunkPos == lastChunkPos) {
					continue;
				}

				Chunk chunk = GetChunk(chunkPos);

				lastChunkPos = chunk.position;

				Vector3Int localPos = (dp - chunk.position).Mod(chunkSize + 1);

				chunk.SetDensity(density, localPos);
				if (setReadyForUpdate)
					chunk.dirty = true;
			}
		}

		public void SetDensity(float density, Vector3Int pos, bool setReadyForUpdate, Chunk[] initChunks) {
			SetDensity(density, pos.x, pos.y, pos.z, setReadyForUpdate, initChunks);
		}

		private void UpdateBounds() {
			float middleX = chunks.X * chunkSize / 2f;
			float middleY = chunks.Y * chunkSize / 2f;
			float middleZ = chunks.Z * chunkSize / 2f;

			Vector3 midPos = new Vector3(middleX, middleY, middleZ);

			Vector3Int size = new Vector3Int(
				chunks.X * chunkSize,
				chunks.Y * chunkSize,
				chunks.Z * chunkSize);

			bounds = new Bounds(midPos, size);
		}

		public bool IsPointInsideWorld(int x, int y, int z) {
			return IsPointInsideWorld(new Vector3Int(x, y, z));
		}

		public bool IsPointInsideWorld(Vector3Int point) {
			return bounds.Contains(point);
		}

		private Chunk CreateChunk(Vector3Int position, float isolevel) {
			Chunk chunk = new GameObject("Chunk (" + position.x + ", " + position.y + ", " + position.z + ")").AddComponent<Chunk>();
			chunk.transform.parent = transform;
			chunk.meshRenderer.material = material;
			chunk.transform.localPosition = position;
			chunk.Initialize(chunkSize, position, isolevel);
			return chunk;
		}
	}
}