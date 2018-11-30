using UnityEngine;
using System;
using System.IO;
using Assets.XKGame.Script.Comm;
using Assets.XKGame.Script.GamePay;
using Assets.XKGame.Script.Server.GamePayManage;

public class XKGlobalData
{
    static int _CoinPlayerOne = 0;
    public static int CoinPlayerOne
    {
        set
        {
            if (pcvr.bIsHardWare == true)
            {
                if (value > 0 && _CoinPlayerOne != value)
                {
                    PlayTouBiAudio();
                    if (value > _CoinPlayerOne)
                    {
                        Instance.SetTotalInsertCoins(Instance.m_TotalInsertCoins + (value - _CoinPlayerOne));
                    }
                }
            }
            _CoinPlayerOne = value;
        }
        get
        {
            if (pcvr.bIsHardWare)
            {
                if (InputEventCtrl.IsUsePcInputTest == true)
                {
                    //测试模式.
                }
                else
                {
                    _CoinPlayerOne = pcvr.GetInstance().mPcvrTXManage.PlayerCoinArray[0];
                }
            }
            return _CoinPlayerOne;
        }
    }
    static int _CoinPlayerTwo = 0;
    public static int CoinPlayerTwo
    {
        set
        {
            if (pcvr.bIsHardWare == true)
            {
                if (value > 0 && _CoinPlayerTwo != value)
                {
                    PlayTouBiAudio();
                    if (value > _CoinPlayerTwo)
                    {
                        Instance.SetTotalInsertCoins(Instance.m_TotalInsertCoins + (value - _CoinPlayerTwo));
                    }
                }
            }
            _CoinPlayerTwo = value;
        }
        get
        {
            if (pcvr.bIsHardWare)
            {
                if (InputEventCtrl.IsUsePcInputTest == true)
                {
                    //测试模式.
                }
                else
                {
                    _CoinPlayerTwo = pcvr.GetInstance().mPcvrTXManage.PlayerCoinArray[1];
                }
            }
            return _CoinPlayerTwo;
        }
    }
    static int _CoinPlayerThree = 0;
    public static int CoinPlayerThree
    {
        set
        {
            if (pcvr.bIsHardWare == true)
            {
                if (value > 0 && _CoinPlayerThree != value)
                {
                    PlayTouBiAudio();
                    if (value > _CoinPlayerThree)
                    {
                        Instance.SetTotalInsertCoins(Instance.m_TotalInsertCoins + (value - _CoinPlayerThree));
                    }
                }
            }
            _CoinPlayerThree = value;
        }
        get
        {
            if (pcvr.bIsHardWare)
            {
                if (InputEventCtrl.IsUsePcInputTest == true)
                {
                    //测试模式.
                }
                else
                {
                    _CoinPlayerThree = pcvr.GetInstance().mPcvrTXManage.PlayerCoinArray[2];
                }
            }
            return _CoinPlayerThree;
        }
    }
    static int _CoinPlayerFour = 0;
    public static int CoinPlayerFour
    {
        set
        {
            if (pcvr.bIsHardWare == true)
            {
                if (value > 0 && _CoinPlayerFour != value)
                {
                    PlayTouBiAudio();
                    if (value > _CoinPlayerFour)
                    {
                        Instance.SetTotalInsertCoins(Instance.m_TotalInsertCoins + (value - _CoinPlayerFour));
                    }
                }
            }
            _CoinPlayerFour = value;
        }
        get
        {
            if (pcvr.bIsHardWare)
            {
                if (InputEventCtrl.IsUsePcInputTest == true)
                {
                    //测试模式.
                }
                else
                {
                    _CoinPlayerFour = pcvr.GetInstance().mPcvrTXManage.PlayerCoinArray[3];
                }
            }
            return _CoinPlayerFour;
        }
    }
    public static int GameNeedCoin;
	/**
	 * GameVersionPlayer == 0 -> 四人版本游戏.
	 * GameVersionPlayer == 1 -> 双人版本游戏.
	 */
	public static int GameVersionPlayer = 0;
	public static bool IsFreeMode;
	public static string GameDiff;
	public static int GameAudioVolume = 10;
    /// <summary>
    /// 红点点游戏盒子编号信息.
    /// </summary>
    internal string m_HddBoxNumInfo = "";
    public enum CaiPiaoPrintState
    {
        /// <summary>
        /// 半票.
        /// </summary>
        BanPiao = 0,
        /// <summary>
        /// 全票.
        /// </summary>
        QuanPiao = 1,
    }
    internal CaiPiaoPrintState m_CaiPiaoPrintState = CaiPiaoPrintState.BanPiao;

