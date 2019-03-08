//#define NOT_SHOW_DAOJISHI_UI //不显示倒计时UI.
using System.Collections;
using UnityEngine;

public class DaoJiShiCtrl : MonoBehaviour
{
	public PlayerEnum PlayerIndex = PlayerEnum.PlayerOne;
	public GameObject ContinueGameObj;
	public GameObject GameOverObj;
    /// <summary>
    /// 免费时间数字UI控制组件.
    /// </summary>
    public UILabel m_TimeMianFeiNum;
    /// <summary>
    /// 下一次免费间隔时间提示.
    /// </summary>
    public GameObject m_MianFeiTimeUI;
    /// <summary>
    /// 电视遥控器确认按键图片.
    /// </summary>
    //public GameObject m_TVYaoKongEnterObj;
	GameObject DaoJiShiObj;
	UISprite DaoJiShiSprite;
    bool _IsPlayDaoJiShi = false;
	internal bool IsPlayDaoJishi
    {
        set { _IsPlayDaoJiShi = value; }
        get
        {
            if (_IsPlayDaoJiShi == false)
            {
                if (SSUIRoot.GetInstance().m_GameUIManage != null)
                {
                    if (SSUIRoot.GetInstance().m_GameUIManage.GetPlayerPingJiUI(PlayerIndex) != null)
                    {
                        //当玩家的评级UI界面被创建时,认为此时该玩家已经播放游戏倒计时了.
                        return true;
                    }
                }
            }
            return _IsPlayDaoJiShi;
        }
    }
	internal int DaoJiShiCount = 9;
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
    
    public static bool GetIsHavePlayDaoJiShi()
    {
        if (InstanceOne != null)
        {
            if (InstanceOne.IsPlayDaoJishi == true)
            {
                return true;
            }
        }
        if (InstanceTwo != null)
        {
            if (InstanceTwo.IsPlayDaoJishi == true)
            {
                return true;
            }
        }
        if (InstanceThree != null)
        {
            if (InstanceThree.IsPlayDaoJishi == true)
            {
                return true;
            }
        }
        return false;
    }

	public void StartPlayDaoJiShi()
	{
        if (XKGlobalData.GetInstance().m_SSGameXuMingData != null)
        {
            if (XKGlobalData.GetInstance().m_SSGameXuMingData.GetIsCanXuMing(PlayerIndex) == false)
            {
                //续命次数超出.
                //此时需要发送不允许继续游戏的消息给手柄,30秒之后游戏发送踢出该玩家的消息给手柄(同时需要清除玩家的微信数据)
                if (pcvr.GetInstance().m_HongDDGamePadInterface != null)
                {
                    //清理玩家微信数据.
                    pcvr.GetInstance().m_HongDDGamePadInterface.SendWXPadShowFangChenMiPanel(PlayerIndex);
                }
            }
        }

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
            //if (pcvr.GetInstance().m_HongDDGamePadInterface.GetHongDDGamePadWXPay() != null)
            //{
            //    //玩家币值不足,通知游戏服务端拉起手机微信复活重置界面.
            //    pcvr.GetInstance().m_HongDDGamePadInterface.GetHongDDGamePadWXPay().CToS_OnPlayerDeath("0");
            //}
        }

        if (IsPlayDaoJishi)
        {
			return;
		}
		IsPlayDaoJishi = true;
        //SSDebug.Log("StartPlayDaoJiShi******************************************************IsPlayDaoJishi " + IsPlayDaoJishi + ", Time ======= " + Time.time.ToString("f2"));

#if NOT_SHOW_DAOJISHI_UI
        if (pcvr.IsHongDDShouBing == true)
        {
            //微信支付版本不显示倒计时数字信息.
            ContinueGameObj.SetActive(true);
            return;
        }
#endif

        if (XKGlobalData.GetInstance().m_MianFeiShiWanCount <= 0)
        {
            //首次付费模式.
            if (m_MianFeiTimeUI != null)
            {
                m_MianFeiTimeUI.SetActive(false);
            }
        }
        else
        {
            //首次免费模式.
            if (m_MianFeiTimeUI != null)
            {
                m_MianFeiTimeUI.SetActive(true);
                if (m_TimeMianFeiNum != null)
                {
                    //显示下次免费游戏的间隔时间数字.
                    //m_TimeMianFeiNum.ShowNumUI(XKGlobalData.GetInstance().m_TimeMianFeiNum);
                    m_TimeMianFeiNum.text = XKGlobalData.GetInstance().m_TimeMianFeiNum.ToString();
                }
            }
        }

