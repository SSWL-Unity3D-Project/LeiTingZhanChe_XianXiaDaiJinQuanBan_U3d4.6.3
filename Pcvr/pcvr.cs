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
    static public bool bIsHardWare = true;
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
    
    public enum LedState
    {
        Liang,
        Shan,
        Mie
    }

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
}