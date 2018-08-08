using UnityEngine;
using System.Collections;
using System;

public class SetPanelUiRoot : MonoBehaviour {

	public UILabel CoinStartLabel;
	public GameObject DuiGouDiffLow;
	public GameObject DuiGouDiffMiddle;
	public GameObject DuiGouDiffHigh;
	
	public GameObject DuiGouYunYingMode;
	public GameObject DuiGouFreeMode;

	public GameObject DuiGouTextCh;
	public GameObject DuiGouTextEn;
	public Transform StarTran;
	
	public UILabel GameAudioVolumeLB;
	int GameAudioVolume;
	GameObject DirAdjustObj;
	public UITexture AdjustDir;
	public Texture[] AdjustDirUI;
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

	Vector3 [] SetPanelStarPos = new Vector3[38]{
		new Vector3(-620f, 275f, 0f),
		new Vector3(-620f, 230f, 0f),
		new Vector3(-620f, 180f, 0f),
		new Vector3(-620f, 130f, 0f),
		new Vector3(-335f, 82f, 0f),
		new Vector3(-244f, 82f, 0f),
		new Vector3(-153f, 82f, 0f),
		new Vector3(-60f, 82f, 0f),
		new Vector3(-620f, 35f, 0f),
		new Vector3(-304f, -9f, 0f),
		new Vector3(-248f, -9f, 0f),
		new Vector3(-190f, -9f, 0f),
		new Vector3(-132f, -9f, 0f),
		new Vector3(-188f, -55f, 0f),
		new Vector3(-135f, -55f, 0f),
		new Vector3(-76f, -55f, 0f),
		new Vector3(-18f, -55f, 0f),
		new Vector3(-620f, -103f, 0f),
		new Vector3(-620f, -148f, 0f),
		new Vector3(-336f, -148f, 0f),
		new Vector3(-620f, -192f, 0f),
		new Vector3(-110f, -145f, 0f),
		new Vector3(112f, 250f, 0f),
		new Vector3(280f, 250f, 0f),
		new Vector3(280f, 150f, 0f),
		new Vector3(112f, 150f, 0f),
		new Vector3(435f, 250f, 0f),
		new Vector3(598f, 250f, 0f),
		new Vector3(598f, 150f, 0f),
		new Vector3(435f, 150f, 0f),
		new Vector3(112f, 8f, 0f),
		new Vector3(280f, 8f, 0f),
		new Vector3(280f, -90f, 0f),
		new Vector3(112f, -90f, 0f),
		new Vector3(435f, 8f, 0f),
		new Vector3(598f, 8f, 0f),
		new Vector3(598f, -90f, 0f),
		new Vector3(435f, -90f, 0f)
	};

	enum SelectSetPanelDate
	{
		CoinStart = 1,
		GameMode,
		GameDiff,
		GameLanguage,
		DianJiSpeedP1,
		DianJiSpeedP2,
		DianJiSpeedP3,
		DianJiSpeedP4,
		GameTestBt,
		AdjustDirP1,
		AdjustDirP2,
		AdjustDirP3,
		AdjustDirP4,
		AdjustYouMenShaCheP1, //校准油门刹车.
		AdjustYouMenShaCheP2,
		AdjustYouMenShaCheP3,
		AdjustYouMenShaCheP4,
		ResetFactory,
		GameAudioSet,
		GameAudioReset,
		GameVersion,
		Exit,
		CheckQiNang1,
		CheckQiNang2,
		CheckQiNang3,
		CheckQiNang4,
		CheckQiNang5,
		CheckQiNang6,
		CheckQiNang7,
		CheckQiNang8,
		CheckQiNang9,
		CheckQiNang10,
		CheckQiNang11,
		CheckQiNang12,
		CheckQiNang13,
		CheckQiNang14,
		CheckQiNang15,
		CheckQiNang16,
	}

	string startCoinInfo = "";

