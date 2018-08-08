using UnityEngine;
using System.Collections;

public class JiFenJieMianCtrl : MonoBehaviour {
	public GameObject JiFenJieMianObj;
	public GameObject ScreenDanHeiObj;
	GameObject JiFenZongJieMianObj;
	bool IsShowFinishTask;
	bool IsMakeJiFenStop;
	float TimeStartVal;
	static JiFenJieMianCtrl Instance;
	public static JiFenJieMianCtrl GetInstance()
	{
		return Instance;
	}

	// Use this for initialization
	void Start()
	{
		Instance = this;
		TimeStartVal = Time.realtimeSinceStartup;
		JiFenZongJieMianObj = gameObject;
		UITexture screenTexture = ScreenDanHeiObj.GetComponent<UITexture>();
		screenTexture.alpha = 0f;
		ScreenDanHeiObj.SetActive(false);
		JiFenZongJieMianObj.SetActive(false);
		JiFenJieMianObj.SetActive(false);
	}

	public bool GetIsShowFinishTask()
	{
		return IsShowFinishTask;
	}

	public void ShowFinishTaskInfo()
	{
		if (IsShowFinishTask) {
			return;
		}
		IsShowFinishTask = true;
		DanYaoInfoCtrl.GetInstanceOne().HiddenPlayerDanYaoInfo();
		DanYaoInfoCtrl.GetInstanceTwo().HiddenPlayerDanYaoInfo();
//		ZhunXingCtrl.GetInstanceOne().SetActiveZhunXingObj(false);
//		ZhunXingCtrl.GetInstanceTwo().SetActiveZhunXingObj(false);
		CoinPlayerCtrl.GetInstanceOne().HiddenPlayerCoin();
		CoinPlayerCtrl.GetInstanceTwo().HiddenPlayerCoin();
		YouLiangAddCtrl.GetInstance().HiddenYouLiangAdd();
		YouLiangCtrl.GetInstance().HiddenYouLiang();
		JiFenZongJieMianObj.SetActive(true);
		
		if (Network.peerType == NetworkPeerType.Server) {
			ScreenDanHeiCtrl.GetInstance().OpenPlayerUI();
			return;
		}
//		FinishTaskObj.SetActive(true);
		XKGlobalData.GetInstance().PlayAudioRenWuOver();
	}

	public void DelayActiveJiFenJieMian()
	{
		ScreenDanHeiObj.SetActive(true);
//		CancelInvoke("ActiveJiFenJieMian");
//		Invoke("ActiveJiFenJieMian", 2f);
	}

	public void ActiveJiFenJieMian()
	{
		if (Network.peerType == NetworkPeerType.Server) {
			return;
		}

		if (JiFenJieMianObj.activeSelf) {
			return;
		}
//		FinishTaskObj.SetActive(false);
		IsShowFinishTask = true;
		IsMakeJiFenStop = false;
		//pcvr.CloseAllQiNangArray(PlayerEnum.Null, 1);

		if (!JiFenZongJieMianObj.activeSelf) {
			CoinPlayerCtrl.GetInstanceOne().HiddenPlayerCoin();
			CoinPlayerCtrl.GetInstanceTwo().HiddenPlayerCoin();
			CoinPlayerCtrl.GetInstanceThree().HiddenPlayerCoin();
			CoinPlayerCtrl.GetInstanceFour().HiddenPlayerCoin();
			GameTimeCtrl.GetInstance().HiddenGameTime();
			DaoJuCtrl.GetInstance().HiddenAllPlayerDaoJu();
			//DongGanUICtrl.InstanceOne.HiddenDongGanUI();
			//DongGanUICtrl.InstanceTwo.HiddenDongGanUI();
			//DongGanUICtrl.InstanceThree.HiddenDongGanUI();
			//DongGanUICtrl.InstanceFour.HiddenDongGanUI();
			JiFenZongJieMianObj.SetActive(true);
		}
		PaiMingCtrl.GetInstance().ShowGamePaiMing();

		System.GC.Collect();
		JiFenJieMianObj.SetActive(true);
		Invoke("StopJiFenTime", 8f);
	}

	public void StopJiFenTime()
	{
		if (Time.realtimeSinceStartup - TimeStartVal < 8f) {
			Debug.Log("Unity:"+"StopJiFenTime -> TimeStartVal was wrong!");
			return;
		}

		if (IsMakeJiFenStop) {
			return;
		}
		IsMakeJiFenStop = true;
		IsShowFinishTask = false;
		JiFenJieMianObj.SetActive(false);
		Debug.Log("Unity:"+"StopJiFenTime...");

		if (GameOverCtrl.IsShowGameOver) {
			XkGameCtrl.LoadingGameMovie();
			return;
		}

		CountJiFenOpen++;
		//CountJiFenOpen = 4; //test.
		if (CountJiFenOpen < 4) {
			XKBossXueTiaoCtrl.IsWuDiPlayer = false;
			XKTriggerStopMovePlayer.IsActiveTrigger = false;
			GameTimeCtrl.GetInstance().OpenGameTime();
			
			CoinPlayerCtrl.GetInstanceOne().ShwoPlayerCoin();
			CoinPlayerCtrl.GetInstanceTwo().ShwoPlayerCoin();
			CoinPlayerCtrl.GetInstanceThree().ShwoPlayerCoin();
			CoinPlayerCtrl.GetInstanceFour().ShwoPlayerCoin();
			DaoJuCtrl.GetInstance().ShowAllPlayerDaoJu();
			XKPlayerScoreCtrl.ShowPlayerScore(PlayerEnum.Null);
		}
		else {
			QuanBuTongGuanCtrl.GetInstance().ShowQuanBuTongGuan();
		}
	}
	int CountJiFenOpen;

	void MakeOtherPortStopJiFenTime()
	{
		if (Network.peerType == NetworkPeerType.Disconnected) {
			return;
		}

		if (NetCtrl.GetInstance() != null) {
			NetCtrl.GetInstance().MakeOtherPortStopJiFenTime();
		}
	}
}