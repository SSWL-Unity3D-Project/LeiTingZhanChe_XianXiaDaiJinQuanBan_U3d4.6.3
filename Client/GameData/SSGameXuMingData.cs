using UnityEngine;

public class SSGameXuMingData
{
    public class XuMingData
    {
        /// <summary>
        /// 续命次数.
        /// </summary>
        int xuMingCount = 0;
        /// <summary>
        /// 最大续命次数.
        /// </summary>
        int maxXuMingNum = 10;
        //int maxXuMingNum = 1; //test
        /// <summary>
        /// 增加续命次数.
        /// </summary>
        internal void AddXuMingCount()
        {
            xuMingCount++;
        }

        /// <summary>
        /// 重置续命次数.
        /// </summary>
        internal void ResetXuMingCount()
        {
            xuMingCount = 0;
        }

        /// <summary>
        /// 获取续命次数.
        /// </summary>
        internal bool GetIsCanXuMing()
        {
            //SSDebug.LogWarning("GetIsCanXuMing -> xuMingCount == " + xuMingCount + ", maxXuMingNum == " + maxXuMingNum);
            return xuMingCount < maxXuMingNum ? true : false;
        }
    }
    /// <summary>
    /// 玩家续命数据列表
    /// </summary>
    public XuMingData[] m_XuMingDtArray;

    /// <summary>
    /// 初始化.
    /// </summary>
    internal void Init()
    {
        m_XuMingDtArray = new XuMingData[3];
        for (int i = 0; i < m_XuMingDtArray.Length; i++)
        {
            m_XuMingDtArray[i] = new XuMingData();
        }
    }

    /// <summary>
    /// 获取是否可以续命.
    /// </summary>
    internal bool GetIsCanXuMing(PlayerEnum indexPlayer)
    {
        int indexVal = (int)indexPlayer - 1;
        if (indexVal >= 0 && indexVal < m_XuMingDtArray.Length)
        {
            if (m_XuMingDtArray[indexVal] != null)
            {
                //SSDebug.LogWarning("GetIsCanXuMing -> indexPlayer == " + indexPlayer
                //    + ", isCanXuMing == " + m_XuMingDtArray[indexVal].GetIsCanXuMing());
                return m_XuMingDtArray[indexVal].GetIsCanXuMing();
            }
        }
        return false;
    }

    /// <summary>
    /// 添加续命次数.
    /// </summary>
    internal void AddXuMingCount(PlayerEnum indexPlayer)
    {
        int indexVal = (int)indexPlayer - 1;
        if (indexVal >= 0 && indexVal < m_XuMingDtArray.Length)
        {
            if (m_XuMingDtArray[indexVal] != null)
            {
                //SSDebug.LogWarning("AddXuMingCount -> indexPlayer == " + indexPlayer);
                m_XuMingDtArray[indexVal].AddXuMingCount();
            }
        }
    }
    
    /// <summary>
    /// 重置续命次数.
    /// </summary>
    internal void ResetXuMingCount(PlayerEnum indexPlayer)
    {
        int indexVal = (int)indexPlayer - 1;
        if (indexVal >= 0 && indexVal < m_XuMingDtArray.Length)
        {
            if (m_XuMingDtArray[indexVal] != null)
            {
                //SSDebug.LogWarning("ResetXuMingCount -> indexPlayer == " + indexPlayer);
                m_XuMingDtArray[indexVal].ResetXuMingCount();
            }
        }
    }
}
