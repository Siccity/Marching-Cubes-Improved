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
				chunk.voxels.Foreach((x, y, z, voxel) => {
					Vector3 pos = voxel.localPosition + chunk.transform.position;
					float size = Mathf.Clamp01(voxel.density) * 0.2f;
					Handles.color = size > 1 ? Color.red : Color.white;
					if (voxel.density > 0) Handles.SphereHandleCap(-1, pos, Quaternion.identity, size, EventType.Repaint);
				});
			}
			MarchingCubes.Terrain terrain = chunk.GetComponentInParent<MarchingCubes.Terrain>();
			if (terrain) {
				Vector3 center = chunk.transform.position;
				Vector3 size = Vector3.one * terrain.ChunkSize;
				Handles.DrawWireCube(center + (size * 0.5f), size);
			}
		}
	}
}