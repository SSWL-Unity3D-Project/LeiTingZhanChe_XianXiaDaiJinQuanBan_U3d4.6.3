using UnityEngine;
using System.Collections;

public class GameOverCtrl : MonoBehaviour {
	GameObject GameOverObj;
	public static bool IsShowGameOver;
	static GameOverCtrl Instance;
	public static GameOverCtrl GetInstance()
	{
		return Instance;
	}

	// Use this for initialization
	void Start()
	{
		Instance = this;
		IsShowGameOver = false;
		GameOverObj = gameObject;
		GameOverObj.SetActive(false);
	}

	public void ShowGameOver(int key = 0)
	{
		if (IsShowGameOver) {
			return;
		}
		IsShowGameOver = true;
		GameOverObj.SetActive(true);

		//if (pcvr.bIsHardWare) {
		//	MyCOMDevice.GetInstance().ForceRestartComPort();
		//}

		switch (key) {
		case 0:
			XKGlobalData.GetInstance().PlayAudioGameOver();
			Invoke("HiddenGameOver", 3f);
			//MakeServerShowGameOver();
			break;
		default:
			Invoke("DelayLoadingGameMovie", 5f);
			break;
		}
	}

	void DelayLoadingGameMovie()
	{
		GameOverObj.SetActive(false);
		XkGameCtrl.LoadingGameMovie(); //Back Movie Scene.
	}

	void HiddenGameOver()
	{
		GameOverObj.SetActive(false);
		//XkGameCtrl.LoadingGameMovie(); //Back Movie Scene.
		JiFenJieMianCtrl.GetInstance().ActiveJiFenJieMian();
	}

	void MakeServerShowGameOver()
	{
		if (Network.peerType == NetworkPeerType.Disconnected) {
			return;
		}
		
		if (NetCtrl.GetInstance() != null) {
			NetCtrl.GetInstance().MakeServerShowGameOver();
		}
	}
}

public enum GameLevel
{
	None = -1,
    root = 0,
	Movie = 1,
	Scene_1 = 2,
	SetPanel = 3,
}