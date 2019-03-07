/// <summary>
/// 玩家可以进行免费游戏的数据信息.
/// </summary>
public class SSPlayerMianFeiData
{
    /// <summary>
    /// 玩家免费游戏数据.
    /// </summary>
    public class PlayerMianFeiData
    {
        /// <summary>
        /// 免费续命次数信息.
        /// </summary>
        int mianFeiXuMingNum = 0;
        internal void SetMianFeiNum(int mianFeiNum)
        {
            if (mianFeiNum < 1)
            {
                mianFeiNum = 1;
            }
            mianFeiXuMingNum = mianFeiNum - 1;
        }

        internal void SubMianFeiNum()
        {
            if (mianFeiXuMingNum > 0)
            {
                mianFeiXuMingNum--;
                //SSDebug.LogWarning("SubMianFeiNum -> mianFeiNum ========================================== " + mianFeiXuMingNum);
            }
        }

        /// <summary>
        /// 获取是否可以继续免费进行游戏.
        /// </summary>
        internal bool GetIsCanMianFeiPlayGame()
        {
            bool isCanPlay = false;
            if (mianFeiXuMingNum > 0)
            {
                isCanPlay = true;
                //SubMianFeiNum();
            }
            return isCanPlay;
        }
    }
    /// <summary>
    /// 玩家进行免费游戏的数据.
    /// </summary>
    PlayerMianFeiData[] m_PlayerMianFeiData;

    internal void Init()
    {
        m_PlayerMianFeiData = new PlayerMianFeiData[3];
        for (int i = 0; i < m_PlayerMianFeiData.Length; i++)
        {
            m_PlayerMianFeiData[i] = new PlayerMianFeiData();
        }
    }

    /// <summary>
    /// 当玩家以首次免费形式激活游戏时进入此函数.
    /// 设置玩家免费次数.
    /// </summary>
    internal void SetMianFeiNum(PlayerEnum playerIndex, int mianFeiNum)
    {
        if (m_PlayerMianFeiData == null)
        {
            return;
        }

        int indexVal = (int)playerIndex - 1;
        if (indexVal >= 0 && indexVal < m_PlayerMianFeiData.Length)
        {
            if (m_PlayerMianFeiData[indexVal] != null)
            {
                if (mianFeiNum < 0)
                {
                    mianFeiNum = 0;
                }
                //SSDebug.LogWarning("SetMianFeiNum -> mianFeiNum == " + mianFeiNum + ", playerIndex == " + playerIndex);
                m_PlayerMianFeiData[indexVal].SetMianFeiNum(mianFeiNum);
            }
        }
    }

    /// <summary>
    /// 减少玩家免费次数.
    /// </summary>
    internal void SubMianFeiNum(PlayerEnum playerIndex)
    {
        if (m_PlayerMianFeiData == null)
        {
            return;
        }

        int indexVal = (int)playerIndex - 1;
        if (indexVal >= 0 && indexVal < m_PlayerMianFeiData.Length)
        {
            if (m_PlayerMianFeiData[indexVal] != null)
            {
                //SSDebug.LogWarning("SubMianFeiNum -> playerIndex ========================================== " + playerIndex);
                m_PlayerMianFeiData[indexVal].SubMianFeiNum();
            }
        }
    }

    /// <summary>
    /// 当对玩家进行账户扣费时调用此函数进行判断.
    /// 设置玩家免费次数.
    /// </summary>
    internal bool GetIsCanMianFeiPlayGame(PlayerEnum playerIndex)
    {
        if (m_PlayerMianFeiData == null)
        {
            return false;
        }

        bool isCanPlay = false;
        int indexVal = (int)playerIndex - 1;
        if (indexVal >= 0 && indexVal < m_PlayerMianFeiData.Length)
        {
            if (m_PlayerMianFeiData[indexVal] != null)
            {
                isCanPlay = m_PlayerMianFeiData[indexVal].GetIsCanMianFeiPlayGame();
            }
        }
        //SSDebug.LogWarning("GetIsCanMianFeiPlayGame -> playerIndex == " + playerIndex + ", isCanPlay == " + isCanPlay);
        return isCanPlay;
    }
}
