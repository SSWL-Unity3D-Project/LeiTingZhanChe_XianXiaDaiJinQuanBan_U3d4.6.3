using UnityEngine;
using System.Collections;

public class XKTriggerEndCartoon : MonoBehaviour {
	public XKTriggerSpawnNpc[] SpawnArray;
	public XKTriggerRemoveNpc[] RemoveArray;
	public AiPathCtrl TestPlayerPath;
	static XKTriggerEndCartoon _Instance;
	public static XKTriggerEndCartoon GetInstance()
	{
		return _Instance;
	}

	void Awake()
	{
		_Instance = this;

		bool isOutputError = false;
		int max = SpawnArray.Length;
		for (int i = 0; i < max; i++) {
			if (SpawnArray[i] == null) {
				Debug.LogWarning("Unity:"+"SpawnArray was wrong! index = "+i);
				isOutputError = true;
				break;
			}
		}

		max = RemoveArray.Length;
		for (int i = 0; i < max; i++) {
			if (RemoveArray[i] == null) {
				Debug.LogWarning("Unity:"+"RemoveArray was wrong! index = "+i);
				isOutputError = true;
				break;
			}
			XkGameCtrl.AddCartoonTriggerSpawnList(RemoveArray[i]);
		}

		if (isOutputError) {
			GameObject obj = null;
			obj.name = "null";
		}
		XkGameCtrl.GetInstance().ChangeBoxColliderSize(transform);
	}

	/***************************************************************
	 * 1.结束开场动画，镜头交给主角.
	 * 2.打开所有主角UI.
	 * 3.关闭指定触发器.
	 * ************************************************************/
	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<XkPlayerCtrl>() == null) {
			return;
		}
		//Debug.Log("Unity:"+"XKTriggerSpawnNpc::OnTriggerEnter -> hit "+other.name);
		CloseStartCartoon();
	}

	void MakeOtherPortCloseCartoon()
	{
		if (!gameObject.activeSelf) {
			return;
		}
		
		if (ScreenDanHeiCtrl.IsStartGame) {
			return;
		}

		if (Network.peerType == NetworkPeerType.Disconnected) {
			return;
		}

		if (NetCtrl.GetInstance() != null) {
			NetCtrl.GetInstance().MakeOtherPortCloseCartoon();
		}
	}

	public void CloseStartCartoon()
	{
		if (!gameObject.activeSelf) {
			return;
		}

		if (ScreenDanHeiCtrl.IsStartGame) {
			return;
		}
		MakeOtherPortCloseCartoon();
		ScreenDanHeiCtrl.GetInstance().CloseStartCartoon();

		int max = SpawnArray.Length;
		for (int i = 0; i < max; i++) {
			if (SpawnArray[i] != null) {
				SpawnArray[i].gameObject.SetActive(false);
			}
		}
		
		max = RemoveArray.Length;
		for (int i = 0; i < max; i++) {
			if (RemoveArray[i] != null) {
				RemoveArray[i].gameObject.SetActive(false);
			}
		}

		ScreenDanHeiCtrl.GetInstance().InitPlayScreenDanHei();
		//RenWuXinXiCtrl.GetInstance().HiddenRenWuXinXi();
		gameObject.SetActive(false);
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
}