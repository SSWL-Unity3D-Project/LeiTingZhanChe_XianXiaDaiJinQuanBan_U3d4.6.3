//#define NOT_SHOW_DAOJISHI_UI //不显示倒计时UI.
using UnityEngine;

public class DaoJiShiCtrl : MonoBehaviour {
	public PlayerEnum PlayerIndex = PlayerEnum.PlayerOne;
	public GameObject ContinueGameObj;
	public GameObject GameOverObj;
    /// <summary>
    /// 电视遥控器确认按键图片.
    /// </summary>
    //public GameObject m_TVYaoKongEnterObj;
	GameObject DaoJiShiObj;
	UISprite DaoJiShiSprite;
	internal bool IsPlayDaoJishi;
	int DaoJiShiCount = 9;
//	public static bool IsActivePlayerOne;
//	public static bool IsActivePlayerTwo;
	public static int CountDaoJiShi;
	
	static DaoJiShiCtrl InstanceOne;
	public static DaoJiShiCtrl GetInstanceOne()
	{
		return InstanceOne;
	}
	
	static DaoJiShiCtrl InstanceTwo;
	public static DaoJiShiCtrl GetInstanceTwo()
	{
		return InstanceTwo;
	}
	
	static DaoJiShiCtrl InstanceThree;
	public static DaoJiShiCtrl GetInstanceThree()
	{
		return InstanceThree;
	}
	
	static DaoJiShiCtrl InstanceFour;
	public static DaoJiShiCtrl GetInstanceFour()
	{
		return InstanceFour;
	}

	public static DaoJiShiCtrl GetInstance(PlayerEnum indexPlayer)
	{
		DaoJiShiCtrl djsInstance = null;
		switch (indexPlayer) {
		case PlayerEnum.PlayerOne:
			djsInstance = InstanceOne;
			break;
		case PlayerEnum.PlayerTwo:
			djsInstance = InstanceTwo;
			break;
		case PlayerEnum.PlayerThree:
			djsInstance = InstanceThree;
			break;
		case PlayerEnum.PlayerFour:
			djsInstance = InstanceFour;
			break;
		}
		return djsInstance;
	}

