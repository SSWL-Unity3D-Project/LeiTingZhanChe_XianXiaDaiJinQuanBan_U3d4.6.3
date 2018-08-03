using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class XKGlobalData
{
	public static int CoinPlayerOne = 0;
	public static int CoinPlayerTwo = 0;
	public static int CoinPlayerThree = 0;
	public static int CoinPlayerFour = 0;
	public static int GameNeedCoin;
    /// <summary>
    /// 1币兑换多少张彩票.
    /// </summary>
    public int m_CoinToCaiPiao = 20;
	/**
	 * GameVersionPlayer == 0 -> 四人版本游戏.
	 * GameVersionPlayer == 1 -> 双人版本游戏.
	 */
	public static int GameVersionPlayer = 0;
	public static bool IsFreeMode;
	public static string GameDiff;
	public static int GameAudioVolume = 10;
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

			string speedStr = HandleJsonObj.ReadFromFileXml(FileName, "DianJiSpeedP1");
			if(speedStr == null || speedStr == "") {
				speedStr = "5";
				HandleJsonObj.WriteToFileXml(FileName, "DianJiSpeedP1", speedStr);
			}
			pcvr.DianJiSpeedP1 = Convert.ToInt32(speedStr);
			
			speedStr = HandleJsonObj.ReadFromFileXml(FileName, "DianJiSpeedP2");
			if(speedStr == null || speedStr == "") {
				speedStr = "5";
				HandleJsonObj.WriteToFileXml(FileName, "DianJiSpeedP2", speedStr);
			}
			pcvr.DianJiSpeedP2 = Convert.ToInt32(speedStr);
			
			speedStr = HandleJsonObj.ReadFromFileXml(FileName, "DianJiSpeedP3");
			if(speedStr == null || speedStr == "") {
				speedStr = "5";
				HandleJsonObj.WriteToFileXml(FileName, "DianJiSpeedP3", speedStr);
			}
			pcvr.DianJiSpeedP3 = Convert.ToInt32(speedStr);
			
			speedStr = HandleJsonObj.ReadFromFileXml(FileName, "DianJiSpeedP4");
			if(speedStr == null || speedStr == "") {
				speedStr = "5";
				HandleJsonObj.WriteToFileXml(FileName, "DianJiSpeedP4", speedStr);
			}
			pcvr.DianJiSpeedP4 = Convert.ToInt32(speedStr);
			
			string val = HandleJsonObj.ReadFromFileXml(FileName, "GameAudioVolume");
			if (val == null || val == "") {
				val = "7";
				HandleJsonObj.WriteToFileXml(FileName, "GameAudioVolume", val);
			}
			GameAudioVolume = Convert.ToInt32(val);

			val = HandleJsonObj.ReadFromFileXml(FileName, "GameVersionPlayer");
			if (val == null || val == "") {
				val = "0"; //四人版本.
				HandleJsonObj.WriteToFileXml(FileName, "GameVersionPlayer", val);
			}
			GameVersionPlayer = Convert.ToInt32(val);
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

    public static void SetCoinPlayerOne(int coin)
	{
		if (XKGlobalData.GameVersionPlayer != 0) {
			CoinPlayerOne = coin;
			SetCoinPlayerThree(coin);
			return;
		}

		if (coin > 0 && CoinPlayerOne != coin) {
			PlayTouBiAudio();
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
			audioVal.volume = AudioListCtrl.GetInstance().ASGuanKaBJ[audioIndex].volume;

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