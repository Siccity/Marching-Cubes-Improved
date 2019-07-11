using MarchingCubes;
using UnityEditor;
using UnityEngine;

namespace MarchingCubesEditor {
	[CustomEditor(typeof(MarchingCubes.Terrain))]
	public class TerrainEditor : Editor {
		public readonly string[] terrainShapes = new string[] { "Noise", "Sphere", "Plane", "Cube" };
		public int terrainShape = 0;
		public bool automatic;
		private MarchingCubes.Terrain terrain;
		public float isolevel = 0.5f;
		public float noiseScale = 0.1f;
		public float radius = 10f;
		public float height = 10f;
		public int seed = 0;
		public Vector3 center = new Vector3(20, 20, 20);

		public void OnEnable() {
			terrain = target as MarchingCubes.Terrain;
		}

		public override void OnInspectorGUI() {
			EditorGUI.BeginChangeCheck();
			isolevel = EditorGUILayout.FloatField("Iso Level", isolevel);
			automatic = EditorGUILayout.Toggle("Automatic", automatic);
			terrainShape = GUILayout.Toolbar(terrainShape, terrainShapes);
			switch (terrainShape) {
				case 0:
					noiseScale = EditorGUILayout.FloatField("Noise Scale", noiseScale);
					seed = EditorGUILayout.IntField("Seed", seed);
					height = EditorGUILayout.FloatField("Height", height);
					center = EditorGUILayout.Vector3Field("Offset", center);
					if (GUILayout.Button(terrain.initialized ? "Update" : "Initialize") || (EditorGUI.EndChangeCheck() && automatic)) {
						if (!terrain.initialized) terrain.Initialize(new Vector3Int(5, 5, 5), 8, isolevel);
						FastNoise noise = new FastNoise(seed);
						terrain.Generate(x => GenerateNoise(x, noise));
					}
					break;
				case 1:
					center = EditorGUILayout.Vector3Field("Center", center);
					radius = EditorGUILayout.FloatField("Radius", radius);
					if (GUILayout.Button(terrain.initialized ? "Update" : "Initialize") || (EditorGUI.EndChangeCheck() && automatic)) {
						if (!terrain.initialized) terrain.Initialize(new Vector3Int(5, 5, 5), 8, isolevel);
						terrain.Generate(GenerateSphere);
					}
					break;
				case 2:
					height = EditorGUILayout.FloatField("Height", height);
					if (GUILayout.Button(terrain.initialized ? "Update" : "Initialize") || (EditorGUI.EndChangeCheck() && automatic)) {
						if (!terrain.initialized) terrain.Initialize(new Vector3Int(5, 5, 5), 8, isolevel);
						terrain.Generate(GenerateFlat);
					}
					break;
				case 3:
					center = EditorGUILayout.Vector3Field("Center", center);
					radius = EditorGUILayout.FloatField("Radius", radius);
					if (GUILayout.Button(terrain.initialized ? "Update" : "Initialize") || (EditorGUI.EndChangeCheck() && automatic)) {
						if (!terrain.initialized) terrain.Initialize(new Vector3Int(5, 5, 5), 8, isolevel);
						terrain.Generate(GenerateCube);
					}
					break;
			}
			if (terrain.initialized) {
				if (GUILayout.Button("Deinitialize")) terrain.Deinitialize();
			}
		}

		public float GenerateNoise(Vector3Int pos, FastNoise noise) {
			return (center.y - pos.y) + noise.GetPerlin((center.x - pos.x) / noiseScale, (center.z - pos.z) / noiseScale) * height;
		}

		public float GenerateSphere(Vector3Int pos) {
			return 1 - (Vector3.Distance(center, pos) / radius);
		}

		public float GenerateFlat(Vector3Int pos) {
			return height + 0.5f - pos.y;
		}

		public float GenerateCube(Vector3Int pos) {
			Vector3 local = pos - center;
			return Mathf.Min(radius - Mathf.Abs(local.x), radius - Mathf.Abs(local.y), radius - Mathf.Abs(local.z));
		}
	}
}