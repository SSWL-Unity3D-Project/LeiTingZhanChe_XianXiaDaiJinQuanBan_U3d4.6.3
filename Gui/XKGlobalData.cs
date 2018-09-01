using UnityEngine;
using System;
using System.IO;

public class XKGlobalData
{
    static int _CoinPlayerOne = 0;
    public static int CoinPlayerOne
    {
        set
        {
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
                    _CoinPlayerThree = pcvr.GetInstance().mPcvrTXManage.PlayerCoinArray[1];
                }
            }
            return _CoinPlayerThree;
        }
    }
    public static int CoinPlayerFour = 0;
	public static int GameNeedCoin;
	/**
	 * GameVersionPlayer == 0 -> 四人版本游戏.
	 * GameVersionPlayer == 1 -> 双人版本游戏.
	 */
	public static int GameVersionPlayer = 0;
	public static bool IsFreeMode;
	public static string GameDiff;
	public static int GameAudioVolume = 10;

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

            Instance.InitIsPrintCaiPiao();
            Instance.InitCoinToCard();
            Instance.InitTotalOutPrintCards();
            Instance.InitTotalInsertCoins();
            Instance.InitZhanCheCaiChi();
            Instance.InitDaoJuCaiChi();
            Instance.InitJPBossCaiChi();
            Instance.InitCaiPiaoPrintState();
        }
		return Instance;
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
    internal int m_YuZhiCaiChi = 0;
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
        int val = 0;
        bool isUpdateYuZhiCaiChi = false;
        if (IsResetYuZhiCaiPiaoData)
        {
            isUpdateYuZhiCaiChi = true;
            IsResetYuZhiCaiPiaoData = false;
            val = yuZhiCaiPiaoBeiLv * m_CoinToCard;
        }
        else
        {
            string info = HandleJsonObj.ReadFromFileXml(FileName, "YuZhiCaiChi");
            if (info == null || info == "")
            {
                val = yuZhiCaiPiaoBeiLv * m_CoinToCard;
                isUpdateYuZhiCaiChi = true;
            }
            else
            {
                val = Convert.ToInt32(info);
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
        int val = m_YuZhiCaiPiaoBeiLv * m_CoinToCard;
        IsResetYuZhiCaiPiaoData = true;
        SetYuZhiCaiChi(val);
    }

    /// <summary>
    /// 设置预支彩池.
    /// </summary>
    public void SetYuZhiCaiChi(int val)
    {
        m_YuZhiCaiChi = val;
        HandleJsonObj.WriteToFileXml(FileName, "YuZhiCaiChi", val.ToString());
    }
    
    /// <summary>
    /// JP大奖彩池.
    /// </summary>
    internal int m_JPBossCaiChi = 0;
    /// <summary>
    /// 初始化JP大奖彩池.
    /// </summary>
    void InitJPBossCaiChi()
    {
        string info = HandleJsonObj.ReadFromFileXml(FileName, "JPBossCaiChi");
        if (info == null || info == "")
        {
            info = "0";
        }

        int val = Convert.ToInt32(info);
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
    public void SetJPBossCaiChi(int val)
    {
        m_JPBossCaiChi = val;
        HandleJsonObj.WriteToFileXml(FileName, "JPBossCaiChi", val.ToString());
    }

    /// <summary>
    /// 道具彩池.
    /// </summary>
    internal int m_DaoJuCaiChi = 0;
    /// <summary>
    /// 初始化道具彩池.
    /// </summary>
    void InitDaoJuCaiChi()
    {
        string info = HandleJsonObj.ReadFromFileXml(FileName, "DaoJuCaiChi");
        if (info == null || info == "")
        {
            info = "0";
        }

        int val = Convert.ToInt32(info);
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
    public void SetDaoJuCaiChi(int val)
    {
        m_DaoJuCaiChi = val;
        HandleJsonObj.WriteToFileXml(FileName, "DaoJuCaiChi", val.ToString());
    }

    /// <summary>
    /// 战车彩池.
    /// </summary>
    internal int m_ZhanCheCaiChi = 0;
    /// <summary>
    /// 初始化战车彩池.
    /// </summary>
    void InitZhanCheCaiChi()
    {
        string info = HandleJsonObj.ReadFromFileXml(FileName, "ZhanCheCaiChi");
        if (info == null || info == "")
        {
            info = "0";
        }

        int val = Convert.ToInt32(info);
        m_ZhanCheCaiChi = val;
    }

    /// <summary>
    /// 重置战车彩池.
    /// </summary>
    public void ResetZhanCheCaiChi()
    {
        SetZhanCheCaiChi(0);
    }

    /// <summary>
    /// 设置战车彩池.
    /// </summary>
    public void SetZhanCheCaiChi(int val)
    {
        m_ZhanCheCaiChi = val;
        HandleJsonObj.WriteToFileXml(FileName, "ZhanCheCaiChi", val.ToString());
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
    /// 一币兑换彩票数.
    /// </summary>
    internal int m_CoinToCard = 20;
    /// <summary>
    /// 初始化1币兑换彩票数.
    /// </summary>
    void InitCoinToCard()
    {
        string info = HandleJsonObj.ReadFromFileXml(FileName, "CoinToCard");
        if (info == null || info == "")
        {
            info = "20";
        }

        int val = Convert.ToInt32(info);
        if (val < 10 || val > 50)
        {
            val = 20;
        }
        m_CoinToCard = val;
    }

    /// <summary>
    /// 设置1币兑换彩票数.
    /// </summary>
    public void SetCoinToCardVal(int val)
    {
        m_CoinToCard = val;
        HandleJsonObj.WriteToFileXml(FileName, "CoinToCard", m_CoinToCard.ToString());
    }

    public static void SetCoinPlayerInfo(PlayerEnum indexPlayer, int coin)
    {
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

		if (coin > 0 && CoinPlayerOne != coin) {
			PlayTouBiAudio();
            if (coin > CoinPlayerOne)
            {
                Instance.SetTotalInsertCoins(Instance.m_TotalInsertCoins + (coin - CoinPlayerOne));
            }
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

		if (coin > 0 && CoinPlayerTwo != coin) {
			PlayTouBiAudio();
            if (coin > CoinPlayerTwo)
            {
                Instance.SetTotalInsertCoins(Instance.m_TotalInsertCoins + (coin - CoinPlayerTwo));
            }
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
		if (coin > 0 && CoinPlayerThree != coin) {
			PlayTouBiAudio();
            if (coin > CoinPlayerThree)
            {
                Instance.SetTotalInsertCoins(Instance.m_TotalInsertCoins + (coin - CoinPlayerThree));
            }
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
		if (coin > 0 && CoinPlayerFour != coin) {
			PlayTouBiAudio();
            if (coin > CoinPlayerFour)
            {
                Instance.SetTotalInsertCoins(Instance.m_TotalInsertCoins + (coin - CoinPlayerFour));
            }
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