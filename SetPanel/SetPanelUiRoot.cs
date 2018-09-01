using UnityEngine;
using System;

public class SetPanelUiRoot : MonoBehaviour
{
	public UILabel CoinStartLabel;
	public GameObject DuiGouDiffLow;
	public GameObject DuiGouDiffMiddle;
	public GameObject DuiGouDiffHigh;
	
	public GameObject DuiGouYunYingMode;
	public GameObject DuiGouFreeMode;

	GameObject DuiGouTextCh;
	GameObject DuiGouTextEn;
	public Transform StarTran;
	
	UILabel GameAudioVolumeLB;
	int GameAudioVolume;
	GameObject StarObj;
	
	enum PanelState
	{
		SetPanel,
		JiaoYanPanel,
		CeShiPanel
	}
	PanelState PanelStVal = PanelState.SetPanel;
	
	int StarMoveCount;
	int GameDiffState;
	bool IsFreeGameMode;
	string FileName = XKGlobalData.FileName;
	HandleJson HandleJsonObj;

	Vector3 [] SetPanelStarPos = new Vector3[11]{
        new Vector3(-565f, 265f, 0f),
        new Vector3(-565f, 211f, 0f),
        new Vector3(-565f, 160f, 0f),
        new Vector3(-565f, 103f, 0f),
        new Vector3(-565f, 49f, 0f),
        new Vector3(-565f, -4f, 0f),
        new Vector3(-565f, -58f, 0f),
        new Vector3(108f, -58f, 0f),
        new Vector3(-11f, -112f, 0f),
        new Vector3(191f, -112f, 0f),
        new Vector3(400f, -112f, 0f),
    };

	enum SelectSetPanelDate
	{
		CoinStart = 1,
		GameMode,
		GameDiff,
        CoinToCard,
        CardMode,
        CaiPiaoPrintState,
		//GameLanguage,
		ResetFactory,
		//GameAudioSet,
		//GameAudioReset,
		Exit,
        CaiPiaoJiP1,
        CaiPiaoJiP2,
        CaiPiaoJiP3,
    }
	string startCoinInfo = "";

	enum AdjustDirState
	{
		DirectionRight = 0,
		DirectionCenter,
		DirectionLeft
	}
	//AdjustDirState AdjustDirSt = AdjustDirState.DirectionRight;
    
	bool IsMoveStar = true;
	public static SetPanelUiRoot _Instance;
	public static SetPanelUiRoot GetInstance()
	{
		return _Instance;
	}

	// Use this for initialization
	void Start () {
		_Instance = this;
		if(HandleJsonObj == null) {
			HandleJsonObj = HandleJson.GetInstance();
		}
		Time.timeScale = 1.0f;
		GameOverCtrl.IsShowGameOver = false;
		//pcvr.DongGanState = 1;
		StarObj = StarTran.gameObject;
		XkGameCtrl.SetActivePlayerOne(false);
		XkGameCtrl.SetActivePlayerTwo(false);
		XkGameCtrl.SetActivePlayerThree(false);
		XkGameCtrl.SetActivePlayerFour(false);
		//SetGameTextInfo();

		SetStarObjActive(true);

		InitHandleJson();
		InitStarImgPos();
		InitCoinStartLabel();
		InitGameDiffDuiGou();
		InitGameModeDuiGou();
        InitCoinToCard();
        InitPrintCaiPiaoUI();
        InitCaiPiaoPrintState();
        //InitGameAudioValue();

        InputEventCtrl.GetInstance().ClickSetEnterBtEvent += ClickSetEnterBtEvent;
		InputEventCtrl.GetInstance().ClickSetMoveBtEvent += ClickSetMoveBtEvent;

		InputEventCtrl.GetInstance().ClickFireBtOneEvent += ClickFireBtOneEvent;
		InputEventCtrl.GetInstance().ClickFireBtTwoEvent += ClickFireBtTwoEvent;
		InputEventCtrl.GetInstance().ClickFireBtThreeEvent += ClickFireBtThreeEvent;

		InputEventCtrl.GetInstance().ClickDaoDanBtOneEvent += ClickDaoDanBtOneEvent;
		InputEventCtrl.GetInstance().ClickDaoDanBtTwoEvent += ClickDaoDanBtTwoEvent;
		InputEventCtrl.GetInstance().ClickDaoDanBtThreeEvent += ClickDaoDanBtThreeEvent;

		InputEventCtrl.GetInstance().ClickStartBtOneEvent += ClickStartBtEventP1;
		InputEventCtrl.GetInstance().ClickStartBtTwoEvent += ClickStartBtEventP2;
		InputEventCtrl.GetInstance().ClickStartBtThreeEvent += ClickStartBtEventP3;

        //InputEventCtrl.GetInstance().click += ClickStartBtEventP1;
        //InputEventCtrl.GetInstance().ClickStartBtTwoEvent += ClickStartBtEventP2;
        //InputEventCtrl.GetInstance().ClickStartBtThreeEvent += ClickStartBtEventP3;
    }

