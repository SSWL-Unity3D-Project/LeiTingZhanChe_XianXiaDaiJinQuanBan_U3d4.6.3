//#define TEST_SHOW_PLAYER_CAIPIAO
using UnityEngine;
using System.Collections;
using Assets.XKGame.Script.HongDDGamePad;

public class pcvr : MonoBehaviour
{
    /// <summary>
    /// 是否是硬件版.
    /// </summary>
    static public bool bIsHardWare = false;
	
    #region 红点点微信虚拟游戏手柄控制单元
    /// <summary>
    /// 是否为红点点微信手柄操作模式.
    /// </summary>
    public static bool IsHongDDShouBing = true;

    /// <summary>
    /// 红点点微信虚拟游戏手柄控制接口.
    /// </summary>
    internal HongDDGamePadInterface m_HongDDGamePadInterface;

    /// <summary>
    /// 创建红点点微信虚拟游戏手柄消息接口组件.
    /// </summary>
    internal void CreatHongDDGanePadInterface()
    {
        if (m_HongDDGamePadInterface == null)
        {
            m_HongDDGamePadInterface = gameObject.AddComponent<HongDDGamePadInterface>();
            m_HongDDGamePadInterface.CreatHongDDGanePad();
        }
    }
    #endregion
	
    public enum ButtonState : int
    {
        UP = 1,
        DOWN = -1
    }
    /// <summary>
    /// 是否校验hid.
    /// </summary>
	static public bool IsJiaoYanHid = false;
    /// <summary>
    /// pcvr通信数据管理.
    /// </summary>
    [HideInInspector]
    public pcvrTXManage mPcvrTXManage;
    static private pcvr Instance = null;
    static public pcvr GetInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("_PCVR");
            DontDestroyOnLoad(obj);
            Instance = obj.AddComponent<pcvr>();
            Instance.mPcvrTXManage = obj.AddComponent<pcvrTXManage>();
            if (bIsHardWare)
            {
                MyCOMDevice.GetInstance();
            }
            //Instance.CreatHongDDGanePadInterface();
        }
        return Instance;
    }

    void FixedUpdateTest()
    {
        UpdateStartLedState();
    }

    #region pcvr 游戏彩票管理
    /// <summary>
    /// 开始打印彩票.
    /// </summary>
    internal void StartPrintPlayerCaiPiao(PlayerEnum indexPlayer, int caiPiao)
    {
        if (bIsHardWare && mPcvrTXManage != null)
        {
            int indexVal = (int)indexPlayer;
            if (indexVal < 1 || indexVal > 3)
            {
                Debug.LogWarning("StartPrintPlayerCaiPiao -> indexVal was wrong! indexVal ==== " + indexVal);
                return;
            }

            if (IsOpenDelayPrintPlayerCaiPiao[indexVal - 1] == false)
            {
                pcvrTXManage.CaiPiaoJi indexCaiPiaoJi = (pcvrTXManage.CaiPiaoJi)(indexVal - 1);
                if (mPcvrTXManage.GetIsCanPrintCaiPiao(indexCaiPiaoJi) == true)
                {
                    pcvrTXManage.CaiPiaoPrintCmd cmd = pcvrTXManage.CaiPiaoPrintCmd.BanPiaoPrint;
                    if (XKGlobalData.GetInstance().m_CaiPiaoPrintState == XKGlobalData.CaiPiaoPrintState.QuanPiao)
                    {
                        cmd = pcvrTXManage.CaiPiaoPrintCmd.QuanPiaoPrint;
                    }
                    mPcvrTXManage.SetCaiPiaoPrintCmd(cmd, indexCaiPiaoJi, caiPiao);
                }
                else
                {
                    StartCoroutine(DelayCheckPrintPlayerCaiPiao(indexPlayer, caiPiao));
                }
            }
        }
    }

    /// <summary>
    /// 彩票机打印状态标记.
    /// </summary>
    bool[] IsOpenDelayPrintPlayerCaiPiao = new bool[4];
    /// <summary>
    /// 延迟检测玩家彩票机是否可以打印彩票.
    /// </summary>
    IEnumerator DelayCheckPrintPlayerCaiPiao(PlayerEnum indexPlayer, int caiPiao)
    {
        int indexVal = (int)indexPlayer;
        IsOpenDelayPrintPlayerCaiPiao[indexVal - 1] = true;
        pcvrTXManage.CaiPiaoJi indexCaiPiaoJi = (pcvrTXManage.CaiPiaoJi)(indexVal - 1);
        do
        {
            yield return new WaitForSeconds(0.1f);
            if (mPcvrTXManage.GetIsCanPrintCaiPiao(indexCaiPiaoJi) == true)
            {
                pcvrTXManage.CaiPiaoPrintCmd cmd = pcvrTXManage.CaiPiaoPrintCmd.BanPiaoPrint;
                if (XKGlobalData.GetInstance().m_CaiPiaoPrintState == XKGlobalData.CaiPiaoPrintState.QuanPiao)
                {
                    cmd = pcvrTXManage.CaiPiaoPrintCmd.QuanPiaoPrint;
                }
                mPcvrTXManage.SetCaiPiaoPrintCmd(cmd, indexCaiPiaoJi, caiPiao);
                break;
            }
        }
        while (true);
        IsOpenDelayPrintPlayerCaiPiao[indexVal - 1] = false;
    }

    /// <summary>
    /// 缺票后重新开始打印彩票.
    /// </summary>
    internal void RestartPrintCaiPiao(PlayerEnum indexPlayer)
    {
        if (bIsHardWare && mPcvrTXManage != null)
        {
            int indexVal = (int)indexPlayer;
            if (indexVal < 1 || indexVal > 3)
            {
                Debug.LogWarning("StartPrintPlayerCaiPiao -> indexVal was wrong! indexVal ==== " + indexVal);
                return;
            }

            pcvrTXManage.CaiPiaoJi indexCaiPiaoJi = (pcvrTXManage.CaiPiaoJi)(indexVal - 1);
            if (mPcvrTXManage.GetIsCanPrintCaiPiao(indexCaiPiaoJi) == true)
            {
                pcvrTXManage.CaiPiaoPrintCmd cmd = pcvrTXManage.CaiPiaoPrintCmd.BanPiaoPrint;
                if (XKGlobalData.GetInstance().m_CaiPiaoPrintState == XKGlobalData.CaiPiaoPrintState.QuanPiao)
                {
                    cmd = pcvrTXManage.CaiPiaoPrintCmd.QuanPiaoPrint;
                }
                int caiPiao = XkPlayerCtrl.GetInstanceFeiJi().m_SpawnNpcManage.m_CaiPiaoDataManage.GetPlayerCaiPiaoVal(indexPlayer);
                mPcvrTXManage.SetCaiPiaoPrintCmd(cmd, indexCaiPiaoJi, caiPiao);
            }
        }
    }

    /// <summary>
    /// 工作人员清理彩票不足机台的彩票数据信息.
    /// </summary>
    internal void ClearCaiPiaoData(PlayerEnum indexPlayer)
    {
        if (bIsHardWare && mPcvrTXManage != null)
        {
            int indexVal = (int)indexPlayer;
            if (indexVal < 1 || indexVal > 3)
            {
                Debug.LogWarning("StartPrintPlayerCaiPiao -> indexVal was wrong! indexVal ==== " + indexVal);
                return;
            }

            pcvrTXManage.CaiPiaoJi indexCaiPiaoJi = (pcvrTXManage.CaiPiaoJi)(indexVal - 1);
            if (mPcvrTXManage.GetIsCanPrintCaiPiao(indexCaiPiaoJi) == true)
            {
                mPcvrTXManage.ClearCaiPiaoJiData(indexCaiPiaoJi);
            }
        }
    }
    #endregion

    #region 减币操作
    public void SubPlayerCoin(PlayerEnum indexPlayer, int subNum)
    {
        if (!bIsHardWare)
        {
            return;
        }

        int indexVal = (int)indexPlayer;
        if (indexVal < 1 || indexVal > 3)
        {
            Debug.LogWarning("SubPlayerCoin -> indexVal was wrong! indexVal ===== " + indexVal);
            return;
        }
        pcvrTXManage.PlayerCoinEnum indexPlayerCoin = (pcvrTXManage.PlayerCoinEnum)(indexVal - 1);

        if (mPcvrTXManage != null)
        {
            mPcvrTXManage.SubPlayerCoin(subNum, indexPlayerCoin);
        }
    }
    #endregion

    #region Start Led
    float m_LastStartLedTime = 0f;
    /// <summary>
    /// 开始灯.
    /// </summary>
    bool[] LedStart = new bool[4];
    /// <summary>
    /// 是否打开开始灯.
    /// </summary>
    bool IsOpenLedStart = false;
    /// <summary>
    /// 打开玩家开始灯.
    /// </summary>
    public void OpenPlayerStartLed(PlayerEnum indexPlayer)
    {
        if (!bIsHardWare)
        {
            return;
        }

        int indexVal = (int)indexPlayer;
        if (indexVal < 1 || indexVal > 3)
        {
            Debug.LogWarning("OpenPlayerStartLed -> indexVal was wrong! indexVal ===== " + indexVal);
            return;
        }

        /** *****************************************************************************************
         开始灯1 ---------- 4
         开始灯2 ---------- 5
         开始灯3 ---------- 6
         ***************************************************************************************** */
        if (mPcvrTXManage != null)
        {
            if (LedStart[indexVal - 1] == false)
            {
                LedStart[indexVal - 1] = true;

                int indexValLed = indexVal + 2;
                mPcvrTXManage.LedState[indexValLed] = true;
            }
        }
    }

    /// <summary>
    /// 更新开始灯状态.
    /// </summary>
    void UpdateStartLedState()
    {
        if (Time.time - m_LastStartLedTime >= 0.25f)
        {
            m_LastStartLedTime = Time.time;
            int length = 3;
            int indexValLed = 0;
            for (int i = 0; i < length; i++)
            {
                if (LedStart[i] == true)
                {
                    if (mPcvrTXManage != null)
                    {
                        indexValLed = i + 3;
                        mPcvrTXManage.LedState[indexValLed] = !IsOpenLedStart;
                    }
                }
            }
            IsOpenLedStart = !IsOpenLedStart;
        }
    }

    /// <summary>
    /// 关闭玩家开始灯.
    /// </summary>
    public void ClosePlayerStartLed(PlayerEnum indexPlayer)
    {
        if (!bIsHardWare)
        {
            return;
        }

        int indexVal = (int)indexPlayer;
        if (indexVal < 1 || indexVal > 3)
        {
            Debug.LogWarning("ClosePlayerStartLed -> indexVal was wrong! indexVal ===== " + indexVal);
            return;
        }

        /** *****************************************************************************************
         开始灯1 ---------- 4
         开始灯2 ---------- 5
         开始灯3 ---------- 6
         ***************************************************************************************** */
        if (mPcvrTXManage != null)
        {
            if (LedStart[indexVal - 1] == true)
            {
                LedStart[indexVal - 1] = false;

                int indexValLed = indexVal + 2;
                mPcvrTXManage.LedState[indexValLed] = false;
            }
        }
    }
    #endregion
    
