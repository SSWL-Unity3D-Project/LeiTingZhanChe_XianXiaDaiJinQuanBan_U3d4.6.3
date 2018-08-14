using UnityEngine;

public class SSCaiPiaoFlyManage : SSGameMono
{
    /// <summary>
    /// 飞出的彩票预制.
    /// </summary>
    GameObject m_FlyCaiPiaoPrefab;
    /// <summary>
    /// 彩票产生的点.
    /// </summary>
    Vector3 m_CaiPiaoStartPos;
    /// <summary>
    /// 彩票移动的最终点.
    /// </summary>
    Transform m_CaiPiaoEndTr;
    /// <summary>
    /// 彩票的最大数量.
    /// </summary>
    int m_MaxCaiPiao;
    /// <summary>
    /// 彩票飞行时间.
    /// </summary>
    float m_TimeFly = 1f;
    /// <summary>
    /// 累积产生的彩票数量.
    /// </summary>
    int m_CaiPiaoCount;
    /// <summary>
    /// 彩票产生间隔时间.
    /// </summary>
    float m_CaiPiaoTime = 0.1f;
    float m_LastCaiPiaoTime;
    /// <summary>
    /// 玩家索引.
    /// </summary>
    PlayerEnum IndexPlayer;
    /// <summary>
    /// 是否创建彩票.
    /// </summary>
    bool IsCreatCaiPiao = false;
    void Update()
    {
        if (IsCreatCaiPiao == true)
        {
            if (Time.time - m_LastCaiPiaoTime >= m_CaiPiaoTime)
            {
                m_LastCaiPiaoTime = Time.time;
                //创建飞出的彩票.
                CreatFlyCaiPiao();
            }
        }
    }

    void CreatFlyCaiPiao()
    {
        if (m_FlyCaiPiaoPrefab == null)
        {
            UnityLogWarning("CreatFlyCaiPiao -> m_FlyCaiPiaoPrefab was null..............");
            return;
        }
        m_CaiPiaoCount++;

        GameObject obj = (GameObject)Instantiate(m_FlyCaiPiaoPrefab, XkGameCtrl.NpcAmmoArray);
        obj.transform.position = m_CaiPiaoStartPos;
        obj.transform.localEulerAngles = new Vector3(Random.Range(0f, 180f), Random.Range(0f, 180f), Random.Range(0f, 180f));
        //初始化彩票移动信息.
        SSCaiPiaoFly caiPiaoFly = obj.GetComponent<SSCaiPiaoFly>();
        if (caiPiaoFly != null)
        {
            Vector3[] path = new Vector3[2];
            path[0] = m_CaiPiaoStartPos;
            path[1] = m_CaiPiaoEndTr.position;
            caiPiaoFly.Init(m_CaiPiaoCount, m_TimeFly, path, IndexPlayer, m_DeCaiState);
        }

        if (m_CaiPiaoCount >= m_MaxCaiPiao)
        {
            //结束产生飞行彩票.
            IsCreatCaiPiao = false;
            Destroy(this);
        }
    }

    SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState m_DeCaiState;
    public void Init(Transform startTr, PlayerEnum indexPlayer, SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState type)
    {
        m_CaiPiaoStartPos = startTr.position;
        switch (indexPlayer)
        {
            case PlayerEnum.PlayerOne:
                {
                    m_CaiPiaoEndTr = XkGameCtrl.GetInstance().m_CaiPiaoFlyData.m_CaiPiaoEndTrP1;
                    break;
                }
            case PlayerEnum.PlayerTwo:
                {
                    m_CaiPiaoEndTr = XkGameCtrl.GetInstance().m_CaiPiaoFlyData.m_CaiPiaoEndTrP2;
                    break;
                }
            case PlayerEnum.PlayerThree:
                {
                    m_CaiPiaoEndTr = XkGameCtrl.GetInstance().m_CaiPiaoFlyData.m_CaiPiaoEndTrP3;
                    break;
                }
        }

        switch (type)
        {
            case SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhanChe:
                {
                    m_MaxCaiPiao = XkGameCtrl.GetInstance().m_CaiPiaoFlyData.m_ZhanCheCaiPiaoFlyDt.MaxCaiPiao;
                    m_TimeFly = XkGameCtrl.GetInstance().m_CaiPiaoFlyData.m_ZhanCheCaiPiaoFlyDt.TimeFly;
                    break;
                }
            case SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.SuiJiDaoJu:
                {
                    m_MaxCaiPiao = XkGameCtrl.GetInstance().m_CaiPiaoFlyData.m_SuiJiCaiPiaoFlyDt.MaxCaiPiao;
                    m_TimeFly = XkGameCtrl.GetInstance().m_CaiPiaoFlyData.m_SuiJiCaiPiaoFlyDt.TimeFly;
                    break;
                }
        }

        m_FlyCaiPiaoPrefab = XkGameCtrl.GetInstance().m_CaiPiaoFlyData.m_FlyCaiPiaoPrefab;
        m_LastCaiPiaoTime = Time.time;
        m_CaiPiaoCount = 0;
        IndexPlayer = indexPlayer;
        m_DeCaiState = type;
        IsCreatCaiPiao = true;
        CreatFlyCaiPiao();
    }
}