	void Update()
	{
		if (SetBtSt == pcvr.ButtonState.DOWN && Time.time - TimeSetMoveBt > 1f && Time.frameCount % 200 == 0) {
			MoveStarImg();
		}

		UpdateDirTestInfo();
        UpdateTotalOutPrintCards();
        UpdateTotalInsertCoins();
    }

	void ClickSetEnterBtEvent(pcvr.ButtonState val)
	{
		if(val == pcvr.ButtonState.UP)
        {
			return;
		}
		HanldeClickEnterBtEvent();
	}

	float TimeSetMoveBt;
	pcvr.ButtonState SetBtSt = pcvr.ButtonState.UP;
	void ClickSetMoveBtEvent(pcvr.ButtonState val)
	{
		SetBtSt = val;
		if (val == pcvr.ButtonState.DOWN) {
			TimeSetMoveBt = Time.time;
			return;
		}

		if (Time.time - TimeSetMoveBt > 1f) {
			return;
		}
		MoveStarImg();
	}

	void ClickFireBtOneEvent(pcvr.ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerOne, 0, val);
	}

	void ClickFireBtTwoEvent(pcvr.ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerTwo, 0, val);
	}

	void ClickFireBtThreeEvent(pcvr.ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerThree, 0, val);
	}

	void ClickFireBtFourEvent(pcvr.ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerFour, 0, val);
	}

	void ClickDaoDanBtOneEvent(pcvr.ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerOne, 1, val);
	}
	
	void ClickDaoDanBtTwoEvent(pcvr.ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerTwo, 1, val);
	}
	
	void ClickDaoDanBtThreeEvent(pcvr.ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerThree, 1, val);
	}
	
	void ClickDaoDanBtFourEvent(pcvr.ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerFour, 1, val);
	}

	void ClickStartBtEventP1(pcvr.ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerOne, 2, val);
	}

	void ClickStartBtEventP2(pcvr.ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerTwo, 2, val);
	}

	void ClickStartBtEventP3(pcvr.ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerThree, 2, val);
	}

	void ClickStartBtEventP4(pcvr.ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerFour, 2, val);
	}

	void SetStarObjActive(bool isActive)
	{
		StarObj.SetActive(isActive);
	}

	void InitCoinStartLabel()
	{
		startCoinInfo = HandleJsonObj.ReadFromFileXml(FileName, "START_COIN");
		if(startCoinInfo == null || startCoinInfo == "")
		{
			startCoinInfo = "1";
			HandleJsonObj.WriteToFileXml(FileName, "START_COIN", startCoinInfo);
		}
		XKGlobalData.GameNeedCoin = Convert.ToInt32( startCoinInfo );

		SetCoinStartLabelInfo();
		SetCoinStartLabelInfo(1);
	}

	public void SetCoinStartLabelInfo(int key = 0)
	{
		switch (key) {
		case 0:
			HandleJsonObj.WriteToFileXml(FileName, "START_COIN", XKGlobalData.GameNeedCoin.ToString());
			CoinStartLabel.text = XKGlobalData.GameNeedCoin.ToString("d2");
			break;
		default:
			UpdatePlayerCoinInfo();
			break;
		}
	}

	void InitHandleJson()
	{
		XKGlobalData.GetInstance();
		FileName = XKGlobalData.FileName;
		HandleJsonObj = XKGlobalData.HandleJsonObj;
	}

	void InitGameDiffDuiGou()
	{
		string diffStr = HandleJsonObj.ReadFromFileXml(FileName, "GAME_DIFFICULTY");
		if(diffStr == null || diffStr == "")
		{
			diffStr = "1";
			HandleJsonObj.WriteToFileXml(FileName, "GAME_DIFFICULTY", diffStr);
		}
		XKGlobalData.GameDiff = diffStr;

		SetGameDiffState();
	}

	void SetGameDiffState()
	{
		switch (XKGlobalData.GameDiff)
		{
		case "0":
			DuiGouDiffLow.SetActive(true);
			DuiGouDiffMiddle.SetActive(false);
			DuiGouDiffHigh.SetActive(false);
			GameDiffState = 0;
			break;
			
		case "1":
			DuiGouDiffLow.SetActive(false);
			DuiGouDiffMiddle.SetActive(true);
			DuiGouDiffHigh.SetActive(false);
			GameDiffState = 1;
			break;
			
		case "2":
			DuiGouDiffLow.SetActive(false);
			DuiGouDiffMiddle.SetActive(false);
			DuiGouDiffHigh.SetActive(true);
			GameDiffState = 2;
			break;

		default:
			XKGlobalData.GameDiff = "1";
			DuiGouDiffLow.SetActive(false);
			DuiGouDiffMiddle.SetActive(true);
			DuiGouDiffHigh.SetActive(false);
			GameDiffState = 1;
			break;
		}
		HandleJsonObj.WriteToFileXml(FileName, "GAME_DIFFICULTY", XKGlobalData.GameDiff);
		GameDiffState++;
	}

	void InitGameModeDuiGou()
	{
		bool isFreeModeTmp = false;
		string modeGame = HandleJsonObj.ReadFromFileXml(FileName, "GAME_MODE");
		if(modeGame == null || modeGame == "")
		{
			modeGame = "1";
			HandleJsonObj.WriteToFileXml(FileName, "GAME_MODE", modeGame);
		}
		
		if(modeGame == "0")
		{
			isFreeModeTmp = true;
		}
		XKGlobalData.IsFreeMode = isFreeModeTmp;
		
		SetGameModeState();
	}

	void SetGameModeState()
	{
		string modeGame = "";
		if (XKGlobalData.IsFreeMode) {
			modeGame = "0";
		}
		else {
			modeGame = "1";
		}

		DuiGouYunYingMode.SetActive(!XKGlobalData.IsFreeMode);
		DuiGouFreeMode.SetActive(XKGlobalData.IsFreeMode);
		IsFreeGameMode = XKGlobalData.IsFreeMode;
		HandleJsonObj.WriteToFileXml(FileName, "GAME_MODE", modeGame);
	}

	void HanldeClickEnterBtEvent()
	{
		if (PanelStVal == PanelState.SetPanel || PanelStVal == PanelState.JiaoYanPanel) {
			SelectSetPanelDate ssDtEnum = (SelectSetPanelDate)StarMoveCount;
			switch (ssDtEnum) {
			case SelectSetPanelDate.CoinStart:
				if (XKGlobalData.GameNeedCoin >= 10) {
					XKGlobalData.GameNeedCoin = 0;
				}
				XKGlobalData.GameNeedCoin++;
				SetCoinStartLabelInfo();
				break;

			case SelectSetPanelDate.GameDiff:
				if (GameDiffState >= 3) {
					GameDiffState = 0;
				}
				XKGlobalData.GameDiff = GameDiffState.ToString();
				SetGameDiffState();
				break;

			case SelectSetPanelDate.GameMode:
				IsFreeGameMode = !IsFreeGameMode;
				XKGlobalData.IsFreeMode = IsFreeGameMode;
				SetGameModeState();

                if (XKGlobalData.IsFreeMode == true)
                {
                    SetPrintCaiPiaoUI(false);
                }
				break;

			case SelectSetPanelDate.ResetFactory:
				ResetFactoryInfo();
				break;

			//case SelectSetPanelDate.GameAudioSet:
			//	GameAudioVolume++;
			//	if (GameAudioVolume > 10) {
			//		GameAudioVolume = 0;
			//	}
			//	GameAudioVolumeLB.text = GameAudioVolume.ToString();
			//	HandleJsonObj.WriteToFileXml(FileName, "GameAudioVolume", GameAudioVolume.ToString());
			//	XKGlobalData.GameAudioVolume = GameAudioVolume;
			//	break;
            			
			//case SelectSetPanelDate.GameAudioReset:
			//	GameAudioVolume = 7;
			//	GameAudioVolumeLB.text = GameAudioVolume.ToString();
			//	HandleJsonObj.WriteToFileXml(FileName, "GameAudioVolume", "7");
			//	XKGlobalData.GameAudioVolume = GameAudioVolume;
			//	break;

			case SelectSetPanelDate.Exit:
				ExitSetPanle();
				break;

            case SelectSetPanelDate.CoinToCard:
                {
                    SetCoinToCardIndo(XKGlobalData.GetInstance().m_CoinToCard + 5);
                    break;
                }
            case SelectSetPanelDate.CardMode:
                {
                    SetPrintCaiPiaoUI(!XKGlobalData.GetInstance().IsPrintCaiPiao);
                    break;
                }
            case SelectSetPanelDate.CaiPiaoPrintState:
                {
                    UpdateCaiPiaoPrintState();
                    break;
                }
            case SelectSetPanelDate.CaiPiaoJiP1:
                {
                    StartPrintCaiPiao(PlayerEnum.PlayerOne);
                    break;
                }
            case SelectSetPanelDate.CaiPiaoJiP2:
                {
                    StartPrintCaiPiao(PlayerEnum.PlayerTwo);
                    break;
                }
            case SelectSetPanelDate.CaiPiaoJiP3:
                {
                    StartPrintCaiPiao(PlayerEnum.PlayerThree);
                    break;
                }
            }
		}
	}

    /// <summary>
    /// 总出票数.
    /// </summary>
    public UILabel m_TotalOutPrintCardsLB;
    void UpdateTotalOutPrintCards()
    {
        m_TotalOutPrintCardsLB.text = XKGlobalData.GetInstance().m_TotalOutPrintCards.ToString();
    }

    /// <summary>
    /// 总投币数.
    /// </summary>
    public UILabel m_TotalInsertCoinsLB;
    void UpdateTotalInsertCoins()
    {
        m_TotalInsertCoinsLB.text = XKGlobalData.GetInstance().m_TotalInsertCoins.ToString();
    }

    /// <summary>
    /// 开始打印彩票.
    /// </summary>
    void StartPrintCaiPiao(PlayerEnum indexPlayer)
    {
        pcvr.GetInstance().StartPrintPlayerCaiPiao(indexPlayer, 1);
    }

    /// <summary>
    /// 一币兑换多少张彩票.
    /// </summary>
    public UILabel m_CoinToCardLB;
    void InitCoinToCard()
    {
        m_CoinToCardLB.text = XKGlobalData.GetInstance().m_CoinToCard.ToString();
    }

    void SetCoinToCardIndo(int cardVal)
    {
        int cardTmp = cardVal;
        if (cardTmp > 50)
        {
            cardTmp = 10;
        }
        //else
        //{
        //    cardTmp += 5;
        //}

        m_CoinToCardLB.text = cardTmp.ToString();
        XKGlobalData.GetInstance().SetCoinToCardVal(cardTmp);
    }
    
    public GameObject m_CaiPiaoShi;
    public GameObject m_CaiPiaoFou;
    void InitPrintCaiPiaoUI()
    {
        bool isPrintCaiPiao = XKGlobalData.GetInstance().IsPrintCaiPiao;
        m_CaiPiaoShi.SetActive(isPrintCaiPiao);
        m_CaiPiaoFou.SetActive(!isPrintCaiPiao);
    }

    /// <summary>
    /// 设置是否打印彩票.
    /// </summary>
    void SetPrintCaiPiaoUI(bool isPrintCaiPiao)
    {
        m_CaiPiaoShi.SetActive(isPrintCaiPiao);
        m_CaiPiaoFou.SetActive(!isPrintCaiPiao);

        XKGlobalData.GetInstance().SetIsPrintCaiPiao(isPrintCaiPiao);
    }

    public UILabel[] SteerInfoLB;
	void UpdateDirTestInfo()
	{
		float valFX = 0;
		float valYM = 0;
        for (int i = 0; i < 3; i++)
        {
            valFX = InputEventCtrl.PlayerFX[i];
            valYM = InputEventCtrl.PlayerYM[i];
            if (valYM == 0f && valFX == 0f)
            {
                SteerInfoLB[i].text = "Mid";
                continue;
            }
            if (valYM > 0f)
            {
                SteerInfoLB[i].text = "Up";
                continue;
            }
            if (valYM < 0f)
            {
                SteerInfoLB[i].text = "Down";
                continue;
            }

			if (valFX > 0f) {
				SteerInfoLB[i].text = "Right";
				continue;
			}
			if (valFX < 0f) {
				SteerInfoLB[i].text = "Left";
				continue;
			}
		}
	}

    /// <summary>
    /// 退出设置界面.
    /// </summary>
	void ExitSetPanle()
	{
		BackMovieScene();
	}

	void ResetFactoryInfo()
	{
		ResetPlayerCoinCur();
		XKGlobalData.GameNeedCoin = 1;
		XKGlobalData.GameDiff = "1";
		XKGlobalData.IsFreeMode = false;

		HandleJsonObj.WriteToFileXml(FileName, "START_COIN", XKGlobalData.GameNeedCoin.ToString());
		HandleJsonObj.WriteToFileXml(FileName, "GAME_DIFFICULTY", "1");
		HandleJsonObj.WriteToFileXml(FileName, "GAME_MODE", "1");
		
		//GameAudioVolume = 7;
		//GameAudioVolumeLB.text = GameAudioVolume.ToString();
		//HandleJsonObj.WriteToFileXml(FileName, "GameAudioVolume", "7");
		//XKGlobalData.GameAudioVolume = GameAudioVolume;

		InitCoinStartLabel();
		InitGameDiffDuiGou();
		InitGameModeDuiGou();
        SetPrintCaiPiaoUI(true);
        SetCoinToCardIndo(20);
        XKGlobalData.GetInstance().ResetTotalInsertCoins();
        XKGlobalData.GetInstance().ResetTotalOutPrintCards();
        XKGlobalData.GetInstance().ResetZhanCheCaiChi();
        XKGlobalData.GetInstance().ResetDaoJuCaiChi();
        XKGlobalData.GetInstance().ResetJPBossCaiChi();
        XKGlobalData.GetInstance().ResetYuZhiCaiChi();
        ResetCaiPiaoPrintState();
    }

	void InitStarImgPos()
	{
		MoveStarImg();
	}

	void MoveStarImg()
	{
		if (!StarObj.activeSelf) {
			return;
		}

		Vector3 pos = Vector3.zero;
		switch(PanelStVal)
		{
		case PanelState.SetPanel:
			OnClickMoveBt();
			if (StarMoveCount >= SetPanelStarPos.Length) {
				StarMoveCount = 0;
			}
			pos = SetPanelStarPos[StarMoveCount];
			break;
		}

		if (IsMoveStar) {
			StarTran.localPosition = pos;
			StarMoveCount++;
		}
	}

	void OnClickMoveBt()
    {
        //SelectSetPanelDate ssDt = (SelectSetPanelDate)StarMoveCount;
        //switch (ssDt)
        //{
        //    case SelectSetPanelDate.GameLanguage:
        //        //跳过座椅电机速度设置.
        //        StarMoveCount = (int)SelectSetPanelDate.DianJiSpeedP4;
        //        break;
        //    case SelectSetPanelDate.Exit:
        //        StarMoveCount = (int)SelectSetPanelDate.CheckQiNang16;
        //        break;
        //}
    }

	void ResetPlayerCoinCur()
	{
        if (pcvr.bIsHardWare)
        {
            pcvr.GetInstance().SubPlayerCoin(PlayerEnum.PlayerOne, XKGlobalData.CoinPlayerOne);
            pcvr.GetInstance().SubPlayerCoin(PlayerEnum.PlayerTwo, XKGlobalData.CoinPlayerTwo);
            pcvr.GetInstance().SubPlayerCoin(PlayerEnum.PlayerThree, XKGlobalData.CoinPlayerThree);
            //pcvr.GetInstance().SubPlayerCoin(PlayerEnum.PlayerFour, XKGlobalData.CoinPlayerFour);
        }
        else
        {
            XKGlobalData.CoinPlayerOne = 0;
            XKGlobalData.CoinPlayerTwo = 0;
            XKGlobalData.CoinPlayerThree = 0;
            //XKGlobalData.CoinPlayerFour = 0;
        }
	}

	void BackMovieScene()
    {
        if (Application.loadedLevel != (int)GameLevel.Movie) {
			XkGameCtrl.ResetGameInfo();
			if (!XkGameCtrl.IsGameOnQuit) {
				System.GC.Collect();
				Application.LoadLevel((int)GameLevel.Movie);
			}
		}
	}

	void SetGameTextInfo()
	{
		DuiGouTextCh.SetActive(true);
		DuiGouTextEn.SetActive(false);
	}
    
	public UILabel[] JiQiangBtLB;
	public UILabel[] DaoDanBtLB;
	public UILabel[] StartBtLB;

	/**
	 * key == 0 -> 机枪按键.
	 * key == 1 -> 导弹按键.
	 * key == 2 -> 开始按键.
	 */
	void SetAnJianTestInfo(PlayerEnum indexPlayer, int key, pcvr.ButtonState btState)
	{
		int indexVal = ((int)indexPlayer) - 1;
        if (indexVal < 0 || indexVal > 2)
        {
            return;
        }

        string btStateStr = btState == pcvr.ButtonState.DOWN ? "Down" : "Up";
        switch (key)
        {
            case 0:
                {
                    JiQiangBtLB[indexVal].text = btStateStr;
                    break;
                }
            case 1:
                {
                    DaoDanBtLB[indexVal].text = btStateStr;
                    break;
                }
            case 2:
                {
                    StartBtLB[indexVal].text = btStateStr;
                    break;
                }
        }
	}

	public UILabel[] PlayerCoinLB;
	void UpdatePlayerCoinInfo()
	{
		PlayerCoinLB[0].text = XKGlobalData.CoinPlayerOne.ToString("d2");
		PlayerCoinLB[1].text = XKGlobalData.CoinPlayerTwo.ToString("d2");
		PlayerCoinLB[2].text = XKGlobalData.CoinPlayerThree.ToString("d2");
		//PlayerCoinLB[3].text = XKGlobalData.CoinPlayerFour.ToString("d2");
	}	
	
	void InitGameAudioValue()
	{
		string val = HandleJsonObj.ReadFromFileXml(FileName, "GameAudioVolume");
		if (val == null || val == "") {
			val = "7";
			HandleJsonObj.WriteToFileXml(FileName, "GameAudioVolume", val);
		}
		GameAudioVolume = Convert.ToInt32(val);
		GameAudioVolumeLB.text = GameAudioVolume.ToString();
	}

    /// <summary>
    /// 半票UI.
    /// </summary>
    public GameObject m_CaiPiaoBanPiao;
    /// <summary>
    /// 全票UI.
    /// </summary>
    public GameObject m_CaiPiaoQuanPiao;
    /// <summary>
    /// 初始化彩票打印状态.
    /// </summary>
    void InitCaiPiaoPrintState()
    {
        XKGlobalData.CaiPiaoPrintState type = XKGlobalData.GetInstance().m_CaiPiaoPrintState;
        if (m_CaiPiaoBanPiao != null)
        {
            m_CaiPiaoBanPiao.SetActive(type == XKGlobalData.CaiPiaoPrintState.BanPiao ? true : false);
        }

        if (m_CaiPiaoQuanPiao != null)
        {
            m_CaiPiaoQuanPiao.SetActive(type == XKGlobalData.CaiPiaoPrintState.QuanPiao ? true : false);
        }
    }

    void UpdateCaiPiaoPrintState()
    {
        XKGlobalData.CaiPiaoPrintState type = XKGlobalData.GetInstance().m_CaiPiaoPrintState;
        switch (type)
        {
            case XKGlobalData.CaiPiaoPrintState.BanPiao:
                {
                    type = XKGlobalData.CaiPiaoPrintState.QuanPiao;
                    break;
                }
            case XKGlobalData.CaiPiaoPrintState.QuanPiao:
                {
                    type = XKGlobalData.CaiPiaoPrintState.BanPiao;
                    break;
                }
        }
        XKGlobalData.GetInstance().SetCaiPiaoPrintState(type);

        if (m_CaiPiaoBanPiao != null)
        {
            m_CaiPiaoBanPiao.SetActive(type == XKGlobalData.CaiPiaoPrintState.BanPiao ? true : false);
        }

        if (m_CaiPiaoQuanPiao != null)
        {
            m_CaiPiaoQuanPiao.SetActive(type == XKGlobalData.CaiPiaoPrintState.QuanPiao ? true : false);
        }
    }

    /// <summary>
    /// 重置彩票打印为半票.
    /// </summary>
    void ResetCaiPiaoPrintState()
    {
        XKGlobalData.CaiPiaoPrintState type = XKGlobalData.CaiPiaoPrintState.BanPiao;
        XKGlobalData.GetInstance().SetCaiPiaoPrintState(type);

        if (m_CaiPiaoBanPiao != null)
        {
            m_CaiPiaoBanPiao.SetActive(type == XKGlobalData.CaiPiaoPrintState.BanPiao ? true : false);
        }

        if (m_CaiPiaoQuanPiao != null)
        {
            m_CaiPiaoQuanPiao.SetActive(type == XKGlobalData.CaiPiaoPrintState.QuanPiao ? true : false);
        }
    }
}