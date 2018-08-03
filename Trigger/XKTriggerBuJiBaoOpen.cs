using UnityEngine;
using System.Collections;

public class XKTriggerBuJiBaoOpen : MonoBehaviour {
	public bool IsLoopSpawn;
	public GameObject[] BuJiBao;
	public Transform[] BuJiBaoPoint;
	[Range(0.1f, 10f)] public float[] TimeBuJi = {1f};
	int CountBJ;
	bool IsActiveTrigger;
	public AiPathCtrl TestPlayerPath;
	void Start()
	{
		XkGameCtrl.GetInstance().ChangeBoxColliderSize(transform);
	}

	void OnTriggerEnter(Collider other)
	{
		if (Network.peerType != NetworkPeerType.Disconnected) {
			if (Network.peerType == NetworkPeerType.Client) {
				return;
			}
		}

		if (other.GetComponent<XkPlayerCtrl>() == null) {
			return;
		}

//		if (IsActiveTrigger) {
//			return;
//		}
//		IsActiveTrigger = true;
		StartCoroutine(SpawnBuJiBaoToPlayer());
	}

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
	{
		if (!XkGameCtrl.IsDrawGizmosObj) {
			return;
		}

		if (!enabled) {
			return;
		}
		
		if (TestPlayerPath != null) {
			TestPlayerPath.DrawPath();
		}
	}
#endif

    IEnumerator SpawnBuJiBaoToPlayer()
	{
		do {
			if (CountBJ >= BuJiBao.Length) {
				CountBJ = 0;
				if (!IsLoopSpawn) {
					break;
				}
			}

			if (BuJiBao[CountBJ] != null && BuJiBaoPoint[CountBJ] != null) {
				//Spawn BuJiBao.
				SpawnPointDaoJu(BuJiBao[CountBJ],
				                BuJiBaoPoint[CountBJ].position,
				                BuJiBaoPoint[CountBJ].rotation);
				yield return new WaitForSeconds(TimeBuJi[CountBJ]);
			}
			CountBJ++;
		} while (true);
	}

	void SpawnPointDaoJu(GameObject objPrefab, Vector3 pos, Quaternion rot)
	{
		GameObject obj = null;
		if (Network.peerType == NetworkPeerType.Disconnected) {
			obj = (GameObject)Instantiate(objPrefab, pos, rot);
		}
		else {
			int playerID = int.Parse(Network.player.ToString());
			obj = (GameObject)Network.Instantiate(objPrefab, pos, rot, playerID);
			if (NetworkServerNet.GetInstance() != null) {
				NetworkServerNet.GetInstance().AddNpcObjList(obj);
			}
		}
		obj.transform.parent = XkGameCtrl.PlayerAmmoArray;
	}

	public bool CloseSpawnBuJiBaoToPlayer()
	{
//		if (!IsActiveTrigger) {
//			return false;
//		}
		StopCoroutine(SpawnBuJiBaoToPlayer());
//		gameObject.SetActive(false);
		return true;
	}
}