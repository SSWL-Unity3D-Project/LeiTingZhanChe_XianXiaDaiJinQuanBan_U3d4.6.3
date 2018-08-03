using UnityEngine;

public class TestSpawnPoint : MonoBehaviour
{
	[Range(-1f, 1000f)]public float FireRadius = 10f;
	public LayerMask TerrainLayer;
	public bool IsMakeObjToTerrain;
	void OnDrawGizmos()
	{
		if (!enabled) {
			return;
		}
		
		if (FireRadius > 0f) {
			Gizmos.color = new Color(0.5f, 0.9f, 1.0f, 0.15f);
			Gizmos.DrawSphere(transform.position, FireRadius);
		}
	}

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
	{
		if (!enabled) {
			return;
		}
		
		if (FireRadius > 0f) {
			Gizmos.color = new Color(0.5f, 0.9f, 1.0f, 0.3f);
			Gizmos.DrawSphere(transform.position, FireRadius);
			
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(transform.position, FireRadius);
		}
		MakeObjMoveToLand();
	}
#endif

    void MakeObjMoveToLand()
	{
		if (!IsMakeObjToTerrain) {
			return;
		}
		
		RaycastHit hitInfo;
		Vector3 startPos = transform.position;
		Vector3 forwardVal = Vector3.down;
		if (Physics.Raycast(startPos, forwardVal, out hitInfo, 10f, TerrainLayer.value)){
			transform.position = hitInfo.point;
		}
	}
}