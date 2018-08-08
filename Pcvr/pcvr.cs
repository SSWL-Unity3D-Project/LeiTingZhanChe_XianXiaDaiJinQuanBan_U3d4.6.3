using UnityEngine;

public class pcvr : MonoBehaviour
{
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
    //[HideInInspector]
    //public pcvrTXManage mPcvrTXManage;
    static private pcvr Instance = null;
    static public pcvr GetInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("_PCVR");
            DontDestroyOnLoad(obj);
            Instance = obj.AddComponent<pcvr>();
            //Instance.mPcvrTXManage = obj.AddComponent<pcvrTXManage>();
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
    internal void StartPrintPlayerCaiPiao(PlayerEnum index, int caiPiao)
    {
    }

    /// <summary>
    /// 缺票后重新开始打印彩票.
    /// </summary>
    internal void RestartPrintCaiPiao(PlayerEnum indexPlayer)
    {
    }

    /// <summary>
    /// 工作人员清理彩票不足机台的彩票数据信息.
    /// </summary>
    internal void ClearCaiPiaoData(PlayerEnum indexPlayer)
    {
    }
    #endregion

    public void SubPlayerCoin(PlayerEnum indexPlayer, int subNum)
    {
        if (!bIsHardWare)
        {
            return;
        }

        //switch (indexPlayer)
        //{
        //    case PlayerEnum.PlayerOne:
        //        if (CoinNumCurrentP1 < subNum)
        //        {
        //            return;
        //        }
        //        CoinNumCurrentP1 -= subNum;
        //        break;
        //    case PlayerEnum.PlayerTwo:
        //        if (CoinNumCurrentP2 < subNum)
        //        {
        //            return;
        //        }
        //        CoinNumCurrentP2 -= subNum;
        //        break;
        //    case PlayerEnum.PlayerThree:
        //        if (CoinNumCurrentP3 < subNum)
        //        {
        //            return;
        //        }
        //        CoinNumCurrentP3 -= subNum;
        //        break;
        //    case PlayerEnum.PlayerFour:
        //        if (CoinNumCurrentP4 < subNum)
        //        {
        //            return;
        //        }
        //        CoinNumCurrentP4 -= subNum;
        //        break;
        //}
    }
}

public enum LedState
{
	Liang,
	Shan,
	Mie
}