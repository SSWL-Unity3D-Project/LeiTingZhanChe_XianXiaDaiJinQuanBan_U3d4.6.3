//#define USE_CENTER_ZUOYI
//#define COM_TANK_TEST
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class pcvr : MonoBehaviour {
    /// <summary>
    /// 是否为红点点微信手柄操作模式.
    /// </summary>
    public static bool IsHongDDShouBing = false;
    /// <summary>
    /// 是否为硬件版本.
    /// </summary>
	public static bool bIsHardWare = false;
	public static bool IsTestInput = false;
	public static bool IsTestHardWareError = false;
	public static Vector3 CrossPositionOne;
	public static Vector3 CrossPositionTwo;
	public static bool IsJiaoYanHid;
	public static bool IsSlowLoopCom;
	public static bool IsTestHardWare;
	private static int HID_BUF_LEN_WRITE = 0;
	private System.IntPtr gTestHidPtr;
	float lastUpTime = 0.0f;
	public static TKMoveState TKMoveSt = TKMoveState.YaoGanBan;
	public static float mGetSteer = 0f;
	public static Vector3 MousePositionP1;
	public static Vector3 MousePositionP2;
	public static bool IsClickFireBtDown;
	public static uint gOldCoinNum = 0;
	public int CoinNumCurrent = 0;
	
	public static bool bPlayerHitTaBan_P1 = false;
	public static bool bPlayerHitTaBan_P2 = false;
	
	public static float VerticalVal;
	public static float TanBanDownCount_P1 = 0;
	public static float TanBanDownCount_P2 = 0;
	
	public static LedState StartLightStateP1 = LedState.Mie;
	public static LedState StartLightStateP2 = LedState.Mie;
	public static LedState StartLightStateP3 = LedState.Mie;
	public static LedState StartLightStateP4 = LedState.Mie;
	public static LedState StartLightStateP5 = LedState.Mie;
	public static bool IsOpenStartLightP1 = false;
	public static bool IsOpenStartLightP2 = false;
	int SubCoinNum_12 = 0;
    int SubCoinNum_34 = 0;
	
	static string FileName;
	static HandleJson HandleJsonObj;
	public static uint[] SteerValMaxAy = new uint[4]{999999, 999999, 999999, 999999};
	public static uint[] SteerValCenAy = new uint[4]{1765, 1765, 1765, 1765};
	public static uint[] SteerValMinAy = new uint[4];
	public static uint[] SteerValCurAy = new uint[4];
	public static PcvrShuiBengState ShuiBengState = PcvrShuiBengState.Level_1;
	
	bool IsSubPlayerCoin = false;
	bool IsSubCoinP1 = false;
	bool IsSubCoinP2 = false;
	bool IsSubCoinP3 = false;
	bool IsSubCoinP4 = false;
	public int CoinNumCurrentP1 = 0;
	public int CoinNumCurrentP2 = 0;
	public int CoinNumCurrentP3 = 0;
	public int CoinNumCurrentP4 = 0;
	static bool IsFireBtDownP1 = false;
	static bool IsFireBtDownP2 = false;
	static bool IsFireBtDownP3 = false;
	static bool IsFireBtDownP4 = false;
	static bool IsDaoDanBtDownP1 = false;
	static bool IsDaoDanBtDownP2 = false;
	static bool IsDaoDanBtDownP3 = false;
	static bool IsDaoDanBtDownP4 = false;
	static public bool bPlayerStartKeyDownP1 = false;
	static public bool bPlayerStartKeyDownP2 = false;
	static public bool bPlayerStartKeyDownP3 = false;
	static public bool bPlayerStartKeyDownP4 = false;
	static public bool IsClickDongGanBtOne = false;
	static public bool IsClickDongGanBtTwo = false;
	static public bool IsClickDongGanBtThree = false;
	static public bool IsClickDongGanBtFour = false;
	private bool bSetEnterKeyDown = false;
	static public bool bSetMoveKeyDown = false;
	public static uint CoinCurPcvr12;
	public static uint CoinCurPcvr34;
    /// <summary>
    /// 二维码生成脚本.
    /// </summary>
    public BarcodeCam m_BarcodeCam;

    /// <summary>
    /// BoxPostNet控制组件.
    /// </summary>
    [HideInInspector]
    public SSBoxPostNet m_SSBoxPostNet;

    static pcvr _Instance;
	public static pcvr GetInstance()
	{
		if (_Instance == null) {
			GameObject obj = new GameObject("_PCVR");
			DontDestroyOnLoad(obj);
			_Instance = obj.AddComponent<pcvr>();
            _Instance.InitInfo();
			if (bIsHardWare) {
				obj.AddComponent<MyCOMDevice>();
			}

			#if COM_TANK_TEST
			obj.AddComponent<TestTanKCom>();
			#endif

			if (IsTestHardWareError) {
				TestComPort.GetInstance();
			}
            //NetworkServerNet.GetInstance();
            
            if (GameMovieCtrl.GetInstance() != null && IsHongDDShouBing)
            {
                GameObject websocketObj = GameMovieCtrl.GetInstance().SpawnWebSocketBox(obj.transform);
                _Instance.m_SSBoxPostNet = websocketObj.GetComponent<SSBoxPostNet>();
                _Instance.m_SSBoxPostNet.Init();
                _Instance.m_BarcodeCam = obj.AddComponent<BarcodeCam>();
            }
		}
		return _Instance;
	}
	
    void InitInfo()
    {
        //for (int i = 0; i < m_GmWXLoginDt.Length; i++)
        //{
        //    m_GmWXLoginDt[i] = new GameWeiXinLoginData();
        //}
        //InitHandleJsonInfo();
    }

	// Use this for initialization
	void Awake()
	{
		#if COM_TANK_TEST
		MyCOMDevice.PcvrComSt = MyCOMDevice.PcvrComState.TanKeGunZhenDong; //test.
		#endif

		/*if (!HardwareCheckCtrl.IsTestHardWare
		    && Application.loadedLevel == (int)GameLevel.SetPanel
		    && !GameTypeCtrl.IsSetTKMoveSt) {
			TKMoveSt = TKMoveState.U_FangXiangPan; //test在设置界面测试方向盘版硬件信息.
		}*/

		switch (MyCOMDevice.PcvrComSt) {
		case MyCOMDevice.PcvrComState.TanKeFangXiangZhenDong:
			MyCOMDevice.ComThreadClass.BufLenRead = 39;
			MyCOMDevice.ComThreadClass.BufLenWrite = 32;
			break;
		case MyCOMDevice.PcvrComState.TanKeGunZhenDong:
			MyCOMDevice.ComThreadClass.BufLenRead = 27;
			MyCOMDevice.ComThreadClass.BufLenWrite = 23;
			break;
		}

		if (Application.loadedLevel == (int)GameLevel.Movie) {
			AudioManager.Instance.SetParentTran(transform);
		}
	}

	void Start()
	{
		HID_BUF_LEN_WRITE = MyCOMDevice.ComThreadClass.BufLenWrite;
		lastUpTime = Time.realtimeSinceStartup;
		//InitJiaoYanMiMa();
		//InitSteerInfo();
		//InitYouMenInfo();

  //      if (IsHongDDShouBing)
  //      {
  //          if (m_SSBoxPostNet != null)
  //          {
  //              WebSocketSimpet webSocketSimpet = m_SSBoxPostNet.m_WebSocketSimpet;
  //              if (webSocketSimpet != null)
  //              {
  //                  Debug.Log("Unity:"+"add webSocketSimpet event...");
  //                  webSocketSimpet.OnEventPlayerLoginBox += OnEventPlayerLoginBox;
  //                  webSocketSimpet.OnEventPlayerExitBox += OnEventPlayerExitBox;
  //                  webSocketSimpet.OnEventDirectionAngle += OnEventDirectionAngle;
  //                  webSocketSimpet.OnEventActionOperation += OnEventActionOperation;
  //              }
  //              else
  //              {
  //                  Debug.Log("Unity:"+"webSocketSimpet is null!");
  //              }
  //          }
  //      }
    }

    /// <summary>
    /// 添加遥控器按键信息响应事件.
    /// </summary>
    public void AddTVYaoKongBtEvent()
    {
        if (!bIsHardWare)
        {
            InputEventCtrl.GetInstance().ClickTVYaoKongEnterBtEvent += ClickTVYaoKongEnterBtEvent;
            InputEventCtrl.GetInstance().ClickTVYaoKongUpBtEvent += ClickTVYaoKongUpBtEvent;
            InputEventCtrl.GetInstance().ClickTVYaoKongDownBtEvent += ClickTVYaoKongDownBtEvent;
            InputEventCtrl.GetInstance().ClickTVYaoKongLeftBtEvent += ClickTVYaoKongLeftBtEvent;
            InputEventCtrl.GetInstance().ClickTVYaoKongRightBtEvent += ClickTVYaoKongRightBtEvent;
        }
    }

    private void ClickTVYaoKongUpBtEvent(ButtonState val)
    {
        if (m_GmTVLoginDt != null)
        {
            int index = m_GmTVLoginDt.Index;
            InputEventCtrl.GetInstance().OnClickFangXiangUBt(index, val);

            if (val == ButtonState.DOWN)
            {
                InputEventCtrl.GetInstance().OnClickFireBt(index, ButtonState.DOWN);
                InputEventCtrl.GetInstance().OnClickDaoDanBt(index, ButtonState.DOWN);
            }
        }
    }

    private void ClickTVYaoKongDownBtEvent(ButtonState val)
    {
        if (m_GmTVLoginDt != null)
        {
            int index = m_GmTVLoginDt.Index;
            InputEventCtrl.GetInstance().OnClickFangXiangDBt(index, val);

            if (val == ButtonState.DOWN)
            {
                InputEventCtrl.GetInstance().OnClickFireBt(index, ButtonState.DOWN);
                InputEventCtrl.GetInstance().OnClickDaoDanBt(index, ButtonState.DOWN);
            }
        }
    }

    private void ClickTVYaoKongLeftBtEvent(ButtonState val)
    {
        if (m_GmTVLoginDt != null)
        {
            int index = m_GmTVLoginDt.Index;
            InputEventCtrl.GetInstance().OnClickFangXiangLBt(index, val);

            if (val == ButtonState.DOWN)
            {
                InputEventCtrl.GetInstance().OnClickFireBt(index, ButtonState.DOWN);
                InputEventCtrl.GetInstance().OnClickDaoDanBt(index, ButtonState.DOWN);
            }
        }
    }

    private void ClickTVYaoKongRightBtEvent(ButtonState val)
    {
        if (m_GmTVLoginDt != null)
        {
            int index = m_GmTVLoginDt.Index;
            InputEventCtrl.GetInstance().OnClickFangXiangRBt(index, val);

            if (val == ButtonState.DOWN)
            {
                InputEventCtrl.GetInstance().OnClickFireBt(index, ButtonState.DOWN);
                InputEventCtrl.GetInstance().OnClickDaoDanBt(index, ButtonState.DOWN);
            }
        }
    }

    public class GameWeiXinLoginData
    {
        /// <summary>
        /// 是否登陆微信手柄.
        /// </summary>
        public bool IsLoginWX = false;
        /// <summary>
        /// 是否激活游戏.
        /// </summary>
        public bool IsActiveGame = false;
        /// <summary>
        /// 游戏外设操作设备.
        /// </summary>
        public GamePadType m_GamePadType = GamePadType.Null;
    }
    /// <summary>
    /// 游戏微信手柄登陆信息.
    /// </summary>
    public GameWeiXinLoginData[] m_GmWXLoginDt = new GameWeiXinLoginData[4];

    public enum GamePadType
    {
        Null,
        /// <summary>
        /// 电视遥控器.
        /// </summary>
        TV_YaoKongQi,
        /// <summary>
        /// 微信虚拟手柄.
        /// </summary>
        WeiXin_ShouBing,
    }

    /// <summary>
    /// 电视遥控器激活玩家时的数据信息.
    /// </summary>
    public class TVYaoKongPlayerData
    {
        /// <summary>
        /// 最后一个血值耗尽的玩家索引信息.
        /// </summary>
        public int Index = -1;
        /// <summary>
        /// 游戏外设操作设备.
        /// </summary>
        public GamePadType m_GamePadType = GamePadType.Null;
        public TVYaoKongPlayerData(int indexVal, GamePadType pad)
        {
            Index = indexVal;
            m_GamePadType = pad;
        }

        public void Reset()
        {
            Index = -1;
            m_GamePadType = GamePadType.Null;
        }
    }
    /// <summary>
    /// 遥控器确定键激活玩家数据列表.
    /// 按照谁最后挂掉优先激活谁的顺序排列.
    /// </summary>
    List<TVYaoKongPlayerData> m_TVYaoKongPlayerDt = new List<TVYaoKongPlayerData>();
    /// <summary>
    /// 游戏电视遥控器登陆的玩家信息.
    /// </summary>
    public TVYaoKongPlayerData m_GmTVLoginDt;

    void ClickTVYaoKongEnterBtEvent(ButtonState val)
    {
        if (val == ButtonState.UP)
        {
            Debug.Log("Unity: pcvr -> ClickTVYaoKongEnterBtEvent...");
            int count = m_TVYaoKongPlayerDt.Count;
            if (count > 0)
            {
                TVYaoKongPlayerData playerDt = m_TVYaoKongPlayerDt[count - 1];
                int indexPlayer = playerDt.Index;
                //清理最后一个血值耗尽的玩家信息.
                m_TVYaoKongPlayerDt.RemoveAt(count - 1);

                if (indexPlayer > -1 && indexPlayer < 4)
                {
                    switch (playerDt.m_GamePadType)
                    {
                        case GamePadType.TV_YaoKongQi:
                            {
                                if (m_GmWXLoginDt[indexPlayer].IsLoginWX)
                                {
                                    if (!m_GmWXLoginDt[indexPlayer].IsActiveGame)
                                    {
                                        Debug.Log("Unity: click TVYaoKong EnterBt -> active TV_YaoKongQi " + indexPlayer + " player!");
                                        m_GmWXLoginDt[indexPlayer].IsActiveGame = true;
                                        InputEventCtrl.GetInstance().OnClickGameStartBt(indexPlayer);
                                    }
                                }
                                break;
                            }
                        case GamePadType.WeiXin_ShouBing:
                            {
                                if (m_GmWXLoginDt[indexPlayer].IsLoginWX)
                                {
                                    if (!m_GmWXLoginDt[indexPlayer].IsActiveGame)
                                    {
                                        Debug.Log("Unity: click TVYaoKong EnterBt -> active " + indexPlayer + " player!");
                                        m_GmWXLoginDt[indexPlayer].IsActiveGame = true;
                                        InputEventCtrl.GetInstance().OnClickGameStartBt(indexPlayer);
                                    }
                                }
                                break;
                            }
                    }
                }
            }
            else
            {
                if (SSUIRoot.GetInstance().m_ExitUICom == null)
                {
                    //没有退出游戏界面出现时,可以用遥控器进入游戏.
                    if (m_GmTVLoginDt == null)
                    {
                        //遥控器激活玩家.
                        int index = GetActivePlayerIndex();
                        if (index < 4 && index > -1)
                        {
                            Debug.Log("Unity: click TVYaoKong EnterBt -> --> active TV_YaoKongQi " + index + " player!");
                            m_GmWXLoginDt[index].IsLoginWX = true;
                            m_GmWXLoginDt[index].IsActiveGame = true;
                            m_GmWXLoginDt[index].m_GamePadType = GamePadType.TV_YaoKongQi;
                            m_PlayerHeadUrl[index] = "";
                            m_GmTVLoginDt = new TVYaoKongPlayerData(index, GamePadType.TV_YaoKongQi);
                            InputEventCtrl.GetInstance().OnClickGameStartBt(index);
                        }
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class GamePlayerData
    {
        /// <summary>
        /// 玩家在游戏中的索引.
        /// </summary>
        public int Index = -1;
        /// <summary>
        /// 是否退出微信.
        /// </summary>
        public bool IsExitWeiXin = false;
        /// <summary>
        /// 玩家的微信数据信息.
        /// </summary>
        public WebSocketSimpet.PlayerWeiXinData m_PlayerWeiXinData;
    }
    public List<GamePlayerData> m_GamePlayerData = new List<GamePlayerData>();

    /// <summary>
    /// 玩家激活游戏状态.
    /// </summary>
    enum PlayerActiveState
    {
        WeiJiHuo = 0, //未激活.
        JiHuo = 1, //激活.
    }

    /// <summary>
    /// 红点点微信手柄发射按键.
    /// </summary>
    enum PlayerShouBingFireBt
    {
        fireXDown,
        fireYDown,
        fireADown,
        fireBDown,
        fireXUp,
        fireYUp,
        fireAUp,
        fireBUp,
    }

    /// <summary>
    /// 红点点微信手柄方向.
    /// </summary>
    enum PlayerShouBingDir
    {
        up = 0, //没有操作方向盘(手指离开摇杆).
        DirLeft = 1,
        DirLeftDown = 2,
        DirDown = 3,
        DirRightDown = 4,
        DirRight = 5,
        DirRightUp = 6,
        DirUp = 7,
        DirLeftUp = 8,
    }

    /// <summary>
    /// 4个玩家激活游戏的列表状态(0 未激活, 1 激活).
    /// </summary>
    [HideInInspector]
    public byte[] m_IndexPlayerActiveGameState = new byte[4];
    public void SetIndexPlayerActiveGameState(int index, byte activeState)
    {
        //if (XkPlayerCtrl.GetInstanceFeiJi() != null
        //    && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage != null
        //    && XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage != null)
        //{
        //    if (activeState == (int)PlayerActiveState.JiHuo)
        //    {
        //        PlayerEnum indexPlayer = (PlayerEnum)(index + 1);
        //        XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.SetPlayerCoinTimeActive(indexPlayer);
        //    }
        //}

        //m_IndexPlayerActiveGameState[index] = activeState;
        //if (activeState == (int)PlayerActiveState.WeiJiHuo)
        //{
        //    GamePlayerData playerDt = m_GamePlayerData.Find((dt) => { return dt.Index.Equals(index); });
        //    if (playerDt != null)
        //    {
        //        if (playerDt.IsExitWeiXin)
        //        {
        //            //玩家已经退出微信.
        //            Debug.Log("Unity:"+"player have exit weiXin! clean the player data...");
        //            m_GamePlayerData.Remove(playerDt);
        //        }
        //        else
        //        {
        //            //玩家血值耗尽应该续费了,找到玩家数据.
        //            Debug.Log("Unity:"+"player should buy game coin!");
        //        }
        //    }

        //    m_GmWXLoginDt[index].IsActiveGame = false;
        //    if (m_GmWXLoginDt[index].IsLoginWX)
        //    {
        //        Debug.Log("Unity:" + "player m_GamePadType ==  " + m_GmWXLoginDt[index].m_GamePadType);
        //        switch (m_GmWXLoginDt[index].m_GamePadType)
        //        {
        //            case GamePadType.WeiXin_ShouBing:
        //                {
        //                    //微信手柄玩家血值耗尽了.
        //                    m_TVYaoKongPlayerDt.Add(new TVYaoKongPlayerData(index, GamePadType.WeiXin_ShouBing));
        //                    break;
        //                }
        //            case GamePadType.TV_YaoKongQi:
        //                {
        //                    //电视遥控器玩家血值耗尽了.
        //                    m_TVYaoKongPlayerDt.Add(new TVYaoKongPlayerData(index, GamePadType.TV_YaoKongQi));
        //                    if (m_GmTVLoginDt != null)
        //                    {
        //                        //关闭玩家发射子弹的按键消息.
        //                        int indexVal = m_GmTVLoginDt.Index;
        //                        InputEventCtrl.GetInstance().OnClickDaoDanBt(indexVal, ButtonState.UP);
        //                        InputEventCtrl.GetInstance().OnClickFireBt(indexVal, ButtonState.UP);
        //                    }
        //                    break;
        //                }
        //        }
        //    }
        //}
        //else
        //{
        //    if (!IsHongDDShouBing)
        //    {
        //        //软件版本测试用,模拟微信手柄登陆.
        //        m_GmWXLoginDt[index].IsLoginWX = true;
        //        m_GmWXLoginDt[index].m_GamePadType = GamePadType.Null;
        //        Debug.Log("Unity: SetIndexPlayerActiveGameState -> index == " + index);
        //    }
        //}
    }

    /// <summary>
    /// 获取未激活玩家的索引. returnVal == -1 -> 所有玩家都处于激活状态.
    /// </summary>
    int GetActivePlayerIndex()
    {
        int indexPlayer = -1;
        for (int i = 0; i < m_IndexPlayerActiveGameState.Length; i++)
        {
            if (m_IndexPlayerActiveGameState[i] == 0)
            {
                if (!m_GmWXLoginDt[i].IsLoginWX)
                {
                    //未激活且未登陆过微信手柄的玩家索引.
                    indexPlayer = i;
                    break;
                }
            }
        }
        return indexPlayer;
    }

    /// <summary>
    /// 发射按键响应.
    /// </summary>
    private void OnEventActionOperation(string val, int userId)
    {
        //Debug.Log("Unity:"+"pcvr::OnEventActionOperation -> userId " + userId + ", val " + val);
        GamePlayerData playerDt = m_GamePlayerData.Find((dt) => { return dt.m_PlayerWeiXinData.userId.Equals(userId); });
        if (playerDt != null && playerDt.Index > -1 && playerDt.Index < 4)
        {
            //Debug.Log("Unity:"+"OnEventActionOperation -> playerIndex == " + playerDt.Index);
            playerDt.IsExitWeiXin = false;
            if (m_IndexPlayerActiveGameState[playerDt.Index] == (int)PlayerActiveState.JiHuo)
            {
                //处于激活状态的玩家才可以进行游戏操作.
                if (val == PlayerShouBingFireBt.fireADown.ToString()
                    || val == PlayerShouBingFireBt.fireXDown.ToString())
                {
                    InputEventCtrl.GetInstance().OnClickFireBt(playerDt.Index, ButtonState.DOWN);
                }

                if (val == PlayerShouBingFireBt.fireAUp.ToString()
                    || val == PlayerShouBingFireBt.fireXUp.ToString())
                {
                    InputEventCtrl.GetInstance().OnClickFireBt(playerDt.Index, ButtonState.UP);
                }

                if (val == PlayerShouBingFireBt.fireBDown.ToString()
                    || val == PlayerShouBingFireBt.fireYDown.ToString())
                {
                    InputEventCtrl.GetInstance().OnClickDaoDanBt(playerDt.Index, ButtonState.DOWN);
                }

                if (val == PlayerShouBingFireBt.fireBUp.ToString()
                    || val == PlayerShouBingFireBt.fireYUp.ToString())
                {
                    InputEventCtrl.GetInstance().OnClickDaoDanBt(playerDt.Index, ButtonState.UP);
                }
            }
        }
    }

    private void OnEventDirectionAngle(string dirValStr, int userId)
    {
        //Debug.Log("Unity:"+"pcvr::OnEventDirectionAngle -> userId " + userId + ", val " + val);
        GamePlayerData playerDt = m_GamePlayerData.Find((dt) => { return dt.m_PlayerWeiXinData.userId.Equals(userId); });
        if (playerDt != null && playerDt.Index > -1 && playerDt.Index < 4)
        {
            //Debug.Log("Unity:"+"OnEventDirectionAngle -> playerIndex == " + playerDt.Index);
            playerDt.IsExitWeiXin = false;
            if (m_IndexPlayerActiveGameState[playerDt.Index] == (int)PlayerActiveState.JiHuo)
            {
                //处于激活状态的玩家才可以进行游戏操作.
                int dirVal = 0;
                if (dirValStr == PlayerShouBingDir.up.ToString())
                {
                    //玩家手指离开摇杆(摇杆回中了).
                }
                else
                {
                    int val = Convert.ToInt32(dirValStr);
                    if (val >= -22.5f && val < 22.5f)
                    {
                        dirVal = 1;
                    }
                    else if (val >= -67.5f && val < -22.5f)
                    {
                        dirVal = 2;
                    }
                    else if (val >= -112.5f && val < -67.5f)
                    {
                        dirVal = 3;
                    }
                    else if (val >= -157.5f && val < -112.5f)
                    {
                        dirVal = 4;
                    }
                    else if ((val >= -180f && val < -157.5f)
                        || (val <= 180f && val > 157.5f))
                    {
                        dirVal = 5;
                    }
                    else if (val > 112.5f && val <= 157.5f)
                    {
                        dirVal = 6;
                    }
                    else if (val > 67.5f && val <= 112.5f)
                    {
                        dirVal = 7;
                    }
                    else if (val >= 22.5f && val <= 67.5f)
                    {
                        dirVal = 8;
                    }
                }

                PlayerShouBingDir dirState = (PlayerShouBingDir)dirVal;
                //if (dirState != PlayerShouBingDir.up)
                //{
                //    //自动回中摇杆.
                //    StopCoroutine(DelayResetPlayerShouBingDir(playerDt.Index));
                //    StartCoroutine(DelayResetPlayerShouBingDir(playerDt.Index));
                //}

                switch (dirState)
                {
                    case PlayerShouBingDir.DirLeft:
                        {
                            InputEventCtrl.GetInstance().OnClickFangXiangUBt(playerDt.Index, ButtonState.UP);
                            InputEventCtrl.GetInstance().OnClickFangXiangDBt(playerDt.Index, ButtonState.UP);
                            InputEventCtrl.GetInstance().OnClickFangXiangLBt(playerDt.Index, ButtonState.DOWN);
                            InputEventCtrl.GetInstance().OnClickFangXiangRBt(playerDt.Index, ButtonState.UP);
                            break;
                        }
                    case PlayerShouBingDir.DirLeftDown:
                        {
                            InputEventCtrl.GetInstance().OnClickFangXiangUBt(playerDt.Index, ButtonState.UP);
                            InputEventCtrl.GetInstance().OnClickFangXiangDBt(playerDt.Index, ButtonState.DOWN);
                            InputEventCtrl.GetInstance().OnClickFangXiangLBt(playerDt.Index, ButtonState.DOWN);
                            InputEventCtrl.GetInstance().OnClickFangXiangRBt(playerDt.Index, ButtonState.UP);
                            break;
                        }
                    case PlayerShouBingDir.DirDown:
                        {
                            InputEventCtrl.GetInstance().OnClickFangXiangUBt(playerDt.Index, ButtonState.UP);
                            InputEventCtrl.GetInstance().OnClickFangXiangDBt(playerDt.Index, ButtonState.DOWN);
                            InputEventCtrl.GetInstance().OnClickFangXiangLBt(playerDt.Index, ButtonState.UP);
                            InputEventCtrl.GetInstance().OnClickFangXiangRBt(playerDt.Index, ButtonState.UP);
                            break;
                        }
                    case PlayerShouBingDir.DirRightDown:
                        {
                            InputEventCtrl.GetInstance().OnClickFangXiangUBt(playerDt.Index, ButtonState.UP);
                            InputEventCtrl.GetInstance().OnClickFangXiangDBt(playerDt.Index, ButtonState.DOWN);
                            InputEventCtrl.GetInstance().OnClickFangXiangLBt(playerDt.Index, ButtonState.UP);
                            InputEventCtrl.GetInstance().OnClickFangXiangRBt(playerDt.Index, ButtonState.DOWN);
                            break;
                        }
                    case PlayerShouBingDir.DirRight:
                        {
                            InputEventCtrl.GetInstance().OnClickFangXiangUBt(playerDt.Index, ButtonState.UP);
                            InputEventCtrl.GetInstance().OnClickFangXiangDBt(playerDt.Index, ButtonState.UP);
                            InputEventCtrl.GetInstance().OnClickFangXiangLBt(playerDt.Index, ButtonState.UP);
                            InputEventCtrl.GetInstance().OnClickFangXiangRBt(playerDt.Index, ButtonState.DOWN);
                            break;
                        }
                    case PlayerShouBingDir.DirRightUp:
                        {
                            InputEventCtrl.GetInstance().OnClickFangXiangUBt(playerDt.Index, ButtonState.DOWN);
                            InputEventCtrl.GetInstance().OnClickFangXiangDBt(playerDt.Index, ButtonState.UP);
                            InputEventCtrl.GetInstance().OnClickFangXiangLBt(playerDt.Index, ButtonState.UP);
                            InputEventCtrl.GetInstance().OnClickFangXiangRBt(playerDt.Index, ButtonState.DOWN);
                            break;
                        }
                    case PlayerShouBingDir.DirUp:
                        {
                            InputEventCtrl.GetInstance().OnClickFangXiangUBt(playerDt.Index, ButtonState.DOWN);
                            InputEventCtrl.GetInstance().OnClickFangXiangDBt(playerDt.Index, ButtonState.UP);
                            InputEventCtrl.GetInstance().OnClickFangXiangLBt(playerDt.Index, ButtonState.UP);
                            InputEventCtrl.GetInstance().OnClickFangXiangRBt(playerDt.Index, ButtonState.UP);
                            break;
                        }
                    case PlayerShouBingDir.DirLeftUp:
                        {
                            InputEventCtrl.GetInstance().OnClickFangXiangUBt(playerDt.Index, ButtonState.DOWN);
                            InputEventCtrl.GetInstance().OnClickFangXiangDBt(playerDt.Index, ButtonState.UP);
                            InputEventCtrl.GetInstance().OnClickFangXiangLBt(playerDt.Index, ButtonState.DOWN);
                            InputEventCtrl.GetInstance().OnClickFangXiangRBt(playerDt.Index, ButtonState.UP);
                            break;
                        }
                    case PlayerShouBingDir.up:
                        {
                            InputEventCtrl.GetInstance().OnClickFangXiangUBt(playerDt.Index, ButtonState.UP);
                            InputEventCtrl.GetInstance().OnClickFangXiangDBt(playerDt.Index, ButtonState.UP);
                            InputEventCtrl.GetInstance().OnClickFangXiangLBt(playerDt.Index, ButtonState.UP);
                            InputEventCtrl.GetInstance().OnClickFangXiangRBt(playerDt.Index, ButtonState.UP);
                            break;
                        }
                }
            }
        }
    }
    
    IEnumerator DelayResetPlayerShouBingDir(int index)
    {
        yield return new WaitForSeconds(1f);
        InputEventCtrl.GetInstance().OnClickFangXiangUBt(index, ButtonState.UP);
        InputEventCtrl.GetInstance().OnClickFangXiangDBt(index, ButtonState.UP);
        InputEventCtrl.GetInstance().OnClickFangXiangLBt(index, ButtonState.UP);
        InputEventCtrl.GetInstance().OnClickFangXiangRBt(index, ButtonState.UP);
    }

    private void OnEventPlayerExitBox(int userId)
    {
        Debug.Log("Unity:"+"pcvr::OnEventPlayerExitBox -> userId " + userId);
        //GamePlayerData playerDt = m_GamePlayerData.Find((dt) => { return dt.m_PlayerWeiXinData.userId.Equals(userId); });
        //if (playerDt != null)
        //{
        //    playerDt.IsExitWeiXin = true;
        //    if (playerDt.Index > -1 && playerDt.Index < 4)
        //    {
        //        if (m_IndexPlayerActiveGameState[playerDt.Index] == (int)PlayerActiveState.WeiJiHuo)
        //        {
        //            //玩家血值耗尽,清理玩家微信数据.
        //            m_GamePlayerData.Remove(playerDt);
        //            Debug.Log("Unity:"+"OnEventPlayerExitBox -> clear player weiXin data...");
        //        }
        //    }
        //}
    }

    public void ClearGameWeiXinData()
    {
        //Debug.Log("Unity: ClearGameWeiXinData...");
        //if (m_GamePlayerData != null)
        //{
        //    m_GamePlayerData.Clear();
        //}

        //for (int i = 0; i < m_GmWXLoginDt.Length; i++)
        //{
        //    m_GmWXLoginDt[i].IsLoginWX = false;
        //    m_GmWXLoginDt[i].IsActiveGame = false;
        //    m_GmWXLoginDt[i].m_GamePadType = GamePadType.Null;
        //}
        //m_TVYaoKongPlayerDt.Clear();

        //if (m_GmTVLoginDt != null)
        //{
        //    m_GmTVLoginDt.Reset();
        //    m_GmTVLoginDt = null;
        //}
    }

    private void OnEventPlayerLoginBox(WebSocketSimpet.PlayerWeiXinData val)
    {
        Debug.Log("Unity:"+"pcvr::OnEventPlayerLoginBox -> userName " + val.userName + ", userId " + val.userId);
        GamePlayerData playerDt = m_GamePlayerData.Find((dt) => {
            if (dt.m_PlayerWeiXinData != null)
            {
                return dt.m_PlayerWeiXinData.userId.Equals(val.userId);
            }
            return dt.m_PlayerWeiXinData.Equals(val); });

        int indexPlayer = -1;
        bool isActivePlayer = false;
        if (playerDt == null)
        {
            indexPlayer = GetActivePlayerIndex();
            if (indexPlayer > -1 && indexPlayer < 4)
            {
                Debug.Log("Unity:"+"Active player, indexPlayer == " + indexPlayer);
                playerDt = new GamePlayerData();
                playerDt.m_PlayerWeiXinData = val;
                playerDt.Index = indexPlayer;
                m_GamePlayerData.Add(playerDt);
                isActivePlayer = true;
                m_GmWXLoginDt[indexPlayer].IsLoginWX = true;
                m_GmWXLoginDt[indexPlayer].m_GamePadType = GamePadType.WeiXin_ShouBing;
            }
            else
            {
                Debug.Log("Unity:"+"have not empty player!");
            }
        }
        else
        {
            Debug.Log("Unity:"+"player have active game!");
            playerDt.IsExitWeiXin = false;
            if (playerDt.Index > -1 && playerDt.Index < 4)
            {
                PlayerActiveState playerSt = (PlayerActiveState)m_IndexPlayerActiveGameState[playerDt.Index];
                switch (playerSt)
                {
                    case PlayerActiveState.WeiJiHuo:
                        {
                            isActivePlayer = true;
                            indexPlayer = playerDt.Index;
                            m_GmWXLoginDt[indexPlayer].IsLoginWX = true;
                            m_GmWXLoginDt[indexPlayer].m_GamePadType = GamePadType.WeiXin_ShouBing;
                            break;
                        }
                    case PlayerActiveState.JiHuo:
                        {
                            GamePlayerData playerTmpDt = m_GamePlayerData.Find((dt) =>
                            {
                                return dt.Index.Equals(playerDt.Index);
                            });
                            
                            if (playerTmpDt != null
                                && playerTmpDt.m_PlayerWeiXinData != null
                                && val.userId == playerTmpDt.m_PlayerWeiXinData.userId)
                            {
                                Debug.Log("Unity: " + playerTmpDt.m_PlayerWeiXinData.userName + " have active " + " player!");
                            }
                            else
                            {
                                indexPlayer = GetActivePlayerIndex();
                                if (indexPlayer > -1 && indexPlayer < 4)
                                {
                                    Debug.Log("Unity: player active -> indexPlayer == " + indexPlayer);
                                    isActivePlayer = true;
                                    playerDt.Index = indexPlayer;
                                    m_GmWXLoginDt[indexPlayer].IsLoginWX = true;
                                    m_GmWXLoginDt[indexPlayer].m_GamePadType = GamePadType.WeiXin_ShouBing;
                                }
                                else
                                {
                                    Debug.Log("Unity:" + " ---> have not empty player!");
                                }
                            }
                            break;
                        }
                }
            }
        }

        if (isActivePlayer)
        {
            m_GmWXLoginDt[indexPlayer].IsActiveGame = true;
            m_PlayerHeadUrl[indexPlayer] = playerDt.m_PlayerWeiXinData.headUrl;
            InputEventCtrl.GetInstance().OnClickGameStartBt(indexPlayer);
        }
    }

    public string[] m_PlayerHeadUrl = new string[4];

    #region pcvr old code
#if OLD_PCVR_CODE
    // Update is called once per frame
    void Update()
	{
		UpdateZuoYiQiNangState();
		if (!bIsHardWare || XkGameCtrl.IsLoadingLevel) {
			return;
		}

		CheckIsPlayerActivePcvr();
		if (!TestTanKCom.IsTestTankCom && TKMoveSt == TKMoveState.U_FangXiangPan) {
			GetPcvrSteerVal();
			GetPcvrYouMenVal();
		}
		
		float dTime = Time.realtimeSinceStartup - lastUpTime;
		if (IsJiaoYanHid) {
			if (dTime < 0.1f) {
				return;
			}
		}
		else {
			if (dTime < 0.03f) {
				return;
			}
		}
		lastUpTime = Time.realtimeSinceStartup;
		
		GetMessage();
		SendMessage();
	}

//	static byte ReadHead_1 = 0x01;
//	static byte ReadHead_2 = 0x55;
	static byte WriteHead_1 = 0x02;
	static byte WriteHead_2 = 0x55;
	static byte WriteEnd_1 = 0x0d;
	static byte WriteEnd_2 = 0x0a;
	byte EndRead_1 = 0x41;
	byte EndRead_2 = 0x42;
	byte EndRead_3 = 0x43;
	byte EndRead_4 = 0x44;
	static void CheckMovePlayerZuoYi(PlayerEnum indexPlayer = PlayerEnum.Null)
	{
		switch (indexPlayer) {
		case PlayerEnum.Null:
			CheckPlayerZuoYiState(PlayerEnum.PlayerOne);
			CheckPlayerZuoYiState(PlayerEnum.PlayerTwo);
			CheckPlayerZuoYiState(PlayerEnum.PlayerThree);
			CheckPlayerZuoYiState(PlayerEnum.PlayerFour);
			break;
		default:
			CheckPlayerZuoYiState(indexPlayer);
			break;
		}
	}

	static void CheckPlayerZuoYiState(PlayerEnum indexPlayer)
	{
		bool isMoveZuoYi = false;
		int indexVal = (int)indexPlayer - 1;
		int minVal = indexVal * 4;
		int maxVal = 4 + (indexVal * 4);
		for (int i = minVal; i < maxVal; i++) {
			if (QiNangArray[i] == 1) {
				isMoveZuoYi = true;
				break;
			} 
		}
		
		if (isMoveZuoYi) {
			if (ZuoYiDianJiSpeedVal[indexVal] == 0x00) {
				_Instance.SetZuoYiDianJiSpeed(indexPlayer, 1);
			}
		}
		else {
			if (RunZuoYiState[indexVal] == 0x00) {
				if (_Instance != null) {
					_Instance.SetZuoYiDianJiSpeed(indexPlayer, 0);
				}
			}
		}
	}

	/**
****************.显示器.****************
QiNangArray[0]			QiNangArray[1]
QiNangArray[3]			QiNangArray[2]
* 0-3   16 --> 1P.
* 4-7   17 --> 2P.
* 8-11  18 --> 3P.
* 12-15 19 --> 4P.
	 */
	public static byte[] QiNangArray = {0, 0, 0, 0,
										0, 0, 0, 0,
										0, 0, 0, 0,
										0, 0, 0, 0,
										0, 0, 0, 0};
	/**
	 * key == 0 -> 关闭动感.
	 * key == 1 -> 关闭气囊,使座椅运动到最低位置.
	 */
	public static void CloseAllQiNangArray(PlayerEnum indexPlayer = PlayerEnum.Null, int key = 0)
	{
		int indexVal = (int)indexPlayer - 1;
		int minVal = indexVal * 4;
		int maxVal = 4 + (indexVal * 4);
		if (indexPlayer == PlayerEnum.Null) {
			minVal = 0;
			maxVal = QiNangArray.Length;
		}
		else {
			int indexTmp = indexVal + 16;
			QiNangArray[indexTmp] = 0;
		}

		for (int i = minVal; i < maxVal;  i++) {
			QiNangArray[i] = 0;
		}

		switch(key) {
		case 0:
			DongGanState = 0;
			CheckMovePlayerZuoYi();
			break;
		case 1:
			if (_Instance != null) {
				if (indexPlayer == PlayerEnum.Null) {
					_Instance.SetZuoYiDianJiSpeed(PlayerEnum.PlayerOne, 0);
					_Instance.SetZuoYiDianJiSpeed(PlayerEnum.PlayerTwo, 0);
					_Instance.SetZuoYiDianJiSpeed(PlayerEnum.PlayerThree, 0);
					_Instance.SetZuoYiDianJiSpeed(PlayerEnum.PlayerFour, 0);
				}
				else {
					_Instance.SetZuoYiDianJiSpeed(indexPlayer, 0);
				}
			}
			break;
		}
	}

	public static void OpenDongGanState()
	{
		DongGanState = 1;
	}
	
	
	public static void CloseDongGanState()
	{
		CloseAllQiNangArray();
	}

	public static void OpenZuoYiQiNang(PlayerEnum indexPlayer)
	{
		if (indexPlayer == PlayerEnum.Null || DongGanState == 0) {
			return;
		}

		if (XKGlobalData.GameVersionPlayer != 0) {
			switch (indexPlayer) {
			case PlayerEnum.PlayerThree:
				indexPlayer = PlayerEnum.PlayerOne;
				break;
			case PlayerEnum.PlayerFour:
				indexPlayer = PlayerEnum.PlayerTwo;
				break;
			}
		}

		switch (indexPlayer) {
		case PlayerEnum.PlayerOne:
			if (XKGlobalData.GameVersionPlayer == 0) {
				if (!XkGameCtrl.IsActivePlayerOne) {
					return;
				}
			}
			else {
				if (!XkGameCtrl.IsActivePlayerThree) {
					return;
				}
			}
			break;
		case PlayerEnum.PlayerTwo:
			if (XKGlobalData.GameVersionPlayer == 0) {
				if (!XkGameCtrl.IsActivePlayerTwo) {
					return;
				}
			}
			else {
				if (!XkGameCtrl.IsActivePlayerFour) {
					return;
				}
			}
			break;
		case PlayerEnum.PlayerThree:
			if (!XkGameCtrl.IsActivePlayerThree) {
				return;
			}
			break;
		case PlayerEnum.PlayerFour:
			if (!XkGameCtrl.IsActivePlayerFour) {
				return;
			}
			break;
		}
		int indexVal = (int)indexPlayer - 1 + 16;
		QiNangArray[indexVal] = 1;
		ZuoYiQNTime[indexVal-16] = Time.time;
		//Debug.LogWarning("Unity:"+"OpenZuoYiQiNang -> indexVal "+indexVal);
	}

	static float TimeZuoYiQNCheck;
	void UpdateZuoYiQiNangState()
	{
		if (HardwareCheckCtrl.IsTestHardWare) {
			return;
		}

		if (DongGanState == 0 || Time.time - TimeZuoYiQNCheck < 0.2f) {
			return;
		}
		TimeZuoYiQNCheck = Time.time;

		int indexVal = 0;
		for (int i = 0; i < 4; i++) {
			indexVal = i + 16;
			if (QiNangArray[indexVal] == 0 || Time.time - ZuoYiQNTime[indexVal-16] < 1f) {
				continue;
			}
			QiNangArray[indexVal] = 0;
//			Debug.LogWarning("Unity:"+"UpdateZuoYiQiNangState -> indexVal "+indexVal);
		}
	}

	static float[] ZuoYiQNTime = new float[4];
	static bool[] IsOpenQiNangQian = new bool[4];
	static bool[] IsOpenQiNangHou = new bool[4];
	static bool[] IsOpenQiNangZuo = new bool[4];
	static bool[] IsOpenQiNangYou = new bool[4];
	public static void OpenQiNangQian(PlayerEnum indexPlayer)
	{
		PlayerEnum indexPlayerTmp = indexPlayer;
		if (XKGlobalData.GameVersionPlayer != 0) {
			switch (indexPlayer) {
			case PlayerEnum.PlayerThree:
				indexPlayer = PlayerEnum.PlayerOne;
				break;
			case PlayerEnum.PlayerFour:
				indexPlayer = PlayerEnum.PlayerTwo;
				break;
			}
		}

		int indexVal = (int)indexPlayer - 1;
		if (IsOpenQiNangQian[indexVal]) {
			return;
		}
		IsOpenQiNangQian[indexVal] = true;
		
		byte qnState = (byte)(XkGameCtrl.GetIsActivePlayer(indexPlayer) == true ? 1 : 0);
		if (XKGlobalData.GameVersionPlayer != 0) {
			qnState = (byte)(XkGameCtrl.GetIsActivePlayer(indexPlayerTmp) == true ? 1 : 0);
		}
		indexVal *= 4;
		int indexA = 0 + indexVal;
		int indexB = 1 + indexVal;
		QiNangArray[indexA] = qnState;
		QiNangArray[indexB] = qnState;
		CheckMovePlayerZuoYi(indexPlayer);
	}
	
	/**
	 * key == 0 -> 关闭前边全部气囊.
	 * key == 1 -> 关闭前边的左气囊.
	 * key == 2 -> 关闭前边的右气囊.
	 */
	public static void CloseQiNangQian(PlayerEnum indexPlayer, int key = 0)
	{
		if (XKGlobalData.GameVersionPlayer != 0) {
			switch (indexPlayer) {
			case PlayerEnum.PlayerThree:
				indexPlayer = PlayerEnum.PlayerOne;
				break;
			case PlayerEnum.PlayerFour:
				indexPlayer = PlayerEnum.PlayerTwo;
				break;
			}
		}

		int indexVal = (int)indexPlayer - 1;
		if (!IsOpenQiNangQian[indexVal]) {
			return;
		}
		IsOpenQiNangQian[indexVal] = false;

		indexVal *= 4;
		switch (key) {
		case 1:
			QiNangArray[indexVal] = 0;
			break;
		case 2:
			QiNangArray[indexVal + 1] = 0;
			break;
		default:
			QiNangArray[indexVal] = 0;
			QiNangArray[indexVal + 1] = 0;
			break;
		}
		CheckMovePlayerZuoYi(indexPlayer);
	}
	
	public static void OpenQiNangHou(PlayerEnum indexPlayer)
	{
		PlayerEnum indexPlayerTmp = indexPlayer;
		if (XKGlobalData.GameVersionPlayer != 0) {
			switch (indexPlayer) {
			case PlayerEnum.PlayerThree:
				indexPlayer = PlayerEnum.PlayerOne;
				break;
			case PlayerEnum.PlayerFour:
				indexPlayer = PlayerEnum.PlayerTwo;
				break;
			}
		}

		int indexVal = (int)indexPlayer - 1;
		if (IsOpenQiNangHou[indexVal]) {
			return;
		}
		IsOpenQiNangHou[indexVal] = true;
		
		byte qnState = (byte)(XkGameCtrl.GetIsActivePlayer(indexPlayer) == true ? 1 : 0);
		if (XKGlobalData.GameVersionPlayer != 0) {
			qnState = (byte)(XkGameCtrl.GetIsActivePlayer(indexPlayerTmp) == true ? 1 : 0);
		}
		indexVal *= 4;
		int indexA = 3 + indexVal;
		int indexB = 2 + indexVal;
		QiNangArray[indexA] = qnState;
		QiNangArray[indexB] = qnState;
		CheckMovePlayerZuoYi(indexPlayer);
	}
	
	/**
	 * key == 0 -> 关闭前边全部气囊.
	 * key == 1 -> 关闭前边的左气囊.
	 * key == 2 -> 关闭前边的右气囊.
	 */
	public static void CloseQiNangHou(PlayerEnum indexPlayer, int key = 0)
	{
		if (XKGlobalData.GameVersionPlayer != 0) {
			switch (indexPlayer) {
			case PlayerEnum.PlayerThree:
				indexPlayer = PlayerEnum.PlayerOne;
				break;
			case PlayerEnum.PlayerFour:
				indexPlayer = PlayerEnum.PlayerTwo;
				break;
			}
		}

		int indexVal = (int)indexPlayer - 1;
		if (!IsOpenQiNangHou[indexVal]) {
			return;
		}
		IsOpenQiNangHou[indexVal] = false;

		indexVal = 3 + (indexVal * 4);
		switch (key) {
		case 1:
			QiNangArray[indexVal] = 0;
			break;
		case 2:
			QiNangArray[indexVal - 1] = 0;
			break;
		default:
			QiNangArray[indexVal] = 0;
			QiNangArray[indexVal - 1] = 0;
			break;
		}
		CheckMovePlayerZuoYi(indexPlayer);
	}
	
	public static void OpenQiNangZuo(PlayerEnum indexPlayer)
	{
		PlayerEnum indexPlayerTmp = indexPlayer;
		if (XKGlobalData.GameVersionPlayer != 0) {
			switch (indexPlayer) {
			case PlayerEnum.PlayerThree:
				indexPlayer = PlayerEnum.PlayerOne;
				break;
			case PlayerEnum.PlayerFour:
				indexPlayer = PlayerEnum.PlayerTwo;
				break;
			}
		}

		int indexVal = (int)indexPlayer - 1;
		if (IsOpenQiNangZuo[indexVal]) {
			return;
		}
		IsOpenQiNangZuo[indexVal] = true;

		byte qnState = (byte)(XkGameCtrl.GetIsActivePlayer(indexPlayer) == true ? 1 : 0);
		if (XKGlobalData.GameVersionPlayer != 0) {
			qnState = (byte)(XkGameCtrl.GetIsActivePlayer(indexPlayerTmp) == true ? 1 : 0);
		}
		indexVal *= 4;
		int indexA = 0 + indexVal;
		int indexB = 3 + indexVal;
		QiNangArray[indexA] = qnState;
		QiNangArray[indexB] = qnState;
		CheckMovePlayerZuoYi(indexPlayer);
	}

	/**
	 * key == 0 -> 关闭左边全部气囊.
	 * key == 1 -> 关闭左边的前气囊.
	 * key == 2 -> 关闭左边的后气囊.
	 */
	public static void CloseQiNangZuo(PlayerEnum indexPlayer, int key = 0)
	{
		if (XKGlobalData.GameVersionPlayer != 0) {
			switch (indexPlayer) {
			case PlayerEnum.PlayerThree:
				indexPlayer = PlayerEnum.PlayerOne;
				break;
			case PlayerEnum.PlayerFour:
				indexPlayer = PlayerEnum.PlayerTwo;
				break;
			}
		}

		int indexVal = (int)indexPlayer - 1;
		if (!IsOpenQiNangZuo[indexVal]) {
			return;
		}
		IsOpenQiNangZuo[indexVal] = false;

		indexVal *= 4;
		switch (key) {
		case 1:
			QiNangArray[indexVal] = 0;
			break;
		case 2:
			QiNangArray[indexVal + 3] = 0;
			break;
		default:
			QiNangArray[indexVal] = 0;
			QiNangArray[indexVal + 3] = 0;
			break;
		}
		CheckMovePlayerZuoYi(indexPlayer);
	}
	
	public static void OpenQiNangYou(PlayerEnum indexPlayer)
	{
		PlayerEnum indexPlayerTmp = indexPlayer;
		if (XKGlobalData.GameVersionPlayer != 0) {
			switch (indexPlayer) {
			case PlayerEnum.PlayerThree:
				indexPlayer = PlayerEnum.PlayerOne;
				break;
			case PlayerEnum.PlayerFour:
				indexPlayer = PlayerEnum.PlayerTwo;
				break;
			}
		}

		int indexVal = (int)indexPlayer - 1;
		if (IsOpenQiNangYou[indexVal]) {
			return;
		}
		IsOpenQiNangYou[indexVal] = true;

		byte qnState = (byte)(XkGameCtrl.GetIsActivePlayer(indexPlayer) == true ? 1 : 0);
		if (XKGlobalData.GameVersionPlayer != 0) {
			qnState = (byte)(XkGameCtrl.GetIsActivePlayer(indexPlayerTmp) == true ? 1 : 0);
		}
		indexVal *= 4;
		int indexA = 1 + indexVal;
		int indexB = 2 + indexVal;
		QiNangArray[indexA] = qnState;
		QiNangArray[indexB] = qnState;
		CheckMovePlayerZuoYi(indexPlayer);
	}
	
	/**
	 * key == 0 -> 关闭右边全部气囊.
	 * key == 1 -> 关闭右边的前气囊.
	 * key == 2 -> 关闭右边的后气囊.
	 */
	public static void CloseQiNangYou(PlayerEnum indexPlayer, int key = 0)
	{
		if (XKGlobalData.GameVersionPlayer != 0) {
			switch (indexPlayer) {
			case PlayerEnum.PlayerThree:
				indexPlayer = PlayerEnum.PlayerOne;
				break;
			case PlayerEnum.PlayerFour:
				indexPlayer = PlayerEnum.PlayerTwo;
				break;
			}
		}

		int indexVal = (int)indexPlayer - 1;
		if (!IsOpenQiNangYou[indexVal]) {
			return;
		}
		IsOpenQiNangYou[indexVal] = false;

		indexVal = 1 + (indexVal * 4);
		switch (key) {
		case 1:
			QiNangArray[indexVal] = 0;
			break;
		case 2:
			QiNangArray[indexVal + 1] = 0;
			break;
		default:
			QiNangArray[indexVal] = 0;
			QiNangArray[indexVal + 1] = 0;
			break;
		}
		CheckMovePlayerZuoYi(indexPlayer);
	}
	
	public static bool IsPlayerHitShake;
	public void OnPlayerHitShake(PlayerEnum indexPlayer = PlayerEnum.PlayerOne)
	{
		if (IsPlayerHitShake) {
			return;
		}
		IsPlayerHitShake = true;
		//Debug.Log("Unity:"+"OnPlayerHitShake...");
		StartCoroutine(PcvrQiNangHitShake(indexPlayer));
	}
	
	void ClosePlayerHitShake()
	{
		if (!IsPlayerHitShake) {
			return;
		}
		IsPlayerHitShake = false;
		//Debug.Log("Unity:"+"ClosePlayerHitShake...");
	}
	
	IEnumerator PcvrQiNangHitShake(PlayerEnum indexPlayer)
	{
		bool isHitShake = true;
		int count = 0;
		do {
			if (count % 2 == 0) {
				OpenQiNangZuo(indexPlayer);
				CloseQiNangYou(indexPlayer);
			}
			else {
				OpenQiNangYou(indexPlayer);
				CloseQiNangZuo(indexPlayer);
			}
			yield return new WaitForSeconds(0.25f);
			
			if (count >= 4) {
				isHitShake = false;
				ClosePlayerHitShake();
				yield break;
			}
			count++;
		} while (isHitShake);
	}

	void SendMessage()
	{
		if (!MyCOMDevice.IsFindDeviceDt) {
			return;
		}
		
		byte []buffer;
		buffer = new byte[HID_BUF_LEN_WRITE];
		buffer[0] = WriteHead_1;
		buffer[1] = WriteHead_2;
		buffer[HID_BUF_LEN_WRITE - 2] = WriteEnd_1;
		buffer[HID_BUF_LEN_WRITE - 1] = WriteEnd_2;
		switch (MyCOMDevice.PcvrComSt) {
		case MyCOMDevice.PcvrComState.TanKeFangXiangZhenDong:
		{
#if !COM_TANK_TEST
			for (int i = 5; i < HID_BUF_LEN_WRITE - 2; i++) {
				buffer[i] = (byte)(UnityEngine.Random.Range(0, 10000) % 256);
			}
		
			if (IsSubPlayerCoin) {
				buffer[2] = 0xaa;
				buffer[3] = (byte)SubCoinNum_12;
				buffer[4] = (byte)SubCoinNum_34;
				//ScreenLog.Log("sub coinP12 "+SubCoinNum_12+", coinP34 "+SubCoinNum_34);
			}
		
			switch (StartLightStateP1) {
			case LedState.Liang:
				buffer[5] |= 0x01;
				break;
			
			case LedState.Shan:
				buffer[5] |= 0x01;
				break;
			
			case LedState.Mie:
				buffer[5] &= 0xfe;
				break;
			}
		
			switch (StartLightStateP2) {
			case LedState.Liang:
				buffer[5] |= 0x02;
				break;
			
			case LedState.Shan:
				buffer[5] |= 0x02;
				break;
			
			case LedState.Mie:
				buffer[5] &= 0xfd;
				break;
			}

			switch (StartLightStateP3) {
			case LedState.Liang:
				buffer[5] |= 0x04;
				break;
			
			case LedState.Shan:
				buffer[5] |= 0x04;
				break;
			
			case LedState.Mie:
				buffer[5] &= 0xfb;
				break;
			}

			switch (StartLightStateP4) {
			case LedState.Liang:
				buffer[5] |= 0x08;
				break;
			
			case LedState.Shan:
				buffer[5] |= 0x08;
				break;
			
			case LedState.Mie:
				buffer[5] &= 0xf7;
				break;
			}

			switch (StartLightStateP5) {
			case LedState.Liang:
				buffer[5] |= 0x10;
				break;
			
			case LedState.Shan:
				buffer[5] |= 0x10;
				break;
			
			case LedState.Mie:
				buffer[5] &= 0xef;
				break;
			}
		
			if (DongGanState == 1 || HardwareCheckCtrl.IsTestHardWare) {
				buffer[6] = (byte)(QiNangArray[0]
				                   + (QiNangArray[1] << 1)
				                   + (QiNangArray[2] << 2)
				                   + (QiNangArray[3] << 3)
				                   + (QiNangArray[4] << 4)
				                   + (QiNangArray[5] << 5)
				                   + (QiNangArray[6] << 6)
				                   + (QiNangArray[7] << 7));
				buffer[7] = (byte)(QiNangArray[8]
				                   + (QiNangArray[9] << 1)
				                   + (QiNangArray[10] << 2)
				                   + (QiNangArray[11] << 3)
				                   + (QiNangArray[12] << 4)
				                   + (QiNangArray[13] << 5)
				                   + (QiNangArray[14] << 6)
				                   + (QiNangArray[15] << 7));
			}
			else {
				buffer[6] = 0x00;
				buffer[7] = 0x00;
				if (RunZuoYiState[0] == 0x00) {
					SetRunZuoYiState(PlayerEnum.PlayerOne, 1);
				}
				
				if (RunZuoYiState[1] == 0x00) {
					SetRunZuoYiState(PlayerEnum.PlayerTwo, 1);
				}
				
				if (RunZuoYiState[2] == 0x00) {
					SetRunZuoYiState(PlayerEnum.PlayerThree, 1);
				}
				
				if (RunZuoYiState[3] == 0x00) {
					SetRunZuoYiState(PlayerEnum.PlayerFour, 1);
				}
			}

			if (HardwareCheckCtrl.IsTestHardWare) {
				buffer[8] = HardwareCheckCtrl.GunZhenDongVal[0];
				buffer[9] = HardwareCheckCtrl.GunZhenDongVal[1];
				buffer[10] = HardwareCheckCtrl.GunZhenDongVal[2];
				buffer[11] = HardwareCheckCtrl.GunZhenDongVal[3];
			}
			else {
				buffer[8] = 0x00;
				buffer[9] = 0x00;
				buffer[10] = 0x00;
				buffer[11] = 0x00;
				/*if (Application.loadedLevel != (int)GameLevel.Scene_1) {
					buffer[8] = 0x00;
					buffer[9] = 0x00;
					buffer[10] = 0x00;
					buffer[11] = 0x00;
				}
				else {
					buffer[8] = (byte)(XkGameCtrl.IsActivePlayerOne == false ? 0x00 : 0xaa);
					buffer[9] = (byte)(XkGameCtrl.IsActivePlayerTwo == false ? 0x00 : 0xaa);
                   	buffer[10] = (byte)(XkGameCtrl.IsActivePlayerThree == false ? 0x00 : 0xaa);
                    buffer[11] = (byte)(XkGameCtrl.IsActivePlayerFour == false ? 0x00 : 0xaa);
				}*/
			}

			//气囊17-20.
			buffer[12] = (byte)(QiNangArray[16]
			                   + (QiNangArray[17] << 1)
			                   + (QiNangArray[18] << 2)
			                   + (QiNangArray[19] << 3));

			//玩家是否激活游戏控制.
			if (Application.loadedLevel != (int)GameLevel.Scene_1 && !HardwareCheckCtrl.IsTestHardWare) {
					buffer[13] = 0x00;
			}
			else {
				byte activePlayer1 = (byte)(XkGameCtrl.IsActivePlayerOne == false ? 0x00 : 0x01);
				byte activePlayer2 = (byte)(XkGameCtrl.IsActivePlayerTwo == false ? 0x00 : 0x01);
				byte activePlayer3 = (byte)(XkGameCtrl.IsActivePlayerThree == false ? 0x00 : 0x01);
				byte activePlayer4 = (byte)(XkGameCtrl.IsActivePlayerFour == false ? 0x00 : 0x01);
				
				buffer[13] = (byte)(activePlayer1
				                    + (activePlayer2 << 1)
				                    + (activePlayer3 << 2)
				                    + (activePlayer4 << 3));
			}

			//buffer[12] = ZuoYiDianJiSpeedVal[0];
			//buffer[13] = ZuoYiDianJiSpeedVal[1];
			buffer[14] = ZuoYiDianJiSpeedVal[2];
			buffer[15] = ZuoYiDianJiSpeedVal[3];
			buffer[16] = FangXiangPanDouDongVal[0];
			buffer[17] = FangXiangPanDouDongVal[1];
			buffer[18] = FangXiangPanDouDongVal[2];
			buffer[19] = FangXiangPanDouDongVal[3];

			if (IsJiaoYanHid) {
				for (int i = 0; i < 4; i++) {
					buffer[i + 21] = JiaoYanMiMa[i];
				}
			
				for (int i = 0; i < 4; i++) {
					buffer[i + 25] = JiaoYanDt[i];
				}
			}
			else {
				RandomJiaoYanMiMaVal();
				for (int i = 0; i < 4; i++) {
					buffer[i + 21] = JiaoYanMiMaRand[i];
				}
			
				//0x41 0x42 0x43 0x44
				for (int i = 26; i < 29; i++) {
					buffer[i] = (byte)UnityEngine.Random.Range(0x00, 0x40);
				}
				buffer[25] = 0x00;
			
				for (int i = 26; i < 29; i++) {
					buffer[25] ^= buffer[i];
				}
			}
		
			buffer[20] = 0x00;
			for (int i = 2; i <= 11; i++) {
				buffer[20] ^= buffer[i];
			}

			buffer[29] = 0x00;
			for (int i = 0; i < HID_BUF_LEN_WRITE; i++) {
				if (i == 29) {
					continue;
				}
				buffer[29] ^= buffer[i];
			}
#endif
		}
		break;
		case MyCOMDevice.PcvrComState.TanKeGunZhenDong:{
#if COM_TANK_TEST
			for (int i = 4; i < HID_BUF_LEN_WRITE - 2; i++) {
				buffer[i] = (byte)(UnityEngine.Random.Range(0, 10000) % 256);
			}
			
			if (IsSubPlayerCoin) {
				buffer[2] = 0xaa;
//				buffer[3] = (byte)subCoinNum12;
			}
			
			switch (StartLightStateP1) {
			case LedState.Liang:
				buffer[4] |= 0x01;
				break;
				
			case LedState.Shan:
				buffer[4] |= 0x01;
				break;
				
			case LedState.Mie:
				buffer[4] &= 0xfe;
				break;
			}
			
			switch (StartLightStateP2) {
			case LedState.Liang:
				buffer[4] |= 0x02;
				break;
				
			case LedState.Shan:
				buffer[4] |= 0x02;
				break;
				
			case LedState.Mie:
				buffer[4] &= 0xfd;
				break;
			}
			
			if (DongGanState == 1 || HardwareCheckCtrl.IsTestHardWare) {
				buffer[5] = (byte)(QiNangArray[0]
				                   + (QiNangArray[1] << 1)
				                   + (QiNangArray[2] << 2)
				                   + (QiNangArray[3] << 3)
				                   + (QiNangArray[4] << 4)
				                   + (QiNangArray[5] << 5)
				                   + (QiNangArray[6] << 6)
				                   + (QiNangArray[7] << 7));
			}
			else {
				buffer[5] = 0x00;
			}
			//buffer[5] = 0x00;
			
			if (IsJiaoYanHid) {
				for (int i = 0; i < 4; i++) {
					buffer[i + 10] = JiaoYanMiMa[i];
				}
				
				for (int i = 0; i < 4; i++) {
					buffer[i + 14] = JiaoYanDt[i];
				}
			}
			else {
				RandomJiaoYanMiMaVal();
				for (int i = 0; i < 4; i++) {
					buffer[i + 10] = JiaoYanMiMaRand[i];
				}
				
				//0x41 0x42 0x43 0x44
				for (int i = 15; i < 18; i++) {
					buffer[i] = (byte)UnityEngine.Random.Range(0x00, 0x40);
				}
				buffer[14] = 0x00;
				
				for (int i = 15; i < 18; i++) {
					buffer[14] ^= buffer[i];
				}
			}
			buffer[8] = 0x00;
			buffer[9] = 0x00;
			
			buffer[6] = 0x00;
			for (int i = 2; i <= 11; i++) {
				if (i == 6) {
					continue;
				}
				buffer[6] ^= buffer[i];
			}
			
			buffer[19] = 0x00;
			for (int i = 0; i < HID_BUF_LEN_WRITE; i++) {
				if (i == 19) {
					continue;
				}
				buffer[19] ^= buffer[i];
			}
#endif
		}
		break;
		}
		MyCOMDevice.ComThreadClass.WriteByteMsg = buffer;
	}

	/**
	 * 座椅电机速度设置.
	 * ZuoYiDianJiSpeedVal[0] -> 1P.
	 * ZuoYiDianJiSpeedVal[1] -> 2P.
	 * ZuoYiDianJiSpeedVal[2] -> 3P.
	 * ZuoYiDianJiSpeedVal[3] -> 4P.
	 */
	public static byte[] ZuoYiDianJiSpeedVal = {0, 0, 0, 0};
	public static int DianJiSpeedP1 = 0x01;
	public static int DianJiSpeedP2 = 0x01;
	public static int DianJiSpeedP3 = 0x01;
	public static int DianJiSpeedP4 = 0x01;
	/**
	 * moveState == 1 -> 向上.
	 * moveState == 0 -> 停止.
	 * moveState == -1 -> 向下.
	 */
	public void SetZuoYiDianJiSpeed(PlayerEnum indexPlayer, int moveState)
	{
		int indexVal = (int)indexPlayer - 1;
		byte speedTmp = 0x00;
		byte speed = 0x00;
		switch (indexPlayer) {
		case PlayerEnum.PlayerOne:
			speed = (byte)DianJiSpeedP1;
			break;
		case PlayerEnum.PlayerTwo:
			speed = (byte)DianJiSpeedP2;
			break;
		case PlayerEnum.PlayerThree:
			speed = (byte)DianJiSpeedP3;
			break;
		case PlayerEnum.PlayerFour:
			speed = (byte)DianJiSpeedP4;
			break;
		}

		switch (moveState) {
		case 1:
			speedTmp = speed;
			SetRunZuoYiState(indexPlayer, 0);
			break;
		case 0:
			speedTmp = 0x00;
			SetRunZuoYiState(indexPlayer, 1);
			break;
		case -1:
			speedTmp = (byte)(0x10 + speed);
			SetRunZuoYiState(indexPlayer, 0);
			break;
		}
		ZuoYiDianJiSpeedVal[indexVal] = speedTmp;
		//Debug.Log("Unity:"+"**ZuoYiDianJiSpeedVal["+indexVal+"] "+ZuoYiDianJiSpeedVal[indexVal]+", moveState "+moveState);
	}
	
	/**
	 * RunZuoYiState[0] == 0 -> run zuoYi.
	 * RunZuoYiState[0] == 1 -> stop zuoYi.
	 * RunZuoYiState[0] -> player1.
	 * RunZuoYiState[1] -> player2.
	 * RunZuoYiState[2] -> player3.
	 * RunZuoYiState[3] -> player4.
	 */
	public static byte[] RunZuoYiState = {0, 0, 0, 0};
	/**
	 * runVal == 0 -> run zuoYi.
	 * runVal == 1 -> stop zuoYi.
	 */
	public static void SetRunZuoYiState(PlayerEnum indexPlayer, byte runVal)
	{
		int indexVal = (int)indexPlayer - 1;
		RunZuoYiState[indexVal] = runVal;
	}

	/**
	 * ZuoYiTrigger[0] -> 玩家1座椅传感器1.
	 * ZuoYiTrigger[1] -> 玩家1座椅传感器2.
	 * ZuoYiTrigger[2] -> 玩家1座椅传感器3.
	 */
	byte[] ZuoYiTrigger = { 0, 0, 0,
							0, 0, 0,
							0, 0, 0,
							0, 0, 0};
	/**
	 * indexTrigger ==  1 -> 上.
	 * indexTrigger ==  0 -> 中.
	 * indexTrigger == -1 -> 下.
	 */
	void OnZuoYiDianJiMoveOver(PlayerEnum indexPlayer, int indexTrigger, ButtonState btState)
	{
//		Debug.Log("Unity:"+"OnZuoYiDianJiMoveOver -> indexPlayer "+indexPlayer+", indexTrigger "+indexTrigger
//		          +", btState "+btState);
		if (indexTrigger == 0 && TKMoveSt == TKMoveState.U_FangXiangPan) {
			float shaCheVal = btState == ButtonState.UP ? -1f : 0f; //当传感器弹起时刹车有效.
			SetPcvrShaCheInfo(indexPlayer, shaCheVal);
		}

		if (btState == ButtonState.DOWN) {
			int indexVal = (int)indexPlayer - 1;
#if !USE_CENTER_ZUOYI
			switch (indexTrigger) {
			case 1:
				SetZuoYiDianJiSpeed(indexPlayer, -1);
				break;
			case -1:
				CheckPlayerZuoYiState(indexPlayer);
				if (RunZuoYiState[indexVal] == 0) {
					SetZuoYiDianJiSpeed(indexPlayer, 1);
				}
				else {
					SetZuoYiDianJiSpeed(indexPlayer, 0);
				}
				break;
			}
#else
			if (indexTrigger == -1 && ZuoYiDianJiMvCenter[indexVal] == 1) {
				SetZuoYiDianJiSpeed(indexPlayer, 1);
			}
			else {
				if (indexTrigger == 0 && ZuoYiDianJiMvCenter[indexVal] == 1) {
					Debug.Log("Unity:"+"OnZuoYiDianJiMoveOver -> indexPlayer "+indexPlayer
					          +", ZuoYi back center!");
					ZuoYiDianJiMvCenter[indexVal] = 0;
				}
				SetZuoYiDianJiSpeed(indexPlayer, 0);
			}
#endif
		}

		if (HardwareCheckCtrl.IsTestHardWare) {
			bool isActiveTrigger = btState == ButtonState.DOWN ? true : false;
			switch(indexPlayer) {
			case PlayerEnum.PlayerOne:
				switch(indexTrigger) {
				case 1:
					HardwareCheckCtrl.Instance.SetZuoYiActiveShangP1(isActiveTrigger);
					break;
				case 0:
					HardwareCheckCtrl.Instance.SetZuoYiActiveZhongP1(isActiveTrigger);
					break;
				case -1:
					HardwareCheckCtrl.Instance.SetZuoYiActiveXiaP1(isActiveTrigger);
					break;
				}
				break;
			case PlayerEnum.PlayerTwo:
				switch(indexTrigger) {
				case 1:
					HardwareCheckCtrl.Instance.SetZuoYiActiveShangP2(isActiveTrigger);
					break;
				case 0:
					HardwareCheckCtrl.Instance.SetZuoYiActiveZhongP2(isActiveTrigger);
					break;
				case -1:
					HardwareCheckCtrl.Instance.SetZuoYiActiveXiaP2(isActiveTrigger);
					break;
				}
				break;
			case PlayerEnum.PlayerThree:
				switch(indexTrigger) {
				case 1:
					HardwareCheckCtrl.Instance.SetZuoYiActiveShangP3(isActiveTrigger);
					break;
				case 0:
					HardwareCheckCtrl.Instance.SetZuoYiActiveZhongP3(isActiveTrigger);
					break;
				case -1:
					HardwareCheckCtrl.Instance.SetZuoYiActiveXiaP3(isActiveTrigger);
					break;
				}
				break;
			case PlayerEnum.PlayerFour:
				switch(indexTrigger) {
				case 1:
					HardwareCheckCtrl.Instance.SetZuoYiActiveShangP4(isActiveTrigger);
					break;
				case 0:
					HardwareCheckCtrl.Instance.SetZuoYiActiveZhongP4(isActiveTrigger);
					break;
				case -1:
					HardwareCheckCtrl.Instance.SetZuoYiActiveXiaP4(isActiveTrigger);
					break;
				}
				break;
			}
		}
	}

	/**
	 * ZuoYiDianJiMvCenter[0] == 1 -> 座椅1归中.
	 * ZuoYiDianJiMvCenter[1] == 1 -> 座椅2归中.
	 * ZuoYiDianJiMvCenter[2] == 1 -> 座椅3归中.
	 * ZuoYiDianJiMvCenter[3] == 1 -> 座椅4归中.
	 */
	byte[] ZuoYiDianJiMvCenter = {0, 0, 0, 0};
	public void MakeZuoYiMoveCenter(PlayerEnum indexPlayer)
	{
		int indexVal = (int)indexPlayer - 1;
		int indexCenter = (indexVal * 3) + 1;
		if (ZuoYiTrigger[indexCenter] == 1) {
			return;
		}
		ZuoYiDianJiMvCenter[indexVal] = 1;
		SetZuoYiDianJiSpeed(indexPlayer, -1);
	}

	public static byte[] FangXiangPanDouDongVal = {0, 0, 0, 0};
	/**
	 * 方向盘是否循环抖动控制.
	 */
	public static byte[] FangXiangPanDouDongLPVal = {0, 0, 0, 0};
	public void ActiveFangXiangDouDong(PlayerEnum playerVal, bool isLoopDouDong)
	{
		if (XKGlobalData.GameVersionPlayer != 0) {
			switch (playerVal) {
			case PlayerEnum.PlayerThree:
				playerVal = PlayerEnum.PlayerOne;
				break;
			case PlayerEnum.PlayerFour:
				playerVal = PlayerEnum.PlayerTwo;
				break;
			}
		}

		int indexVal = (int)playerVal - 1;
		FangXiangPanDouDongVal[indexVal] = 0xaa;
		FangXiangPanDouDongLPVal[indexVal] = (byte)(isLoopDouDong == true ? 1 : 0);
		StopCoroutine(PlayFangXiangPanDouDong(playerVal));
		StartCoroutine(PlayFangXiangPanDouDong(playerVal));
	}

	public static void OpenAllPlayerFangXiangPanPower(PlayerEnum playerVal = PlayerEnum.Null)
	{
		if (playerVal != PlayerEnum.Null) {
			int index = (int)(playerVal - 1);
			if (XKGlobalData.GameVersionPlayer == 0) {
				FangXiangPanDouDongVal[index] = (byte)(XkGameCtrl.GetIsActivePlayer(playerVal) == true ? 0xaa : 0x00);
			}
			else {
				if (playerVal == PlayerEnum.PlayerThree || playerVal == PlayerEnum.PlayerFour) {
					FangXiangPanDouDongVal[index] = 0x00;
					FangXiangPanDouDongVal[index-2] = (byte)(XkGameCtrl.GetIsActivePlayer(playerVal) == true ? 0xaa : 0x00);
				}
			}
			return;
		}

		for (int i = 0; i < 4; i++) {
			if (Application.loadedLevel == (int)GameLevel.Scene_1) {
				PlayerEnum indexPlayer = (PlayerEnum)(i+1);
				if (XKGlobalData.GameVersionPlayer == 0) {
					FangXiangPanDouDongVal[i] = (byte)(XkGameCtrl.GetIsActivePlayer(indexPlayer) == true ? 0xaa : 0x00);
				}
				else {
					if (indexPlayer == PlayerEnum.PlayerThree || indexPlayer == PlayerEnum.PlayerFour) {
						FangXiangPanDouDongVal[i] = 0x00;
						FangXiangPanDouDongVal[i-2] = (byte)(XkGameCtrl.GetIsActivePlayer(indexPlayer) == true ? 0xaa : 0x00);
					}
				}
			}
			else {
				FangXiangPanDouDongVal[i] = 0xaa;
			}
		}
	}

	IEnumerator PlayFangXiangPanDouDong(PlayerEnum playerVal)
	{
		bool isPlayDouDong = true;
		int count = 0;
		int indexVal = (int)playerVal - 1;
		do {
			//Debug.Log("Unity:"+"PlayFangXiangPanDouDong -> playerVal "+playerVal+", count "+count);
			if ((count >= 6 && FangXiangPanDouDongLPVal[indexVal] == 0)
			    || FangXiangPanDouDongVal[indexVal] == 0x00) {
				if (FangXiangPanDouDongVal[indexVal] != 0x00) {
					if (XKGlobalData.GameVersionPlayer == 0) {
						FangXiangPanDouDongVal[indexVal] = (byte)(XkGameCtrl.GetIsActivePlayer(playerVal) == true ? 0xaa : 0x00);
					}
					else {
						if (playerVal == PlayerEnum.PlayerOne) {
							playerVal = PlayerEnum.PlayerThree;
						}
						if (playerVal == PlayerEnum.PlayerTwo) {
							playerVal = PlayerEnum.PlayerFour;
						}
						FangXiangPanDouDongVal[indexVal] = (byte)(XkGameCtrl.GetIsActivePlayer(playerVal) == true ? 0xaa : 0x00);
					}
				}
				isPlayDouDong = false;
				/*Debug.Log("Unity:"+"PlayFangXiangPanDouDong -> playerVal "+playerVal
				          +", FangXiangPanDouDongVal "+FangXiangPanDouDongVal[indexVal].ToString("X2"));*/
				yield break;
			}
			FangXiangPanDouDongVal[indexVal] = (byte)(count % 2 == 0 ? 0x55 : 0xaa);
			count++;
			yield return new WaitForSeconds(0.3f);
		} while (isPlayDouDong);
	}

	public void CloseFangXiangPanPower(PlayerEnum playerVal = PlayerEnum.Null)
	{
		if (XKGlobalData.GameVersionPlayer != 0) {
			switch (playerVal) {
			case PlayerEnum.PlayerThree:
				playerVal = PlayerEnum.PlayerOne;
				break;
			case PlayerEnum.PlayerFour:
				playerVal = PlayerEnum.PlayerTwo;
				break;
			}
		}

		if (playerVal != PlayerEnum.Null) {
			int indexVal = (int)playerVal - 1;
			StopCoroutine(PlayFangXiangPanDouDong(playerVal));
			FangXiangPanDouDongVal[indexVal] = 0xaa;
			FangXiangPanDouDongLPVal[indexVal] = 0;
		}
		else {
			StopCoroutine(PlayFangXiangPanDouDong(PlayerEnum.PlayerOne));
			StopCoroutine(PlayFangXiangPanDouDong(PlayerEnum.PlayerTwo));
			StopCoroutine(PlayFangXiangPanDouDong(PlayerEnum.PlayerThree));
			StopCoroutine(PlayFangXiangPanDouDong(PlayerEnum.PlayerFour));
			for (int i = 0; i < 4; i++) {
				FangXiangPanDouDongLPVal[i] = 0;
				FangXiangPanDouDongVal[i] = 0x00;
			}
		}
	}

	static float TimeJiOuVal;
	void GetMessage()
	{
		if (!MyCOMDevice.ComThreadClass.IsReadComMsg) {
			return;
		}
		
		if (MyCOMDevice.ComThreadClass.IsReadMsgComTimeOut) {
			return;
		}
		int len = MyCOMDevice.ComThreadClass.ReadByteMsg.Length;
		uint[] readMsg = new uint[len];
		for (int i = 0; i < len; i++) {
			readMsg[i] = (uint)MyCOMDevice.ComThreadClass.ReadByteMsg[i];
		}
		
		if (readMsg.Length < (MyCOMDevice.ComThreadClass.BufLenRead - MyCOMDevice.ComThreadClass.BufLenReadEnd)) {
			//Debug.Log("Unity:"+"ReadBufLen was wrong! len is "+readMsg.Length);
			return;
		}
		
		if (IsJiOuJiaoYanFailed) {
			if (Time.time - TimeJiOuVal < 5f) {
				return;
			}
			IsJiOuJiaoYanFailed = false;
			JiOuJiaoYanCount = 0;
			//return;
		}
		
		switch (MyCOMDevice.PcvrComSt) {
		case MyCOMDevice.PcvrComState.TanKeFangXiangZhenDong:
		{
#if !COM_TANK_TEST
			if ((readMsg[34]&0x01) == 0x01) {
				JiOuJiaoYanCount++;
				if (JiOuJiaoYanCount >= JiOuJiaoYanMax && !IsJiOuJiaoYanFailed) {
					TimeJiOuVal = Time.time;
					IsJiOuJiaoYanFailed = true; //JiOuJiaoYanFailed
					ScreenLog.LogWarning("IsJiOuJiaoYanFailed "+IsJiOuJiaoYanFailed);
					return;
				}
			}
			//IsJiOuJiaoYanFailed = true; //test
			
			uint tmpVal = 0x00;
//			string testA = "";
//			string testB = "";
			for (int i = 0; i < (MyCOMDevice.ComThreadClass.BufLenRead - 4); i++) {
//				testB += readMsg[i].ToString("X2") + " ";
				if (i == 18 || i == 19 || i == 33) {
					continue;
				}
//				testA += readMsg[i].ToString("X2") + " ";
				tmpVal ^= readMsg[i];
			}
			tmpVal ^= EndRead_1;
			tmpVal ^= EndRead_2;
			tmpVal ^= EndRead_3;
			tmpVal ^= EndRead_4;
//			testA += EndRead_1 + " ";
//			testA += EndRead_2 + " ";
//			testA += EndRead_3 + " ";
//			testA += EndRead_4 + " ";
			
			if (tmpVal != readMsg[33]) {
				if (MyCOMDevice.ComThreadClass.IsStopComTX) {
					return;
				}
				MyCOMDevice.ComThreadClass.IsStopComTX = true;
//				ScreenLog.Log("testB: "+testB);
//				ScreenLog.Log("testA: "+testA);
//				ScreenLog.LogError("tmpVal: "+tmpVal.ToString("X2")+", byte[33] "+readMsg[33].ToString("X2"));
//				ScreenLog.Log("byte[33] was wrong!");
				return;
			}
			
			if (IsJiaoYanHid) {
				tmpVal = 0x00;
	//			string testStrA = readMsg[30].ToString("X2") + " ";
				string testStrB = "";
				string testStrA = "";
				for (int i = 0; i < readMsg.Length; i++) {
					testStrA += readMsg[i].ToString("X2") + " ";
				}
				ScreenLog.Log("readStr: "+testStrA);
	//
	//			for (int i = 0; i < JiaoYanDt.Length; i++) {
	//				testStrB += JiaoYanDt[i].ToString("X2") + " ";
	//			}
	//			ScreenLog.Log("GameSendDt: "+testStrB);
	//
	//			string testStrC = "";
	//			for (int i = 0; i < JiaoYanDt.Length; i++) {
	//				testStrC += JiaoYanMiMa[i].ToString("X2") + " ";
	//			}
	//			ScreenLog.Log("GameSendMiMa: "+testStrC);
				
				testStrA = "";
				for (int i = 26; i < 29; i++) {
					tmpVal ^= readMsg[i];
					testStrA += readMsg[i].ToString("X2") + " ";
				}
				
				if (tmpVal == readMsg[25]) {
					bool isJiaoYanDtSucceed = false;
					tmpVal = 0x00;
					for (int i = 30; i < 33; i++) {
						tmpVal ^= readMsg[i];
					}
					
					//校验2...
					if ( tmpVal == readMsg[29]
					    && (JiaoYanDt[1]&0xef) == readMsg[30]
					    && (JiaoYanDt[2]&0xfe) == readMsg[31]
					    && (JiaoYanDt[3]|0x28) == readMsg[32] ) {
						isJiaoYanDtSucceed = true;
					}
					
					JiaoYanCheckCount++;
					if (isJiaoYanDtSucceed) {
						//JiaMiJiaoYanSucceed
						OnEndJiaoYanIO(JIAOYANENUM.SUCCEED);
						//ScreenLog.Log("JMJYCG...");
					}
					else {
						if (JiaoYanCheckCount > 0) {
							OnEndJiaoYanIO(JIAOYANENUM.FAILED);
						}
						testStrA = "";
						for (int i = 0; i < 35; i++) {
							testStrA += readMsg[i].ToString("X2") + " ";
						}
						
						testStrB = "";
						for (int i = 29; i < 33; i++) {
							testStrB += readMsg[i].ToString("X2") + " ";
						}
						
						string testStrD = "";
						for (int i = 0; i < 4; i++) {
							testStrD += JiaoYanDt[i].ToString("X2") + " ";
						}
						ScreenLog.LogWarning("JiaoYan2 ShiBai...");
						ScreenLog.Log("ReadByte[0 - 34] "+testStrA);
						ScreenLog.Log("ReadByte[29 - 32] "+testStrB);
						ScreenLog.Log("SendByte[25 - 28] "+testStrD);
						ScreenLog.LogError("校验数据错误!");
					}
				}
				else {
					ScreenLog.LogWarning("JiaoYan1 ShiBai...");
					testStrB = "byte[25] "+readMsg[25].ToString("X2")+" "
						+", tmpVal "+tmpVal.ToString("X2");
					ScreenLog.Log("ReadByte[26 - 28] "+testStrA);
					ScreenLog.Log(testStrB);
					ScreenLog.LogError("ReadByte[25] was wrong!");
				}
			}
#endif
		}
			break;
		case MyCOMDevice.PcvrComState.TanKeGunZhenDong:
		{
#if COM_TANK_TEST
			if ((readMsg[22]&0x01) == 0x01) {
				JiOuJiaoYanCount++;
				if (JiOuJiaoYanCount >= JiOuJiaoYanMax && !IsJiOuJiaoYanFailed) {
					IsJiOuJiaoYanFailed = true;
					//JiOuJiaoYanFailed
				}
			}
			//IsJiOuJiaoYanFailed = true; //test
			
			byte tmpVal = 0x00;
			string testA = "";
			for (int i = 2; i < (MyCOMDevice.ComThreadClass.BufLenRead - 4); i++) {
				if (i == 18 || i == 21) {
					continue;
				}
				testA += readMsg[i].ToString("X2") + " ";
				tmpVal ^= readMsg[i];
			}
			tmpVal ^= EndRead_1;
			tmpVal ^= EndRead_2;
			testA += EndRead_1 + " ";
			testA += EndRead_2 + " ";
			testA += EndRead_3 + " ";
			testA += EndRead_4 + " ";
			
			if (tmpVal != readMsg[21]) {
				if (MyCOMDevice.ComThreadClass.IsStopComTX) {
					return;
				}
				MyCOMDevice.ComThreadClass.IsStopComTX = true;
	//			ScreenLog.Log("testA: "+testA);
	//			ScreenLog.LogError("tmpVal: "+tmpVal.ToString("X2")+", byte[21] "+readMsg[21].ToString("X2"));
	//			ScreenLog.Log("byte21 was wrong!");
				return;
			}
#endif
		}
			break;
		}

		keyProcess(readMsg);
	}
	
	public static byte DongGanState = 1;
	void keyProcess(uint []buffer)
	{
		switch (MyCOMDevice.PcvrComSt) {
		case MyCOMDevice.PcvrComState.TanKeFangXiangZhenDong:
		{
#if !COM_TANK_TEST
			SteerValCurAy[0] = ((buffer[2] & 0x0f) << 8) + buffer[3];
			YouMenCurVal[0] = ((buffer[4] & 0x0f) << 8) + buffer[5];

			SteerValCurAy[1] = ((buffer[6] & 0x0f) << 8) + buffer[7];
			YouMenCurVal[1] = ((buffer[8] & 0x0f) << 8) + buffer[9];

			SteerValCurAy[2] = ((buffer[10] & 0x0f) << 8) + buffer[11];
			YouMenCurVal[2] = ((buffer[12] & 0x0f) << 8) + buffer[13];

			SteerValCurAy[3] = ((buffer[14] & 0x0f) << 8) + buffer[15];
			YouMenCurVal[3] = ((buffer[16] & 0x0f) << 8) + buffer[17];
			CheckYouMenValInfo();

			//game coinInfo
			CoinCurPcvr12 = buffer[18];
			uint coinP1 = CoinCurPcvr12 & 0x0f;
			uint coinP2 = (CoinCurPcvr12 & 0xf0) >> 4;
			CoinCurPcvr34 = buffer[19];
			uint coinP3 = CoinCurPcvr34 & 0x0f;
			uint coinP4 = (CoinCurPcvr34 & 0xf0) >> 4;
			if (IsSubPlayerCoin) {
				if (coinP1 == 0 && IsSubCoinP1) {
					//ScreenLog.Log("sub coinP1 "+coinP1);
					IsSubCoinP1 = false;
					IsSubPlayerCoin = false;
					SubCoinNum_12 = 0;
				}
			
				if (coinP2 == 0 && IsSubCoinP2) {
					IsSubCoinP2 = false;
					IsSubPlayerCoin = false;
					SubCoinNum_12 = 0;
				}
				
				if (XKGlobalData.GameVersionPlayer == 0) {
					if (coinP3 == 0 && IsSubCoinP3) {
						IsSubCoinP3 = false;
						IsSubPlayerCoin = false;
						SubCoinNum_34 = 0;
					}
				
					if (coinP4 == 0 && IsSubCoinP4) {
						IsSubCoinP4 = false;
						IsSubPlayerCoin = false;
						SubCoinNum_34 = 0;
					}
				}
			}
			else {
				if (coinP1 > 0 && coinP1 < 256) {
					//ScreenLog.Log("coinP1 "+coinP1);
					IsSubCoinP1 = true;
					CoinNumCurrentP1 += (int)coinP1;
					XKGlobalData.SetCoinPlayerOne(CoinNumCurrentP1);
					SubPcvrCoin(PlayerEnum.PlayerOne, (int)(CoinCurPcvr12 & 0x0f));
				}
			
				if (coinP2 > 0 && coinP2 < 256) {
					IsSubCoinP2 = true;
					CoinNumCurrentP2 += (int)coinP2;
					XKGlobalData.SetCoinPlayerTwo(CoinNumCurrentP2);
					SubPcvrCoin(PlayerEnum.PlayerTwo, (int)(CoinCurPcvr12 & 0xf0));
				}
				
				if (XKGlobalData.GameVersionPlayer == 0) {
					if (coinP3 > 0 && coinP3 < 256) {
						IsSubCoinP3 = true;
						CoinNumCurrentP3 += (int)coinP3;
						XKGlobalData.SetCoinPlayerThree(CoinNumCurrentP3);
						SubPcvrCoin(PlayerEnum.PlayerThree, (int)(CoinCurPcvr34 & 0x0f));
					}
				
					if (coinP4 > 0 && coinP4 < 256) {
						IsSubCoinP4 = true;
						CoinNumCurrentP4 += (int)coinP4;
						XKGlobalData.SetCoinPlayerFour(CoinNumCurrentP4);
						SubPcvrCoin(PlayerEnum.PlayerFour, (int)(CoinCurPcvr34 & 0xf0));
					}
				}
			}

			//test
	//		buffer[23] = (byte)(UnityEngine.Random.Range(0, 100) % 16);
	//		buffer[24] = (byte)(UnityEngine.Random.Range(0, 100) % 16);
			if ((buffer[23]&0x01) == 0x01 && ZuoYiTrigger[0] == 0) {
				ZuoYiTrigger[0] = 1;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerOne, 1, ButtonState.DOWN);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangUBtP1(ButtonState.UP);
				}
			}
			else if ((buffer[23]&0x01) == 0x00 && ZuoYiTrigger[0] == 1) {
				ZuoYiTrigger[0] = 0;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerOne, 1, ButtonState.UP);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangUBtP1(ButtonState.DOWN);
				}
			}

			if ((buffer[23]&0x02) == 0x02 && ZuoYiTrigger[1] == 0) {
				ZuoYiTrigger[1] = 1;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerOne, 0, ButtonState.DOWN);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangLBtP1(ButtonState.UP);
				}
			}
			else if ((buffer[23]&0x02) == 0x00 && ZuoYiTrigger[1] == 1) {
				ZuoYiTrigger[1] = 0;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerOne, 0, ButtonState.UP);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangLBtP1(ButtonState.DOWN);
				}
			}

			if ((buffer[23]&0x04) == 0x04 && ZuoYiTrigger[2] == 0) {
				ZuoYiTrigger[2] = 1;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerOne, -1, ButtonState.DOWN);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangRBtP1(ButtonState.UP);
				}
			}
			else if ((buffer[23]&0x04) == 0x00 && ZuoYiTrigger[2] == 1) {
				ZuoYiTrigger[2] = 0;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerOne, -1, ButtonState.UP);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangRBtP1(ButtonState.DOWN);
				}
			}

			if ((buffer[23]&0x08) == 0x08 && ZuoYiTrigger[3] == 0) {
				ZuoYiTrigger[3] = 1;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerTwo, 1, ButtonState.DOWN);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangUBtP2(ButtonState.UP);
				}
			}
			else if ((buffer[23]&0x08) == 0x00 && ZuoYiTrigger[3] == 1) {
				ZuoYiTrigger[3] = 0;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerTwo, 1, ButtonState.UP);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangUBtP2(ButtonState.DOWN);
				}
			}
		
			if ((buffer[23]&0x10) == 0x10 && ZuoYiTrigger[4] == 0) {
				ZuoYiTrigger[4] = 1;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerTwo, 0, ButtonState.DOWN);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangLBtP2(ButtonState.UP);
				}
			}
			else if ((buffer[23]&0x10) == 0x00 && ZuoYiTrigger[4] == 1) {
				ZuoYiTrigger[4] = 0;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerTwo, 0, ButtonState.UP);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangLBtP2(ButtonState.DOWN);
				}
			}
		
			if ((buffer[23]&0x20) == 0x20 && ZuoYiTrigger[5] == 0) {
				ZuoYiTrigger[5] = 1;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerTwo, -1, ButtonState.DOWN);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangRBtP2(ButtonState.UP);
				}
			}
			else if ((buffer[23]&0x20) == 0x00 && ZuoYiTrigger[5] == 1) {
				ZuoYiTrigger[5] = 0;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerTwo, -1, ButtonState.UP);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangRBtP2(ButtonState.DOWN);
				}
			}

			if ((buffer[23]&0x40) == 0x40 && ZuoYiTrigger[6] == 0) {
				ZuoYiTrigger[6] = 1;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerThree, 1, ButtonState.DOWN);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangUBtP3(ButtonState.UP);
				}
			}
			else if ((buffer[23]&0x40) == 0x00 && ZuoYiTrigger[6] == 1) {
				ZuoYiTrigger[6] = 0;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerThree, 1, ButtonState.UP);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangUBtP3(ButtonState.DOWN);
				}
			}
		
			if ((buffer[23]&0x80) == 0x80 && ZuoYiTrigger[7] == 0) {
				ZuoYiTrigger[7] = 1;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerThree, 0, ButtonState.DOWN);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangLBtP3(ButtonState.UP);
				}
			}
			else if ((buffer[23]&0x80) == 0x00 && ZuoYiTrigger[7] == 1) {
				ZuoYiTrigger[7] = 0;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerThree, 0, ButtonState.UP);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangLBtP3(ButtonState.DOWN);
				}
			}

			if ((buffer[24]&0x01) == 0x01 && ZuoYiTrigger[8] == 0) {
				ZuoYiTrigger[8] = 1;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerThree, -1, ButtonState.DOWN);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangRBtP3(ButtonState.UP);
				}
			}
			else if ((buffer[24]&0x01) == 0x00 && ZuoYiTrigger[8] == 1) {
				ZuoYiTrigger[8] = 0;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerThree, -1, ButtonState.UP);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangRBtP3(ButtonState.DOWN);
				}
			}
		
			if ((buffer[24]&0x02) == 0x02 && ZuoYiTrigger[9] == 0) {
				ZuoYiTrigger[9] = 1;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerFour, 1, ButtonState.DOWN);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangUBtP4(ButtonState.UP);
				}
			}
			else if ((buffer[24]&0x02) == 0x00 && ZuoYiTrigger[9] == 1) {
				ZuoYiTrigger[9] = 0;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerFour, 1, ButtonState.UP);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangUBtP4(ButtonState.DOWN);
				}
			}
		
			if ((buffer[24]&0x04) == 0x04 && ZuoYiTrigger[10] == 0) {
				ZuoYiTrigger[10] = 1;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerFour, 0, ButtonState.DOWN);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangLBtP4(ButtonState.UP);
				}
			}
			else if ((buffer[24]&0x04) == 0x00 && ZuoYiTrigger[10] == 1) {
				ZuoYiTrigger[10] = 0;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerFour, 0, ButtonState.UP);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangLBtP4(ButtonState.DOWN);
				}
			}

			if ((buffer[24]&0x08) == 0x08 && ZuoYiTrigger[11] == 0) {
				ZuoYiTrigger[11] = 1;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerFour, -1, ButtonState.DOWN);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangRBtP4(ButtonState.UP);
				}
			}
			else if ((buffer[24]&0x08) == 0x00 && ZuoYiTrigger[11] == 1) {
				ZuoYiTrigger[11] = 0;
				OnZuoYiDianJiMoveOver(PlayerEnum.PlayerFour, -1, ButtonState.UP);
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangRBtP4(ButtonState.DOWN);
				}
			}

			//game startBt, hitNpcBt or jiaoZhunBt
			if( !bPlayerStartKeyDownP1 && (buffer[20]&0x01) == 0x01 )
			{
				//ScreenLog.Log("gameP1 startBt down!");
				bPlayerStartKeyDownP1 = true;
				InputEventCtrl.GetInstance().ClickStartBtOne( ButtonState.DOWN );
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangDBtP1(ButtonState.DOWN);
				}
			}
			else if ( bPlayerStartKeyDownP1 && (buffer[20]&0x01) == 0x00 )
			{
				//ScreenLog.Log("gameP1 startBt up!");
				bPlayerStartKeyDownP1 = false;
				InputEventCtrl.GetInstance().ClickStartBtOne( ButtonState.UP );
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangDBtP1(ButtonState.UP);
				}
			}
		
			if( !IsFireBtDownP1 && (buffer[20]&0x02) == 0x02 )
			{
				IsFireBtDownP1 = true;
				InputEventCtrl.IsClickFireBtOneDown = true;
				//ScreenLog.Log("game fireBtP1 down!");
				InputEventCtrl.GetInstance().ClickFireBtOne( ButtonState.DOWN );
			}
			else if( IsFireBtDownP1 && (buffer[20]&0x02) == 0x00 )
			{
				IsFireBtDownP1 = false;
				InputEventCtrl.IsClickFireBtOneDown = false;
				//ScreenLog.Log("game fireBtP1 up!");
				InputEventCtrl.GetInstance().ClickFireBtOne( ButtonState.UP );
			}
		
			if( !IsDaoDanBtDownP1 && (buffer[20]&0x04) == 0x04 )
			{
				IsDaoDanBtDownP1 = true;
				InputEventCtrl.GetInstance().ClickDaoDanBtOne( ButtonState.DOWN );
			}
			else if( IsDaoDanBtDownP1 && (buffer[20]&0x04) == 0x00 )
			{
				IsDaoDanBtDownP1 = false;
				InputEventCtrl.GetInstance().ClickDaoDanBtOne( ButtonState.UP );
			}

			//game startBt, hitNpcBt or jiaoZhunBt
			if( !bPlayerStartKeyDownP2 && (buffer[20]&0x08) == 0x08 )
			{
				//ScreenLog.Log("gameP2 startBt down!");
				bPlayerStartKeyDownP2 = true;
				InputEventCtrl.GetInstance().ClickStartBtTwo( ButtonState.DOWN );
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangDBtP2(ButtonState.DOWN);
				}
			}
			else if ( bPlayerStartKeyDownP2 && (buffer[20]&0x08) == 0x00 )
			{
				//ScreenLog.Log("gameP2 startBt up!");
				bPlayerStartKeyDownP2 = false;
				InputEventCtrl.GetInstance().ClickStartBtTwo( ButtonState.UP );
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangDBtP2(ButtonState.UP);
				}
			}

			if( !IsFireBtDownP2 && (buffer[20]&0x10) == 0x10 )
			{
				IsFireBtDownP2 = true;
				InputEventCtrl.IsClickFireBtTwoDown = true;
				//ScreenLog.Log("game fireBtP2 down!");
				InputEventCtrl.GetInstance().ClickFireBtTwo( ButtonState.DOWN );
			}
			else if( IsFireBtDownP2 && (buffer[20]&0x10) == 0x00 )
			{
				IsFireBtDownP2 = false;
				InputEventCtrl.IsClickFireBtTwoDown = false;
				//ScreenLog.Log("game fireBtP2 up!");
				InputEventCtrl.GetInstance().ClickFireBtTwo( ButtonState.UP );
			}
		
			if( !IsDaoDanBtDownP2 && (buffer[20]&0x20) == 0x20 )
			{
				IsDaoDanBtDownP2 = true;
				InputEventCtrl.GetInstance().ClickDaoDanBtTwo( ButtonState.DOWN );
			}
			else if( IsDaoDanBtDownP2 && (buffer[20]&0x20) == 0x00 )
			{
				IsDaoDanBtDownP2 = false;
				InputEventCtrl.GetInstance().ClickDaoDanBtTwo( ButtonState.UP );
			}

			if( !bPlayerStartKeyDownP3 && (buffer[20]&0x40) == 0x40 )
			{
				//ScreenLog.Log("gameP3 startBt down!");
				bPlayerStartKeyDownP3 = true;
				InputEventCtrl.GetInstance().ClickStartBtThree( ButtonState.DOWN );
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangDBtP3(ButtonState.DOWN);
				}
			}
			else if ( bPlayerStartKeyDownP3 && (buffer[20]&0x40) == 0x00 )
			{
				//ScreenLog.Log("gameP3 startBt up!");
				bPlayerStartKeyDownP3 = false;
				InputEventCtrl.GetInstance().ClickStartBtThree( ButtonState.UP );
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangDBtP3(ButtonState.UP);
				}
			}
		
			if( !IsFireBtDownP3 && (buffer[20]&0x80) == 0x80 )
			{
				IsFireBtDownP3 = true;
				InputEventCtrl.IsClickFireBtThreeDown = true;
				//ScreenLog.Log("game fireBtP3 down!");
				InputEventCtrl.GetInstance().ClickFireBtThree( ButtonState.DOWN );
			}
			else if( IsFireBtDownP3 && (buffer[20]&0x80) == 0x00 )
			{
				IsFireBtDownP3 = false;
				InputEventCtrl.IsClickFireBtThreeDown = false;
				//ScreenLog.Log("game fireBtP3 up!");
				InputEventCtrl.GetInstance().ClickFireBtThree( ButtonState.UP );
			}
		
			if( !IsDaoDanBtDownP3 && (buffer[21]&0x01) == 0x01 )
			{
				IsDaoDanBtDownP3 = true;
				InputEventCtrl.GetInstance().ClickDaoDanBtThree( ButtonState.DOWN );
			}
			else if( IsDaoDanBtDownP3 && (buffer[21]&0x01) == 0x00 )
			{
				IsDaoDanBtDownP3 = false;
				InputEventCtrl.GetInstance().ClickDaoDanBtThree( ButtonState.UP );
			}

			if( !bPlayerStartKeyDownP4 && (buffer[21]&0x02) == 0x02 )
			{
				//ScreenLog.Log("gameP4 startBt down!");
				bPlayerStartKeyDownP4 = true;
				InputEventCtrl.GetInstance().ClickStartBtFour( ButtonState.DOWN );
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangDBtP4(ButtonState.DOWN);
				}
			}
			else if ( bPlayerStartKeyDownP4 && (buffer[21]&0x02) == 0x00 )
			{
				//ScreenLog.Log("gameP4 startBt up!");
				bPlayerStartKeyDownP4 = false;
				InputEventCtrl.GetInstance().ClickStartBtFour( ButtonState.UP );
				if (TKMoveSt == TKMoveState.YaoGanBan) {
					InputEventCtrl.GetInstance().ClickFangXiangDBtP4(ButtonState.UP);
				}
			}
		
			if( !IsFireBtDownP4 && (buffer[21]&0x04) == 0x04 )
			{
				IsFireBtDownP4 = true;
				InputEventCtrl.IsClickFireBtFourDown = true;
				//ScreenLog.Log("game fireBtP4 down!");
				InputEventCtrl.GetInstance().ClickFireBtFour( ButtonState.DOWN );
			}
			else if( IsFireBtDownP4 && (buffer[21]&0x04) == 0x00 )
			{
				IsFireBtDownP4 = false;
				InputEventCtrl.IsClickFireBtFourDown = false;
				//ScreenLog.Log("game fireBtP4 up!");
				InputEventCtrl.GetInstance().ClickFireBtFour( ButtonState.UP );
			}
		
			if( !IsDaoDanBtDownP4 && (buffer[21]&0x08) == 0x08 )
			{
				IsDaoDanBtDownP4 = true;
				InputEventCtrl.GetInstance().ClickDaoDanBtFour( ButtonState.DOWN );
			}
			else if( IsDaoDanBtDownP4 && (buffer[21]&0x08) == 0x00 )
			{
				IsDaoDanBtDownP4 = false;
				InputEventCtrl.GetInstance().ClickDaoDanBtFour( ButtonState.UP );
			}

			//DongGanBt
			if( !IsClickDongGanBtOne && (buffer[21]&0x10) == 0x10 )
			{
				IsClickDongGanBtOne = true;
				InputEventCtrl.GetInstance().ClickStopDongGanBtOne( ButtonState.DOWN );
			}
			else if ( IsClickDongGanBtOne && (buffer[21]&0x10) == 0x00 )
			{
				IsClickDongGanBtOne = false;
				InputEventCtrl.GetInstance().ClickStopDongGanBtOne( ButtonState.UP );
			}

			if( !IsClickDongGanBtThree && (buffer[21]&0x20) == 0x20 )
			{
				IsClickDongGanBtThree = true;
				InputEventCtrl.GetInstance().ClickStopDongGanBtThree( ButtonState.DOWN );
			}
			else if ( IsClickDongGanBtThree && (buffer[21]&0x20) == 0x00 )
			{
				IsClickDongGanBtThree = false;
				InputEventCtrl.GetInstance().ClickStopDongGanBtThree( ButtonState.UP );
			}

			//setPanel selectBt
			if( !bSetEnterKeyDown && (buffer[21]&0x40) == 0x40 )
			{
				bSetEnterKeyDown = true;
				//ScreenLog.Log("game setEnterBt down!");
				InputEventCtrl.GetInstance().ClickSetEnterBt( ButtonState.DOWN );
			}
			else if ( bSetEnterKeyDown && (buffer[21]&0x40) == 0x00 )
			{
				bSetEnterKeyDown = false;
				//ScreenLog.Log("game setEnterBt up!");
				InputEventCtrl.GetInstance().ClickSetEnterBt( ButtonState.UP );
			}
		
			//setPanel moveBt
			if ( !bSetMoveKeyDown && (buffer[21]&0x80) == 0x80 )
			{
				bSetMoveKeyDown = true;
				//ScreenLog.Log("game setMoveBt down!");
				//FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.DOWN );
				InputEventCtrl.GetInstance().ClickSetMoveBt( ButtonState.DOWN );
			}
			else if( bSetMoveKeyDown && (buffer[21]&0x80) == 0x00 )
			{
				bSetMoveKeyDown = false;
				//ScreenLog.Log("game setMoveBt up!");
				//FramesPerSecond.GetInstance().ClickSetMoveBtEvent( ButtonState.UP );
				InputEventCtrl.GetInstance().ClickSetMoveBt( ButtonState.UP );
			}
