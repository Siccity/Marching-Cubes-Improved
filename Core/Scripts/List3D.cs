using System;
using System.Collections.Generic;
using UnityEngine;

namespace MarchingCubes {
	/// <summary> Serializable 3-dimensional list backed by a jagged list </summary>
	public class List3D<T> {
		public T this [int x, int y, int z] {
			get { return list[GetIndex(x, y, z)]; }
			set { list[GetIndex(x, y, z)] = value; }
		}

		/// <summary> 3-dimensional jagged array </summary>
		[SerializeField] private List<T> list;
		/// <summary> Length in the x dimension </summary>
		[SerializeField] private int x;
		/// <summary> Length in the y dimension </summary>
		[SerializeField] private int y;
		/// <summary> Length in the z dimension </summary>
		[SerializeField] private int z;

		/// <summary> Length in the x dimension </summary>
		public int X { get { return x; } }
		/// <summary> Length in the y dimension </summary>
		public int Y { get { return y; } }
		/// <summary> Length in the z dimension </summary>
		public int Z { get { return z; } }

		public List3D(int x, int y, int z, T value) {
			this.x = x;
			this.y = y;
			this.z = z;
			int items = x * y * z;
			list = new List<T>(x * y * z);
			for (int i = 0; i < items; i++) {
				list.Add(value);
			}
		}

		private int GetIndex(int x, int y, int z) {
			return x + (this.x * y) + (this.x * this.y * z);
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
	}
}