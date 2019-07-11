using UnityEngine;

namespace MarchingCubes {
	public class TerrainPainter : MonoBehaviour {
		[SerializeField] private float force = 2f;
		[SerializeField] private float range = 2f;

		[SerializeField] private float maxReachDistance = 100f;

		[SerializeField] private AnimationCurve forceOverDistance = AnimationCurve.Constant(0, 1, 1);

		[SerializeField] private Transform playerCamera;

		private void Start() {
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void Update() {
			TryEditTerrain();
		}

		private void TryEditTerrain() {
			if (force <= 0 || range <= 0) {
				return;
			}

			if (Input.GetButton("Fire1")) {
				RaycastToTerrain(true);
			} else if (Input.GetButton("Fire2")) {
				RaycastToTerrain(false);
			}
		}

		private void RaycastToTerrain(bool addTerrain) {
			Vector3 startP = playerCamera.position;
			Vector3 destP = startP + playerCamera.forward;
			Vector3 direction = destP - startP;

			Ray ray = new Ray(startP, direction);

			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, maxReachDistance)) {
				Terrain terrain = hit.collider.transform.GetComponentInParent<Terrain>();
				if (terrain != null) EditTerrain(terrain, hit.point, addTerrain, force, range);
			}
		}

		private void EditTerrain(Terrain terrain, Vector3 point, bool addTerrain, float force, float range) {
			terrain.Subtract(x => 1 - (Vector3.Distance(point, x) / range) * force * Time.deltaTime);
		}
	}
}