using System.Collections;
using System.Collections.Generic;
using MarchingCubes;
using MarchingCubes.Examples;
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
		public Vector3 center = Vector3.zero;

		public void OnEnable() {
			terrain = target as MarchingCubes.Terrain;
		}

		public override void OnInspectorGUI() {
			isolevel = EditorGUILayout.FloatField("Iso Level", isolevel);
			automatic = EditorGUILayout.Toggle("Automatic", automatic);
			terrainShape = GUILayout.Toolbar(terrainShape, terrainShapes);
			EditorGUI.BeginChangeCheck();
			switch (terrainShape) {
				case 0:
					noiseScale = EditorGUILayout.FloatField("Noise Scale", noiseScale);
					if (GUILayout.Button(terrain.initialized ? "Update" : "Initialize") || (EditorGUI.EndChangeCheck() && automatic)) {
						if (!terrain.initialized) terrain.Initialize(new Vector3Int(5, 5, 5), 8, isolevel);
						terrain.GenerateNoise(noiseScale);
					}
					break;
				case 1:
					center = EditorGUILayout.Vector3Field("Center", center);
					radius = EditorGUILayout.FloatField("Radius", radius);
					if (GUILayout.Button(terrain.initialized ? "Update" : "Initialize") || (EditorGUI.EndChangeCheck() && automatic)) {
						if (!terrain.initialized) terrain.Initialize(new Vector3Int(5, 5, 5), 8, isolevel);
						terrain.GenerateSphere(center, radius);
					}
					break;
			}
			if (terrain.initialized) {
				if (GUILayout.Button("Deinitialize")) terrain.Deinitialize();
			}
		}
	}
}