using UnityEngine;
using System.Collections;

public class DongGanUICtrl : MonoBehaviour {
	public PlayerEnum PlayerSt;
	public Texture[] DongGanUI;
	UITexture DongGanTexture;
	/**
	 * DongGanCount == 0 -> 关闭动感.
	 * DongGanCount == 1 -> 打开动感.
	 */
	int DongGanCount;
	public static DongGanUICtrl InstanceOne;
	public static DongGanUICtrl InstanceTwo;
	public static DongGanUICtrl InstanceThree;
	public static DongGanUICtrl InstanceFour;
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
		case PlayerEnum.PlayerThree:
			InstanceThree = this;
			break;
		case PlayerEnum.PlayerFour:
			InstanceFour = this;
			break;
		}
		DongGanCount = 1;
		DongGanTexture = GetComponent<UITexture>();
		DongGanTexture.mainTexture = DongGanUI[0];
		gameObject.SetActive(false);
	}

	public static void ShowDongGanInfo(PlayerEnum playerIndex)
	{
		if (!ScreenDanHeiCtrl.IsStartGame) {
			return;
		}

		if (JiFenJieMianCtrl.GetInstance() && JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
			return;
		}

		DongGanUICtrl instanceDG = null;
		switch(playerIndex) {
		case PlayerEnum.PlayerOne:
			instanceDG = InstanceOne;
			break;
		case PlayerEnum.PlayerTwo:
			instanceDG = InstanceTwo;
			break;
		case PlayerEnum.PlayerThree:
			instanceDG = InstanceThree;
			break;
		case PlayerEnum.PlayerFour:
			instanceDG = InstanceFour;
			break;
		}

		if (instanceDG == null) {
			return;
		}
		instanceDG.AddDongGanCount();
	}

	void AddDongGanCount()
	{
		DongGanCount++;
		DongGanCount = DongGanCount > 1 ? 0 : DongGanCount;
		ShowDongGanUI(DongGanCount);
	}

	void ShowDongGanUI(int index)
	{
		DongGanTexture.mainTexture = DongGanUI[index];
		gameObject.SetActive(true);
		CancelInvoke("HiddenDongGanUI");
		if (index == 1) {
			Invoke("HiddenDongGanUI", 3f);
		}
	}

	public void HiddenDongGanUI()
	{
		DongGanCount = 1;
		gameObject.SetActive(false);
	}
}