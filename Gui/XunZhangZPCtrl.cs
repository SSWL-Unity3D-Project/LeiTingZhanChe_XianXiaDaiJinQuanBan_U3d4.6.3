using UnityEngine;
using System.Collections;

public class XunZhangZPCtrl : MonoBehaviour {
	public PlayerEnum PlayerSt = PlayerEnum.Null;
	public GameObject XunZhangBJ;
	public UITexture XunZhangUITexture;
	public Texture[] XunZhangTexture;
	int[] XunZhangZP = {100, 200, 300};
	static XunZhangZPCtrl InstanceOne;
	public static XunZhangZPCtrl GetInstanceOne()
	{
		return InstanceOne;
	}

	static XunZhangZPCtrl InstanceTwo;
	public static XunZhangZPCtrl GetInstanceTwo()
	{
		return InstanceTwo;
	}

	// Use this for initialization
	void Start()
	{
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			InstanceOne = this;
			break;
			
		case PlayerEnum.PlayerTwo:
			InstanceTwo = this;
			break;
		}
		XunZhangBJ.SetActive(false);
		XunZhangUITexture.gameObject.SetActive(false);
	}

	public void ShowPlayerXunZhang()
	{
		if (XunZhangUITexture.gameObject.activeSelf) {
			return;
		}
//		XunZhangZP = XkGameCtrl.GetInstance().XunZhangZP;

		int xunZhangIndex = -1;
		switch (PlayerSt) {
		case PlayerEnum.PlayerOne:
			if (XkGameCtrl.YouLiangDianAddPOne > XunZhangZP[2]) {
				xunZhangIndex = 3;
			}
			else if (XkGameCtrl.YouLiangDianAddPOne > XunZhangZP[1]) {
				xunZhangIndex = 2;
			}
			else if (XkGameCtrl.YouLiangDianAddPOne > XunZhangZP[0]) {
				xunZhangIndex = 1;
			}
			else if (XkGameCtrl.YouLiangDianAddPOne > 0) {
				xunZhangIndex = 0;
			}
			else {
				xunZhangIndex = -1;
			}
			//xunZhangIndex = randValTest;
			break;

		case PlayerEnum.PlayerTwo:
			if (XkGameCtrl.YouLiangDianAddPTwo > XunZhangZP[2]) {
				xunZhangIndex = 3;
			}
			else if (XkGameCtrl.YouLiangDianAddPTwo > XunZhangZP[1]) {
				xunZhangIndex = 2;
			}
			else if (XkGameCtrl.YouLiangDianAddPTwo > XunZhangZP[0]) {
				xunZhangIndex = 1;
			}
			else if (XkGameCtrl.YouLiangDianAddPTwo > 0) {
				xunZhangIndex = 0;
			}
			else {
				xunZhangIndex = -1;
			}
			//xunZhangIndex = randValTest;
			break;
		}

		if (-1 != xunZhangIndex) {
			XunZhangUITexture.mainTexture = XunZhangTexture[xunZhangIndex];
			XunZhangUITexture.gameObject.SetActive(true);
		}

		if (XkGameCtrl.GameModeVal != GameMode.LianJi || !GameMovieCtrl.IsActivePlayer) {
			Invoke("DelayStopJiFenPanel", 2.5f);
		}

		IsOverPlayerZPXunZhang = true;
		CheckLianJiIsShouldStopJiFenPanel();
	}

	public void CheckLianJiIsShouldStopJiFenPanel()
	{
		if (XkGameCtrl.GameModeVal != GameMode.LianJi) {
			return;
		}

		if (!IsShouldStopJiFenPanel && !IsOverPlayerZPXunZhang) {
			return;
		}

		if (IsOverPlayerZPXunZhang
		    && (Application.loadedLevel == (int)(GameLevel.Scene_1) || GameOverCtrl.IsShowGameOver)) {
			//&& (Application.loadedLevel == XkGameCtrl.TestGameEndLv || GameOverCtrl.IsShowGameOver)) { //test
			IsShouldStopJiFenPanel = false;
			IsOverPlayerZPXunZhang = false;
			Invoke("DelayStopJiFenPanel", 2.5f);
			return;
		}

		if (IsShouldStopJiFenPanel) {
			Debug.Log("Unity:"+"CheckLianJiIsShouldStopJiFenPanel...");
			IsShouldStopJiFenPanel = false;
			IsOverPlayerZPXunZhang = false;
			Invoke("DelayStopJiFenPanel", 2.5f);
		}
	}
	public static bool IsOverPlayerZPXunZhang;
	public static bool IsShouldStopJiFenPanel;

	void DelayStopJiFenPanel()
	{
        if (JiFenJieMianCtrl.GetInstance() != null)
        {
            JiFenJieMianCtrl.GetInstance().StopJiFenTime();
        }
	}

	public void ShowXunZhangBJXue()
	{
		if (XunZhangBJ.activeSelf) {
			return;
		}
		XunZhangBJ.SetActive(true);
		XKGlobalData.GetInstance().PlayAudioXunZhangZP();
	}
}