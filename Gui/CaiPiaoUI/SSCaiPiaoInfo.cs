using UnityEngine;

public class SSCaiPiaoInfo : SSGameMono
{
    /// <summary>
    /// 数字动画声音.
    /// </summary>
    public AudioSource m_AniAudio;
    /// <summary>
    /// 彩票数字管理组件.
    /// </summary>
    public SSGameNumUI m_GameNumUI;
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
    public void InitCaiPiaoAnimation(float timeVal, PlayerEnum indexPlayer)
    {
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
    }

    void Update()
    {
        if (m_GameNumUI != null && IsInitCaiPiaoAni)
        {
            if (Time.time - TimeLastCaiPiaoAni <= TimeCaiPiaoAni)
            {
                m_GameNumUI.ShowNumUI(Random.Range(1000, 9999));
            }
            else
            {
                //结束彩票数字动画.
                IsInitCaiPiaoAni = false;
                if (SSUIRoot.GetInstance().m_GameUIManage != null)
                {
                    int indexVal = (int)IndexPlayer - 1;
                    if (SSUIRoot.GetInstance().m_GameUIManage)
                    {
                        //显示玩家当前彩票数据信息.
                        int caiPiao = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_PcvrPrintCaiPiaoData[indexVal].CaiPiaoVal;
                        int caiPiaoCache = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_PcvrPrintCaiPiaoData[indexVal].CaiPiaoValCache;
                        m_GameNumUI.ShowNumUI(caiPiao + caiPiaoCache);
                    }
                }

                if (m_AniAudio != null)
                {
					m_AniAudio.Stop();
                    m_AniAudio.enabled = false;
                }
            }
        }
    }
}