	enum AdjustDirState
	{
		DirectionRight = 0,
		DirectionCenter,
		DirectionLeft
	}
	AdjustDirState AdjustDirSt = AdjustDirState.DirectionRight;

//	AdjustGunDrossState AdjustGunDrossSt = AdjustGunDrossState.GunCrossLU;
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
		DirAdjustObj = AdjustDir.gameObject;
		AdjustYouMenShaCheObj = AdjustYouMenShaCheUI.gameObject;
		QiNangCQObj.SetActive(false);
		XkGameCtrl.SetActivePlayerOne(false);
		XkGameCtrl.SetActivePlayerTwo(false);
		XkGameCtrl.SetActivePlayerThree(false);
		XkGameCtrl.SetActivePlayerFour(false);
		//pcvr.CloseAllQiNangArray(PlayerEnum.Null, 1);
		//pcvr.OpenCheckYouMenValInfo();
		//pcvr.GetInstance().CloseFangXiangPanPower();
		SetGameTextInfo();

		SetStarObjActive(true);
		SetAnJianTestPanel(1);

		InitHandleJson();
		InitStarImgPos();
		InitCoinStartLabel();
		InitGameDiffDuiGou();
		InitGameModeDuiGou();
		InitGameDianJiSpeed();
		InitGameAudioValue();
		InitGameVersionPlayer();

