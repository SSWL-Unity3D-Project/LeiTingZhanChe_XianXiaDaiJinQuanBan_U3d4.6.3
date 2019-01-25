using System.Collections;
using UnityEngine;

public class SSCaiPiaoInfo : SSGameMono
{
    /// <summary>
    /// 数字缩放动画.
    /// </summary>
    public Animator m_AnimationNumSuoFang;
    /// <summary>
    /// 数字动画声音.
    /// </summary>
    public AudioSource m_AniAudio;
    /// <summary>
    /// 彩票数字管理组件.
    /// </summary>
    public SSGameNumUI m_GameNumUI;
    /// <summary>
    /// 代金券商户信息5个字.
    /// </summary>
    public UILabel m_DaiJinQuanShangHuInfo;
    /// <summary>
    /// 正在出票UI.
    /// </summary>
    public GameObject m_ZhengZaiChuPiaoUI;
    /// <summary>
    /// 设置正在出票UI的显示状态.
    /// </summary>
    public void SetActiveZhengZaiChuPiao(bool isActive)
    {
        if (m_ZhengZaiChuPiaoUI != null)
        {
            m_ZhengZaiChuPiaoUI.SetActive(isActive);
        }
        else
        {
            UnityLogWarning("SetActiveZhengZaiChuPiao -> m_ZhengZaiChuPiaoUI was null!");
        }
    }

    bool IsInitCaiPiaoAni = false;
    float TimeCaiPiaoAni = 1f;
    float TimeLastCaiPiaoAni = 0f;
    PlayerEnum IndexPlayer = PlayerEnum.Null;
    public void InitCaiPiaoAnimation(float timeVal, PlayerEnum indexPlayer, SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState deCaiType)
    {
#if OPEN_CAIPIAO_ANIMATION
        IndexPlayer = indexPlayer;
        TimeCaiPiaoAni = timeVal;
        TimeLastCaiPiaoAni = Time.time;
        IsInitCaiPiaoAni = true;

        if (m_AniAudio != null)
        {
            m_AniAudio.enabled = true;
            m_AniAudio.loop = true;
			m_AniAudio.Play();
        }
#else
        IndexPlayer = indexPlayer;
        ShowPlayerCaiPiaoInfo();
        ShowDaiJinQuanShangHuInfo(deCaiType);
#endif

        if (SSUIRoot.GetInstance().m_GameUIManage != null)
        {
            SSUIRoot.GetInstance().m_GameUIManage.CreatPlayerDaiJinQuanUI(indexPlayer);
        }
    }

    internal void ShowDaiJinQuanShangHuInfo(SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState deCaiType)
    {
        if (m_DaiJinQuanShangHuInfo != null)
        {
            string info = "";
            if (XkGameCtrl.GetInstance().m_SSShangHuInfo != null)
            {
                switch (deCaiType)
                {
                    case SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.JPBoss:
                        {
                            info = XkGameCtrl.GetInstance().m_SSShangHuInfo.GetJPBossShangHuMingDt();
                            break;
                        }
                    case SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhanChe:
                        {
                            info = XkGameCtrl.GetInstance().m_SSShangHuInfo.GetShangHuMingDt().ShangHuMing;
                            break;
                        }
                    case SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.SuiJiDaoJu:
                        {
                            info = XkGameCtrl.GetInstance().m_SSShangHuInfo.GetSuiJiDaoJuShangHuDt().ShangHuMing;
                            break;
                        }
                }

                if (info != "")
                {
                    m_DaiJinQuanShangHuInfo.text = info;
                }
            }
        }
    }

    public void Init(PlayerEnum indexPlayer)
    {
        switch (indexPlayer)
        {
            case PlayerEnum.PlayerOne:
                {
                    InputEventCtrl.GetInstance().ClickStartBtOneEvent += ClickStartBtOneEvent;
                    break;
                }
            case PlayerEnum.PlayerTwo:
                {
                    InputEventCtrl.GetInstance().ClickStartBtTwoEvent += ClickStartBtTwoEvent;
                    break;
                }
            case PlayerEnum.PlayerThree:
                {
                    InputEventCtrl.GetInstance().ClickStartBtThreeEvent += ClickStartBtThreeEvent;
                    break;
                }
        }
    }

