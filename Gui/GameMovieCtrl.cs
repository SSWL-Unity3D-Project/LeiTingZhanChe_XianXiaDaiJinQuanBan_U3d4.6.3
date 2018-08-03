//#define TEST_MOVIE
using UnityEngine;

public class GameMovieCtrl : SSGameMono
{
    /// <summary>
    /// 二维码对象.
    /// </summary>
    public GameObject m_ErWeiMaObj;
    public Transform m_UITrParent;
    /// <summary>
    /// 循环动画logo的动画.
    /// </summary>
    public GameObject m_MovieAniPrefab;
#if UNITY_ANDROID
    public string m_MoviePath = "Movie/cartoonNew.mov";
#endif
    /// <summary>
    /// Websocket预制.
    /// </summary>
    public GameObject m_WebSocketBoxPrefab;
//#if UNITY_STANDALONE_WIN
//	public MovieTexture Movie;
//#endif
	public static bool IsTestLJGame; //测试联机小窗口游戏.
	AudioSource AudioSourceObj;
	bool IsStopMovie;
	public static bool IsActivePlayer;
	float TimeVal;
	public static bool IsTestXiaoScreen;
	public static bool IsOpenFXZhenDong = true;
	enum QualityLevelEnum
	{
		Fastest,
		Fast,
		Simple,
		Good,
		Beautiful,
		Fantastic
	}
	static GameMovieCtrl _instance;
	public static GameMovieCtrl GetInstance()
	{
		return _instance;
	}
	
