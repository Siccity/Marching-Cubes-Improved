using System.Collections;
using System.Collections.Generic;
using MarchingCubes;
using UnityEditor;
using UnityEngine;

namespace MarchingCubesEditor {
	[CustomEditor(typeof(MarchingCubes.Chunk))]
	public class ChunkEditor : Editor {
		private Chunk chunk;

		private void OnEnable() {
			chunk = target as Chunk;
		}

		private void OnSceneGUI() {
			if (chunk.initialized) {
				for (int x = 0; x < chunk.points.GetLength(0); x++) {
					for (int y = 0; y < chunk.points.GetLength(1); y++) {
						for (int z = 0; z < chunk.points.GetLength(2); z++) {
							Vector3 pos = chunk.points[x, y, z].localPosition + chunk.transform.position;
							float size = Mathf.Clamp01(chunk.points[x, y, z].density) * 0.2f;
							if (chunk.points[x, y, z].density > 0) Handles.SphereHandleCap(-1, pos, Quaternion.identity, size, EventType.Repaint);
						}
					}
				}
			}
		}
	}
}