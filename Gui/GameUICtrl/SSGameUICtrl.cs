using UnityEngine;
using System.Collections;

public class SSGameUICtrl : SSGameMono
{
    /// <summary>
    /// 游戏UI界面中心锚点.
    /// </summary>
    public Transform m_GameUICenter;
    /// <summary>
    /// 游戏UI界面左下锚点.
    /// </summary>
    public Transform m_GameUIBottomLeft;
    /// <summary>
    /// 玩家UI父级tr.
    /// </summary>
    public Transform[] m_PlayerUIParent = new Transform[3];

    void Awake()
    {
        if (SSUIRoot.GetInstance() != null)
        {
            SSUIRoot.GetInstance().m_GameUIManage = this;
        }
        CreatNpcPiaoFenUICom();
    }

    /// <summary>
    /// 创建战车boss彩票转盘.
    /// </summary>
    public void CreatZhanCheBossCaiPiaoZhuanPan(PlayerEnum indexPlayer, int caiPiaoVal, Vector3 pos, SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState type, GameObject exp)
    {
        //StartCoroutine(DelayCreatZhanCheBossCaiPiaoZhuanPan(indexPlayer, caiPiaoVal, pos, type, exp));
        if (m_GameUIBottomLeft == null)
        {
            UnityLogWarning("CreatZhanCheBossCaiPiaoZhuanPan -> m_GameUIBottomLeft was null.........");
            return;
        }

        GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/GUI/CaiPiaoUI/CaiPiaoZhuanPan");
        if (gmDataPrefab != null)
        {
            UnityLog("CreatZhanCheBossCaiPiaoZhuanPan...");
            GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_GameUIBottomLeft);
            SSCaiPiaoZhanCheBossUI caiPiaoZhuanPan = obj.GetComponent<SSCaiPiaoZhanCheBossUI>();
            if (caiPiaoZhuanPan != null)
            {
                caiPiaoZhuanPan.Init(indexPlayer, caiPiaoVal, pos, type, exp);
            }
        }
        else
        {
            UnityLogWarning("CreatZhanCheBossCaiPiaoZhuanPan -> gmDataPrefab was null");
        }
    }

    //IEnumerator DelayCreatZhanCheBossCaiPiaoZhuanPan(PlayerEnum indexPlayer, int caiPiaoVal, Vector3 pos, SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState type, GameObject exp)
    //{
    //	yield return new WaitForSeconds(1f);
    //	if (m_GameUIBottomLeft == null)
    //	{
    //		UnityLogWarning("CreatZhanCheBossCaiPiaoZhuanPan -> m_GameUIBottomLeft was null.........");
    //		yield break;
    //	}

    //	GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/GUI/CaiPiaoUI/CaiPiaoZhuanPan");
    //	if (gmDataPrefab != null)
    //	{
    //		UnityLog("CreatZhanCheBossCaiPiaoZhuanPan...");
    //		GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_GameUIBottomLeft);
    //		SSCaiPiaoZhanCheBossUI caiPiaoZhuanPan = obj.GetComponent<SSCaiPiaoZhanCheBossUI>();
    //		if (caiPiaoZhuanPan != null)
    //		{
    //			caiPiaoZhuanPan.Init(indexPlayer, caiPiaoVal, pos, type, exp);
    //		}
    //	}
    //	else
    //	{
    //		UnityLogWarning("CreatZhanCheBossCaiPiaoZhuanPan -> gmDataPrefab was null");
    //	}
    //}

    /// <summary>
    /// npc飘分控制组件.
    /// </summary>
    internal SSNpcPiaoFenCtrl m_NpcPiaoFenCom;
    /// <summary>
    /// 创建npc飘分UI组件.
    /// </summary>
    void CreatNpcPiaoFenUICom()
    {
        if (m_GameUIBottomLeft == null)
        {
            UnityLogWarning("CreatNpcPiaoFenUICom -> m_GameUIBottomLeft was null.........");
            return;
        }

        GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/GUI/NpcPiaoFen/NpcPiaoFenCtrl");
        if (gmDataPrefab != null)
        {
            UnityLog("CreatNpcPiaoFenUICom...");
            GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_GameUIBottomLeft);
            m_NpcPiaoFenCom = obj.GetComponent<SSNpcPiaoFenCtrl>();
            if (m_NpcPiaoFenCom != null)
            {
                m_NpcPiaoFenCom.Init();
            }
        }
        else
        {
            UnityLogWarning("CreatNpcPiaoFenUICom -> gmDataPrefab was null");
        }
    }

    /// <summary>
    /// 展示npc飘分UI信息.
    /// </summary>
    public void ShowNpcPiaoFenUI(PlayerEnum indexVal, int fenShuVal, Vector3 piaoFenPos)
    {
        if (m_NpcPiaoFenCom == null)
        {
            UnityLogWarning("ShowNpcPiaoFenUI -> m_NpcPiaoFenCom was null.............");
            return;
        }
        m_NpcPiaoFenCom.ShowFenShuInfo(indexVal, fenShuVal, piaoFenPos);
    }

    /// <summary>
    /// 彩票不足UI.
    /// </summary>
    internal SSCaiPiaoBuZu[] m_CaiPiaoBuZuArray = new SSCaiPiaoBuZu[3];
    /// <summary>
    /// 创建彩票不足UI界面.
    /// </summary>
    public void CreatCaiPiaoBuZuPanel(PlayerEnum indexPlayer)
    {
        int index = (int)indexPlayer - 1;
        if (index < 0 || index > 2)
        {
            UnityLogWarning("CreatCaiPiaoBuZuPanel -> index was wrong! index ==== " + index);
            return;
        }

        GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/GUI/CaiPiaoUI/CaiPiaoBuZu");
        if (gmDataPrefab != null)
        {
            if (m_CaiPiaoBuZuArray[index] == null)
            {
                if (m_PlayerUIParent[index] != null)
                {
                    UnityLog("CreatCaiPiaoBuZuPanel...");
                    GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_PlayerUIParent[index]);
                    m_CaiPiaoBuZuArray[index] = obj.GetComponent<SSCaiPiaoBuZu>();
                    m_CaiPiaoBuZuArray[index].Init(indexPlayer);
                    SetActiveZhengZaiChuPiaoUI(indexPlayer, false);
                }
                else
                {
                    UnityLogWarning("CreatCaiPiaoBuZuPanel -> m_PlayerUIParent was wrong! index ==== " + index);
                }
            }
        }
        else
        {
            UnityLogWarning("CreatCaiPiaoBuZuPanel -> gmDataPrefab was null");
        }
    }

    /// <summary>
    /// 删除彩票不足UI.
    /// </summary>
    public void RemoveCaiPiaoBuZuPanel(PlayerEnum indexPlayer, bool isWorkerDone)
    {
        int index = (int)indexPlayer - 1;
        if (index < 0 || index > 2)
        {
            UnityLogWarning("RemoveCaiPiaoBuZuPanel -> index was wrong! index ==== " + index);
            return;
        }

        if (m_CaiPiaoBuZuArray[index] != null)
        {
            UnityLog("RemoveCaiPiaoBuZuPanel -> index ==== " + index);
            m_CaiPiaoBuZuArray[index].RemoveSelf();
            SetActiveZhengZaiChuPiaoUI(indexPlayer, true);

            if (isWorkerDone)
            {
                //工作人员清票.
                //这里添加彩票不足机位的彩票数据清理逻辑代码.
                if (XkPlayerCtrl.GetInstanceFeiJi() != null)
                {
                    XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.ClearPlayerCaiPiaoData(indexPlayer);
                }
            }
        }
    }

    /// <summary>
    /// 删除所有彩票不足界面.
    /// </summary>
    public void RemoveAllCaiPiaoBuZuPanel()
    {
        //工作人员清票.
        for (int i = 0; i < m_CaiPiaoBuZuArray.Length; i++)
        {
            if (m_CaiPiaoBuZuArray[i] != null)
            {
                RemoveCaiPiaoBuZuPanel((PlayerEnum)(i + 1), true);
            }
        }
    }

    /// <summary>
    /// 玩家彩票数量UI.
    /// </summary>
    internal SSGameNumUI[] m_CaiPiaoInfoArray = new SSGameNumUI[3];
    /// <summary>
    /// 创建玩家彩票数量UI界面.
    /// </summary>
    void CreatCaiPiaoInfoPanel(PlayerEnum indexPlayer, bool isShowCaiPiaoInfo = false)
    {
        if (isShowCaiPiaoInfo == true)
        {
            //强制显示彩票界面.
        }
        else
        {
            if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
            {
                //没有激活任何玩家.
                return;
            }
        }

        int index = (int)indexPlayer - 1;
        if (index < 0 || index > 2)
        {
            UnityLogWarning("CreatCaiPiaoInfoPanel -> index was wrong! index ==== " + index);
            return;
        }

        GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/GUI/CaiPiaoUI/CaiPiaoInfo");
        if (gmDataPrefab != null)
        {
            if (m_CaiPiaoInfoArray[index] == null)
            {
                if (m_PlayerUIParent[index] != null)
                {
                    UnityLog("CreatCaiPiaoInfoPanel...");
                    GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_PlayerUIParent[index]);
                    m_CaiPiaoInfoArray[index] = obj.GetComponent<SSGameNumUI>();
                    SetActiveZhengZaiChuPiaoUI(indexPlayer, true);

                    SSCaiPiaoInfo caiPiaoInfoCom = obj.GetComponent<SSCaiPiaoInfo>();
                    if (caiPiaoInfoCom != null)
                    {
                        caiPiaoInfoCom.Init(indexPlayer);
                    }
                }
                else
                {
                    UnityLogWarning("CreatCaiPiaoInfoPanel -> m_PlayerUIParent was wrong! index ==== " + index);
                }
            }
        }
        else
        {
            UnityLogWarning("CreatCaiPiaoInfoPanel -> gmDataPrefab was null");
        }
    }

    /// <summary>
    /// 删除玩家彩票数量UI.
    /// </summary>
    void RemoveCaiPiaoInfoPanel(PlayerEnum indexPlayer)
    {
        int index = (int)indexPlayer - 1;
        if (index < 0 || index > 2)
        {
            UnityLogWarning("RemoveCaiPiaoInfoPanel -> index was wrong! index ==== " + index);
            return;
        }

        if (m_CaiPiaoInfoArray[index] != null)
        {
            UnityLog("RemoveCaiPiaoInfoPanel -> index ==== " + index);
            RemovePlayerCaiPiaoChengJiu(indexPlayer);
            //Destroy(m_CaiPiaoInfoArray[index].gameObject);

            SSCaiPiaoInfo caiPiaoInfoCom = m_CaiPiaoInfoArray[index].GetComponent<SSCaiPiaoInfo>();
            if (caiPiaoInfoCom != null)
            {
                caiPiaoInfoCom.RemoveSelf(indexPlayer);
            }
        }
    }

    /// <summary>
    /// 初始化彩票数字动画播放逻辑.
    /// </summary>
    public void InitCaiPiaoAnimation(float timeVal, PlayerEnum indexPlayer)
    {
        if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
        {
            //没有激活任何玩家.
            return;
        }

        int indexVal = (int)indexPlayer - 1;
        if (m_CaiPiaoInfoArray[indexVal] != null)
        {
            SSCaiPiaoInfo caiPiaoInfo = m_CaiPiaoInfoArray[indexVal].GetComponent<SSCaiPiaoInfo>();
            if (caiPiaoInfo != null)
            {
                caiPiaoInfo.InitCaiPiaoAnimation(timeVal, indexPlayer);
            }
            else
            {
                UnityLogWarning("InitCaiPiaoAnimation -> caiPiaoInfo was null............");
            }
        }
    }

    void SetActiveZhengZaiChuPiaoUI(PlayerEnum indexPlayer, bool isActive)
    {
        //if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
        //{
        //    //没有激活任何玩家.
        //    return;
        //}

        int index = (int)indexPlayer - 1;
        if (index < 0 || index > 2)
        {
            UnityLogWarning("SetActiveZhengZaiChuPiaoUI -> index was wrong! index ==== " + index);
            return;
        }

        if (m_CaiPiaoInfoArray[index] != null)
        {
            SSCaiPiaoInfo caiPiaoInfo = m_CaiPiaoInfoArray[index].GetComponent<SSCaiPiaoInfo>();
            if (caiPiaoInfo != null)
            {
                caiPiaoInfo.SetActiveZhengZaiChuPiao(isActive);
            }
            else
            {
                UnityLogWarning("SetActiveZhengZaiChuPiaoUI -> caiPiaoInfo was null!");
            }
        }
    }

    /// <summary>
    /// 是否删除玩家彩票UI.
    /// </summary>
    bool[] IsRemoveCaiPiaoInfo = new bool[3];
    public void ShowPlayerCaiPiaoInfo(PlayerEnum indexPlayer, int num, bool isPlayCaiPiaoNumAni = false, bool isShowCaiPiaoInfo = false)
    {
        int index = (int)indexPlayer - 1;
        if (index < 0 || index > 2)
        {
            UnityLogWarning("ShowPlayerCaiPiaoInfo -> index was wrong! index ==== " + index);
            return;
        }

        if (num <= 0)
        {
            //删除玩家彩票UI信息.
            if (IsRemoveCaiPiaoInfo[index] == false)
            {
                IsRemoveCaiPiaoInfo[index] = true;
                StartCoroutine(DelayRemoveCaiPiaoInfoPanle(indexPlayer));
            }

            if (m_CaiPiaoInfoArray[index] != null)
            {
                //显示彩票数量UI.
                m_CaiPiaoInfoArray[index].ShowNumUI(num);
            }
        }
        else
        {
            if (IsRemoveCaiPiaoInfo[index] == true)
            {
                //重置信息.
                IsRemoveCaiPiaoInfo[index] = false;
            }

            if (m_CaiPiaoInfoArray[index] == null)
            {
                //创建彩票数据信息.
                CreatCaiPiaoInfoPanel(indexPlayer, isShowCaiPiaoInfo);
            }

            if (m_CaiPiaoInfoArray[index] != null)
            {
                //显示彩票数量UI.
                m_CaiPiaoInfoArray[index].ShowNumUI(num);

                if (isPlayCaiPiaoNumAni == true)
                {
                    //播放彩票数字缩放动画.
                    SSCaiPiaoInfo caiPiaoInfoCom = m_CaiPiaoInfoArray[index].GetComponent<SSCaiPiaoInfo>();
                    if (caiPiaoInfoCom != null)
                    {
                        caiPiaoInfoCom.PlayCaiPiaoNumSuoFangAnimation();
                    }
                }

                if (m_CaiPiaoBuZuArray[index] != null)
                {
                    //彩票不足UI有显示.
                    SetActiveZhengZaiChuPiaoUI(indexPlayer, false);
                }
            }
        }
    }

    /// <summary>
    /// 延迟删除彩票数据信息.
    /// </summary>
    IEnumerator DelayRemoveCaiPiaoInfoPanle(PlayerEnum indexPlayer)
    {
        yield return new WaitForSeconds(1f);
        int index = (int)indexPlayer - 1;
        if (index < 0 || index > 2)
        {
            UnityLogWarning("DelayRemoveCaiPiaoInfoPanle -> index was wrong! index ==== " + index);
            yield break;
        }

        if (IsRemoveCaiPiaoInfo[index] == true)
        {
            RemoveCaiPiaoInfoPanel(indexPlayer);
        }
    }

    /// <summary>
    /// 彩票大奖UI控制组件.
    /// </summary>
    internal SSCaiPiaoDaJiang m_CaiPiaoDaJiang;
    /// <summary>
    /// 玩家获得JPBoss大奖时创建该界面.
    /// 创建彩票大奖UI界面.
    /// </summary>
    public void CreatCaiPiaoDaJiangPanel(PlayerEnum indexPlayer, int num)
    {
        if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
        {
            //没有激活任何玩家.
            return;
        }

        int index = (int)indexPlayer - 1;
        if (index >= 0 && index <= 2)
        {
            if (m_CaiPiaoDaJiang == null)
            {
                GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/GUI/CaiPiaoUI/CaiPiaoDaJiang");
                if (gmDataPrefab != null)
                {
                    if (m_GameUICenter != null)
                    {
                        UnityLog("CreateCaiPiaoDaJiangPanel...");
                        GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_GameUICenter);
                        m_CaiPiaoDaJiang = obj.GetComponent<SSCaiPiaoDaJiang>();
                        if (m_CaiPiaoDaJiang != null)
                        {
                            m_CaiPiaoDaJiang.ShowDaJiangCaiPiaoNum(indexPlayer, num);
                        }
                    }
                    else
                    {
                        UnityLogWarning("CreateCaiPiaoDaJiangPanel -> m_GameUICenter was null!");
                    }
                }
            }
            else
            {
                if (m_CaiPiaoDaJiang != null)
                {
                    m_CaiPiaoDaJiang.ShowDaJiangCaiPiaoNum(indexPlayer, num);
                }
            }
        }
        else
        {
            UnityLogWarning("CreateCaiPiaoDaJiangPanel -> index was wrong! index ======= " + index);
        }
    }

    /// <summary>
    /// 玩家彩票出完后删除该界面.
    /// 删除彩票大奖UI界面.
    /// </summary>
    public void RemoveCaiPiaoDaJiangPanel()
    {
        if (m_CaiPiaoDaJiang != null)
        {
            UnityLog("RemoveCaiPiaoDaJiangPanel...");
            Destroy(m_CaiPiaoDaJiang.gameObject);
        }
    }

    /// <summary>
    /// 彩票成就控制组件列表.
    /// </summary>
    SSPlayerCaiPiaoChengJiu[] m_PlayerCaiPiaoChengJiuArray = new SSPlayerCaiPiaoChengJiu[3];
    /// <summary>
    /// 创建玩家基础彩票成就UI.
    /// </summary>
    public void CreatePlayerCaiPiaoChengJiu(PlayerEnum indexPlayer, int caiPiaoNum)
    {
        int indexVal = (int)indexPlayer - 1;
        if (indexVal < 0 || indexVal > 2)
        {
            UnityLogWarning("CreatePlayerCaiPiaoChengJiu -> index was wrong! indexVal ==== " + indexVal);
            return;
        }

        if (m_PlayerCaiPiaoChengJiuArray[indexVal] == null)
        {
            GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/GUI/CaiPiaoUI/PlayerCaiPiaoChengJiu");
            if (gmDataPrefab != null)
            {
                UnityLog("CreatePlayerCaiPiaoChengJiu -> indexVal ==== " + indexVal);
                GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_PlayerUIParent[indexVal]);
                m_PlayerCaiPiaoChengJiuArray[indexVal] = obj.GetComponent<SSPlayerCaiPiaoChengJiu>();
                if (m_PlayerCaiPiaoChengJiuArray[indexVal] != null)
                {
                    m_PlayerCaiPiaoChengJiuArray[indexVal].Init(indexPlayer, XkGameCtrl.PlayerJiFenArray[indexVal], caiPiaoNum);
                }
            }
        }
    }
    
    /// <summary>
    /// 删除玩家基础彩票成就UI.
    /// </summary>
    public void RemovePlayerCaiPiaoChengJiu(PlayerEnum indexPlayer)
    {
        int indexVal = (int)indexPlayer - 1;
        if (indexVal < 0 || indexVal > 2)
        {
            UnityLogWarning("CreatePlayerCaiPiaoChengJiu -> index was wrong! indexVal ==== " + indexVal);
            return;
        }

        if (m_PlayerCaiPiaoChengJiuArray[indexVal] != null)
        {
            m_PlayerCaiPiaoChengJiuArray[indexVal].RemoveSelf();
            m_PlayerCaiPiaoChengJiuArray[indexVal] = null;
        }
    }
    internal SSCaiPiaoYanHua m_SSCaiPiaoYanHua;
}