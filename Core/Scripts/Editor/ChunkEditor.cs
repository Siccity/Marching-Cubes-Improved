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
				for (int x = 0; x < chunk.voxels.X; x++) {
					for (int y = 0; y < chunk.voxels.Y; y++) {
						for (int z = 0; z < chunk.voxels.Z; z++) {
							Vector3 pos = chunk.voxels[x, y, z].localPosition + chunk.transform.position;
							float size = Mathf.Clamp01(chunk.voxels[x, y, z].density) * 0.2f;
							if (chunk.voxels[x, y, z].density > 0) Handles.SphereHandleCap(-1, pos, Quaternion.identity, size, EventType.Repaint);
						}
					}
				}
			}
		}
	}
}