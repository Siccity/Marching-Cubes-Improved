using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MarchingCubes {
	/// <summary> Serializable 3-dimensional list backed by a jagged list </summary>
	public class List3D<T> : ISerializationCallbackReceiver {
		public T this [int x, int y, int z] {
			get { return list[GetIndex(x, y, z)]; }
			set { list[GetIndex(x, y, z)] = value; }
		}

		/// <summary> 3-dimensional jagged array </summary>
		[SerializeField] protected List<T> list;
		/// <summary> Length in the x dimension </summary>
		[SerializeField] protected int x;
		/// <summary> Length in the y dimension </summary>
		[SerializeField] protected int y;
		/// <summary> Length in the z dimension </summary>
		[SerializeField] protected int z;

		/// <summary> Length in the x dimension </summary>
		public int X { get { return x; } }
		/// <summary> Length in the y dimension </summary>
		public int Y { get { return y; } }
		/// <summary> Length in the z dimension </summary>
		public int Z { get { return z; } }

		/// <summary> x * y cached </summary>
		private int xy;

		public List3D(int x, int y, int z) : this(x, y, z, default(T)) { }

		public List3D(int x, int y, int z, T value) {
			this.x = x;
			this.y = y;
			this.z = z;
			xy = x * y;
			int items = x * y * z;
			list = new List<T>(x * y * z);
			for (int i = 0; i < items; i++) {
				list.Add(value);
			}
		}

		public void Foreach(Action<int, int, int, T> action) {
			int count = 0;
			for (int z = 0; z < this.z; z++) {
				for (int y = 0; y < this.y; y++) {
					for (int x = 0; x < this.x; x++) {
						action(x, y, z, list[count++]);
					}
				}
			}
		}

		private int GetIndex(int x, int y, int z) {
			return x + (this.x * y) + (xy * z);
		}

		public void InsertX(int index, T value) {
			if (index < 0 || index > x) throw new IndexOutOfRangeException();
			x++;
			for (int z = 0; z < this.z; z++) {
				for (int y = 0; y < this.y; y++) {
					int i = GetIndex(index, y, z);
					list.Insert(i, value);
				}
			}
			xy = x * y;
		}

		public void InsertY(int index, T value) {
			if (index < 0 || index > y) throw new IndexOutOfRangeException();
			y++;
			for (int z = 0; z < this.z; z++) {
				for (int x = 0; x < this.x; x++) {
					int i = GetIndex(x, index, z);
					list.Insert(i, value);
				}
			}
			xy = x * y;
		}

		public void InsertZ(int index, T value) {
			if (index < 0 || index > z) throw new IndexOutOfRangeException();
			z++;
			for (int y = 0; y < this.y; y++) {
				for (int x = 0; x < this.x; x++) {
					int i = GetIndex(x, y, index);
					list.Insert(i, value);
				}
			}
		}

		public void OnBeforeSerialize() { }

		public void OnAfterDeserialize() {
			xy = x * y;
		}
	}
}