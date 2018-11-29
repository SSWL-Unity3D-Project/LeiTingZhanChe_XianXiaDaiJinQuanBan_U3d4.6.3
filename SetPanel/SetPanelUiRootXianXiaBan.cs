using UnityEngine;

public class SetPanelUiRootXianXiaBan : SSGameMono
{

	// Use this for initialization
	void Start()
    {
        Init();
    }
	
    void Init()
    {
        Screen.showCursor = true;
        Screen.SetResolution(400, 300, false);
        InitDanMuInfo();
    }

    /// <summary>
    /// 弹幕信息.
    /// </summary>
    public UILabel m_DanMuLabel;
    /// <summary>
    /// 弹幕输入.
    /// </summary>
    public UIInput m_DanMuInput;
    /// <summary>
    /// 初始化弹幕信息.
    /// </summary>
    void InitDanMuInfo()
    {
        if (XKGlobalData.GetInstance().m_MoRenDanMuInfo != XKGlobalData.GetInstance().m_DanMuInfo)
        {
            m_DanMuLabel.text = XKGlobalData.GetInstance().m_DanMuInfo;
        }
        else
        {
            m_DanMuLabel.text = XKGlobalData.GetInstance().m_MoRenDanMuInfo;
        }
    }

    /// <summary>
    /// 重置弹幕信息.
    /// </summary>
    void ResetDanMuInfo()
    {
        SetDanMuInfo(XKGlobalData.GetInstance().m_MoRenDanMuInfo);
        m_DanMuInput.value = "";
        m_DanMuLabel.text = XKGlobalData.GetInstance().m_MoRenDanMuInfo;
    }

    /// <summary>
    /// 设置弹幕信息.
    /// </summary>
    void SetDanMuInfo(string info)
    {
        if (info.Length > 0 && info.Length <= 10)
        {
            XKGlobalData.GetInstance().SetDanMuInfo(info);
        }
        else
        {
            //UnityLogWarning("DanMuInfo length wrong! info == " + info);
        }
    }

    public void OnDanMuInputChange(string info)
    {
        //UnityLog("OnDanMuInputChange -> info == " + info);
        SetDanMuInfo(info);
    }

    /// <summary>
    /// 点击重置信息按键.
    /// </summary>
    public void OnClickResetBt()
    {
        UnityLog("OnClickResetBt...");
        ResetDanMuInfo();
    }

    /// <summary>
    /// 点击退出按键.
    /// </summary>
    public void OnClickExitBt()
    {
        UnityLog("OnClickExitBt...");
        BackMovieScene();
    }

    void BackMovieScene()
    {
        if (Application.loadedLevel != (int)GameLevel.Movie)
        {
            XkGameCtrl.ResetGameInfo();
            if (!XkGameCtrl.IsGameOnQuit)
            {
                Screen.showCursor = false;
                Screen.SetResolution(XkGameCtrl.ScreenData.width, XkGameCtrl.ScreenData.height, true);
                System.GC.Collect();
                Application.LoadLevel((int)GameLevel.Movie);
            }
        }
    }
}