    static string FilePath = "";
	static public string FileName = "../config/GameConfig.xml";
    /// <summary>
    /// 彩池信息配置文件.
    /// </summary>
    static public string FileNameCaiChi = "GameConfig.db";
    static public HandleJson HandleJsonObj = null;
	float TimeValDaoDanJingGao;
	static XKGlobalData Instance;
	public static XKGlobalData GetInstance()
	{
		if (Instance == null)
        {
			Instance = new XKGlobalData();
			Instance.InitInfo();

#if UNITY_STANDALONE_WIN
            if (!Directory.Exists(FilePath))
            {
				Directory.CreateDirectory(FilePath);
			}
#endif

            if(HandleJsonObj == null) {
				HandleJsonObj = HandleJson.GetInstance();
			}
			//return Instance;

			string startCoinInfo = HandleJsonObj.ReadFromFileXml(FileName, "START_COIN");
			if(startCoinInfo == null || startCoinInfo == "") {
				startCoinInfo = "1";
				HandleJsonObj.WriteToFileXml(FileName, "START_COIN", startCoinInfo);
			}
			GameNeedCoin = Convert.ToInt32( startCoinInfo );

			string modeGame = HandleJsonObj.ReadFromFileXml(FileName, "GAME_MODE");
			if (modeGame == null || modeGame == "") {
				modeGame = "1";
				HandleJsonObj.WriteToFileXml(FileName, "GAME_MODE", modeGame);
			}
			IsFreeMode = modeGame == "0" ? true : false;

			GameDiff = HandleJsonObj.ReadFromFileXml(FileName, "GAME_DIFFICULTY");
			if (GameDiff == null || GameDiff == "") {
				GameDiff = "1";
				HandleJsonObj.WriteToFileXml(FileName, "GAME_DIFFICULTY", GameDiff);
			}
			
			//string val = HandleJsonObj.ReadFromFileXml(FileName, "GameAudioVolume");
			//if (val == null || val == "") {
			//	val = "7";
			//	HandleJsonObj.WriteToFileXml(FileName, "GameAudioVolume", val);
			//}
            string val = "10";
            GameAudioVolume = Convert.ToInt32(val);

			//val = HandleJsonObj.ReadFromFileXml(FileName, "GameVersionPlayer");
			//if (val == null || val == "") {
			//	val = "0"; //四人版本.
			//	HandleJsonObj.WriteToFileXml(FileName, "GameVersionPlayer", val);
			//}
            val = "0"; //四人版本.
            GameVersionPlayer = Convert.ToInt32(val);
            
            Instance.Init();
        }
		return Instance;
	}

    void Init()
    {
        InitIsPrintCaiPiao();
        InitCoinToCard();
        InitTotalOutPrintCards();
        InitTotalInsertCoins();
        InitZhanCheCaiChi();
        InitDaoJuCaiChi();
        InitJPBossCaiChi();
        InitCaiPiaoPrintState();
        InitGameWXPayDataManage();
        InitDanMuInfo();
        InitHddBoxNumInfo();

        InitCaiChiFanJiangLv();
        InitCaiChiBaoJiangLv();
        InitMianFeiShiWanCount();
        InitGameCoinToMoney();
    }
    
	void InitInfo()
	{
#if UNITY_STANDALONE_WIN
        FilePath = Application.dataPath + "/../config";
#endif

#if UNITY_ANDROID
        FileName = "GameConfig.xml";
#endif
    }

    /// <summary>
    /// 游戏营收数据管理.
    /// </summary>
    internal SSGameWXPayDataManage m_GameWXPayDataManage = null;
    /// <summary>
    /// 初始化游戏微信营收数据信息.
    /// </summary>
    void InitGameWXPayDataManage()
    {
        if (m_GameWXPayDataManage == null)
        {
            m_GameWXPayDataManage = new SSGameWXPayDataManage();
            m_GameWXPayDataManage.Init();
        }
    }

    /// <summary>
    /// 初始化彩票打印半票或全票.
    /// </summary>
    void InitCaiPiaoPrintState()
    {
        int val = 0;
        string info = HandleJsonObj.ReadFromFileXml(FileName, "CaiPiaoPrintState");
        if (info == null || info == "")
        {
            val = 0;
        }
        else
        {
            val = Convert.ToInt32(info);
        }

        m_CaiPiaoPrintState = val == 0 ? CaiPiaoPrintState.BanPiao : CaiPiaoPrintState.QuanPiao;
    }

    /// <summary>
    /// 重置彩票打印为半票.
    /// </summary>
    public void ResetCaiPiaoPrintState()
    {
        m_CaiPiaoPrintState = CaiPiaoPrintState.BanPiao;
        HandleJsonObj.WriteToFileXml(FileName, "CaiPiaoPrintState", "0");
    }

    /// <summary>
    /// 设置打印彩票是半票还是全票.
    /// </summary>
    public void SetCaiPiaoPrintState(CaiPiaoPrintState type)
    {
        m_CaiPiaoPrintState = type;
        HandleJsonObj.WriteToFileXml(FileName, "CaiPiaoPrintState", type == CaiPiaoPrintState.BanPiao ? "0" : "1");
    }

    /// <summary>
    /// 预支彩池.
    /// </summary>
    internal float m_YuZhiCaiChi = 0;
    /// <summary>
    /// 预支彩池倍率.
    /// </summary>
    int m_YuZhiCaiPiaoBeiLv = 100;
    /// <summary>
    /// 是否重置了预支彩池数据.
    /// </summary>
    bool IsResetYuZhiCaiPiaoData = false;
    /// <summary>
    /// 初始化预支彩池.
    /// </summary>
    public bool InitYuZhiCaiChi(int yuZhiCaiPiaoBeiLv)
    {
        float val = 0;
        bool isUpdateYuZhiCaiChi = false;
        if (IsResetYuZhiCaiPiaoData)
        {
            isUpdateYuZhiCaiChi = true;
            IsResetYuZhiCaiPiaoData = false;
            val = (int)(yuZhiCaiPiaoBeiLv * m_CoinToCard);
        }
        else
        {
            string info = HandleJsonObj.ReadFromFileXml(FileNameCaiChi, "YuZhiData"); //预制彩池信息
            if (info == null || info == "")
            {
                val = (int)(yuZhiCaiPiaoBeiLv * m_CoinToCard);
                isUpdateYuZhiCaiChi = true;
            }
            else
            {
                val = MathConverter.StringToFloat(info);
            }
        }
        m_YuZhiCaiPiaoBeiLv = yuZhiCaiPiaoBeiLv;
        m_YuZhiCaiChi = val;

        return isUpdateYuZhiCaiChi;
    }

