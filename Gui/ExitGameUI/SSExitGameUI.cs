using UnityEngine;

public class SSExitGameUI : MonoBehaviour
{
    public Vector3 m_BigScale = new Vector3(1.2f, 1.2f, 1f);
    public Vector3 m_SmallScale = Vector3.one;
    /// <summary>
    /// 确定按键的闪烁UI.
    /// </summary>
    public GameObject m_QueDingFlashObj;
    public UITexture QueDingUI;
    /// <summary>
    /// QueDingImg[0] 确定弹起.
    /// QueDingImg[1] 确定按下.
    /// </summary>
    public Texture[] QueDingImg;
    /// <summary>
    /// 返回按键的闪烁UI.
    /// </summary>
    public GameObject m_QuXiaoFlashObj;
    public UITexture QuXiaoUI;
    /// <summary>
    /// QuXiaoImg[0] 取消弹起.
    /// QuXiaoImg[1] 取消按下.
    /// </summary>
    public Texture[] QuXiaoImg;
    enum ExitEnum
    {
        QueDing,
        QuXiao,
    }
    ExitEnum m_ExitType = ExitEnum.QueDing;
    //static SSExitGameUI _Instance;
    //public static SSExitGameUI GetInstance()
    //{
    //    return _Instance;
    //}

    public void Init ()
    {
        m_ExitType = ExitEnum.QuXiao;
        SSUIRoot.GetInstance().m_ExitUICom = this;
        QueDingUI.mainTexture = QueDingImg[0];
        QuXiaoUI.mainTexture = QuXiaoImg[1];
        SetAcitveBtFlash();
        QueDingUI.transform.localScale = m_SmallScale;
        QuXiaoUI.transform.localScale = m_BigScale;
        //InputEventCtrl.GetInstance().ClickTVYaoKongEnterBtEvent += ClickTVYaoKongEnterBtEvent;
        //InputEventCtrl.GetInstance().ClickTVYaoKongExitBtEvent += ClickTVYaoKongExitBtEvent;
        //InputEventCtrl.GetInstance().ClickTVYaoKongLeftBtEvent += ClickTVYaoKongLeftBtEvent;
        //InputEventCtrl.GetInstance().ClickTVYaoKongRightBtEvent += ClickTVYaoKongRightBtEvent;
    }

    public void RemoveSelf()
    {
        SSUIRoot.GetInstance().m_ExitUICom = null;
        //InputEventCtrl.GetInstance().ClickTVYaoKongEnterBtEvent -= ClickTVYaoKongEnterBtEvent;
        //InputEventCtrl.GetInstance().ClickTVYaoKongExitBtEvent -= ClickTVYaoKongExitBtEvent;
        //InputEventCtrl.GetInstance().ClickTVYaoKongLeftBtEvent -= ClickTVYaoKongLeftBtEvent;
        //InputEventCtrl.GetInstance().ClickTVYaoKongRightBtEvent -= ClickTVYaoKongRightBtEvent;
        Destroy(gameObject);
    }
    
    private void ClickTVYaoKongLeftBtEvent(pcvr.ButtonState val)
    {
        if (val == pcvr.ButtonState.UP)
        {
            return;
        }
        m_ExitType = ExitEnum.QuXiao;
        QueDingUI.mainTexture = QueDingImg[0];
        QuXiaoUI.mainTexture = QuXiaoImg[1];
        QueDingUI.transform.localScale = m_SmallScale;
        QuXiaoUI.transform.localScale = m_BigScale;
        SetAcitveBtFlash();
    }

    private void ClickTVYaoKongRightBtEvent(pcvr.ButtonState val)
    {
        if (val == pcvr.ButtonState.UP)
        {
            return;
        }
        m_ExitType = ExitEnum.QueDing;
        QueDingUI.mainTexture = QueDingImg[1];
        QuXiaoUI.mainTexture = QuXiaoImg[0];
        QueDingUI.transform.localScale = m_BigScale;
        QuXiaoUI.transform.localScale = m_SmallScale;
        SetAcitveBtFlash();
    }

    void SetAcitveBtFlash()
    {
        if (m_QueDingFlashObj == null || m_QuXiaoFlashObj == null)
        {
            return;
        }

        switch (m_ExitType)
        {
            case ExitEnum.QueDing:
                {
                    m_QueDingFlashObj.SetActive(true);
                    m_QuXiaoFlashObj.SetActive(false);
                    break;
                }
            case ExitEnum.QuXiao:
                {
                    m_QueDingFlashObj.SetActive(false);
                    m_QuXiaoFlashObj.SetActive(true);
                    break;
                }
        }
    }

    private void ClickTVYaoKongEnterBtEvent(pcvr.ButtonState val)
    {

        if (m_ExitType == ExitEnum.QuXiao)
        {
            switch (val)
            {
                case pcvr.ButtonState.DOWN:
                    {
                        QuXiaoUI.mainTexture = QuXiaoImg[1];
                        break;
                    }
                case pcvr.ButtonState.UP:
                    {
                        QuXiaoUI.mainTexture = QuXiaoImg[0];
                        Debug.Log("Unity:" + "Player close exit game ui...");
                        if (XkGameCtrl.GetInstance() != null)
                        {
                            XkGameCtrl.GetInstance().RemoveExitGameUI();
                        }

                        if (GameMovieCtrl.GetInstance() != null)
                        {
                            GameMovieCtrl.GetInstance().RemoveExitGameUI();
                        }
                        break;
                    }
            }
        }

        if (m_ExitType == ExitEnum.QueDing)
        {
            switch (val)
            {
                case pcvr.ButtonState.DOWN:
                    {
                        QueDingUI.mainTexture = QueDingImg[1];
                        break;
                    }
                case pcvr.ButtonState.UP:
                    {
                        QueDingUI.mainTexture = QueDingImg[0];
                        Debug.Log("Unity:" + "Player exit application...");
                        if (XkGameCtrl.GetInstance() != null)
                        {
                            XkGameCtrl.GetInstance().RemoveExitGameUI();
                        }

                        if (GameMovieCtrl.GetInstance() != null)
                        {
                            GameMovieCtrl.GetInstance().RemoveExitGameUI();
                        }
                        Application.Quit();
                        break;
                    }
            }
        }
    }

    private void ClickTVYaoKongExitBtEvent(pcvr.ButtonState val)
    {
        switch (val)
        {
            case pcvr.ButtonState.DOWN:
                {
                    ClickTVYaoKongLeftBtEvent(val);
                    QuXiaoUI.mainTexture = QuXiaoImg[1];
                    break;
                }
            case pcvr.ButtonState.UP:
                {
                    QuXiaoUI.mainTexture = QuXiaoImg[0];
                    Debug.Log("Unity:" + "Player close exit game ui...");
                    if (XkGameCtrl.GetInstance() != null)
                    {
                        XkGameCtrl.GetInstance().RemoveExitGameUI();
                    }

                    if (GameMovieCtrl.GetInstance() != null)
                    {
                        GameMovieCtrl.GetInstance().RemoveExitGameUI();
                    }
                    break;
                }
        }
    }
}