	// Use this for initialization
	void Awake()
	{
		_instance = this;
		try
		{
            XKGlobalData.GetInstance();
			//Debug.Log("Unity:!!!!!!GetInstance!!!!!!");
			AudioListener.volume = (float)XKGlobalData.GameAudioVolume / 10f;
			if (AudioListCtrl.GetInstance() != null)
			{
				AudioListCtrl.GetInstance().CloseGameAudioBJ();
			}
			else
			{
				Debug.Log("Unity:!!!!!!AudioListCtrl.GetInstance() == null!!!!!!");
			}
			Screen.showCursor = false;
			LoadingGameCtrl.ResetLoadingInfo();
			//Debug.Log("Unity:!!!!!!ResetLoadingInfo!!!!!!");
			Time.timeScale = 1.0f;
			AudioManager.Instance.SetParentTran(null);
			//Debug.Log("Unity:!!!!!!Instance.SetParentTran!!!!!!");
			GameOverCtrl.IsShowGameOver = false;
			//Debug.Log("Unity:!!!!!!IsOpenFXZhenDong!!!!!!"+ IsOpenFXZhenDong.ToString());
			if (IsOpenFXZhenDong)
			{
				pcvr.OpenAllPlayerFangXiangPanPower();
			}
			pcvr.CloseAllQiNangArray(PlayerEnum.Null, 1);
			//Debug.Log("Unity:!!!!!!CloseAllQiNangArray!!!!!!");
			//IsTestLJGame = true; //test
			//IsTestXiaoScreen = true; //test
			if (!XkGameCtrl.IsGameOnQuit)
			{
				if (Screen.fullScreen
					|| Screen.currentResolution.width != 1280
					|| Screen.currentResolution.height != 720)
				{
					if (!IsTestLJGame && !IsTestXiaoScreen)
					{
						Screen.SetResolution(1280, 720, false);
					}
				}
			}
			Debug.Log("Unity:!!!!!!IsGameOnQuit!!!!!!");

			if (!IsTestLJGame)
			{
				IsActivePlayer = true;
				if (IsTestXiaoScreen)
				{
					Screen.SetResolution(680, 384, false); //test
				}
			}
			Debug.Log("Unity:!!!!!!IsTestLJGame!!!!!!");

			QualitySettings.SetQualityLevel((int)QualityLevelEnum.Fast);
			Debug.Log("Unity:!!!!!!SetQualityLevel!!!!!!");
			//AudioSourceObj = transform.GetComponent<AudioSource>();
			Invoke("DelayResetIsLoadingLevel", 4f);
			if (IsOpenFXZhenDong)
			{
				IsOpenFXZhenDong = false;
				Invoke("CloseAllFangXiangPanPower", 10f);
			}
			Debug.Log("Unity:!!!!!!IsOpenFXZhenDong!!!!!!");
            //PlayMovie();
            //创建Logo播放对象.
            CrateMovieLogoAni();
            InputEventCtrl.GetInstance().ClickTVYaoKongExitBtEvent += ClickTVYaoKongExitBtEvent;
            pcvr.GetInstance().AddTVYaoKongBtEvent();
        }
		catch (System.Exception e)
		{
			Debug.Log("Unity:!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
			Debug.LogException(e);
			Debug.Log("Unity:"+e.Message);
		}
    }

    bool IsCrateMovieLogo = false;
    //MovieLogoAni m_MovieLogoAni;
    /// <summary>
    /// 创建Logo播放对象.
    /// </summary>
    void CrateMovieLogoAni()
    {
        Debug.Log("Unity: CrateMovieLogoAni...");
        if (IsCrateMovieLogo)
        {
            return;
        }
        IsCrateMovieLogo = true;

        //if (m_MovieAniPrefab != null && m_UITrParent != null)
        //{
        //    GameObject obj = (GameObject)Instantiate(m_MovieAniPrefab, m_UITrParent);
        //    m_MovieLogoAni = obj.GetComponent<MovieLogoAni>();
        //}
        //else
        //{
        //    Debug.LogWarning("Unity: m_MovieAniPrefab or m_UITrParent was null");
        //}
    }

    void Start()
    {
        //加载游戏关卡.
        Application.LoadLevel((int)GameLevel.Scene_1);
    }

    void RemoveMovieLogoAni()
    {
        Debug.Log("Unity: RemoveMovieLogoAni...");
        if (IsCrateMovieLogo)
        {
            IsCrateMovieLogo = false;
            //Destroy(m_MovieLogoAni.gameObject);
        }
    }

	void DelayResetIsLoadingLevel()
	{
		Debug.Log("Unity:!!!!!!DelayResetIsLoadingLevel2!!!!!!");
		XkGameCtrl.ResetIsLoadingLevel();
		if (NetworkServerNet.GetInstance() != null) {
			NetworkServerNet.GetInstance().TryToCreateServer();
        }
		Debug.Log("Unity:!!!!!!DelayResetIsLoadingLevel3!!!!!!");

		//InputEventCtrl.GetInstance().ClickStartBtOne(ButtonState.DOWN); //test.
        //InputEventCtrl.GetInstance().ClickStartBtOne(ButtonState.UP);
		//Debug.Log("Unity:!!!!!!DelayResetIsLoadingLevel4!!!!!!");
	}

	public void PlayMovie()
	{
#if UNITY_STANDALONE_WIN
  //      if (renderer != null) {
		//	renderer.enabled = true;
		//	renderer.material.mainTexture = Movie;
		//}
		//Movie.loop = false;
		//Movie.Play();
		
#if TEST_MOVIE
		TimeMv = Time.realtimeSinceStartup;
#endif
		
		//if (AudioSourceObj != null) {
		//	AudioSourceObj.clip = Movie.audioClip;
		//	AudioSourceObj.enabled = true;
		//	AudioSourceObj.Play();
		//}
#endif
        RemoveMovieLogoAni();
    }

	public void StopPlayMovie()
	{
#if UNITY_STANDALONE_WIN
        if (IsStopMovie) {
			return;
		}
		IsStopMovie = true;
		//Movie.Stop();
		//if (AudioSourceObj != null) {
		//	AudioSourceObj.Stop();
		//	AudioSourceObj.enabled = false;
		//}

		if (Network.peerType == NetworkPeerType.Server) {
			return;
		}
		gameObject.SetActive(false);
#endif
        RemoveMovieLogoAni();
        RemoveExitGameUI();
        
        if (m_ErWeiMaObj != null)
        {
            Destroy(m_ErWeiMaObj);
        }
    }

	void CloseAllFangXiangPanPower()
	{
		Debug.Log("Unity:!!!!!!CloseAllFangXiangPanPower!!!!!!");
		pcvr.GetInstance().CloseFangXiangPanPower();
	}

    /// <summary>
    /// 产生Websocket预制.
    /// </summary>
    public GameObject SpawnWebSocketBox(Transform tr)
    {
        Debug.Log("Unity:"+"SpawnWebSocketBox...");
        GameObject obj = (GameObject)Instantiate(m_WebSocketBoxPrefab);
        obj.transform.parent = tr;
        return obj;
    }

    private void ClickTVYaoKongExitBtEvent(ButtonState val)
    {
        if (val == ButtonState.UP)
        {
            if (m_ExitUICom == null)
            {
                SpawnExitGameUI();
            }
        }
    }

    /// <summary>
    /// 动态产生的UI父级Center.
    /// </summary>
    public Transform UICenterTrParent;
    /// <summary>
    /// 确定退出游戏的UI界面预制.
    /// </summary>
    public GameObject ExitGameUIPrefab;
    /// <summary>
    /// 退出游戏UI界面控制脚本.
    /// </summary>
    SSExitGameUI m_ExitUICom;
    /// <summary>
    /// 产生退出游戏UI界面.
    /// </summary>
    void SpawnExitGameUI()
    {
        Debug.Log("Unity: SpawnExitGameUI...");
        if (m_ExitUICom == null)
        {
            //m_MovieLogoAni.SetActiveHiddenObj(false);
            GameObject obj = (GameObject)Instantiate(ExitGameUIPrefab, UICenterTrParent);
            m_ExitUICom = obj.GetComponent<SSExitGameUI>();
            m_ExitUICom.Init();
        }
    }

    public void RemoveExitGameUI()
    {
        Debug.Log("Unity: RemoveExitGameUI...");
        if (m_ExitUICom != null)
        {
            m_ExitUICom.RemoveSelf();
            //m_MovieLogoAni.SetActiveHiddenObj(true);
        }
    }

#if TEST_MOVIE
	int CountMv = 0;
	float TimeMv = 0f;
	void OnGUI()
	{
		float timeMvPlay = Time.realtimeSinceStartup - TimeMv;
		if (timeMvPlay >= Movie.duration) {
			TimeMv = Time.realtimeSinceStartup;
			timeMvPlay = 0f;
			CountMv++;
		}

		string mvInfo = "MvDuration "+Movie.duration.ToString()
						+", CountMv "+CountMv
						+", timeMvPlay "+timeMvPlay.ToString("f2");
		GUI.Box(new Rect(0f, 0f, 500f, 35f), mvInfo);
	}
#endif
}