    /// <summary>
    /// 重置预支彩池.
    /// </summary>
    public void ResetYuZhiCaiChi()
    {
        int val = (int)(m_YuZhiCaiPiaoBeiLv * m_CoinToCard);
        IsResetYuZhiCaiPiaoData = true;
        SetYuZhiCaiChi(val);
    }

    /// <summary>
    /// 设置预支彩池.
    /// </summary>
    public void SetYuZhiCaiChi(float val)
    {
        m_YuZhiCaiChi = val;
        HandleJsonObj.WriteToFileXml(FileNameCaiChi, "YuZhiData", val.ToString()); //预制彩池信息
    }
    
    /// <summary>
    /// JP大奖彩池.
    /// </summary>
    internal float m_JPBossCaiChi = 0;
    /// <summary>
    /// 初始化JP大奖彩池.
    /// </summary>
    void InitJPBossCaiChi()
    {
        string info = HandleJsonObj.ReadFromFileXml(FileNameCaiChi, "JPBossData"); //Boss彩池信息
        if (info == null || info == "")
        {
            info = "0";
        }

        //int val = Convert.ToInt32(info);
        float val = MathConverter.StringToFloat(info);
        m_JPBossCaiChi = val;
    }

    /// <summary>
    /// 重置JP大奖彩池.
    /// </summary>
    public void ResetJPBossCaiChi()
    {
        SetJPBossCaiChi(0);
    }

    /// <summary>
    /// 设置JP大奖彩池.
    /// </summary>
    public void SetJPBossCaiChi(float val)
    {
        m_JPBossCaiChi = val;
        HandleJsonObj.WriteToFileXml(FileNameCaiChi, "JPBossData", val.ToString()); //Boss彩池信息
    }

    /// <summary>
    /// 道具彩池.
    /// </summary>
    internal float m_DaoJuCaiChi = 0;
    /// <summary>
    /// 初始化道具彩池.
    /// </summary>
    void InitDaoJuCaiChi()
    {
        string info = HandleJsonObj.ReadFromFileXml(FileNameCaiChi, "DaoJuData"); //道具彩池信息
        if (info == null || info == "")
        {
            info = "0";
        }

        //int val = Convert.ToInt32(info);
        float val = MathConverter.StringToFloat(info);
        m_DaoJuCaiChi = val;
    }

    /// <summary>
    /// 重置道具彩池.
    /// </summary>
    public void ResetDaoJuCaiChi()
    {
        SetDaoJuCaiChi(0);
    }

    /// <summary>
    /// 设置道具彩池.
    /// </summary>
    public void SetDaoJuCaiChi(float val)
    {
        m_DaoJuCaiChi = val;
        HandleJsonObj.WriteToFileXml(FileNameCaiChi, "DaoJuData", val.ToString()); //道具彩池信息
    }

    /// <summary>
    /// 战车彩池.
    /// 战车01代金券奖池.
    /// </summary>
    internal float m_ZhanCheCaiChi_01 = 0;
    /// <summary>
    /// 战车彩池.
    /// 战车02代金券奖池.
    /// </summary>
    internal float m_ZhanCheCaiChi_02 = 0;
    /// <summary>
    /// 初始化战车彩池.
    /// </summary>
    void InitZhanCheCaiChi()
    {
        string info = HandleJsonObj.ReadFromFileXml(FileNameCaiChi, "ZhanCheData_01"); //战车彩池信息
        if (info == null || info == "")
        {
            info = "0";
        }
        //int val = Convert.ToInt32(info);
        float val = MathConverter.StringToFloat(info);
        m_ZhanCheCaiChi_01 = val;
        
        info = HandleJsonObj.ReadFromFileXml(FileNameCaiChi, "ZhanCheData_02"); //战车彩池信息
        if (info == null || info == "")
        {
            info = "0";
        }
        //int val = Convert.ToInt32(info);
        val = MathConverter.StringToFloat(info);
        m_ZhanCheCaiChi_02 = val;
    }

    /// <summary>
    /// 重置战车彩池.
    /// </summary>
    public void ResetZhanCheCaiChi()
    {
        SetZhanCheCaiChi(0, SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01);
        SetZhanCheCaiChi(0, SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02);
    }

    /// <summary>
    /// 设置战车彩池.
    /// </summary>
    public void SetZhanCheCaiChi(float val, SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState daiJinQuan)
    {
        switch (daiJinQuan)
        {
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01:
                {
                    m_ZhanCheCaiChi_01 = val;
                    HandleJsonObj.WriteToFileXml(FileNameCaiChi, "ZhanCheData_01", val.ToString()); //战车彩池信息
                    break;
                }
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02:
                {
                    m_ZhanCheCaiChi_02 = val;
                    HandleJsonObj.WriteToFileXml(FileNameCaiChi, "ZhanCheData_02", val.ToString()); //战车彩池信息
                    break;
                }
        }
    }

