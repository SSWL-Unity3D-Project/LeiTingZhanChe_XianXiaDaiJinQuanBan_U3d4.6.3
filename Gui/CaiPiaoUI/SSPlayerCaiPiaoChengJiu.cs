using UnityEngine;
using System.Collections;

/// <summary>
/// 玩家游戏结束后的彩票成就UI管理.
/// </summary>
public class SSPlayerCaiPiaoChengJiu : SSGameMono
{
    PlayerEnum IndexPlayer;
    /// <summary>
    /// 成就图片列表.
    /// </summary>
    public Texture[] m_ChengJiuImgArray = new Texture[9];
    /// <summary>
    /// 成就UI.
    /// </summary>
    public UITexture m_ChengJiuUITexture;
    
    /// <summary>
    /// 分数显示组件.
    /// </summary>
    public SSGameNumUI m_ScoreNumUICom;
    /// <summary>
    /// 分数滚动音效.
    /// </summary>
    public AudioSource m_ScoreAudio;
    /// <summary>
    /// 播放数字随机动画.
    /// </summary>
    bool IsPlayRandNumAni = false;
    float m_LastRandNumAniTime = 0;

    /// <summary>
    /// 彩票数字UI组件.
    /// </summary>
    public SSGameNumUI m_CaiPiaoNumUICom;
    /// <summary>
    /// 彩票数字信息UI总父级对象.
    /// </summary>
    public GameObject m_CaiPiaoInfoParent;
    /// <summary>
    /// 彩票数量.
    /// </summary>
    int m_CardNumVal = 0;
	// Use this for initialization
	public void Init(PlayerEnum indexPlayer, int score, int caiPiaoNum)
    {
        IndexPlayer = indexPlayer;
        if (m_ChengJiuUITexture != null)
        {
            m_ChengJiuUITexture.mainTexture = m_ChengJiuImgArray[Random.Range(0, 100) % m_ChengJiuImgArray.Length];
        }

        if (m_ScoreNumUICom != null)
        {
            m_ScoreNumUICom.ShowNumUI(score);
        }
        m_CardNumVal = caiPiaoNum;
        m_ChengJiuCount++;

        StartCoroutine(DelayPlayGameNumAnimation());
    }

    IEnumerator DelayPlayGameNumAnimation()
    {
        yield return new WaitForSeconds(1f);
        IsPlayRandNumAni = true;
        m_LastRandNumAniTime = Time.time;
        if (m_ScoreAudio != null)
        {
            m_ScoreAudio.Play();
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        UpdateGameNumUIAnimation();
    }

    /// <summary>
    /// 更新游戏分数动画.
    /// </summary>
    void UpdateGameNumUIAnimation()
    {
        if (IsPlayRandNumAni == true)
        {
            if (Time.time - m_LastRandNumAniTime >= 3f)
            {
                IsPlayRandNumAni = false;
                if (m_ScoreAudio != null)
                {
                    m_ScoreAudio.Stop();
                }
                //动画播放结束.
                m_ScoreNumUICom.ShowNumUI(0, "KillNum_");
                //显示剩余正常彩票数.
                ShowCaiPiaoNumUI();
                return;
            }
            m_ScoreNumUICom.ShowNumUI(Random.Range(10000000, 99999999), "KillNum_");
        }
    }

    /// <summary>
    /// 显示剩余彩票数量.
    /// </summary>
    void ShowCaiPiaoNumUI()
    {
        if (m_CaiPiaoInfoParent != null)
        {
            m_CaiPiaoInfoParent.SetActive(true);
            if (m_CaiPiaoNumUICom != null)
            {
                m_CaiPiaoNumUICom.ShowNumUI(m_CardNumVal);
            }
        }
        PlayCaiPiaoNumAnimationSuoFang();

        IsDelayRemoveSelf = true;
        StartCoroutine(DelayRemoveSelf());
    }

    bool IsDelayRemoveSelf = false;
    IEnumerator DelayRemoveSelf()
    {
        yield return new WaitForSeconds(5f);
        if (SSUIRoot.GetInstance().m_GameUIManage != null)
        {
            SSUIRoot.GetInstance().m_GameUIManage.RemovePlayerCaiPiaoChengJiu(IndexPlayer);
        }
    }

    /// <summary>
    /// 是否播放了彩票数的缩放动画.
    /// </summary>
    bool IsPlayCaiPiaoNumAniSuoFang = false;
    /// <summary>
    /// 播放玩家当前打印彩票的数字缩放动画.
    /// </summary>
    void PlayCaiPiaoNumAnimationSuoFang()
    {
        if (IsPlayCaiPiaoNumAniSuoFang == true)
        {
            return;
        }
        IsPlayCaiPiaoNumAniSuoFang = true;

        //播放玩家当前打印彩票的数字缩放动画.
        if (SSUIRoot.GetInstance().m_GameUIManage != null)
        {
            int indexVal = (int)IndexPlayer - 1;
            if (indexVal >= 0 && indexVal <= 2)
            {
                //添加剩余彩票数.
                XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.AddCaiPiaoToPlayer(IndexPlayer, m_CardNumVal, SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhengChang, false);

                int caiPiao = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_PcvrPrintCaiPiaoData[indexVal].CaiPiaoVal;
                int caiPiaoCache = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_PcvrPrintCaiPiaoData[indexVal].CaiPiaoValCache;
                //显示玩家彩票数量.
                SSUIRoot.GetInstance().m_GameUIManage.ShowPlayerCaiPiaoInfo(IndexPlayer, caiPiao + caiPiaoCache, true, true);
            }
        }
    }

    /// <summary>
    /// 成就UI计数.
    /// </summary>
    static byte m_ChengJiuCount = 0;
    static float m_LastTimeJiaoYanVal = 0f;
    bool IsRemoveSelf = false;
    public void RemoveSelf()
    {
        if (IsRemoveSelf == false)
        {
            IsRemoveSelf = true;
            if (IsDelayRemoveSelf == true)
            {
                StopCoroutine(DelayRemoveSelf());
            }

            if (IsPlayCaiPiaoNumAniSuoFang == false)
            {
                PlayCaiPiaoNumAnimationSuoFang();
            }

            m_ChengJiuCount--;
            int randVal = Random.Range(0, 100) % 2;
            if (Time.time - m_LastTimeJiaoYanVal > 60f * 20f)
            {
                //超过一定时间必然执行校验.
                randVal = 0;
                m_LastTimeJiaoYanVal = Time.time;
            }

            if (XkGameCtrl.PlayerActiveNum <= 0 && m_ChengJiuCount == 0 && randVal == 0)
            {
                //没有玩家激活游戏,进行一次精锐4加密校验.
                SSJingRuiJiaMi.OnGameOverCheckJingRuiJiaMi();

                if (pcvr.GetInstance().mPcvrTXManage != null)
                {
                    //进行一次加密芯片校验.
                    pcvr.GetInstance().mPcvrTXManage.StartJiaoYanIO();
                }
            }
            Destroy(gameObject);
        }
    }
}