#endif
		}
		break;
		}
	}

	void SubPcvrCoin(PlayerEnum indexPlayer, int subNum)
	{
		if (!bIsHardWare) {
			return;
		}
		IsSubPlayerCoin = true;
		switch (indexPlayer) {
		case PlayerEnum.PlayerOne:
		case PlayerEnum.PlayerTwo:
			SubCoinNum_12 += subNum;
			break;
		case PlayerEnum.PlayerThree:
		case PlayerEnum.PlayerFour:
			SubCoinNum_34 += subNum;
			break;
		}
	}
	
	public void SubPlayerCoin(PlayerEnum indexPlayer, int subNum)
	{
		if (!bIsHardWare) {
			return;
		}
		
		switch (indexPlayer) {
		case PlayerEnum.PlayerOne:
			if (CoinNumCurrentP1 < subNum) {
				return;
			}
			CoinNumCurrentP1 -= subNum;
			break;
		case PlayerEnum.PlayerTwo:
			if (CoinNumCurrentP2 < subNum) {
				return;
			}
			CoinNumCurrentP2 -= subNum;
			break;
		case PlayerEnum.PlayerThree:
			if (CoinNumCurrentP3 < subNum) {
				return;
			}
			CoinNumCurrentP3 -= subNum;
			break;
		case PlayerEnum.PlayerFour:
			if (CoinNumCurrentP4 < subNum) {
				return;
			}
			CoinNumCurrentP4 -= subNum;
			break;
		}
	}
	
	public static void InitHandleJsonInfo()
	{
		XKGlobalData.GetInstance();
		FileName = XKGlobalData.FileName;
		HandleJsonObj = XKGlobalData.HandleJsonObj;
	}
	
	void InitSteerInfo()
	{
		int indexVal = 0;
		string strVal = "";
		for (int i = 0; i < 4; i++) {
			indexVal = i+1;
			strVal = HandleJsonObj.ReadFromFileXml(FileName, "SteerValMaxP"+indexVal);
			if (strVal == null || strVal == "") {
				strVal = "2";
				HandleJsonObj.WriteToFileXml(FileName, "SteerValMaxP"+indexVal, strVal);
			}
			SteerValMaxAy[i] = Convert.ToUInt32( strVal );
		}

		for (int i = 0; i < 4; i++) {
			indexVal = i+1;
			strVal = HandleJsonObj.ReadFromFileXml(FileName, "SteerValCenP"+indexVal);
			if (strVal == null || strVal == "") {
				strVal = "1";
				HandleJsonObj.WriteToFileXml(FileName, "SteerValCenP"+indexVal, strVal);
			}
			SteerValCenAy[i] = Convert.ToUInt32( strVal );
		}
		
		for (int i = 0; i < 4; i++) {
			indexVal = i+1;
			strVal = HandleJsonObj.ReadFromFileXml(FileName, "SteerValMinP"+indexVal);
			if (strVal == null || strVal == "") {
				strVal = "0";
				HandleJsonObj.WriteToFileXml(FileName, "SteerValMinP"+indexVal, strVal);
			}
			SteerValMinAy[i] = Convert.ToUInt32( strVal );
		}
	}
	
	public static void SaveSteerVal(PcvrValState key, PlayerEnum indexPlayer)
	{
		int indexVal = (int)indexPlayer - 1;
		int playerNum = (int)indexPlayer;
		switch (key) {
		case PcvrValState.ValMin:
			SteerValMinAy[indexVal] = SteerValCurAy[indexVal];
			HandleJsonObj.WriteToFileXml(FileName, "SteerValMinP"+playerNum, SteerValCurAy[indexVal].ToString());
			break;
			
		case PcvrValState.ValCenter:
			SteerValCenAy[indexVal] = SteerValCurAy[indexVal];
			HandleJsonObj.WriteToFileXml(FileName, "SteerValCenP"+playerNum, SteerValCurAy[indexVal].ToString());
			break;
			
		case PcvrValState.ValMax:
			SteerValMaxAy[indexVal] = SteerValCurAy[indexVal];
			HandleJsonObj.WriteToFileXml(FileName, "SteerValMaxP"+playerNum, SteerValCurAy[indexVal].ToString());
			break;
		}
	}
	
	void GetPcvrSteerVal()
	{
		if (!bIsHardWare) {
			return;
		}

		uint steerMaxVal = 0;
		uint steerMinVal = 0;
		uint steerCenVal = 0;
		uint fangXiangVal = 0;
		float fangXiangValTmp = 0f;
		for (int i = 0; i < 4; i++) {
			steerMaxVal = SteerValMaxAy[i];
			steerMinVal = SteerValMinAy[i];
			steerCenVal = SteerValCenAy[i];
			fangXiangVal = SteerValCurAy[i];
			fangXiangValTmp = 0f;
			if (fangXiangVal >= steerCenVal) {
				if (steerMaxVal > steerMinVal) {
					fangXiangValTmp = ((float)fangXiangVal - steerCenVal) / (steerMaxVal - steerCenVal);
				}
				else {
					fangXiangValTmp = ((float)steerCenVal - fangXiangVal) / (steerMinVal - steerCenVal);
				}
			}
			else {
				if (steerMaxVal > steerMinVal) {
					fangXiangValTmp = ((float)fangXiangVal - steerCenVal) / (steerCenVal - steerMinVal);
				}
				else {
					fangXiangValTmp = ((float)steerCenVal - fangXiangVal) / (steerCenVal - steerMaxVal);
				}
			}
			
			fangXiangValTmp = Mathf.Clamp(fangXiangValTmp, -1f, 1f);
			fangXiangValTmp = Mathf.Abs(fangXiangValTmp) <= 0.05f ? 0f : fangXiangValTmp;
			InputEventCtrl.PlayerFX[i] = fangXiangValTmp;
		}
	}
	
	public static uint[] YouMenCurVal = new uint[4];
	static uint[] YouMenMaxVal = new uint[4];
	static uint[] YouMenMinVal = new uint[4];
	static int CountYM = 100;
	float TimeYM;
	public static void OpenCheckYouMenValInfo()
	{
		CountYM = 0;
		for (int i = 0; i < 4; i++) {
			YouMenMinVal[i] = 0;
		}
	}

	void CheckYouMenValInfo()
	{
		if (!bIsHardWare) {
			return;
		}

		if (CountYM >= 10) {
			return;
		}

		if (Time.realtimeSinceStartup - TimeYM < 0.1f) {
			return;
		}
		TimeYM = Time.realtimeSinceStartup;

		CountYM++;
		int playerNum = 0;
		for (int i = 0; i < 4; i++) {
			YouMenMinVal[i] += YouMenCurVal[i];
			//Debug.Log("Unity:"+"YouMenMinVal["+i+"] "+YouMenMinVal[i]);
			if (CountYM >= 10) {
				playerNum = i+1;
				YouMenMinVal[i] = (uint)((float)YouMenMinVal[i] / 10f);
				//Debug.Log("Unity:"+"***YouMenMinVal["+i+"] "+YouMenMinVal[i]);
				HandleJsonObj.WriteToFileXml(FileName, "YouMenMinValP"+playerNum, YouMenMinVal[i].ToString());
			}
		}
	}

	void InitYouMenInfo()
	{
		int indexVal = 0;
		string strVal = "";
		for (int i = 0; i < 4; i++) {
			indexVal = i+1;
			strVal = HandleJsonObj.ReadFromFileXml(FileName, "YouMenMaxValP"+indexVal);
			if (strVal == null || strVal == "") {
				strVal = "2";
				HandleJsonObj.WriteToFileXml(FileName, "YouMenMaxValP"+indexVal, strVal);
			}
			YouMenMaxVal[i] = Convert.ToUInt32( strVal );
		}

		if (Application.loadedLevel == (int)GameLevel.SetPanel) {
			return;
		}

		for (int i = 0; i < 4; i++) {
			indexVal = i+1;
			strVal = HandleJsonObj.ReadFromFileXml(FileName, "YouMenMinValP"+indexVal);
			if (strVal == null || strVal == "") {
				strVal = "0";
				HandleJsonObj.WriteToFileXml(FileName, "YouMenMinValP"+indexVal, strVal);
			}
			YouMenMinVal[i] = Convert.ToUInt32( strVal );
		}
	}

	public static void SaveYouMenVal(PlayerEnum indexPlayer)
	{
		int indexVal = (int)indexPlayer - 1;
		int playerNum = (int)indexPlayer;
		YouMenMaxVal[indexVal] = YouMenCurVal[indexVal];
		HandleJsonObj.WriteToFileXml(FileName, "YouMenMaxValP"+playerNum, YouMenCurVal[indexVal].ToString());
	}

	void GetPcvrYouMenVal()
	{
		if (!bIsHardWare || IsTestInput) {
			return;
		}
		
		uint youMenCurVal = 0;
		float youMenInput = 0f;
		for (int i = 0; i < 4; i++) {
			youMenCurVal = YouMenCurVal[i];
			if (YouMenMinVal[i] < YouMenMaxVal[i]) {
				//油门正接.
				youMenInput = ((float)youMenCurVal - YouMenMinVal[i]) / (YouMenMaxVal[i] - YouMenMinVal[i]);
			}
			else {
				//油门反接.
				youMenInput = ((float)YouMenMinVal[i] - youMenCurVal) / (YouMenMinVal[i] - YouMenMaxVal[i]);
			}

			youMenInput = Mathf.Clamp01(youMenInput);
			youMenInput = youMenInput >= 0.05f ? 1f : 0f;
			if (SetPanelUiRoot.GetInstance() != null || HardwareCheckCtrl.IsTestHardWare) {
				InputEventCtrl.PlayerYM[i] = youMenInput;
			}
			else {
				InputEventCtrl.PlayerYM[i] = InputEventCtrl.PlayerSC[i] < 0f ? -1f : youMenInput;
			}
		}
	}

	void SetPcvrShaCheInfo(PlayerEnum indexPlayer, float shaCheVal)
	{
		int indexVal = (int)indexPlayer - 1;
		InputEventCtrl.PlayerSC[indexVal] = shaCheVal;
	}

	static void RandomJiaoYanDt()
	{	
		for (int i = 1; i < 4; i++) {
			JiaoYanDt[i] = (byte)UnityEngine.Random.Range(0x00, 0x7b);
		}
		JiaoYanDt[0] = 0x00;
		for (int i = 1; i < 4; i++) {
			JiaoYanDt[0] ^= JiaoYanDt[i];
		}
	}
	
	public void StartJiaoYanIO()
	{
		if (IsJiaoYanHid) {
			return;
		}
		
		if (!HardwareCheckCtrl.IsTestHardWare) {
			if (JiaoYanSucceedCount >= JiaoYanFailedMax) {
				return;
			}
			
			if (JiaoYanState == JIAOYANENUM.FAILED && JiaoYanFailedCount >= JiaoYanFailedMax) {
				return;
			}
		}
		else {
			HardwareCheckCtrl.Instance.SetJiaMiJYMsg("校验中...", JiaMiJiaoYanEnum.Null);
		}
		RandomJiaoYanDt();
		JiaoYanCheckCount = 0;
		IsJiaoYanHid = true;
		CancelInvoke("CloseJiaoYanIO");
		Invoke("CloseJiaoYanIO", 3f);
		//ScreenLog.Log("开始校验...");
	}
	
	void CloseJiaoYanIO()
	{
		if (!IsJiaoYanHid) {
			return;
		}
		IsJiaoYanHid = false;
		OnEndJiaoYanIO(JIAOYANENUM.FAILED);
	}
	
	void OnEndJiaoYanIO(JIAOYANENUM val)
	{
		IsJiaoYanHid = false;
		if (IsInvoking("CloseJiaoYanIO")) {
			CancelInvoke("CloseJiaoYanIO");
		}
		
		switch (val) {
		case JIAOYANENUM.FAILED:
			JiaoYanFailedCount++;
			if (HardwareCheckCtrl.IsTestHardWare) {
				HardwareCheckCtrl.Instance.JiaMiJiaoYanFailed();
			}
			break;
			
		case JIAOYANENUM.SUCCEED:
			JiaoYanSucceedCount++;
			JiaoYanFailedCount = 0;
			if (HardwareCheckCtrl.IsTestHardWare) {
				HardwareCheckCtrl.Instance.JiaMiJiaoYanSucceed();
			}
			break;
		}
		JiaoYanState = val;
		//Debug.Log("Unity:"+"*****JiaoYanState "+JiaoYanState);
		
		if (JiaoYanFailedCount >= JiaoYanFailedMax || IsJiOuJiaoYanFailed) {
			//JiaoYanFailed
			if (IsJiOuJiaoYanFailed) {
				//JiOuJiaoYanFailed
				//Debug.Log("Unity:"+"JOJYSB...");
			}
			else {
				//JiaMiXinPianJiaoYanFailed
				//Debug.Log("Unity:"+"JMXPJYSB...");
				IsJiaMiJiaoYanFailed = true;
			}
		}
	}
	public static bool IsJiaMiJiaoYanFailed;
	
	enum JIAOYANENUM
	{
		NULL,
		SUCCEED,
		FAILED,
	}
	static int JiaoYanCheckCount;
	static JIAOYANENUM JiaoYanState = JIAOYANENUM.NULL;
	static byte JiaoYanFailedMax = 0x01;
	static byte JiaoYanSucceedCount;
	static byte JiaoYanFailedCount;
	static byte[] JiaoYanDt = new byte[4];
	static byte[] JiaoYanMiMa = new byte[4];
	static byte[] JiaoYanMiMaRand = new byte[4];
	byte JiOuJiaoYanCount;
	byte JiOuJiaoYanMax = 5;
	public static bool IsJiOuJiaoYanFailed;
	
	void InitJiaoYanMiMa()
	{
		JiaoYanMiMa[1] = 0x8e; //0x8e
		JiaoYanMiMa[2] = 0xc3; //0xc3
		JiaoYanMiMa[3] = 0xd7; //0xd7
		JiaoYanMiMa[0] = 0x00;
		for (int i = 1; i < 4; i++) {
			JiaoYanMiMa[0] ^= JiaoYanMiMa[i];
		}
	}
	
	void RandomJiaoYanMiMaVal()
	{
		for (int i = 0; i < 4; i++) {
			JiaoYanMiMaRand[i] = (byte)UnityEngine.Random.Range(0x00, (JiaoYanMiMa[i] - 1));
		}
		
		byte TmpVal = 0x00;
		for (int i = 1; i < 4; i++) {
			TmpVal ^= JiaoYanMiMaRand[i];
		}
		
		if (TmpVal == JiaoYanMiMaRand[0]) {
			JiaoYanMiMaRand[0] ^= 0x01; //fix JiaoYanMiMaRand[0].
		}
	}

	public static bool IsPlayerActivePcvr = true;
	public static float TimeLastActivePcvr;
	void CheckIsPlayerActivePcvr()
	{
		if (!IsPlayerActivePcvr) {
			return;
		}
		
		if (Time.realtimeSinceStartup - TimeLastActivePcvr > 60f) {
			IsPlayerActivePcvr = false;
		}
	}
	
	public static void SetIsPlayerActivePcvr()
	{
		if (!bIsHardWare) {
			return;
		}
		IsPlayerActivePcvr = true;
		TimeLastActivePcvr = Time.realtimeSinceStartup;
	}