#if TEST_SHOW_PLAYER_CAIPIAO
    private void OnGUI()
    {
        string info = "";
        Rect rectVal = new Rect(15f, 15f, Screen.width - 30f, 25f);
        
        info = "CaiPiJiP1: ";
        byte[] buffer = MyCOMDevice.ComThreadClass.ReadByteMsg;
        //UpdateCaiPiaoJiInfo(buffer[44], buffer[15], buffer[16]);
        pcvrTXManage.CaiPiaoPrintState type = (pcvrTXManage.CaiPiaoPrintState)buffer[44];
        switch (type)
        {
            case pcvrTXManage.CaiPiaoPrintState.WuXiao:
                {
                    info += "无效";
                    break;
                }
            case pcvrTXManage.CaiPiaoPrintState.Failed:
                {
                    info += "失败";
                    break;
                }
            case pcvrTXManage.CaiPiaoPrintState.Succeed:
                {
                    info += "成功";
                    break;
                }
        }
        
        info += ", CaiPiJiP2: ";
        type = (pcvrTXManage.CaiPiaoPrintState)buffer[15];
        switch (type)
        {
            case pcvrTXManage.CaiPiaoPrintState.WuXiao:
                {
                    info += "无效";
                    break;
                }
            case pcvrTXManage.CaiPiaoPrintState.Failed:
                {
                    info += "失败";
                    break;
                }
            case pcvrTXManage.CaiPiaoPrintState.Succeed:
                {
                    info += "成功";
                    break;
                }
        }

        info += ", CaiPiJiP3: ";
        type = (pcvrTXManage.CaiPiaoPrintState)buffer[16];
        switch (type)
        {
            case pcvrTXManage.CaiPiaoPrintState.WuXiao:
                {
                    info += "无效";
                    break;
                }
            case pcvrTXManage.CaiPiaoPrintState.Failed:
                {
                    info += "失败";
                    break;
                }
            case pcvrTXManage.CaiPiaoPrintState.Succeed:
                {
                    info += "成功";
                    break;
                }
        }
        rectVal = new Rect(15f, 45f, Screen.width - 30f, 25f);
        GUI.Box(rectVal, "");
        GUI.Label(rectVal, info);

        info = "PcvrCaiPiaoP1: " + mPcvrTXManage.CaiPiaoCountPrint[0].ToString()
            + ", PcvrCaiPiaoP2: " + mPcvrTXManage.CaiPiaoCountPrint[1].ToString()
            + ", PcvrCaiPiaoP3: " + mPcvrTXManage.CaiPiaoCountPrint[2].ToString();
        rectVal = new Rect(15f, 75f, Screen.width - 30f, 25f);
        GUI.Box(rectVal, "");
        GUI.Label(rectVal, info);

        info = "PcvrCaiPiaoPrintFailedP1: " + mPcvrTXManage.CaiPiaoPrintFailedCount[0].ToString()
            + ", PcvrCaiPiaoPrintFailedP2: " + mPcvrTXManage.CaiPiaoPrintFailedCount[1].ToString()
            + ", PcvrCaiPiaoPrintFailedP3: " + mPcvrTXManage.CaiPiaoPrintFailedCount[2].ToString();
        rectVal = new Rect(15f, 105f, Screen.width - 30f, 25f);
        GUI.Box(rectVal, "");
        GUI.Label(rectVal, info);
    }
#endif
}