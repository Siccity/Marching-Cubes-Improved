using System;
using UnityEngine;

namespace MarchingCubes {
	public class Terrain : MonoBehaviour {
		public Material material;

		public float isolevel;

		public bool initialized { get; private set; }
		public Chunk[, , ] chunks { get; private set; }
		public int chunkSize { get; private set; }
		public Bounds bounds { get; private set; }

		private void OnDrawGizmos() {
			if (initialized) {
				Gizmos.color = new Color(1f, 1f, 1f, 0.05f);
				EnumerateChunks(chunk => Gizmos.DrawWireCube(chunk.bounds.center, chunk.bounds.size));
				Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
				Gizmos.DrawWireCube(bounds.center, bounds.size);
			}
		}

		public void Initialize(Vector3Int size, int chunkSize = 8) {
			CreateChunks(size, chunkSize);
			initialized = true;
		}

		public void Generate(Func<Vector3Int, float> densityFunction) {
			EnumerateChunks(x => x.Set(densityFunction));
		}

		public void Union(Func<Vector3Int, float> densityFunction) {
			EnumerateChunks(x => x.Union(densityFunction));
		}

		public void Subtract(Func<Vector3Int, float> densityFunction) {
			EnumerateChunks(x => x.Subtract(densityFunction));
		}

		public void Intersection(Func<Vector3Int, float> densityFunction) {
			EnumerateChunks(x => x.Intersection(densityFunction));
		}

		private void CreateChunks(Vector3Int size, int chunkSize) {
			this.chunkSize = chunkSize;
			chunks = new Chunk[size.x, size.y, size.z];
			for (int x = 0; x < chunks.GetLength(0); x++) {
				for (int y = 0; y < chunks.GetLength(1); y++) {
					for (int z = 0; z < chunks.GetLength(2); z++) {
						Vector3Int pos = new Vector3Int(x, y, z) * chunkSize;
						Chunk chunk = CreateChunk(pos);
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
			for (int x = 0; x < chunks.GetLength(0); x++) {
				for (int y = 0; y < chunks.GetLength(1); y++) {
					for (int z = 0; z < chunks.GetLength(2); z++) {
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
			float middleX = chunks.GetLength(0) * chunkSize / 2f;
			float middleY = chunks.GetLength(1) * chunkSize / 2f;
			float middleZ = chunks.GetLength(2) * chunkSize / 2f;

			Vector3 midPos = new Vector3(middleX, middleY, middleZ);

			Vector3Int size = new Vector3Int(
				chunks.GetLength(0) * chunkSize,
				chunks.GetLength(1) * chunkSize,
				chunks.GetLength(2) * chunkSize);

			bounds = new Bounds(midPos, size);
		}

		public bool IsPointInsideWorld(int x, int y, int z) {
			return IsPointInsideWorld(new Vector3Int(x, y, z));
		}

		public bool IsPointInsideWorld(Vector3Int point) {
			return bounds.Contains(point);
		}

		private Chunk CreateChunk(Vector3Int position) {
			Chunk chunk = new GameObject("Chunk").AddComponent<Chunk>();
			chunk.meshRenderer.material = material;
			chunk.transform.position = position;
			chunk.Initialize(this, chunkSize, position);
			return chunk;
		}
	}
}