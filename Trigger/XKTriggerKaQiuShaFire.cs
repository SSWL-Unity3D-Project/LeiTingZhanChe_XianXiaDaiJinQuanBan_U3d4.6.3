using UnityEngine;
using System.Collections;

public class XKTriggerKaQiuShaFire : MonoBehaviour
{
	public XKSpawnNpcPoint SpawnNpcPoint;
	public AiPathCtrl TestPlayerPath;
	public static bool IsCloseKaQiuShaTest = true;
	public static bool IsFireKaQiuSha;
	void Start()
	{
		if (SpawnNpcPoint == null) {
			Debug.LogError("Unity:"+"SpawnNpcPoint is null");
			GameObject obj = null;
			obj.name = "null";
		}
		XkGameCtrl.GetInstance().ChangeBoxColliderSize(transform);
		CheckIsHiddenObj();
	}
	
	void CheckIsHiddenObj()
	{
		bool isHiddenObj = false;
		if (Network.peerType == NetworkPeerType.Disconnected) {
			if (XkGameCtrl.GameModeVal != GameMode.LianJi
			    && XkGameCtrl.GameJiTaiSt == GameJiTaiType.TanKeJiTai) {
				isHiddenObj = true;
			}
		}
		else {
			if (Network.peerType == NetworkPeerType.Server
			    || XkGameCtrl.GameJiTaiSt == GameJiTaiType.TanKeJiTai) {
				isHiddenObj = true;
			}
		}
		
		if (isHiddenObj) {
			gameObject.SetActive(false);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (IsCloseKaQiuShaTest) {
			return; //test;
		}
		Debug.Log("Unity:"+"XKTriggerKaQiuShaFire -> kaQiuSha fire...");
		if (JiFenJieMianCtrl.GetInstance() != null && JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
			return;
		}

		if (Network.peerType != NetworkPeerType.Disconnected) {
			if (Network.peerType == NetworkPeerType.Client) {
				//Check PlayerCameraFielfOfView
				if (XKTriggerCameraFieldOfView.Instance != null) {
					XKTriggerCameraFieldOfView.Instance.ChangeWorldTimeVal(1);
				}
				IsFireKaQiuSha = true;
				return;
			}
		}

		XkPlayerCtrl playerScript = other.GetComponent<XkPlayerCtrl>();
		if (playerScript == null) {
			return;
		}

		if (playerScript.PlayerSt == PlayerTypeEnum.FeiJi && XkGameCtrl.GameJiTaiSt == GameJiTaiType.TanKeJiTai) {
			return;
		}
		
		if (playerScript.PlayerSt == PlayerTypeEnum.TanKe && XkGameCtrl.GameJiTaiSt != GameJiTaiType.TanKeJiTai) {
			return;
		}

		GameObject npcObj = SpawnNpcPoint.GetNpcLoopObj();
		if (npcObj != null) {
			//Check PlayerCameraFielfOfView
			if (XKTriggerCameraFieldOfView.Instance != null) {
				XKTriggerCameraFieldOfView.Instance.ChangeWorldTimeVal(1);
			}
			XKCannonCtrl cannonScript = npcObj.GetComponentInChildren<XKCannonCtrl>();
			cannonScript.SetIsActiveTrigger();
			Debug.Log("Unity:"+"XKTriggerKaQiuShaFire -> Open kaQiuSha fire...");
//			if (Network.peerType == NetworkPeerType.Server) {
//				XKNpcMoveCtrl npcScript = npcObj.GetComponent<XKNpcMoveCtrl>();
//				if (npcScript != null) {
//					Debug.Log("Unity:"+"XKTriggerKaQiuShaFire -> Send open kaQiuSha fire...");
//					npcScript.OpenKaQiuShaFire();
//				}
//			}
		}
		else {
			Debug.LogWarning("Unity:"+"XKTriggerKaQiuShaFire -> KaQiuSha has been deleted");
		}
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

    void HandleServerKaQiuShaFire()
	{
		if (Network.peerType != NetworkPeerType.Client) {
			return;
		}

		if (NetCtrl.GetInstance() != null) {
			NetCtrl.GetInstance().TryActiveKaQiuShaFire();
		}
	}

	public static void SetIsFireKaQiuSha()
	{
		IsFireKaQiuSha = true;
	}
}