        if (SSUIRoot.GetInstance().m_GameUIManage != null)
        {
            SSUIRoot.GetInstance().m_GameUIManage.RemovePlayerDaiJinQunUI(PlayerIndex);
        }
        CountDaoJiShi++;
		DaoJiShiCount = 9;
        DaoJiShiSprite.enabled = false;
        DaoJiShiSprite.spriteName = "daoJiShi9";
        DaoJiShiObj.SetActive(true);
        StartCoroutine(DelayShowPlayerDaoJiShi());
        //m_TVYaoKongEnterObj.SetActive(true);
        //DaoJiShiObj.SetActive(true);
        //ContinueGameObj.SetActive(true);
        //ShowDaoJiShiInfo();
		//XKGlobalData.GetInstance().StopAudioRanLiaoJingGao();
		//pcvr.CloseAllQiNangArray(PlayerIndex, 1);
    }

    IEnumerator DelayShowPlayerDaoJiShi()
    {
        if (ContinueGameObj.activeInHierarchy == true)
        {
            yield break;
        }

        //float timeVal = XKPlayerGlobalDt.GetInstance().m_DaoJiShiDelayShowPlayerDead;
        //yield return new WaitForSeconds(timeVal);
        yield return new WaitForSeconds(0.5f);
        //DaoJiShiObj.SetActive(true);
        DaoJiShiSprite.enabled = true;
        ContinueGameObj.SetActive(true);
        ShowDaoJiShiInfo();
        yield break;
    }

    public void StopDaoJiShi()
	{
        //SSDebug.Log("StopDaoJiShi******************************************************IsPlayDaoJishi " + IsPlayDaoJishi + ", Time ======= " + Time.time.ToString("f2"));
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

        bool isActive = XkGameCtrl.GetIsActivePlayer(PlayerIndex);
        if (isActive == false && pcvr.GetInstance() != null)
        {
            pcvr.GetInstance().m_HongDDGamePadInterface.OnPlayerGameDaoJiShiOver(PlayerIndex);
        }
    }

	void ShowDaoJiShiInfo()
	{
		XKGlobalData.GetInstance().PlayAudioXuBiDaoJiShi();
		TweenScale tweenScaleCom = GetComponent<TweenScale>();
		if (tweenScaleCom != null) {
			DestroyObject(tweenScaleCom);
		}

        m_TimeLastChange = Time.time;
        IsOpenChangeDaoJiShi = true;
        tweenScaleCom = DaoJiShiObj.AddComponent<TweenScale>();
		tweenScaleCom.enabled = false;
		tweenScaleCom.duration = m_TimeChangeVal;
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

    void Update()
    {
        if (IsOpenChangeDaoJiShi == true)
        {
            UpdataChangDaoJiShiVal();
        }
    }

    /// <summary>
    /// 倒计时变化间隔时间.
    /// </summary>
    float m_TimeChangeVal = 3f;
    float m_TimeLastChange = 0f;
    bool IsOpenChangeDaoJiShi = false;
    /// <summary>
    /// 避免因为游戏卡顿导致倒计时界面始终不动停留在游戏画面上.
    /// </summary>
    void UpdataChangDaoJiShiVal()
    {
        if (IsOpenChangeDaoJiShi == false)
        {
            return;
        }

        if (Time.time - m_TimeLastChange >= m_TimeChangeVal + 6f)
        {
            m_TimeLastChange = Time.time;
            ChangeDaoJiShiVal();
        }
    }

	void ChangeDaoJiShiVal()
	{
        m_TimeLastChange = Time.time;
        IsOpenChangeDaoJiShi = false;
        if (JiFenJieMianCtrl.GetInstance() != null && JiFenJieMianCtrl.GetInstance().GetIsShowFinishTask()) {
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
            
            if (XKGlobalData.GetInstance().m_SSGameXuMingData != null)
            {
                //如果当前机位游戏续命倒计时结束之后就清除续命计数信息.
                XKGlobalData.GetInstance().m_SSGameXuMingData.ResetXuMingCount(PlayerIndex);
            }
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

        CoinPlayerCtrl playerCoinCom = CoinPlayerCtrl.GetInstance(PlayerIndex);
        if (playerCoinCom != null)
        {
            playerCoinCom.SetActiveMianFeiTiYanUI(false);
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