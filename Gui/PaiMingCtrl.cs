using UnityEngine;
using System.Collections.Generic;

public class PaiMingCtrl : MonoBehaviour
{
	List<int> PlayerJiFenList;
	static PaiMingCtrl _Instance;
	public static PaiMingCtrl GetInstance()
	{
		return _Instance;
	}

	void Awake()
	{
		_Instance = this;
	}

	void Update()
	{
		if (pcvr.bIsHardWare) {
			return;
		}
		if (Input.GetKeyUp(KeyCode.B)) {
			ShowGamePaiMing();
		}
	}

	//public int[] PlayerIndexArray = {0, 1, 2, 3};
	public UITexture[] PlayerNumUITexture;
	/**
	 * 四人版.
	 */
	public Texture[] PlayerTexture;
	/**
	 * 双人版.
	 */
	public Texture[] PlayerTextureShR;
	public UISprite[] PlayerJF_1;
	public UISprite[] PlayerJF_2;
	public UISprite[] PlayerJF_3;
	public UISprite[] PlayerJF_4;
	public GameObject[] PlayerPaiMingObjArray;
	public GameObject XinJiLuObj;
	public GameObject HuangGuanObj;
	public static int XinJiLuVal;
	void SetPlayerJiFen(UISprite[] uiSpriteJF, int jiFen)
	{
		int max = uiSpriteJF.Length;
		int numVal = jiFen;
		int valTmp = 0;
		int powVal = 0;
		for (int i = 0; i < max; i++) {
			powVal = (int)Mathf.Pow(10, max - i - 1);
			valTmp = numVal / powVal;
			//Debug.Log("Unity:"+"valTmp *** "+valTmp);
			uiSpriteJF[i].spriteName = "KillNum_" + valTmp;
			numVal -= valTmp * powVal;
		}
	}

	/**
	 * BossFenShuVal -> boss战中剩余时间给玩家的积分倍率.
	 */
	const int BossFenShuVal = 300;
	const int MaxGameFenShu = 99999999;
	public void ShowGamePaiMing()
	{
        //Debug.Log("Unity:"+"ShowGamePaiMing...");
        int timeShengYu = 0;
        if (GameTimeBossCtrl.GetInstance() != null)
        {
            timeShengYu = GameTimeBossCtrl.GetInstance().GetTimeBossResidual();
        }
		//timeShengYu = 51; //test.
		BossJiFenCtrl.GetInstance().ShowBossJiFenInfo(timeShengYu);

		int fenShuBoss = BossFenShuVal * timeShengYu;
		//Debug.Log("Unity:"+"fenShuBoss ****************** "+fenShuBoss);
		if (XkGameCtrl.IsActivePlayerOne) {
			XkGameCtrl.PlayerJiFenArray[0] += fenShuBoss;
			if (fenShuBoss > 0) {
				XKPlayerScoreCtrl.ChangePlayerScore(PlayerEnum.PlayerOne);
			}
		}
		if (XkGameCtrl.IsActivePlayerTwo) {
			XkGameCtrl.PlayerJiFenArray[1] += fenShuBoss;
			if (fenShuBoss > 0) {
				XKPlayerScoreCtrl.ChangePlayerScore(PlayerEnum.PlayerTwo);
			}
		}
		if (XkGameCtrl.IsActivePlayerThree) {
			XkGameCtrl.PlayerJiFenArray[2] += fenShuBoss;
			if (fenShuBoss > 0) {
				XKPlayerScoreCtrl.ChangePlayerScore(PlayerEnum.PlayerThree);
			}
		}
		if (XkGameCtrl.IsActivePlayerFour) {
			XkGameCtrl.PlayerJiFenArray[3] += fenShuBoss;
			if (fenShuBoss > 0) {
				XKPlayerScoreCtrl.ChangePlayerScore(PlayerEnum.PlayerFour);
			}
		}

		for (int i = 0; i < XkGameCtrl.PlayerJiFenArray.Length; i++) {
			XkGameCtrl.PlayerJiFenArray[i] = XkGameCtrl.PlayerJiFenArray[i] > MaxGameFenShu
				? MaxGameFenShu : XkGameCtrl.PlayerJiFenArray[i];
		}

		for (int i = 0; i < PlayerPaiMingObjArray.Length; i++) {
			PlayerPaiMingObjArray[i].SetActive(false);
		}
		XinJiLuObj.SetActive(false);
		HuangGuanObj.SetActive(false);

		int countJF = 0;
		bool isActiveXinJiLu = false;
		bool isActivePlayerJF = false;
		
		//test start.
		/*XkGameCtrl.PlayerJiFenArray[0] = 123;
		XkGameCtrl.PlayerJiFenArray[1] = 12345;
		XkGameCtrl.PlayerJiFenArray[2] = 1234;
		XkGameCtrl.PlayerJiFenArray[3] = 1234;*/
		//test end.
		int[] playerJFArray = XkGameCtrl.PlayerJiFenArray;
		if (PlayerJiFenList != null) {
			PlayerJiFenList.Clear();
			PlayerJiFenList = null;
		}

		PlayerJiFenList = new List<int>(playerJFArray);
		PlayerJiFenList.Sort();
		PlayerJiFenList.Reverse();
		int[] jiLuFenShuKey = {0, 0, 0, 0};
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 4; j++) {
				/*Debug.Log("Unity:"+"playerJFArray["+i+"] *** "+playerJFArray[i]
				          +", PlayerJiFenList["+j+"] *** "+PlayerJiFenList[j]);*/
				if (playerJFArray[i] == PlayerJiFenList[j] && jiLuFenShuKey[j] == 0) {
					jiLuFenShuKey[j] = 1;
					isActivePlayerJF = playerJFArray[i] > 0 ? true : false;
					if (!isActivePlayerJF && j > 0) {
						break;
					}

					countJF++;
					if (XKGlobalData.GameVersionPlayer == 0)
                    {
                        PlayerNumUITexture[j].mainTexture = PlayerTexture[i];
                    }
					else
                    {
						PlayerNumUITexture[j].mainTexture = PlayerTextureShR[i];
					}

					switch (j) {
					case 0:
						isActivePlayerJF = true;
						if (XinJiLuVal < PlayerJiFenList[j]) {
							isActiveXinJiLu = true;
							XinJiLuVal = PlayerJiFenList[j];
						}
						SetPlayerJiFen(PlayerJF_1, PlayerJiFenList[j]);
						break;
					case 1:
						SetPlayerJiFen(PlayerJF_2, PlayerJiFenList[j]);
						break;
					case 2:
						SetPlayerJiFen(PlayerJF_3, PlayerJiFenList[j]);
						break;
					case 3:
						SetPlayerJiFen(PlayerJF_4, PlayerJiFenList[j]);
						break;
					}
					PlayerPaiMingObjArray[j].SetActive(isActivePlayerJF);
					break;
				}
			}
		}

		if (countJF > 1) {
			HuangGuanObj.SetActive(true);
		}
		XinJiLuObj.SetActive(isActiveXinJiLu);
		
		if (GameJiLuFenShuCtrl.GetInstance() != null) {
			GameJiLuFenShuCtrl.GetInstance().ShowGameJiLuScore();
		}		
		XKPlayerScoreCtrl.HiddenPlayerScore(PlayerEnum.Null);
	}
}