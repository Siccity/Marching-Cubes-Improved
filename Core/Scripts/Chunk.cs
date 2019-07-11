﻿using System;
using UnityEngine;

namespace MarchingCubes {
	[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
	public class Chunk : MonoBehaviour {
		[HideInInspector] public bool dirty;
		public VoxelList voxels;
		[HideInInspector] public int chunkSize;
		[HideInInspector] public Vector3Int position;
		public MeshFilter meshFilter { get { return _meshFilter != null ? _meshFilter : _meshFilter = GetComponent<MeshFilter>(); } }
		private MeshFilter _meshFilter;
		public MeshRenderer meshRenderer { get { return _meshRenderer != null ? _meshRenderer : _meshRenderer = GetComponent<MeshRenderer>(); } }
		private MeshRenderer _meshRenderer;
		public MeshCollider meshCollider { get { return _meshCollider != null ? _meshCollider : _meshCollider = GetComponent<MeshCollider>(); } }
		private MeshCollider _meshCollider;

		private float isolevel;

		private MarchingCubes marchingCubes;
		public bool initialized { get; private set; }
		public Bounds bounds { get; private set; }

		private void Update() {
			if (!initialized) return;

			if (dirty) UpdateMesh();
		}

		public void Initialize(int chunkSize, Vector3Int position, float isolevel) {
			this.chunkSize = chunkSize;
			this.position = position;
			this.isolevel = isolevel;

			bounds = new Bounds(position + (Vector3.one * chunkSize * 0.5f), Vector3Int.one * chunkSize);
			voxels = new VoxelList(chunkSize + 1, chunkSize + 1, chunkSize + 1);

			marchingCubes = new MarchingCubes(voxels, isolevel);

			// Initialize all positions with 0 density
			Set(pos => 0f);
			initialized = true;
		}

		public void Set(Func<Vector3Int, float> densityFunction) {
			voxels = new VoxelList(chunkSize + 1, chunkSize + 1, chunkSize + 1);
			for (int x = 0; x < voxels.X; x++) {
				for (int y = 0; y < voxels.Y; y++) {
					for (int z = 0; z < voxels.Z; z++) {
						Vector3Int pos = new Vector3Int(x, y, z);
						float density = densityFunction(pos + position);
						voxels[x, y, z] = new Voxel(pos, density);
					}
				}
			}
			dirty = true;
		}

		public void Union(Func<Vector3Int, float> densityFunction) {
			for (int x = 0; x < voxels.X; x++) {
				for (int y = 0; y < voxels.Y; y++) {
					for (int z = 0; z < voxels.Z; z++) {
						Vector3Int pos = new Vector3Int(x, y, z);
						Voxel voxel = voxels[x, y, z];
						voxel.density = Mathf.Max(voxel.density, densityFunction(pos));
						voxels[x, y, z] = voxel;
					}
				}
			}
			dirty = true;
		}

		public void Subtract(Func<Vector3Int, float> densityFunction) {
			for (int x = 0; x < voxels.X; x++) {
				for (int y = 0; y < voxels.Y; y++) {
					for (int z = 0; z < voxels.Z; z++) {
						Vector3Int pos = new Vector3Int(x, y, z);
						Voxel voxel = voxels[x, y, z];
						voxel.density = Mathf.Clamp01(voxel.density - densityFunction(pos));
						voxels[x, y, z] = voxel;
					}
				}
			}
			dirty = true;
		}

		public void Intersection(Func<Vector3Int, float> densityFunction) {
			for (int x = 0; x < voxels.X; x++) {
				for (int y = 0; y < voxels.Y; y++) {
					for (int z = 0; z < voxels.Z; z++) {
						Vector3Int pos = new Vector3Int(x, y, z);
						Voxel voxel = voxels[x, y, z];
						voxel.density = Mathf.Min(voxel.density, densityFunction(pos));
						voxels[x, y, z] = voxel;
					}
				}
			}
			dirty = true;
		}

		public void UpdateMesh() {
			Mesh mesh = marchingCubes.CreateMeshData(voxels);
			meshFilter.sharedMesh = mesh;
			meshCollider.sharedMesh = mesh;
			dirty = false;
		}

		public Voxel GetPoint(int x, int y, int z) {
			return voxels[x, y, z];
		}

		public void SetDensity(float density, int x, int y, int z) {
			Voxel voxel = voxels[x, y, z];
			voxel.density = density;
			voxels[x, y, z] = voxel;
		}

		public void SetDensity(float density, Vector3Int pos) {
			SetDensity(density, pos.x, pos.y, pos.z);
		}
	}
}