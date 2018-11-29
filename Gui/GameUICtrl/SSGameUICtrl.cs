using UnityEngine;
using System.Collections;

public class SSGameUICtrl : SSGameMono
{

    /// <summary>
    /// 游戏开始时UI界面中心锚点.
    /// </summary>
    public Transform m_GameStartUICenter;
    /// <summary>
    /// 游戏UI界面中心锚点.
    /// </summary>
    public Transform m_GameUICenter;
    /// <summary>
    /// 游戏UI界面左下锚点.
    /// </summary>
    public Transform m_GameUIBottomLeft;
    /// <summary>
    /// 游戏UI界面右上锚点.
    /// </summary>
    public Transform m_GameUITopRight;
    /// <summary>
    /// 游戏UI界面正上方锚点.
    /// </summary>
    public Transform m_GameUITop;
    /// <summary>
    /// 玩家UI父级tr.
    /// </summary>
    public Transform[] m_PlayerUIParent = new Transform[3];

    void Awake()
    {
        CreatErWeiMaUI();
        if (SSUIRoot.GetInstance() != null)
        {
            SSUIRoot.GetInstance().m_GameUIManage = this;
        }
        CreatNpcPiaoFenUICom();

        //if (pcvr.IsHongDDShouBing == true && System.DateTime.Now.Year < 2018)
        //{
        //    //系统时间错误,请检查系统时间.
        //    CreatFixSystemTimeUI();
        //}

        CreatZhanCheBossCaiPiaoZhuanPan();
        if (XkGameCtrl.GetInstance() != null)
        {
            XkGameCtrl.GameVersion gameVersion = XkGameCtrl.GetInstance().m_GameVersion;
            switch (gameVersion)
            {
                case XkGameCtrl.GameVersion.CeShiBan:
                    {
                        //测试版游戏.
                        CreatCeShiUI();
                        break;
                    }
            }
        }
    }

    private void Start()
    {
        if (pcvr.IsHongDDShouBing)
        {
            //红点点微信二维码游戏.
            if (pcvr.GetInstance().m_HongDDGamePadInterface.GetBoxPostNet() != null)
            {
                SSBoxPostNet.BoxLoginData loginDt = pcvr.GetInstance().m_HongDDGamePadInterface.GetBoxPostNet().m_BoxLoginData;
                if (loginDt != null && loginDt.screenId != "0")
                {
                    string screenId = loginDt.screenId;
                    CreatGameScreenIdUI(System.Convert.ToInt32(screenId));
                }
            }
        }
    }

