using UnityEngine;

namespace MarchingCubes {
	public class TerrainGenerator : MonoBehaviour {
		public Terrain terrain;
		
		private void Reset() {
			terrain = GetComponent<Terrain>();
		}
		
		private void Start() {
			terrain.Initialize(new Vector3Int(5,5,5), 8);
			terrain.GenerateNoise(0.1f);
		}

		public void GenerateSphere() {
			terrain.GenerateSphere(10);
		}
	}

	public static class TerrainGeneratorExtensions {
		public static void GenerateNoise(this Terrain terrain, float noiseScale, int seed = 0) {
			FastNoise noise = new FastNoise(seed);
			terrain.Generate(pos => pos.y - noise.GetPerlin(pos.x / noiseScale, pos.z / noiseScale).Map(-1, 1, 0, 1) * 10 - 10);
		}

		public static void GenerateSphere(this Terrain terrain, int radius) {
			terrain.Generate(pos => pos.x * pos.x + pos.y * pos.y + pos.z * pos.z - radius * radius);
		}

		public static void GenerateFlat(this Terrain terrain, float height) {
			terrain.Generate(pos => pos.y - height + 0.5f);
		}
	}
}