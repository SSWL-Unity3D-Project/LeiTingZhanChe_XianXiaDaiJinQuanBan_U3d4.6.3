using UnityEngine;

public class SSPingJiData : MonoBehaviour
{
    /// <summary>
    /// 评级分数阀值.
    /// </summary>
    public int[] m_PingJiFenShuArray = new int[7] { 0, 100, 200, 300, 400, 500, 600 };
    public enum PingJiLevel
    {
        D = 0,
        C = 1,
        B = 2,
        A = 3,
        S = 4,
        SS = 5,
        SSS = 6,
    }
    /// <summary>
    /// 评级达到多少可以进入抽奖.
    /// </summary>
    public PingJiLevel m_ChouJiangPingJi = PingJiLevel.A;

    private void Awake()
    {
        if (XkGameCtrl.GetInstance() != null)
        {
            XkGameCtrl.GetInstance().m_PingJiData = this;
        }
    }

    /// <summary>
    /// 获取游戏抽奖最低分数.
    /// </summary>
    internal int GetChouJiangMinScore()
    {
        int score = 40000;
        int indexVal = (int)m_ChouJiangPingJi;
        if (indexVal >= 0 && indexVal < m_PingJiFenShuArray.Length)
        {
            score = m_PingJiFenShuArray[indexVal];
        }
        return score;
    }

    /// <summary>
    /// 更新评级分数信息.
    /// </summary>
    internal void UpdataPingJiFenShuInfo(int[] pingJiFenShuArray)
    {
        m_PingJiFenShuArray = pingJiFenShuArray;
        if (XkGameCtrl.GetInstance() != null)
        {
            int chouJiangFenShu = GetChouJiangMinScore();
            XkGameCtrl.GetInstance().ShowChouJiangFenShu3D(chouJiangFenShu);
        }
    }

    internal PingJiLevel GetPlayerPingJiLevel(int fenShuVal)
    {
        if (m_PingJiFenShuArray.Length <= 0 || fenShuVal < m_PingJiFenShuArray[0])
        {
            //最低评级.
            return PingJiLevel.D;
        }

        int length = (int)PingJiLevel.SSS + 1;
        PingJiLevel pingJiLevel = PingJiLevel.D;
        for (int i = 0; i < length; i++)
        {
            if (i == length - 1)
            {
                //评级分数已经到达最高等级.
                pingJiLevel = (PingJiLevel)i;
                break;
            }
            else
            {
                if (fenShuVal >= m_PingJiFenShuArray[i] && fenShuVal < m_PingJiFenShuArray[i + 1])
                {
                    pingJiLevel = (PingJiLevel)i;
                    break;
                }
            }
        }
        return pingJiLevel;
    }
}
