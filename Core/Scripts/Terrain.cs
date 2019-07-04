using System;
using System.Collections.Generic;
using UnityEngine;

namespace MarchingCubes {
	public class Terrain : MonoBehaviour {
		public Material material;
		public int chunkSize = 8;

		public int worldWidth = 5;
		public int worldHeight = 5;
		public int worldDepth = 5;

		public float isolevel;
		public int seed;

		public Dictionary<Vector3Int, Chunk> chunks;

		private Bounds worldBounds;

		private void OnDrawGizmos() {
			Gizmos.DrawWireCube(worldBounds.center, worldBounds.size);
		}

		public void Generate(Func<Vector3Int, float> densityFunction) {
			ValidateChunks();
			// todo
		}

		public void Union(Func<Vector3Int, float> densityFunction) {
			ValidateChunks();
			// todo
		}

		public void Subtract(Func<Vector3Int, float> densityFunction) {
			ValidateChunks();
			// todo
		}

		public void Intersection(Func<Vector3Int, float> densityFunction) {
			ValidateChunks();
			// todo
		}

		private void ValidateChunks() {
			if (chunks == null) {
				CreateChunks();
			}
		}

		private void Start() {
			worldBounds = new Bounds();
			UpdateBounds();

			chunks = new Dictionary<Vector3Int, Chunk>(worldWidth * worldHeight * worldDepth);
			CreateChunks();
		}

		private void CreateChunks() {
			chunks = new Dictionary<Vector3Int, Chunk>(worldWidth * worldHeight * worldDepth);
			Vector3Int pos = new Vector3Int(0, 0, 0);
			for (pos.x = 0; pos.x < worldWidth; pos.x++) {
				for (pos.y = 0; pos.y < worldHeight; pos.y++) {
					for (pos.z = 0; pos.z < worldDepth; pos.z++) {
						CreateChunk(pos * chunkSize);
					}
				}
			}
		}

		private Chunk GetChunk(Vector3Int pos) {
			return GetChunk(pos.x, pos.y, pos.z);
		}

		public Chunk GetChunk(int x, int y, int z) {
			int newX = Utils.FloorToNearestX(x, chunkSize);
			int newY = Utils.FloorToNearestX(y, chunkSize);
			int newZ = Utils.FloorToNearestX(z, chunkSize);

			return chunks[new Vector3Int(newX, newY, newZ)];
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
			float middleX = worldWidth * chunkSize / 2f;
			float middleY = worldHeight * chunkSize / 2f;
			float middleZ = worldDepth * chunkSize / 2f;

			Vector3 midPos = new Vector3(middleX, middleY, middleZ);

			Vector3Int size = new Vector3Int(
				worldWidth * chunkSize,
				worldHeight * chunkSize,
				worldDepth * chunkSize);

			worldBounds.center = midPos;
			worldBounds.size = size;
		}

		public bool IsPointInsideWorld(int x, int y, int z) {
			return IsPointInsideWorld(new Vector3Int(x, y, z));
		}

		public bool IsPointInsideWorld(Vector3Int point) {
			return worldBounds.Contains(point);
		}

		private void CreateChunk(Vector3Int position) {
			Chunk chunk = new GameObject("Chunk").AddComponent<Chunk>();
			chunk.meshRenderer.material = material;
			chunk.transform.position = position;
			chunk.Initialize(this, chunkSize, position);
			chunks.Add(position, chunk);
		}
	}
}