#endif
#endregion
    
#region pcvr 游戏彩票管理
    /// <summary>
    /// 开始打印彩票.
    /// </summary>
    internal void StartPrintPlayerCaiPiao(PlayerEnum index, int caiPiao)
    {
    }

    /// <summary>
    /// 缺票后重新开始打印彩票.
    /// </summary>
    internal void RestartPrintCaiPiao(PlayerEnum indexPlayer)
    {
    }

    /// <summary>
    /// 工作人员清理彩票不足机台的彩票数据信息.
    /// </summary>
    internal void ClearCaiPiaoData(PlayerEnum indexPlayer)
    {
    }
    #endregion

    public void SubPlayerCoin(PlayerEnum indexPlayer, int subNum)
    {
        if (!bIsHardWare)
        {
            return;
        }

        switch (indexPlayer)
        {
            case PlayerEnum.PlayerOne:
                if (CoinNumCurrentP1 < subNum)
                {
                    return;
                }
                CoinNumCurrentP1 -= subNum;
                break;
            case PlayerEnum.PlayerTwo:
                if (CoinNumCurrentP2 < subNum)
                {
                    return;
                }
                CoinNumCurrentP2 -= subNum;
                break;
            case PlayerEnum.PlayerThree:
                if (CoinNumCurrentP3 < subNum)
                {
                    return;
                }
                CoinNumCurrentP3 -= subNum;
                break;
            case PlayerEnum.PlayerFour:
                if (CoinNumCurrentP4 < subNum)
                {
                    return;
                }
                CoinNumCurrentP4 -= subNum;
                break;
        }
    }
    //	void OnGUI()
    //	{
    //		float width = Screen.width;
    //		float hight = 30f;
    //		string infoA = "Coin1: "+XKGlobalData.CoinPlayerOne
    //				+", Coin2: "+XKGlobalData.CoinPlayerTwo
    //				+", Coin3: "+XKGlobalData.CoinPlayerThree
    //				+", Coin4: "+XKGlobalData.CoinPlayerFour;
    //		GUI.Box(new Rect(0f, hight * 4f, width, hight), infoA);
    //	}
}

public enum PcvrValState
{
	ValMax,
	ValMin,
	ValCenter
}

public enum PcvrShuiBengState
{
	Level_1,
	Level_2
}

public enum LedState
{
	Liang,
	Shan,
	Mie
}