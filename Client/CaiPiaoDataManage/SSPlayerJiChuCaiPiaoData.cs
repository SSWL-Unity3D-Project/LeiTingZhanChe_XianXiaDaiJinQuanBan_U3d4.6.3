using UnityEngine;

public class SSPlayerJiChuCaiPiaoData : SSGameMono
{
    /// <summary>
    /// 送票间隔时间.
    /// </summary>
    float m_JianGeTime = 15f;
    /// <summary>
    /// 最大送票次数.
    /// </summary>
    int m_MaxCountSongPiao = 10;
    /// <summary>
    /// 必定送票的次数信息.
    /// </summary>
    int m_BiDingSongPiaoCountFirst = 1;
    int m_BiDingSongPiaoCountLast = 10;
    /// <summary>
    /// 送出1张彩票的概率.
    /// </summary>
    float m_SongPiaoGaiLv01 = 0.8f;
    public class JiChuCaiPiaoData
    {
        /// <summary>
        /// 玩家基础彩票数量.
        /// </summary>
        public int JiChuCaiPiaoVal = 0;
        /// <summary>
        /// 送票次数.
        /// </summary>
        public int SongPiaoCount = 0;
        /// <summary>
        /// 送票时间信息.
        /// </summary>
        public float LastSongPiaoTime = 0f;
        /// <summary>
        /// 是否可以送票.
        /// </summary>
        public bool IsCanSongPiao = false;
        public void Reset()
        {
            SongPiaoCount = 0;
            LastSongPiaoTime = Time.time;
            IsCanSongPiao = false;
        }
    }
    /// <summary>
    /// 玩家基础彩票数据信息.
    /// </summary>
    JiChuCaiPiaoData[] m_JiChuCaiPiaoData = new JiChuCaiPiaoData[3]
    {
        new JiChuCaiPiaoData(),
        new JiChuCaiPiaoData(),
        new JiChuCaiPiaoData(),
    };

    void Update()
    {
        if (XKTriggerYuLeCtrl.IsActiveYuLeTrigger == true)
        {
            return;
        }

        if (Time.frameCount % 10 != 0)
        {
            return;
        }

        for (int i = 0; i < m_JiChuCaiPiaoData.Length; i++)
        {
            if (m_JiChuCaiPiaoData[i] != null)
            {
                UpdateJiChuCaiPiaoInfo(m_JiChuCaiPiaoData[i]);
            }
        }
    }

    void UpdateJiChuCaiPiaoInfo(JiChuCaiPiaoData dt)
    {
        if (dt.JiChuCaiPiaoVal <= 0)
        {
            return;
        }

        if (dt.SongPiaoCount >= m_MaxCountSongPiao)
        {
            return;
        }

        if (dt.IsCanSongPiao == true)
        {
            return;
        }

        if (Time.time - dt.LastSongPiaoTime >= m_JianGeTime)
        {
            //可以送出基础彩票了.
            dt.IsCanSongPiao = true;
            dt.SongPiaoCount++;
        }
    }

    /// <summary>
    /// 添加正常得彩数据.
    /// </summary>
    public void AddPlayerJiChuCaiPiao(PlayerEnum indexPlayer, int caiPiao)
    {
        int indexVal = (int)indexPlayer - 1;
        if (indexVal < 0 || indexVal > 2)
        {
            UnityLogWarning("AddPlayerJiChuCaiPiao -> indexVal was wrong! indexVal ======= " + indexVal);
            return;
        }
        m_JiChuCaiPiaoData[indexVal].JiChuCaiPiaoVal += caiPiao;
        m_JiChuCaiPiaoData[indexVal].Reset();
    }