    /// <summary>
    /// 总出票数.
    /// </summary>
    internal int m_TotalOutPrintCards = 0;
    /// <summary>
    /// 初始化总出票数.
    /// </summary>
    void InitTotalOutPrintCards()
    {
        string info = HandleJsonObj.ReadFromFileXml(FileName, "TotalOutPrintCards");
        if (info == null || info == "")
        {
            info = "0";
        }

        int val = Convert.ToInt32(info);
        m_TotalOutPrintCards = val;
    }

    /// <summary>
    /// 重置总出票数.
    /// </summary>
    public void ResetTotalOutPrintCards()
    {
        SetTotalOutPrintCards(0);
    }

    /// <summary>
    /// 设置总出票数.
    /// </summary>
    public void SetTotalOutPrintCards(int val)
    {
        m_TotalOutPrintCards = val;
        HandleJsonObj.WriteToFileXml(FileName, "TotalOutPrintCards", val.ToString());
    }
    
    /// <summary>
    /// 总投币数.
    /// </summary>
    internal int m_TotalInsertCoins = 0;
    /// <summary>
    /// 初始化总投币数.
    /// </summary>
    void InitTotalInsertCoins()
    {
        string info = HandleJsonObj.ReadFromFileXml(FileName, "TotalInsertCoins");
        if (info == null || info == "")
        {
            info = "0";
        }

        int val = Convert.ToInt32(info);
        m_TotalInsertCoins = val;
    }

    /// <summary>
    /// 重置总投币数.
    /// </summary>
    public void ResetTotalInsertCoins()
    {
        SetTotalInsertCoins(0);
    }

    /// <summary>
    /// 设置总投币数.
    /// </summary>
    public void SetTotalInsertCoins(int val)
    {
        m_TotalInsertCoins = val;
        HandleJsonObj.WriteToFileXml(FileName, "TotalInsertCoins", val.ToString());
    }

    /// <summary>
    /// 是否打印彩票.
    /// </summary>
    internal bool IsPrintCaiPiao = true;
    /// <summary>
    /// 初始化是否打印彩票.
    /// </summary>
    void InitIsPrintCaiPiao()
    {
        string info = HandleJsonObj.ReadFromFileXml(FileName, "PrintCard");
        if (info == null || info == "")
        {
            info = "0";
        }
        
        if (info == "0")
        {
            //初始化打印彩票信息.
            IsPrintCaiPiao = true;
        }
        else
        {
            IsPrintCaiPiao = false;
        }
    }

    /// <summary>
    /// 设置是否打印彩票.
    /// </summary>
    public void SetIsPrintCaiPiao(bool isPrintCaiPiao)
    {
        IsPrintCaiPiao = isPrintCaiPiao;
        HandleJsonObj.WriteToFileXml(FileName, "PrintCard", IsPrintCaiPiao == true ? "0" : "1");
    }

    /// <summary>
    /// 弹幕信息.
    /// </summary>
    internal string m_DanMuInfo = "";
    /// <summary>
    /// 默认弹幕信息.
    /// </summary>
    internal string m_MoRenDanMuInfo = "代金券送不停";
    /// <summary>
    /// 初始化弹幕信息.
    /// </summary>
    void InitDanMuInfo()
    {
        if (HandleJsonObj != null)
        {
            string info = HandleJsonObj.ReadFromFileXml(FileName, "DanMuInfo");
            if (info == null || info == "")
            {
                info = m_MoRenDanMuInfo;
                HandleJsonObj.WriteToFileXml(FileName, "DanMuInfo", info);
            }
            m_DanMuInfo = info;
        }
    }

    /// <summary>
    /// 设置弹幕信息.
    /// </summary>
    internal void SetDanMuInfo(string info)
    {
        if (HandleJsonObj != null)
        {
            HandleJsonObj.WriteToFileXml(FileName, "DanMuInfo", info);
        }
        m_DanMuInfo = info;
    }

    /// <summary>
    /// 服务器更新游戏弹幕信息.
    /// </summary>
    internal void UpdateDanMuInfo(string info)
    {
        SetDanMuInfo(info);
        if (SSUIRoot.GetInstance().m_GameUIManage != null
            && SSUIRoot.GetInstance().m_GameUIManage.m_DanMuTextUI != null)
        {
            SSUIRoot.GetInstance().m_GameUIManage.m_DanMuTextUI.UpdateDanMuInfo();
        }
    }

    /// <summary>
    /// 初始化红点点游戏盒子编号信息.
    /// </summary>
    void InitHddBoxNumInfo()
    {
        if (HandleJsonObj != null)
        {
            string info = HandleJsonObj.ReadFromFileXml(FileName, "HddBoxNumInfo");
            if (info == null || info == "")
            {
                info = "0123456";
                HandleJsonObj.WriteToFileXml(FileName, "HddBoxNumInfo", info);
            }
            m_HddBoxNumInfo = info;
        }
    }

