using UnityEngine;

public class pcvr : MonoBehaviour
{
    public enum ButtonState : int
    {
        UP = 1,
        DOWN = -1
    }
    /// <summary>
    /// 是否是硬件版.
    /// </summary>
    static public bool bIsHardWare = false;
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
        }
        return Instance;
    }

    void FixedUpdate()
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
        }
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
                        indexValLed = i + 4;
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

                int indexValLed = indexVal + 3;
                mPcvrTXManage.LedState[indexValLed] = false;
            }
        }
    }
    #endregion
}