    private void ClickStartBtOneEvent(pcvr.ButtonState val)
    {
        if (val == pcvr.ButtonState.UP)
        {
            PcvrRestartPrintCaiPiao(PlayerEnum.PlayerOne);
        }
    }

    private void ClickStartBtTwoEvent(pcvr.ButtonState val)
    {
        if (val == pcvr.ButtonState.UP)
        {
            PcvrRestartPrintCaiPiao(PlayerEnum.PlayerTwo);
        }
    }

    private void ClickStartBtThreeEvent(pcvr.ButtonState val)
    {
        if (val == pcvr.ButtonState.UP)
        {
            PcvrRestartPrintCaiPiao(PlayerEnum.PlayerThree);
        }
    }
    
    /// <summary>
    /// 重新开始出票.
    /// </summary>
    void PcvrRestartPrintCaiPiao(PlayerEnum indexPlayer)
    {
        //这里添加pcvr重新出票的代码.
        pcvr.GetInstance().RestartPrintCaiPiao(indexPlayer);
    }

    void Update()
    {
        if (m_GameNumUI != null && IsInitCaiPiaoAni)
        {
            if (Time.time - TimeLastCaiPiaoAni <= TimeCaiPiaoAni)
            {
                m_GameNumUI.ShowNumUI(UnityEngine.Random.Range(1000, 9999));
            }
            else
            {
                //结束彩票数字动画.
                IsInitCaiPiaoAni = false;
                ShowPlayerCaiPiaoInfo();

                if (m_AniAudio != null)
                {
					m_AniAudio.Stop();
                    m_AniAudio.enabled = false;
                }
            }
        }
    }

    /// <summary>
    /// 展示玩家彩票信息.
    /// </summary>
    void ShowPlayerCaiPiaoInfo()
    {
        if (SSUIRoot.GetInstance().m_GameUIManage != null)
        {
            int indexVal = (int)IndexPlayer - 1;
            if (SSUIRoot.GetInstance().m_GameUIManage)
            {
                //显示玩家当前彩票数据信息.
                int caiPiao = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_PcvrPrintCaiPiaoData[indexVal].CaiPiaoVal;
                int caiPiaoCache = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_PcvrPrintCaiPiaoData[indexVal].CaiPiaoValCache;
                //m_GameNumUI.ShowNumUI(caiPiao + caiPiaoCache);
                if (SSUIRoot.GetInstance().m_GameUIManage != null)
                {
                    //显示玩家彩票数量.
                    SSUIRoot.GetInstance().m_GameUIManage.ShowPlayerCaiPiaoInfo(IndexPlayer, caiPiao + caiPiaoCache);
                }
            }
        }
    }

    /// <summary>
    /// 播放彩票数字缩放动画.
    /// </summary>
    public void PlayCaiPiaoNumSuoFangAnimation()
    {
        if (m_AnimationNumSuoFang != null)
        {
            m_AnimationNumSuoFang.enabled = true;
            m_AnimationNumSuoFang.SetBool("IsPlay", true);
            StartCoroutine(DelayCloseCaiPiaoNumSuoFangAnimation());
        }
    }

    IEnumerator DelayCloseCaiPiaoNumSuoFangAnimation()
    {
        yield return new WaitForSeconds(3f);
        if (m_AnimationNumSuoFang != null)
        {
            m_AnimationNumSuoFang.SetBool("IsPlay", false);
            //m_AnimationNumSuoFang.enabled = false;
        }
    }

    bool IsRemoveSelf = false;
    internal void RemoveSelf(PlayerEnum indexPlayer)
    {
        if (IsRemoveSelf == false)
        {
            IsRemoveSelf = true;

            switch (indexPlayer)
            {
                case PlayerEnum.PlayerOne:
                    {
                        InputEventCtrl.GetInstance().ClickStartBtOneEvent -= ClickStartBtOneEvent;
                        break;
                    }
                case PlayerEnum.PlayerTwo:
                    {
                        InputEventCtrl.GetInstance().ClickStartBtTwoEvent -= ClickStartBtTwoEvent;
                        break;
                    }
                case PlayerEnum.PlayerThree:
                    {
                        InputEventCtrl.GetInstance().ClickStartBtThreeEvent -= ClickStartBtThreeEvent;
                        break;
                    }
            }
            Destroy(gameObject);
        }
    }
}