    /// <summary>
    /// 设置红点点游戏盒子编号信息.
    /// </summary>
    internal void SetHddBoxNumInfo(string info)
    {
        if (HandleJsonObj != null)
        {
            HandleJsonObj.WriteToFileXml(FileName, "HddBoxNumInfo", info);
        }
        m_HddBoxNumInfo = info;
    }


    float _CoinToCard = 1;
    /// <summary>
    /// 一币兑换彩票数.
    /// 1币兑换代金券数(1币等于1张1元代金券).
    /// 1币等于多少人民币代金券,默认一币等于2元(单位是元)
    /// </summary>
    internal float m_CoinToCard
    {
        get { return _CoinToCard; }
        set
        {
            _CoinToCard = value;
        }
    }

    /// <summary>
    /// 游戏商户信息配置.
    /// </summary>
    internal SSGameHddPayData.GameShangHuData m_ShangHuDt;
    /// <summary>
    /// 游戏彩池返奖率数据信息.
    /// </summary>
    internal float m_CaiChiFanJiangLv;
    /// <summary>
    /// 初始化彩池返奖率信息.
    /// </summary>
    void InitCaiChiFanJiangLv()
    {
        string info = HandleJsonObj.ReadFromFileXml(FileNameCaiChi, "CaiChiFanJiangLv");
        if (info == null || info == "")
        {
            info = "0.5";
        }

        float val = MathConverter.StringToFloat(info);
        if (val < 0f || val > 1f)
        {
            val = 0.5f;
        }
        m_CaiChiFanJiangLv = val;
    }

    /// <summary>
    /// 设置彩池返奖率信息.
    /// </summary>
    internal void SetCaiChiFanJiangLv(float args)
    {
        m_CaiChiFanJiangLv = args;
        HandleJsonObj.WriteToFileXml(FileNameCaiChi, "CaiChiFanJiangLv", args.ToString());
        SSDebug.Log("SetCaiChiFanJiangLv -> m_CaiChiFanJiangLv ============== " + m_CaiChiFanJiangLv);
    }


    /// <summary>
    /// 游戏免费试玩次数信息.
    /// </summary>
    internal int m_MianFeiShiWanCount;
    /// <summary>
    /// 初始化免费试玩次数信息.
    /// </summary>
    void InitMianFeiShiWanCount()
    {
        string info = HandleJsonObj.ReadFromFileXml(FileNameCaiChi, "MianFeiShiWanCount");
        if (info == null || info == "")
        {
            info = "0";
            SetMianFeiShiWanCount(0);
        }
        else
        {
            int val = Convert.ToInt32(info);
            if (val < 0 || val > 1)
            {
                val = 0;
            }
            m_MianFeiShiWanCount = val;
        }

        //更新游戏免费试玩信息.
        pcvr.GetInstance().m_HongDDGamePadInterface.UpdateMianFeiCountInfo(m_MianFeiShiWanCount);
    }

    /// <summary>
    /// 设置免费试玩次数信息.
    /// </summary>
    internal void SetMianFeiShiWanCount(int args)
    {
        m_MianFeiShiWanCount = args;
        HandleJsonObj.WriteToFileXml(FileNameCaiChi, "MianFeiShiWanCount", args.ToString());
        SSDebug.Log("SetMianFeiShiWanCount -> m_MianFeiShiWanCount ============== " + m_MianFeiShiWanCount);
    }
    
    /// <summary>
    /// 随机道具爆奖率.
    /// 1元代金券.
    /// </summary>
    internal float SuiJiDaoJuBaoJiangLv = 0f;
    /// <summary>
    /// 战车得彩爆奖率.
    /// 战车代金券01低面额.
    /// </summary>
    internal float ZhanCheBaoJiangLv_01 = 0f;
    /// <summary>
    /// 战车得彩爆奖率.
    /// 战车代金券02高面额.
    /// </summary>
    internal float ZhanCheBaoJiangLv_02 = 0f;
    /// <summary>
    /// JPBoss爆奖率.
    /// 100元代金券.
    /// </summary>
    internal float JPBossBaoJiangLv = 0.3f;
    /// <summary>
    /// 初始化彩池爆奖率信息.
    /// </summary>
    void InitCaiChiBaoJiangLv()
    {
        string info = "";
        float val = 0f;
        info = HandleJsonObj.ReadFromFileXml(FileNameCaiChi, "SuiJiDaoJuBaoJiangLv");
        if (info == null || info == "")
        {
            info = "0";
        }

        val = MathConverter.StringToFloat(info);
        if (val < 0f || val > 1f)
        {
            val = 0f;
        }
        SuiJiDaoJuBaoJiangLv = val;

        info = HandleJsonObj.ReadFromFileXml(FileNameCaiChi, "ZhanCheBaoJiangLv_01");
        if (info == null || info == "")
        {
            info = "0";
        }

        val = MathConverter.StringToFloat(info);
        if (val < 0f || val > 1f)
        {
            val = 0f;
        }
        ZhanCheBaoJiangLv_01 = val;

        info = HandleJsonObj.ReadFromFileXml(FileNameCaiChi, "ZhanCheBaoJiangLv_02");
        if (info == null || info == "")
        {
            info = "0";
        }

        val = MathConverter.StringToFloat(info);
        if (val < 0f || val > 1f)
        {
            val = 0f;
        }
        ZhanCheBaoJiangLv_02 = val;

        info = HandleJsonObj.ReadFromFileXml(FileNameCaiChi, "JPBossBaoJiangLv");
        if (info == null || info == "")
        {
            info = "0.3";
        }

        val = MathConverter.StringToFloat(info);
        if (val < 0f || val > 1f)
        {
            val = 0.3f;
        }
        JPBossBaoJiangLv = val;
    }