		InputEventCtrl.GetInstance().ClickSetEnterBtEvent += ClickSetEnterBtEvent;
		InputEventCtrl.GetInstance().ClickSetMoveBtEvent += ClickSetMoveBtEvent;
		InputEventCtrl.GetInstance().ClickFireBtOneEvent += ClickFireBtOneEvent;
		InputEventCtrl.GetInstance().ClickFireBtTwoEvent += ClickFireBtTwoEvent;
		InputEventCtrl.GetInstance().ClickFireBtThreeEvent += ClickFireBtThreeEvent;
		InputEventCtrl.GetInstance().ClickFireBtFourEvent += ClickFireBtFourEvent;
		InputEventCtrl.GetInstance().ClickDaoDanBtOneEvent += ClickDaoDanBtOneEvent;
		InputEventCtrl.GetInstance().ClickDaoDanBtTwoEvent += ClickDaoDanBtTwoEvent;
		InputEventCtrl.GetInstance().ClickDaoDanBtThreeEvent += ClickDaoDanBtThreeEvent;
		InputEventCtrl.GetInstance().ClickDaoDanBtFourEvent += ClickDaoDanBtFourEvent;
		InputEventCtrl.GetInstance().ClickStartBtOneEvent += ClickStartBtEventP1;
		InputEventCtrl.GetInstance().ClickStartBtTwoEvent += ClickStartBtEventP2;
		InputEventCtrl.GetInstance().ClickStartBtThreeEvent += ClickStartBtEventP3;
		InputEventCtrl.GetInstance().ClickStartBtFourEvent += ClickStartBtEventP4;
		InputEventCtrl.GetInstance().ClickStopDongGanBtOneEvent += ClickStopDongGanBtEventP1;
		InputEventCtrl.GetInstance().ClickStopDongGanBtTwoEvent += ClickStopDongGanBtEventP2;
		InputEventCtrl.GetInstance().ClickStopDongGanBtThreeEvent += ClickStopDongGanBtEventP3;
		InputEventCtrl.GetInstance().ClickStopDongGanBtFourEvent += ClickStopDongGanBtEventP4;
	}

	void Update()
	{
		if (SetBtSt == ButtonState.DOWN && Time.time - TimeSetMoveBt > 1f && Time.frameCount % 200 == 0) {
			MoveStarImg();
		}

		UpdateDirTestInfo();
		UpdateYouMenTestInfo();
		UpdateShaCheTestInfo();
	}

	void ClickSetEnterBtEvent(ButtonState val)
	{
		if(val == ButtonState.DOWN){
			return;
		}
		//BackMovieScene(); //test.
		HanldeClickEnterBtEvent();
	}

	float TimeSetMoveBt;
	ButtonState SetBtSt = ButtonState.UP;
	void ClickSetMoveBtEvent(ButtonState val)
	{
		SetBtSt = val;
		if (val == ButtonState.DOWN) {
			TimeSetMoveBt = Time.time;
			return;
		}

		if (Time.time - TimeSetMoveBt > 1f) {
			return;
		}
		MoveStarImg();
	}

	void ClickFireBtOneEvent(ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerOne, 0, val);
		if (val == ButtonState.DOWN) {
			return;
		}

		HandleClickStartBtEvent();
	}

	void ClickFireBtTwoEvent(ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerTwo, 0, val);
		if (val == ButtonState.DOWN) {
			return;
		}

		HandleClickStartBtEvent();
	}

	void ClickFireBtThreeEvent(ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerThree, 0, val);
		if (val == ButtonState.DOWN) {
			return;
		}
		
		HandleClickStartBtEvent();
	}

	void ClickFireBtFourEvent(ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerFour, 0, val);
		if (val == ButtonState.DOWN) {
			return;
		}
		
		HandleClickStartBtEvent();
	}

	void ClickDaoDanBtOneEvent(ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerOne, 1, val);
	}
	
	void ClickDaoDanBtTwoEvent(ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerTwo, 1, val);
	}
	
	void ClickDaoDanBtThreeEvent(ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerThree, 1, val);
	}
	
	void ClickDaoDanBtFourEvent(ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerFour, 1, val);
	}

	void ClickStartBtEventP1(ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerOne, 2, val);
		if(val == ButtonState.DOWN)
		{
			return;
		}
		HandleClickStartBtEvent();
	}

	void ClickStartBtEventP2(ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerTwo, 2, val);
		if(val == ButtonState.DOWN)
		{
			return;
		}
		HandleClickStartBtEvent();
	}

	void ClickStartBtEventP3(ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerThree, 2, val);
		if(val == ButtonState.DOWN)
		{
			return;
		}
		HandleClickStartBtEvent();
	}

	void ClickStartBtEventP4(ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerFour, 2, val);
		if(val == ButtonState.DOWN)
		{
			return;
		}
		HandleClickStartBtEvent();
	}

	void ClickStopDongGanBtEventP1(ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerOne, 3, val);
	}

	void ClickStopDongGanBtEventP2(ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerTwo, 3, val);
	}

	void ClickStopDongGanBtEventP3(ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerThree, 3, val);
	}

	void ClickStopDongGanBtEventP4(ButtonState val)
	{
		SetAnJianTestInfo(PlayerEnum.PlayerFour, 3, val);
	}

	float TimeStartVal;
	void HandleClickStartBtEvent()
	{
		if (Time.realtimeSinceStartup - TimeStartVal < 0.5f) {
			return;
		}
		TimeStartVal = Time.realtimeSinceStartup;

		if (PanelStVal != PanelState.JiaoYanPanel) {
			return;
		}
		
		SelectSetPanelDate ssDtEnum = (SelectSetPanelDate) StarMoveCount;
		switch (ssDtEnum) {
		case SelectSetPanelDate.AdjustDirP1:
		case SelectSetPanelDate.AdjustDirP2:
		case SelectSetPanelDate.AdjustDirP3:
		case SelectSetPanelDate.AdjustDirP4:
		case SelectSetPanelDate.AdjustYouMenShaCheP1:
		case SelectSetPanelDate.AdjustYouMenShaCheP2:
		case SelectSetPanelDate.AdjustYouMenShaCheP3:
		case SelectSetPanelDate.AdjustYouMenShaCheP4:
			OpenJiaoYanPanelObj();
			break;
		}
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
				break;

			case SelectSetPanelDate.DianJiSpeedP1:
			case SelectSetPanelDate.DianJiSpeedP2:
			case SelectSetPanelDate.DianJiSpeedP3:
			case SelectSetPanelDate.DianJiSpeedP4:
				SetGameDianJiSpeed();
				break;

			case SelectSetPanelDate.GameTestBt:
				SetAnJianTestPanel();
				break;

			case SelectSetPanelDate.AdjustDirP1:
			case SelectSetPanelDate.AdjustDirP2:
			case SelectSetPanelDate.AdjustDirP3:
			case SelectSetPanelDate.AdjustDirP4:
			case SelectSetPanelDate.AdjustYouMenShaCheP1:
			case SelectSetPanelDate.AdjustYouMenShaCheP2:
			case SelectSetPanelDate.AdjustYouMenShaCheP3:
			case SelectSetPanelDate.AdjustYouMenShaCheP4:
				OpenJiaoYanPanelObj();
				break;

			case SelectSetPanelDate.CheckQiNang1:
			case SelectSetPanelDate.CheckQiNang2:
			case SelectSetPanelDate.CheckQiNang3:
			case SelectSetPanelDate.CheckQiNang4:
			case SelectSetPanelDate.CheckQiNang5:
			case SelectSetPanelDate.CheckQiNang6:
			case SelectSetPanelDate.CheckQiNang7:
			case SelectSetPanelDate.CheckQiNang8:
			case SelectSetPanelDate.CheckQiNang9:
			case SelectSetPanelDate.CheckQiNang10:
			case SelectSetPanelDate.CheckQiNang11:
			case SelectSetPanelDate.CheckQiNang12:
			case SelectSetPanelDate.CheckQiNang13:
			case SelectSetPanelDate.CheckQiNang14:
			case SelectSetPanelDate.CheckQiNang15:
			case SelectSetPanelDate.CheckQiNang16:
				OnClickCheckQiNang();
				break;

			case SelectSetPanelDate.ResetFactory:
				ResetFactoryInfo();
				break;

			case SelectSetPanelDate.GameAudioSet:
				GameAudioVolume++;
				if (GameAudioVolume > 10) {
					GameAudioVolume = 0;
				}
				GameAudioVolumeLB.text = GameAudioVolume.ToString();
				HandleJsonObj.WriteToFileXml(FileName, "GameAudioVolume", GameAudioVolume.ToString());
				XKGlobalData.GameAudioVolume = GameAudioVolume;
				break;
				
			case SelectSetPanelDate.GameAudioReset:
				GameAudioVolume = 7;
				GameAudioVolumeLB.text = GameAudioVolume.ToString();
				HandleJsonObj.WriteToFileXml(FileName, "GameAudioVolume", "7");
				XKGlobalData.GameAudioVolume = GameAudioVolume;
				break;

			case SelectSetPanelDate.GameVersion:
				ChangeGameVersionInfo();
				break;

			case SelectSetPanelDate.Exit:
				ExitSetPanle();
				break;
			}
		}
	}

	void InitAdjustDir()
	{
		PanelStVal = PanelState.JiaoYanPanel;
		DirAdjustObj.SetActive(true);
		AdjustDirSt = AdjustDirState.DirectionRight;
		ChangeAdjustDirImg();
	}

	void ChangeAdjustDirImg()
	{
		int index = (int)AdjustDirSt;
		AdjustDir.mainTexture = AdjustDirUI[index];
	}

	public UITexture AdjustYouMenShaCheUI;
	public Texture[] YouMenShaCheUI;
	GameObject AdjustYouMenShaCheObj;
	enum AdjustYouMenShaCheState
	{
		YouMen,
		ShaChe,
	}
	AdjustYouMenShaCheState AdjustYMSCSt = AdjustYouMenShaCheState.YouMen;
	void InitAdjustYouMenShaChe()
	{
		PanelStVal = PanelState.JiaoYanPanel;
		AdjustYouMenShaCheObj.SetActive(true);
		AdjustYMSCSt = AdjustYouMenShaCheState.YouMen;
		ChangeAdjustYouMenShaCheImg();
	}
	
	void ChangeAdjustYouMenShaCheImg()
	{
		int index = (int)AdjustYMSCSt;
		AdjustYouMenShaCheUI.mainTexture = YouMenShaCheUI[index];
	}

	void InitAdjustGunCross()
	{
//		AdjustGunDrossSt = AdjustGunDrossState.GunCrossLU;
		ChangeAdjustGunCrossImg();
	}

	void ChangeAdjustGunCrossImg()
	{
//		int index = (int)AdjustGunDrossSt;
//		SpriteAdjustGunCross.spriteName = "GunJY_" + index.ToString();
	}

	void CloseAllJiaoYanPanel()
	{
		PlayerEnum indexPlayer = PlayerEnum.Null;
		if (DirAdjustObj.activeSelf) {
			indexPlayer = (PlayerEnum)(1 + StarMoveCount - (int)SelectSetPanelDate.AdjustDirP1);
			switch (AdjustDirSt) {
			case AdjustDirState.DirectionRight:
				AdjustDirSt = AdjustDirState.DirectionCenter;
				ChangeAdjustDirImg();
				if (pcvr.bIsHardWare) {
					//pcvr.SaveSteerVal(PcvrValState.ValMax, indexPlayer);
				}
				return;

			case AdjustDirState.DirectionCenter:
				AdjustDirSt = AdjustDirState.DirectionLeft;
				ChangeAdjustDirImg();
				if (pcvr.bIsHardWare) {
					//pcvr.SaveSteerVal(PcvrValState.ValCenter, indexPlayer);
				}
				return;

			case AdjustDirState.DirectionLeft:
				if (pcvr.bIsHardWare) {
					//pcvr.SaveSteerVal(PcvrValState.ValMin, indexPlayer);
				}
				break;
			}
		}

		if (AdjustYouMenShaCheObj.activeSelf) {
			indexPlayer = (PlayerEnum)(1 + StarMoveCount - (int)SelectSetPanelDate.AdjustYouMenShaCheP1);
			switch (AdjustYMSCSt) {
			case AdjustYouMenShaCheState.YouMen:
				AdjustYMSCSt = AdjustYouMenShaCheState.ShaChe;
				ChangeAdjustYouMenShaCheImg();
				if (pcvr.bIsHardWare) {
					//记录油门数据.
					//pcvr.SaveYouMenVal(indexPlayer);
				}
				//return; //校准刹车.
				break; //不用校准刹车.
			case AdjustYouMenShaCheState.ShaChe:
				if (pcvr.bIsHardWare) {
					//记录刹车数据.
				}
				break;
			}
		}
		PanelStVal = PanelState.SetPanel;
		DirAdjustObj.SetActive(false);
		AdjustYouMenShaCheObj.SetActive(false);
		Screen.showCursor = false;
		
		IsMoveStar = true;
		StarObj.SetActive(true);
	}

	void OpenJiaoYanPanelObj()
	{
		if (DirAdjustObj.activeSelf || AdjustYouMenShaCheObj.activeSelf) {
			CloseAllJiaoYanPanel();
			return;
		}

		IsMoveStar = false;
		StarObj.SetActive(false);
		SelectSetPanelDate ssDtEnum = (SelectSetPanelDate)StarMoveCount;
		switch (ssDtEnum) {
		case SelectSetPanelDate.AdjustDirP1:
		case SelectSetPanelDate.AdjustDirP2:
		case SelectSetPanelDate.AdjustDirP3:
		case SelectSetPanelDate.AdjustDirP4:
			InitAdjustDir();
			break;
		case SelectSetPanelDate.AdjustYouMenShaCheP1:
		case SelectSetPanelDate.AdjustYouMenShaCheP2:
		case SelectSetPanelDate.AdjustYouMenShaCheP3:
		case SelectSetPanelDate.AdjustYouMenShaCheP4:
			InitAdjustYouMenShaChe();
			break;
		}
	}

	public void SetHitAimObjInfoActive(bool isActive)
	{
//		if (isActive == HitAimObjInfo.activeSelf) {
//			return;
//		}
//		HitAimObjInfo.SetActive(isActive);
	}

	public UILabel[] SteerInfoLB;
	void UpdateDirTestInfo()
	{
		float valFX = 0;
		for (int i = 0; i < 4; i++) {
			valFX = InputEventCtrl.PlayerFX[i];
			if (valFX == 0f) {
				SteerInfoLB[i].text = "Mid";
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
	
	public UILabel[] YouMenInfoLB;
	void UpdateYouMenTestInfo()
	{
		float valYM = 0;
		for (int i = 0; i < 4; i++) {
			valYM = InputEventCtrl.PlayerYM[i];
			YouMenInfoLB[i].text = valYM > 0f ? "Open" : "Close";
		}
	}

	public UILabel[] ShaCheInfoLB;
	void UpdateShaCheTestInfo()
	{
		float valYM = 0;
		for (int i = 0; i < 4; i++) {
			if (InputEventCtrl.PlayerSC[i] < 0f || InputEventCtrl.PlayerYM[i] < 0f) {
				valYM = -1f;
			}
			else {
				valYM = 0f;
			}
			ShaCheInfoLB[i].text = valYM < 0f ? "Open" : "Close";
		}
	}

	void ExitSetPanle()
	{
		BackMovieScene();
	}

	void ResetFactoryInfo()
	{
		ResetGameVersionPlayer();
		ResetPlayerCoinCur();
		XKGlobalData.GameNeedCoin = 1;
		XKGlobalData.GameDiff = "1";
		XKGlobalData.IsFreeMode = false;

		HandleJsonObj.WriteToFileXml(FileName, "START_COIN", XKGlobalData.GameNeedCoin.ToString());
		HandleJsonObj.WriteToFileXml(FileName, "GAME_DIFFICULTY", "1");
		HandleJsonObj.WriteToFileXml(FileName, "GAME_MODE", "1");
		
		GameAudioVolume = 7;
		GameAudioVolumeLB.text = GameAudioVolume.ToString();
		HandleJsonObj.WriteToFileXml(FileName, "GameAudioVolume", "7");
		XKGlobalData.GameAudioVolume = GameAudioVolume;

		InitCoinStartLabel();
		InitGameDiffDuiGou();
		InitGameModeDuiGou();
		ResetGameDianJiSpeed();
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
		SelectSetPanelDate ssDt = (SelectSetPanelDate)StarMoveCount;
		switch (ssDt) {
		case SelectSetPanelDate.AdjustDirP2:
			if (XKGlobalData.GameVersionPlayer != 0) {
				//双人版跳过3p和4p的校准.
				StarMoveCount = (int)SelectSetPanelDate.AdjustDirP4;
			}
			break;
		case SelectSetPanelDate.AdjustYouMenShaCheP2:
			if (XKGlobalData.GameVersionPlayer != 0) {
				//双人版跳过3p和4p的校准.
				StarMoveCount = (int)SelectSetPanelDate.AdjustYouMenShaCheP4;
			}
			break;
		case SelectSetPanelDate.CheckQiNang1:
		case SelectSetPanelDate.CheckQiNang2:
		case SelectSetPanelDate.CheckQiNang3:
		case SelectSetPanelDate.CheckQiNang4:
		case SelectSetPanelDate.CheckQiNang5:
		case SelectSetPanelDate.CheckQiNang6:
		case SelectSetPanelDate.CheckQiNang7:
		case SelectSetPanelDate.CheckQiNang8:
		case SelectSetPanelDate.CheckQiNang9:
		case SelectSetPanelDate.CheckQiNang10:
		case SelectSetPanelDate.CheckQiNang11:
		case SelectSetPanelDate.CheckQiNang12:
		case SelectSetPanelDate.CheckQiNang13:
		case SelectSetPanelDate.CheckQiNang14:
		case SelectSetPanelDate.CheckQiNang15:
		case SelectSetPanelDate.CheckQiNang16:
			if (ssDt == SelectSetPanelDate.CheckQiNang8) {
				if (XKGlobalData.GameVersionPlayer != 0) {
					//双人版跳过3p和4p的校准.
					StarMoveCount = (int)SelectSetPanelDate.CheckQiNang16;
				}
			}
			QiNangCQObj.SetActive(false);
			//pcvr.CloseAllQiNangArray(PlayerEnum.Null, 1);
			break;
		case SelectSetPanelDate.GameTestBt:
			SetAnJianTestPanel(1); //关闭按键测试界面.
			if (pcvr.TKMoveSt == TKMoveState.YaoGanBan) {
				//摇杆版跳过方向,油门,刹车的校准逻辑.
				StarMoveCount = (int)SelectSetPanelDate.AdjustYouMenShaCheP4;
			}
			break;
		case SelectSetPanelDate.GameLanguage:
			//跳过座椅电机速度设置.
			StarMoveCount = (int)SelectSetPanelDate.DianJiSpeedP4;
			/*if (pcvr.TKMoveSt == TKMoveState.YaoGanBan) {
				//摇杆版跳过座椅电机速度设置.
				StarMoveCount = (int)SelectSetPanelDate.DianJiSpeedP4;
			}*/
			break;
		case SelectSetPanelDate.Exit:
			if (pcvr.TKMoveSt == TKMoveState.YaoGanBan) {
				StarMoveCount = (int)SelectSetPanelDate.CheckQiNang16;
			}
			break;
		}
	}

	void ResetPlayerCoinCur()
	{
		XKGlobalData.CoinPlayerOne = 0;
		XKGlobalData.CoinPlayerTwo = 0;
		XKGlobalData.CoinPlayerThree = 0;
		XKGlobalData.CoinPlayerFour = 0;
		if (pcvr.bIsHardWare) {
			pcvr.GetInstance().CoinNumCurrentP1 = 0;
			pcvr.GetInstance().CoinNumCurrentP2 = 0;
			pcvr.GetInstance().CoinNumCurrentP3 = 0;
			pcvr.GetInstance().CoinNumCurrentP4 = 0;
		}
	}

	void BackMovieScene()
    {
        pcvr.GetInstance().ClearGameWeiXinData();
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

	public UILabel[] DianJiSpeedLB;
	int[] DianJiSpeedVal = new int[4]{5, 5, 5, 5};
	void InitGameDianJiSpeed()
	{
		string speedStr = HandleJsonObj.ReadFromFileXml(FileName, "DianJiSpeedP1");
		if(speedStr == null || speedStr == "") {
			speedStr = "5";
			HandleJsonObj.WriteToFileXml(FileName, "DianJiSpeedP1", speedStr);
		}
		DianJiSpeedVal[0] = Convert.ToInt32(speedStr);

		speedStr = HandleJsonObj.ReadFromFileXml(FileName, "DianJiSpeedP2");
		if(speedStr == null || speedStr == "") {
			speedStr = "5";
			HandleJsonObj.WriteToFileXml(FileName, "DianJiSpeedP2", speedStr);
		}
		DianJiSpeedVal[1] = Convert.ToInt32(speedStr);

		speedStr = HandleJsonObj.ReadFromFileXml(FileName, "DianJiSpeedP3");
		if(speedStr == null || speedStr == "") {
			speedStr = "5";
			HandleJsonObj.WriteToFileXml(FileName, "DianJiSpeedP3", speedStr);
		}
		DianJiSpeedVal[2] = Convert.ToInt32(speedStr);

		speedStr = HandleJsonObj.ReadFromFileXml(FileName, "DianJiSpeedP4");
		if(speedStr == null || speedStr == "") {
			speedStr = "5";
			HandleJsonObj.WriteToFileXml(FileName, "DianJiSpeedP4", speedStr);
		}
		DianJiSpeedVal[3] = Convert.ToInt32(speedStr);

		SetGameDianJiSpeed(1);
	}

	void ResetGameDianJiSpeed()
	{
		for (int i = 0; i < DianJiSpeedVal.Length; i++) {
			DianJiSpeedVal[i] = 5;
		}
		SetGameDianJiSpeed(2);
	}

	/**
	 * key == 0 -> 电机转动速度递增.
	 * key == 1 -> 电机转动速度不递增.
	 * key == 2 -> 电机转动速度重置.
	 */
	void SetGameDianJiSpeed(int key = 0)
	{
		int indexVal = 0;
		SelectSetPanelDate ssDtEnum = (SelectSetPanelDate)StarMoveCount;
		switch (ssDtEnum) {
		case SelectSetPanelDate.DianJiSpeedP1:
			indexVal = 0;
			break;
		case SelectSetPanelDate.DianJiSpeedP2:
			indexVal = 1;
			break;
		case SelectSetPanelDate.DianJiSpeedP3:
			indexVal = 2;
			break;
		case SelectSetPanelDate.DianJiSpeedP4:
			indexVal = 3;
			break;
		}

		if (key == 0) {
			DianJiSpeedVal[indexVal]++;
		}
		DianJiSpeedVal[indexVal] = DianJiSpeedVal[indexVal] >= 16 ? 1 : DianJiSpeedVal[indexVal];
		//pcvr.DianJiSpeedP1 = DianJiSpeedVal[0];
		//pcvr.DianJiSpeedP2 = DianJiSpeedVal[1];
		//pcvr.DianJiSpeedP3 = DianJiSpeedVal[2];
		//pcvr.DianJiSpeedP4 = DianJiSpeedVal[3];

		for (int i = 0; i < 4; i++) {
			DianJiSpeedLB[i].text = DianJiSpeedVal[i].ToString("d2");
		}

		if (key == 0 || key == 2) {
			HandleJsonObj.WriteToFileXml(FileName, "DianJiSpeedP1", DianJiSpeedVal[0].ToString());
			HandleJsonObj.WriteToFileXml(FileName, "DianJiSpeedP2", DianJiSpeedVal[1].ToString());
			HandleJsonObj.WriteToFileXml(FileName, "DianJiSpeedP3", DianJiSpeedVal[2].ToString());
			HandleJsonObj.WriteToFileXml(FileName, "DianJiSpeedP4", DianJiSpeedVal[3].ToString());
		}
	}

	public GameObject AnJianTestPanel;
	public GameObject JiQiangBtObj;
	public UILabel[] JiQiangBtLB;
	public GameObject DaoDanBtObj;
	public UILabel[] DaoDanBtLB;
	public GameObject StartBtObj;
	public UILabel[] StartBtLB;
	public GameObject JinJiBtObj;
	void SetAnJianTestPanel(int key = 0)
	{
		if (key == 0) {
			if (!AnJianTestPanel.activeSelf) {
				JiQiangBtObj.SetActive(false);
				DaoDanBtObj.SetActive(false);
				StartBtObj.SetActive(false);
				JinJiBtObj.SetActive(false);
			}
			AnJianTestPanel.SetActive(!AnJianTestPanel.activeSelf);
		}
		else {
			AnJianTestPanel.SetActive(false);
		}
	}

	/**
	 * key == 0 -> 机枪按键.
	 * key == 1 -> 导弹按键.
	 * key == 2 -> 开始按键.
	 * key == 3 -> 紧急按键.
	 */
	void SetAnJianTestInfo(PlayerEnum indexPlayer, int key, ButtonState btState)
	{
		int indexVal = (int)indexPlayer;
		bool isActive = btState == ButtonState.DOWN ? true : false;
		for (int i = 0; i < 2; i++) {
			JiQiangBtLB[i].text = indexVal.ToString();
			DaoDanBtLB[i].text = indexVal.ToString();
			StartBtLB[i].text = indexVal.ToString();
		}

		switch (key) {
		case 0:
			JiQiangBtObj.SetActive(isActive);
			break;
		case 1:
			DaoDanBtObj.SetActive(isActive);
			break;
		case 2:
			StartBtObj.SetActive(isActive);
			break;
		case 3:
			JinJiBtObj.SetActive(isActive);
			break;
		}
	}

	public UILabel[] PlayerCoinLB;
	void UpdatePlayerCoinInfo()
	{
		PlayerCoinLB[0].text = XKGlobalData.CoinPlayerOne.ToString("d2");
		PlayerCoinLB[1].text = XKGlobalData.CoinPlayerTwo.ToString("d2");
		PlayerCoinLB[2].text = XKGlobalData.CoinPlayerThree.ToString("d2");
		PlayerCoinLB[3].text = XKGlobalData.CoinPlayerFour.ToString("d2");
	}
	
	public GameObject QiNangCQObj;
	void OnClickCheckQiNang()
	{
		int indexVal = StarMoveCount - (int)SelectSetPanelDate.CheckQiNang1;
		//Debug.Log("Unity:"+"*** "+pcvr.QiNangArray[indexVal]+", DongGanState "+pcvr.DongGanState);
		//pcvr.QiNangArray[indexVal] = (byte)(pcvr.QiNangArray[indexVal] != 1 ? 1 : 0);
		//QiNangCQObj.SetActive(pcvr.QiNangArray[indexVal] == 0 ? false : true);
		//Debug.Log("Unity:"+"--- "+pcvr.QiNangArray[indexVal]);
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

	public UITexture GameVersionTexture;
	/**
	 * TextureGV[0] -> 四人版游戏.
	 * TextureGV[1] -> 双人版游戏.
	 */
	public Texture[] TextureGV;
	int GameVersionPlayer = 0;
	void InitGameVersionPlayer()
	{
		string val = HandleJsonObj.ReadFromFileXml(FileName, "GameVersionPlayer");
		if (val == null || val == "") {
			val = "0"; //四人版本.
			HandleJsonObj.WriteToFileXml(FileName, "GameVersionPlayer", val);
		}
		GameVersionPlayer = Convert.ToInt32(val);
		GameVersionTexture.mainTexture = TextureGV[GameVersionPlayer];
	}

	void ChangeGameVersionInfo()
	{
		GameVersionPlayer = (GameVersionPlayer == 0 ? 1 : 0);
		string val = GameVersionPlayer.ToString();
		HandleJsonObj.WriteToFileXml(FileName, "GameVersionPlayer", val);
		GameVersionTexture.mainTexture = TextureGV[GameVersionPlayer];
		XKGlobalData.GameVersionPlayer = GameVersionPlayer;
	}

	void ResetGameVersionPlayer()
	{
		GameVersionPlayer = 0;
		string val = GameVersionPlayer.ToString();
		HandleJsonObj.WriteToFileXml(FileName, "GameVersionPlayer", val);
		GameVersionTexture.mainTexture = TextureGV[GameVersionPlayer];
		XKGlobalData.GameVersionPlayer = GameVersionPlayer;
	}
}