    /// <summary>
    /// 检测玩家送票信息.
    /// </summary>
    public void CheckPlayerSongPiaoInfo(PlayerEnum indexPlayer, Vector3 pos)
    {
        if (XKTriggerYuLeCtrl.IsActiveYuLeTrigger == true)
        {
            //娱乐触发器激活后不去送基础彩票.
            return;
        }

        int indexVal = (int)indexPlayer - 1;
        if (indexVal < 0 || indexVal > 2)
        {
            UnityLogWarning("CheckPlayerSongPiaoInfo -> indexVal was wrong! indexVal ======= " + indexVal);
            return;
        }

        if (m_JiChuCaiPiaoData[indexVal].IsCanSongPiao == false)
        {
            return;
        }

        if (m_JiChuCaiPiaoData[indexVal].JiChuCaiPiaoVal <= 0)
        {
            return;
        }

        int randVal = Random.Range(0, 100) % 2;
        if (m_JiChuCaiPiaoData[indexVal].SongPiaoCount == m_BiDingSongPiaoCountFirst
            || m_JiChuCaiPiaoData[indexVal].SongPiaoCount == m_BiDingSongPiaoCountLast)
        {
            //必定送票.
            randVal = 0;
        }

        if (m_JiChuCaiPiaoData[indexVal].SongPiaoCount < m_BiDingSongPiaoCountLast)
        {
            if (m_JiChuCaiPiaoData[indexVal].JiChuCaiPiaoVal <= 1)
            {
                //基础彩票留下1张到最后一次送出.
                return;
            }
        }

        if (randVal != 0)
        {
            return;
        }

        //送出彩票数量.
        int chuPiaoVal = 1;
        float randGaiLv = Random.Range(0f, 100f) / 100f;
        if (randGaiLv > m_SongPiaoGaiLv01)
        {
            chuPiaoVal = 2;
        }

        if (m_JiChuCaiPiaoData[indexVal].JiChuCaiPiaoVal < chuPiaoVal)
        {

            if (m_JiChuCaiPiaoData[indexVal].SongPiaoCount < m_BiDingSongPiaoCountLast)
            {
                //给最后一次留1张票.
                chuPiaoVal = m_JiChuCaiPiaoData[indexVal].JiChuCaiPiaoVal - 1;
            }
            else
            {
                //最后一次送出剩余基础彩票.
                chuPiaoVal = m_JiChuCaiPiaoData[indexVal].JiChuCaiPiaoVal;
            }
        }

        if (chuPiaoVal <= 0)
        {
            UnityLogWarning("CheckPlayerSongPiaoInfo -> jiChuCaiPiao was out over..........");
            return;
        }
        UnityLog("CheckPlayerSongPiaoInfo -> chuPiaoVal ====== " + chuPiaoVal
            + ", count == " + m_JiChuCaiPiaoData[indexVal].SongPiaoCount
            + ", time == " + Time.time);

        //进入下一周期计时.
        m_JiChuCaiPiaoData[indexVal].IsCanSongPiao = false;
        m_JiChuCaiPiaoData[indexVal].LastSongPiaoTime = Time.time;
        m_JiChuCaiPiaoData[indexVal].JiChuCaiPiaoVal -= chuPiaoVal;

        if (XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage != null)
        {
            //减少玩家正常得彩.
            XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.SubPlayerZhengChangDeCai(indexPlayer, chuPiaoVal);
        }

        //可以送出基础彩票.
        if (XkGameCtrl.GetInstance().m_CaiPiaoFlyData != null
            && XkGameCtrl.GetInstance().m_CaiPiaoFlyData.m_JiChuCaiPiaoLiZiPrefab != null)
        {
            GameObject obj = (GameObject)Instantiate(XkGameCtrl.GetInstance().m_CaiPiaoFlyData.m_JiChuCaiPiaoLiZiPrefab, XkGameCtrl.PlayerAmmoArray);
            obj.transform.position = pos + (Vector3.up * 2f);

            if (XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage != null)
            {
                //int value = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.m_GameCaiPiaoData.GetPrintCaiPiaoValueByDeCaiState(SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.SuiJiDaoJu, DaoJuType);
                SSCaiPiaoLiZiManage caiPiaoLiZi = obj.GetComponent<SSCaiPiaoLiZiManage>();
                if (caiPiaoLiZi != null)
                {
                    caiPiaoLiZi.ShowNumUI(chuPiaoVal, indexPlayer);
                }
                else
                {
                    Debug.LogWarning("CheckPlayerSongPiaoInfo -> caiPiaoLiZi was null.................");
                }
            }

            if (XkGameCtrl.GetInstance().m_CaiPiaoFlyData != null)
            {
                //初始化飞出的彩票逻辑.
                XkGameCtrl.GetInstance().m_CaiPiaoFlyData.InitCaiPiaoFly(obj.transform.position, indexPlayer, SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState.ZhengChang);
            }
            else
            {
                Debug.LogWarning("CheckPlayerSongPiaoInfo -> m_CaiPiaoFlyData was null............");
            }
        }
    }
}