    /// <summary>
    /// 设置彩池爆奖率信息.
    /// </summary>
    internal void SetCaiChiBaoJiangLv(float suiJiBaoJiangLv, float zhanCheBaoJiangLv_01, float zhanCheBaoJiangLv_02, float jpBossBaoJiangLv)
    {
        SuiJiDaoJuBaoJiangLv = suiJiBaoJiangLv;
        HandleJsonObj.WriteToFileXml(FileNameCaiChi, "SuiJiDaoJuBaoJiangLv", SuiJiDaoJuBaoJiangLv.ToString());

        ZhanCheBaoJiangLv_01 = zhanCheBaoJiangLv_01;
        HandleJsonObj.WriteToFileXml(FileNameCaiChi, "ZhanCheBaoJiangLv_01", ZhanCheBaoJiangLv_01.ToString());

        ZhanCheBaoJiangLv_02 = zhanCheBaoJiangLv_02;
        HandleJsonObj.WriteToFileXml(FileNameCaiChi, "ZhanCheBaoJiangLv_02", ZhanCheBaoJiangLv_02.ToString());

        JPBossBaoJiangLv = jpBossBaoJiangLv;
        HandleJsonObj.WriteToFileXml(FileNameCaiChi, "JPBossBaoJiangLv", JPBossBaoJiangLv.ToString());

        SSDebug.Log("SetCaiChiBaoJiangLv -> SuiJiDaoJuBaoJiangLv ============== " + SuiJiDaoJuBaoJiangLv);
        SSDebug.Log("SetCaiChiBaoJiangLv -> ZhanCheBaoJiangLv_01 ============== " + ZhanCheBaoJiangLv_01);
        SSDebug.Log("SetCaiChiBaoJiangLv -> ZhanCheBaoJiangLv_02 ============== " + ZhanCheBaoJiangLv_02);
        SSDebug.Log("SetCaiChiBaoJiangLv -> JPBossBaoJiangLv ============== " + JPBossBaoJiangLv);
    }
    
    /// <summary>
    /// 根据代金券分类获取对应的爆奖率数据.
    /// </summary>
    internal float GetDaiJinQuanBaoJiangLv(SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState type)
    {
        float val = 0f;
        switch (type)
        {
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.SuiJiDaoJuDaiJinQuan:
                {
                    val = SuiJiDaoJuBaoJiangLv;
                    break;
                }
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_01:
                {
                    val = ZhanCheBaoJiangLv_01;
                    break;
                }
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.ZhanCheDaiJinQuan_02:
                {
                    val = ZhanCheBaoJiangLv_02;
                    break;
                }
            case SSCaiPiaoDataManage.GameCaiPiaoData.DaiJinQuanState.JPBossDaiJinQuan:
                {
                    val = JPBossBaoJiangLv;
                    break;
                }
        }
        return val;
    }

    /// <summary>
    /// 初始化1币兑换彩票数.
    /// 1币等于多少面值人民币,单位为元.(默认值为2元)
    /// </summary>
    void InitCoinToCard()
    {
        string info = HandleJsonObj.ReadFromFileXml(FileName, "CoinToCard");
        if (info == null || info == "")
        {
            info = "2";
            SetCoinToCardVal(2f);
        }
        else
        {
            float val = MathConverter.StringToFloat(info);
            if (val < 0.01f || val > 100f)
            {
                val = 2;
            }
            m_CoinToCard = val;
        }
    }

    /// <summary>
    /// 设置1币兑换彩票数.
    /// </summary>
    public void SetCoinToCardVal(float val)
    {
        m_CoinToCard = val;
        HandleJsonObj.WriteToFileXml(FileName, "CoinToCard", m_CoinToCard.ToString());
    }

    /// <summary>
    /// 游戏币和红点点金币的转换率.
    /// 1币 == 2元人民币 == 200分.
    /// 游戏币转换为红点点账户金币（单位为分）.
    /// </summary>
    internal int GameCoinToMoney = 1;
    /// <summary>
    /// 初始化游戏币和红点点金币的转换率.
    /// 1币 == 2元人民币 == 200分.
    /// </summary>
    void InitGameCoinToMoney()
    {
        string info = HandleJsonObj.ReadFromFileXml(FileName, "GameCoinToMoney");
        if (info == null || info == "")
        {
            info = "200";
            SetGameCoinToMoneyVal(200);
        }
        else
        {
            int val = Convert.ToInt32(info);
            if (val < 1 || val > 10000)
            {
                val = 200;
            }
            GameCoinToMoney = val;
        }
        SSDebug.Log("InitGameCoinToMoney -> GameCoinToMoney =========== " + GameCoinToMoney);
    }

    /// <summary>
    /// 设置游戏币和红点点金币的转换率..
    /// </summary>
    public void SetGameCoinToMoneyVal(int val)
    {
        GameCoinToMoney = val;
        HandleJsonObj.WriteToFileXml(FileName, "GameCoinToMoney", val.ToString());
    }

