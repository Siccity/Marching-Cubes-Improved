using UnityEngine;

namespace MarchingCubes.Examples {
	public static class TerrainGeneratorExtensions {
		public static void GenerateNoise(this Terrain terrain, float noiseScale, int seed = 0) {
			FastNoise noise = new FastNoise(seed);
			terrain.Generate(pos => pos.y - noise.GetPerlin(pos.x / noiseScale, pos.z / noiseScale).Map(-1, 1, 0, 1) * 10 - 10);
		}

		public static void GenerateSphere(this Terrain terrain, Vector3 center, float radius) {
			terrain.Generate(pos => Vector3.Distance(center, pos) / radius);
		}

		public static void GenerateFlat(this Terrain terrain, float height) {
			terrain.Generate(pos => pos.y - height + 0.5f);
		}
	}
}