	// Use this for initialization
	void Start()
	{
		CountDaoJiShi = 0;
		switch (PlayerIndex) {
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
//		IsActivePlayerOne = false;
//		IsActivePlayerTwo = false;
		DaoJiShiObj = gameObject;
		DaoJiShiSprite = GetComponent<UISprite>();
		DaoJiShiObj.SetActive(false);
		ContinueGameObj.SetActive(false);
        //m_TVYaoKongEnterObj.SetActive(false);
        GameOverObj.SetActive(false);
    }

	public void StartPlayDaoJiShi()
	{
		if (GameOverCtrl.IsShowGameOver) {
			return;
		}


        if (XkGameCtrl.GetInstance().GetPlayerIsCanContinuePlayGame(PlayerIndex) == true)
        {
            //玩家币值充足,可以继续进行游戏.
            return;
        }
        else
        {
            //玩家币值不足,需要进行充值.
            if (pcvr.GetInstance().m_HongDDGamePadInterface.GetHongDDGamePadWXPay() != null)
            {
                //玩家币值不足,通知游戏服务端拉起手机微信复活重置界面.
                pcvr.GetInstance().m_HongDDGamePadInterface.GetHongDDGamePadWXPay().CToS_OnPlayerDeath("0");
            }
        }

        if (IsPlayDaoJishi)
        {
			return;
		}
		IsPlayDaoJishi = true;

#if NOT_SHOW_DAOJISHI_UI
        if (pcvr.IsHongDDShouBing == true)
        {
            //微信支付版本不显示倒计时数字信息.
            ContinueGameObj.SetActive(true);
            return;
        }
#endif

        if (SSUIRoot.GetInstance().m_GameUIManage != null)
        {
            SSUIRoot.GetInstance().m_GameUIManage.RemovePlayerDaiJinQunUI(PlayerIndex);
        }
        CountDaoJiShi++;
		DaoJiShiCount = 9;
		DaoJiShiSprite.spriteName = "daoJiShi9";
        //m_TVYaoKongEnterObj.SetActive(true);
		DaoJiShiObj.SetActive(true);
		ContinueGameObj.SetActive(true);
        ShowDaoJiShiInfo();
		//XKGlobalData.GetInstance().StopAudioRanLiaoJingGao();
		//pcvr.CloseAllQiNangArray(PlayerIndex, 1);
    }

	public void StopDaoJiShi()
	{
        if (GameOverObj.activeInHierarchy == true)
        {
            if (IsInvoking("HiddenGameOverObj"))
            {
                CancelInvoke("HiddenGameOverObj");
            }
            GameOverObj.SetActive(false);
        }

        if (!IsPlayDaoJishi)
        {
            //重置玩家信息.
            XkGameCtrl.GetInstance().ResetPlayerInfo(PlayerIndex);
            XKPlayerScoreCtrl.ShowPlayerScore(PlayerIndex);
            return;
		}
		IsPlayDaoJishi = false;
		CountDaoJiShi--;
		ContinueGameObj.SetActive(false);
		DaoJiShiObj.SetActive(false);
        //m_TVYaoKongEnterObj.SetActive(false);
    }

	void ShowDaoJiShiInfo()
	{
		XKGlobalData.GetInstance().PlayAudioXuBiDaoJiShi();
		TweenScale tweenScaleCom = GetComponent<TweenScale>();
		if (tweenScaleCom != null) {
			DestroyObject(tweenScaleCom);
		}

		tweenScaleCom = DaoJiShiObj.AddComponent<TweenScale>();
		tweenScaleCom.enabled = false;
		tweenScaleCom.duration = 2f;
		tweenScaleCom.from = new Vector3(1.2f, 1.2f, 1f);
		tweenScaleCom.to = new Vector3(1f, 1f, 1f);
		EventDelegate.Add(tweenScaleCom.onFinished, delegate{
			ChangeDaoJiShiVal();
		});
		tweenScaleCom.enabled = true;
		tweenScaleCom.PlayForward();
	}

    /// <summary>
    /// 微信玩家关闭倒计时界面,当玩家续费失败后.
    /// </summary>
    internal void WXPlayerStopGameDaoJiShi()
    {
        if (IsPlayDaoJishi == false)
        {
            return;
        }
        StopDaoJiShi();
        ShowGameOverObj();
        //玩家没有进行续币.
        //重置玩家续币信息.
        XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.ResetPlayerXuBiInfo(PlayerIndex);
        XkGameCtrl.GetInstance().ResetPlayerInfo(PlayerIndex);
    }

	void ChangeDaoJiShiVal()
	{
		if (JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
			StopDaoJiShi();
			return;
		}

		if (DaoJiShiCount <= 1) {
			StopDaoJiShi();
			ShowGameOverObj();
//			if (XkGameCtrl.PlayerActiveNum <= 0 && CountDaoJiShi > 0) {
//				Debug.LogWarning("Unity:"+"ChangeDaoJiShiVal -> CountDaoJiShi "+CountDaoJiShi);
//			}

            //玩家没有进行续币.
            //重置玩家续币信息.
            XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.ResetPlayerXuBiInfo(PlayerIndex);
            //if (XkGameCtrl.PlayerActiveNum <= 0 && CountDaoJiShi <= 0) {
            //	GameOverCtrl.GetInstance().ShowGameOver();
            //}

            XkGameCtrl.GetInstance().ResetPlayerInfo(PlayerIndex);
            return;
		}

		DaoJiShiCount--;
		DaoJiShiSprite.spriteName = "daoJiShi" + DaoJiShiCount;
		ShowDaoJiShiInfo();
	}

	public bool GetIsPlayDaoJishi()
	{
		return IsPlayDaoJishi;
	}

	void ShowGameOverObj()
	{
		GameOverObj.SetActive(true);
		Invoke("HiddenGameOverObj", 3f);
	}

	void HiddenGameOverObj()
	{
		if (!GameOverObj.activeSelf) {
			return;
		}
		CancelInvoke("HiddenGameOverObj");
		GameOverObj.SetActive(false);
#if NOT_SHOW_DAOJISHI_UI //不显示倒计时UI.
        if (XkGameCtrl.PlayerActiveNum <= 0)
        {
            //没有激活一个玩家.
            XkGameCtrl.GetInstance().OpenAllAiPlayerTank();
        }
#else
        if (XkGameCtrl.PlayerActiveNum <= 0)
        {
            //没有激活一个玩家.
            bool isDisplay = GetIsHaveDaoJiShiDisplaying();
            if (isDisplay == false)
            {
                XkGameCtrl.GetInstance().OpenAllAiPlayerTank();
            }
        }
#endif

        //游戏倒计时结束后清空玩家所得彩票数(代金券)
        if (XkPlayerCtrl.GetInstanceFeiJi() != null)
        {
            XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.ClearPlayerCaiPiaoData(PlayerIndex);
        }
    }

    /// <summary>
    /// 获取当前是否有倒计时在显示.
    /// </summary>
    bool GetIsHaveDaoJiShiDisplaying()
    {
        bool isDisplay = false;
        if (InstanceOne.IsPlayDaoJishi || InstanceTwo.IsPlayDaoJishi || InstanceThree.IsPlayDaoJishi)
        {
            isDisplay = true;
        }
        return isDisplay;
    }
}