    /// <summary>
    /// 获取玩家币值信息.
    /// </summary>
    public int GetCoinPlayer(PlayerEnum indexPlayer)
    {
        int coin = 0;
        switch (indexPlayer)
        {
            case PlayerEnum.PlayerOne:
                {
                    coin = CoinPlayerOne;
                    break;
                }
            case PlayerEnum.PlayerTwo:
                {
                    coin = CoinPlayerTwo;
                    break;
                }
            case PlayerEnum.PlayerThree:
                {
                    coin = CoinPlayerThree;
                    break;
                }
            case PlayerEnum.PlayerFour:
                {
                    coin = CoinPlayerFour;
                    break;
                }
        }
        return coin;
    }

    public static void SetCoinPlayerInfo(PlayerEnum indexPlayer, int coin)
    {
        //Debug.Log("SetCoinPlayerInfo ----------> indexPlayer == " + indexPlayer + ", coin == " + coin);
        switch (indexPlayer)
        {
            case PlayerEnum.PlayerOne:
                {
                    SetCoinPlayerOne(coin);
                    break;
                }
            case PlayerEnum.PlayerTwo:
                {
                    SetCoinPlayerTwo(coin);
                    break;
                }
            case PlayerEnum.PlayerThree:
                {
                    SetCoinPlayerThree(coin);
                    break;
                }
            case PlayerEnum.PlayerFour:
                {
                    SetCoinPlayerFour(coin);
                    break;
                }
        }
    }

    public static void SetCoinPlayerOne(int coin)
	{
		if (XKGlobalData.GameVersionPlayer != 0) {
			CoinPlayerOne = coin;
			SetCoinPlayerThree(coin);
			return;
		}

        if (pcvr.bIsHardWare == false)
        {
            if (coin > 0 && CoinPlayerOne != coin)
            {
                PlayTouBiAudio();
                if (coin > CoinPlayerOne)
                {
                    Instance.SetTotalInsertCoins(Instance.m_TotalInsertCoins + (coin - CoinPlayerOne));
                }
            }
        }

        if (coin < 0)
        {
            coin = 0;
        }

		CoinPlayerOne = coin;
		if (CoinPlayerCtrl.GetInstanceOne() != null) {
			CoinPlayerCtrl.GetInstanceOne().SetPlayerCoin(coin);
		}
		
		if (SetPanelUiRoot.GetInstance() != null) {
			SetPanelUiRoot.GetInstance().SetCoinStartLabelInfo(1);
		}
	}

	public static void SetCoinPlayerTwo(int coin)
	{
		if (XKGlobalData.GameVersionPlayer != 0) {
			CoinPlayerTwo = coin;
			SetCoinPlayerFour(coin);
			return;
		}

        if (pcvr.bIsHardWare == false)
        {
            if (coin > 0 && CoinPlayerTwo != coin)
            {
                PlayTouBiAudio();
                if (coin > CoinPlayerTwo)
                {
                    Instance.SetTotalInsertCoins(Instance.m_TotalInsertCoins + (coin - CoinPlayerTwo));
                }
            }
        }

        if (coin < 0)
        {
            coin = 0;
        }

        CoinPlayerTwo = coin;
		if (CoinPlayerCtrl.GetInstanceTwo() != null) {
			CoinPlayerCtrl.GetInstanceTwo().SetPlayerCoin(coin);
		}

		if (SetPanelUiRoot.GetInstance() != null) {
			SetPanelUiRoot.GetInstance().SetCoinStartLabelInfo(1);
		}
	}
	
	public static void SetCoinPlayerThree(int coin)
	{
        if (pcvr.bIsHardWare == false)
        {
            if (coin > 0 && CoinPlayerThree != coin)
            {
                PlayTouBiAudio();
                if (coin > CoinPlayerThree)
                {
                    Instance.SetTotalInsertCoins(Instance.m_TotalInsertCoins + (coin - CoinPlayerThree));
                }
            }
        }

        if (coin < 0)
        {
            coin = 0;
        }

        CoinPlayerThree = coin;
		if (CoinPlayerCtrl.GetInstanceThree() != null) {
			CoinPlayerCtrl.GetInstanceThree().SetPlayerCoin(coin);
		}
		
		if (SetPanelUiRoot.GetInstance() != null) {
			SetPanelUiRoot.GetInstance().SetCoinStartLabelInfo(1);
		}
	}

	public static void SetCoinPlayerFour(int coin)
	{
        if (pcvr.bIsHardWare == false)
        {
            if (coin > 0 && CoinPlayerFour != coin)
            {
                PlayTouBiAudio();
                if (coin > CoinPlayerFour)
                {
                    Instance.SetTotalInsertCoins(Instance.m_TotalInsertCoins + (coin - CoinPlayerFour));
                }
            }
        }

        if (coin < 0)
        {
            coin = 0;
        }

        CoinPlayerFour = coin;
		if (CoinPlayerCtrl.GetInstanceFour() != null) {
			CoinPlayerCtrl.GetInstanceFour().SetPlayerCoin(coin);
		}
		
		if (SetPanelUiRoot.GetInstance() != null) {
			SetPanelUiRoot.GetInstance().SetCoinStartLabelInfo(1);
		}
	}