    /// <summary>
    /// 游戏营收UI界面.
    /// </summary>
    SSGamePayUI m_PayDataPanel = null;
    /// <summary>
    /// 创建游戏营收数据表UI界面.
    /// </summary>
    internal void CreatGamePayDataPanel()
    {
        if (m_GameUICenter == null)
        {
            UnityLogWarning("CreatGamePayDataPanel -> m_GameUICenter was null.........");
            return;
        }

        if (m_PayDataPanel == null)
        {
            GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/GUI/PayDataUI/Panel-PayDataUI");
            if (gmDataPrefab != null)
            {
                GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_GameUICenter);
                m_PayDataPanel = obj.GetComponent<SSGamePayUI>();
                if (m_PayDataPanel != null)
                {
                    m_PayDataPanel.Init();
                }
            }
            else
            {
                UnityLogWarning("CreatGamePayDataPanel -> gmDataPrefab was null");
            }
        }
    }

    /// <summary>
    /// 删除游戏营收数据表UI界面.
    /// </summary>
    internal void RemoveGamePayDataPanel()
    {
        if (m_PayDataPanel != null)
        {
            m_PayDataPanel.RemoveSelf();
        }
    }

    /// <summary>
    /// 战车和JPBoss的彩票转盘控制脚本.
    /// </summary>
    SSCaiPiaoZhanCheBossUI m_CaiPiaoZhanCheBossUI;
    /// <summary>
    /// 创建战车和JPBoss的彩票转盘.
    /// </summary>
    void CreatZhanCheBossCaiPiaoZhuanPan()
    {
        string prefabPath = "Prefabs/GUI/CaiPiaoUI/CaiPiaoZhuanPan";
        GameObject gmDataPrefab = (GameObject)Resources.Load(prefabPath);
        if (gmDataPrefab != null)
        {
            UnityLog("CreatZhanCheBossCaiPiaoZhuanPan -> init...");
            GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_GameUIBottomLeft);
            m_CaiPiaoZhanCheBossUI = obj.GetComponent<SSCaiPiaoZhanCheBossUI>();
            if (m_CaiPiaoZhanCheBossUI != null)
            {
                m_CaiPiaoZhanCheBossUI.SetActive(false);
            }
        }
        else
        {
            UnityLogWarning("CreatZhanCheBossCaiPiaoZhuanPan -> gmDataPrefab was null, prefabPath == " + prefabPath);
        }
    }

    /// <summary>
    /// 创建战车boss彩票转盘.
    /// </summary>
    public void CreatZhanCheBossCaiPiaoZhuanPan(PlayerEnum indexPlayer, int caiPiaoVal, Vector3 pos, SSCaiPiaoDataManage.GameCaiPiaoData.DeCaiState type, GameObject exp)
    {
        if (m_CaiPiaoZhanCheBossUI != null)
        {
            m_CaiPiaoZhanCheBossUI.Init(indexPlayer, caiPiaoVal, pos, type, exp);
        }
        else
        {
            SSDebug.LogWarning("CreatZhanCheBossCaiPiaoZhuanPan -> m_CaiPiaoZhanCheBossUI was null");
        }

        //StartCoroutine(DelayCreatZhanCheBossCaiPiaoZhuanPan(indexPlayer, caiPiaoVal, pos, type, exp));
        
        //if (m_GameUIBottomLeft == null)
        //{
        //    UnityLogWarning("CreatZhanCheBossCaiPiaoZhuanPan -> m_GameUIBottomLeft was null.........");
        //    return;
        //}

        //GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/GUI/CaiPiaoUI/CaiPiaoZhuanPan");
        //if (gmDataPrefab != null)
        //{
        //    //UnityLog("CreatZhanCheBossCaiPiaoZhuanPan...");
        //    GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_GameUIBottomLeft);
        //    SSCaiPiaoZhanCheBossUI caiPiaoZhuanPan = obj.GetComponent<SSCaiPiaoZhanCheBossUI>();
        //    if (caiPiaoZhuanPan != null)
        //    {
        //        caiPiaoZhuanPan.Init(indexPlayer, caiPiaoVal, pos, type, exp);
        //    }
        //}
        //else
        //{
        //    UnityLogWarning("CreatZhanCheBossCaiPiaoZhuanPan -> gmDataPrefab was null");
        //}
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
                    UnityLog("CreatCaiPiaoInfoPanel -> indexPlayer ======== " + indexPlayer);
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
                m_CaiPiaoInfoArray[index] = null;
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
                if (num > 999)
                {
                    num = 999; //彩票数量最高为999.
                }
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
        yield return new WaitForSeconds(0f);
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
    float m_TimeLastDaJiang = 0f;
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
            if (m_CaiPiaoXiaoJiang != null)
            {
                //删除小奖UI.
                m_CaiPiaoXiaoJiang.RemoveSelf();
                m_CaiPiaoXiaoJiang = null;
            }

            if (m_CaiPiaoDaJiang != null)
            {
                //删除大奖UI.
                m_CaiPiaoDaJiang.RemoveSelf();
                m_CaiPiaoDaJiang = null;
            }

            if (m_CaiPiaoDaJiang == null)
            {
                m_TimeLastDaJiang = Time.time;
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
    /// 彩票小奖UI控制组件.
    /// </summary>
    internal SSCaiPiaoDaJiang m_CaiPiaoXiaoJiang;
    /// <summary>
    /// 玩家获得小奖时创建该界面.
    /// 创建彩票小奖UI界面.
    /// </summary>
    public void CreatCaiPiaoXiaoJiangPanel(PlayerEnum indexPlayer, int num)
    {
        if (XkGameCtrl.GetInstance().m_GamePlayerAiData.IsActiveAiPlayer == true)
        {
            //没有激活任何玩家.
            return;
        }

        if (Time.time - m_TimeLastDaJiang <= 13f)
        {
            //有大奖UI在屏幕中闪烁时,不去显示小奖UI.
            return;
        }

        int index = (int)indexPlayer - 1;
        if (index >= 0 && index <= 2)
        {
            if (m_CaiPiaoXiaoJiang != null)
            {
                //删除小奖UI.
                m_CaiPiaoXiaoJiang.RemoveSelf();
                m_CaiPiaoXiaoJiang = null;
            }

            if (m_CaiPiaoXiaoJiang == null)
            {
                GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/GUI/CaiPiaoUI/CaiPiaoXiaoJiang");
                if (gmDataPrefab != null)
                {
                    if (m_GameUICenter != null)
                    {
                        UnityLog("CreatCaiPiaoXiaoJiangPanel...");
                        GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_GameUICenter);
                        m_CaiPiaoXiaoJiang = obj.GetComponent<SSCaiPiaoDaJiang>();
                        if (m_CaiPiaoXiaoJiang != null)
                        {
                            m_CaiPiaoXiaoJiang.ShowDaJiangCaiPiaoNum(indexPlayer, num);
                        }
                    }
                    else
                    {
                        UnityLogWarning("CreatCaiPiaoXiaoJiangPanel -> m_GameUICenter was null!");
                    }
                }
            }
            else
            {
                if (m_CaiPiaoXiaoJiang != null)
                {
                    m_CaiPiaoXiaoJiang.ShowDaJiangCaiPiaoNum(indexPlayer, num);
                }
            }
        }
        else
        {
            UnityLogWarning("CreatCaiPiaoXiaoJiangPanel -> index was wrong! index ======= " + index);
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

    /// <summary>
    /// 是否产生修改系统时间UI.
    /// </summary>
    bool IsCreatFixSystemTime = false;
    /// <summary>
    /// 创建修改系统时间UI提示.
    /// </summary>
    internal void CreatFixSystemTimeUI()
    {
        if (IsCreatFixSystemTime == false)
        {
            IsCreatFixSystemTime = true;
            GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/GUI/FixSystemTime/FixTime");
            if (gmDataPrefab != null)
            {
                Instantiate(gmDataPrefab, m_GameUICenter);
            }
            else
            {
                UnityLogWarning("CreatFixSystemTimeUI -> gmDataPrefab was null!");
            }
        }
    }
    
    /// <summary>
    /// 游戏弹幕控制脚本.
    /// </summary>
    internal DanMuTextUI m_DanMuTextUI = null;
    /// <summary>
    /// 产生游戏弹幕UI界面.
    /// </summary>
    internal void CreatDanMuTextUI()
    {
        Debug.Log("Unity: CreatDanMuTextUI...");
        if (m_DanMuTextUI == null)
        {
            GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/GUI/DanMuTextUI/DanMuTextUI");
            if (gmDataPrefab != null)
            {
                GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_GameUICenter);
                m_DanMuTextUI = obj.GetComponent<DanMuTextUI>();
                m_DanMuTextUI.Init();
            }
            else
            {
                UnityLogWarning("CreatDanMuTextUI -> gmDataPrefab was null!");
            }
        }

        //删除彩票大奖UI界面.
        RemoveCaiPiaoDaJiangPanel();
    }

    internal void RemoveDanMuTextUI()
    {
        Debug.Log("Unity: RemoveDanMuTextUI...");
        if (m_DanMuTextUI != null)
        {
            m_DanMuTextUI.RemoveSelf();
            m_DanMuTextUI = null;
        }
    }

    public enum WangLuoGuZhang
    {
        Null = 0,
        Post = 1,
    }
    /// <summary>
    /// 是否产生网络故障UI界面.
    /// </summary>
    internal GameObject m_WangLuoGuZhangUI;
    /// <summary>
    /// 产生网络故障UI界面.
    /// </summary>
    internal void CreatWangLuoGuZhangUI(WangLuoGuZhang guZhang = WangLuoGuZhang.Null)
    {
        //if (pcvr.GetInstance().m_HongDDGamePadInterface.GetBarcodeCam().m_ErWeuMaImg != null)
        //{
        //    //微信虚拟游戏手柄二维码信息存在时,不去创建网络故障UI.
        //    return;
        //}

        if (m_WangLuoGuZhangUI == null)
        {
            string prefabPath = "Prefabs/GUI/wangLuoGuZhang/WangLuoGuZhang";
            switch (guZhang)
            {
                case WangLuoGuZhang.Post:
                    {
                        //首次出现网络故障.
                        prefabPath = "Prefabs/GUI/wangLuoGuZhang/WangLuoGuZhangFirst";
                        break;
                    }
            }
            GameObject gmDataPrefab = (GameObject)Resources.Load(prefabPath);
            if (gmDataPrefab != null)
            {
                Debug.Log("Unity: CreatWangLuoGuZhangUI................................. guZhang == " + guZhang);
                m_WangLuoGuZhangUI = (GameObject)Instantiate(gmDataPrefab, m_GameUICenter);
                if (guZhang != WangLuoGuZhang.Post)
                {
                    m_WangLuoGuZhangUI.AddComponent<SSWangLuoGuZhang>();
                }
            }
            else
            {
                UnityLogWarning("CreatWangLuoGuZhangUI -> gmDataPrefab was null! prefabPath == " + prefabPath);
            }
        }
    }

    /// <summary>
    /// 删除网络故障UI界面.
    /// </summary>
    internal void RemoveWangLuoGuZhangUI()
    {
        if (m_WangLuoGuZhangUI != null)
        {
            SSWangLuoGuZhang wangLuoGuZhangCom = m_WangLuoGuZhangUI.GetComponent<SSWangLuoGuZhang>();
            if (wangLuoGuZhangCom != null && wangLuoGuZhangCom.IsLoadingReconnectServerScene == true)
            {
                //准备加载网络重连关卡.
                return;
            }
            UnityLog("RemoveWangLuoGuZhangUI...");
            Destroy(m_WangLuoGuZhangUI);
            m_WangLuoGuZhangUI = null;
        }
    }

    GameObject[] m_DaiJinQuanYiCunRuArray = new GameObject[3];
    /// <summary>
    /// 产生"代金券已存入我的卡包中"UI界面.
    /// </summary>
    internal void CreatPlayerDaiJinQuanUI(PlayerEnum indexPlayer)
    {
        if (DaoJiShiCtrl.GetInstance(indexPlayer) != null
            && DaoJiShiCtrl.GetInstance(indexPlayer).IsPlayDaoJishi)
        {
            //倒计时显示时不再展示"代金券放入卡包"UI.
            return;
        }

        Debug.Log("Unity: CreatPlayerDaiJinQuanUI...");
        int index = (int)indexPlayer - 1;
        if (index >= 0 && index <= 2)
        {
            if (m_DaiJinQuanYiCunRuArray[index] == null)
            {
                GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/GUI/DaiJinQuanUI/DaiJinQuanYiCunRu");
                if (gmDataPrefab != null)
                {
                    m_DaiJinQuanYiCunRuArray[index] = (GameObject)Instantiate(gmDataPrefab, m_PlayerUIParent[index]);
                }
                else
                {
                    UnityLogWarning("CreatPlayerDaiJinQuanUI -> gmDataPrefab was null!");
                }
            }
        }
    }

    /// <summary>
    /// 删除"代金券已存入我的卡包中"UI界面.
    /// </summary>
    internal void RemovePlayerDaiJinQunUI(PlayerEnum indexPlayer)
    {
        Debug.Log("Unity: RemovePlayerDaiJinQunUI...");
        int index = (int)indexPlayer - 1;
        if (index >= 0 && index <= 2)
        {
            if (m_DaiJinQuanYiCunRuArray[index] != null)
            {
                Destroy(m_DaiJinQuanYiCunRuArray[index]);
                m_DaiJinQuanYiCunRuArray[index] = null;
            }
        }
    }

    internal bool IsCreatGameScreenIdUI = false;
    /// <summary>
    /// 产生游戏红点点屏幕码UI.
    /// </summary>
    internal void CreatGameScreenIdUI(int screenId)
    {
        if (IsCreatGameScreenIdUI == true)
        {
            return;
        }
        IsCreatGameScreenIdUI = true;

        UnityLog("CreatGameScreenIdUI...");
        GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/GUI/ScreenIdUI/ScreenId");
        if (gmDataPrefab != null)
        {
            GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_GameUITopRight);
            SSGameHddSreenId screenIdCom = obj.GetComponent<SSGameHddSreenId>();
            if (screenIdCom != null)
            {
                screenIdCom.Init(screenId);
            }
        }
        else
        {
            UnityLogWarning("CreatGameScreenIdUI -> gmDataPrefab was null!");
        }
    }

    bool IsCreatCompanyLogo = false;
    /// <summary>
    /// 创建公司Logo.
    /// </summary>
    public void CreateCompanyLogo()
    {
        if (IsCreatCompanyLogo == true)
        {
            return;
        }
        IsCreatCompanyLogo = true;

        GameObject gmDataPrefab = (GameObject)Resources.Load("Prefabs/GUI/Logo/Logo");
        if (gmDataPrefab != null)
        {
            UnityLog("CreateCompanyLogo...");
            Instantiate(gmDataPrefab, m_GameUICenter);
        }
    }

    /// <summary>
    /// Boss来袭UI界面.
    /// </summary>
    internal XKBossLXCtrl m_BossLaiXiUI;
    /// <summary>
    /// 产生Boss来袭UI界面.
    /// </summary>
    internal void CreatBossLaiXiUI()
    {
        if (m_BossLaiXiUI == null)
        {
            string prefabPath = "Prefabs/GUI/BossLaiXi/BossLaiXi";
            GameObject gmDataPrefab = (GameObject)Resources.Load(prefabPath);
            if (gmDataPrefab != null)
            {
                Debug.Log("Unity: CreatBossLaiXiUI......................................................");
                GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_GameUICenter);
                m_BossLaiXiUI = obj.GetComponent<XKBossLXCtrl>();
                m_BossLaiXiUI.StartPlayBossLaiXi();
            }
            else
            {
                UnityLogWarning("CreatBossLaiXiUI -> gmDataPrefab was null! prefabPath == " + prefabPath);
            }
        }
    }

    /// <summary>
    /// 删除Boss来袭UI界面.
    /// </summary>
    internal void RemoveBossLaiXiUI()
    {
        if (m_BossLaiXiUI != null)
        {
            UnityLog("RemoveBossLaiXiUI...");
            m_BossLaiXiUI.RemoveSelf();
            m_BossLaiXiUI = null;
            Resources.UnloadUnusedAssets();
        }
    }
    
    /// <summary>
    /// 游戏测试UI界面.
    /// </summary>
    internal GameObject m_CeShiUI = null;
    /// <summary>
    /// 产生游戏测试UI界面.
    /// </summary>
    internal void CreatCeShiUI()
    {
        if (m_CeShiUI == null)
        {
            string prefabPath = "Prefabs/GUI/CeShiUI/CeShiUI";
            GameObject gmDataPrefab = (GameObject)Resources.Load(prefabPath);
            if (gmDataPrefab != null)
            {
                Debug.Log("Unity: CreatCeShiUI......................................................");
                m_CeShiUI = (GameObject)Instantiate(gmDataPrefab, m_GameUICenter);
            }
            else
            {
                UnityLogWarning("CreatCeShiUI -> gmDataPrefab was null! prefabPath == " + prefabPath);
            }
        }
    }

    /// <summary>
    /// 删除游戏测试UI界面.
    /// </summary>
    internal void RemoveCeShiUI()
    {
        if (m_CeShiUI != null)
        {
            UnityLog("RemoveCeShiUI...");
            Destroy(m_CeShiUI);
            m_CeShiUI = null;
            Resources.UnloadUnusedAssets();
        }
    }

    /// <summary>
    /// 游戏总积分界面UI.
    /// </summary>
    internal JiFenJieMianCtrl m_TotalJiFenJieMianUI = null;
    /// <summary>
    /// 产生游戏测试UI界面.
    /// </summary>
    internal void CreatTotalJiFenJieMianUI()
    {
        if (m_TotalJiFenJieMianUI == null)
        {
            string prefabPath = "Prefabs/GUI/JiFenJieMian/JiFenJieMianGroup";
            GameObject gmDataPrefab = (GameObject)Resources.Load(prefabPath);
            if (gmDataPrefab != null)
            {
                Debug.Log("Unity: CreatTotalJiFenJieMianUI......................................................");
                GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_GameUICenter);
                m_TotalJiFenJieMianUI = obj.GetComponent<JiFenJieMianCtrl>();
                m_TotalJiFenJieMianUI.Init();
            }
            else
            {
                UnityLogWarning("CreatTotalJiFenJieMianUI -> gmDataPrefab was null! prefabPath == " + prefabPath);
            }
        }
    }

    /// <summary>
    /// 删除游戏总积分界面UI.
    /// </summary>
    internal void RemoveTotalJiFenJieMianUI()
    {
        if (m_TotalJiFenJieMianUI != null)
        {
            UnityLog("RemoveTotalJiFenJieMianUI...");
            m_TotalJiFenJieMianUI.RemoveSelf();
            m_TotalJiFenJieMianUI = null;
            Resources.UnloadUnusedAssets();
        }
    }
    
    /// <summary>
    /// 悬崖路段提示界面UI.
    /// </summary>
    internal GameObject m_XuanYaLuDuanUI = null;
    /// <summary>
    /// 产生悬崖路段提示UI界面.
    /// </summary>
    internal void CreatXuanYaLuDuanUI()
    {
        if (m_XuanYaLuDuanUI == null)
        {
            string prefabPath = "Prefabs/GUI/XunYaLuDuan/XuanYaTiShi";
            GameObject gmDataPrefab = (GameObject)Resources.Load(prefabPath);
            if (gmDataPrefab != null)
            {
                Debug.Log("Unity: CreatXuanYaLuDuanUI......................................................");
                m_XuanYaLuDuanUI = (GameObject)Instantiate(gmDataPrefab, m_GameUICenter);
            }
            else
            {
                UnityLogWarning("CreatXuanYaLuDuanUI -> gmDataPrefab was null! prefabPath == " + prefabPath);
            }
        }
    }

    /// <summary>
    /// 删除悬崖路段提示界面UI.
    /// </summary>
    internal void RemoveXuanYaLuDuanUI()
    {
        if (m_XuanYaLuDuanUI != null)
        {
            UnityLog("RemoveXuanYaLuDuanUI...");
            Destroy(m_XuanYaLuDuanUI);
            m_XuanYaLuDuanUI = null;
            Resources.UnloadUnusedAssets();
        }
    }

    /// <summary>
    /// 游戏时间界面UI.
    /// </summary>
    internal GameTimeCtrl m_GameTimeUI = null;
    /// <summary>
    /// 产生游戏时间UI界面.
    /// </summary>
    internal void CreatGameTimeUI()
    {
        if (m_GameTimeUI == null)
        {
            string prefabPath = "Prefabs/GUI/GameTimeUI/GameTimeCtrl";
            GameObject gmDataPrefab = (GameObject)Resources.Load(prefabPath);
            if (gmDataPrefab != null)
            {
                Debug.Log("Unity: CreatGameTimeUI......................................................");
                GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_GameUICenter);
                m_GameTimeUI = obj.GetComponent<GameTimeCtrl>();
                m_GameTimeUI.Init();
            }
            else
            {
                UnityLogWarning("CreatGameTimeUI -> gmDataPrefab was null! prefabPath == " + prefabPath);
            }
        }
    }

    /// <summary>
    /// 删除游戏时间界面UI.
    /// </summary>
    internal void RemoveGameTimeUI()
    {
        if (m_GameTimeUI != null)
        {
            UnityLog("RemoveGameTimeUI...");
            m_GameTimeUI.RemoveSelf();
            m_GameTimeUI = null;
            Resources.UnloadUnusedAssets();
        }
    }

    /// <summary>
    /// 游戏总GameOver界面UI.
    /// </summary>
    internal GameOverCtrl m_TotalGameOverUI = null;
    /// <summary>
    /// 产生游戏总GameOver界面UI.
    /// </summary>
    internal void CreatTotalGameOverUI()
    {
        if (m_TotalGameOverUI == null)
        {
            string prefabPath = "Prefabs/GUI/TotalGameOver/GameOverCtrl";
            GameObject gmDataPrefab = (GameObject)Resources.Load(prefabPath);
            if (gmDataPrefab != null)
            {
                Debug.Log("Unity: CreatTotalGameOverUI......................................................");
                GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_GameUICenter);
                m_TotalGameOverUI = obj.GetComponent<GameOverCtrl>();
                m_TotalGameOverUI.Init();
            }
            else
            {
                UnityLogWarning("CreatTotalGameOverUI -> gmDataPrefab was null! prefabPath == " + prefabPath);
            }
        }
    }

    /// <summary>
    /// 删除游戏总GameOver界面UI.
    /// </summary>
    internal void RemoveTotalGameOverUI()
    {
        if (m_TotalGameOverUI != null)
        {
            UnityLog("RemoveTotalGameOverUI...");
            m_TotalGameOverUI.RemoveSelf();
            m_TotalGameOverUI = null;
            Resources.UnloadUnusedAssets();
        }
    }

    /// <summary>
    /// 游戏全部通关界面UI.
    /// </summary>
    internal QuanBuTongGuanCtrl m_QuanBuTongGuanUI = null;
    /// <summary>
    /// 产生游戏全部通关界面UI.
    /// </summary>
    internal void CreatQuanBuTongGuanUI()
    {
        if (m_QuanBuTongGuanUI == null)
        {
            string prefabPath = "Prefabs/GUI/QuanBuTongGuan/QuanBuTongGuanCtrl";
            GameObject gmDataPrefab = (GameObject)Resources.Load(prefabPath);
            if (gmDataPrefab != null)
            {
                Debug.Log("Unity: CreatQuanBuTongGuanUI......................................................");
                GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_GameUICenter);
                m_QuanBuTongGuanUI = obj.GetComponent<QuanBuTongGuanCtrl>();
                m_QuanBuTongGuanUI.Init();
            }
            else
            {
                UnityLogWarning("CreatQuanBuTongGuanUI -> gmDataPrefab was null! prefabPath == " + prefabPath);
            }
        }
    }

    /// <summary>
    /// 删除游戏全部通关界面UI.
    /// </summary>
    internal void RemoveQuanBuTongGuanUI()
    {
        if (m_QuanBuTongGuanUI != null)
        {
            UnityLog("RemoveQuanBuTongGuanUI...");
            m_QuanBuTongGuanUI.RemoveSelf();
            m_QuanBuTongGuanUI = null;
            Resources.UnloadUnusedAssets();
        }
    }
    
    /// <summary>
    /// 游戏阶段控制界面UI.
    /// </summary>
    internal XKGameStageCtrl m_GameStageUI = null;
    /// <summary>
    /// 产生游戏阶段控制界面UI.
    /// </summary>
    internal void CreatGameStageUI()
    {
        if (m_GameStageUI == null)
        {
            string prefabPath = "Prefabs/GUI/GameStageUI/GameStageCtrl";
            GameObject gmDataPrefab = (GameObject)Resources.Load(prefabPath);
            if (gmDataPrefab != null)
            {
                Debug.Log("Unity: CreatGameStageUI......................................................");
                GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_GameUICenter);
                m_GameStageUI = obj.GetComponent<XKGameStageCtrl>();
                m_GameStageUI.Init();
            }
            else
            {
                UnityLogWarning("CreatGameStageUI -> gmDataPrefab was null! prefabPath == " + prefabPath);
            }
        }
    }

    /// <summary>
    /// 删除游戏阶段控制界面UI.
    /// </summary>
    internal void RemoveGameStageUI()
    {
        if (m_GameStageUI != null)
        {
            UnityLog("RemoveGameStageUI...");
            m_GameStageUI.RemoveSelf();
            m_GameStageUI = null;
            Resources.UnloadUnusedAssets();
        }
    }
    
    /// <summary>
    /// 游戏Boss血条界面UI.
    /// </summary>
    internal SSBossXueTiaoManage m_BossXueTiaoUI = null;
    /// <summary>
    /// 产生游戏Boss血条界面UI.
    /// </summary>
    internal void CreatBossXueTiaoUI()
    {
        if (m_BossXueTiaoUI == null)
        {
            string prefabPath = "Prefabs/GUI/BossXueTiaoUI/BossXueTiaoCtrl";
            GameObject gmDataPrefab = (GameObject)Resources.Load(prefabPath);
            if (gmDataPrefab != null)
            {
                Debug.Log("Unity: CreatBossXueTiaoUI......................................................");
                GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_GameUICenter);
                m_BossXueTiaoUI = obj.GetComponent<SSBossXueTiaoManage>();
                m_BossXueTiaoUI.Init();
            }
            else
            {
                UnityLogWarning("CreatBossXueTiaoUI -> gmDataPrefab was null! prefabPath == " + prefabPath);
            }
        }
    }

    /// <summary>
    /// 删除游戏Boss血条界面UI.
    /// </summary>
    internal void RemoveBossXueTiaoUI()
    {
        if (m_BossXueTiaoUI != null)
        {
            UnityLog("RemoveBossXueTiaoUI...");
            m_BossXueTiaoUI.RemoveSelf();
            m_BossXueTiaoUI = null;
            Resources.UnloadUnusedAssets();
        }
    }

    /// <summary>
    /// 游戏Boss血条界面UI.
    /// </summary>
    internal GameStartTimeCtrl m_GameStartTimeUI = null;
    /// <summary>
    /// 产生游戏Boss血条界面UI.
    /// </summary>
    internal void CreatGameStartTimeUI()
    {
        if (m_GameStartTimeUI == null)
        {
            string prefabPath = "Prefabs/GUI/GameStartTimeUI/GameStartTimeCtrl";
            GameObject gmDataPrefab = (GameObject)Resources.Load(prefabPath);
            if (gmDataPrefab != null)
            {
                Debug.Log("Unity: CreatGameStartTimeUI......................................................");
                GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_GameStartUICenter);
                m_GameStartTimeUI = obj.GetComponent<GameStartTimeCtrl>();
                m_GameStartTimeUI.Init();
            }
            else
            {
                UnityLogWarning("CreatGameStartTimeUI -> gmDataPrefab was null! prefabPath == " + prefabPath);
            }
        }
    }

    /// <summary>
    /// 删除游戏Boss血条界面UI.
    /// </summary>
    internal void RemoveGameStartTimeUI()
    {
        if (m_GameStartTimeUI != null)
        {
            UnityLog("RemoveGameStartTimeUI...");
            m_GameStartTimeUI.RemoveSelf();
            m_GameStartTimeUI = null;
            Resources.UnloadUnusedAssets();
        }
    }

    /// <summary>
    /// 产生游戏二维码界面UI.
    /// </summary>
    internal void CreatErWeiMaUI()
    {
        string prefabPath = "Prefabs/GUI/erWeiMa/erWeiMa";
        GameObject gmDataPrefab = (GameObject)Resources.Load(prefabPath);
        if (gmDataPrefab != null)
        {
            Debug.Log("Unity: CreatErWeiMaUI......................................................");
            GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_GameUITopRight);
            ErWeiMaUI erWeiMaCom = obj.GetComponent<ErWeiMaUI>();
            if (erWeiMaCom != null)
            {
                erWeiMaCom.Init(camera);
            }
        }
        else
        {
            UnityLogWarning("CreatErWeiMaUI -> gmDataPrefab was null! prefabPath == " + prefabPath);
        }
    }

    /// <summary>
    /// 游戏代金券npc血条界面UI.
    /// </summary>
    internal SSDaiJinQuanXuanTiao  m_DaiJinQuanNpcXueTiaoUI = null;
    /// <summary>
    /// 产生游戏代金券npc血条界面UI.
    /// </summary>
    internal void CreatDaiJinQuanNpcXueTiaoUI(float maxFillAmount)
    {
        if (m_DaiJinQuanNpcXueTiaoUI == null)
        {
            string prefabPath = "Prefabs/GUI/DaiJinQuanXueTiao/DaiJinQuanXueTiao";
            GameObject gmDataPrefab = (GameObject)Resources.Load(prefabPath);
            if (gmDataPrefab != null)
            {
                Debug.Log("Unity: CreatDaiJinQuanNpcXueTiaoUI......................................................");
                GameObject obj = (GameObject)Instantiate(gmDataPrefab, m_GameUITop);
                m_DaiJinQuanNpcXueTiaoUI = obj.GetComponent<SSDaiJinQuanXuanTiao>();
                m_DaiJinQuanNpcXueTiaoUI.Init(maxFillAmount);

                if (m_CaiPiaoDaJiang != null)
                {
                    //删除大奖UI.
                    m_CaiPiaoDaJiang.RemoveSelf();
                    m_CaiPiaoDaJiang = null;
                }
            }
            else
            {
                UnityLogWarning("CreatDaiJinQuanNpcXueTiaoUI -> gmDataPrefab was null! prefabPath == " + prefabPath);
            }
        }
    }

    /// <summary>
    /// 删除游戏代金券npc血条界面UI.
    /// </summary>
    internal void RemoveDaiJinQuanNpcXueTiaoUI()
    {
        if (m_DaiJinQuanNpcXueTiaoUI != null)
        {
            UnityLog("RemoveDaiJinQuanNpcXueTiaoUI...");
            m_DaiJinQuanNpcXueTiaoUI.RemoveSelf();
            m_DaiJinQuanNpcXueTiaoUI = null;
            Resources.UnloadUnusedAssets();
        }
    }

    /// <summary>
    /// 设置代金券npc的血条amount信息.
    /// </summary>
    internal void SetDaiJinQuanNpcXueTiaoAmount(float bloodAmount)
    {
        if (m_DaiJinQuanNpcXueTiaoUI != null)
        {
            m_DaiJinQuanNpcXueTiaoUI.SetBloodBossAmount(bloodAmount);
        }
    }
}