	public static void SetGameNeedCoin(int coin)
	{
		GameNeedCoin = coin;
		CoinPlayerCtrl.GetInstanceOne().SetGameNeedCoin(coin);
		CoinPlayerCtrl.GetInstanceTwo().SetGameNeedCoin(coin);
	}
	
	public static void PlayAudioSetMove()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASSetMove);
	}

	public static void PlayAudioSetEnter()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASSetEnter);
	}

	static void PlayTouBiAudio()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASTouBi);
	}

	public void PlayStartBtAudio()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASStartBt);
	}
	
	public void PlayModeBeiJingAudio()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASModeBeiJing, 2);
	}
	
	public void StopModeBeiJingAudio()
	{
		AudioListCtrl.StopLoopAudio(AudioListCtrl.GetInstance().ASModeBeiJing);
	}

	public void PlayModeXuanZeAudio()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASModeXuanZe);
	}
	
	public void PlayModeQueRenAudio()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASModeQueRen);
	}

	public void PlayGuanKaBeiJingAudio(int indexBeiJingAd = 0)
	{
		indexBeiJingAd = indexBeiJingAd % AudioListCtrl.GetInstance().ASGuanKaBJ.Length;
		int audioIndex = indexBeiJingAd;
//		int audioIndex = Application.loadedLevel - 1;
//		if (XkGameCtrl.GetInstance().IsCartoonShootTest) {
//			audioIndex = 1; //test
//		}

		if (AudioListCtrl.GetInstance().ASGuanKaBJ[audioIndex] != null) {
			AudioSource audioVal = AudioListCtrl.GetInstance().ASGuanKaBJ[audioIndex].gameObject.AddComponent<AudioSource>();
			audioVal.clip = AudioListCtrl.GetInstance().ASGuanKaBJ[audioIndex].clip;
            AudioBeiJingCtrl beiJingAudio = AudioListCtrl.GetInstance().ASGuanKaBJ[audioIndex].GetComponent<AudioBeiJingCtrl>();
            if (beiJingAudio != null)
            {
                audioVal.volume = beiJingAudio.m_VolumeStart;
                //Debug.Log("Unity: volume ================= " + beiJingAudio.m_VolumeStart);
            }
            //audioVal.volume = AudioListCtrl.GetInstance().ASGuanKaBJ[audioIndex].volume;

			AudioListCtrl.GetInstance().RemoveAudioSource(AudioListCtrl.GetInstance().ASGuanKaBJ[audioIndex]);
			AudioListCtrl.GetInstance().ASGuanKaBJ[audioIndex] = audioVal;
		}
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASGuanKaBJ[audioIndex], 2);
	}

	public void PlayDaoDanJingGaoAudio()
	{
		if (Time.realtimeSinceStartup - TimeValDaoDanJingGao < 0.5f) {
			return;
		}
		TimeValDaoDanJingGao = Time.realtimeSinceStartup;
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASDaoDanJingGao, 1);
	}

	public void PlayJiaYouBaoZhaAudio()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASJiaYouBaoZha);
	}

	public void PlayAudioRanLiaoJingGao()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASRanLiaoJingGao, 2);
	}

	public void StopAudioRanLiaoJingGao()
	{
		AudioListCtrl.StopLoopAudio(AudioListCtrl.GetInstance().ASRanLiaoJingGao);
	}

	public void PlayAudioBossLaiXi()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASBossLaiXi);
		//AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASBossLaiXi, 2);
	}
	
	public void StopAudioBossLaiXi()
	{
		AudioListCtrl.StopLoopAudio(AudioListCtrl.GetInstance().ASBossLaiXi);
	}
	
	public void PlayAudioHitBuJiBao()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASHitBuJiBao);
	}
	
	public void PlayAudioStage1()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASStage1);
	}

	public void PlayAudioStage2()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASStage2);
	}

	public void PlayAudioRenWuOver()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASRenWuOver);
		MakeAudioVolumeDown();
	}

	public void PlayAudioBossShengLi()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASBossShengLi);
	}
	
	public void PlayAudioXuanYaDiaoLuo()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASXuanYaDiaoLuo);
	}

	public void PlayAudioQuanBuTongGuan()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASQuanBuTongGuan);
	}

	public void PlayAudioGameOver()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASGameOver);
		MakeAudioVolumeDown();
	}

	void MakeAudioVolumeDown()
	{
		int loadLevelNum = Application.loadedLevel - 1;
		if (loadLevelNum < 0 || loadLevelNum > 3) {
			loadLevelNum = 0;
		}
		AudioListCtrl.StopLoopAudio(AudioListCtrl.GetInstance().ASGuanKaBJ[loadLevelNum], 1);
	}
	
	public void PlayAudioXuBiDaoJiShi()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASXuBiDaoJiShi);
	}
	
	public void PlayAudioXunZhangJB()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASXunZhangJB);
	}

	public void PlayAudioXunZhangZP()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASXunZhangZP);
	}

	public void PlayAudioJiFenGunDong()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASJiFenGunDong, 2);
	}
	
	public void StopAudioJiFenGunDong()
	{
		AudioListCtrl.StopLoopAudio(AudioListCtrl.GetInstance().ASJiFenGunDong);
	}

	public void PlayAudioZhunXingTX()
	{
		AudioListCtrl.PlayAudioSource(AudioListCtrl.GetInstance().ASZhunXingTX);
	}
}